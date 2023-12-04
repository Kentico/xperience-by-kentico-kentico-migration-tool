namespace Migration.Toolkit.Common.MigrationProtocol;

using Microsoft.Data.SqlClient;
using Migration.Toolkit.Common.Helpers;

public static class HandbookReferences
{
    #region "Warnings - nothing needs to be done"

    public static HandbookReference EntityExplicitlyExcludedByCodeName(string codeName, string entityName) =>
        new("EntityExplicitlyExcludedByCodeName", $"CodeName={codeName}, EntityName={entityName}");

    public static HandbookReference CmsClassCmsRootClassTypeSkip => new("CmsClass_CmsRootClassTypeSkip");
    public static HandbookReference CmsUserAdminUserSkip => new("CmsUser_SkipAdminUser");
    public static HandbookReference CmsUserPublicUserSkip => new("CmsUser_SkipPublicUser");
    public static HandbookReference CmsTreeTreeRootSkip => new("CmsTree_TreeRootSkipped");
    public static HandbookReference CmsSettingsKeyExclusionListSkip => new("CmsSettingsKey_SkipExclusionList"); // TODO tk: 2022-07-19 generalize

    public static HandbookReference SourcePageIsNotPublished(Guid sourcePageGuid) => new("SourcePageIsNotPublished", $"PageGuid={sourcePageGuid}");

    public static HandbookReference CmsTreeTreeIsLinkFromDifferentSite => new("CmsTree_TreeIsLinkFromDifferentSite");

    public static HandbookReference TemporaryAttachmentMigrationIsNotSupported => new("TemporaryAttachmentMigrationIsNotSupported");

    public static HandbookReference LinkedDataAlreadyMaterializedInTargetInstance =>
        new HandbookReference("LinkedDataAlreadyMaterializedInTargetInstance");

    public static HandbookReference DataAlreadyExistsInTargetInstance =>
        new HandbookReference("DataAlreadyExistsInTargetInstance");

    #endregion


    #region "Errors - something need to be done"

    public static HandbookReference InvalidSourceCmsVersion() => new HandbookReference("InvalidSourceCmsVersion")
        .NeedsManualAction();

    public static HandbookReference CommandConstraintBroken(string constraintName) => new HandbookReference("CommandConstraintBroken")
        .WithMessage($"Command constraint '{constraintName}' broken.")
        .NeedsManualAction();

    public static HandbookReference MediaFileIsMissingOnSourceFilesystem => new("MediaFileIsMissingOnSourceFilesystem");

    public static HandbookReference MissingRequiredDependency<TSourceDependency>(string fieldName, object? sourceValue) =>
        new HandbookReference("MissingRequiredDependency")
            .WithData(new { DependencyType = typeof(TSourceDependency).Name, FieldName = fieldName, SourceValue = sourceValue });

    public static HandbookReference MissingRequiredDependency<TSourceDependency>() =>
        new HandbookReference("MissingRequiredDependency")
            .WithData(new { DependencyType = typeof(TSourceDependency).Name, });

    public static HandbookReference SourceEntityIsNull<TSource>() =>
        new HandbookReference("SourceEntityIsNull")
            .WithData(new { SourceEntityType = typeof(TSource).FullName, });

    public static HandbookReference SourceValueIsRequired<TSource>(string valueName) =>
        new HandbookReference("SourceValueIsRequired")
            .NeedsManualAction()
            .WithData(new { SourceEntityType = typeof(TSource).FullName, ValueName = valueName });

    public static HandbookReference FailedToCreateTargetInstance<TTarget>() =>
        new HandbookReference("FailedToCreateTargetInstance")
            .NeedsManualAction()
            .WithData(new { TargetEntityType = typeof(TTarget).FullName, });

    public static HandbookReference ErrorCreatingTargetInstance<TTarget>(Exception exception) =>
        new HandbookReference("FailedToCreateTargetInstance")
            .NeedsManualAction()
            .WithData(new { TargetEntityType = typeof(TTarget).FullName, Exception = exception.ToString(), });

    public static HandbookReference ErrorUpdatingTargetInstance<TTarget>(Exception exception) =>
        new HandbookReference("FailedToUpdateTargetInstance")
            .NeedsManualAction()
            .WithData(new { TargetEntityType = typeof(TTarget).FullName, Exception = exception.ToString(), });

    public static HandbookReference ErrorSavingTargetInstance<TTarget>(Exception exception) =>
        new HandbookReference("ErrorSavingTargetInstance")
            .NeedsManualAction()
            .WithData(new { TargetEntityType = typeof(TTarget).FullName, Exception = exception.ToString(), });

    public static HandbookReference MissingConfiguration<TCommand>(string configurationName) =>
        new HandbookReference("MissingConfiguration")
            .NeedsManualAction()
            .WithData(new { Command = typeof(TCommand).FullName, ConfigurationName = configurationName });

    public static HandbookReference DbConstraintBroken<TEntity>(SqlException ex, TEntity entity)
        => new HandbookReference("DbConstraintBroken")
            .NeedsManualAction()
            .WithIdentityPrint(entity)
            .WithMessage(ex.Message);

    public static HandbookReference BulkCopyColumnMismatch(string tableName) => new("BulkCopyColumnMismatch", $"TableName={tableName}");

    public static HandbookReference DataMustNotExistInTargetInstanceTable(string tableName) =>
        new HandbookReference("DataMustNotExistInTargetInstanceTable")
            .WithData(new { tableName });

    public static HandbookReference ValueTruncationSkip(string tableName) =>
        new HandbookReference("ValueTruncationSkip").NeedsManualAction().WithData(new { tableName });

    public static HandbookReference NotCurrentlySupportedSkip<TSource>() =>
        new HandbookReference("NotCurrentlySupportedSkip").WithData(new { Type = ReflectionHelper<TSource>.CurrentType.Name });

    public static HandbookReference NotSupportedSkip<TSource>() =>
        new HandbookReference("NotSupportedSkip").WithData(new { Type = ReflectionHelper<TSource>.CurrentType.Name });

    public static HandbookReference NotCurrentlySupportedSkip() =>
        new HandbookReference("NotCurrentlySupportedSkip");

    public static HandbookReference InvalidSourceData<TSource>() => new HandbookReference("InvalidSourceData")
        .NeedsManualAction()
        .WithData(new { SourceEntityType = typeof(TSource).Name, });

    #endregion

    #region Form components

    public static HandbookReference FormComponentNotSupportedInLegacyMode(string componentIdentifier, string recommendedNewFormComponent) =>
        new HandbookReference("FormComponentNotSupportedInLegacyMode")
            .NeedsManualAction()
            .WithMessage($"Component is no longer supported in target instance, use NewRecommendedComponentIdentifier - data will be migrated to fit")
            .WithData(new { ComponentIdentifier = componentIdentifier, NewRecommendedComponentIdentifier = recommendedNewFormComponent, });

    public static HandbookReference FormComponentCustom(string componentIdentifier) => new HandbookReference("CustomFormComponent")
        .NeedsManualAction()
        .WithMessage($"Custom form component used - don't forget to migrate code")
        .WithData(new { ComponentIdentifier = componentIdentifier, });

    #endregion
}