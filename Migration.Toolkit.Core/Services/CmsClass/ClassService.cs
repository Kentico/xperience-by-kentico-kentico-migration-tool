using System.Collections.Concurrent;
using CMS.DataEngine;
using CMS.FormEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;

namespace Migration.Toolkit.Core.Services.CmsClass;

public record ClassColumnModel(string ClassName)
{
    public string? ColumnName;
    public Type? ColumnType;
    public CmsFormUserControl? FormControl;
    public string? FormControlCodeName;
}


public class ClassService
{
    private readonly ILogger<ClassService> _logger;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;

    public ClassService(ILogger<ClassService> logger, IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory)
    {
        _logger = logger;
        _kx13ContextFactory = kx13ContextFactory;
    }
    
    private ConcurrentDictionary<string, KX13M.CmsFormUserControl?> _userControlsCache = new(StringComparer.InvariantCultureIgnoreCase);
    public KX13M.CmsFormUserControl? GetFormControlDefinition(string userControlCodeName)
    {
        var kx13Context = _kx13ContextFactory.CreateDbContext();
        return _userControlsCache.GetOrAdd(userControlCodeName, s =>
        {
            return kx13Context.CmsFormUserControls.FirstOrDefault(x => x.UserControlCodeName == userControlCodeName);
        });
    }

    
    
    public IEnumerable<ClassColumnModel> GetClassFields(Guid classGuid)
    {
        var kx13Context = _kx13ContextFactory.CreateDbContext();

        var kx13Class = kx13Context.CmsClasses.FirstOrDefault(c => c.ClassGuid == classGuid);

        if (kx13Class != null)
        {
            var classColumn = new ClassColumnModel(kx13Class.ClassName);
            
            var classStructureInfo = new ClassStructureInfo(kx13Class.ClassName, kx13Class.ClassXmlSchema, kx13Class.ClassTableName);
            var columnDefinitionByColumnName = classStructureInfo.ColumnDefinitions.ToDictionary(k => k.ColumnName);
            var formInfo = new FormInfo(kx13Class.ClassFormDefinition);
            foreach (var columnName in classStructureInfo.ColumnNames.Union(formInfo.GetColumnNames()))
            {
                var field = formInfo.GetFormField(columnName);
                var controlName = field.Settings["controlname"]?.ToString()?.ToLowerInvariant();

                if (columnDefinitionByColumnName.TryGetValue(columnName, out var columnDefinition))
                {
                    classColumn = classColumn with
                    {
                        ColumnName = columnDefinition.ColumnName,
                        ColumnType = columnDefinition.ColumnType,
                    };
                }

                
                if (controlName != null)
                {
                    var formControlDefinition = GetFormControlDefinition(controlName);
                      
                    classColumn = classColumn with
                    {
                        FormControl = formControlDefinition,
                        FormControlCodeName = formControlDefinition?.UserControlCodeName,
                        
                    };
                }
                
                yield return classColumn;
            }
        }
        
        yield break;
    }
}