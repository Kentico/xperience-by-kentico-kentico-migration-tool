using System.Linq.Expressions;

using CMS.DataEngine;
using CMS.FormEngine;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Core.KX13.Constants;
using Migration.Tool.Core.KX13.Helpers;
using Migration.Tool.KX13.Context;
using Migration.Tool.KX13.Models;
using Migration.Tool.KXP.Api;
using Migration.Tool.KXP.Api.Services.CmsClass;

namespace Migration.Tool.Core.KX13.Handlers.Base;

public abstract class MigrateCommerceHandlerBase
{
    protected readonly ILogger Logger;
    protected readonly IDbContextFactory<KX13Context> Kx13ContextFactory;
    protected readonly ToolConfiguration ToolConfiguration;
    protected readonly IFieldMigrationService FieldMigrationService;
    protected readonly KxpClassFacade KxpClassFacade;

    protected MigrateCommerceHandlerBase(
        ILogger logger,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        ToolConfiguration toolConfiguration,
        IFieldMigrationService fieldMigrationService,
        KxpClassFacade kxpClassFacade)
    {
        Logger = logger;
        Kx13ContextFactory = kx13ContextFactory;
        ToolConfiguration = toolConfiguration;
        FieldMigrationService = fieldMigrationService;
        KxpClassFacade = kxpClassFacade;
    }

    protected async Task<List<CmsSite>> GetCommerceSites(KX13Context kx13Context)
    {
        var commerceSiteNames = ToolConfiguration.CommerceConfiguration?.CommerceSiteNames ?? [];

        if (commerceSiteNames.Count == 0)
        {
            throw new InvalidOperationException("No commerce site names configured.");
        }

        var predicate = BuildSiteNameOrFilter(commerceSiteNames);
        var commerceSites = await kx13Context.CmsSites
            .Where(predicate)
            .ToListAsync();

        var foundSiteNames = commerceSites.Select(s => s.SiteName).ToList();
        var missingSiteNames = commerceSiteNames.Except(foundSiteNames).ToList();

        if (missingSiteNames.Count > 0)
        {
            throw new InvalidOperationException($"Commerce site(s) '{string.Join(", ", missingSiteNames)}' not found.");
        }

        return commerceSites;
    }

    protected async Task MigrateCommerceClass(
        string sourceClassName,
        string targetClassName,
        List<string>? includeSystemFieldsConfig,
        string logEntityName,
        bool addSiteOriginField,
        bool addCurrencyField,
        CancellationToken cancellationToken)
    {
        await using var kx13Context = await Kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        var kx13Class = kx13Context.CmsClasses
            .FirstOrDefault(c => c.ClassName == sourceClassName);

        if (kx13Class == null)
        {
            Logger.LogWarning("KX13 {SourceClassName} class not found, skipping custom field migration", sourceClassName);
            return;
        }

        var xbkClass = KxpClassFacade.GetClass(targetClassName);
        if (xbkClass == null)
        {
            Logger.LogWarning("XbK {TargetClassName} class not found, skipping custom field migration", logEntityName);
            return;
        }

        var patcher = new FormDefinitionPatcher(
            Logger,
            kx13Class.ClassFormDefinition,
            FieldMigrationService,
            classIsForm: false,
            classIsDocumentType: false,
            discardSysFields: false,
            classIsCustom: false
        );

        patcher.PatchFields();
        patcher.RemoveCategories();

        var patchedDefinition = patcher.GetPatched();
        if (string.IsNullOrWhiteSpace(patchedDefinition))
        {
            Logger.LogDebug("No custom fields found in KX13 {SourceClassName} class", sourceClassName);
            return;
        }

        var includedSystemFields = includeSystemFieldsConfig ?? [];

        string systemFieldPrefix = CommerceHelper.GetSystemFieldPrefix(ToolConfiguration);

        var xbkFormInfo = new FormInfo(xbkClass.ClassFormDefinition);
        var kx13FormInfo = new FormInfo(patchedDefinition);
        var existingColumns = xbkFormInfo.GetColumnNames();
        int addedFieldsCount = 0;

        foreach (string columnName in kx13FormInfo.GetColumnNames())
        {
            var field = kx13FormInfo.GetFormField(columnName);

            bool isIncludedSystemField = field.System && includedSystemFields.Contains(columnName, StringComparer.OrdinalIgnoreCase);

            string targetFieldName = field.System
                ? $"{systemFieldPrefix}{columnName}"
                : columnName;

            if (
                !field.PrimaryKey &&
                (isIncludedSystemField || !field.System) &&
                !existingColumns.Contains(targetFieldName)
            )
            {
                if (isIncludedSystemField)
                {
                    field.System = false;
                    field.Name = targetFieldName;
                    Logger.LogInformation("Added system field '{OriginalFieldName}' as '{PrefixedFieldName}' to {TargetClassName} class",
                        columnName, targetFieldName, logEntityName);
                }
                else
                {
                    Logger.LogInformation("Added custom field '{FieldName}' to {TargetClassName} class", columnName, logEntityName);
                }

                xbkFormInfo.AddFormItem(field);
                addedFieldsCount++;
            }
            else if (!field.PrimaryKey && existingColumns.Contains(targetFieldName))
            {
                Logger.LogDebug("Field '{FieldName}' already exists in {TargetClassName} class, skipping", targetFieldName, logEntityName);
            }
        }

        if (addSiteOriginField)
        {
            string siteOriginFieldName = $"{systemFieldPrefix}{CommerceConstants.SITE_ORIGIN_FIELD_NAME}";
            addedFieldsCount += AddField(siteOriginFieldName, CommerceConstants.SITE_ORIGIN_FIELD_DISPLAY_NAME, 200, existingColumns, kx13FormInfo, xbkFormInfo, logEntityName);
        }

        if (addCurrencyField)
        {
            string currencyFieldName = $"{systemFieldPrefix}{CommerceConstants.CURRENCY_CODE_FIELD_NAME}";
            addedFieldsCount += AddField(currencyFieldName, CommerceConstants.CURRENCY_CODE_FIELD_DISPLAY_NAME, 3, existingColumns, kx13FormInfo, xbkFormInfo, logEntityName);
        }

        if (addedFieldsCount > 0)
        {
            xbkClass.ClassFormDefinition = xbkFormInfo.GetXmlDefinition();
            DataClassInfoProvider.ProviderObject.Set(xbkClass);
            Logger.LogInformation("Migrated {Count} custom field(s) to {TargetClassName} class", addedFieldsCount, logEntityName);
        }
        else
        {
            Logger.LogDebug("No new custom fields to migrate to {TargetClassName} class", logEntityName);
        }
    }


    private int AddField(
        string fieldName,
        string fieldCaption,
        int size,
        IEnumerable<string> existingColumns,
        FormInfo kx13FormInfo,
        FormInfo xbkFormInfo,
        string logEntityName)
    {


        if (!existingColumns.Contains(fieldName) &&
            !kx13FormInfo.GetColumnNames().Contains(CommerceConstants.SITE_ORIGIN_FIELD_NAME))
        {
            var siteOriginNameField = new FormFieldInfo
            {
                Name = fieldName,
                DataType = FieldDataType.Text,
                Size = size,
                AllowEmpty = true,
                System = false,
                Visible = true,
                Enabled = true,
                Caption = fieldCaption,
                Guid = Guid.NewGuid()
            };

            xbkFormInfo.AddFormItem(siteOriginNameField);
            Logger.LogInformation("Added new custom field '{FieldName}' to {TargetClassName} class", fieldName, logEntityName);
            return 1;
        }

        return 0;
    }


    /// <summary>
    /// Builds an expression tree that generates SQL-compatible OR predicates for site name filtering,
    /// avoiding EF Core's OPENJSON optimization which requires SQL Server 2016+ compatibility.
    /// </summary>
    /// <remarks>
    /// This method builds an expression tree manually to avoid EF Core's OPENJSON optimization.
    /// When using .Where(s => commerceSiteNames.Contains(s.SiteName)), EF Core's SQL Server provider
    /// translates the collection into SQL using OPENJSON with JSON path syntax (e.g., WITH ([value] nvarchar(100) '$')).
    /// While efficient for modern SQL Server, OPENJSON and the '$' JSON path syntax are only available in:
    /// - SQL Server 2016 (v13) or newer
    /// - Database compatibility level ≥ 130
    /// 
    /// For SQL Server 2014 or older, or databases with compatibility level below 130, this generates:
    /// "Microsoft.Data.SqlClient.SqlException: Incorrect syntax near '$'."
    /// 
    /// By building an explicit OR chain (s.SiteName == name1 || s.SiteName == name2 || ...),
    /// EF Core generates standard SQL with individual equality comparisons or a simple IN clause,
    /// ensuring compatibility with older SQL Server versions.
    /// </remarks>
    public static Expression<Func<CmsSite, bool>> BuildSiteNameOrFilter(IEnumerable<string> names)
    {
        var list = names?.Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().ToArray() ?? Array.Empty<string>();
        if (list.Length == 0)
        {
            return s => false;
        }

        var param = Expression.Parameter(typeof(CmsSite), "s");
        var siteNameProp = Expression.Property(param, nameof(CmsSite.SiteName));

        Expression body = list
            .Select(name => Expression.Equal(siteNameProp, Expression.Constant(name, typeof(string))))
            .Aggregate(Expression.OrElse);

        return Expression.Lambda<Func<CmsSite, bool>>(body!, param);
    }


    public static Expression<Func<KX13Type, bool>> BuildSiteNameOrFilterGeneric<KX13Type>(IEnumerable<int> names, string columnName)
    {
        var list = names.ToArray();
        if (list.Length == 0)
        {
            return s => false;
        }

        var param = Expression.Parameter(typeof(KX13Type), "s");
        var siteNameProp = Expression.Property(param, columnName);

        Expression body = list
            .Select(name => Expression.Equal(siteNameProp, Expression.Constant(name, typeof(int?))))
            .Aggregate(Expression.OrElse);

        return Expression.Lambda<Func<KX13Type, bool>>(body!, param);
    }


    public static Expression<Func<KX13Type, bool>> BuildNullableIntOrFilter<KX13Type>(IEnumerable<int> values, string nullablePropertyName)
    {
        var list = values.ToArray();
        if (list.Length == 0)
        {
            return s => false;
        }

        var param = Expression.Parameter(typeof(KX13Type), "s");
        var nullableProp = Expression.Property(param, nullablePropertyName);
        var hasValueProp = Expression.Property(nullableProp, nameof(Nullable<int>.HasValue));
        var valueProp = Expression.Property(nullableProp, nameof(Nullable<int>.Value));

        Expression body = list
            .Select(value => Expression.Equal(valueProp, Expression.Constant(value, typeof(int))))
            .Aggregate(Expression.OrElse);

        var hasValueAndMatches = Expression.AndAlso(hasValueProp, body);

        return Expression.Lambda<Func<KX13Type, bool>>(hasValueAndMatches, param);
    }
}
