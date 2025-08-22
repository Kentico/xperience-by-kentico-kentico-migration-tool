using CMS.ContentEngine;
using Kentico.Xperience.UMT.Model;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common.Builders;

namespace Migration.Tool.Common.Abstractions;

public interface IClassMappingHandler
{
    void EnsureContentItem(ContentItemModel contentItemModel, CustomTableMappingHandlerContext context);
    void EnsureContentItemCommonData(ContentItemCommonDataModel commonDataModel, CustomTableMappingHandlerContext context);
    void EnsureContentItemLanguageMetadata(ContentItemLanguageMetadataModel languageMetadataInfo, CustomTableMappingHandlerContext context);
    Task<List<CustomTableDataLanguageVersion>> ProduceLanguageVersions(CustomTableMappingHandlerContext context);
    int? GetCreatedByUserId(CustomTableMappingHandlerContext context, string sourceClassName, string sourceClassFormDefinition);
    int? GetModifiedByUserId(CustomTableMappingHandlerContext context, string sourceClassName, string sourceClassFormDefinition);
}

public record CustomTableDataLanguageVersion(ContentLanguageInfo Language, Dictionary<string, object?> Values);

public class ClassMappingHandlerWrapper(IClassMappingHandler impl, ILogger logger) : IClassMappingHandler
{
    private readonly string implTypeFullName = impl.GetType().FullName!;

    public void EnsureContentItem(ContentItemModel contentItemModel, CustomTableMappingHandlerContext context)
    {
        impl.EnsureContentItem(contentItemModel, context);

        if (string.IsNullOrWhiteSpace(contentItemModel.ContentItemName))
        {
            throw new InvalidOperationException($"Implementation '{implTypeFullName}.EnsureContentItem' did not set required value ContentItemName.");
        }

        if (contentItemModel.ContentItemIsSecured is null)
        {
            throw new InvalidOperationException($"Implementation '{implTypeFullName}.EnsureContentItem' did not set required value ContentItemIsSecured.");
        }

        if (contentItemModel.ContentItemGUID is null)
        {
            throw new InvalidOperationException($"Implementation '{implTypeFullName}.EnsureContentItem' did not set required value ContentItemGUID. Produce value in deterministic manner so migration can be repeated or create new guid, but then migration is required to be run on clean target instance everytime.");
        }
    }

    public void EnsureContentItemLanguageMetadata(ContentItemLanguageMetadataModel languageMetadataInfo, CustomTableMappingHandlerContext context)
    {
        impl.EnsureContentItemLanguageMetadata(languageMetadataInfo, context);

        if (languageMetadataInfo.ContentItemLanguageMetadataGUID is null)
        {
            throw new InvalidOperationException($"Implementation '{implTypeFullName}.EnsureContentItemLanguageMetadata' did not set required value ContentItemLanguageMetadataGUID. Produce value in deterministic manner so migration can be repeated or create new guid, but then migration is required to be run on clean target instance everytime.");
        }

        if (languageMetadataInfo.ContentItemLanguageMetadataDisplayName is null)
        {
            throw new InvalidOperationException($"Implementation '{implTypeFullName}.EnsureContentItemLanguageMetadata' did not set required value ContentItemLanguageMetadataDisplayName");
        }

        if (languageMetadataInfo.ContentItemLanguageMetadataCreatedWhen is null)
        {
            throw new InvalidOperationException($"Implementation '{implTypeFullName}.EnsureContentItemLanguageMetadata' did not set required value ContentItemLanguageMetadataCreatedWhen");
        }

        if (languageMetadataInfo.ContentItemLanguageMetadataModifiedWhen is null)
        {
            throw new InvalidOperationException($"Implementation '{implTypeFullName}.EnsureContentItemLanguageMetadata' did not set required value ContentItemLanguageMetadataModifiedWhen");
        }
    }

    public void EnsureContentItemCommonData(ContentItemCommonDataModel commonDataModel, CustomTableMappingHandlerContext context)
    {
        impl.EnsureContentItemCommonData(commonDataModel, context);

        if (commonDataModel.ContentItemCommonDataGUID is null)
        {
            throw new InvalidOperationException($"Implementation '{implTypeFullName}.EnsureContentItemCommonData' did not set required value ContentItemCommonDataGUID. Produce value in deterministic manner so migration can be repeated or create new guid, but then migration is required to be run on clean target instance everytime.");
        }
    }

    public async Task<List<CustomTableDataLanguageVersion>> ProduceLanguageVersions(CustomTableMappingHandlerContext context)
    {
        switch (await impl.ProduceLanguageVersions(context))
        {
            case { Count: 0 }:
            case null:
            {
                throw new InvalidOperationException($"Implementation '{implTypeFullName}.ProduceLanguageVersions' produced no language version, please fix custom implementation");
            }
            case { Count: > 0 } languageVersions:
            {
                var result = await impl.ProduceLanguageVersions(context);
                logger.LogTrace("{ImplTypeName}.ProduceLanguageVersions '{UserId}'", implTypeFullName, string.Join(",", languageVersions.Select(x => x.Language)));
                return result;
            }

            default:
                throw new NotImplementedException("Internal error 0376d796-bc54-4bf6-bc0c-4f320a1ba7ed. Report this issue.");
        }
    }

    public int? GetCreatedByUserId(CustomTableMappingHandlerContext context, string sourceClassName, string sourceClassFormDefinition)
    {
        int? userId = impl.GetCreatedByUserId(context, sourceClassName, sourceClassFormDefinition);
        logger.LogTrace("{ImplTypeName}.GetCreatedByUserID '{UserId}'", implTypeFullName, userId);
        return userId;
    }

    public int? GetModifiedByUserId(CustomTableMappingHandlerContext context, string sourceClassName, string sourceClassFormDefinition)
    {
        int? userId = impl.GetModifiedByUserId(context, sourceClassName, sourceClassFormDefinition);
        logger.LogTrace("{ImplTypeName}.GetModifiedByUserID '{UserId}'", implTypeFullName, userId);
        return userId;
    }
}
