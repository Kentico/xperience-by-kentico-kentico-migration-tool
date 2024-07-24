using System.Diagnostics;

using CMS.DataEngine;

using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.K11.Models;

namespace Migration.Toolkit.Core.K11.Mappers;

public class CmsSettingsKeyMapper(ILogger<CmsSettingsKeyMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : EntityMapperBase<CmsSettingsKey, SettingsKeyInfo>(logger, pkContext, protocol)
{
    private const string SOURCE_KEY_NAME = "CMSDefaultUserID";

    protected override SettingsKeyInfo CreateNewInstance(CmsSettingsKey source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override SettingsKeyInfo MapInternal(CmsSettingsKey source, SettingsKeyInfo target, bool newInstance,
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
                    ? mappingHelper.TranslateRequiredId<CmsUser>(u => u.UserId, cmsDefaultUserId, out int targetCmsDefaultUserId)
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
