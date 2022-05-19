namespace Migration.Toolkit.Core.MigrationProtocol;

public static class HandbookReferences
{
    #region "Warnings - nothing needs to be done"

    public static HandbookReference CmsClassCmsRootClassTypeSkip => new("CmsClass_CmsRootClassTypeSkip");
    public static HandbookReference CmsUserAdminUserSkip => new("CmsUser_SkipAdminUser");
    public static HandbookReference CmsUserPublicUserSkip => new("CmsUser_SkipPublicUser");
    public static HandbookReference CmsWebFarmServerSkip => new("CmsWebFarm_SkipPublicWebFarm");

    #endregion


    #region "Errors - something need to be done"

    public static HandbookReference CmsUserEmailConstraintBroken => new("CmsUser_EmailConstraintBroken");

    #endregion
}