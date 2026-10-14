using Migration.Tool.Source.Mappers.FormMapperDirectives;

namespace Migration.Tool.Extensions.CommunityMigrations;

public class SampleFormWorkspaceDirector : FormDirectorBase
{
    public override void Direct(FormSource source, IFormActionProvider options)
    {
        // Migrate forms of each source site to workspace named after the site. Workspace is created if it doesn't exist.
        if (source.SourceSite is { } site)
        {
            options.OverrideWorkspace(site.SiteName, site.SiteDisplayName);
        }

        // Migrate selected forms to existing workspace identified by GUID
        // if (source.SourceForm.FormName == "ContactUs")
        // {
        //     options.OverrideWorkspace(new Guid("00000000-0000-0000-0000-000000000000"));
        // }
    }
}
