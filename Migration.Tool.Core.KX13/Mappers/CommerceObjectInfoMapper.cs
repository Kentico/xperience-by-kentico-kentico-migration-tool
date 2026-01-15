using CMS.DataEngine;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.Core.KX13.Helpers;
using Migration.Tool.KXP.Api;

namespace Migration.Tool.Core.KX13.Mappers;

public abstract class CommerceObjectInfoMapper<TSourceEntity, TTargetEntity, TSourceModel>(
    ILogger logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    KxpClassFacade kxpClassFacade,
    ToolConfiguration toolConfiguration)
    : EntityMapperBase<TSourceEntity, TTargetEntity>(logger, primaryKeyMappingContext, protocol)
    where TTargetEntity : BaseInfo
{
    protected abstract string TargetObjectClassName { get; }

    /// <summary>
    /// Maps the core fields from the source entity to the target entity.
    /// Core fields are the essential properties that define the identity and primary
    /// business semantics of the commerce object (for example, identifiers, GUIDs,
    /// codes, names, and key relationships) that cannot be mapped generically.
    /// </summary>
    /// <remarks>
    /// This method is the main extension point for derived mappers to implement
    /// object-specific mapping logic. It is invoked by <see cref="MapInternal"/> before
    /// <see cref="MapSystemAndCustomFields(TSourceEntity,TTargetEntity,IEnumerable{CustomizedFieldInfo},string)"/>.
    /// Implementations should only map fields that are not covered by
    /// <see cref="MapSystemAndCustomFields(TSourceEntity,TTargetEntity,IEnumerable{CustomizedFieldInfo},string)"/>,
    /// which handles system and custom fields discovered from the class metadata.
    /// </remarks>
    /// <param name="source">The source entity instance being migrated.</param>
    /// <param name="target">The target <see cref="BaseInfo"/> instance to which core fields are mapped.</param>
    /// <param name="newInstance">
    /// True if the target represents a newly created instance; false if mapping onto an existing instance
    /// (for example, updates where some fields may already be populated).
    /// </param>
    /// <param name="mappingHelper">Helper providing common mapping utilities and conversions.</param>
    /// <param name="addFailure">
    /// Delegate to record mapping failures or validation issues encountered while mapping core fields.
    /// </param>
    protected abstract void MapCoreFields(TSourceEntity source, TTargetEntity target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure);


    /// <summary>
    /// Maps system and custom fields from the source model to the target entity.
    /// </summary>
    /// <remarks>
    /// This method iterates over all customized fields for the target object class and copies
    /// corresponding values from the source model. When a target field name starts with
    /// <paramref name="systemFieldPrefix"/>, it is treated as a migrated system field whose name
    /// was prefixed during the <c>MigrateCommerceClass</c> process; the prefix is stripped to
    /// resolve the original source property name. Custom fields (without the prefix) are mapped
    /// directly by name. Derived classes can override this method to customize how individual
    /// fields are resolved or transformed before being assigned to the target entity.
    /// </remarks>
    /// <param name="source">The source entity instance from which data is being migrated.</param>
    /// <param name="target">The target <see cref="BaseInfo"/> entity to which field values are assigned.</param>
    /// <param name="customizedFieldInfos">
    /// The collection of customized field metadata for the target object class, including both
    /// migrated system fields (with prefix) and custom fields.
    /// </param>
    /// <param name="systemFieldPrefix">
    /// The prefix applied to migrated system fields in the target schema; used to determine the
    /// corresponding source field name by removing the prefix when present.
    /// </param>
    protected virtual void MapSystemAndCustomFields(TSourceEntity source, TTargetEntity target, IEnumerable<CustomizedFieldInfo> customizedFieldInfos, string systemFieldPrefix)
    {
        var sourceModel = GetSourceModel(source);

        foreach (var customizedFieldInfo in customizedFieldInfos)
        {
            string fieldName = customizedFieldInfo.FieldName;

            // If the field name in the target has the system field prefix, it corresponds to a system field that was migrated and prefixed during the MigrateCommerceClass process.
            // In this case, strip the prefix to get the original source field name.
            // For custom fields (without the prefix in the target), the field name maps directly to the custom field in the source.
            string sourceFieldName = fieldName.StartsWith(systemFieldPrefix, StringComparison.OrdinalIgnoreCase)
                ? fieldName[systemFieldPrefix.Length..]
                : fieldName;

            if (ReflectionHelper<TSourceModel>.TryGetPropertyValue(sourceModel, sourceFieldName, StringComparison.InvariantCultureIgnoreCase, out object? value))
            {
                target.SetValue(fieldName, value);
            }
        }
    }

    protected override TTargetEntity MapInternal(TSourceEntity source, TTargetEntity target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        MapCoreFields(source, target, newInstance, mappingHelper, addFailure);

        var customizedFieldInfos = kxpClassFacade.GetCustomizedFieldInfosAll(TargetObjectClassName);

        string systemFieldPrefix = CommerceHelper.GetSystemFieldPrefix(toolConfiguration);

        MapSystemAndCustomFields(source, target, customizedFieldInfos, systemFieldPrefix);

        return target;
    }

    protected abstract TSourceModel GetSourceModel(TSourceEntity source);
}
