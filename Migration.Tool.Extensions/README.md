## Custom migrations

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

examples are `Migration.Tool.Extensions/ClassMappings/ClassMappingSample.cs`

### Class remodeling sample

demonstrated in method `AddSimpleRemodelingSample`, goal is to take single data class and change it to more suitable shape.

### Class merge sample

demonstrated in method `AddClassMergeExample`, goal is to take multiple data classes from source instance and define their relation to new class

lets define new class:
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

define new primary key:
```csharp
m.BuildField("EventID").AsPrimaryKey();
```

and finally lets define relations to fields:

1) build field title 
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

2) in similar fashion map other fields

3) if needed custom value conversion can be used
```csharp
var startDate = m.BuildField("StartDate");
startDate.SetFrom("_ET.Event1", "EventDateStart", true);
// if needed use value conversion to adapt value
startDate.ConvertFrom("_ET.Event2", "EventStartDateAsText", false,
    v => v?.ToString() is { } av && !string.IsNullOrWhiteSpace(av) ? DateTime.Parse(av) : null
);
startDate.WithFieldPatch(f => f.Caption = "Event start date");
```

4) register class mapping to dependency injection ocntainer
```csharp
serviceCollection.AddSingleton<IClassMapping>(m); 
```

### Inject and use reusable schema

demonstrated in method `AddReusableSchemaIntegrationSample`, goal is to take single data class and assign reusable schema.

## Custom widget migrations

Custom widget migration allows you to remodel the original widget as a new widget type. The prominent operations are 
changing the target widget type and recombining the original properties.

To create custom widget migration:
- create new file in `Migration.Tool.Extensions/CommunityMigrations` (directory if you need more files for single migration)
- implement interface `Migration.Tool.KXP.Api.Services.CmsClass.IWidgetMigration`
  - implement property `Rank`, set number bellow 100 000 - for example 5000. Rank determines the order by which the migrations are tested to be eligible via the `ShallMigrate` method
  - implement method `ShallMigrate`. If method returns true, migration will be used. This method receives a context, by which you can decide - typically by the original widget's type
  - implement `MigrateWidget`, where objective is to convert old JToken representing the widget's JSON to new converted JToken value
    - Widget property migration will still be applied after your custom widget migration
    - In the following cases, you must explicitly specify the property migration to be used, via `PropertyMigrations` in returned value (because it can't be infered from the original widget)
      - If you add a new property. That includes renaming an original property.
      - In the special case when you introduce a new property whose name overlaps with original property. Otherwise the migration infered from the original property would be used
        - If your new property is not supposed to be subject to property migrations and the original one was, explicitly specify `WidgetNoOpMigration` for this property
    - You can also override the property migration of an original property if that suits your case
    
- finally register in `Migration.Tool.Extensions/ServiceCollectionExtensions.cs` as `Transient` dependency into service collection. For example `services.AddTransient<IWidgetMigration, YourMigrationClass>()`

Samples:
- [Sample BannerWidget migration](./CommunityMigrations/SampleWidgetMigration.cs)

## Custom widget property migrations

To create custom widget property migration:
- create new file in `Migration.Tool.Extensions/CommunityMigrations` (directory if you need more files for single migration)
- implement interface `Migration.Tool.KXP.Api.Services.CmsClass.IWidgetPropertyMigration`
  - implement property `Rank`, set number bellow 100 000 - for example 5000. Rank determines the order by which the migrations are tested to be eligible via the `ShallMigrate` method
  - implement method `ShallMigrate` (if method returns true, migration will be used)
  - implement `MigrateWidgetProperty`, where objective is to convert old JToken representing json value to new converted JToken value  
- finally register in `Migration.Tool.Extensions/ServiceCollectionExtensions.cs` as `Transient` dependency into service collection. For example `services.AddTransient<IWidgetPropertyMigration, WidgetPathSelectorMigration>()`

Samples:
- [Path selector migration](./DefaultMigrations/WidgetPathSelectorMigration.cs)
- [Page selector migration](./DefaultMigrations/WidgetPageSelectorMigration.cs)
- [File selector migration](./DefaultMigrations/WidgetFileMigration.cs)