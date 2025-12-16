using Migration.Tool.Common;

namespace Migration.Tool.Core.KX13.Constants;

public static class CommerceConstants
{
    /// <summary>
    /// KX13 customer class name.
    /// </summary>
    public const string KX13_CUSTOMER_CLASS_NAME = "ecommerce.customer";

    /// <summary>
    /// KX13 address class name.
    /// </summary>
    public const string KX13_ADDRESS_CLASS_NAME = "ecommerce.address";

    /// <summary>
    /// Site origin custom field name.
    /// </summary>
    /// <remarks>
    /// This field is used to store the site origin for commerce entities.
    /// It is because commerce in XbK doesn't have multi store support, so we need
    /// to preserved site origin value, to be able to identify order and customer origin.
    /// </remarks>
    public const string SITE_ORIGIN_FIELD_NAME = "SiteOriginName";

    /// <summary>
    /// Site origin custom field display name.
    /// </summary>
    public const string SITE_ORIGIN_FIELD_DISPLAY_NAME = "Site Origin Name";

    /// <summary>
    /// Default prefix applied to system fields when migrating them to the target instance.
    /// </summary>
    /// <remarks>
    /// This constant serves as the default/fallback value when no custom prefix is configured
    /// in <see cref="CommerceConfiguration.SystemFieldPrefix"/>.
    /// System fields from the source instance are prefixed with this value to avoid conflicts
    /// with XbK's internal system fields while making it clear they originated from the source system.
    /// </remarks>
    public const string SYSTEM_FIELD_PREFIX = "xp_";

    /// <summary>
    /// Gets the configured system field prefix or the default value.
    /// </summary>
    /// <param name="toolConfiguration">The tool configuration instance.</param>
    /// <returns>The configured system field prefix from <see cref="CommerceConfiguration.SystemFieldPrefix"/> 
    /// or <see cref="SYSTEM_FIELD_PREFIX"/> if not configured.</returns>
    public static string GetSystemFieldPrefix(ToolConfiguration toolConfiguration)
    {
        return toolConfiguration.CommerceConfiguration?.SystemFieldPrefix != null
            ? toolConfiguration.CommerceConfiguration.SystemFieldPrefix
            : SYSTEM_FIELD_PREFIX;
    }
}
