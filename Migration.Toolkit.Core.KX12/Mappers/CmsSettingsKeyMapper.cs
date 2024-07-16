
using System.Diagnostics;

using CMS.DataEngine;

using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX12.Contexts;

namespace Migration.Toolkit.Core.KX12.Mappers;
public class CmsSettingsKeyMapper : EntityMapperBase<KX12M.CmsSettingsKey, SettingsKeyInfo>
{
    private const string SOURCE_KEY_NAME = "CMSDefaultUserID";

    public CmsSettingsKeyMapper(ILogger<CmsSettingsKeyMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : base(logger, pkContext, protocol)
    {

    }

    protected override SettingsKeyInfo CreateNewInstance(KX12M.CmsSettingsKey source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override SettingsKeyInfo MapInternal(KX12M.CmsSettingsKey source, SettingsKeyInfo target, bool newInstance,
        MappingHelper mappingHelper, AddFailure addFailure)
    {
        if (newInstance)
        {
            target.KeyName = source.KeyName;
            target.KeyDisplayName = source.KeyDisplayName;
            target.KeyDescription = source.KeyDescription;
            target.KeyType = source.KeyType;
            target.KeyGUID = source.KeyGuid;
            target.KeyValidation = source.KeyValidation;
            target.KeyEditingControlPath = source.KeyEditingControlPath;
            target.KeyFormControlSettings = source.KeyFormControlSettings;
            target.KeyExplanationText = source.KeyExplanationText;
        }
        else
        {
            target.KeyName = source.KeyName;
            target.KeyDescription = source.KeyDescription;
            target.KeyType = source.KeyType;
            target.KeyValidation = source.KeyValidation;
            target.KeyEditingControlPath = source.KeyEditingControlPath;
            target.KeyFormControlSettings = source.KeyFormControlSettings;
            target.KeyExplanationText = source.KeyExplanationText;
        }

        // special migrations for keys
        switch (source.KeyName)
        {
            case SOURCE_KEY_NAME:
            {
                target.KeyValue = int.TryParse(source.KeyValue, out int cmsDefaultUserId)
                    ? mappingHelper.TranslateRequiredId<KX12M.CmsUser>(u => u.UserId, cmsDefaultUserId, out int targetCmsDefaultUserId)
                        ? targetCmsDefaultUserId.ToString()
                        : source.KeyValue
                    : source.KeyValue;
                break;
            }
            default:
                target.KeyValue = source.KeyValue;
                break;
        }

        Debug.Assert(!source.SiteId.HasValue, "!source.SiteId.HasValue");
        target.KeyLastModified = source.KeyLastModified;
        return target;
    }
}
