using CMS.FormEngine;
using CMS.OnlineForms;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Mappers;

public class CmsFormMapper(
    ILogger<CmsFormMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol)
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
}
