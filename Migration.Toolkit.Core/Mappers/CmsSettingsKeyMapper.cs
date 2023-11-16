// using Microsoft.Extensions.Logging;
// using Migration.Toolkit.Core.Abstractions;
// using Migration.Toolkit.Core.Contexts;
// using Migration.Toolkit.Core.MigrationProtocol;
//
// namespace Migration.Toolkit.Core.Mappers;
//
// using Migration.Toolkit.KXP.Models;
//
// public class CmsSettingsKeyMapper : EntityMapperBase<Migration.Toolkit.KX13.Models.CmsSettingsKey, CmsSettingsKey>
// {
//     private const string SOURCE_KEY_NAME = "CMSDefaultUserID";
//
//     public CmsSettingsKeyMapper(ILogger<CmsSettingsKeyMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : base(logger, pkContext, protocol)
//     {
//
//     }
//
//     protected override CmsSettingsKey CreateNewInstance(KX13.Models.CmsSettingsKey source, MappingHelper mappingHelper, AddFailure addFailure) => new();
//
//     protected override CmsSettingsKey MapInternal(KX13.Models.CmsSettingsKey source, CmsSettingsKey target, bool newInstance,
//         MappingHelper mappingHelper, AddFailure addFailure)
//     {
//         if (newInstance)
//         {
//             target.KeyName = source.KeyName;
//             target.KeyDisplayName = source.KeyDisplayName;
//             target.KeyDescription = source.KeyDescription;
//             target.KeyType = source.KeyType;
//             target.KeyGuid = source.KeyGuid;
//             // target.KeyOrder = source.KeyOrder;
//             target.KeyDefaultValue = source.KeyDefaultValue;
//             target.KeyValidation = source.KeyValidation;
//             target.KeyEditingControlPath = source.KeyEditingControlPath;
//             // target.KeyIsGlobal = source.KeyIsGlobal;
//             // target.KeyIsCustom = source.KeyIsCustom;
//             target.KeyFormControlSettings = source.KeyFormControlSettings;
//             target.KeyExplanationText = source.KeyExplanationText;
//         }
//         else
//         {
//             target.KeyName = source.KeyName;
//             // target.KeyDisplayName = source.KeyDisplayName;
//             target.KeyDescription = source.KeyDescription;
//             target.KeyType = source.KeyType;
//             target.KeyGuid = source.KeyGuid;
//             // target.KeyOrder = source.KeyOrder;
//             target.KeyDefaultValue = source.KeyDefaultValue;
//             target.KeyValidation = source.KeyValidation;
//             target.KeyEditingControlPath = source.KeyEditingControlPath;
//             // target.KeyIsGlobal = source.KeyIsGlobal;
//             // target.KeyIsCustom = source.KeyIsCustom;
//             target.KeyFormControlSettings = source.KeyFormControlSettings;
//             target.KeyExplanationText = source.KeyExplanationText;
//         }
//
//         // special migrations for keys
//         switch (source.KeyName)
//         {
//             case SOURCE_KEY_NAME:
//             {
//                 target.KeyValue = int.TryParse(source.KeyValue, out var cmsDefaultUserId)
//                     ? mappingHelper.TranslateRequiredId<KX13.Models.CmsUser>(u => u.UserId, cmsDefaultUserId, out var targetCmsDefaultUserId)
//                         ? targetCmsDefaultUserId.ToString()
//                         : source.KeyValue
//                     : source.KeyValue;
//                 break;
//             }
//             default:
//                 target.KeyValue = source.KeyValue;
//                 break;
//         }
//
//         if (mappingHelper.TranslateIdAllowNulls<KX13.Models.CmsSite>(s => s.SiteId, source.SiteId, out var siteId))
//         {
//             target.SiteId = siteId;
//         }
//         target.KeyLastModified = source.KeyLastModified;
//         // target.KeyIsHidden = source.KeyIsHidden; - not mapped / internal
//
//         // if (source.KeyCategory != null)
//         // {
//         //     switch (_cmsCategoryMapper.Map(source.KeyCategory, target.KeyCategory))
//         //     {
//         //         case { Success: true } result:
//         //         {
//         //             target.KeyCategory = result.Item;
//         //             break;
//         //         }
//         //         case { Success: false } result:
//         //         {
//         //             addFailure(new MapperResultFailure<KXO.Models.CmsSettingsKey>(result.HandbookReference));
//         //             break;
//         //         }
//         //     }
//         // }
//
//         return target;
//     }
// }