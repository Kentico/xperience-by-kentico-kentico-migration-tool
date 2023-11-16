// namespace Migration.Toolkit.Core.MappersNG;
//
// using CMS.ContentEngine;
// using Microsoft.Extensions.Logging;
// using Migration.Toolkit.Core.Abstractions;
// using Migration.Toolkit.Core.Contexts;
// using Migration.Toolkit.Core.MigrationProtocol;
//
// public record CultureMapperSource(KX13M.CmsCulture CmsCulture, bool CultureIsDefault);
// public class CultureMapper: EntityMapperBase<CultureMapperSource, ContentLanguageInfo>
// {
//     public CultureMapper(ILogger logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : base(logger, pkContext, protocol)
//     {
//
//     }
//
//     protected override ContentLanguageInfo? CreateNewInstance(CultureMapperSource source, MappingHelper mappingHelper, AddFailure addFailure)
//     {
//         return new ContentLanguageInfo();
//     }
//
//     protected override ContentLanguageInfo MapInternal(CultureMapperSource source, ContentLanguageInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
//     {
//         var (cmsCulture, cultureIsDefault) = source;
//
//         // target.ContentLanguageID = 0;
//         target.ContentLanguageDisplayName = cmsCulture.CultureName;
//         target.ContentLanguageName = cmsCulture.CultureCode;
//         target.ContentLanguageIsDefault = cultureIsDefault;
//         target.ContentLanguageFallbackContentLanguageID = 0; // TODO tomas.krch: 2023-11-01 decision - which culture is default
//         target.ContentLanguageCultureFormat = cmsCulture.CultureCode;
//         target.ContentLanguageGUID = cmsCulture.CultureGuid;
//     }
// }