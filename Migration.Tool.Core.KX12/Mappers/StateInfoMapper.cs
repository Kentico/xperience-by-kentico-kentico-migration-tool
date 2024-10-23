using CMS.Globalization;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX12.Contexts;

namespace Migration.Tool.Core.KX12.Mappers;

public class StateInfoMapper : EntityMapperBase<KX12M.CmsState, StateInfo>
{
    public StateInfoMapper(ILogger<StateInfoMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : base(logger, pkContext, protocol)
    {
    }

    protected override StateInfo? CreateNewInstance(KX12M.CmsState source, MappingHelper mappingHelper, AddFailure addFailure)
        => StateInfo.New();

    protected override StateInfo MapInternal(KX12M.CmsState source, StateInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.StateName = source.StateName;
        target.StateDisplayName = source.StateDisplayName;
        target.StateLastModified = source.StateLastModified;
        target.StateGUID = source.StateGuid;
        target.StateCode = source.StateCode;

        if (mappingHelper.TranslateRequiredId<KX12M.CmsCountry>(k => k.CountryId, source.CountryId, out int countryId))
        {
            target.CountryID = countryId;
        }

        return target;
    }
}
