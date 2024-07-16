using CMS.FormEngine;
using CMS.OnlineForms;

using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.K11.Models;

namespace Migration.Toolkit.Core.K11.Mappers;

public class CmsFormMapper(
    ILogger<CmsFormMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol)
    : EntityMapperBase<CmsForm, BizFormInfo>(logger, primaryKeyMappingContext, protocol)
{
    protected override BizFormInfo? CreateNewInstance(CmsForm source, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var newBizFormInfo = BizFormInfo.New();
        newBizFormInfo.FormGUID = source.FormGuid;
        return newBizFormInfo;
    }

    protected override BizFormInfo MapInternal(CmsForm source, BizFormInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.FormDisplayName = source.FormDisplayName;
        target.FormName = source.FormName;
        target.FormItems = source.FormItems;
        target.FormReportFields = source.FormReportFields;
        target.FormSubmitButtonText = source.FormSubmitButtonText;
        target.FormAccess = source.FormAccess.AsEnum<FormAccessEnum>();
        target.FormSubmitButtonImage = source.FormSubmitButtonImage;
        target.FormLastModified = source.FormLastModified;
        target.FormLogActivity = source.FormLogActivity.UseKenticoDefault();
        // target.FormBuilderLayout = source.FormBuilderLayout;

        if (mappingHelper.TranslateRequiredId<CmsClass>(c => c.ClassId, source.FormClassId, out int formClassId))
        {
            target.FormClassID = formClassId;
        }

        return target;
    }
}

public class CmsFormMapperEf(ILogger<CmsFormMapperEf> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : EntityMapperBase<CmsForm, KXP.Models.CmsForm>(logger, pkContext, protocol)
{
    protected override KXP.Models.CmsForm? CreateNewInstance(CmsForm source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override KXP.Models.CmsForm MapInternal(CmsForm source, KXP.Models.CmsForm target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.FormDisplayName = source.FormDisplayName;
        target.FormName = source.FormName;
        // target.FormSendToEmail = source.FormSendToEmail;
        // target.FormSendFromEmail = source.FormSendFromEmail;
        // target.FormEmailSubject = source.FormEmailSubject;
        // target.FormEmailTemplate = source.FormEmailTemplate;
        // target.FormEmailAttachUploadedDocs = source.FormEmailAttachUploadedDocs;
        target.FormItems = source.FormItems;
        target.FormReportFields = source.FormReportFields;
        target.FormSubmitButtonText = source.FormSubmitButtonText;
        // target.FormConfirmationEmailField = source.FormConfirmationEmailField;
        // target.FormConfirmationTemplate = source.FormConfirmationTemplate;
        // target.FormConfirmationSendFromEmail = source.FormConfirmationSendFromEmail;
        // target.FormConfirmationEmailSubject = source.FormConfirmationEmailSubject;
        target.FormAccess = source.FormAccess;
        target.FormSubmitButtonImage = source.FormSubmitButtonImage;
        target.FormGuid = source.FormGuid;
        target.FormLastModified = source.FormLastModified;
        target.FormLogActivity = source.FormLogActivity ?? false;
        // target.FormBuilderLayout = source.FormBuilderLayout;

        if (mappingHelper.TranslateRequiredId<CmsClass>(c => c.ClassId, source.FormClassId, out int classId))
        {
            target.FormClassId = classId;
        }

        return target;
    }
}
