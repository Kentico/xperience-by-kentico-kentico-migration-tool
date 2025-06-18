using System.Collections.Concurrent;
using System.Diagnostics;
using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.Core;
using CMS.Core.Internal;
using CMS.DataEngine;
using CMS.DataEngine.Internal;
using CMS.DataEngine.Query;
using CMS.FormEngine;
using CMS.Websites;
using CMS.Websites.Internal;
using CMS.Websites.Routing.Internal;
using HotChocolate.Types;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Builders;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Common.Model;
using Migration.Tool.KXP.Api.Auxiliary;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Helpers;
using Migration.Tool.Source.Mappers;
using Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Providers;
using Migration.Tool.Source.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Handlers;
// ReSharper disable once UnusedType.Global
public class MigratePagesCommandHandler(
    ILogger<MigratePagesCommandHandler> logger,
    ToolConfiguration toolConfiguration,
    IProtocol protocol,
    IImporter importer,
    IUmtMapper<CmsTreeMapperSource> mapper,
    ModelFacade modelFacade,
    DeferredPathService deferredPathService,
    DeferredTreeNodesService deferredTreeNodesService,
    SpoiledGuidContext spoiledGuidContext,
    SourceInstanceContext sourceInstanceContext,
    ClassMappingProvider classMappingProvider,
    IEnumerable<IReusableSchemaBuilder> reusableSchemaBuilders
)
    : IRequestHandler<MigratePagesCommand, CommandResult>
{
    private const string ClassCmsRoot = "CMS.Root";

    private EntityConfiguration? classEntityConfiguration;
    private Dictionary<string, Guid>? cultureCodeToLanguageGuid;
    public async Task<CommandResult> Handle(MigratePagesCommand request, CancellationToken cancellationToken)
    {
        classEntityConfiguration = toolConfiguration.EntityConfigurations.GetEntityConfiguration<DataClassInfo>();
        cultureCodeToLanguageGuid = modelFacade.SelectAll<ICmsCulture>()
            .ToDictionary(c => c.CultureCode, c => c.CultureGUID, StringComparer.InvariantCultureIgnoreCase);

        await MigratePages();

        await ExecDeferredVisualBuilderPatch();

        await MigrateRedirects();

        return new GenericCommandResult();
    }

    private async Task MigrateRedirects()
    {
        if (modelFacade.SelectVersion() is { Major: 13 })
        {
            Dictionary<string, (Guid ContentItemGuid, ContentLanguageInfo LanguageInfo)> pathToXbykPage = [];
            List<(Guid DocumentGuid, Guid SiteGuid, ContentLanguageInfo LanguageInfo, Guid ContentItemGuid, string RedirectUrl)> sourceInstanceRedirects = [];

            var sites = modelFacade.GetMigratedSites();
            // Walk all pages in source instance. Gather their redirections & mapping from their original URLs to webpage entities in target instance
            foreach (var ksSite in sites)
            {
                var channelInfo = ChannelInfoProvider.ProviderObject.Get(ksSite.SiteGUID);

                var ksTrees = modelFacade.Select<ICmsTree>(
                    "NodeSiteId = @siteId",
                    "NodeLevel, NodeOrder",
                    new SqlParameter("siteId", ksSite.SiteID)
                );

                foreach (var ksTree in ksTrees)
                {
                    var xbykContentItemGuid = spoiledGuidContext.EnsureNodeGuid(ksTree.NodeGUID, ksTree.NodeSiteID, ksTree.NodeID);
                    var ksUrlPaths = modelFacade.Select<ICmsPageUrlPath>("PageUrlPathNodeID = @nodeId", "PageUrlPathID", new SqlParameter("nodeId", ksTree.NodeID));
                    foreach (CmsPageUrlPathK13 ksPath in ksUrlPaths)
                    {
                        var xbykLanguageInfo = GetLanguageInfo(ksPath.PageUrlPathCulture);
                        pathToXbykPage[$"{ksSite.SiteGUID}|{NormalizeUrlPath(ksPath.PageUrlPathUrlPath)}"] = (xbykContentItemGuid, xbykLanguageInfo);
                    }

                    var ksDocuments = modelFacade
                        .SelectWhere<ICmsDocument>("DocumentNodeID = @nodeId", new SqlParameter("nodeId", ksTree.NodeID))
                        .ToList();

                    foreach (CmsDocumentK13 ksDocument in ksDocuments)
                    {
                        if (ksDocument.DocumentUnpublishedRedirectUrl is not null)
                        {
                            sourceInstanceRedirects.Add((ksDocument.DocumentGUID!.Value, ksSite.SiteGUID, GetLanguageInfo(ksDocument.DocumentCulture), xbykContentItemGuid, NormalizeUrlPath(ksDocument.DocumentUnpublishedRedirectUrl)));
                        }
                    }
                }
            }

            foreach (var (DocumentGuid, SiteGuid, LanguageInfo, ContentItemGuid, RedirectUrl) in sourceInstanceRedirects)
            {
                if (pathToXbykPage.TryGetValue($"{SiteGuid}|{NormalizeUrlPath(RedirectUrl)}", out var targetPage))
                {
                    var targetContentItem = ContentItemInfo.Provider.Get(targetPage.ContentItemGuid);
                    if (targetContentItem is null)
                    {
                        logger.LogWarning("Redirect URL could not be migrated. Page does not exist in target instance. Site GUID='{SiteGUID}', Redirect URL='{RedirectURL}', Document GUID='{DocumentGUID}", SiteGuid, RedirectUrl, DocumentGuid);
                        continue;
                    }
                    var targetWebPageItem = WebPageItemInfo.Provider.Get().WhereEquals(nameof(WebPageItemInfo.WebPageItemContentItemID), targetContentItem.ContentItemID).FirstOrDefault();
                    if (targetWebPageItem is null)
                    {
                        logger.LogWarning("Unpublished redirect URL path '{RedirectURLPath}' of source Document '{DocumentGuid}' could not be migrated. WebPageItem related to the redirection target page was not found", RedirectUrl, DocumentGuid);
                        continue;
                    }

                    bool webPageItemFound = false;
                    var redirectedContentItem = ContentItemInfo.Provider.Get(ContentItemGuid);
                    var redirectedContentItemLanguageMetadata = ContentItemLanguageMetadataInfo.Provider.Get().WhereEquals(nameof(ContentItemLanguageMetadataInfo.ContentItemLanguageMetadataContentItemID), redirectedContentItem.ContentItemID)
                        .WhereEquals(nameof(ContentItemLanguageMetadataInfo.ContentItemLanguageMetadataContentLanguageID), LanguageInfo.ContentLanguageID).FirstOrDefault();

                    if (redirectedContentItem is not null && redirectedContentItemLanguageMetadata is not null)
                    {
                        var redirectedWebPageItem = WebPageItemInfo.Provider.Get().WhereEquals(nameof(WebPageItemInfo.WebPageItemContentItemID), redirectedContentItem.ContentItemID).FirstOrDefault();
                        if (redirectedWebPageItem is not null)
                        {
                            var redirectedWebPageUrlPath = WebPageUrlPathInfo.Provider.Get().WhereEquals(nameof(WebPageUrlPathInfo.WebPageUrlPathIsLatest), true)
                                    .And().WhereEquals(nameof(WebPageUrlPathInfo.WebPageUrlPathContentLanguageID), LanguageInfo.ContentLanguageID)
                                    .And().WhereEquals(nameof(WebPageUrlPathInfo.WebPageUrlPathWebPageItemID), redirectedWebPageItem.WebPageItemID).FirstOrDefault();
                            if (redirectedWebPageUrlPath != null)
                            {
                                webPageItemFound = true;
                                var now = Service.Resolve<IDateTimeNowService>().GetDateTimeNow();
                                bool isScheduled = redirectedContentItemLanguageMetadata.ContentItemLanguageMetadataScheduledUnpublishWhen is { } scheduledUnpublishDate && scheduledUnpublishDate > now;
                                var formerUrlModel = new WebPageFormerUrlPathModel
                                {
                                    WebPageFormerUrlPathGUID = GuidHelper.CreateWebPageFormerUrlPathGuid($"redirect|{DocumentGuid}"),
                                    WebPageFormerUrlPath = redirectedWebPageUrlPath.WebPageUrlPath,
                                    WebPageFormerUrlPathContentLanguageGuid = LanguageInfo.ContentLanguageGUID,
                                    WebPageFormerUrlPathIsRedirect = true,
                                    WebPageFormerUrlPathIsRedirectScheduled = isScheduled,
                                    WebPageFormerUrlPathWebPageItemGuid = targetWebPageItem.WebPageItemGUID,
                                    WebPageFormerUrlPathSourceWebPageItemGuid = redirectedWebPageItem.WebPageItemGUID,
                                    WebPageFormerUrlPathWebsiteChannelGuid = SiteGuid
                                };
                                var importResult = await importer.ImportAsync(formerUrlModel);
                                if (importResult is { Success: true })
                                {
                                    logger.LogInformation("Unpublish redirect for Document '{DocumentGuid}' imported", DocumentGuid);
                                }
                                else
                                {
                                    logger.LogError("Failed to import WebPageFormerUrlPathModel for Document '{DocumentGuid}': {Exception}", DocumentGuid, importResult.Exception);
                                }
                            }
                        }
                    }

                    if (!webPageItemFound)
                    {
                        logger.LogError("Unpublish redirect URL path '{RedirectURLPath}' of source Document '{DocumentGuid}' could not be migrated. One of the following entities related to the Document was not found: ContentItem, ContentItemLanguageMetadata, WebPageItem, WebPageUrlPath", RedirectUrl, DocumentGuid);
                        continue;
                    }
                }
                else
                {
                    logger.LogError("Unpublished redirect URL path '{RedirectURLPath}' of source Document '{DocumentGuid}' could not be migrated. Path not found in target instance.", RedirectUrl, DocumentGuid);
                    continue;
                }
            }

        }
    }

    private string NormalizeUrlPath(string path) => path.ToLower().TrimStart('~').TrimStart('/').TrimEnd('/');

    private async Task MigratePages()
    {
        var sites = modelFacade.GetMigratedSites();
        foreach (var ksSite in sites)
        {
            var channelInfo = ChannelInfoProvider.ProviderObject.Get(ksSite.SiteGUID);
            if (channelInfo == null)
            {
                logger.LogError("Target channel for site '{SiteName}' not exists!", ksSite.SiteName);
                continue;
            }

            logger.LogInformation("Migrating pages for site '{SourceSiteName}' to target channel '{TargetChannelName}' as content items", ksSite.SiteName, channelInfo.ChannelName);

            var ksTrees = modelFacade.Select<ICmsTree>(
                "NodeSiteId = @siteId",
                "NodeLevel, NodeOrder",
                new SqlParameter("siteId", ksSite.SiteID)
            );

            // Map from original node GUID to mapped result. Using original node GUID is possible only within context of one site
            // as only then is the GUID guaranteed by legacy versions to be unique
            Dictionary<Guid, NodeMapResult> mappedSiteNodes = [];

            for (int pass = 0; pass < 2; pass++)
            {
                deferredTreeNodesService.Clear();
                foreach (var ksTreeOriginal in ksTrees)
                {
                    logger.LogDebug("Page '{NodeAliasPath}' migration", ksTreeOriginal.NodeAliasPath);

                    protocol.FetchedSource(ksTreeOriginal);

                    var ksNode = ksTreeOriginal;
                    var nodeLinkedNode = modelFacade.SelectById<ICmsTree>(ksNode.NodeLinkedNodeID);
                    var migratedDocuments = modelFacade
                        .SelectWhere<ICmsDocument>("DocumentNodeID = @nodeId", new SqlParameter("nodeId", ksNode.NodeID))
                        .ToList();

                    bool wasLinkedNode = nodeLinkedNode != null;
                    if (wasLinkedNode)
                    {
                        if (nodeLinkedNode?.NodeSiteID != ksNode.NodeSiteID)
                        {
                            // skip & write to protocol
                            logger.LogWarning("Linked node with NodeGuid {NodeGuid} is linked from different site - unable to migrate", ksTreeOriginal.NodeGUID);
                            protocol.Warning(HandbookReferences.CmsTreeTreeIsLinkFromDifferentSite, ksNode);
                            continue;
                        }

                        // materialize linked node & write to protocol
                        var linkedNode = modelFacade.SelectWhere<ICmsTree>("NodeSiteID = @nodeSiteID AND NodeGUID = @nodeGuid",
                            new SqlParameter("nodeSiteID", ksNode.NodeSiteID),
                            new SqlParameter("nodeGuid", nodeLinkedNode.NodeGUID)
                        ).SingleOrDefault();

                        Debug.Assert(ksNode != null, nameof(ksNode) + " != null");
                        Debug.Assert(linkedNode != null, nameof(linkedNode) + " != null");

                        migratedDocuments.Clear();

                        var linkedNodeDocuments = modelFacade
                            .SelectWhere<ICmsDocument>("DocumentNodeID = @nodeId", new SqlParameter("nodeId", linkedNode.NodeID))
                            .ToList();

                        for (int i = 0; i < linkedNodeDocuments.Count; i++)
                        {
                            var linkedDocument = linkedNodeDocuments[i];
                            var fixedDocumentGuid = GuidHelper.CreateDocumentGuid($"{linkedDocument.DocumentID}|{ksNode.NodeID}|{ksNode.NodeSiteID}"); //Guid.NewGuid();
                            var patchedNodeGuid = spoiledGuidContext.EnsureNodeGuid(ksNode.NodeGUID, ksNode.NodeSiteID, ksNode.NodeID);
                            if (ContentItemInfo.Provider.Get(patchedNodeGuid)?.ContentItemID is { } contentItemId)
                            {
                                if (cultureCodeToLanguageGuid!.TryGetValue(linkedDocument.DocumentCulture, out var languageGuid) &&
                                    ContentLanguageInfoProvider.ProviderObject.Get(languageGuid) is { } languageInfo)
                                {
                                    if (ContentItemCommonDataInfo.Provider.Get()
                                            .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataContentItemID), contentItemId)
                                            .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataContentLanguageID), languageInfo.ContentLanguageID)
                                            .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataIsLatest), true)
                                            .FirstOrDefault() is { } contentItemCommonDataInfo)
                                    {
                                        fixedDocumentGuid = contentItemCommonDataInfo.ContentItemCommonDataGUID;
                                        logger.LogTrace("Page '{NodeAliasPath}' is linked => ContentItemCommonDataGUID copy to DocumentGuid", ksNode.NodeAliasPath);
                                    }
                                }
                            }

                            linkedNodeDocuments[i] = linkedDocument switch
                            {
                                CmsDocumentK11 doc => doc with { DocumentGUID = fixedDocumentGuid, DocumentID = 0 },
                                CmsDocumentK12 doc => doc with { DocumentGUID = fixedDocumentGuid, DocumentID = 0 },
                                CmsDocumentK13 doc => doc with { DocumentGUID = fixedDocumentGuid, DocumentID = 0 },
                                _ => linkedNodeDocuments[i]
                            };

                            migratedDocuments.Add(linkedNodeDocuments[i]);
                            ksNode = ksNode switch
                            {
                                CmsTreeK11 node => node with { NodeLinkedNodeID = null, NodeLinkedNodeSiteID = null },
                                CmsTreeK12 node => node with { NodeLinkedNodeID = null, NodeLinkedNodeSiteID = null },
                                CmsTreeK13 node => node with { NodeLinkedNodeID = null, NodeLinkedNodeSiteID = null },
                                _ => ksNode
                            };

                            logger.LogWarning("Linked node with NodeGuid {NodeGuid} was materialized (Xperience by Kentico doesn't support links), it no longer serves as link to original document. This affect also routing, this document will have own link generated from node alias path", ksNode.NodeGUID);
                        }
                    }

                    var ksNodeClass = modelFacade.SelectById<ICmsClass>(ksNode.NodeClassID) ?? throw new InvalidOperationException($"Node with missing class, node id '{ksNode.NodeID}'");
                    string nodeClassClassName = ksNodeClass.ClassName;
                    if (classEntityConfiguration!.ExcludeCodeNames.Contains(nodeClassClassName, StringComparer.InvariantCultureIgnoreCase))
                    {
                        protocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(nodeClassClassName, "PageType"), ksNode);
                        logger.LogWarning("Page: page of class {ClassName} was skipped => it is explicitly excluded in configuration", nodeClassClassName);
                        continue;
                    }

                    if (nodeClassClassName == ClassCmsRoot)
                    {
                        logger.LogInformation("Root node skipped, V27 has no support for root nodes");
                        continue;
                    }

                    Debug.Assert(migratedDocuments.Count > 0, "migratedDocuments.Count > 0");

                    if (ksTreeOriginal is { NodeSKUID: not null })
                    {
                        logger.LogWarning("Page '{NodeAliasPath}' has SKU bound, SKU info will be discarded", ksTreeOriginal.NodeAliasPath);
                        protocol.Append(HandbookReferences.NotCurrentlySupportedSkip()
                            .WithMessage($"Page '{ksTreeOriginal.NodeAliasPath}' has SKU bound, SKU info will be discarded")
                            .WithIdentityPrint(ksTreeOriginal)
                            .WithData(new { ksTreeOriginal.NodeSKUID })
                        );
                    }

                    string safeNodeName = await Service.Resolve<IContentItemCodeNameProvider>().Get(ksNode.NodeName);
                    var ksNodeParent = modelFacade.SelectById<ICmsTree>(ksNode.NodeParentID);
                    var nodeParentGuid = ksNodeParent?.NodeAliasPath == "/" || ksNodeParent == null
                        ? (Guid?)null
                        : spoiledGuidContext.EnsureNodeGuid(ksNodeParent);

                    DataClassInfo targetClass = null!;
                    var classMapping = classMappingProvider.GetMapping(ksNodeClass.ClassName);

                    var producedReusableSchemas = reusableSchemaBuilders.Where(x =>
                        string.Equals(x.SourceClassName, nodeClassClassName, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                    if (classMapping is null && producedReusableSchemas.Length != 0)
                    {
                        logger.LogInformation("Page {NodeAliasPath} of class {ClassName} skipped. The class is converted to the following reusable field schemas: {ReusableFieldSchemas}. " +
                                              "In such situation only custom class mapping of the class is supported and no mapping was provided",
                            ksNode.NodeAliasPath, nodeClassClassName, string.Join(", ", producedReusableSchemas.Select(x => x.SchemaName).ToArray()));
                        continue;
                    }

                    targetClass = classMapping != null
                        ? DataClassInfoProvider.ProviderObject.Get(classMapping.TargetClassName)
                        : DataClassInfoProvider.ProviderObject.Get(ksNodeClass.ClassGUID);

                    var results = mapper.Map(new CmsTreeMapperSource(
                        ksNode,
                        safeNodeName,
                        ksSite.SiteGUID,
                        nodeParentGuid,
                        cultureCodeToLanguageGuid!,
                        targetClass?.ClassFormDefinition,
                        ksNodeClass.ClassFormDefinition,
                        migratedDocuments,
                        ksSite,
                        pass != 0
                    ));
                    try
                    {
                        WebPageItemInfo? webPageItemInfo = null;
                        var commonDataInfos = new List<ContentItemCommonDataInfo>();
                        ContentItemInfo? contentItemInfo = null;
                        ContentItemDirectiveBase? contentItemDirective = null;

                        foreach (var umtModel in results)
                        {
                            if (umtModel is ContentItemDirectiveBase yieldedDirective)
                            {
                                contentItemDirective = yieldedDirective;
                                mappedSiteNodes[contentItemDirective!.Node!.NodeGUID] = new(contentItemDirective!.Node!, contentItemDirective.ContentItemGuid, [], contentItemDirective.TargetClassInfo!, contentItemDirective.ChildLinks);
                            }
                            else
                            {
                                switch (await importer.ImportAsync(umtModel))
                                {
                                    case { Success: false } result:
                                    {
                                        logger.LogError("Failed to import: {Exception}, {ValidationResults}", result.Exception, JsonConvert.SerializeObject(result.ModelValidationResults));
                                        break;
                                    }
                                    case { Success: true, Imported: ContentItemCommonDataInfo ccid }:
                                    {
                                        if (contentItemInfo is not null && ccid.ContentItemCommonDataContentItemID == contentItemInfo.ContentItemID)
                                        {
                                            commonDataInfos.Add(ccid);
                                        }
                                        Debug.Assert(ccid.ContentItemCommonDataContentLanguageID != 0, "ccid.ContentItemCommonDataContentLanguageID != 0");
                                        break;
                                    }
                                    case { Success: true, Imported: ContentItemLanguageMetadataInfo cclm }:
                                    {
                                        Debug.Assert(cclm.ContentItemLanguageMetadataContentLanguageID != 0, "ccid.ContentItemCommonDataContentLanguageID != 0");
                                        break;
                                    }
                                    case { Success: true, Imported: WebPageItemInfo wp }:
                                    {
                                        webPageItemInfo = wp;
                                        break;
                                    }
                                    case { Success: true, Imported: ContentItemInfo cii }:
                                    {
                                        contentItemInfo = cii;
                                        break;
                                    }
                                    case { Success: true, Imported: ContentItemDataInfo cidi }:
                                    {
                                        mappedSiteNodes[ksNode.NodeGUID].ContentItemDataGuids.Add(cidi.ContentItemDataGUID);
                                        break;
                                    }

                                    default:
                                        break;
                                }
                            }
                        }

                        if (contentItemDirective is not DropDirective)
                        {
                            AssertVersionStatusRule(commonDataInfos);

                            if (webPageItemInfo != null && targetClass is { ClassWebPageHasUrl: true })
                            {
                                await GenerateDefaultPageUrlPath(ksNode, webPageItemInfo);
                                if (!contentItemDirective!.RegenerateUrlPath)
                                {
                                    foreach (var migratedDocument in migratedDocuments)
                                    {
                                        var languageGuid = cultureCodeToLanguageGuid![migratedDocument.DocumentCulture];

                                        await MigratePageUrlPaths(ksSite.SiteGUID,
                                            languageGuid,
                                            commonDataInfos,
                                            migratedDocument,
                                            ksNode,
                                            migratedDocument.DocumentCulture,
                                            wasLinkedNode, webPageItemInfo);
                                    }
                                }

                                if (contentItemDirective.FormerUrlPaths is not null)
                                {
                                    MigrateFormerUrls(ksNode, webPageItemInfo, contentItemDirective.FormerUrlPaths);
                                }

                                var urls = WebPageUrlPathInfo.Provider.Get()
                                    .WhereEquals(nameof(WebPageUrlPathInfo.WebPageUrlPathWebPageItemID), webPageItemInfo.WebPageItemID);

                                if (urls.Count < 1)
                                {
                                    logger.LogWarning("No url for page {Page}", new { webPageItemInfo.WebPageItemName, webPageItemInfo.WebPageItemTreePath, webPageItemInfo.WebPageItemGUID });
                                }
                            }
                            else
                            {
                                logger.LogTrace("No webpage item produced for '{NodeAliasPath}'", ksNode.NodeAliasPath);
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        protocol.Append(HandbookReferences
                            .ErrorCreatingTargetInstance<WebPageItemInfo>(ex)
                            .NeedsManualAction()
                            .WithIdentityPrint(ksNode)
                        );
                        logger.LogError("Failed to import content item: {Exception}", ex);
                    }
                }
                ksTrees = deferredTreeNodesService.GetNodes();
            }

            await LinkChildren(mappedSiteNodes);
        }
    }

    private async Task LinkChildren(Dictionary<Guid, NodeMapResult> mappedSiteNodes)
    {
        var reusableItems = mappedSiteNodes.Values.Where(x => x.TargetClassInfo.IsReusableContentType());
        var nonReusableItemsWithChildLinks = mappedSiteNodes.Values.Where(x => !x.TargetClassInfo.IsReusableContentType() && x.ChildLinks.Count != 0);
        foreach (var item in nonReusableItemsWithChildLinks)
        {
            logger.LogError("Content item {ContentItemGuid} (original node {OriginalNodeGuid}) is not reusable, but specifies linked children. Add the content type to appsettings ConvertClassesToContentHub collection", item.ContentItemGuid, item.Node.NodeGUID);
        }

        // [content type name] -> { [field name] -> allowed referenced types }
        Dictionary<string, Dictionary<string, (Guid? fieldGuid, HashSet<Guid> allowedTypes)>> referenceFields = [];

        // Gather necessary reference fields and/or allowed referenced types
        foreach (var parentItem in reusableItems)
        {
            if (!referenceFields.ContainsKey(parentItem.TargetClassInfo.ClassName))
            {
                referenceFields[parentItem.TargetClassInfo.ClassName] = [];
            }
            var childCollections = parentItem.ChildLinks.GroupBy(x => x.fieldName);
            foreach (var collection in childCollections)
            {
                if (!referenceFields[parentItem.TargetClassInfo.ClassName].ContainsKey(collection.Key))
                {
                    referenceFields[parentItem.TargetClassInfo.ClassName][collection.Key] = (null, []);
                }
                foreach (var (fieldName, node) in collection)
                {
                    var childTargetType = mappedSiteNodes[node.NodeGUID].TargetClassInfo.ClassGUID;
                    referenceFields[parentItem.TargetClassInfo.ClassName][collection.Key].allowedTypes.Add(childTargetType);
                }
            }
        }

        // Merge required allowed referenced types with already existing ones
        foreach (var classInfo in reusableItems.Select(x => x.TargetClassInfo).GroupBy(x => x.ClassName).Select(x => x.First()))
        {
            if (referenceFields.ContainsKey(classInfo.ClassName))
            {
                foreach (var field in new FormInfo(classInfo.ClassFormDefinition).GetFields<FormFieldInfo>().Where(x => x.DataType == "contentitemreference"))
                {
                    if (referenceFields[classInfo.ClassName].ContainsKey(field.Name))
                    {
                        string existingSerialized = (string?)field.Settings[FormDefinitionPatcher.AllowedContentItemTypeIdentifiers] ?? "[]";
                        var existingAllowedTypes = JsonConvert.DeserializeObject<Guid[]>(existingSerialized)!.ToHashSet();
                        foreach (var existingAllowedType in existingAllowedTypes)
                        {
                            referenceFields[classInfo.ClassName][field.Name].allowedTypes.Add(existingAllowedType);
                        }
                    }
                }
            }
        }

        foreach (var classRefFields in referenceFields)
        {
            var classInfo = DataClassInfoProvider.ProviderObject.Get(classRefFields.Key);
            var fi = new FormInfo(classInfo.ClassFormDefinition);
            foreach (var field in classRefFields.Value)
            {
                var ffi = fi.GetFormField(field.Key);
                if (ffi is null)
                {
                    ffi = new FormFieldInfo
                    {
                        Guid = GuidHelper.CreateFieldGuid($"{field.Key.ToLower()}|{classInfo.ClassName}"),
                        AllowEmpty = true,
                        Caption = field.Key,
                        DataType = "contentitemreference",
                        Name = field.Key,
                        Enabled = true,
                        Visible = true,
                        System = false,
                        DefaultValue = null,
                        Settings = {
                            { FormDefinitionPatcher.SettingsElemControlname, FormComponents.AdminContentItemSelectorComponent },
                        }
                    };
                    fi.AddFormItem(ffi);
                }
                classRefFields.Value[field.Key] = (ffi.Guid, field.Value.allowedTypes);
                ffi.Settings[FormDefinitionPatcher.AllowedContentItemTypeIdentifiers] = JsonConvert.SerializeObject(field.Value.allowedTypes.ToArray());
            }
            classInfo.ClassFormDefinition = fi.GetXmlDefinition();
            DataClassInfoProvider.ProviderObject.Set(classInfo);
        }

        foreach (var parentItem in reusableItems)
        {
            var parentMappedNode = mappedSiteNodes[parentItem.Node.NodeGUID];
            foreach (var contentItemDataGuid in parentItem.ContentItemDataGuids)
            {
                var dataModel = new ContentItemDataModel
                {
                    ContentItemDataGUID = contentItemDataGuid,
                    ContentItemDataCommonDataGuid = contentItemDataGuid,
                    ContentItemContentTypeName = parentMappedNode.TargetClassInfo.ClassName,
                    CustomProperties = []
                };

                foreach (var childCollection in parentItem.ChildLinks.GroupBy(x => x.fieldName))
                {
                    var guidArray = childCollection.Select(x => new { Identifier = mappedSiteNodes[x.node.NodeGUID].ContentItemGuid }).ToArray();
                    string serializedValue = JsonConvert.SerializeObject(guidArray);
                    dataModel.CustomProperties[childCollection.Key] = serializedValue;
                }

                var importResult = await importer.ImportAsync(dataModel);
                if (importResult is { Success: true })
                {
                    logger.LogInformation("Imported linked children of content item {ContentItemGuid}", parentItem.ContentItemGuid);
                }
                else
                {
                    logger.LogError("Failed to import linked children of content item {ContentItemGuid}: {Exception}", parentItem.ContentItemGuid, importResult.Exception);
                }
            }
        }
    }

    [Conditional("DEBUG")]
    private static void AssertVersionStatusRule(List<ContentItemCommonDataInfo> commonDataInfos)
    {
        foreach (var contentItemCommonDataInfos in commonDataInfos.GroupBy(x => x.ContentItemCommonDataContentLanguageID))
        {
            VersionStatus? versionStatus = null;
            bool onlyOneStatus = contentItemCommonDataInfos.Aggregate(true, (acc, i) =>
            {
                try
                {
                    if (versionStatus.HasValue)
                    {
                        return versionStatus != i.ContentItemCommonDataVersionStatus;
                    }

                    return true;
                }
                finally
                {
                    versionStatus = i.ContentItemCommonDataVersionStatus;
                }
            });
            Debug.Assert(onlyOneStatus);
        }
    }

    public enum PageRoutingModeEnum // copy of enum from KX13 dll
    {
        /// <summary>
        /// Routing based on custom routes of standard MVC support.
        /// </summary>
        Custom = 0,
        /// <summary>
        /// Routing based on system routes driven by content tree structure.
        /// </summary>
        BasedOnContentTree = 1,
    }

    private readonly Dictionary<int, ICmsClass> cmsClassCache = [];
    private ICmsClass GetCmsClass(int classId)
    {
        if (cmsClassCache.TryGetValue(classId, out var cmsClass))
        {
            return cmsClass;
        }

        cmsClass = modelFacade.SelectById<ICmsClass>(classId);
        cmsClassCache[classId] = cmsClass ?? throw new InvalidOperationException($"CMS Class with class id {classId} not found => invalid data");
        return cmsClass;
    }

    private async Task MigratePageUrlPaths(Guid webSiteChannelGuid, Guid languageGuid,
        List<ContentItemCommonDataInfo> contentItemCommonDataInfos, ICmsDocument? ksDocument, ICmsTree ksTree, string documentCulture, bool wasLinkedNode, WebPageItemInfo webPageItemInfo)
    {
        var languageInfo = ContentLanguageInfoProvider.ProviderObject.Get(languageGuid);
        var webSiteChannel = WebsiteChannelInfoProvider.ProviderObject.Get(webSiteChannelGuid);

        #region Migration of custom routing model

        if (modelFacade.SelectVersion() is { Major: 13 } && KenticoHelper.GetSettingsKey<int>(modelFacade, ksTree.NodeSiteID, "CMSRoutingMode") is (int)PageRoutingModeEnum.Custom)
        {
            if (modelFacade.SelectById<ICmsSite>(ksTree.NodeSiteID) is not { } site)
            {
                logger.LogError("Unable to find source site with ID '{SiteID}', fallback url will be used for node {NodeID}", ksTree.NodeSiteID, ksTree.NodeID);
            }
            // for ability to resolve macros we query source instance where we can resolve marcos in url pattern for particular page
            else if (!sourceInstanceContext.HasInfo)
            {
                logger.LogWarning("Cannot migrate url for document '{DocumentID}' / node '{NodeID}', source instance context is not available or set-up correctly - default fallback will be used.", ksDocument?.DocumentID, ksTree.NodeID);
            }
            else if (GetCmsClass(ksTree.NodeClassID) is { } cmsClass && string.IsNullOrWhiteSpace(cmsClass.ClassURLPattern))
            {
                logger.LogWarning("Cannot migrate url for document '{DocumentID}' / node '{NodeID}', class {ClassName} has no url pattern set - cannot migrate to former url.", ksDocument?.DocumentID, ksTree.NodeID, cmsClass.ClassName);
            }
            else if (sourceInstanceContext.GetNodeUrls(ksTree.NodeID, site.SiteName) is not { Count: > 0 } pageModels)
            {
                logger.LogError("No information could be found in source instance about node {NodeID} on site {SiteName}", ksTree.NodeID, site.SiteName);
            }
            else if (pageModels.FirstOrDefault(pm => pm.DocumentCulture == documentCulture) is not { } pageModel)
            {
                logger.LogWarning("Page url information for document {DocumentID} / node {NodeID} not found for culture {Culture}", ksDocument?.DocumentID, ksTree.NodeID, documentCulture);
            }
            else if (string.IsNullOrWhiteSpace(pageModel.CultureUrl))
            {
                logger.LogWarning("Page url information for document {DocumentID} / node {NodeID} was found for culture {Culture}, but culture url is empty - unexpected", ksDocument?.DocumentID, ksTree.NodeID, documentCulture);
            }
            else
            {
                string patchedUrl = pageModel.CultureUrl.TrimStart(['~']).TrimStart(['/']);
                string urlHash = modelFacade.HashPath(patchedUrl);
                var webPageFormerUrlPathInfo = new WebPageFormerUrlPathInfo
                {
                    WebPageFormerUrlPath = patchedUrl,
                    WebPageFormerUrlPathHash = urlHash,
                    WebPageFormerUrlPathWebPageItemID = webPageItemInfo.WebPageItemID,
                    WebPageFormerUrlPathWebsiteChannelID = webSiteChannel.WebsiteChannelID,
                    WebPageFormerUrlPathContentLanguageID = languageInfo.ContentLanguageID,
                    WebPageFormerUrlPathLastModified = Service.Resolve<IDateTimeNowService>().GetDateTimeNow()
                };
                WebPageFormerUrlPathInfo.Provider.Set(webPageFormerUrlPathInfo);
                logger.LogEntitySetAction(true, webPageItemInfo);
                return;
            }
        }

        #endregion

        if (modelFacade.IsAvailable<ICmsPageUrlPath>())
        {
            var ksPaths = modelFacade.SelectWhere<CmsPageUrlPathK13>("PageUrlPathNodeId = @nodeId AND PageUrlPathCulture = @culture",
                new SqlParameter("nodeId", ksTree.NodeID),
                new SqlParameter("culture", documentCulture)
            ).ToList();

            if (ksPaths.Count > 0)
            {
                foreach (var ksPath in ksPaths)
                {
                    logger.LogTrace("Page url path: C={Culture} S={Site} P={Path}", ksPath.PageUrlPathCulture, ksPath.PageUrlPathSiteID, ksPath.PageUrlPathUrlPath);

                    foreach (var contentItemCommonDataInfo in contentItemCommonDataInfos.Where(x => x.ContentItemCommonDataContentLanguageID == languageInfo.ContentLanguageID))
                    {
                        logger.LogTrace("Page url path common data info: CIID={ContentItemId} CLID={Language} ID={Id}", contentItemCommonDataInfo.ContentItemCommonDataContentItemID,
                            contentItemCommonDataInfo.ContentItemCommonDataContentLanguageID, contentItemCommonDataInfo.ContentItemCommonDataID);

                        Debug.Assert(!string.IsNullOrWhiteSpace(ksPath.PageUrlPathUrlPath), "!string.IsNullOrWhiteSpace(kx13PageUrlPath.PageUrlPathUrlPath)");

                        string path = ksPath.PageUrlPathUrlPath.TrimStart('/');
                        string? hash = ksPath.PageUrlPathUrlPathHash;

                        // Check collisions with other pages
                        string uniquePath;
                        try
                        {
                            uniquePath = PreventUrlPathCollisions(webPageItemInfo, path);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Unable to resolve collision");
                            continue;
                        }

                        if (!string.Equals(uniquePath, path, StringComparison.OrdinalIgnoreCase))
                        {
                            logger.LogWarning("Path '{Path}' of tree node GUID='{NodeGuid}', NodeAliasPath='{NodeAliasPath}' could not be used as is due to collision with already existing path(s). " +
                                "Unique identifier was appended. New path = '{NewPath}'", path, ksTree.NodeGUID, ksTree.NodeAliasPath, uniquePath);
                            path = uniquePath;
                            hash = null;    // Let UMT compute new hash
                        }

                        var webPageUrlPath = new WebPageUrlPathModel
                        {
                            WebPageUrlPathGUID = contentItemCommonDataInfo.ContentItemCommonDataVersionStatus == VersionStatus.Draft
                                ? Guid.NewGuid()
                                : ksPath.PageUrlPathGUID,
                            WebPageUrlPath = path,
                            WebPageUrlPathHash = hash,
                            WebPageUrlPathWebPageItemGuid = webPageItemInfo.WebPageItemGUID,
                            WebPageUrlPathWebsiteChannelGuid = webSiteChannelGuid,
                            WebPageUrlPathContentLanguageGuid = languageGuid,
                            WebPageUrlPathIsLatest = contentItemCommonDataInfo.ContentItemCommonDataIsLatest,
                            WebPageUrlPathIsDraft = contentItemCommonDataInfo.ContentItemCommonDataVersionStatus switch
                            {
                                VersionStatus.InitialDraft => false,
                                VersionStatus.Draft => true,
                                VersionStatus.Published => false,
                                VersionStatus.Unpublished => false,
                                _ => throw new ArgumentOutOfRangeException()
                            }
                        };

                        CheckPathAlreadyExists(webPageUrlPath, languageInfo, webSiteChannel, webPageItemInfo.WebPageItemID);

                        var importResult = await importer.ImportAsync(webPageUrlPath);

                        LogImportResult(importResult);
                    }
                }
            }
        }
        else
        {
            foreach (var contentItemCommonDataInfo in contentItemCommonDataInfos.Where(x => x.ContentItemCommonDataContentLanguageID == languageInfo.ContentLanguageID))
            {
                logger.LogTrace("Page url path common data info: CIID={ContentItemId} CLID={Language} ID={Id}", contentItemCommonDataInfo.ContentItemCommonDataContentItemID,
                    contentItemCommonDataInfo.ContentItemCommonDataContentLanguageID, contentItemCommonDataInfo.ContentItemCommonDataID);

                string? urlPath = (ksDocument switch
                {
                    CmsDocumentK11 doc => wasLinkedNode ? null : doc.DocumentUrlPath,
                    CmsDocumentK12 doc => wasLinkedNode ? null : doc.DocumentUrlPath,
                    _ => null
                }).NullIf(string.Empty)?.TrimStart('/');

                if (urlPath is not null)
                {
                    // Check collisions with other pages
                    string uniquePath = PreventUrlPathCollisions(webPageItemInfo, urlPath);
                    if (!string.Equals(uniquePath, urlPath, StringComparison.OrdinalIgnoreCase))
                    {
                        logger.LogWarning("Path '{Path}' of tree node GUID='{NodeGuid}', NodeAliasPath='{NodeAliasPath}' could not be used as is due to collision with already existing path(s)." +
                            "Unique identifier was appended. New path = '{NewPath}'", urlPath, ksTree.NodeGUID, ksTree.NodeAliasPath, uniquePath);
                        urlPath = uniquePath;
                    }

                    var webPageUrlPath = new WebPageUrlPathModel
                    {
                        WebPageUrlPathGUID = GuidHelper.CreateWebPageUrlPathGuid($"{urlPath}|{documentCulture}|{webSiteChannel.WebsiteChannelGUID}|{ksTree.NodeID}"),
                        WebPageUrlPath = urlPath,
                        WebPageUrlPathWebPageItemGuid = webPageItemInfo.WebPageItemGUID,
                        WebPageUrlPathWebsiteChannelGuid = webSiteChannelGuid,
                        WebPageUrlPathContentLanguageGuid = languageGuid,
                        WebPageUrlPathIsLatest = contentItemCommonDataInfo.ContentItemCommonDataIsLatest,
                        WebPageUrlPathIsDraft = contentItemCommonDataInfo.ContentItemCommonDataVersionStatus switch
                        {
                            VersionStatus.InitialDraft => false,
                            VersionStatus.Draft => true,
                            VersionStatus.Published => false,
                            VersionStatus.Unpublished => false,
                            _ => throw new ArgumentOutOfRangeException()
                        }
                    };

                    CheckPathAlreadyExists(webPageUrlPath, languageInfo, webSiteChannel, webPageItemInfo.WebPageItemID);

                    var importResult = await importer.ImportAsync(webPageUrlPath);

                    LogImportResult(importResult);
                }
            }
        }
    }

    private string PreventUrlPathCollisions(WebPageItemInfo webPageItemInfo, string path) =>
        UniqueNameHelper.MakeUnique(path, testedUniquePath =>
        {
            var collidingPaths = GetCollidingPaths(stored => stored.Where(x => string.Equals(NormalizeUrlPath(x.Path), NormalizeUrlPath(path))).Concat([new PagePath(webPageItemInfo.WebPageItemID, webPageItemInfo.WebPageItemWebsiteChannelID, testedUniquePath)])).Where(x => x.WebPageItemID != webPageItemInfo.WebPageItemID);

            return !collidingPaths.Any();
        });

    private record PagePath(int WebPageItemID, int WebsiteChannelID, string Path);
    private IEnumerable<PagePath> GetCollidingPaths(Func<IEnumerable<PagePath>, IEnumerable<PagePath>>? preprocessTestedSet = null)
    {
        var storedPaths = WebPageUrlPathInfo.Provider.Get()
            .Columns(nameof(WebPageUrlPathInfo.WebPageUrlPath), nameof(WebPageUrlPathInfo.WebPageUrlPathWebsiteChannelID), nameof(WebPageUrlPathInfo.WebPageUrlPathWebPageItemID)).ToArray()
            .Select(x => new PagePath(x.WebPageUrlPathWebPageItemID, x.WebPageUrlPathWebsiteChannelID, x.WebPageUrlPath));

        var testedPaths = preprocessTestedSet is null ? storedPaths : preprocessTestedSet(storedPaths);

        var groups = testedPaths.Select(x => (x.Path, NormalizedPath: NormalizeUrlPath(x.Path), x.WebsiteChannelID, x.WebPageItemID)).GroupBy(x => $"{x.WebsiteChannelID}|{x.NormalizedPath}");

        // If one WebPageItem has multiple WebPageUrlPath entries (e.g. in draft), we will have multiple entries in a group, but this isn't really a collision
        var groupsFiltered = groups.Select(g => g.DistinctBy(x => $"{x.WebPageItemID}|{x.WebsiteChannelID}|{x.NormalizedPath}"));

        return groupsFiltered.Where(x => x.Count() > 1).SelectMany(x => x.Select(y => new PagePath(y.WebPageItemID, y.WebsiteChannelID, y.Path)));
    }

    private async Task GenerateDefaultPageUrlPath(ICmsTree ksTree, WebPageItemInfo webPageItemInfo)
    {
        var man = Service.Resolve<IWebPageUrlManager>();

        // In case of collision, try to avoid it by appending 4 random characters
        bool resolved = false;
        List<PagePath> collisions = [];

        for (int i = 0; i < UniqueNameHelper.UniqueSuffixCount; i++)
        {
            var alias = ksTree.NodeAlias + (i != 0 ? $"-{UniqueNameHelper.GetSuffix(i)}" : string.Empty);
            var collisionData = await man.GeneratePageUrlPath(webPageItemInfo, alias, VersionStatus.InitialDraft, CancellationToken.None);
            collisionData = collisionData.Where(x => x.WebPageItemID != webPageItemInfo.WebPageItemID);

            resolved = !collisionData.Any();

            // Catch collision cases that GeneratePageUrlPath doesn't catch
            var collidingPaths = GetCollidingPaths().Where(x => x.WebPageItemID == webPageItemInfo.WebPageItemID);
            if (collidingPaths.Any())
            {
                collisions.AddRange(collidingPaths);
                resolved = false;
                // Cleanup added paths
                WebPageUrlPathInfo.Provider.BulkDelete(new WhereCondition().WhereEquals(nameof(WebPageUrlPathInfo.WebPageUrlPathWebPageItemID), webPageItemInfo.WebPageItemID));
            }

            if (resolved)
            {
                break;
            }
        }

        if (resolved)
        {
            if (collisions.Any())
            {
                logger.LogWarning("Default path for tree node GUID='{NodeGuid}', NodeAliasPath='{NodeAliasPath}' had to be suffixed with unique identifier, because the original URL path was already occupied. " +
                    "Note that the default path may not be used in the end, if there exists path for this tree node in the source instance", ksTree.NodeGUID, ksTree.NodeAliasPath);
            }
        }
        else        // No attempt went through without collision. This shouldn't happen in any concievable case, but if so, we want to know about it
        {
            logger.LogError("Cannot generate default path for tree node GUID='{NodeGuid}', NodeAliasPath='{NodeAliasPath}'. The following paths were tried, but all are in collision with already existing data. " +
                "Note that the default path may not be used in the end, if there exists path for this tree node in the source instance. Collisions: {Collisions}", ksTree.NodeGUID, ksTree.NodeAliasPath, string.Join(";", collisions.Select(x => x.Path)));
        }

    }

    private void CheckPathAlreadyExists(WebPageUrlPathModel webPageUrlPath,
        ContentLanguageInfo languageInfo,
        WebsiteChannelInfo webSiteChannel, int webPageItemId)
    {
        Debug.Assert(webPageUrlPath is not { WebPageUrlPathIsLatest: false, WebPageUrlPathIsDraft: true }, "webPageUrlPath is not { WebPageUrlPathIsLatest: false, WebPageUrlPathIsDraft: true }");

        var existingPaths = WebPageUrlPathInfo.Provider.Get()
            .WhereEquals(nameof(WebPageUrlPathInfo.WebPageUrlPathWebPageItemID), webPageItemId)
            .ToList();

        var ep = existingPaths.FirstOrDefault(ep =>
            ep.WebPageUrlPathContentLanguageID == languageInfo.ContentLanguageID &&
            ep.WebPageUrlPathIsDraft == webPageUrlPath.WebPageUrlPathIsDraft &&
            ep.WebPageUrlPathWebsiteChannelID == webSiteChannel.WebsiteChannelID &&
            ep.WebPageUrlPathWebPageItemID == webPageItemId
        );

        if (ep != null)
        {
            webPageUrlPath.WebPageUrlPathGUID = ep.WebPageUrlPathGUID;
            logger.LogTrace("Existing page url path found for '{Path}', fixing GUID to '{Guid}'", webPageUrlPath.WebPageUrlPath, webPageUrlPath.WebPageUrlPathGUID);
        }
    }

    private void LogImportResult(IImportResult importResult)
    {
        switch (importResult)
        {
            case { Success: true, Imported: WebPageUrlPathInfo imported }:
            {
                logger.LogInformation("Page url path imported '{Path}' '{Guid}'", imported.WebPageUrlPath, imported.WebPageUrlPathGUID);
                break;
            }
            case { Success: false, Exception: { } exception }:
            {
                logger.LogError("Failed to import page url path: {Error}", exception.ToString());
                break;
            }
            case { Success: false, ModelValidationResults: { } validation }:
            {
                foreach (var validationResult in validation)
                {
                    logger.LogError("Failed to import page url path {Members}: {Error}", string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage);
                }

                break;
            }

            default:
                break;
        }
    }

    internal static QueryExpression GetWebPageUrlPathHashQueryExpression(string urlPath) => $"CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', LOWER(N'{SqlHelper.EscapeQuotes(urlPath)}')), 2)".AsExpression();

    private void MigrateFormerUrls(ICmsTree ksNode, WebPageItemInfo targetPage, IEnumerable<FormerPageUrlPath> formerUrlPaths)
    {
        foreach (var path in formerUrlPaths)
        {
            try
            {
                var languageInfo = GetLanguageInfo(path.LanguageName);
                var ktPath = WebPageFormerUrlPathInfo.Provider.Get()
                    .WhereEquals(nameof(WebPageFormerUrlPathInfo.WebPageFormerUrlPathHash), GetWebPageUrlPathHashQueryExpression(path.Path))
                    .WhereEquals(nameof(WebPageFormerUrlPathInfo.WebPageFormerUrlPathWebsiteChannelID), targetPage.WebPageItemWebsiteChannelID)
                    .WhereEquals(nameof(WebPageFormerUrlPathInfo.WebPageFormerUrlPathContentLanguageID), languageInfo.ContentLanguageID)
                    .SingleOrDefault();

                if (ktPath != null)
                {
                    protocol.FetchedTarget(ktPath);
                }

                var webPageFormerUrlPathInfo = ktPath ?? new WebPageFormerUrlPathInfo();
                webPageFormerUrlPathInfo.WebPageFormerUrlPath = path.Path;
                webPageFormerUrlPathInfo.WebPageFormerUrlPathHash = modelFacade.HashPath(path.Path);
                webPageFormerUrlPathInfo.WebPageFormerUrlPathWebPageItemID = targetPage.WebPageItemID;
                webPageFormerUrlPathInfo.WebPageFormerUrlPathWebsiteChannelID = targetPage.WebPageItemWebsiteChannelID;
                webPageFormerUrlPathInfo.WebPageFormerUrlPathContentLanguageID = languageInfo.ContentLanguageID;
                webPageFormerUrlPathInfo.WebPageFormerUrlPathLastModified = path.LastModified ?? DateTime.Now;

                WebPageFormerUrlPathInfo.Provider.Set(webPageFormerUrlPathInfo);
                logger.LogInformation("Former page url path imported '{Path}'", webPageFormerUrlPathInfo.WebPageFormerUrlPath);
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to import page former url path: {Exception}", ex);
            }
        }
    }

    private readonly ConcurrentDictionary<string, ContentLanguageInfo> languages = new(StringComparer.InvariantCultureIgnoreCase);
    private ContentLanguageInfo GetLanguageInfo(string culture) => languages.GetOrAdd(
                culture,
                s => ContentLanguageInfoProvider.ProviderObject.Get().WhereEquals(nameof(ContentLanguageInfo.ContentLanguageName), s).SingleOrDefault() ?? throw new InvalidOperationException($"Missing content language '{s}'")
            );

    #region Deffered patch

    private async Task ExecDeferredVisualBuilderPatch()
    {
        logger.LogInformation("Executing TreePath patch");

        foreach ((var uniqueId, string className, int webSiteChannelId) in deferredPathService.GetWidgetsToPatch())
        {
            if (className == ContentItemCommonDataInfo.TYPEINFO.ObjectClassName)
            {
                var contentItemCommonDataInfo = await ContentItemCommonDataInfo.Provider.GetAsync(uniqueId);

                contentItemCommonDataInfo.ContentItemCommonDataVisualBuilderWidgets = DeferredPatchVisualBuilderWidgets(
                    contentItemCommonDataInfo.ContentItemCommonDataVisualBuilderWidgets, webSiteChannelId, out bool anythingChangedW);
                contentItemCommonDataInfo.ContentItemCommonDataVisualBuilderTemplateConfiguration = DeferredPatchVisualTemplateConfiguration(
                    contentItemCommonDataInfo.ContentItemCommonDataVisualBuilderTemplateConfiguration, webSiteChannelId, out bool anythingChangedC);

                if (anythingChangedC || anythingChangedW)
                {
                    contentItemCommonDataInfo.Update();
                }
            }
            else if (className == PageTemplateConfigurationInfo.TYPEINFO.ObjectClassName)
            {
                var pageTemplateConfigurationInfo = await PageTemplateConfigurationInfo.Provider.GetAsync(uniqueId);
                pageTemplateConfigurationInfo.PageTemplateConfigurationWidgets = DeferredPatchVisualBuilderWidgets(
                    pageTemplateConfigurationInfo.PageTemplateConfigurationWidgets,
                    webSiteChannelId,
                    out bool anythingChangedW
                );
                pageTemplateConfigurationInfo.PageTemplateConfigurationTemplate = DeferredPatchVisualTemplateConfiguration(
                    pageTemplateConfigurationInfo.PageTemplateConfigurationTemplate,
                    webSiteChannelId,
                    out bool anythingChangedC
                );
                if (anythingChangedW || anythingChangedC)
                {
                    PageTemplateConfigurationInfo.Provider.Set(pageTemplateConfigurationInfo);
                }
            }
        }
    }

    private string DeferredPatchVisualTemplateConfiguration(string documentPageTemplateConfiguration, int webSiteChannelId, out bool anythingChanged)
    {
        if (!string.IsNullOrWhiteSpace(documentPageTemplateConfiguration))
        {
            var configuration = JObject.Parse(documentPageTemplateConfiguration);
            VisualBuilderWidgetsPatcher.DeferredPatchProperties(configuration, TreePathConvertor.GetSiteConverter(webSiteChannelId), out anythingChanged);
            return JsonConvert.SerializeObject(configuration);
        }

        anythingChanged = false;
        return documentPageTemplateConfiguration;
    }

    private string DeferredPatchVisualBuilderWidgets(string documentPageBuilderWidgets, int webSiteChannelId, out bool anythingChanged)
    {
        if (!string.IsNullOrWhiteSpace(documentPageBuilderWidgets))
        {
            var patched = VisualBuilderWidgetsPatcher.DeferredPatchConfiguration(
                JsonConvert.DeserializeObject<EditableAreasConfiguration>(documentPageBuilderWidgets),
                TreePathConvertor.GetSiteConverter(webSiteChannelId),
                out anythingChanged
            );
            return JsonConvert.SerializeObject(patched);
        }

        anythingChanged = false;
        return documentPageBuilderWidgets;
    }

    #endregion
}
