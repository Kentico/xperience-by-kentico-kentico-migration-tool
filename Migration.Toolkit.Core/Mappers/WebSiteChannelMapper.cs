namespace Migration.Toolkit.Core.Mappers;

using CMS.ContentEngine;
using CMS.Websites;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;

public record WebSiteChannelMappingResult(ChannelInfo ChannelInfo, WebsiteChannelInfo WebsiteChannelInfo);
public class WebSiteChannelMapper: EntityMapperBase<KX13.Models.CmsSite, WebSiteChannelMappingResult>
{
    public WebSiteChannelMapper(ILogger logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : base(logger, pkContext, protocol)
    {
    }

    protected override WebSiteChannelMappingResult? CreateNewInstance(KX13M.CmsSite source, MappingHelper mappingHelper, AddFailure addFailure)
    {
        throw new NotImplementedException();
    }

    protected override WebSiteChannelMappingResult MapInternal(KX13M.CmsSite source, WebSiteChannelMappingResult target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        throw new NotImplementedException();
    }
}