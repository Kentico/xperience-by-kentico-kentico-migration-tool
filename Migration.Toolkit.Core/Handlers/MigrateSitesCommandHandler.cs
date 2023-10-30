namespace Migration.Toolkit.Core.Handlers;

using CMS.ContentEngine;
using CMS.Websites;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Mappers;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXP.Context;

// ReSharper disable once UnusedType.Global
public class MigrateSitesCommandHandler: IRequestHandler<MigrateSitesCommand, CommandResult>, IDisposable
{
    private readonly ILogger<MigrateSitesCommandHandler> _logger;
    private readonly IDbContextFactory<KxpContext> _kxpContextFactory;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly IEntityMapper<KX13.Models.CmsSite, WebSiteChannelMappingResult> _channelMapper;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;
    private readonly IImportService _importService;

    private KxpContext _kxpContext;

    public MigrateSitesCommandHandler(
        ILogger<MigrateSitesCommandHandler> logger,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        IEntityMapper<KX13.Models.CmsSite, WebSiteChannelMappingResult> channelMapper,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol,
        IImportService importService
    )
    {
        _logger = logger;
        _kxpContextFactory = kxpContextFactory;
        _kx13ContextFactory = kx13ContextFactory;
        _channelMapper = channelMapper;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _protocol = protocol;
        _importService = importService;
        _kxpContext = _kxpContextFactory.CreateDbContext();
    }

    public async Task<CommandResult> Handle(MigrateSitesCommand request, CancellationToken cancellationToken)
    {


        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var kx13CmsSite in kx13Context.CmsSites)
        {
            _protocol.FetchedSource(kx13CmsSite);
            _logger.LogTrace("Migrating site {SiteName} with SiteGuid {SiteGuid}", kx13CmsSite.SiteName, kx13CmsSite.SiteGuid);

            var targetSiteId = _primaryKeyMappingContext.MapFromSourceOrNull<CmsSite>(s => s.SiteId, kx13CmsSite.SiteId);
            if (targetSiteId is null)
            {
                // TODO tk: 2022-05-26 add site guid mapping
                _logger.LogWarning("Site '{SiteName}' with Guid {Guid} migration skipped", kx13CmsSite.SiteName, kx13CmsSite.SiteGuid);
                continue;
            }

            var channelInfo = ChannelInfoProvider.ProviderObject.Get(kx13CmsSite.SiteGuid);
            var websiteChannelInfo = WebsiteChannelInfoProvider.ProviderObject.Get(kx13CmsSite.SiteGuid);
            var xbkTarget = new WebSiteChannelMappingResult(channelInfo, websiteChannelInfo);
            _protocol.FetchedTarget(xbkTarget);

            var mapped = _channelMapper.Map(kx13CmsSite, xbkTarget);
            _protocol.MappedTarget(mapped);

            // TODO tomas.krch: 2023-10-30 refactor needed,
            // var observer = _importService.StartImport(new UmtModel[]
            // {
            //     // TODO tomas.krch: 2023-10-30 channel import model
            //     new DataClassModel
            //     {
            //
            //     }
            // }.AsEnumerable(), new ImporterContext("[to be removed]", "[to be removed]"), null);
            // await observer.ImportCompletedTask;

            if (mapped is { Success : true } result)
            {
                var (webSiteChannelMappingResult, newInstance) = result;
                ArgumentNullException.ThrowIfNull(webSiteChannelMappingResult, nameof(webSiteChannelMappingResult));

                (channelInfo, websiteChannelInfo) = webSiteChannelMappingResult;

                ChannelInfoProvider.ProviderObject.Set(channelInfo);
                WebsiteChannelInfoProvider.ProviderObject.Set(websiteChannelInfo);

                _protocol.Success(kx13CmsSite, webSiteChannelMappingResult, mapped);
                _logger.LogEntitySetAction(newInstance, webSiteChannelMappingResult);

                // TODO tomas.krch: 2023-10-30 might be needed to set reference to web site channel id
                _primaryKeyMappingContext.SetMapping<CmsSite>(r => r.SiteId, kx13CmsSite.SiteId, channelInfo.ChannelID);
            }
        }

        return new GenericCommandResult();
    }

    public void Dispose()
    {
        _kxpContext.Dispose();
    }
}