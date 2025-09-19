namespace Migration.Tool.Common.Services;

/// <summary>
/// Master command passed to CLI tool
/// </summary>
public enum MasterCommand
{
    /// <summary>
    /// Migrate data. Database patching will be performed before the migration
    /// </summary>
    Migrate,

    /// <summary>
    /// Only patch the database, don't mgirate any data
    /// </summary>
    Patch
}
