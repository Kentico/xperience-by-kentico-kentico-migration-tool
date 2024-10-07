namespace Migration.Toolkit.KXP.Api.Services.CmsClass;

public interface IFieldMigrationService
{
    IFieldMigration? GetFieldMigration(FieldMigrationContext fieldMigrationContext);
}
