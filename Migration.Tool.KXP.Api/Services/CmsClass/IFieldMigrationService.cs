namespace Migration.Tool.KXP.Api.Services.CmsClass;

public interface IFieldMigrationService
{
    IFieldMigration? GetFieldMigration(FieldMigrationContext fieldMigrationContext, bool allowNullSourceFormControl = false);
}
