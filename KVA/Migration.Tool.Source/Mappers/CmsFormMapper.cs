using CMS.FormEngine;
using CMS.OnlineForms;
using CMS.Workspaces;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.ContentItemOptions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Common.Services;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Mappers.FormMapperDirectives;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Mappers;

public class CmsFormMapper(
    ILogger<CmsFormMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    WorkspaceService workspaceService,
    ModelFacade modelFacade,
    IEnumerable<FormDirectorBase> directors)
    : EntityMapperBase<ICmsForm, BizFormInfo>(logger, primaryKeyMappingContext, protocol)
{
    protected override BizFormInfo? CreateNewInstance(ICmsForm source, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var newBizFormInfo = BizFormInfo.New();
        newBizFormInfo.FormGUID = source.FormGUID;
        return newBizFormInfo;
    }

    protected override BizFormInfo MapInternal(ICmsForm source, BizFormInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.FormDisplayName = source.FormDisplayName;
        target.FormName = source.FormName;
        target.FormItems = source.FormItems;
        target.FormReportFields = source.FormReportFields;
        target.FormSubmitButtonText = source.FormSubmitButtonText;
        target.FormAccess = source.FormAccess.AsEnum<FormAccessEnum>();
        target.FormSubmitButtonImage = source.FormSubmitButtonImage;
        target.FormLastModified = source.FormLastModified;
        target.FormGUID = source.FormGUID;

        var actionProvider = new FormActionProvider();
        var formSource = new FormSource(source, modelFacade.SelectById<ICmsSite>(source.FormSiteID));
        foreach (var director in directors)
        {
            director.Direct(formSource, actionProvider);
        }

        if (actionProvider.WorkspaceOptions is { } workspaceOptions)
        {
            // Workspace explicitly chosen by director is always applied
            if (ResolveWorkspace(workspaceOptions) is { } workspace)
            {
                target.FormWorkspaceID = workspace.WorkspaceID;
            }
            else
            {
                addFailure(HandbookReferences
                    .MissingRequiredDependency<WorkspaceInfo>(nameof(target.FormWorkspaceID), workspaceOptions)
                    .AsFailure<BizFormInfo>()
                );
            }
        }
        // Assign fallback workspace only to forms without one, so that forms reassigned in target instance are not moved back on repeated migration
        else if (newInstance || target.FormWorkspaceID == 0)
        {
            target.FormWorkspaceID = workspaceService.FallbackWorkspace.Value.WorkspaceID;
        }

        switch (source)
        {
            case CmsFormK11 s:
            {
                target.FormLogActivity = s.FormLogActivity.UseKenticoDefault();
                break;
            }
            case CmsFormK12 s:
            {
                target.FormLogActivity = s.FormLogActivity.UseKenticoDefault();
                target.FormBuilderLayout = s.FormBuilderLayout;
                break;
            }
            case CmsFormK13 s:
            {
                target.FormLogActivity = s.FormLogActivity;
                target.FormBuilderLayout = s.FormBuilderLayout;
                break;
            }

            default:
                break;
        }

        if (mappingHelper.TranslateRequiredId<ICmsClass>(c => c.ClassID, source.FormClassID, out int formClassId))
        {
            target.FormClassID = formClassId;
        }

        return target;
    }

    private WorkspaceInfo? ResolveWorkspace(WorkspaceOptions options) =>
        workspaceService.EnsureWorkspace(options) is { } workspaceGuid
            ? workspaceService.GetWorkspace(workspaceGuid)
            : null;
}
