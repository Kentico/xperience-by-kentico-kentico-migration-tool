namespace Migration.Tool.Core.KX13.Constants;

public class CommerceConstants
{
    /// <summary>
    /// Currency code custom field name.
    /// </summary>
    /// <remarks>
    /// This field is used to store the currency code for the order.
    /// It is because order in XbK doesn't have currency support, so we need to preserved currency value.
    /// </remarks>
    public const string CURRENCY_CODE_FIELD_NAME = "CurrencyCode";


    /// <summary>
    /// Site origin custom field name.
    /// </summary>
    /// <remarks>
    /// This field is used to store the site origin for the order.
    /// It is because order in XbK doesn't have multi store support, so we need to preserved site origin value,
    /// to be able to identify order and customer origin.
    /// </remarks>
    public const string SITE_ORIGIN_FIELD_NAME = "SiteOriginName";
}