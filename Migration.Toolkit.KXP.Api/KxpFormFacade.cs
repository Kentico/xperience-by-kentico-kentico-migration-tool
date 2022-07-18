namespace Migration.Toolkit.KXP.Api;

using CMS.OnlineForms;
using CMS.SiteProvider;
using Microsoft.Extensions.Logging;

public class KxpFormFacade
{
    private readonly ILogger<KxpClassFacade> _logger;
    private readonly KxpApiInitializer _kxpApiInitializer;

    public KxpFormFacade(ILogger<KxpClassFacade> logger, KxpApiInitializer kxpApiInitializer)
    {
        _logger = logger;
        _kxpApiInitializer = kxpApiInitializer;

        _kxpApiInitializer.EnsureApiIsInitialized();
    }
    
    public void SetForm(string formDisplayName, string formName, string tableName, SiteInfo siteInfo)
    {
        // new TableManager().CreateTable();
        // BizFormInfoProvider
        BizFormHelper.Create(formDisplayName, formName, tableName, siteInfo);
        // BizFormInfoProvider.CreateBizFormDataClass();
        // BizFormInfoProvider.CreateBizFormDataClass()
    }
    
    public BizFormInfo GetForm(Guid formGuid, int siteId)
    {
        return BizFormInfoProvider.GetBizFormInfoByGUID(formGuid, siteId);
    }
}