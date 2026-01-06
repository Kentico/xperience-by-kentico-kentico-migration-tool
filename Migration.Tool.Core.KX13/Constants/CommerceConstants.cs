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
    /// KX13 order class name.
    /// </summary>
    public const string KX13_ORDER_CLASS_NAME = "ecommerce.order";

    /// <summary>
    /// KX13 order items class name.
    /// </summary>
    public const string KX13_ORDER_ITEMS_CLASS_NAME = "ecommerce.orderitem";

    /// <summary>
    /// KX13 order address class name.
    /// </summary>
    public const string KX13_ORDER_ADDRESS_CLASS_NAME = "ecommerce.orderaddress";

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
    public const string SYSTEM_FIELD_PREFIX = "KX13_";

    /// <summary>
    /// Currency code custom field name.
    /// </summary>
    /// <remarks>
    /// This field is used to store the currency code for orders.
    /// It is because commerce in XbK doesn't have multi currency support, so we need
    /// to preserved currency code value, to be able to identify order currency.
    /// </remarks>
    public const string CURRENCY_CODE_FIELD_NAME = "CurrencyCode";

    /// <summary>
    /// Currency code custom field display name.
    /// </summary>
    public const string CURRENCY_CODE_FIELD_DISPLAY_NAME = "Currency Code";
}
