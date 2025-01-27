using System.Collections;
using System.Diagnostics;
using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;
using CMS.FormEngine;

using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Builders;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.Model;
using Migration.Tool.KXP.Api;
using Migration.Tool.Source.Mappers;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Providers;
using Migration.Tool.Source.Services;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Handlers;

public class MigrateCategoriesCommandHandler(
    ILogger<MigrateCategoriesCommandHandler> logger,
    ModelFacade modelFacade,
    IImporter importer,
    ReusableSchemaService reusableSchemaService,
    IUmtMapper<TagModelSource> tagModelMapper,
    SpoiledGuidContext spoiledGuidContext,
    KxpClassFacade kxpClassFacade,
    ClassMappingProvider classMappingProvider,
    DeferredTreeNodesService deferredTreeNodesService
) : IRequestHandler<MigrateCategoriesCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateCategoriesCommand request, CancellationToken cancellationToken)
    {
        string taxonomyName = "Categories";
        var result = await importer.ImportAsync(new TaxonomyModel
        {
            TaxonomyName = taxonomyName,
            TaxonomyGUID = GuidHelper.CreateTaxonomyGuid(taxonomyName),
            TaxonomyTitle = "Categories",
            TaxonomyDescription = "Container for legacy taxonomy",
            TaxonomyTranslations = null
        });

        if (result.Imported is TaxonomyInfo taxonomy)
        {
            string query = """
                           SELECT C.ClassName, C.ClassGuid, C.ClassID, CC.CategoryID
                           FROM View_CMS_Tree_Joined [TJ]
                                    JOIN dbo.CMS_DocumentCategory [CDC] on [TJ].DocumentID = [CDC].DocumentID
                                    JOIN CMS_Class [C] ON TJ.NodeClassID = [C].ClassID
                                    JOIN dbo.CMS_Category CC on CDC.CategoryID = CC.CategoryID AND CC.CategoryUserID IS NULL
                           GROUP BY C.ClassName, C.ClassGuid, C.ClassID, CC.CategoryID
                           """;

            var classesWithCategories = modelFacade.Select(query, (reader, version) => new { ClassName = reader.Unbox<string>("ClassName"), ClassGuid = reader.Unbox<Guid>("ClassGuid"), ClassID = reader.Unbox<int>("ClassID"), CategoryID = reader.Unbox<int>("CategoryID") })
                .GroupBy(x => x.ClassGuid)
                .Select(x => new { ClassGuid = x.Key, x.First().ClassName, x.First().ClassID, Categories = x.Select(row => row.CategoryID) });

            // For each source instance class whose documents have some categories assigned, include taxonomy-storage reusable schema in the target class
            #region Ensure reusable schema
            var skippedClasses = new List<int>();
            var schemaGuid = Guid.Empty;
            string categoryFieldName = "Category_Legacy";
            string widgetCategoryFieldName = "Category";

            foreach (var classWithCategoryUsage in classesWithCategories)
            {
                var targetDataClass = DataClassInfoProvider.ProviderObject.Get(classWithCategoryUsage.ClassGuid);
                if (targetDataClass is null)
                {
                    // No direct-mapped target class found. Try to identify custom-mapped target class
                    var classMapping = classMappingProvider.GetMapping(classWithCategoryUsage.ClassName);
                    if (classMapping is not null)
                    {
                        if (classWithCategoryUsage.Categories.Any(cat => classMapping.IsCategoryMapped(classWithCategoryUsage.ClassName, cat)))
                        {
                            targetDataClass = kxpClassFacade.GetClass(classMapping.TargetClassName);
                        }
                    }
                }

                if (targetDataClass is null)
                {
                    logger.LogWarning($"Class(ClassGuid {{Guid}}) has documents with categories, but no directly-mapped data class nor custom-mapped class that would receive the categories (declared via {nameof(IClassMapping.IsCategoryMapped)}) was found", classWithCategoryUsage.ClassGuid);
                    continue;
                }

                if (Guid.Empty.Equals(schemaGuid))
                {
                    schemaGuid = EnsureReusableFieldSchema(taxonomy, categoryFieldName, "Category");
                }

                if (!reusableSchemaService.HasClassReusableSchema(targetDataClass, schemaGuid))
                {
                    reusableSchemaService.AddReusableSchemaToDataClass(targetDataClass, schemaGuid);
                    DataClassInfoProvider.SetDataClassInfo(targetDataClass);
                }
            }
            #endregion


            var categories = modelFacade.Select<ICmsCategory>(
                "CategoryEnabled = 1 AND CategoryUserID IS NULL",
                "CategoryLevel ASC, CategoryOrder ASC"
            );

            var categoryId2Guid = new Dictionary<int, Guid>();

            foreach (var cmsCategory in categories)
            {
                categoryId2Guid.Add(cmsCategory.CategoryID, cmsCategory.CategoryGUID);
                // CategorySiteID - not migrated, Taxonomies are global!

                var tagUMTModels = tagModelMapper.Map(new TagModelSource(
                    taxonomy.TaxonomyGUID,
                    cmsCategory,
                    categoryId2Guid
                ));

                foreach (var tagUMTModel in tagUMTModels)
                {
                    if (await importer
                            .ImportAsync(tagUMTModel)
                            .AssertSuccess<TagInfo>(logger) is { Success: true, Info: { } tag })
                    {
                        query = """
                                SELECT TJ.DocumentGUID, TJ.NodeSiteID, TJ.NodeID, TJ.DocumentID, CDC.CategoryID, TJ.DocumentCheckedOutVersionHistoryID, TJ.NodeClassID, C.ClassName
                                FROM View_CMS_Tree_Joined [TJ]
                                         JOIN dbo.CMS_DocumentCategory [CDC] on [TJ].DocumentID = [CDC].DocumentID
                                         JOIN dbo.CMS_Category CC on CDC.CategoryID = CC.CategoryID AND CC.CategoryUserID IS NULL
                                         JOIN CMS_Class [C] ON TJ.NodeClassID = [C].ClassID
                                WHERE CDC.CategoryID = @categoryId
                                """;

                        var docsWithCategories = modelFacade.Select(query, (reader, _) => new
                        {
                            CategoryID = reader.Unbox<int?>("CategoryID"),
                            DocumentCheckedOutVersionHistoryID = reader.Unbox<int?>("DocumentCheckedOutVersionHistoryID"),
                            NodeClassID = reader.Unbox<int>("NodeClassID"),
                            NodeClassName = reader.Unbox<string>("ClassName"),
                            NodeSiteID = reader.Unbox<int>("NodeSiteID"),
                            DocumentGUID = spoiledGuidContext.EnsureDocumentGuid(
                                reader.Unbox<Guid>("DocumentGUID"),
                                reader.Unbox<int>("NodeSiteID"),
                                reader.Unbox<int>("NodeID"),
                                reader.Unbox<int>("DocumentID")
                            )
                        }, new SqlParameter("categoryId", cmsCategory.CategoryID));

                        foreach (var dwc in docsWithCategories)
                        {
                            if (skippedClasses.Contains(dwc.NodeClassID))
                            {
                                logger.LogWarning("ContentItemCommonDataInfo cannot have categories migrated, data class is not patched with taxonomy field. DocumentGuid {Guid}", dwc.DocumentGUID);
                                continue;
                            }

                            var classMapping = classMappingProvider.GetMapping(dwc.NodeClassName);
                            if (classMapping is not null)
                            {
                                Debug.Assert(dwc.CategoryID.HasValue, "dwc.CategoryID should have value, otherwise the row would not be included in the query due to inner join");
                                if (!classMapping.IsCategoryMapped(dwc.NodeClassName, dwc.CategoryID.Value))
                                {
                                    continue;
                                }
                            }

                            if (deferredTreeNodesService.WidgetizedDocuments.TryGetValue(dwc.DocumentGUID, out var widget))
                            {
                                var widgetHostPageCommonData = ContentItemCommonDataInfo.Provider.Get()
                                    .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataGUID), widget.ContentItemCommonDataGuid)
                                    .FirstOrDefault();

                                if (widgetHostPageCommonData.ContentItemCommonDataVisualBuilderWidgets is not null)
                                {
                                    var visualBuilderWidgets = JsonConvert.DeserializeObject<EditableAreasConfiguration>(widgetHostPageCommonData.ContentItemCommonDataVisualBuilderWidgets);
                                    foreach (var area in visualBuilderWidgets.EditableAreas)
                                    {
                                        foreach (var section in area.Sections)
                                        {
                                            foreach (var zone in section.Zones)
                                            {
                                                foreach (var w in zone.Widgets)
                                                {
                                                    foreach (var variant in w.Variants)
                                                    {
                                                        if (variant.Identifier.Equals(widget.WidgetVariantGuid))
                                                        {
                                                            JArray array;
                                                            if (variant.Properties.ContainsKey(widgetCategoryFieldName))
                                                            {
                                                                array = variant.Properties.Value<JArray>(widgetCategoryFieldName);
                                                            }
                                                            else
                                                            {
                                                                array = new JArray();
                                                            }
                                                            array.Add(new JObject { { "identifier", tag.TagGUID.ToString() } });
                                                            variant.Properties[widgetCategoryFieldName] = array;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    widgetHostPageCommonData.ContentItemCommonDataVisualBuilderWidgets = JsonConvert.SerializeObject(visualBuilderWidgets);
                                    ContentItemCommonDataInfo.Provider.Set(widgetHostPageCommonData);
                                }
                            }
                            var commonData = ContentItemCommonDataInfo.Provider.Get()
                                .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataGUID), dwc.DocumentGUID)
                                .FirstOrDefault();

                            if (commonData is null)
                            {
                                logger.LogWarning("ContentItemCommonDataInfo not found by Guid {Guid}. Taxonomy cannot be migrated", dwc.DocumentGUID);
                                continue;
                            }

                            var infosWithTag = ContentItemCommonDataInfo.Provider.Get()
                                .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataContentItemID), commonData.ContentItemCommonDataContentItemID)
                                .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataContentLanguageID), commonData.ContentItemCommonDataContentLanguageID);

                            foreach (var infoWithTag in infosWithTag)
                            {
                                HashSet<Guid> tagReferences = [];
                                if (infoWithTag[categoryFieldName] is string jsonTags)
                                {
                                    tagReferences = new HashSet<Guid>(JsonConvert.DeserializeObject<List<TagReference>>(jsonTags)?.Select(x => x.Identifier) ?? []);
                                }
                                tagReferences.Add(tag.TagGUID);
                                infoWithTag[categoryFieldName] = JsonConvert.SerializeObject(tagReferences.Select(x => new TagReference { Identifier = x }).ToList());

                                ContentItemCommonDataInfo.Provider.Set(infoWithTag);
                            }
                        }
                    }
                }
            }
        }

        return new GenericCommandResult();
    }

    private Guid EnsureReusableFieldSchema(TaxonomyInfo taxonomy, string categoryFieldName, string categoryFieldDisplayName) => reusableSchemaService.EnsureReusableFieldSchema(
        "categories_container",
        "Categories container",
        "Container for legacy categories",
        new FormFieldInfo
        {
            Enabled = true,
            Visible = true,
            AllowEmpty = true,
            DataType = "taxonomy",
            Name = categoryFieldName,
            Caption = categoryFieldDisplayName,
            Guid = new Guid("F65FE16C-53B0-47F7-B865-E8E300EC5F91"),
            Settings = new Hashtable { { "controlname", "Kentico.Administration.TagSelector" }, { "TaxonomyGroup", $"[\"{taxonomy.TaxonomyGUID}\"]" } }
        });
}
