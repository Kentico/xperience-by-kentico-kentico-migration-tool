using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.Model;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Helpers;

public static class VisualBuilderWidgetsPatcher
{
    public static EditableAreasConfiguration DeferredPatchConfiguration(EditableAreasConfiguration configuration, TreePathConvertor convertor, out bool anythingChanged)
    {
        anythingChanged = false;
        foreach (var configurationEditableArea in configuration.EditableAreas ?? [])
        {
            foreach (var sectionConfiguration in configurationEditableArea.Sections ?? [])
            {
                foreach (var sectionConfigurationZone in sectionConfiguration.Zones ?? [])
                {
                    foreach (var configurationZoneWidget in sectionConfigurationZone.Widgets ?? [])
                    {
                        DeferredPatchWidget(configurationZoneWidget, convertor, out bool anythingChangedTmp);
                        anythingChanged = anythingChanged || anythingChangedTmp;
                    }
                }
            }
        }

        return configuration;
    }

    private static void DeferredPatchWidget(WidgetConfiguration? configurationZoneWidget, TreePathConvertor convertor, out bool anythingChanged)
    {
        anythingChanged = false;
        if (configurationZoneWidget == null)
        {
            return;
        }

        var list = configurationZoneWidget.Variants ?? [];
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] is { } variantJson)
            {
                var variant = JObject.FromObject(variantJson);
                DeferredPatchProperties(variant, convertor, out bool anythingChangedTmp);

                list[i] = variant.ToObject<WidgetVariantConfiguration>();
                anythingChanged = anythingChanged || anythingChangedTmp;
            }
        }
    }

    public static void DeferredPatchProperties(JObject propertyContainer, TreePathConvertor convertor, out bool anythingChanged)
    {
        anythingChanged = false;
        if (propertyContainer?["properties"] is JObject { Count: 1 } properties)
        {
            foreach ((string key, var value) in properties)
            {
                switch (key)
                {
                    case "TreePath" when value?.Value<string>() is { } nodeAliasPath:
                    {
                        string treePath = convertor.GetConvertedOrUnchangedAssumingChannel(nodeAliasPath);
                        if (!TreePathConvertor.TreePathComparer.Equals(nodeAliasPath, treePath))
                        {
                            properties["TreePath"] = JToken.FromObject(treePath);
                            anythingChanged = true;
                        }

                        break;
                    }

                    default:
                        break;
                }
            }
        }
    }
}
