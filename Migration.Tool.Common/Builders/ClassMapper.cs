using CMS.DataEngine;
using CMS.FormEngine;
using Migration.Tool.Common.Abstractions;

namespace Migration.Tool.Common.Builders;

public interface IClassMapping
{
    public string TargetClassName { get; }

    public bool IsMatch(string sourceClassName);
    public void PatchTargetDataClass(DataClassInfo target);
    public ICollection<string> SourceClassNames { get; }
    public string PrimaryKey { get; }
    public IList<IFieldMapping> Mappings { get; }
    public IDictionary<string, Action<FormFieldInfo>> TargetFieldPatchers { get; }
    public IFieldMapping? GetMapping(string targetColumnName, string sourceClassName);

    public string? GetTargetFieldName(string sourceColumnName, string sourceClassName);
    public string GetSourceFieldName(string targetColumnName, string nodeClassClassName);
    public bool IsCategoryMapped(string sourceClassName, int categoryID);
    public void UseResusableSchema(string reusableSchemaName);
    public IList<string> ReusableSchemaNames { get; }

    /// <summary>
    /// as for now, supported only for custom tables
    /// </summary>
    public Type? MappingHandler { get; }
}

public interface IFieldMapping
{
    public bool IsTemplate { get; }
    public string SourceFieldName { get; }
    public string SourceClassName { get; }
    public string TargetFieldName { get; }
}

public record FieldMapping(string TargetFieldName, string SourceClassName, string SourceFieldName, bool IsTemplate) : IFieldMapping;

public record FieldMappingWithConversion(string TargetFieldName, string SourceClassName, string SourceFieldName, bool IsTemplate, Func<object?, IConvertorContext, object?> Converter) : IFieldMapping;

public class MultiClassMapping(string targetClassName, Action<DataClassInfo> classPatcher) : IClassMapping
{
    private readonly List<NewFieldDefinition> newFields = [];

    public void PatchTargetDataClass(DataClassInfo target)
    {
        classPatcher(target);

        if (newFields.Count > 0)
        {
            var formInfo = new FormInfo(target.ClassFormDefinition);

            foreach (var newField in newFields)
            {
                if (formInfo.GetFormField(newField.FieldName) != null)
                {
                    continue;
                }

                var formField = new FormFieldInfo
                {
                    Name = newField.FieldName,
                    DataType = newField.DataType,
                    AllowEmpty = newField.AllowEmpty,
                    Enabled = true,
                    Visible = true,
                    Size = newField.Size
                };

                formField.Settings ??= new System.Collections.Hashtable();

                newField.FieldPatcher?.Invoke(formField);
                formInfo.AddFormItem(formField);
            }
            target.ClassFormDefinition = formInfo.GetXmlDefinition();
        }
    }

    ICollection<string> IClassMapping.SourceClassNames => SourceClassNames;
    IList<IFieldMapping> IClassMapping.Mappings => Mappings;
    IDictionary<string, Action<FormFieldInfo>> IClassMapping.TargetFieldPatchers => TargetFieldPatchers;
    public IDictionary<string, Action<FormFieldInfo>> TargetFieldPatchers = new Dictionary<string, Action<FormFieldInfo>>();

    public IFieldMapping? GetMapping(string targetColumnName, string sourceClassName) => Mappings.SingleOrDefault(x => x.TargetFieldName.Equals(targetColumnName, StringComparison.InvariantCultureIgnoreCase) && x.SourceClassName.Equals(sourceClassName, StringComparison.InvariantCultureIgnoreCase));

    public string? GetTargetFieldName(string sourceColumnName, string sourceClassName) =>
        Mappings.SingleOrDefault(x => x.SourceFieldName.Equals(sourceColumnName, StringComparison.InvariantCultureIgnoreCase) && x.SourceClassName.Equals(sourceClassName, StringComparison.InvariantCultureIgnoreCase)) switch
        {
            { } m => m.TargetFieldName,
            _ => sourceColumnName
        };

    public string GetSourceFieldName(string targetColumnName, string sourceClassName) => GetMapping(targetColumnName, sourceClassName) switch
    {
        null => targetColumnName,
        FieldMapping fm => fm.SourceFieldName,
        FieldMappingWithConversion fm => fm.SourceFieldName,
        _ => targetColumnName
    };

    string IClassMapping.TargetClassName => targetClassName;

    public List<IFieldMapping> Mappings { get; } = [];
    public string PrimaryKey { get; set; } = null!;

    public HashSet<string> SourceClassNames = new(StringComparer.InvariantCultureIgnoreCase);

    public FieldBuilder BuildField(string targetFieldName)
    {
        if (Mappings.Any(x => x.TargetFieldName.Equals(targetFieldName, StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new InvalidOperationException($"Field mapping is already defined for field '{targetFieldName}'");
        }
        return new FieldBuilder(this, targetFieldName);
    }

    /// <summary>
    /// Adds a completely new field to the target class that doesn't map from any source field.
    /// Useful for adding fields like taxonomies, content references, or other new fields that don't exist in the source.
    /// </summary>
    /// <param name="fieldName">The name of the new field to add</param>
    /// <param name="dataType">The data type (e.g., "text", "contentitemreference", "integer")</param>
    /// <returns>FieldBuilder for fluent configuration with WithFieldPatch()</returns>
    public FieldBuilder AddField(string fieldName, string dataType = "text")
    {
        if (newFields.Any(x => x.FieldName.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new InvalidOperationException($"Field '{fieldName}' is already defined as a new field");
        }

        var newField = new NewFieldDefinition
        {
            FieldName = fieldName,
            DataType = dataType
        };

        newFields.Add(newField);

        return new FieldBuilder(this, fieldName, newField);
    }

    public bool IsMatch(string sourceClassName) => SourceClassNames.Contains(sourceClassName);

    public void UseResusableSchema(string reusableSchemaName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reusableSchemaName);

        if (reusableSchemaNames.Contains(reusableSchemaName))
        {
            throw new Exception($"The reusable Schema {reusableSchemaName} is already assigned");
        }
        reusableSchemaNames.Add(reusableSchemaName);
    }

    private MultiClassMappingCategoryFilter categoryFilter = (_, _) => true;
    public void FilterCategories(MultiClassMappingCategoryFilter filter) => categoryFilter = filter;
    public bool IsCategoryMapped(string sourceClassName, int categoryID) => categoryFilter(sourceClassName, categoryID);

    private readonly IList<string> reusableSchemaNames = [];
    IList<string> IClassMapping.ReusableSchemaNames => reusableSchemaNames;

    #region Handlers

    public Type? MappingHandler { get; private set; }

    public void SetHandler<T>() where T : IClassMappingHandler => MappingHandler = typeof(T);

    #endregion
}

public delegate bool MultiClassMappingCategoryFilter(string sourceClassName, int categoryID);

public interface IConvertorContext;
public record ConvertorTreeNodeContext(Guid NodeGuid, int NodeSiteId, int? DocumentId, bool MigratingFromVersionHistory) : IConvertorContext;
public record ConvertorCustomTableContext() : IConvertorContext;

public interface IMappingHandlerContext;

public record CustomTableMappingHandlerContext(Dictionary<string, object?> Values, DataClassInfo TargetClassInfo, string SourceClassName);

/// <summary>
/// Defines a new field to be added to the target class schema.
/// Used by AddField() to create fields that are independent of source data mappings.
/// </summary>
public class NewFieldDefinition
{
    public string FieldName { get; set; } = null!;
    public string DataType { get; set; } = "text";
    public bool AllowEmpty { get; set; } = true;
    public int Size { get; set; } = 0;
    public Action<FormFieldInfo>? FieldPatcher { get; set; }
}

public class FieldBuilder
{
    private readonly MultiClassMapping multiClassMapping;
    private readonly string targetFieldName;
    private readonly NewFieldDefinition? newFieldDefinition;
    private IFieldMapping? currentFieldMapping;

    public FieldBuilder(MultiClassMapping multiClassMapping, string targetFieldName, NewFieldDefinition? newFieldDefinition = null)
    {
        this.multiClassMapping = multiClassMapping;
        this.targetFieldName = targetFieldName;
        this.newFieldDefinition = newFieldDefinition;
    }

    public FieldBuilder SetFrom(string sourceClassName, string sourceFieldName, bool isTemplate = false)
    {
        if (newFieldDefinition != null)
        {
            throw new InvalidOperationException($"Cannot set source mapping for new field '{targetFieldName}'. New fields are independent and don't map from source fields.");
        }

        currentFieldMapping = new FieldMapping(targetFieldName, sourceClassName, sourceFieldName, isTemplate);
        multiClassMapping.Mappings.Add(currentFieldMapping);
        multiClassMapping.SourceClassNames.Add(sourceClassName);
        return this;
    }

    public FieldBuilder ConvertFrom(string sourceClassName, string sourceFieldName, bool isTemplate, Func<object?, IConvertorContext, object?> converter)
    {
        if (newFieldDefinition != null)
        {
            throw new InvalidOperationException($"Cannot set source mapping for new field '{targetFieldName}'. New fields are independent and don't map from source fields.");
        }

        currentFieldMapping = new FieldMappingWithConversion(targetFieldName, sourceClassName, sourceFieldName, isTemplate, converter);
        multiClassMapping.Mappings.Add(currentFieldMapping);
        multiClassMapping.SourceClassNames.Add(sourceClassName);
        return this;
    }

    public FieldBuilder WithFieldPatch(Action<FormFieldInfo> fieldInfoPatcher)
    {
        if (newFieldDefinition != null)
        {
            if (newFieldDefinition.FieldPatcher != null)
            {
                throw new InvalidOperationException($"Field patcher already defined for new field '{targetFieldName}'");
            }
            newFieldDefinition.FieldPatcher = fieldInfoPatcher;
        }
        else
        {
            if (!multiClassMapping.TargetFieldPatchers.TryAdd(targetFieldName, fieldInfoPatcher))
            {
                throw new InvalidOperationException($"Target field mapper can be dined only once for each field, field '{targetFieldName}' has one already defined");
            }
        }
        return this;
    }

    public MultiClassMapping AsPrimaryKey()
    {
        multiClassMapping.PrimaryKey = targetFieldName;
        return multiClassMapping;
    }
}
