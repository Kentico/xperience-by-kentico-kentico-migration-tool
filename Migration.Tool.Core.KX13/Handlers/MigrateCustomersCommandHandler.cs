using System.Linq.Expressions;

using CMS.Commerce;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Membership;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Core.KX13.Constants;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.Core.KX13.Mappers;
using Migration.Tool.KX13.Context;
using Migration.Tool.KX13.Models;
using Migration.Tool.KXP.Api;
using Migration.Tool.KXP.Api.Services.CmsClass;

namespace Migration.Tool.Core.KX13.Handlers;

public class MigrateCustomersCommandHandler(
    ILogger<MigrateCustomersCommandHandler> logger,
    IDbContextFactory<KX13Context> kx13ContextFactory,
    IEntityMapper<CustomerInfoMapperSource, CustomerInfo> customerInfoMapper,
    IEntityMapper<CustomerAddressInfoMapperSource, CustomerAddressInfo> customerAddressInfoMapper,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    ToolConfiguration toolConfiguration,
    IFieldMigrationService fieldMigrationService,
    KxpClassFacade kxpClassFacade)
    : IRequestHandler<MigrateCustomersCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateCustomersCommand request, CancellationToken cancellationToken)
    {
        await using var kx13Context = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);
        var kx13CommerceSites = await GetCommerceSites(kx13Context);
        var kx13CommerceSiteIds = kx13CommerceSites.Select(s => s.SiteId).ToHashSet();

        // Migrate custom and system fields from KX13 'ecommerce.customer' and 'ecommerce.address' classes to XbK counterparts
        await MigrateCustomerClass(cancellationToken);
        await MigrateCustomerAddressClass(cancellationToken);

        var kx13Customers = kx13Context.ComCustomers
            .Where(c => !c.CustomerSiteId.HasValue || kx13CommerceSiteIds.Contains(c.CustomerSiteId.Value))
            .Include(c => c.CustomerSite)
            .Include(c => c.ComAddresses)
            .Include(c => c.CustomerUser);

        foreach (var kx13Customer in kx13Customers)
        {
            logger.LogTrace("Migrating customer {CustomerId} with CustomerGuid {CustomerGuid}", kx13Customer.CustomerId, kx13Customer.CustomerGuid);

            var kx13User = kx13Customer.CustomerUser;
            var xbkCustomerInfo = CustomerInfo.Provider.Get(kx13Customer.CustomerGuid);

            MemberInfo? xbkMemberInfo = null;
            if (kx13User is not null)
            {
                xbkMemberInfo = MemberInfoProvider.ProviderObject.Get(kx13User.UserGuid);
            }

            var sourceSiteName = kx13Customer.CustomerSite?.SiteName;
            var mapped = customerInfoMapper.Map(new CustomerInfoMapperSource(kx13Customer, sourceSiteName, xbkMemberInfo), xbkCustomerInfo);

            SaveCustomerUsingKenticoApi(mapped!, kx13User);

            if (mapped is { Success: true, Item: CustomerInfo customerInfo })
            {
                foreach (var kx13CustomerAddress in kx13Customer.ComAddresses.Where(a => a.AddressGuid.HasValue))
                {
                    var xbkCustomerAddress = CustomerAddressInfo.Provider.Get(kx13CustomerAddress.AddressGuid!.Value);
                    var mappedAddresses = customerAddressInfoMapper.Map(new CustomerAddressInfoMapperSource(kx13CustomerAddress, customerInfo), xbkCustomerAddress);
                    SaveCustomerAddressUsingKenticoApi(mappedAddresses!, kx13CustomerAddress);
                }
            }
        }

        return new GenericCommandResult();
    }

    private async Task MigrateCustomerClass(CancellationToken cancellationToken) =>
        await MigrateCommerceClass(
            sourceClassName: CommerceConstants.KX13_CUSTOMER_CLASS_NAME,
            targetClassName: CustomerInfo.TYPEINFO.ObjectClassName,
            includeSystemFieldsConfig: toolConfiguration.CommerceConfiguration?.IncludeCustomerSystemFields,
            logEntityName: nameof(CustomerInfo),
            addSiteOriginField: true,
            cancellationToken
        );

    private async Task MigrateCustomerAddressClass(CancellationToken cancellationToken) =>
        await MigrateCommerceClass(
            sourceClassName: CommerceConstants.KX13_ADDRESS_CLASS_NAME,
            targetClassName: CustomerAddressInfo.TYPEINFO.ObjectClassName,
            includeSystemFieldsConfig: toolConfiguration.CommerceConfiguration?.IncludeAddressSystemFields,
            logEntityName: nameof(CustomerAddressInfo),
            addSiteOriginField: false,
            cancellationToken
        );

    private async Task MigrateCommerceClass(
        string sourceClassName,
        string targetClassName,
        string? includeSystemFieldsConfig,
        string logEntityName,
        bool addSiteOriginField,
        CancellationToken cancellationToken)
    {
        await using var kx13Context = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        // Get KX13 class definition
        var kx13Class = kx13Context.CmsClasses
            .FirstOrDefault(c => c.ClassName == sourceClassName);

        if (kx13Class == null)
        {
            logger.LogWarning("KX13 {SourceClassName} class not found, skipping custom field migration", sourceClassName);
            return;
        }

        // Get XbK target class
        var xbkClass = kxpClassFacade.GetClass(targetClassName);
        if (xbkClass == null)
        {
            logger.LogWarning("XbK {TargetClassName} class not found, skipping custom field migration", logEntityName);
            return;
        }

        // Patch KX13 form definition
        var patcher = new FormDefinitionPatcher(
            logger,
            kx13Class.ClassFormDefinition,
            fieldMigrationService,
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
            logger.LogDebug("No custom fields found in KX13 {SourceClassName} class", sourceClassName);
            return;
        }

        var includedSystemFields = includeSystemFieldsConfig?.Split('|', StringSplitOptions.RemoveEmptyEntries) ?? [];

        // Get configured prefix or use default
        string systemFieldPrefix = toolConfiguration.CommerceConfiguration?.SystemFieldPrefix != null
            ? toolConfiguration.CommerceConfiguration.SystemFieldPrefix
            : CommerceConstants.SYSTEM_FIELD_PREFIX;

        // Merge custom fields into XbK class
        var xbkFormInfo = new FormInfo(xbkClass.ClassFormDefinition);
        var kx13FormInfo = new FormInfo(patchedDefinition);
        var existingColumns = xbkFormInfo.GetColumnNames();
        int addedFieldsCount = 0;

        foreach (string columnName in kx13FormInfo.GetColumnNames())
        {
            var field = kx13FormInfo.GetFormField(columnName);

            bool isSystemFieldMigration = field.System && includedSystemFields.Contains(columnName, StringComparer.OrdinalIgnoreCase);

            // Determine the target field name based on whether it's a system field
            string targetFieldName = isSystemFieldMigration
                ? $"{systemFieldPrefix}{columnName}"
                : columnName;

            if (
                !field.PrimaryKey &&
                !existingColumns.Contains(targetFieldName)
                && (includedSystemFields.Contains(columnName, StringComparer.OrdinalIgnoreCase) || !field.System)
            )
            {
                // Prefix system fields with configured prefix when migrating them
                if (isSystemFieldMigration)
                {
                    field.System = false; // no longer system field
                    field.Name = targetFieldName;
                    logger.LogInformation("Added system field '{OriginalFieldName}' as '{PrefixedFieldName}' to {TargetClassName} class",
                        columnName, targetFieldName, logEntityName);
                }
                else
                {
                    logger.LogInformation("Added custom field '{FieldName}' to {TargetClassName} class", columnName, logEntityName);
                }

                xbkFormInfo.AddFormItem(field);
                addedFieldsCount++;
            }
            else if (!field.PrimaryKey && existingColumns.Contains(targetFieldName))
            {
                logger.LogDebug("Field '{FieldName}' already exists in {TargetClassName} class, skipping", targetFieldName, logEntityName);
            }
        }

        // Add custom SiteOriginName field if requested and it doesn't exist
        if (addSiteOriginField)
        {
            string siteOriginFieldName = $"{systemFieldPrefix}{CommerceConstants.SITE_ORIGIN_FIELD_NAME}";

            if (!existingColumns.Contains(siteOriginFieldName) &&
                !kx13FormInfo.GetColumnNames().Contains(CommerceConstants.SITE_ORIGIN_FIELD_NAME))
            {
                var siteOriginNameField = new FormFieldInfo
                {
                    Name = siteOriginFieldName,
                    DataType = FieldDataType.Text,
                    Size = 200,
                    AllowEmpty = true,
                    System = false,
                    Visible = true,
                    Enabled = true,
                    Caption = CommerceConstants.SITE_ORIGIN_FIELD_DISPLAY_NAME,
                    Guid = Guid.NewGuid()
                };

                xbkFormInfo.AddFormItem(siteOriginNameField);
                addedFieldsCount++;
                logger.LogInformation("Added new custom field '{FieldName}' to {TargetClassName} class", siteOriginFieldName, logEntityName);
            }
        }

        if (addedFieldsCount > 0)
        {
            xbkClass.ClassFormDefinition = xbkFormInfo.GetXmlDefinition();
            DataClassInfoProvider.ProviderObject.Set(xbkClass);
            logger.LogInformation("Migrated {Count} custom field(s) to {TargetClassName} class", addedFieldsCount, logEntityName);
        }
        else
        {
            logger.LogDebug("No new custom fields to migrate to {TargetClassName} class", logEntityName);
        }
    }

    private async Task<List<CmsSite>> GetCommerceSites(KX13Context kx13Context)
    {
        var commerceSiteNames = toolConfiguration.CommerceConfiguration?.CommerceSiteNames ?? [];

        if (commerceSiteNames.Count == 0)
        {
            throw new InvalidOperationException("No commerce site names configured.");
        }

        // Builds: s => (s.SiteName == names[0]) || (s.SiteName == names[1]) || ...
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
        // Return 'false' if empty to avoid invalid SQL
        if (list.Length == 0)
        {
            return s => false;
        }

        // Parameter: s
        var param = Expression.Parameter(typeof(CmsSite), "s");

        // Member: s.SiteName
        var siteNameProp = Expression.Property(param, nameof(CmsSite.SiteName));

        // Build the OR expression for all names
        Expression body = list
            .Select(name => Expression.Equal(siteNameProp, Expression.Constant(name, typeof(string))))
            .Aggregate(Expression.OrElse);

        return Expression.Lambda<Func<CmsSite, bool>>(body!, param);
    }

    private void SaveCustomerUsingKenticoApi(IModelMappingResult<CustomerInfo> mapped, CmsUser? kx13User) =>
        SaveEntityUsingKenticoApi(
            mapped,
            CustomerInfo.Provider.Set,
            entity =>
            {
                if (kx13User is not null)
                {
                    primaryKeyMappingContext.SetMapping<CmsUser>(r => r.UserId, kx13User.UserId, entity.CustomerID);
                }
            });

    private void SaveCustomerAddressUsingKenticoApi(IModelMappingResult<CustomerAddressInfo> mapped, ComAddress kx13CustomerAddress) =>
        SaveEntityUsingKenticoApi(
            mapped,
            CustomerAddressInfo.Provider.Set,
            entity => primaryKeyMappingContext.SetMapping<ComAddress>(r => r.AddressId, kx13CustomerAddress.AddressId, entity.CustomerAddressID));


    private void SaveEntityUsingKenticoApi<TInfo>(
        IModelMappingResult<TInfo> mapped,
        Action<TInfo> saveAction,
        Action<TInfo>? postSaveAction = null)
        where TInfo : class
    {
        if (mapped is { Success: true } result)
        {
            (var entity, bool newInstance) = result;
            ArgumentNullException.ThrowIfNull(entity);

            try
            {
                saveAction(entity);
                logger.LogEntitySetAction(newInstance, entity);
            }
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                logger.LogEntitySetError(sqlException, newInstance, entity);
                return;
            }
            catch (Exception ex)
            {
                logger.LogEntitySetError(ex, newInstance, entity);
                return;
            }

            postSaveAction?.Invoke(entity);
        }
    }
}
