using System.Xml.Linq;
using CMS.Websites;
using Migration.Tool.Common;
using Migration.Tool.Common.Services;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source.Contexts;
using Newtonsoft.Json;

namespace Migration.Tool.Extensions.CommunityMigrations;

/// <summary>
/// Migrates fields of type 'text' using the 'Page selector' form control to the
/// 'Kentico.Administration.WebPageSelector' component with data type 'webpages'.
///
/// The source value is a single NodeGUID (string, nullable).
/// The target value is a JSON array of <see cref="WebPageRelatedItem"/> objects.
/// </summary>
public class SelectDocumentMigration(ISpoiledGuidContext spoiledGuidContext) : IFieldMigration
{
    public int Rank => 10;

    public bool ShallMigrate(FieldMigrationContext context) =>
        context.SourceDataType is "text" &&
        string.Equals(context.SourceFormControl, "selectdocument", StringComparison.OrdinalIgnoreCase);

    public void MigrateFieldDefinition(FormDefinitionPatcher formDefinitionPatcher, XElement field, XAttribute? columnTypeAttr, string fieldDescriptor)
    {
        columnTypeAttr?.SetValue("webpages");

        var settings = field.EnsureElement(FormDefinitionPatcher.FieldElemSettings);
        settings.EnsureElement(FormDefinitionPatcher.SettingsElemControlname, e => e.Value = "Kentico.Administration.WebPageSelector");
    }

    public Task<FieldMigrationResult> MigrateValue(object? sourceValue, FieldMigrationContext context)
    {
        if (sourceValue is not string rawValue || string.IsNullOrWhiteSpace(rawValue))
        {
            return Task.FromResult(new FieldMigrationResult(true, null));
        }

        if (!Guid.TryParse(rawValue.Trim(), out var nodeGuid))
        {
            return Task.FromResult(new FieldMigrationResult(false, null));
        }

        Guid webPageGuid = context.SourceObjectContext is DocumentSourceObjectContext documentContext
            ? spoiledGuidContext.EnsureNodeGuid(nodeGuid, documentContext.Site.SiteID)
            : nodeGuid;

        var relatedItems = new[] { new WebPageRelatedItem { WebPageGuid = webPageGuid } };
        string json = JsonConvert.SerializeObject(relatedItems);

        return Task.FromResult(new FieldMigrationResult(true, json));
    }
}
