using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Migration.Toolkit.Common;

using System.Runtime.Serialization;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Autofix enum
/// </summary>
/// <remarks>do not update value names, they are used in json configuration</remarks>
public enum AutofixEnum
{
    DiscardData,
    AttemptFix,
    Error,
}

public class ToolkitConfiguration
{
    #region Connection string of source instance

    private string? _kxConnectionString;

    [ConfigurationKeyName(ConfigurationNames.SourceConnectionString)]
    [Obsolete("Use KxConnectionString instead")]
    public string? ObsoleteSourceConnectionString { get; set; }

    [ConfigurationKeyName(ConfigurationNames.KxConnectionString)]
    public string KxConnectionString
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_kxConnectionString.NullIf(ConfigurationNames.TodoPlaceholder)))
            {
#pragma warning disable CS0618 // usage is related to resolving deprecation and backwards compatibility
                return ObsoleteSourceConnectionString!;
#pragma warning restore CS0618
            }

            return _kxConnectionString!;
        }
        set => _kxConnectionString = value;
    }

    #endregion

    #region Path to CMS dir of source instance

    private string? _kxCmsDirPath;

    [ConfigurationKeyName(ConfigurationNames.SourceCmsDirPath)]
    [Obsolete("Use KxCmsDirPath instead")]
    public string? ObsoleteSourceCmsDirPath { get; set; }

    [ConfigurationKeyName(ConfigurationNames.KxCmsDirPath)]
    public string? KxCmsDirPath
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_kxCmsDirPath.NullIf(ConfigurationNames.TodoPlaceholder)))
            {
#pragma warning disable CS0618
                return ObsoleteSourceCmsDirPath;
#pragma warning restore CS0618
            }

            return _kxCmsDirPath;
        }
        set => _kxCmsDirPath = value;
    }

    #endregion

    #region Connection string of target instance

    private string? _xbKConnectionString;

    [ConfigurationKeyName(ConfigurationNames.TargetConnectionString)]
    [Obsolete("Use XbKConnectionString instead")]
    public string? ObsoleteTargetConnectionString { get; set; }

    [ConfigurationKeyName(ConfigurationNames.XbKConnectionString)]
    public string? XbKConnectionString
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_xbKConnectionString.NullIf(ConfigurationNames.TodoPlaceholder)))
            {
#pragma warning disable CS0618 // usage is related to resolving deprecation and backwards compatibility
                return ObsoleteTargetConnectionString;
#pragma warning restore CS0618
            }

            return _xbKConnectionString;
        }
        set => _xbKConnectionString = value;
    }

    #endregion

    #region Path to root directory of target instance

    private string? _xbKDirPath = null;


    [ConfigurationKeyName(ConfigurationNames.TargetCmsDirPath)]
    [Obsolete("Use XbKDirPath instead")]
    public string? ObsoleteTargetCmsDirPath { get; set; } = null;

    [ConfigurationKeyName(ConfigurationNames.XbKDirPath)]
    public string? XbKDirPath
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_xbKDirPath.NullIf(ConfigurationNames.TodoPlaceholder)))
            {
#pragma warning disable CS0618 // usage is related to resolving deprecation and backwards compatibility
                return ObsoleteTargetCmsDirPath;
#pragma warning restore CS0618
            }

            return _xbKDirPath;
        }
        set => _xbKDirPath = value;
    }

    #endregion

    [ConfigurationKeyName(ConfigurationNames.EntityConfigurations)]
    public EntityConfigurations EntityConfigurations { get; set; } = new();

    [ConfigurationKeyName(ConfigurationNames.MigrateOnlyMediaFileInfo)]
    public bool? MigrateOnlyMediaFileInfo { get; set; } = true;

    [ConfigurationKeyName(ConfigurationNames.UseOmActivityNodeRelationAutofix)]
    public AutofixEnum? UseOmActivityNodeRelationAutofix { get; set; } = AutofixEnum.Error;

    [ConfigurationKeyName(ConfigurationNames.UseOmActivitySiteRelationAutofix)]
    public AutofixEnum? UseOmActivitySiteRelationAutofix { get; set; } = AutofixEnum.Error;

    [ConfigurationKeyName(ConfigurationNames.MigrationProtocolPath)]
    public string? MigrationProtocolPath { get; set; }

    [ConfigurationKeyName(ConfigurationNames.MemberIncludeUserSystemFields)]
    public string? MemberIncludeUserSystemFields { get; set; }
    
    public Dictionary<int, int> RequireExplicitMapping<TEntityType>(Expression<Func<TEntityType, object>> keyNameSelector)
    {
        var memberName = keyNameSelector.GetMemberName();
        var migratedIds = EntityConfigurations?.GetEntityConfiguration<TEntityType>()?.ExplicitPrimaryKeyMapping[memberName];
        if (migratedIds == null)
        {
            throw new InvalidOperationException(string.Format(Resources.Exception_MappingIsRequired, typeof(TEntityType).Name, memberName));
        }

        return migratedIds.ToDictionary(kvp =>
        {
            if (int.TryParse(kvp.Key, out var id))
            {
                return id;
            }

            throw new InvalidOperationException(string.Format(Resources.Exception_MappingIsRequired, typeof(TEntityType).Name, memberName));
        }, kvp =>
        {
            if (kvp.Value is { } id)
            {
                return id;
            }

            throw new InvalidOperationException(string.Format(Resources.Exception_MappingIsRequired, typeof(TEntityType).Name, memberName));
        });
    }

    public void AddExplicitMapping<TEntityType>(Expression<Func<TEntityType, object>> keyNameSelector, int sourceId, int targetId)
    {
        var memberName = keyNameSelector.GetMemberName();
        EntityConfigurations ??= new EntityConfigurations();

        var entityConfiguration = EntityConfigurations.GetEntityConfiguration<TEntityType>();
        var mapping = entityConfiguration.ExplicitPrimaryKeyMapping;
        if (!mapping.ContainsKey(memberName))
        {
            mapping.Add(memberName, new());
        }

        if (!mapping[memberName].ContainsKey(sourceId.ToString()))
        {
            mapping[memberName].Add(sourceId.ToString(), targetId);
        }
        else
        {
            mapping[memberName][sourceId.ToString()] = targetId;
        }

        EntityConfigurations.SetEntityConfiguration<TEntityType>(entityConfiguration);
    }


    #region Opt-in features

    [ConfigurationKeyName(ConfigurationNames.OptInFeatures)]
    public OptInFeatures? OptInFeatures { get; set; }

    #endregion
}