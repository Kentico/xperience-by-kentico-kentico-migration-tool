using CMS.DataEngine;
using CMS.FormEngine;
using Microsoft.Extensions.DependencyInjection;
using Migration.Tool.Common.Builders;
using Migration.Tool.KXP.Api.Auxiliary;

// ReSharper disable ArrangeMethodOrOperatorBody

namespace Migration.Tool.Extensions.ClassMappings;

public static class ClassMappingSample
{
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
        startDate.ConvertFrom(sourceClassName2, "EventStartDateAsText", false,
            v => v?.ToString() is { } av && !string.IsNullOrWhiteSpace(av) ? DateTime.Parse(av) : null
        );
        startDate.WithFieldPatch(f => f.Caption = "Event start date");

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
}
