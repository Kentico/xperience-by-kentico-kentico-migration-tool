using Migration.Tool.Source.Mappers.ContentItemMapperDirectives;

namespace Migration.Tool.Extensions.CommunityMigrations;

public class SampleChildLinkDirector : ContentItemDirectorBase
{
    public override void Direct(ContentItemSource source, IContentItemActionProvider options)
    {
        //
        // Note that linking will take effect only for reusable content items (i.e. types in appsettings ConvertClassesToContentHub).
        // For the purpose of this sample, ConvertClassesToContentHub must contain DancingGoatCore.ProductSection.
        //
        // If the field to link the children in doesn't exist, it will be created automatically.
        // If the field doesn't allow any of the linked child's content type, the type will be added to allowed types automatically.

        // You can link any subset of child pages in one or more content item reference field, based on any filtering criteria
        if (source.SourceNode?.NodeName == "Accessories")
        {
            int filterPackClassID = 5550;
            int tablewareClassID = 5531;
            options.LinkChildren("Filters", source.ChildNodes!.Where(x => x.NodeClassID == filterPackClassID));
            options.LinkChildren("Tableware", source.ChildNodes!.Where(x => x.NodeClassID == tablewareClassID));
        }
        else
        {
            // or link all children in general
            options.LinkChildren("Children", source.ChildNodes!);
        }
    }
}