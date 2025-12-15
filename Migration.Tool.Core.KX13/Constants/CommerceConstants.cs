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
}
