namespace Migration.Tool.KXP.Api.Services.CmsClass;

public interface ICustomMigration
{
    /// <summary>
    /// custom migrations are sorted by this number, first encountered migration wins. Values higher than 100 000 are set to default migrations, set number bellow 100 000 for custom migrations
    /// </summary>
    int Rank { get; }
}
