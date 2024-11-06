# Custom migrations

Samples:

- `Migration.Tool.Extensions/CommunityMigrations/SampleTextMigration.cs` contains simplest implementation for migration of text fields
- `Migration.Tool.Extensions/DefaultMigrations/AssetMigration.cs` contains real world migration of assets (complex example)

To create custom migration:

- create new file in `Migration.Tool.Extensions/CommunityMigrations` (directory if you need more files for single migration)
- implement interface `Migration.Tool.KXP.Api.Services.CmsClass.IFieldMigration`
  - implement property rank, set number bellow 100 000 - for example 5000
  - implement method shall migrate (if method returns true, migration will be used)
  - implement `MigrateFieldDefinition`, where objective is to mutate argument `XElement field` that represents one particular field
  - implement `MigrateValue` where goal is to return new migrated value derived from `object? sourceValue`
- finally register in `Migration.Tool.Extensions/ServiceCollectionExtensions.cs` as `Transient` dependency into service collection. For example `services.AddTransient<IFieldMigration, AssetMigration>()`

## Custom class mappings for page types

Example code is found in `Migration.Tool.Extensions/ClassMappings/ClassMappingSample.cs`.

### Class remodeling sample

Example code is found in the method `AddSimpleRemodelingSample`.

The goal of this method is to take a **single data class** and change it to more suitable shape.

### Class merge sample

Example code is found in the method `AddClassMergeExample`.

The goal of this method is to take **multiple data classes** from the source instance and define their relation to a new class.

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

2. in similar fashion map other fields
3. if needed custom value conversion can be used

   ```csharp
   var startDate = m.BuildField("StartDate");
   startDate.SetFrom("_ET.Event1", "EventDateStart", true);
   // if needed use value conversion to adapt value
   startDate.ConvertFrom("_ET.Event2", "EventStartDateAsText", false,
       v => v?.ToString() is { } av && !string.IsNullOrWhiteSpace(av) ? DateTime.Parse(av) : null
   );
   startDate.WithFieldPatch(f => f.Caption = "Event start date");
   ```

4. register class mapping to dependency injection ocntainer

   ```csharp
   serviceCollection.AddSingleton<IClassMapping>(m);
   ```

### Inject and use reusable schema

Example code is found in the method `AddReusableSchemaIntegrationSample`

The goal of this method is to take a **single data class** and assign reusable schema.

## Custom widget property migrations

To create custom widget property migration:

- Create new file in `Migration.Tool.Extensions/CommunityMigrations` (directory if you need more files for single migration)
- Implement interface `Migration.Tool.KXP.Api.Services.CmsClass.IWidgetPropertyMigration`
  - Implement property rank, set number bellow 100 000 - for example 5000
  - Implement method shall migrate (if method returns true, migration will be used)
  - Implement `MigrateWidgetProperty`, where objective is to convert old JToken representing json value to new converted JToken value
- Register in `Migration.Tool.Extensions/ServiceCollectionExtensions.cs` as `Transient` dependency into service collection. For example `services.AddTransient<IWidgetPropertyMigration, WidgetPathSelectorMigration>()`

### Widget migration samples

- [Path selector migration](./DefaultMigrations/WidgetPathSelectorMigration.cs)
- [Page selector migration](./DefaultMigrations/WidgetPageSelectorMigration.cs)
- [File selector migration](./DefaultMigrations/WidgetFileMigration.cs)
