using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.KXO.Models;

namespace Migration.Toolkit.Core.MigratePageTypes;

public class CmsClassMapper : IEntityMapper<KX13.Models.CmsClass, KXO.Models.CmsClass>
{
    private readonly ILogger<CmsClass> _logger;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;

    public CmsClassMapper(ILogger<CmsClass> logger, PrimaryKeyMappingContext primaryKeyMappingContext)
    {
        _logger = logger;
        _primaryKeyMappingContext = primaryKeyMappingContext;
    }
    
    public ModelMappingResult<CmsClass> Map(KX13.Models.CmsClass? source, CmsClass? target)
    {
        if (source is null)
        {
            _logger.LogTrace("Source entity is not defined.");
            return new ModelMappingFailedSourceNotDefined<CmsClass>();
        }

        var newInstance = false;
        if (target is null)
        {
            _logger.LogTrace("Null target supplied, creating new instance.");
            target = new CmsClass();
            newInstance = true;
        }
        else if (source.ClassName != target.ClassName)
        {
            // assertion failed
            _logger.LogTrace("Assertion failed, entity Guid mismatch.");
            return new ModelMappingFailedKeyMismatch<CmsClass>();
        }
        
        // target.ClassId = source.ClassId;
        // TODO tk: 2022-05-17: check assigned sites target.Sites
        
        target.ClassDisplayName = source.ClassDisplayName;
        target.ClassName = source.ClassName;
        target.ClassUsesVersioning = source.ClassUsesVersioning;
        target.ClassIsDocumentType = source.ClassIsDocumentType;
        target.ClassIsCoupledClass = source.ClassIsCoupledClass;
        target.ClassXmlSchema = source.ClassXmlSchema;
        target.ClassFormDefinition = source.ClassFormDefinition;
        target.ClassNodeNameSource = source.ClassNodeNameSource;
        target.ClassTableName = source.ClassTableName;
        target.ClassFormLayout = source.ClassFormLayout;
        target.ClassShowAsSystemTable = source.ClassShowAsSystemTable;
        target.ClassUsePublishFromTo = source.ClassUsePublishFromTo;
        target.ClassShowTemplateSelection = source.ClassShowTemplateSelection;
        target.ClassSkumappings = source.ClassSkumappings;
        target.ClassIsMenuItemType = source.ClassIsMenuItemType;
        target.ClassNodeAliasSource = source.ClassNodeAliasSource;
        target.ClassLastModified = source.ClassLastModified;
        target.ClassGuid = source.ClassGuid;
        target.ClassCreateSku = source.ClassCreateSku;
        target.ClassIsProduct = source.ClassIsProduct;
        target.ClassIsCustomTable = source.ClassIsCustomTable;
        target.ClassShowColumns = source.ClassShowColumns;
        target.ClassSearchTitleColumn = source.ClassSearchTitleColumn;
        target.ClassSearchContentColumn = source.ClassSearchContentColumn;
        target.ClassSearchImageColumn = source.ClassSearchImageColumn;
        target.ClassSearchCreationDateColumn = source.ClassSearchCreationDateColumn;
        target.ClassSearchSettings = source.ClassSearchSettings;
        target.ClassInheritsFromClassId = source.ClassInheritsFromClassId;
        target.ClassSearchEnabled = source.ClassSearchEnabled;
        target.ClassSkudefaultDepartmentName = source.ClassSkudefaultDepartmentName;
        target.ClassSkudefaultDepartmentId = source.ClassSkudefaultDepartmentId;
        target.ClassContactMapping = source.ClassContactMapping;
        target.ClassContactOverwriteEnabled = source.ClassContactOverwriteEnabled;
        target.ClassSkudefaultProductType = source.ClassSkudefaultProductType;
        target.ClassConnectionString = source.ClassConnectionString;
        target.ClassIsProductSection = source.ClassIsProductSection;
        target.ClassFormLayoutType = source.ClassFormLayoutType;
        target.ClassVersionGuid = source.ClassVersionGuid;
        target.ClassDefaultObjectType = source.ClassDefaultObjectType;
        target.ClassIsForm = source.ClassIsForm;
        target.ClassResourceId = source.ClassResourceId;
        target.ClassCustomizedColumns = source.ClassCustomizedColumns;
        target.ClassCodeGenerationSettings = source.ClassCodeGenerationSettings;
        target.ClassIconClass = source.ClassIconClass;
        target.ClassUrlpattern = source.ClassUrlpattern;
        target.ClassUsesPageBuilder = source.ClassUsesPageBuilder;
        target.ClassIsNavigationItem = source.ClassIsNavigationItem;
        target.ClassHasUrl = source.ClassHasUrl;
        target.ClassHasMetadata = source.ClassHasMetadata;
        target.ClassSearchIndexDataSource = source.ClassSearchIndexDataSource;

        return new ModelMappingSuccess<CmsClass>(target, newInstance);
    }
}