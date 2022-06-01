using CMS.DocumentEngine;

namespace Migration.Toolkit.KXO.Api;

public class KxoPageFacade
{
    public KxoPageFacade()
    {
        
    }

    public void SetPage()
    {
        // Gets the current site's root "/" page, which will serve as the parent page
        TreeNode parentPage = new DocumentQuery<TreeNode>()
            .Path("/", PathTypeEnum.Single)
            .OnSite("MySite")
            .Culture("en-us")
            .TopN(1)
            .FirstOrDefault();

        if (parentPage != null)
        {
            // Creates a new page of the custom page type
            TreeNode newPage = TreeNode.New("Custom.Article");

            // Sets the properties of the new page
            newPage.DocumentName = "Articles";
            newPage.DocumentCulture = "en-us";

            if (newPage.IsCoupled)
            {
                
            }
            
            // Inserts the new page as a child of the parent page
            newPage.Insert(parentPage);
        }
    }
}