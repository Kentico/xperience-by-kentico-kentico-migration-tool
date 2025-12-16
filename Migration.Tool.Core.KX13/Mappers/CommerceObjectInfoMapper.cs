using CMS.DataEngine;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Constants;
using Migration.Tool.Core.KX13.Contexts;
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
    protected abstract void MapCoreFields(TSourceEntity source, TTargetEntity target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure);

    protected virtual void MapSystemAndCustomFields(TSourceEntity source, TTargetEntity target, IEnumerable<CustomizedFieldInfo> customizedFieldInfos, string systemFieldPrefix)
    {
        var sourceModel = GetSourceModel(source);

        foreach (var customizedFieldInfo in customizedFieldInfos)
        {
            string fieldName = customizedFieldInfo.FieldName;

            // Strip the system field prefix (if present - field was system in source instance) to find the original source field name
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

        var customizedFieldInfos = kxpClassFacade.GetCustomizedFieldInfosAll(GetTargetObjectClassName());

        string systemFieldPrefix = CommerceConstants.GetSystemFieldPrefix(toolConfiguration);

        MapSystemAndCustomFields(source, target, customizedFieldInfos, systemFieldPrefix);

        return target;
    }

    protected abstract TSourceModel GetSourceModel(TSourceEntity source);

    protected abstract string GetTargetObjectClassName();
}
