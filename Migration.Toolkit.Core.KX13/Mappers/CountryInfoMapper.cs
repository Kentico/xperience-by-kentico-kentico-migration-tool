using CMS.Globalization;

using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX13.Contexts;

namespace Migration.Toolkit.Core.KX13.Mappers;

public class CountryInfoMapper : EntityMapperBase<KX13M.CmsCountry, CountryInfo>
{
    public CountryInfoMapper(ILogger<CountryInfoMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : base(logger, pkContext, protocol)
    {
    }

    protected override CountryInfo? CreateNewInstance(KX13M.CmsCountry source, MappingHelper mappingHelper, AddFailure addFailure)
        => CountryInfo.New();

    protected override CountryInfo MapInternal(KX13M.CmsCountry source, CountryInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.CountryName = source.CountryName;
        target.CountryDisplayName = source.CountryDisplayName;
        target.CountryGUID = source.CountryGuid;
        target.CountryLastModified = source.CountryLastModified;
        target.CountryThreeLetterCode = source.CountryThreeLetterCode;
        target.CountryTwoLetterCode = source.CountryTwoLetterCode;
        return target;
    }
}
