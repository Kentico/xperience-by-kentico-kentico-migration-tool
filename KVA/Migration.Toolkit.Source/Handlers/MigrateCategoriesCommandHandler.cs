namespace Migration.Toolkit.Source.Handlers;

using System.Collections;
using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;
using CMS.FormEngine;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Source.Mappers;
using Migration.Toolkit.Source.Model;
using Migration.Toolkit.Source.Services;
using Newtonsoft.Json;

public class MigrateCategoriesCommandHandler(
    ILogger<MigrateCategoriesCommandHandler> logger,
    ModelFacade modelFacade,
    IImporter importer,
    ReusableSchemaService reusableSchemaService,
    IUmtMapper<TagModelSource> tagModelMapper,
    SpoiledGuidContext spoiledGuidContext
) : IRequestHandler<MigrateCategoriesCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateCategoriesCommand request, CancellationToken cancellationToken)
    {
        var taxonomyName = "Categories";
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
            var query = """
                        SELECT C.ClassName, C.ClassGuid, C.ClassID
                        FROM View_CMS_Tree_Joined [TJ]
                                 JOIN dbo.CMS_DocumentCategory [CDC] on [TJ].DocumentID = [CDC].DocumentID
                                 JOIN CMS_Class [C] ON TJ.NodeClassID = [C].ClassID
                                 JOIN dbo.CMS_Category CC on CDC.CategoryID = CC.CategoryID AND CC.CategoryUserID IS NULL
                        GROUP BY C.ClassName, C.ClassGuid, C.ClassID
                        """;

            var classesWithCategories = modelFacade.Select(query, (reader, version) => new
            {
                ClassName = reader.Unbox<string>("ClassName"),
                ClassGuid = reader.Unbox<Guid>("ClassGuid"),
                ClassID = reader.Unbox<int>("ClassID"),
            });

            var skippedClasses = new List<int>();
            Guid schemaGuid = Guid.Empty;
            var categoryFieldName = "Category_Legacy";
            foreach (var classWithCategoryUsage in classesWithCategories)
            {
                var targetDataClass = DataClassInfoProvider.ProviderObject.Get(classWithCategoryUsage.ClassGuid);
                if (targetDataClass == null)
                {
                    skippedClasses.Add(classWithCategoryUsage.ClassID);
                    logger.LogWarning("Data class not found by ClassGuid {Guid}", classWithCategoryUsage.ClassGuid);
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

            var categories = modelFacade.Select<ICmsCategory>(
                "CategoryEnabled = 1 AND CategoryUserID IS NULL",
                "CategoryLevel ASC, CategoryOrder ASC"
            );

            var categoryId2Guid = new Dictionary<int, Guid>();

            foreach (var cmsCategory in categories)
            {
                categoryId2Guid.Add(cmsCategory.CategoryID, cmsCategory.CategoryGUID);
                // CategorySiteID - not migrated, Taxonomies are global!

                var mapped = tagModelMapper.Map(new TagModelSource(
                    taxonomy.TaxonomyGUID,
                    cmsCategory,
                    categoryId2Guid
                ));

                foreach (var umtModel in mapped)
                {
                    if (await importer
                            .ImportAsync(umtModel)
                            .AssertSuccess<TagInfo>(logger) is {Success:true, Info: {} tag})
                    {
                        query = """
                                SELECT TJ.DocumentGUID, TJ.NodeSiteID, TJ.NodeID, TJ.DocumentID, CDC.CategoryID, TJ.DocumentCheckedOutVersionHistoryID, TJ.NodeClassID
                                FROM View_CMS_Tree_Joined [TJ]
                                         JOIN dbo.CMS_DocumentCategory [CDC] on [TJ].DocumentID = [CDC].DocumentID
                                         JOIN dbo.CMS_Category CC on CDC.CategoryID = CC.CategoryID AND CC.CategoryUserID IS NULL
                                WHERE CDC.CategoryID = @categoryId
                                """;

                        var docsWithCategories = modelFacade.Select(query, (reader, _) => new
                        {
                            CategoryID = reader.Unbox<int?>("CategoryID"),
                            DocumentCheckedOutVersionHistoryID = reader.Unbox<int?>("DocumentCheckedOutVersionHistoryID"),
                            NodeClassID = reader.Unbox<int>("NodeClassID"),
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

                            var commonData = ContentItemCommonDataInfo.Provider.Get()
                                .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataGUID), dwc.DocumentGUID)
                                .FirstOrDefault();

                            if (commonData is null)
                            {
                                logger.LogWarning("ContentItemCommonDataInfo not found by guid {Guid}, taxonomy cannot be migrated", dwc.DocumentGUID);
                                continue;
                            }

                            var infosWithTag = ContentItemCommonDataInfo.Provider.Get()
                                .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataContentItemID), commonData.ContentItemCommonDataContentItemID)
                                .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataContentLanguageID), commonData.ContentItemCommonDataContentLanguageID);

                            foreach (var infoWithTag in infosWithTag)
                            {
                                List<TagReference> tagReferences = [];
                                if (infoWithTag[categoryFieldName] is string jsonTags)
                                {
                                    tagReferences = JsonConvert.DeserializeObject<List<TagReference>>(jsonTags) ?? [];
                                }

                                tagReferences.Add(new TagReference { Identifier = tag.TagGUID });

                                infoWithTag[categoryFieldName] = JsonConvert.SerializeObject(tagReferences);

                                ContentItemCommonDataInfo.Provider.Set(infoWithTag);
                            }
                        }
                    }
                }
            }
        }

        return new GenericCommandResult();
    }

    private Guid EnsureReusableFieldSchema(TaxonomyInfo taxonomy, string categoryFieldName, string categoryFieldDisplayName)
    {
        return reusableSchemaService.EnsureReusableFieldSchema(
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
                Settings = new Hashtable
                {
                    { "controlname", "Kentico.Administration.TagSelector" },
                    { "TaxonomyGroup", $"[\"{taxonomy.TaxonomyGUID}\"]" }
                }
            });
    }
}