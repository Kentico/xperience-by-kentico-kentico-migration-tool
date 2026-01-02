using Migration.Tool.Common;
using Migration.Tool.Core.KX13.Constants;
using Migration.Tool.Core.KX13.Handlers.Base;
using Migration.Tool.KX13.Models;

namespace Migration.Tool.Core.KX13.Helpers;

internal static class CommerceHelper
{
    /// <summary>
    /// Gets the configured system field prefix or the default value.
    /// </summary>
    /// <param name="toolConfiguration">The tool configuration instance.</param>
    /// <returns>The configured system field prefix from <see cref="CommerceConfiguration.SystemFieldPrefix"/> 
    /// or <see cref="CommerceConstants.SYSTEM_FIELD_PREFIX"/> if not configured.</returns>
    public static string GetSystemFieldPrefix(ToolConfiguration toolConfiguration) => toolConfiguration.CommerceConfiguration?.SystemFieldPrefix ?? CommerceConstants.SYSTEM_FIELD_PREFIX;


    public static IQueryable<ComOrder> FilterByDate(this IQueryable<ComOrder> source, DateTime? fromDate, DateTime? toDate)
    {
        if (fromDate is null && toDate is not null)
        {
            throw new ArgumentException("From date is required if to date is provided");
        }
        if (fromDate is not null && toDate is null)
        {
            throw new ArgumentException("To date is required if from date is provided");
        }
        if (fromDate is not null && toDate is not null)
        {
            source = source.Where(s => s.OrderDate >= fromDate && s.OrderDate <= toDate);
        }

        return source;
    }


    public static IQueryable<ComOrder> FilterByOrderStatus(this IQueryable<ComOrder> source, IEnumerable<int>? orderStatusIDs)
    {
        if (orderStatusIDs is null)
        {
            return source;
        }

        var orderStatusIdsArray = orderStatusIDs.ToArray();
        if (orderStatusIdsArray.Length == 0)
        {
            return source;
        }

        var predicate = MigrateCommerceHandlerBase.BuildNullableIntOrFilter<ComOrder>(orderStatusIdsArray, nameof(ComOrder.OrderStatusId));

        return source.Where(predicate);
    }
}
