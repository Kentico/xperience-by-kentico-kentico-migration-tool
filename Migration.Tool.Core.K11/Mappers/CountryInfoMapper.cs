using CMS.Globalization;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.K11.Contexts;
using Migration.Tool.K11.Models;

namespace Migration.Tool.Core.K11.Mappers;

public class CountryInfoMapper(ILogger<CountryInfoMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : EntityMapperBase<CmsCountry, CountryInfo>(logger, pkContext, protocol)
{
    protected override CountryInfo? CreateNewInstance(CmsCountry source, MappingHelper mappingHelper, AddFailure addFailure)
        => CountryInfo.New();

    protected override CountryInfo MapInternal(CmsCountry source, CountryInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
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
