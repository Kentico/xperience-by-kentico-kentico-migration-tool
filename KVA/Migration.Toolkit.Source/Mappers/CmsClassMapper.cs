namespace Migration.Toolkit.Source.Mappers;

using System.Diagnostics;
using System.Xml;
using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Enumerations;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KXP.Api.Services.CmsClass;
using Migration.Toolkit.Source.Contexts;
using Migration.Toolkit.Source.Model;
using Migration.Toolkit.Source.Services;

public class CmsClassMapper(ILogger<CmsClassMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol,
        FieldMigrationService fieldMigrationService,
        ModelFacade modelFacade
    )
    : EntityMapperBase<ICmsClass, DataClassInfo>(logger,
    primaryKeyMappingContext, protocol)
{
    protected override DataClassInfo? CreateNewInstance(ICmsClass source, MappingHelper mappingHelper, AddFailure addFailure) =>
        DataClassInfo.New();

    protected override DataClassInfo MapInternal(ICmsClass source, DataClassInfo target, bool newInstance, MappingHelper mappingHelper,
        AddFailure addFailure)
    {
        target.ClassDisplayName = source.ClassDisplayName;
        target.ClassName = source.ClassName;

        var isCustomizableSystemClass = false;
        var classIsCustom = true;
        var classResource = modelFacade.SelectById<ICmsResource>(source.ClassResourceID);
        if (classResource?.ResourceName is { } resourceName)
        {
            // TODO tomas.krch 2024-03-20: FIX K12SystemResource - shall be version independent
            isCustomizableSystemClass = source.ClassShowAsSystemTable.GetValueOrDefault(false) &&
                                        K12SystemResource.All.Contains(resourceName);

            classIsCustom = !K12SystemResource.All.Contains(resourceName);

            logger.LogDebug("{ClassName} is {@Properties}", source.ClassName, new { isCustomizableSystemClass, classIsCustom, source.ClassResourceID, classResource?.ResourceName });
        }

        MapFormDefinitionFields(source, target, isCustomizableSystemClass, classIsCustom);

        target.ClassHasUnmanagedDbSchema = false;
        if (!string.IsNullOrWhiteSpace(source.ClassTableName))
        {
            target.ClassTableName = source.ClassTableName;
        }
        target.ClassShowTemplateSelection = source.ClassShowTemplateSelection.UseKenticoDefault();
        target.ClassLastModified = source.ClassLastModified;
        target.ClassGUID = source.ClassGUID;

        if (source.ClassContactMapping != null)
        {
            var mapInfo = new FormInfo(source.ClassContactMapping);
            var newMappings = new FormInfo();
            if (mapInfo.ItemsList.Count > 0)
            {
                var ffiLookup = mapInfo.ItemsList.OfType<FormFieldInfo>().ToLookup(f => f.MappedToField, f => f);

                foreach (var formFieldInfos in ffiLookup)
                {
                    newMappings.AddFormItem(formFieldInfos.First());
                }
            }

            target.ClassContactMapping = newMappings.GetXmlDefinition();
        }

        target.ClassContactOverwriteEnabled = source.ClassContactOverwriteEnabled.UseKenticoDefault();
        target.ClassConnectionString = source.ClassConnectionString;
        target.ClassDefaultObjectType = source.ClassDefaultObjectType;
        target.ClassCodeGenerationSettings = source.ClassCodeGenerationSettings;
        target.ClassIconClass = source.ClassIconClass;
        if (source.ClassIsDocumentType)
        {
            target.ClassWebPageHasUrl = true;
        }

        if (mappingHelper.TranslateIdAllowNulls<ICmsResource>(c => c.ResourceID, source.ClassResourceID, out var resourceId))
        {
            if (resourceId.HasValue)
            {
                target.ClassResourceID = resourceId.Value;
            }
        }

        switch (source)
        {
            // Special
            case { ClassName: { } className } when
                className.Equals("cms.site", StringComparison.InvariantCultureIgnoreCase) ||
                className.Equals("cms.root", StringComparison.InvariantCultureIgnoreCase)
                :
            {
                throw new Exception("Unable to map obsolete dataclass");
                return target;
            }
            // Target Other,null
            // Target System,null
            case not null when target is { ClassType: ClassType.OTHER or ClassType.SYSTEM_TABLE }:
            {
                break;
            }
        }

        switch (source)
        {
            // Custom class from custom resource?
            case
            {
                ClassIsForm: false or null,
                ClassIsDocumentType: false,
                ClassResourceID: { } classResourceId
            }:
            {
                target.ClassType = ClassType.OTHER;
                target.ClassContentTypeType = null;

                target = PatchDataClassInfo(target, out var oldPrimaryKeyName, out var documentNameField);

                break;
            }

            // Target Form,null
            case
                {
                    ClassIsDocumentType: false,
                    ClassIsCoupledClass: true,
                    ClassIsForm: true,
                    // ClassIsPage: false
                }
                :
            {
                target.ClassType = ClassType.FORM;
                target.ClassContentTypeType = "";

                break;
            }

            // Target Content,Reusable
            case
            {
                ClassIsDocumentType: false,
                ClassIsForm: false or null,
                ClassShowAsSystemTable: false,
                ClassIsCustomTable: false,
                // ClassIsPage: false
            }:
            {
                target.ClassType = ClassType.CONTENT_TYPE;
                target.ClassContentTypeType = ClassContentTypeType.REUSABLE;

                target = PatchDataClassInfo(target, out var oldPrimaryKeyName, out var documentNameField);
                break;
            }

            // Target Content,Website
            case { ClassName: { } className } when className.Equals("cms.folder", StringComparison.InvariantCultureIgnoreCase):
            case
            {
                ClassIsDocumentType: true,
                ClassIsForm: false or null,
                // ClassIsPage: true
            }:
            {
                target.ClassType = ClassType.CONTENT_TYPE;
                target.ClassContentTypeType = ClassContentTypeType.WEBSITE;

                target = PatchDataClassInfo(target, out var oldPrimaryKeyName, out var documentNameField);
                break;
            }
        }

        return target;
    }

    private void MapFormDefinitionFields(ICmsClass source, DataClassInfo target, bool isCustomizableSystemClass, bool classIsCustom)
    {
        if (!string.IsNullOrWhiteSpace(source.ClassFormDefinition))
        {
            var patcher = new FormDefinitionPatcher(
                logger,
                source.ClassFormDefinition,
                fieldMigrationService,
                source.ClassIsForm.GetValueOrDefault(false),
                source.ClassIsDocumentType,
                isCustomizableSystemClass,
                classIsCustom
            );

            patcher.PatchFields();
            patcher.RemoveCategories(); // TODO tk: 2022-10-11 remove when supported

            var result = patcher.GetPatched();
            if (isCustomizableSystemClass)
            {
                result = FormHelper.MergeFormDefinitions(target.ClassFormDefinition, result);
            }

            var formInfo = new FormInfo(result);
            target.ClassFormDefinition = formInfo.GetXmlDefinition();
        }
        else
        {
            target.ClassFormDefinition = new FormInfo().GetXmlDefinition();
        }
    }

    private DataClassInfo PatchDataClassInfo(DataClassInfo dataClass, out string? oldPrimaryKeyName, out string? documentNameField)
    {
        oldPrimaryKeyName = null;
        documentNameField = null;
        if (dataClass.ClassType is ClassType.CONTENT_TYPE)
        {
            var fi = new FormInfo(dataClass.ClassFormDefinition);
            var tableName = dataClass.ClassTableName;
            var contentTypeManager = Service.Resolve<IContentTypeManager>();
            contentTypeManager.Initialize(dataClass);
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                dataClass.ClassTableName = tableName;
            }
            var nfi = new FormInfo(dataClass.ClassFormDefinition);
            AppendDocumentNameField(nfi, out documentNameField);

            foreach (var dataDefinitionItem in fi.GetFormElements(true, true) ?? new())
            {
                if (dataDefinitionItem is FormFieldInfo ffi)
                {
                    if (ffi is { PrimaryKey: true })
                    {
                        oldPrimaryKeyName = ffi.Name;
                        continue;
                    }

                    if (ffi.DataType.Equals("contentitems", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ffi.DataType = "contentitemreference";
                        if (!ffi.AllowEmpty)
                        {
                            ffi.ValidationRuleConfigurationsXmlData = AppendRequiredValidationRule(ffi.ValidationRuleConfigurationsXmlData);
                        }
                    }

                    if (ffi.DataType.Equals("pages", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ffi.DataType = "webpages";
                    }

                    if (string.IsNullOrWhiteSpace(ffi.Settings["controlname"] as string))
                    {
                        ffi.Visible = false;
                    }
                }

                nfi.AddFormItem(dataDefinitionItem);
            }

            Debug.WriteLineIf(oldPrimaryKeyName == null, $"WARN: old PK is null for class '{dataClass.ClassName}'");

            dataClass.ClassFormDefinition = nfi.GetXmlDefinition();
            return dataClass;
        }

        return dataClass;
    }

    private static void AppendDocumentNameField(FormInfo nfi, out string documentNameField)
    {
        // no DocumentName in v27, we supply one in migration
        documentNameField = "DocumentName";
        var i = 0;
        while (nfi.GetFormField(documentNameField) is { })
        {
            documentNameField = $"DocumentName{++i}";
        }

        nfi.AddFormItem(new FormFieldInfo
        {
            Caption = "Page name", // as in v26.x.x
            Name = documentNameField,
            AllowEmpty = false,
            DataType = "text",
            Size = 100,
            Precision = 0,
            DefaultValue = null,
            Guid = Guid.NewGuid(),
            System = false, // no longer system field, system doesn't rely on this field anymore
            Settings = { { "controlname", "Kentico.Administration.TextInput" } }
        });
    }

    private const string RequiredRuleIdentifier = "Kentico.Administration.RequiredValue";

    private string AppendRequiredValidationRule(string rulesXml)
    {
        if (string.IsNullOrWhiteSpace(rulesXml))
        {
            return
                @"<validationrulesdata><ValidationRuleConfiguration><ValidationRuleIdentifier>Kentico.Administration.RequiredValue</ValidationRuleIdentifier><RuleValues /></ValidationRuleConfiguration></validationrulesdata>";
        }

        XmlDocument document = new XmlDocument();
        document.LoadXml($"<root>{rulesXml}</root>");
        var mbIdentifierNodes = document.SelectNodes("//ValidationRuleIdentifier");

        if (mbIdentifierNodes != null)
        {
            if (mbIdentifierNodes.Cast<XmlElement>().Any(identifierNode => identifierNode is { InnerText: RequiredRuleIdentifier }))
            {
                return rulesXml;
            }
        }

        const string RULE_PROTOTYPE =
            @"<ValidationRuleConfiguration><ValidationRuleIdentifier>Kentico.Administration.RequiredValue</ValidationRuleIdentifier><RuleValues /></ValidationRuleConfiguration>";
        var prototype = new XmlDocument();
        prototype.LoadXml(RULE_PROTOTYPE);

        Debug.Assert(prototype.FirstChild != null, "prototype.FirstChild != null");
        var oNode = prototype.FirstChild.CloneNode(true);

        Debug.Assert(document.FirstChild != null, "document.FirstChild != null");
        document.FirstChild.AppendChild(document.ImportNode(oNode, true));
        return document.FirstChild.InnerXml;
    }
}