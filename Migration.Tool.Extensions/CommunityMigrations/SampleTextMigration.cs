using System.Xml.Linq;
using CMS.DataEngine;
using Migration.Tool.Common;
using Migration.Tool.KXP.Api.Auxiliary;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source.Contexts;

namespace Migration.Tool.Extensions.CommunityMigrations;

public class SampleTextMigration : IFieldMigration
{
    // Migrations will be sorted by this number before checking with "ShallMigrate" method. Set rank to number bellow 100 000 (default migration will have 100 000 or higher)
    public int Rank => 5000;

    // this method will check, if this implementation handles migration (for both, definition and value)
    public bool ShallMigrate(FieldMigrationContext context) => context.SourceDataType is "text" or "longtext" && context.SourceFormControl is "MY_COMMUNITY_TEXT_EDITOR";

    public void MigrateFieldDefinition(FormDefinitionPatcher formDefinitionPatcher, XElement field, XAttribute? columnTypeAttr, string fieldDescriptor)
    {
        // now we migrate field definition

        // field is element from class form definition, for example
        /*
    <field allowempty="true" column="MediaSelectionString" columnsize="200" columntype="text"
           guid="6371fc70-0540-4596-b5e3-e7b3c98123bf" visible="true">
        <properties>
            <fieldcaption>Media selection</fieldcaption>
        </properties>
        <settings>
            <controlname>MediaSelectionControl</controlname>
            <ShowPreview>True</ShowPreview>
        </settings>
    </field>
         */

        // we usually want to change field type and column type, lets assume our custom control content is HTML, then target type would be:
        columnTypeAttr?.SetValue(FieldDataType.RichTextHTML);

        var settings = field.EnsureElement(FormDefinitionPatcher.FieldElemSettings);
        // RichText editor control
        settings.EnsureElement(FormDefinitionPatcher.SettingsElemControlname, e => e.Value = FormComponents.AdminRichTextEditorComponent);
        // and rich text configuration
        settings.EnsureElement("ConfigurationName", e => e.Value = "Kentico.Administration.StructuredContent");
    }

    public Task<FieldMigrationResult> MigrateValue(object? sourceValue, FieldMigrationContext context)
    {
        // if required, migrate value (for example cut out any unsupported features or migrated them to supported product variants if available)

        // check context
        if (context.SourceObjectContext is DocumentSourceObjectContext(_, _, _, _, _, _))
        {
            // do migration logic

            // return result
            return Task.FromResult(new FieldMigrationResult(true, sourceValue));
        }
        else
        {
            return Task.FromResult(new FieldMigrationResult(false, null));
        }
    }
}
