using Migration.Tool.Source.Mappers.ContentItemMapperDirectives;

namespace Migration.Tool.Extensions.CommunityMigrations;

/// <summary>
/// Sample implementation of linked page director showing different strategies for handling linked pages.
/// This class demonstrates how to customize the migration of linked pages based on various criteria.
/// </summary>
public class SampleLinkedPageDirector : ContentItemDirectorBase
{
    public override void Direct(ContentItemSource source, IContentItemActionProvider options)
    {
        // This method is called for regular content item migration
        // If you only need linked page handling, you can leave this empty or add your regular content logic
    }

    public override void DirectLinkedNode(LinkedPageSource source, ILinkedPageActionProvider options)
    {
        // Access the context information
        var sourceSite = source.SourceSite; // The site where the linked page exists
        var sourceNode = source.SourceNode; // The node that contains the link
        var linkedNode = source.LinkedNode; // The target node being linked to

        // Strategy 1: Path-based decisions
        if (sourceNode.NodeAliasPath.StartsWith("/archive/"))
        {
            // Archived content: drop linked pages to reduce clutter
            options.Drop();
            return;
        }

        if (sourceNode.NodeAliasPath.Contains("/temp/") ||
            linkedNode.NodeName.StartsWith("TEMP_"))
        {
            // Temporary content: skip migration
            options.Drop();
            return;
        }

        // Strategy 2: Content type-based decisions (requires class lookup)
        // You would need to inject ModelFacade or similar service to look up class by NodeClassID
        // Example: var nodeClass = modelFacade.SelectById<ICmsClass>(linkedNode.NodeClassID);

        // Strategy 3: Site-specific handling
        switch (source.SourceSite.SiteName?.ToLowerInvariant())
        {
            case "mainsite":
                // Main site: preserve structure by materializing
                options.Materialize();
                return;

            case "microsite":
            case "landingpages":
                // Smaller sites: store references to main content
                options.StoreReferenceInAncestor(-1, "ReferencedContent");
                return;
        }

        // Default strategy: Materialize (this is also the default if no action is called)
        options.Materialize();
    }
}
