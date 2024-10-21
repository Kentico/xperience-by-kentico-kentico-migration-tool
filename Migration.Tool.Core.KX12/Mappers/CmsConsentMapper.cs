using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

using CMS.ContentEngine;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX12.Contexts;
using Migration.Tool.KXP.Models;

namespace Migration.Tool.Core.KX12.Mappers;

public class CmsConsentMapper : EntityMapperBase<KX12M.CmsConsent, CmsConsent>
{
    public CmsConsentMapper(ILogger<CmsConsentMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : base(logger, pkContext, protocol)
    {
    }

    protected override CmsConsent? CreateNewInstance(KX12M.CmsConsent source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override CmsConsent MapInternal(KX12M.CmsConsent source, CmsConsent target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ConsentDisplayName = source.ConsentDisplayName;
        var defaultContentLanguageInfo = ContentLanguageInfo.Provider.Get().WhereEquals(nameof(ContentLanguageInfo.ContentLanguageIsDefault), true).FirstOrDefault() ?? throw new InvalidCastException("Missing default content language");
        target.ConsentName = source.ConsentName;
        target.ConsentContent = ConsentContentPatcher.PatchConsentContent(source.ConsentContent, defaultContentLanguageInfo);
        target.ConsentGuid = source.ConsentGuid;
        target.ConsentLastModified = source.ConsentLastModified;
        target.ConsentHash = source.ConsentHash;

        return target;
    }
}

static file class ConsentContentPatcher
{
    public static string PatchConsentContent(string content, ContentLanguageInfo defaultContentLanguage)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return content;
        }

        XDocument doc;
        try
        {
            doc = XDocument.Parse(content);
        }
        catch (Exception)
        {
            // cannot patch xml that cannot be parsed
            return content;
        }

        foreach (var cultureCodeElement in doc.XPathSelectElements("//CultureCode"))
        {
            cultureCodeElement.Name = "LanguageName";
            if (!string.Equals(defaultContentLanguage.ContentLanguageName, cultureCodeElement.Value, StringComparison.InvariantCultureIgnoreCase))
            {
                // mLogger.LogWarning($"Consent '{consentInfo.ConsentName}' has unknown content language set '{cultureCodeElement.Value}'");
            }

            // if elements are not swapped, FULLTEXT is not shown in UI
            var p = cultureCodeElement.NextNode;
            if (p is XElement e && e.Name == "FullText")
            {
                p.ReplaceWith(cultureCodeElement);
                cultureCodeElement.ReplaceWith(p);
            }
        }

        var builder = new StringBuilder();
        using (var writer = new CMS.IO.StringWriter(builder))
        {
            doc.Save(writer);
        }

        return builder.ToString();
    }
}
