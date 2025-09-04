using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.Core;
using CMS.Core.Internal;
using Kentico.Xperience.UMT.Model;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common.Builders;
using Migration.Tool.Common.Helpers;

namespace Migration.Tool.Common.Abstractions;

public class DefaultCustomTableClassMappingHandler(ILogger<DefaultCustomTableClassMappingHandler> logger) : IClassMappingHandler
{
    private readonly IContentItemCodeNameProvider contentItemCodeNameProvider = Service.Resolve<IContentItemCodeNameProvider>();

    public virtual void EnsureContentItem(ContentItemModel contentItemModel, CustomTableMappingHandlerContext context)
    {
        if (context.Values.TryGetValue("ItemGUID", out object? mbItemGuid) && mbItemGuid is Guid itemGuid)
        {
            contentItemModel.ContentItemGUID = itemGuid;
            logger.LogTrace("ContentItemGUID '{ContentItemGuid}' assigned from ItemGUID", contentItemModel.ContentItemGUID);
        }
        if (context.Values.TryGetValue("ItemID", out object? mbItemId) && mbItemId is int itemId)
        {
            if (contentItemModel.ContentItemGUID is null)
            {
                contentItemModel.ContentItemGUID = GuidHelper.CreateContentItemGuid($"{context.SourceClassName}|{itemId}");
                logger.LogTrace("ContentItemGUID '{ContentItemGuid}' derived from ItemID {ItemId}", contentItemModel.ContentItemGUID, itemId);
            }

            if (contentItemModel.ContentItemName is null)
            {
                // intl todo - async
                contentItemModel.ContentItemName = contentItemCodeNameProvider.Get($"{context.SourceClassName} - {itemId}").GetAwaiter().GetResult();
                logger.LogTrace("ContentItemName '{ContentItemName}' derived from ItemID", contentItemModel.ContentItemName);
            }
        }
        else
        {
            // should lead to exception
        }
    }

    public virtual void EnsureContentItemCommonData(ContentItemCommonDataModel commonDataModel, CustomTableMappingHandlerContext context)
    {
        string guidKey = $"{context.SourceClassName}|{commonDataModel.ContentItemCommonDataContentItemGuid}|{commonDataModel.ContentItemCommonDataContentLanguageGuid}";
        commonDataModel.ContentItemCommonDataGUID = GuidHelper.CreateContentItemCommonDataGuid(guidKey);
        logger.LogTrace("ContentItemCommonDataGUID '{ContentItemCommonDataGUID}' derived from key '{Key}'", commonDataModel.ContentItemCommonDataGUID, guidKey);

        commonDataModel.ContentItemCommonDataIsLatest = true;
    }

    public virtual void EnsureContentItemLanguageMetadata(ContentItemLanguageMetadataModel languageMetadataInfo, CustomTableMappingHandlerContext context)
    {
        string guidKey = $"{context.SourceClassName}|{languageMetadataInfo.ContentItemLanguageMetadataContentItemGuid}|{languageMetadataInfo.ContentItemLanguageMetadataContentLanguageGuid}";
        languageMetadataInfo.ContentItemLanguageMetadataGUID = GuidHelper.CreateContentItemLanguageMetadataGuid(guidKey);
        logger.LogTrace("ContentItemLanguageMetadataGUID '{ContentItemCommonDataGUID}' derived from key '{Key}'", languageMetadataInfo.ContentItemLanguageMetadataGUID, guidKey);

        if (languageMetadataInfo.ContentItemLanguageMetadataDisplayName is null)
        {
            if (context.Values.TryGetValue("ItemID", out object? mbItemId) && mbItemId is int itemId)
            {
                languageMetadataInfo.ContentItemLanguageMetadataDisplayName = $"{context.SourceClassName} - {itemId}";
                logger.LogTrace("ContentItemLanguageMetadataDisplayName '{ContentItemLanguageMetadataDisplayName}' derived from ItemID", languageMetadataInfo.ContentItemLanguageMetadataDisplayName);
            }
        }

        if (languageMetadataInfo.ContentItemLanguageMetadataCreatedWhen is null)
        {
            if (context.Values.TryGetValue("ItemCreatedWhen", out object? mbDate) && mbDate is DateTime date)
            {
                languageMetadataInfo.ContentItemLanguageMetadataCreatedWhen = date;
                logger.LogTrace("ContentItemLanguageMetadataCreatedWhen '{ContentItemLanguageMetadataCreatedWhen}' derived from 'ItemCreatedWhen'", languageMetadataInfo.ContentItemLanguageMetadataCreatedWhen);
            }
            else
            {
                languageMetadataInfo.ContentItemLanguageMetadataCreatedWhen = Service.Resolve<IDateTimeNowService>().GetDateTimeNow();
                logger.LogTrace("ContentItemLanguageMetadataCreatedWhen '{ContentItemLanguageMetadataCreatedWhen}' set to now date", languageMetadataInfo.ContentItemLanguageMetadataCreatedWhen);
            }
        }

        if (languageMetadataInfo.ContentItemLanguageMetadataModifiedWhen is null)
        {
            if (context.Values.TryGetValue("ItemModifiedWhen", out object? mbDate) && mbDate is DateTime date)
            {
                languageMetadataInfo.ContentItemLanguageMetadataModifiedWhen = date;
                logger.LogTrace("ContentItemLanguageMetadataModifiedWhen '{ContentItemLanguageMetadataModifiedWhen}' derived from 'ItemModifiedWhen'", languageMetadataInfo.ContentItemLanguageMetadataModifiedWhen);
            }
            else
            {
                languageMetadataInfo.ContentItemLanguageMetadataModifiedWhen = Service.Resolve<IDateTimeNowService>().GetDateTimeNow();
                logger.LogTrace("ContentItemLanguageMetadataModifiedWhen '{ContentItemLanguageMetadataModifiedWhen}' set to now date", languageMetadataInfo.ContentItemLanguageMetadataModifiedWhen);
            }
        }
    }

    public virtual async Task<List<CustomTableDataLanguageVersion>> ProduceLanguageVersions(CustomTableMappingHandlerContext context)
    {
        var defaultContentLanguage = await Service.Resolve<IContentLanguageRetriever>().GetDefaultContentLanguage();
        return
        [
            new CustomTableDataLanguageVersion(defaultContentLanguage, context.Values)
        ];

        // or distribute to desired languages with own selection:
        // return ContentLanguageInfo.Provider.Get().AsEnumerable().Select(cl =>
        // {
        //     return new CustomTableDataLanguageVersion(cl, context.Values);
        // }).ToList();
    }

    public virtual int? GetCreatedByUserId(CustomTableMappingHandlerContext context, string sourceClassName, string sourceClassFormDefinition)
    {
        if (context.Values.TryGetValue("ItemCreatedByUserID", out object? mbUserId) && mbUserId is int userId)
        {
            return userId;
        }
        else
        {
            return null;
        }
    }

    public virtual int? GetModifiedByUserId(CustomTableMappingHandlerContext context, string sourceClassName, string sourceClassFormDefinition)
    {
        if (context.Values.TryGetValue("ItemModifiedByUserID", out object? mbUserId) && mbUserId is int userId)
        {
            return userId;
        }
        else
        {
            return null;
        }
    }
}
