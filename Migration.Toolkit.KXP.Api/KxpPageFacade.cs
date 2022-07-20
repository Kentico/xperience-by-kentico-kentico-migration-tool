namespace Migration.Toolkit.KXP.Api;

using CMS.Helpers;

public record IsPublishedArguments(
    bool? DocumentCanBePublished,
    int? DocumentWorkflowStepId,
    bool? DocumentIsArchived,
    int? DocumentCheckedOutVersionHistoryId,
    int? DocumentPublishedVersionHistoryId,
    DateTime? DocumentPublishFrom,
    DateTime? DocumentPublishTo
);

public class KxpPageFacade
{
    public bool IsPublished(IsPublishedArguments arg)
    {
        // code copied from api:
        // DocumentHelper.GetPublished((IDataContainer)this);
        
        var (documentCanBePublished, documentWorkflowStepId, documentIsArchived, documentCheckedOutVersionHistoryId, documentPublishedVersionHistoryId, documentPublishFrom, documentPublishTo) = arg;
        if (documentCanBePublished.HasValue)
        {
            if (!documentCanBePublished.Value)
                return false;
        }
        else
        {
            if (documentWorkflowStepId == null || documentIsArchived.GetValueOrDefault(false) || documentCheckedOutVersionHistoryId != null != (documentPublishedVersionHistoryId != null))
                return false;
        }
        
        return DateTime.Now >= ValidationHelper.GetDateTime(documentPublishFrom, DateTime.MinValue) && DateTime.Now <= ValidationHelper.GetDateTime(documentPublishTo, DateTime.MaxValue);
    }
}