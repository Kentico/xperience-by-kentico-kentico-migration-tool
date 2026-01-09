using CMS.DataEngine;
using CMS.FormEngine;
using Kentico.Xperience.UMT.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Builders;
using Migration.Tool.Common.Helpers;
using Migration.Tool.KXP.Api.Auxiliary;

// ReSharper disable ArrangeMethodOrOperatorBody

namespace Migration.Tool.Extensions.ClassMappings;

public static class ClassMappingSample
{
    public static IServiceCollection AddReusableRemodelingSample(this IServiceCollection serviceCollection)
    {
        const string targetClassName = "DancingGoatCore.CoffeeRemodeled";
        // declare target class
        var m = new MultiClassMapping(targetClassName, target =>
        {
            target.ClassName = targetClassName;
            target.ClassTableName = "DancingGoatCore_CoffeeRemodeled";
            target.ClassDisplayName = "Coffee remodeled";
            target.ClassType = ClassType.CONTENT_TYPE;
            target.ClassContentTypeType = ClassContentTypeType.REUSABLE;
            target.ClassWebPageHasUrl = false;
        });

        // set new primary key
        m.BuildField("CoffeeRemodeledID").AsPrimaryKey();

        // change fields according to new requirements
        const string sourceClassName = "DancingGoatCore.Coffee";
        m
            .BuildField("FarmRM")
            .SetFrom(sourceClassName, "CoffeeFarm", true)
            .WithFieldPatch(f => f.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Farm RM"));

        // field clone sample
        m
            .BuildField("FarmRM_Clone")
            .SetFrom(sourceClassName, "CoffeeFarm", true)
            .WithFieldPatch(f => f.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Farm RM Clone"));

        m
            .BuildField("CoffeeCountryRM")
            .WithFieldPatch(f => f.Caption = "Country RM")
            .SetFrom(sourceClassName, "CoffeeCountry", true);

        m
            .BuildField("CoffeeVarietyRM")
            .SetFrom(sourceClassName, "CoffeeVariety", true)
            .WithFieldPatch(f => f.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Variety RM"));

        m
            .BuildField("CoffeeProcessingRM")
            .SetFrom(sourceClassName, "CoffeeProcessing", true)
            .WithFieldPatch(f => f.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Processing RM"));

        m
            .BuildField("CoffeeAltitudeRM")
            .SetFrom(sourceClassName, "CoffeeAltitude", true)
            .WithFieldPatch(f => f.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Altitude RM"));

        m
            .BuildField("CoffeeIsDecafRM")
            .SetFrom(sourceClassName, "CoffeeIsDecaf", true)
            .WithFieldPatch(f => f.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "IsDecaf RM"));

        // Example of adding a new field that doesn't exist in the source class
        m
            .AddField("CoffeeRating", "integer")
            .WithFieldPatch(f =>
            {
                f.Caption = "Coffee Rating";
                f.AllowEmpty = true;
                f.DataType = FieldDataType.Integer;
                f.DefaultValue = "0";
            });

        // Example of adding a new taxonomy field
        m
            .AddField("CoffeeCategories", "taxonomy")
            .WithFieldPatch(f =>
            {
                f.Caption = "Coffee Categories";
                f.AllowEmpty = true;
                f.DataType = "taxonomy";
                f.Settings["controlname"] = "Kentico.Administration.TagSelector";
                // example of setting taxonomy group by its GUID
                f.Settings["TaxonomyGroup"] = "[\"1C9D79E0-482E-468C-9C2A-6CBB53BE53F7\"]";
            });

        // register class mapping
        serviceCollection.AddSingleton<IClassMapping>(m);

        return serviceCollection;
    }

    public static IServiceCollection AddSimpleRemodelingSample(this IServiceCollection serviceCollection)
    {
        const string targetClassName = "DancingGoatCore.CoffeeRemodeled";
        // declare target class
        var m = new MultiClassMapping(targetClassName, target =>
        {
            target.ClassName = targetClassName;
            target.ClassTableName = "DancingGoatCore_CoffeeRemodeled";
            target.ClassDisplayName = "Coffee remodeled";
            target.ClassType = ClassType.CONTENT_TYPE;
            target.ClassContentTypeType = ClassContentTypeType.WEBSITE;
        });

        // set new primary key
        m.BuildField("CoffeeRemodeledID").AsPrimaryKey();

        // change fields according to new requirements
        const string sourceClassName = "DancingGoatCore.Coffee";
        m
            .BuildField("FarmRM")
            .SetFrom(sourceClassName, "CoffeeFarm", true)
            .WithFieldPatch(f => f.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Farm RM"));

        m
            .BuildField("CoffeeCountryRM")
            .WithFieldPatch(f => f.Caption = "Country RM")
            .SetFrom(sourceClassName, "CoffeeCountry", true);

        m
            .BuildField("CoffeeVarietyRM")
            .SetFrom(sourceClassName, "CoffeeVariety", true)
            .WithFieldPatch(f => f.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Variety RM"));

        m
            .BuildField("CoffeeProcessingRM")
            .SetFrom(sourceClassName, "CoffeeProcessing", true)
            .WithFieldPatch(f => f.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Processing RM"));

        m
            .BuildField("CoffeeAltitudeRM")
            .SetFrom(sourceClassName, "CoffeeAltitude", true)
            .WithFieldPatch(f => f.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Altitude RM"));

        m
            .BuildField("CoffeeIsDecafRM")
            .SetFrom(sourceClassName, "CoffeeIsDecaf", true)
            .WithFieldPatch(f => f.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "IsDecaf RM"));

        // register class mapping
        serviceCollection.AddSingleton<IClassMapping>(m);

        return serviceCollection;
    }

    public static IServiceCollection AddClassMergeExample(this IServiceCollection serviceCollection)
    {
        const string targetClassName = "ET.Event";

        const string sourceClassName1 = "_ET.Event1";
        const string sourceClassName2 = "_ET.Event2";

        var m = new MultiClassMapping(targetClassName, target =>
        {
            target.ClassName = targetClassName;
            target.ClassTableName = "ET_Event";
            target.ClassDisplayName = "ET - MY new transformed event";
            target.ClassType = ClassType.CONTENT_TYPE;
            target.ClassContentTypeType = ClassContentTypeType.WEBSITE;
        });

        // register custom table handler once for all custom table mappings
        serviceCollection.AddTransient<SampleCustomTableHandler>();

        m.BuildField("EventID").AsPrimaryKey();

        // build new field
        var title = m.BuildField("Title");
        // map "EventTitle" field form source data class "_ET.Event1" also use it as template for target field
        title.SetFrom(sourceClassName1, "EventTitle", true);
        // map "EventTitle" field form source data class "_ET.Event2"
        title.SetFrom(sourceClassName2, "EventTitle");
        // patch field definition, in this case lets change field caption 
        title.WithFieldPatch(f => f.Caption = "Event title");


        var description = m.BuildField("Description");
        description.SetFrom(sourceClassName2, "EventSmallDesc", true);
        description.WithFieldPatch(f => f.Caption = "Event description");

        var teaser = m.BuildField("Teaser");
        teaser.SetFrom(sourceClassName1, "EventTeaser", true);
        teaser.WithFieldPatch(f => f.Caption = "Event teaser");

        var text = m.BuildField("Text");
        text.SetFrom(sourceClassName1, "EventText", true);
        text.SetFrom(sourceClassName2, "EventHtml");
        text.WithFieldPatch(f => f.Caption = "Event text");

        var startDate = m.BuildField("StartDate");
        startDate.SetFrom(sourceClassName1, "EventDateStart", true);
        // if needed use value conversion to adapt value
        Func<object?, IConvertorContext, object?> dateConvertor = (v, context) =>
        {
            switch (context)
            {
                case ConvertorTreeNodeContext treeNodeContext:
                    // here you can use available treenode context
                    // (var nodeGuid, int nodeSiteId, int? documentId, bool migratingFromVersionHistory) = treeNodeContext;
                    break;
                default:
                    // no context is available (in future, mapping feature could be extended and therefore different context will be supplied or no context at all)
                    break;
            }

            // be strict about value assertion and isolate any unexpected value

            return v switch
            {
                string stringValue => !string.IsNullOrWhiteSpace(stringValue) ? DateTime.Parse(stringValue) : null, // valid date or empty string isd expected, otherwise error
                DateTime dateTimeValue => dateTimeValue,
                null => null,
                _ => throw new InvalidOperationException($"I didn't expected this value in instance of my class! {v}")
            };
        };
        startDate.ConvertFrom(sourceClassName2, "EventStartDateAsText", false, dateConvertor);
        startDate.WithFieldPatch(f => f.Caption = "Event start date");

        serviceCollection.AddSingleton<IClassMapping>(m);

        return serviceCollection;
    }

    public static IServiceCollection AddClassMergeExampleAsReusable(this IServiceCollection serviceCollection)
    {
        const string targetClassName = "ET.Event";

        const string sourceClassName1 = "_ET.Event1";
        const string sourceClassName2 = "_ET.Event2";
        const string sourceClassName3 = "_ET.EventCustomTable";

        var m = new MultiClassMapping(targetClassName, target =>
        {
            target.ClassName = targetClassName;
            target.ClassTableName = "ET_Event";
            target.ClassDisplayName = "ET - MY new transformed event";
            target.ClassType = ClassType.CONTENT_TYPE;
            target.ClassContentTypeType = ClassContentTypeType.REUSABLE;
        });

        m.SetHandler<SampleCustomTableHandler>();

        // register custom table handler once for all custom table mappings
        serviceCollection.AddTransient<SampleCustomTableHandler>();

        m.BuildField("EventID").AsPrimaryKey();

        // build new field
        var title = m.BuildField("Title");
        // map "EventTitle" field form source data class "_ET.Event1" also use it as template for target field
        title.SetFrom(sourceClassName1, "EventTitle", true);
        // map "EventTitle" field form source data class "_ET.Event2"
        title.SetFrom(sourceClassName2, "EventTitle");
        // map "TitleCT" from custom table
        title.SetFrom(sourceClassName3, "TitleCT");
        // patch field definition, in this case lets change field caption 
        title.WithFieldPatch(f => f.Caption = "Event title");


        var description = m.BuildField("Description");
        description.SetFrom(sourceClassName2, "EventSmallDesc", true);
        description.SetFrom(sourceClassName3, "DescriptionCT");
        description.WithFieldPatch(f => f.Caption = "Event description");

        var descriptionCopy = m.BuildField("DescriptionCopy");
        descriptionCopy.SetFrom(sourceClassName2, "EventSmallDesc", true);
        descriptionCopy.SetFrom(sourceClassName3, "DescriptionCT");
        descriptionCopy.WithFieldPatch(f =>
        {
            // for copied field, we also need to adjust field guid
            f.Guid = GuidHelper.CreateFieldGuid($"DescriptionCopy"); // deterministic guid to ensure not conflicts occur in repeated migration
            f.Caption = "Event description copy";
        });

        var teaser = m.BuildField("Teaser");
        teaser.SetFrom(sourceClassName1, "EventTeaser", true);
        teaser.WithFieldPatch(f => f.Caption = "Event teaser");

        var text = m.BuildField("Text");
        text.SetFrom(sourceClassName1, "EventText", true);
        text.SetFrom(sourceClassName2, "EventHtml");
        text.SetFrom(sourceClassName3, "EventHtmlCT");
        text.WithFieldPatch(f => f.Caption = "Event text");

        var startDate = m.BuildField("StartDate");
        startDate.SetFrom(sourceClassName1, "EventDateStart", true);
        // if needed use value conversion to adapt value
        Func<object?, IConvertorContext, object?> dateConvertor = (v, context) =>
        {
            switch (context)
            {
                case ConvertorTreeNodeContext treeNodeContext:
                    // here you can use available treenode context
                    // (var nodeGuid, int nodeSiteId, int? documentId, bool migratingFromVersionHistory) = treeNodeContext;
                    break;
                case ConvertorCustomTableContext customTableContext:
                {
                    // handle any specific requirements if source data are of custom table class type 
                    break;
                }
                default:
                    // no context is available (in future, mapping feature could be extended and therefore different context will be supplied or no context at all)
                    break;
            }

            // be strict about value assertion and isolate any unexpected value

            return v switch
            {
                string stringValue => !string.IsNullOrWhiteSpace(stringValue) ? DateTime.Parse(stringValue) : null, // valid date or empty string isd expected, otherwise error
                DateTime dateTimeValue => dateTimeValue,
                null => null,
                _ => throw new InvalidOperationException($"I didn't expected this value in instance of my class! {v}")
            };
        };
        startDate.ConvertFrom(sourceClassName2, "EventStartDateAsText", false, dateConvertor);
        startDate.ConvertFrom(sourceClassName3, "EventDateStartCT", false, dateConvertor);
        startDate.WithFieldPatch(f => f.Caption = "Event start date");

        // if desired add system field from custom table
        var itemGuid = m.BuildField("EventGuid");
        itemGuid.SetFrom(sourceClassName3, "ItemGuid", true);
        itemGuid.WithFieldPatch(f =>
        {
            f.AllowEmpty = true; // allow empty, otherwise unmapped classes will throw
            f.Caption = "Event guid";
        });

        serviceCollection.AddSingleton<IClassMapping>(m);



        return serviceCollection;
    }

    public sealed class SampleCustomTableHandler : DefaultCustomTableClassMappingHandler
    {
        private readonly ILogger<DefaultCustomTableClassMappingHandler> logger;

        public SampleCustomTableHandler(ILogger<DefaultCustomTableClassMappingHandler> logger) : base(logger) => this.logger = logger;

        public override void EnsureContentItemLanguageMetadata(ContentItemLanguageMetadataModel languageMetadataInfo, CustomTableMappingHandlerContext context)
        {
            // always call base if You want to use default fallbacks and guid derivation/generation
            // if base is not called, You have to handle all fallbacks Yourself 
            base.EnsureContentItemLanguageMetadata(languageMetadataInfo, context);

            if (context.SourceClassName.Equals("_ET.EventCustomTable", StringComparison.InvariantCultureIgnoreCase))
            {
                if (context.Values.TryGetValue("TitleCT", out object? mbTitle) && mbTitle is string titleCt)
                {
                    languageMetadataInfo.ContentItemLanguageMetadataDisplayName = titleCt;
                }
                else
                {
                    // no value assigned to my custom "TitleCT", i will leave fallback value generated with "DefaultCustomTableClassMappingHandler"
                }
            }
            else
            {
                // unexpected dataclass?
                logger.LogError("Unexpected data class '{ClassName}' in my mapping", context.SourceClassName);
            }
        }
    }

    /// <summary>
    /// This sample uses data from the official Kentico 11 e-shop demo
    /// </summary>
    public static IServiceCollection AddControlledCategoryMigrationSample(this IServiceCollection serviceCollection)
    {
        const string targetClassName = "Eshop.BookRemodeled";
        // declare target class
        var m = new MultiClassMapping(targetClassName, target =>
        {
            target.ClassName = targetClassName;
            target.ClassTableName = "Eshop_BookRemodeled";
            target.ClassDisplayName = "Book remodeled";
            target.ClassType = ClassType.CONTENT_TYPE;
            target.ClassContentTypeType = ClassContentTypeType.REUSABLE;
            target.ClassWebPageHasUrl = false;
        });

        // set new primary key
        m.BuildField("BookRemodeledID").AsPrimaryKey();

        // change fields according to new requirements
        const string sourceClassName1 = "CMSProduct.Book";

        // field clone sample
        m
            .BuildField("BookAuthor")
            .SetFrom(sourceClassName1, "BookAuthor", true)
            .WithFieldPatch(f => f.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "Author's name"));

        m
            .BuildField("BookISBN")
            .SetFrom(sourceClassName1, "BookISBN", true)
            .WithFieldPatch(f => f.Caption = "ISBN");

        // category migration refinement sample
        const int EbookCategoryID = 17;
        int[] excludedCategories = [EbookCategoryID,];
        m.FilterCategories((className, categoryID) => !excludedCategories.Contains(categoryID));

        // register class mapping
        serviceCollection.AddSingleton<IClassMapping>(m);

        return serviceCollection;
    }

    public static IServiceCollection AddReusableSchemaIntegrationSample(this IServiceCollection serviceCollection)
    {
        const string schemaNameDgcCommon = "DGC.Address";
        const string schemaNameDgcName = "DGC.Name";
        const string sourceClassName = "DancingGoatCore.Cafe";

        // create instance of reusable schema builder - class will help us with definition of new reusable schema
        var sb = new ReusableSchemaBuilder(schemaNameDgcCommon, "Common address", "Reusable schema that defines common address");

        sb
            .BuildField("City")
            .WithFactory(() => new FormFieldInfo
            {
                Name = "City",
                Caption = "City",
                Guid = new Guid("F9DC7EBE-29CA-4591-BF43-E782D50624AF"),
                DataType = FieldDataType.Text,
                Size = 400,
                Settings =
                {
                    ["controlname"] = FormComponents.AdminTextInputComponent
                }
            });

        sb
            .BuildField("Street")
            .WithFactory(() => new FormFieldInfo
            {
                Name = "Street",
                Caption = "Street",
                Guid = new Guid("712C0B07-45AC-4CD0-A355-2BA4C46941B6"),
                DataType = FieldDataType.Text,
                Size = 400,
                Settings =
                {
                    ["controlname"] = FormComponents.AdminTextInputComponent
                }
            });

        sb
            .BuildField("ZipCode")
            .CreateFrom(sourceClassName, "CafeZipCode");

        sb
            .BuildField("Phone")
            .CreateFrom(sourceClassName, "CafePhone");


        var sb2 = new ReusableSchemaBuilder(schemaNameDgcName, "Common name", "Reusable schema that defines name field");

        sb2
            .BuildField("Name")
            .WithFactory(() => new FormFieldInfo
            {
                Name = "Name",
                Caption = "Name",
                Guid = new Guid("3213B67F-23F1-4F6F-9E5F-C74DE5F84BC5"),
                DataType = FieldDataType.Text,
                Size = 400,
                Settings =
                {
                    ["controlname"] = FormComponents.AdminTextInputComponent
                }
            });

        var m = new MultiClassMapping("DancingGoatCore.CafeRS", target =>
        {
            target.ClassName = "DancingGoatCore.CafeRS";
            target.ClassTableName = "DancingGoatCore_CafeRS";
            target.ClassDisplayName = "Coffee with reusable schema";
            target.ClassType = ClassType.CONTENT_TYPE;
            target.ClassContentTypeType = ClassContentTypeType.WEBSITE;
        });

        // set primary key
        m.BuildField("CafeID").AsPrimaryKey();

        // declare that we intend to use reusable schema and set mappings to new fields from old ones
        m.UseResusableSchema(schemaNameDgcCommon);
        m.BuildField("City").SetFrom(sourceClassName, "CafeCity");
        m.BuildField("Street").SetFrom(sourceClassName, "CafeStreet");
        m.BuildField("ZipCode").SetFrom(sourceClassName, "CafeZipCode");
        m.BuildField("Phone").SetFrom(sourceClassName, "CafePhone");

        m.UseResusableSchema(schemaNameDgcName);
        m.BuildField("Name").SetFrom(sourceClassName, "CafeName");

        // old fields we leave in data class
        m.BuildField("CafePhoto").SetFrom(sourceClassName, "CafePhoto", isTemplate: true);
        m.BuildField("CafeAdditionalNotes").SetFrom(sourceClassName, "CafeAdditionalNotes", isTemplate: true);

        // in similar manner we can define other classes where we want to use reusable schema
        // var m2 = new MultiClassMapping("DancingGoatCore.MyOtherClass", target =>
        // ...
        // m2.UseResusableSchema(schemaNameDgcCommon);
        // m2.BuildField ...
        // serviceCollection.AddSingleton<IClassMapping>(m2);

        // register mapping
        serviceCollection.AddSingleton<IClassMapping>(m);
        // register reusable schema builder
        serviceCollection.AddSingleton<IReusableSchemaBuilder>(sb);
        serviceCollection.AddSingleton<IReusableSchemaBuilder>(sb2);

        return serviceCollection;
    }

    public static IServiceCollection AddReusableSchemaAutoGenerationSample(this IServiceCollection serviceCollection)
    {
        var sb = new ReusableSchemaBuilder("DancingGoatCore.ArticleBase", "Article Base Info", "");

        // Automatically generate reusable field schema from a source class.
        //
        // transformFieldName allows for overriding the final field names. In the form used here (x => x) it takes the
        // source names without change. That is different from default, non-overriden behaviour which uses class prefix.
        // If transformFieldNames is used, user is responsible for preventing name collisions. Note that RFS field names
        // must be unique compared to all other RFS's and class field names.
        sb.ConvertFrom("DancingGoatCore.Article", x => x);

        // The original class will be excluded from default migration. To have both the converted-to reusable field schema
        // and the source class, the source class must be manually mapped, as we do it here.
        var m = new MultiClassMapping("DancingGoatCore.ArticleRS", target =>
        {
            target.ClassName = "DancingGoatCore.ArticleRS";
            target.ClassTableName = "DancingGoatCore_ArticleRS";
            target.ClassDisplayName = "Article with reusable schema";
            target.ClassType = ClassType.CONTENT_TYPE;
            target.ClassContentTypeType = ClassContentTypeType.WEBSITE;
        });

        // set primary key
        m.BuildField("ArticleID").AsPrimaryKey();

        // declare that we intend to use reusable schema and set mappings to new fields from old ones
        m.UseResusableSchema("DancingGoatCore.ArticleBase");
        m.BuildField("ArticleTitle").SetFrom("DancingGoatCore.Article", "ArticleTitle", true);
        m.BuildField("ArticleTeaser").SetFrom("DancingGoatCore.Article", "ArticleTeaser", true);

        // register mapping
        serviceCollection.AddSingleton<IClassMapping>(m);

        // register reusable schema builder
        serviceCollection.AddSingleton<IReusableSchemaBuilder>(sb);

        return serviceCollection;
    }

    public static IServiceCollection AddMappingToPrefabricatedContentTypeSample(this IServiceCollection serviceCollection)
    {
        // Summary:
        //   This sample shows how to migrate pages to a content type that is already present in the target instance
        //   - i.e., not created by the --page-types command.
        //
        // Prerequisites:
        //   1. CLI is invoked with --bypass-dependency-check argument and without --page-types argument.
        //
        //   2. The following structures already exist in the target XbyK instance. They might have been created by MT,
        //      manually in XbyK administration or by other means.
        //          A. Reusable field schema PrefabBase
        //          B. Content type DancingGoatCore.PrefabArticle that uses PrefabBase. It can be of both reusable and page usage.
        //          C. In case of migrating media to content hub, Legacy Media File content type must exist
        //             and match AssetFacade.LegacyMediaFileContentType class name and class GUID. See PrefabArticleTeaser below.
        //
        //   3. Field types in the target structures must be compatible with source field types - i.e., Migration Tool must support
        //      migration from one to the other.
        //
        //      If you're unsure what the target field type should be, let MT migrate page types (--page-types CLI command)
        //      into a disposable clone of your target instance and see the produced field types.

        var m = new MultiClassMapping("DancingGoatCore.PrefabArticle", _ => { });
        const string sourceClassName = "DancingGoatCore.Article";

        // Field mapping
        m.BuildField("PrefabArticleSummary")
            .SetFrom(sourceClassName, "ArticleSummary");

        // For media file mapping, make sure the target field supports the media file migration strategy 
        // set by appsettings MigrateMediaToMediaLibrary.
        //   - MigrateMediaToMediaLibrary=true -> target field data type = 'Media files'
        //   - MigrateMediaToMediaLibrary=false -> target field data type = 'Pages and reusable content'
        //     and allowed content types must include Legacy Media File content type. See AssetFacade.LegacyMediaFileContentType
        m.BuildField("PrefabArticleTeaser")
            .SetFrom(sourceClassName, "ArticleTeaser");

        // For linked items mapping, make sure the target field has 'Pages and reusable content' data type
        // and allowed content types include the content type of linked items. What this linked content type actually is
        // depends on your specific setup (custom class mapping, content item directors, etc.).
        m.BuildField("PrefabArticleRelatedArticles")
            .SetFrom(sourceClassName, "ArticleRelatedArticles");

        // Reusable field schema field mapping
        m.UseResusableSchema("PrefabBase");
        m.BuildField("PrefabBaseTitle")
            .SetFrom(sourceClassName, "ArticleTitle");

        serviceCollection.AddSingleton<IClassMapping>(m);
        return serviceCollection;
    }
}
