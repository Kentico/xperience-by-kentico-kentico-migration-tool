using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Contexts;

namespace Migration.Toolkit.Core.Mappers;

using System.Diagnostics;
using CMS.DataEngine;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;

public class CmsSettingsKeyMapper : EntityMapperBase<Migration.Toolkit.KX13.Models.CmsSettingsKey, SettingsKeyInfo>
{
    private const string SOURCE_KEY_NAME = "CMSDefaultUserID";

    public CmsSettingsKeyMapper(ILogger<CmsSettingsKeyMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : base(logger, pkContext, protocol)
    {

    }

    protected override SettingsKeyInfo CreateNewInstance(KX13.Models.CmsSettingsKey source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override SettingsKeyInfo MapInternal(KX13.Models.CmsSettingsKey source, SettingsKeyInfo target, bool newInstance,
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
                target.KeyValue = int.TryParse(source.KeyValue, out var cmsDefaultUserId)
                    ? mappingHelper.TranslateRequiredId<KX13.Models.CmsUser>(u => u.UserId, cmsDefaultUserId, out var targetCmsDefaultUserId)
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