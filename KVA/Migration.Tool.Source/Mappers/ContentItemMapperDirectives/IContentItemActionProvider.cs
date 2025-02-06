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
}
