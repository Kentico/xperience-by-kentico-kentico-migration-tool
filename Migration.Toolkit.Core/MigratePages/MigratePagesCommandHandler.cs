// using MediatR;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using Migration.Toolkit.Core.Abstractions;
// using Migration.Toolkit.Core.Contexts;
// using Migration.Toolkit.KX13.Context;
// using Migration.Toolkit.KX13.Models;
// using Migration.Toolkit.KXO.Context;
//
// namespace Migration.Toolkit.Core.MigratePages;
//
// public class MigratePagesCommandHandler: IRequestHandler<MigratePagesCommand, GenericCommandResult>, IDisposable
// {
//     private readonly ILogger<MigratePagesCommandHandler> _logger;
//     private readonly IDbContextFactory<KxoContext> _kxoContextFactory;
//     private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
//     private readonly IEntityMapper<CmsDocument, KXO.Models.CmsDocument> _documentMapper;
//     private readonly PrimaryKeyMappingContext _pkMappingContext;
//     
//     private KxoContext _kxoContext;
//
//     public MigratePagesCommandHandler(
//         ILogger<MigratePagesCommandHandler> logger,
//         IDbContextFactory<KXO.Context.KxoContext> kxoContextFactory,
//         IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory,
//         IEntityMapper<KX13.Models.CmsDocument, KXO.Models.CmsDocument> documentMapper,
//         PrimaryKeyMappingContext pkMappingContext
//         )
//     {
//         _logger = logger;
//         _kxoContextFactory = kxoContextFactory;
//         _kx13ContextFactory = kx13ContextFactory;
//         _documentMapper = documentMapper;
//         _pkMappingContext = pkMappingContext;
//         _kxoContext = kxoContextFactory.CreateDbContext();
//     }
//     
//     public async Task<GenericCommandResult> Handle(MigratePagesCommand request, CancellationToken cancellationToken)
//     {
//         await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);
//         kx13Context.CmsWebFarmServers;
//         await foreach (var kx13Document in kx13Context.CmsDocuments)
//         {
//             // _logger.LogTrace("Migrating pages {userName} with UserGuid {userGuid}", kx13Document.CmsAttachments);
//             var doc = _kxoContext.CmsDocuments.FirstOrDefaultAsync(x=>x.)
//                 
//                 
//         }
//     }
//
//     public void Dispose()
//     {
//         _kxoContext.Dispose();
//     }
// }