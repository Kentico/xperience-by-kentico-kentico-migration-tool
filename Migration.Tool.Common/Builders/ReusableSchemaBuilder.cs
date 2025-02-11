using CMS.FormEngine;

namespace Migration.Tool.Common.Builders;

public record SourceFieldIdentifier(string ClassName, string FieldName);

public interface IReusableSchemaBuilder
{
    string SchemaName { get; }
    string SchemaDisplayName { get; }
    string? SchemaDescription { get; }
    IList<IReusableFieldBuilder> FieldBuilders { get; }
    IReusableFieldBuilder BuildField(string targetFieldName);
    void AssertIsValid();
}

public class ReusableSchemaBuilder(string schemaName, string displayName, string? schemaDescription) : IReusableSchemaBuilder
{
    public string SchemaName { get; } = schemaName;
    public string SchemaDisplayName { get; } = displayName;
    public string? SchemaDescription { get; } = schemaDescription;
    public IList<IReusableFieldBuilder> FieldBuilders { get; set; } = [];

    public IReusableFieldBuilder BuildField(string targetFieldName)
    {
        if (FieldBuilders.Any(fb => fb.TargetFieldName.Equals(targetFieldName, StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new InvalidOperationException($"Target field '{targetFieldName}' already exists");
        }

        var newFieldBuilder = new ReusableFieldBuilder(targetFieldName);
        FieldBuilders.Add(newFieldBuilder);
        return newFieldBuilder;
    }

    public void AssertIsValid()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(SchemaName, nameof(SchemaName));
        ArgumentException.ThrowIfNullOrWhiteSpace(SchemaDisplayName, nameof(SchemaDisplayName));

        foreach (var reusableFieldBuilder in FieldBuilders)
        {
            reusableFieldBuilder.AssertIsValid();
        }
    }
}

public interface IReusableFieldBuilder
{
    string TargetFieldName { get; }
    Func<FormFieldInfo>? Factory { get; }
    SourceFieldIdentifier? SourceFieldIdentifier { get; }

    void AssertIsValid();
    IReusableFieldBuilder WithFactory(Func<FormFieldInfo> formFieldFactory);
    IReusableFieldBuilder CreateFrom(string sourceClassName, string sourceFieldName);
}

public class ReusableFieldBuilder(string targetFieldName) : IReusableFieldBuilder
{
    public string TargetFieldName { get; } = targetFieldName;
    public Func<FormFieldInfo>? Factory { get; private set; }
    public SourceFieldIdentifier? SourceFieldIdentifier { get; private set; }

    public void AssertIsValid()
    {
        if (Factory == null && SourceFieldIdentifier == null)
        {
            throw new NullReferenceException($"Reusable field builder is not valid for field '{TargetFieldName}' call 'WithFactory' or 'CreateFrom' to define source of field value");
        }

        if (Factory != null && SourceFieldIdentifier != null)
        {
            throw new InvalidOperationException($"Reusable field builder is not valid for field '{TargetFieldName}' you cannot call both 'WithFactory' and 'CreateFrom' to define source of field value");
        }
    }

    public IReusableFieldBuilder WithFactory(Func<FormFieldInfo> formFieldFactory)
    {
        ArgumentNullException.ThrowIfNull(formFieldFactory);
        Factory = formFieldFactory;
        return this;
    }

    public IReusableFieldBuilder CreateFrom(string sourceClassName, string sourceFieldName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceClassName, nameof(sourceClassName));
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceFieldName, nameof(sourceFieldName));

        SourceFieldIdentifier = new SourceFieldIdentifier(sourceClassName, sourceFieldName);
        return this;
    }
}
