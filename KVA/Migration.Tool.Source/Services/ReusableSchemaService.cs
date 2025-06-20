using CMS.ContentEngine.Internal;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Source.Mappers;

namespace Migration.Tool.Source.Services;

public class ReusableSchemaService(ILogger<ReusableSchemaService> logger, ToolConfiguration configuration, ModelFacade modelFacade)
{
    private readonly IReusableFieldSchemaManager reusableFieldSchemaManager = Service.Resolve<IReusableFieldSchemaManager>();

    public bool IsConversionToReusableFieldSchemaRequested(string className) => configuration.ClassNamesCreateReusableSchema.Contains(className);

    public DataClassInfo ConvertToReusableSchema(DataClassInfo kxoDataClass, string rfsName, string rfsDisplayName, string? rfsDescription = null, Func<string, string?>? getFieldName = null)
    {
        var reusableSchemaGuid = GuidHelper.CreateReusableSchemaGuid($"{kxoDataClass.ClassName}|{kxoDataClass.ClassGUID}");
        var schema = reusableFieldSchemaManager.Get(reusableSchemaGuid);
        if (schema == null)
        {
            reusableFieldSchemaManager.CreateSchema(new CreateReusableFieldSchemaParameters(rfsName, rfsDisplayName, rfsDescription) { Guid = reusableSchemaGuid });
        }

        var formInfo = new FormInfo(kxoDataClass.ClassFormDefinition);
        var fieldsForRem = new List<FormFieldInfo>();
        var metadataFields = CmsClassMapper.GetLegacyMetadataFields(modelFacade.SelectVersion(),
            CmsClassMapper.IncludedMetadata.Extended);
        foreach (var formFieldInfo in formInfo.GetFields(true, true).Where(ffi => !metadataFields.Any(mf => string.Equals(mf.LegacyFieldName, ffi.Name, StringComparison.InvariantCultureIgnoreCase))))
        {
            if (formFieldInfo is { PrimaryKey: false, System: false })
            {
                bool success = false;
                try
                {
                    try
                    {
                        formFieldInfo.Name = getFieldName?.Invoke(formFieldInfo.Name) ?? GetUniqueFieldName(rfsName, formFieldInfo.Name);
                        reusableFieldSchemaManager.AddField(rfsName, formFieldInfo);
                        success = true;
                    }
                    catch (InvalidOperationException ioex) when (ioex.Message.Contains("already exist on reusable field schema"))
                    {
                        // exists - we do not support update of reusable field schema
                        // _reusableFieldSchemaManager.UpdateField(kxoDataClass.ClassName, formFieldInfo.Name, formFieldInfo);
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to convert field {Name} for reusable schema: {FormFieldInfo}", formFieldInfo.Name, formFieldInfo.ToString());
                }

                if (success)
                {
                    fieldsForRem.Add(formFieldInfo);
                }
            }
        }

        foreach (var formFieldInfo in fieldsForRem)
        {
            formInfo.RemoveFormField(formFieldInfo.Name);
        }

        formInfo.AddFormItem(new FormSchemaInfo { Name = rfsName, Guid = reusableSchemaGuid });
        kxoDataClass.ClassFormDefinition = formInfo.GetXmlDefinition();
        return kxoDataClass;
    }

    public bool HasClassReusableSchema(DataClassInfo dataClassInfo, Guid reusableFieldSchemaGuid)
    {
        var formInfo = new FormInfo(dataClassInfo.ClassFormDefinition);
        return formInfo
            .GetFields<FormSchemaInfo>()
            .Any(x => x.Guid.Equals(reusableFieldSchemaGuid));
    }

    public void AddReusableSchemaToDataClass(DataClassInfo dataClassInfo, Guid reusableFieldSchemaGuid)
    {
        var formInfo = new FormInfo(dataClassInfo.ClassFormDefinition);
        var schema = reusableFieldSchemaManager.Get(reusableFieldSchemaGuid);
        formInfo.AddFormItem(new FormSchemaInfo { Name = schema.Name, Guid = reusableFieldSchemaGuid });
        dataClassInfo.ClassFormDefinition = formInfo.GetXmlDefinition();
    }

    public Guid AddReusableSchemaToDataClass(DataClassInfo dataClassInfo, string reusableFieldSchemaName)
    {
        var formInfo = new FormInfo(dataClassInfo.ClassFormDefinition);
        var schema = reusableFieldSchemaManager.Get(reusableFieldSchemaName);
        if (!formInfo.ItemsList.Any(x => x is FormSchemaInfo fsi && fsi.Guid == schema.Guid))
        {
            formInfo.AddFormItem(new FormSchemaInfo { Name = reusableFieldSchemaName, Guid = schema.Guid });
            dataClassInfo.ClassFormDefinition = formInfo.GetXmlDefinition();
        }
        return schema.Guid;
    }

    public IEnumerable<FormFieldInfo> GetFieldsFromReusableSchema(DataClassInfo dataClassInfo)
    {
        var formInfo = new FormInfo(dataClassInfo.ClassFormDefinition);
        foreach (var formSchemaInfo in formInfo.GetFields<FormSchemaInfo>())
        {
            foreach (var formFieldInfo in reusableFieldSchemaManager.GetSchemaFields(formSchemaInfo.Name))
            {
                yield return formFieldInfo;
            }
        }
    }

    public Guid EnsureReusableFieldSchema(string name, string displayName, string? description, params FormFieldInfo[] fields)
    {
        var reusableSchemaGuid = GuidHelper.CreateReusableSchemaGuid($"{name}");
        var schema = reusableFieldSchemaManager.Get(reusableSchemaGuid);
        if (schema == null)
        {
            reusableFieldSchemaManager.CreateSchema(new CreateReusableFieldSchemaParameters(name, displayName, description) { Guid = reusableSchemaGuid });
        }

        foreach (var formFieldInfo in fields)
        {
            if (formFieldInfo is { PrimaryKey: false, System: false })
            {
                try
                {
                    try
                    {
                        reusableFieldSchemaManager.AddField(name, formFieldInfo);
                    }
                    catch (InvalidOperationException ioex) when (ioex.Message.Contains("already exist on reusable field schema"))
                    {
                        // exists - we do not support update of reusable field schema
                        // _reusableFieldSchemaManager.UpdateField(kxoDataClass.ClassName, formFieldInfo.Name, formFieldInfo);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to convert field {Name} for reusable schema: {FormFieldInfo}", formFieldInfo.Name, formFieldInfo.ToString());
                }
            }
            else
            {
                throw new InvalidOperationException("Unable to add field to reusable schema, field is primary key or system");
            }
        }

        return reusableSchemaGuid;
    }

    public static string GetUniqueFieldName(string className, string fieldName) => $"{className}__{fieldName}".Replace(".", "_");

    public static string RemoveClassPrefix(string className, string schemaFieldName)
        => schemaFieldName.Replace($"{className}__".Replace(".", "_"), "");
}
