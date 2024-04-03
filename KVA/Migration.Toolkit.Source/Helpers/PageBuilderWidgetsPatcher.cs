namespace Migration.Toolkit.Source.Helpers;

using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Source.Services.Model;
using Newtonsoft.Json.Linq;

public static class PageBuilderWidgetsPatcher
{
    public static EditableAreasConfiguration DeferredPatchConfiguration(EditableAreasConfiguration configuration, TreePathConvertor convertor, out bool anythingChanged)
    {
        anythingChanged = false;
        foreach (var configurationEditableArea in configuration.EditableAreas ?? new())
        {
            foreach (var sectionConfiguration in configurationEditableArea.Sections ?? new())
            {
                foreach (var sectionConfigurationZone in sectionConfiguration.Zones ?? new())
                {
                    foreach (var configurationZoneWidget in sectionConfigurationZone.Widgets ?? new())
                    {
                        DeferredPatchWidget(configurationZoneWidget, convertor, out var anythingChangedTmp);
                        anythingChanged = anythingChanged || anythingChangedTmp;
                    }
                }
            }
        }

        return configuration;
    }

    private static void DeferredPatchWidget(WidgetConfiguration configurationZoneWidget, TreePathConvertor convertor, out bool anythingChanged)
    {
        anythingChanged = false;
        if (configurationZoneWidget == null) return;

        var list = configurationZoneWidget.Variants ?? new();
        for (var i = 0; i < list.Count; i++)
        {
            var variant = JObject.FromObject(list[i]);
            DeferredPatchProperties(variant, convertor, out var anythingChangedTmp);

            list[i] = variant.ToObject<WidgetVariantConfiguration>();
            anythingChanged = anythingChanged || anythingChangedTmp;
        }
    }

    public static void DeferredPatchProperties(JObject propertyContainer, TreePathConvertor convertor, out bool anythingChanged)
    {
        anythingChanged = false;
        if (propertyContainer?["properties"] is JObject { Count: 1 } properties)
        {
            foreach (var (key, value) in properties)
            {
                switch (key)
                {
                    case "TreePath":
                    {
                        var nodeAliasPath = value?.Value<string>();
                        var treePath = convertor.GetConvertedOrUnchangedAssumingChannel(nodeAliasPath);
                        if (!TreePathConvertor.TreePathComparer.Equals(nodeAliasPath, treePath))
                        {
                            properties["TreePath"] = JToken.FromObject(treePath);
                            anythingChanged = true;
                        }
                        break;
                    }
                }
            }
        }
    }
}