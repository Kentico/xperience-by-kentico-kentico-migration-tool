using CMS.DataEngine;
using CMS.FormEngine;
using Migration.Tool.Common.Abstractions;

namespace Migration.Tool.Common.Builders;

public interface IClassMapping
{
    string TargetClassName { get; }

    bool IsMatch(string sourceClassName);
    void PatchTargetDataClass(DataClassInfo target);
    ICollection<string> SourceClassNames { get; }
    string PrimaryKey { get; }
    IList<IFieldMapping> Mappings { get; }
    IDictionary<string, Action<FormFieldInfo>> TargetFieldPatchers { get; }
    IFieldMapping? GetMapping(string targetColumnName, string sourceClassName);

    string? GetTargetFieldName(string sourceColumnName, string sourceClassName);
    string GetSourceFieldName(string targetColumnName, string nodeClassClassName);
    bool IsCategoryMapped(string sourceClassName, int categoryID);
    void UseResusableSchema(string reusableSchemaName);
    IList<string> ReusableSchemaNames { get; }

    /// <summary>
    /// as for now, supported only for custom tables
    /// </summary>
    Type? MappingHandler { get; }
}

public interface IFieldMapping
{
    bool IsTemplate { get; }
    string SourceFieldName { get; }
    string SourceClassName { get; }
    string TargetFieldName { get; }
}

public record FieldMapping(string TargetFieldName, string SourceClassName, string SourceFieldName, bool IsTemplate) : IFieldMapping;

public record FieldMappingWithConversion(string TargetFieldName, string SourceClassName, string SourceFieldName, bool IsTemplate, Func<object?, IConvertorContext, object?> Converter) : IFieldMapping;

public class MultiClassMapping(string targetClassName, Action<DataClassInfo>? classPatcher = null) : IClassMapping
{
    public readonly List<NewFieldDefinition> NewFields = [];

    public void PatchTargetDataClass(DataClassInfo target)
    {
        classPatcher?.Invoke(target);

        if (NewFields.Count > 0)
        {
            var formInfo = new FormInfo(target.ClassFormDefinition);

            foreach (var newField in NewFields)
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
/// Used by BuildField().WithoutSource() to create fields that are independent of source data mappings.
/// </summary>
public class NewFieldDefinition
{
    public string FieldName { get; set; } = null!;
    public string DataType { get; set; } = "text";
    public bool AllowEmpty { get; set; } = true;
    public int Size { get; set; } = 0;
}

public class FieldBuilder(MultiClassMapping multiClassMapping, string targetFieldName)
{
    private IFieldMapping? currentFieldMapping;

    /// <summary>
    /// Maps the target field from a source field in a specified source class.
    /// </summary>
    /// <param name="sourceClassName">The name of the source class</param>
    /// <param name="sourceFieldName">The name of the source field</param>
    /// <param name="isTemplate">Whether this is a template field mapping</param>
    /// <returns>The FieldBuilder instance for method chaining</returns>
    public FieldBuilder SetFrom(string sourceClassName, string sourceFieldName, bool isTemplate = false)
    {
        currentFieldMapping = new FieldMapping(targetFieldName, sourceClassName, sourceFieldName, isTemplate);
        multiClassMapping.Mappings.Add(currentFieldMapping);
        multiClassMapping.SourceClassNames.Add(sourceClassName);
        return this;
    }

    /// <summary>
    /// Maps the target field from a source field in a specified source class with a conversion function.
    /// </summary>
    /// <param name="sourceClassName">The name of the source class</param>
    /// <param name="sourceFieldName">The name of the source field</param>
    /// <param name="isTemplate">Whether this is a template field mapping</param>
    /// <param name="converter">The conversion function to apply to the source field value</param>
    /// <returns>The FieldBuilder instance for method chaining</returns>
    public FieldBuilder ConvertFrom(string sourceClassName, string sourceFieldName, bool isTemplate, Func<object?, IConvertorContext, object?> converter)
    {
        currentFieldMapping = new FieldMappingWithConversion(targetFieldName, sourceClassName, sourceFieldName, isTemplate, converter);
        multiClassMapping.Mappings.Add(currentFieldMapping);
        multiClassMapping.SourceClassNames.Add(sourceClassName);
        return this;
    }

    /// <summary>
    /// Defines a new field to be added to the target class schema that does not map to any source field.
    /// </summary>
    /// <param name="dataType">The data type of the new field (default: "text")</param>
    /// <param name="allowEmpty">Whether the new field allows empty values (default: true)</param>
    /// <param name="size">The size of the new field (default: 0)</param>
    /// <returns>The current FieldBuilder instance for method chaining</returns>
    public FieldBuilder WithoutSource(string dataType = "text", bool allowEmpty = true, int size = 0)
    {
        multiClassMapping.NewFields.Add(new NewFieldDefinition
        {
            FieldName = targetFieldName,
            DataType = dataType,
            AllowEmpty = allowEmpty,
            Size = size
        });
        return this;
    }

    /// <summary>
    /// Adds a patch action to modify the target field's form field information.
    /// </summary>
    /// <param name="fieldInfoPatcher">The action to modify the form field information</param>
    /// <returns>The current FieldBuilder instance for method chaining</returns>
    public FieldBuilder WithFieldPatch(Action<FormFieldInfo> fieldInfoPatcher)
    {
        if (!multiClassMapping.TargetFieldPatchers.TryAdd(targetFieldName, fieldInfoPatcher))
        {
            throw new InvalidOperationException($"Target field mapper can be defined only once for each field, field '{targetFieldName}' has one already defined");
        }
        return this;
    }

    /// <summary>
    /// Marks the target field as the primary key for the target class.
    /// </summary>
    /// <returns>The MultiClassMapping instance for method chaining</returns>
    public MultiClassMapping AsPrimaryKey()
    {
        multiClassMapping.PrimaryKey = targetFieldName;
        return multiClassMapping;
    }
}
