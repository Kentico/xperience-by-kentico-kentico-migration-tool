// ReSharper disable InconsistentNaming

using System.Data;

using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;
public interface ICmsPageTemplateConfiguration : ISourceModel<ICmsPageTemplateConfiguration>
{


    static string ISourceModel<ICmsPageTemplateConfiguration>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsPageTemplateConfigurationK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsPageTemplateConfigurationK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsPageTemplateConfigurationK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsPageTemplateConfiguration>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsPageTemplateConfigurationK11.IsAvailable(version),
        { Major: 12 } => CmsPageTemplateConfigurationK12.IsAvailable(version),
        { Major: 13 } => CmsPageTemplateConfigurationK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsPageTemplateConfiguration>.TableName => "CMS_PageTemplateConfiguration";
    static string ISourceModel<ICmsPageTemplateConfiguration>.GuidColumnName => ""; //assumtion, class Guid column doesn't change between versions
    static ICmsPageTemplateConfiguration ISourceModel<ICmsPageTemplateConfiguration>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsPageTemplateConfigurationK11.FromReader(reader, version),
        { Major: 12 } => CmsPageTemplateConfigurationK12.FromReader(reader, version),
        { Major: 13 } => CmsPageTemplateConfigurationK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsPageTemplateConfigurationK11() : ICmsPageTemplateConfiguration, ISourceModel<CmsPageTemplateConfigurationK11>
{
    public static bool IsAvailable(SemanticVersion version) => false;
    public static string GetPrimaryKeyName(SemanticVersion version) => "";
    public static string TableName => "CMS_PageTemplateConfiguration";
    public static string GuidColumnName => "";
    static CmsPageTemplateConfigurationK11 ISourceModel<CmsPageTemplateConfigurationK11>.FromReader(IDataReader reader, SemanticVersion version) => new CmsPageTemplateConfigurationK11(

        );
    public static CmsPageTemplateConfigurationK11 FromReader(IDataReader reader, SemanticVersion version) => new CmsPageTemplateConfigurationK11(

        );
};
public partial record CmsPageTemplateConfigurationK12(int PageTemplateConfigurationID, Guid PageTemplateConfigurationGUID, int PageTemplateConfigurationSiteID, DateTime PageTemplateConfigurationLastModified, string PageTemplateConfigurationName, string? PageTemplateConfigurationDescription, Guid? PageTemplateConfigurationThumbnailGUID, string PageTemplateConfigurationTemplate, string? PageTemplateConfigurationWidgets) : ICmsPageTemplateConfiguration, ISourceModel<CmsPageTemplateConfigurationK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "PageTemplateConfigurationID";
    public static string TableName => "CMS_PageTemplateConfiguration";
    public static string GuidColumnName => "PageTemplateConfigurationGUID";
    static CmsPageTemplateConfigurationK12 ISourceModel<CmsPageTemplateConfigurationK12>.FromReader(IDataReader reader, SemanticVersion version) => new CmsPageTemplateConfigurationK12(
            reader.Unbox<int>("PageTemplateConfigurationID"), reader.Unbox<Guid>("PageTemplateConfigurationGUID"), reader.Unbox<int>("PageTemplateConfigurationSiteID"), reader.Unbox<DateTime>("PageTemplateConfigurationLastModified"), reader.Unbox<string>("PageTemplateConfigurationName"), reader.Unbox<string?>("PageTemplateConfigurationDescription"), reader.Unbox<Guid?>("PageTemplateConfigurationThumbnailGUID"), reader.Unbox<string>("PageTemplateConfigurationTemplate"), reader.Unbox<string?>("PageTemplateConfigurationWidgets")
        );
    public static CmsPageTemplateConfigurationK12 FromReader(IDataReader reader, SemanticVersion version) => new CmsPageTemplateConfigurationK12(
            reader.Unbox<int>("PageTemplateConfigurationID"), reader.Unbox<Guid>("PageTemplateConfigurationGUID"), reader.Unbox<int>("PageTemplateConfigurationSiteID"), reader.Unbox<DateTime>("PageTemplateConfigurationLastModified"), reader.Unbox<string>("PageTemplateConfigurationName"), reader.Unbox<string?>("PageTemplateConfigurationDescription"), reader.Unbox<Guid?>("PageTemplateConfigurationThumbnailGUID"), reader.Unbox<string>("PageTemplateConfigurationTemplate"), reader.Unbox<string?>("PageTemplateConfigurationWidgets")
        );
};
public partial record CmsPageTemplateConfigurationK13(int PageTemplateConfigurationID, Guid PageTemplateConfigurationGUID, int PageTemplateConfigurationSiteID, DateTime PageTemplateConfigurationLastModified, string PageTemplateConfigurationName, string? PageTemplateConfigurationDescription, Guid? PageTemplateConfigurationThumbnailGUID, string PageTemplateConfigurationTemplate, string? PageTemplateConfigurationWidgets) : ICmsPageTemplateConfiguration, ISourceModel<CmsPageTemplateConfigurationK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "PageTemplateConfigurationID";
    public static string TableName => "CMS_PageTemplateConfiguration";
    public static string GuidColumnName => "PageTemplateConfigurationGUID";
    static CmsPageTemplateConfigurationK13 ISourceModel<CmsPageTemplateConfigurationK13>.FromReader(IDataReader reader, SemanticVersion version) => new CmsPageTemplateConfigurationK13(
            reader.Unbox<int>("PageTemplateConfigurationID"), reader.Unbox<Guid>("PageTemplateConfigurationGUID"), reader.Unbox<int>("PageTemplateConfigurationSiteID"), reader.Unbox<DateTime>("PageTemplateConfigurationLastModified"), reader.Unbox<string>("PageTemplateConfigurationName"), reader.Unbox<string?>("PageTemplateConfigurationDescription"), reader.Unbox<Guid?>("PageTemplateConfigurationThumbnailGUID"), reader.Unbox<string>("PageTemplateConfigurationTemplate"), reader.Unbox<string?>("PageTemplateConfigurationWidgets")
        );
    public static CmsPageTemplateConfigurationK13 FromReader(IDataReader reader, SemanticVersion version) => new CmsPageTemplateConfigurationK13(
            reader.Unbox<int>("PageTemplateConfigurationID"), reader.Unbox<Guid>("PageTemplateConfigurationGUID"), reader.Unbox<int>("PageTemplateConfigurationSiteID"), reader.Unbox<DateTime>("PageTemplateConfigurationLastModified"), reader.Unbox<string>("PageTemplateConfigurationName"), reader.Unbox<string?>("PageTemplateConfigurationDescription"), reader.Unbox<Guid?>("PageTemplateConfigurationThumbnailGUID"), reader.Unbox<string>("PageTemplateConfigurationTemplate"), reader.Unbox<string?>("PageTemplateConfigurationWidgets")
        );
};
