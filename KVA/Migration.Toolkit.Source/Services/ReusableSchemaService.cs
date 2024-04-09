namespace Migration.Toolkit.Source.Services;

using CMS.ContentEngine.Internal;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using Kentico.Xperience.Admin.Base.UIPages;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Helpers;

public class ReusableSchemaService(ILogger<ReusableSchemaService> logger, ToolkitConfiguration configuration)
{
    private readonly IReusableFieldSchemaManager _reusableFieldSchemaManager = Service.Resolve<IReusableFieldSchemaManager>();

    public bool IsConversionToReusableFieldSchemaRequested(string className)
    {
        return configuration.ClassNamesCreateReusableSchema.Contains(className);
    }

    public DataClassInfo ConvertToReusableSchema(DataClassInfo kxoDataClass)
    {
        var reusableSchemaGuid = GuidHelper.CreateReusableSchemaGuid($"{kxoDataClass.ClassName}|{kxoDataClass.ClassGUID}");
        var schema = _reusableFieldSchemaManager.Get(reusableSchemaGuid);
        if (schema == null)
        {
            _reusableFieldSchemaManager.CreateSchema(new CreateReusableFieldSchemaParameters(kxoDataClass.ClassName, kxoDataClass.ClassDisplayName) { Guid = reusableSchemaGuid });
        }

        var formInfo = new FormInfo(kxoDataClass.ClassFormDefinition);
        var fieldsForRem = new List<FormFieldInfo>();
        foreach (var formFieldInfo in formInfo.GetFields(true, true))
        {
            if (formFieldInfo is { PrimaryKey: false, System: false })
            {
                var success = false;
                try
                {
                    try
                    {
                        formFieldInfo.Name = GetUniqueFieldName(kxoDataClass.ClassName, formFieldInfo.Name);
                        _reusableFieldSchemaManager.AddField(kxoDataClass.ClassName, formFieldInfo);
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

        formInfo.AddFormItem(new FormSchemaInfo { Name = kxoDataClass.ClassName, Guid = reusableSchemaGuid });
        kxoDataClass.ClassFormDefinition = formInfo.GetXmlDefinition();
        return kxoDataClass;
    }

    public static string GetUniqueFieldName(string className, string fieldName)
    {
        return $"{className}__{fieldName}".Replace(".", "_");
    }

    public static string RemoveClassPrefix(string className, string schemaFieldName)
        => schemaFieldName.Replace($"{className}__".Replace(".", "_"), "");
}