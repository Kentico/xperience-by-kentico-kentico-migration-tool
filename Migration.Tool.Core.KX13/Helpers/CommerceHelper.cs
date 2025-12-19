using Migration.Tool.Common;
using Migration.Tool.Core.KX13.Constants;

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
}
