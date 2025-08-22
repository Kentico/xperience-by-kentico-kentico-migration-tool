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

public class MultiClassMapping(string targetClassName, Action<DataClassInfo> classPatcher) : IClassMapping
{
    public void PatchTargetDataClass(DataClassInfo target) => classPatcher(target);

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

public class FieldBuilder(MultiClassMapping multiClassMapping, string targetFieldName)
{
    private IFieldMapping? currentFieldMapping;

    public FieldBuilder SetFrom(string sourceClassName, string sourceFieldName, bool isTemplate = false)
    {
        currentFieldMapping = new FieldMapping(targetFieldName, sourceClassName, sourceFieldName, isTemplate);
        multiClassMapping.Mappings.Add(currentFieldMapping);
        multiClassMapping.SourceClassNames.Add(sourceClassName);
        return this;
    }

    public FieldBuilder ConvertFrom(string sourceClassName, string sourceFieldName, bool isTemplate, Func<object?, IConvertorContext, object?> converter)
    {
        currentFieldMapping = new FieldMappingWithConversion(targetFieldName, sourceClassName, sourceFieldName, isTemplate, converter);
        multiClassMapping.Mappings.Add(currentFieldMapping);
        multiClassMapping.SourceClassNames.Add(sourceClassName);
        return this;
    }

    public FieldBuilder WithFieldPatch(Action<FormFieldInfo> fieldInfoPatcher)
    {
        if (!multiClassMapping.TargetFieldPatchers.TryAdd(targetFieldName, fieldInfoPatcher))
        {
            throw new InvalidOperationException($"Target field mapper can be dined only once for each field, field '{targetFieldName}' has one already defined");
        }
        return this;
    }

    public MultiClassMapping AsPrimaryKey()
    {
        multiClassMapping.PrimaryKey = targetFieldName;
        return multiClassMapping;
    }
}
