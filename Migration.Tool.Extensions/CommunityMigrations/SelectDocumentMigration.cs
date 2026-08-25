using System.Xml.Linq;
using CMS.Websites;
using Migration.Tool.Common;
using Migration.Tool.Common.Enumerations;
using Migration.Tool.Common.Services;
using Migration.Tool.KXP.Api.Auxiliary;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source.Contexts;
using Newtonsoft.Json;

namespace Migration.Tool.Extensions.CommunityMigrations;

/// <summary>
/// Migrates fields of type 'text' using the 'Page selector' form control to the
/// <see cref="FormComponents.Kentico_Xperience_Admin_Websites_WebPageSelectorComponent"/> component with data type 'webpages'.
///
/// The source value is a single NodeGUID (string, nullable).
/// The target value is a JSON array of <see cref="WebPageRelatedItem"/> objects.
/// </summary>
public class SelectDocumentMigration(ISpoiledGuidContext spoiledGuidContext) : IFieldMigration
{
    public int Rank => 10;

    public bool ShallMigrate(FieldMigrationContext context) =>
        context.SourceDataType is "text" &&
        string.Equals(context.SourceFormControl, Kx13FormControls.UserControlForText.Selectdocument, StringComparison.OrdinalIgnoreCase);

    public void MigrateFieldDefinition(FormDefinitionPatcher formDefinitionPatcher, XElement field, XAttribute? columnTypeAttr, string fieldDescriptor)
    {
        // Change the column type from 'text' to 'webpages' to match the WebPageSelector component
        columnTypeAttr?.SetValue("webpages");

        // Switch the form control to the XbyK web page selector component
        var settings = field.EnsureElement(FormDefinitionPatcher.FieldElemSettings);
        settings.EnsureElement(FormDefinitionPatcher.SettingsElemControlname, e => e.Value = FormComponents.Kentico_Xperience_Admin_Websites_WebPageSelectorComponent);
    }

    public Task<FieldMigrationResult> MigrateValue(object? sourceValue, FieldMigrationContext context)
    {
        // Treat missing or empty values as a successful no-op
        if (sourceValue is not string rawValue || string.IsNullOrWhiteSpace(rawValue))
        {
            return Task.FromResult(new FieldMigrationResult(true, null));
        }

        // The source stores a single NodeGUID; reject values that cannot be parsed
        if (!Guid.TryParse(rawValue.Trim(), out var nodeGuid))
        {
            return Task.FromResult(new FieldMigrationResult(false, null));
        }

        // Only document contexts carry a SiteID; reject anything else as unmappable
        if (context.SourceObjectContext is not DocumentSourceObjectContext documentContext)
        {
            return Task.FromResult(new FieldMigrationResult(false, null));
        }

        // Resolve the GUID through the spoiled-GUID context so that any NodeGUID remapping applied during migration is respected
        Guid webPageGuid = spoiledGuidContext.EnsureNodeGuid(nodeGuid, documentContext.Site.SiteID);

        // Wrap the single GUID in the array format expected by the WebPageSelector component
        var relatedItems = new[] { new WebPageRelatedItem { WebPageGuid = webPageGuid } };
        string json = JsonConvert.SerializeObject(relatedItems);

        return Task.FromResult(new FieldMigrationResult(true, json));
    }
}
