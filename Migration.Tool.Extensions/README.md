# Migration Tool customization

To create custom migrations:

1. Create the custom migration class:
    - [Field migrations](#customize-field-migrations)
    - [Widget migrations](#customize-widget-migrations)
    - [Widget property migrations](#customize-widget-property-migrations)
    - [Custom class mappings](#custom-class-mappings)
2. [Register the migration](#register-migrations)

## Customize field migrations

You can customize field migrations to customize the default mappings of fields of page types, modules, and forms. In the `Migration.Tool.Extensions/CommunityMigrations` folder, create a new file with a class that implements the `IFieldMigration` interface. Implement the following properties and methods required by the interface:

- `Rank` - An integer property that determines the order in which migrations are applied. Use a value lower than *100000*, as that is the value for system migrations.
- `ShallMigrate` - A boolean method that specifies whether the migration shall be applied for the current field. Use properties of the `FieldMigrationContext` object passed as an argument to the method to evaluate the condition. For each field, the migration with the lowest rank that returns `true` from the `ShallMigrate` method is used.
  - `SourceDataType` - A string property that specifies the [data type](https://docs.kentico.com/x/coJwCg) of the source field.
  - `SourceFormControl` - A string property that specifies the [form control](https://docs.kentico.com/x/lAyRBg) used by the source field.
  - `FieldName` - A string property that specifies the code name of the field.
  - `SourceObjectContext` - An interface that can be used to check the context of the source object (e.g. page vs. form).
- `MigrateFieldDefinition` - Migrate the definition of the field. Use the `XElement` attribute to transform the field.
- `MigrateValue` - Migrate the value of the field. Use the source value and the `FieldMigrationContext` with the same parameters as described in `ShallMigrate`.

You can see samples:

- [SampleTextMigration.cs](./CommunityMigrations/SampleTextMigration.cs)
  - A sample migration with further explanations
- [AssetMigration.cs](./DefaultMigrations/AssetMigration.cs)
  - An example of a usable migration

After implementing the migration, you need to [register the migration](#register-migrations) in the system.

## Customize widget migrations

You can customize widget migration to change the widget to which source widgets are migrated in the target instance. In the `Migration.Tool.Extensions/CommunityMigrations` folder, create a new file with a class that implements the `IWidgetMigration` interface. Implement the following properties and methods required by the interface:

- `Rank` - An integer property that determines the order in which migrations are applied. Use a value lower than *100000*, as that is the value for system migrations.
- `ShallMigrate` - A boolean method that specifies whether the migration shall be applied for the current widget. Use properties of the `WidgetMigrationContext` and `WidgetIdentifier` objects passed as an argument to the method to evaluate the condition. For each widget, the migration with the lowest rank that returns `true` from the `ShallMigrate` method is used.
  - `WidgetMigrationContext.SiteId` - An integer property that specifies the ID of the site on which the widget was used in the source instance.
  - `WidgetIdentifier.TypeIdentifier` - A string property that specifies the identifier of the widget.
- `MigrateWidget`- Migrate the widget data using the following properties:
  - `identifier` - A `WidgetIdentifier` object, see `ShallMigrate` to see properties.
  - `value` - A `JToken` object containing the deserialized value of the property.
  - `context` - A `WidgetMigrationContext` object, see `ShallMigrate` to see properties.

You can see a sample: [SampleWidgetMigration.cs](./CommunityMigrations/SampleWidgetMigration.cs)

After implementing the migration, you need to [register the migration](#register-migrations) in the system.

## Customize widget property migrations

In the `Migration.Tool.Extensions/CommunityMigrations` folder, create a new file with a class that implements the `IWidgetPropertyMigration` interface. Implement the following properties and methods required by the interface:

- `Rank` - An integer property that determines the order in which migrations are applied. Use a value lower than *100000*, as that is the value for system migrations.
- `ShallMigrate` - A boolean method that specifies whether the migration shall be applied for the current widget. Use properties of the `WidgetPropertyMigrationContext` and `propertyName` objects passed as an argument to the method to evaluate the condition. For each widget property, the migration with the lowest rank that returns `true` from the `ShallMigrate` method is used.
  - `WidgetPropertyMigrationContext.SiteId` - An integer property that specifies the ID of the site on which the widget was used in the source instance.
  - `WidgetPropertyMigrationContext.EditingFormControlModel` - An object representing the [form control](https://docs.kentico.com/x/lAyRBg) of the property.
  - `propertyName` - A string property that specifies the identifier of the property.
- `MigrateWidgetProperty`- Migrate the widget property data using the following properties:
  - `key` - Name of the property.
  - `value` - A `JToken` object containing the deserialized value of the property.
  - `context` - A `WidgetPropertyMigrationContext`. See `ShallMigrate` method to see the properties.

You can see samples:

- [Path selector migration](./DefaultMigrations/WidgetPathSelectorMigration.cs)
- [Page selector migration](./DefaultMigrations/WidgetPageSelectorMigration.cs)
- [File selector migration](./DefaultMigrations/WidgetFileMigration.cs)

After implementing the migration, you need to [register the migration](#register-migrations) in the system.

## Custom class mappings

Example code is found in `Migration.Tool.Extensions/ClassMappings/ClassMappingSample.cs`.

### Class remodeling sample

Example code is found in the method `AddSimpleRemodelingSample`.

The goal of this method is to take a **single data class** and change it to more suitable shape.

### Class merge sample

Example code is found in the method `AddClassMergeExample`.

The goal of this method is to take **multiple data classes** from the source instance and define their relation to a new class.

### Class mapping with category control

Example code is found in the method `AddK11EshopExample`.

The goal of this method is to show how to **control migration of categories**. You can enable/disable the migration based on category ID and/or source class name. 
This is useful when merging multiple data classes into one (see _Class merge sample_)

### Example

Let's define a new class:

```csharp
var m = new MultiClassMapping(targetClassName, target =>
{
    target.ClassName = targetClassName;
    target.ClassTableName = "ET_Event";
    target.ClassDisplayName = "ET - MY new transformed event";
    target.ClassType = ClassType.CONTENT_TYPE;
    target.ClassContentTypeType = ClassContentTypeType.WEBSITE;
});
```

Then define a new primary key:

```csharp
m.BuildField("EventID").AsPrimaryKey();
```

Finally, let's define relations to fields:

1. build field title

   ```csharp
   // build new field
   var title = m.BuildField("Title");

   // map "EventTitle" field form source data class "_ET.Event1" also use it as template for target field
   title.SetFrom("_ET.Event1", "EventTitle", true);
   // map "EventTitle" field form source data class "_ET.Event2"
   title.SetFrom("_ET.Event2", "EventTitle");

   // patch field definition, in this case lets change field caption
   title.WithFieldPatch(f => f.Caption = "Event title");
   ```

1. in similar fashion map other fields
1. if needed custom value conversion can be used

   ```csharp
   var startDate = m.BuildField("StartDate");
   startDate.SetFrom("_ET.Event1", "EventDateStart", true);
   // if needed use value conversion to adapt value
   startDate.ConvertFrom("_ET.Event2", "EventStartDateAsText", false,
       (v, context) =>
       {
           switch (context)
           {
               case ConvertorTreeNodeContext treeNodeContext:
                   // here you can use available treenode context
                   // (var nodeGuid, int nodeSiteId, int? documentId, bool migratingFromVersionHistory) = treeNodeContext;
                   break;
               default:
                   // no context is available (possibly when tool is extended with other conversion possibilities)
                   break;
           }

           return v?.ToString() is { } av && !string.IsNullOrWhiteSpace(av) ? DateTime.Parse(av) : null;
       });
   startDate.WithFieldPatch(f => f.Caption = "Event start date");
   ```

1. After implementing the mapping, you need to [register it](#register-migrations) in the system.
1. register class mapping to dependency injection container

   ```csharp
   serviceCollection.AddSingleton<IClassMapping>(m);
   ```

### Inject and use reusable schema

Example code is found in the method `AddReusableSchemaIntegrationSample`.

The goal of this method is to take a **single data class** and assign reusable schema.

### Convert page type to reusable content item (content hub)

Example code is found in the method `AddReusableRemodelingSample`.

Please note, that all information unique to page will be lost.

## Register migrations

Register the migration in `Migration.Tool.Extensions/ServiceCollectionExtensions.cs` as a `Transient` dependency into the service collection:

- Field migrations - `services.AddTransient<IFieldMigration, MyFieldMigration>();`
- Widget migrations - `services.AddTransient<IWidgetMigration, MyWidgetMigration>();`
- Widget property migrations - `services.AddTransient<IWidgetPropertyMigration, MyWidgetPropertyMigration>();`
- Custom class mappings - `services.AddMyCustomMapping();`
