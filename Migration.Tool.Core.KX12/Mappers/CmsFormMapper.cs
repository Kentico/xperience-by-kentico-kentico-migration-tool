using CMS.FormEngine;
using CMS.OnlineForms;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX12.Contexts;
using Migration.Tool.KXP.Models;

namespace Migration.Tool.Core.KX12.Mappers;

public class CmsFormMapper : EntityMapperBase<KX12M.CmsForm, BizFormInfo>
{
    public CmsFormMapper(
        ILogger<CmsFormMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    ) : base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override BizFormInfo? CreateNewInstance(KX12M.CmsForm source, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var newBizFormInfo = BizFormInfo.New();
        newBizFormInfo.FormGUID = source.FormGuid;
        return newBizFormInfo;
    }

    protected override BizFormInfo MapInternal(KX12M.CmsForm source, BizFormInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
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
        target.FormBuilderLayout = source.FormBuilderLayout;

        if (mappingHelper.TranslateRequiredId<KX12M.CmsClass>(c => c.ClassId, source.FormClassId, out int formClassId))
        {
            target.FormClassID = formClassId;
        }

        return target;
    }
}

public class CmsFormMapperEf : EntityMapperBase<KX12M.CmsForm, CmsForm>
{
    public CmsFormMapperEf(ILogger<CmsFormMapperEf> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : base(logger, pkContext, protocol)
    {
    }

    protected override CmsForm? CreateNewInstance(KX12M.CmsForm source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override CmsForm MapInternal(KX12M.CmsForm source, CmsForm target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
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
        target.FormBuilderLayout = source.FormBuilderLayout;

        if (mappingHelper.TranslateRequiredId<KX12M.CmsClass>(c => c.ClassId, source.FormClassId, out int classId))
        {
            target.FormClassId = classId;
        }

        return target;
    }
}
