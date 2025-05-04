using Migration.Tool.Source.Model;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
public interface IContentItemActionProvider
{
    /// <param name="widgetType">Identifier as passed to RegisterWidget attribute in target instance</param>
    /// <param name="widgetGuid">Leave null to generate</param>
    /// <param name="widgetVariantGuid">Leave null to generate</param>
    /// <param name="options">Configure how to create the widget</param>
    void AsWidget(string widgetType, Guid? widgetGuid, Guid? widgetVariantGuid, Action<IConvertToWidgetOptions> options);
    void Drop();
    void OverridePageTemplate(string templateIdentifier, JObject? templateProperties = null);
    void OverrideContentFolder(Guid contentFolderGuid);
    void OverrideContentFolder(string displayNamePath);

    /// <summary>
    /// Let the system generate URL path for all cultures and skip migrating URL path from source instance
    /// </summary>
    void RegenerateUrlPath();
    void OverrideFormerUrlPaths(IEnumerable<FormerPageUrlPath> formerPaths);

    /// <summary>
    /// Add references to child content items to a field. If the field doesn't exist, it will be created. 
    /// Child content item's content types will be added to the field's allowed content types as necessary.
    /// If the field exists and is not of content item reference type, the migration will fail.
    /// </summary>
    /// <param name="fieldName">Name of the field to add the child content item references to</param>
    /// <param name="children">One or more child objects passed by <see cref="ContentItemSource.ChildNodes"/></param>
    void LinkChildren(string fieldName, IEnumerable<ICmsTree> children);
}

public record FormerPageUrlPath(string LanguageName, string Path, DateTime? LastModified = null);
