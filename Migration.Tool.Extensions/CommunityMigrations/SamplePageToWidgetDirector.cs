using Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Extensions.CommunityMigrations;
public class SamplePageToWidgetDirector : ContentItemDirectorBase
{
    public override void Direct(ContentItemSource source, IContentItemActionProvider options)
    {
        if (source.SourceNode!.NodeAliasPath.StartsWith("/Store"))
        {
            // 1. Widget host page tree node: Ensure the template exists and is available in XbyK target project
            if (source.SourceNode.NodeAliasPath == "/Store")
            {
                options.OverridePageTemplate("PageWithWidgetsDefaultTemplate");
            }
            // 2. Nodes to be converted to widgets. Here we identify them by alias path. Other methods like SourceClassName are also possible
            else if (source.SourceNode.NodeAliasPath == "/Store/Coffees")
            {
                options.AsWidget("UMT.CoffeeSampleWidget", null, null, options =>
                {
                    // Determine where to embed the widget
                    options.Location
                        .OnAncestorPage(-1)
                        .InEditableArea("main-area")
                        .InSection("SingleColumnSection")
                        .InFirstZone();

                    // Construct the widget's properties
                    options.Properties.Fill(true, (itemProps, reusableItemGuid, childGuids) =>
                    {
                        var widgetProps = JObject.FromObject(itemProps);    // Simple way to achieve basic conversion of all properties. The properties can then be refined in subsequent steps

                        // The converted page is linked as a reusable content item into a single property of the widget.
                        // Be sure to list the page class name appsettings in ConvertClassesToContentHub to make it reusable 
                        widgetProps["LinkedContent"] = LinkedItemPropertyValue(reusableItemGuid!.Value);

                        // Link reusable content items created from page's original subnodes
                        // Be sure to list the page class name in appsettings in ConvertClassesToContentHub to make it reusable 
                        widgetProps["LinkedChildren"] = LinkedItemsPropertyValue(childGuids);

                        //widgetProps["Asset"] = MediaFilePropertyValue("MEDIA_FILE_GUID_HERE");        // Use this helper method to set media file value

                        return widgetProps;
                    });
                });
            }
            else if (source.SourceClassName == "DancingGoatCore.Coffee")
            {
                // Don't drop the children nodes so that they are available in the previous branch
                // We could also have used a filter on NodeAliasPath to identify them
            }
            else
            {
                options.Drop();
            }
        }
        else
        {
            options.Drop();  // for the purpose of this sample, drop everything that's not under /store node
        }
    }
}
