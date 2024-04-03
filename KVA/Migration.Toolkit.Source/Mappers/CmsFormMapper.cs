namespace Migration.Toolkit.Source.Mappers;

using CMS.FormEngine;
using CMS.OnlineForms;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Source.Contexts;
using Migration.Toolkit.Source.Model;

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
        }
        if (mappingHelper.TranslateRequiredId<ICmsClass>(c => c.ClassID, source.FormClassID, out var formClassId))
        {
            target.FormClassID = formClassId;
        }

        return target;
    }
}

public class CmsFormMapperEf : EntityMapperBase<ICmsForm, Migration.Toolkit.KXP.Models.CmsForm>
{
    public CmsFormMapperEf(ILogger<CmsFormMapperEf> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : base(logger, pkContext, protocol)
    {
    }

    protected override Migration.Toolkit.KXP.Models.CmsForm? CreateNewInstance(ICmsForm source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override Migration.Toolkit.KXP.Models.CmsForm MapInternal(ICmsForm source, Migration.Toolkit.KXP.Models.CmsForm target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
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
        target.FormGuid = source.FormGUID;
        target.FormLastModified = source.FormLastModified;
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
        }

        // TODO tk: 2022-05-20 new deduce: target.FormAfterSubmitMode = source.FormAfterSubmitMode;
        // TODO tk: 2022-05-20 new deduce: target.FormAfterSubmitRelatedValue = source.FormAfterSubmitRelatedValue;

        if (mappingHelper.TranslateRequiredId<ICmsClass>(c => c.ClassID, source.FormClassID, out var classId))
        {
            target.FormClassId = classId;
        }

        return target;
    }
}