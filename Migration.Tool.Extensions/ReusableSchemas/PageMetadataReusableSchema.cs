using CMS.DataEngine;
using CMS.FormEngine;
using Migration.Tool.Common.Builders;
using Migration.Tool.Common.Helpers;

namespace Migration.Tool.Extensions.ReusableSchemas;

public class PageMetadataReusableSchema
{
    public const string SchemaName = "PageMetadata";
    public const string SetupDisplayName = "Page Metadata";
    public const string SetupDescription = "Reusable schema for page metadata (Title, Description, Keywords)";

    // Field Names
    public const string FieldNameTitle = "DocumentPageTitle";
    public const string FieldNameDescription = "DocumentPageDescription";
    public const string FieldNameKeywords = "DocumentPageKeyWords";

    // Build the schema
    public static IReusableSchemaBuilder CreateBuilder()
    {
        var builder = new ReusableSchemaBuilder(SchemaName, SetupDisplayName, SetupDescription);

        builder.BuildField(FieldNameTitle)
            .WithFactory(() => new FormFieldInfo
            {
                Name = FieldNameTitle,
                Caption = "Page Title",
                DataType = FieldDataType.Text,
                Size = 100,
                AllowEmpty = true,
                Settings = { { "controlname", "Kentico.Administration.TextInput" } },
                Guid = GuidHelper.CreateFieldGuid($"{FieldNameTitle}|{SchemaName}")
            });

        builder.BuildField(FieldNameDescription)
            .WithFactory(() => new FormFieldInfo
            {
                Name = FieldNameDescription,
                Caption = "Page Description",
                DataType = FieldDataType.LongText, // Description is often long
                AllowEmpty = true,
                Settings = { { "controlname", "Kentico.Administration.TextArea" } },
                Guid = GuidHelper.CreateFieldGuid($"{FieldNameDescription}|{SchemaName}")
            });

        builder.BuildField(FieldNameKeywords)
            .WithFactory(() => new FormFieldInfo
            {
                Name = FieldNameKeywords,
                Caption = "Page Keywords",
                DataType = FieldDataType.LongText,
                AllowEmpty = true,
                Settings = { { "controlname", "Kentico.Administration.TextArea" } },
                Guid = GuidHelper.CreateFieldGuid($"{FieldNameKeywords}|{SchemaName}")
            });

        return builder;
    }
}
