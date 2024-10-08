## Custom migrations

Samples:
- `Migration.Tool.Extensions/CommunityMigrations/SampleTextMigration.cs` contains simplest implementation for migration of text fields
- `Migration.Tool.Extensions/DefaultMigrations/AssetMigration.cs` contains real world migration of assets (complex example)

To create custom migration:
- create new file in `Migration.Tool.Extensions/CommunityMigrations` (directory if you need more files for single migration)
- implement interface `Migration.Toolkit.KXP.Api.Services.CmsClass.IFieldMigration`
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