using System.Diagnostics;
using System.Text;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Internal;
using CMS.FormEngine;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Helpers;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.Core.Services;
using Migration.Toolkit.Core.Services.CmsClass;
using Migration.Toolkit.Core.Services.CmsRelationship;

namespace Migration.Toolkit.Core.Mappers;

public record CmsTreeMapperSource(KX13M.CmsTree CmsTree, string CultureCode);

public class TreeNodeMapper: EntityMapperBase<CmsTreeMapperSource, TreeNode>
{
    private readonly ILogger<TreeNodeMapper> _logger;
    private readonly CoupledDataService _coupledDataService;
    private readonly ClassService _classService;
    private readonly AttachmentMigrator _attachmentMigrator;
    private readonly CmsRelationshipService _relationshipService;

    public TreeNodeMapper(
        ILogger<TreeNodeMapper> logger, 
        PrimaryKeyMappingContext pkContext, 
        CoupledDataService coupledDataService, 
        IMigrationProtocol protocol, 
        ClassService classService,
        AttachmentMigrator attachmentMigrator,
        CmsRelationshipService relationshipService
        ) : base(logger, pkContext, protocol)
    {
        _logger = logger;
        _coupledDataService = coupledDataService;
        _classService = classService;
        _attachmentMigrator = attachmentMigrator;
        _relationshipService = relationshipService;
    }

    protected override TreeNode? CreateNewInstance(CmsTreeMapperSource source, MappingHelper mappingHelper, AddFailure addFailure) 
        => TreeNode.New(source.CmsTree.NodeClass.ClassName);

    protected override TreeNode MapInternal(CmsTreeMapperSource source, TreeNode target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var (cmsTree, cultureCode) = source;
        
        if (!newInstance && cmsTree.NodeGuid != target.NodeGUID)
        {
            // assertion failed
            _logger.LogTrace("Assertion failed, entity key mismatch.");
            throw new InvalidOperationException("Assertion failed, entity key mismatch.");
        }
        
        // mapping of linked nodes is not supported
        Debug.Assert(cmsTree.NodeLinkedNodeId == null, "cmsTree.NodeLinkedNodeId == null");
        Debug.Assert(cmsTree.NodeLinkedNodeSiteId == null, "cmsTree.NodeLinkedNodeSiteId == null");

        target.NodeGUID = cmsTree.NodeGuid;
        target.NodeAlias = cmsTree.NodeAlias;
        // target.NodeLevel = source.NodeLevel;
        target.NodeName = cmsTree.NodeName;
        // target.NodeOrder = source.NodeOrder;

        if (mappingHelper.TranslateRequiredId<KX13M.CmsUser>(u => u.UserId, cmsTree.NodeOwner, out var ownerUserId))
        {
            target.NodeOwner = ownerUserId;
        }

        // target.NodeAliasPath = source.NodeAliasPath;
        // target.NodeClassName = source.NodeClass.ClassName;
        KenticoHelper.CopyCustomData(target.NodeCustomData, cmsTree.NodeCustomData);

        // target.NodeHasChildren = source.NodeHasChildren;
        // target.NodeHasLinks = source.NodeHasLinks;
        // target.NodeID = source.NodeId;
        // target.NodeSiteName =
        // target.NodeParentID = _pkContext.RequireMapFromSource<KX13.Models.CmsTree>(u => u.NodeId, source.NodeParentId ?? -1);
        if (mappingHelper.TranslateRequiredId<KX13M.CmsTree>(t => t.NodeId, cmsTree.NodeParentId, out var nodeParentId))
        {
            target.NodeParentID = nodeParentId;
        }
        // target.NodeSiteID =
        // TODO tk: 2022-06-30 guard linked node id
        // target.NodeLinkedNodeID = ;
        // target.NodeOriginalNodeID = ;
        // TODO tk: 2022-06-30 if different from current site, just skip
        // target.NodeLinkedNodeSiteID = ;
        
        var sourceDocument = cmsTree.CmsDocuments.Single(x => x.DocumentCulture == cultureCode);
        target.DocumentCulture = sourceDocument.DocumentCulture;
        target.DocumentContent.LoadContentXml(sourceDocument.DocumentContent ?? string.Empty);
        target.DocumentName = sourceDocument.DocumentName;

        KenticoHelper.CopyCustomData(target.DocumentCustomData, sourceDocument.DocumentCustomData);

        // target.DocumentID = sourceDocument.DocumentId;
        // target.DocumentIsArchived = sourceDocument.DocumentIsArchived;
        
        // target.DocumentLastPublished = sourceDocument.DocumentLastPublished;
        target.SetValue(nameof(target.DocumentLastPublished), sourceDocument.DocumentLastPublished);
        
        // target.DocumentModifiedWhen = sourceDocument.DocumentModifiedWhen;
        target.SetValue(nameof(target.DocumentModifiedWhen), sourceDocument.DocumentModifiedWhen);
        
        // target.DocumentCreatedWhen = sourceDocument.DocumentCreatedWhen;
        target.SetValue(nameof(target.DocumentCreatedWhen), sourceDocument.DocumentCreatedWhen);
        
        target.DocumentPublishFrom = sourceDocument.DocumentPublishFrom.GetValueOrDefault();
        target.DocumentPublishTo = sourceDocument.DocumentPublishTo.GetValueOrDefault();
        target.DocumentSearchExcluded = sourceDocument.DocumentSearchExcluded.GetValueOrDefault();
        // TODO tk: 2022-06-30  target.DocumentsOnPath
        // target.DocumentCheckedOutWhen = sourceDocument.DocumentCheckedOutWhen;
        // target.DocumentLastVersionName = sourceDocument.DocumentLastVersionNumber;
        // target.DocumentNodeID = sourceDocument.DocumentNodeId;
        // target.DocumentWorkflowActionStatus = sourceDocument.DocumentWorkflowActionStatus;
        target.DocumentGUID = sourceDocument.DocumentGuid.GetValueOrDefault();
        // target.DocumentWorkflowStepID = sourceDocument.DocumentWorkflowStepId;

        if (mappingHelper.TranslateRequiredId<KX13M.CmsUser>(u => u.UserId, sourceDocument.DocumentCreatedByUserId, out var createdByUserId))
        {
            // target.DocumentCreatedByUserID = sourceDocument.DocumentCreatedByUserId;
            target.SetValue(nameof(target.DocumentCreatedByUserID), createdByUserId); // TODO tk: 2022-07-06 not working
        }

        if (mappingHelper.TranslateRequiredId<KX13M.CmsUser>(u => u.UserId, sourceDocument.DocumentModifiedByUserId, out var modifiedByUserId))
        {
            // target.DocumentModifiedByUserID = sourceDocument.DocumentModifiedByUserId;
            target.SetValue(nameof(target.DocumentModifiedByUserID), modifiedByUserId); // TODO tk: 2022-07-06 not working
        }

        // target.DocumentPublishedVersionHistoryID = sourceDocument.DocumentPublishedVersionHistoryId;
        // target.DocumentCheckedOutByUserID = sourceDocument.DocumentCreatedByUserId;
        target.DocumentWorkflowCycleGUID = sourceDocument.DocumentWorkflowCycleGuid.GetValueOrDefault();
        // target.CoupledClassIDColumn = 

        // Set coupled data
        // var fieldsInfo = new DocumentFieldsInfo(cmsTree.NodeClass.ClassName);
        // if (cmsTree.NodeClass.ClassIsCoupledClass)
        // {
        //     Debug.Assert(cmsTree.NodeClass.ClassTableName != null, "cmsTree.NodeClass.ClassTableName != null");
        //     
        //     var coupledDataRow = _coupledDataService.GetSourceCoupledDataRow(cmsTree.NodeClass.ClassTableName, fieldsInfo.TypeInfo.IDColumn, sourceDocument.DocumentForeignKeyValue);
        //     if (coupledDataRow != null)
        //     {
        //         var fields = _classService.GetClassFields(cmsTree.NodeClass.ClassGuid);
        //         foreach (var classColumnModel in fields)
        //         {
        //             ProcessCoupledDataColumn(target, classColumnModel, coupledDataRow, fieldsInfo);
        //         }
        //         // foreach (var (key, value) in coupledDataRow)
        //         // {
        //         //     if (key != fieldsInfo.TypeInfo.IDColumn)
        //         //     {
        //         //         target.SetValue(key, value);
        //         //     }
        //         // }
        //     }
        //     else
        //     {
        //         _logger.LogWarning("Coupled data is missing for source document {documentId} of class {className}", sourceDocument.DocumentId, cmsTree.NodeClass.ClassName);
        //     }
        // }
        
        var classStructureInfo = new ClassStructureInfo(cmsTree.NodeClass.ClassName, cmsTree.NodeClass.ClassXmlSchema, cmsTree.NodeClass.ClassTableName);
        var formInfo = new FormInfo(cmsTree.NodeClass.ClassFormDefinition);
        var fieldsInfo = new DocumentFieldsInfo(cmsTree.NodeClass.ClassName);
        var columnNames = formInfo.GetColumnNames();

        if (cmsTree.NodeClass.ClassIsCoupledClass)
        {
            // Debug.Assert(columnNames.Count == classStructureInfo.ColumnsCount, "columnNames.Count == classStructureInfo.ColumnsCount");
            Debug.Assert(cmsTree.NodeClass.ClassTableName != null, "cmsTree.NodeClass.ClassTableName != null");
            
            var coupledDataRow = _coupledDataService.GetSourceCoupledDataRow(cmsTree.NodeClass.ClassTableName, fieldsInfo.TypeInfo.IDColumn, sourceDocument.DocumentForeignKeyValue);
            
            foreach (var columnName in columnNames)
            {
                var field = formInfo.GetFormField(columnName);
                var controlName = field.Settings["controlname"]?.ToString()?.ToLowerInvariant();

                Debug.Assert(coupledDataRow != null, nameof(coupledDataRow) + " != null");
                
                if (coupledDataRow.TryGetValue(columnName, out var value))
                {
                    if (controlName != null)
                    {
                        // Debug.Assert(ClassHelper.KnownControlNames.Contains(controlName), "ClassHelper.KnownControlNames.Contains(controlName)");
                    
                        switch (_classService.GetFormControlDefinition(controlName))
                        {
                            case { UserControlForFile: true } control: 
                            {
                                // attachment - migrate attachment from field and leave link to it
                                if (value is Guid attachmentGuid)
                                {
                                    var (success, canContinue, mediaFileInfo, mediaLibraryInfo) = _attachmentMigrator.MigrateAttachment(attachmentGuid);
                                    if (success)
                                    {
                                        target.SetValue(columnName, $"Attachment moved to media library, media file path: ({mediaLibraryInfo?.LibraryFolder}/{mediaFileInfo?.FilePath})");    
                                    }
                                }
                                else if (value is string attachmentGuidStr && Guid.TryParse(attachmentGuidStr, out attachmentGuid))
                                {
                                    var (success, canContinue, mediaFileInfo, mediaLibraryInfo) = _attachmentMigrator.MigrateAttachment(attachmentGuid);
                                    if (success)
                                    {
                                        target.SetValue(columnName, $"Attachment moved to media library, media file path: ({mediaLibraryInfo?.LibraryFolder}/{mediaFileInfo?.FilePath})");    
                                    }
                                }

                                break;
                            }
                            case { UserControlForDocAttachments: true } control:
                            {
                                // attachment - migrate attachment from field and leave link to it
                                var result = new StringBuilder();
                                foreach (var (success, canContinue, mediaFileInfo, mediaLibraryInfo) in _attachmentMigrator.MigrateGroupedAttachments(sourceDocument.DocumentId, field.Guid, field.Name))
                                {
                                    if (!canContinue) break;
                                    if (success)
                                    {
                                        result.AppendLine($"Grouped attachment moved to media library, media file path: ({mediaLibraryInfo?.LibraryFolder}/{mediaFileInfo?.FilePath})");
                                    }
                                }
                                target.SetValue(columnName, result.ToString());
                                break;
                            }
                            case { UserControlForDocRelationships: true } control:
                            {
                                // relation to other document
                                var result = new StringBuilder();
                                foreach (var nodeRelationship in _relationshipService.GetNodeRelationships(cmsTree.NodeId))
                                {
                                    result.AppendLine($"Document relationship is currently not supported - related to: {nodeRelationship.RightNode?.NodeAliasPath}");
                                }
                                
                                target.SetValue(columnName, result.ToString());
                                break;
                            }
                            default:
                                // leave as is
                                target.SetValue(columnName, value);
                                break;
                        }
                    }
                    else
                    {
                        target.SetValue(columnName, value);
                    }
                }
                else
                {
                    _logger.LogWarning("Coupled data is missing for source document {DocumentId} of class {ClassName}", sourceDocument.DocumentId, cmsTree.NodeClass.ClassName);
                }
            }
        }
        
        

        return target;
    }

    private static void ProcessCoupledDataColumn(TreeNode target, ClassColumnModel classColumnModel, Dictionary<string, object> coupledDataRow, DocumentFieldsInfo fieldsInfo)
    {
        Debug.Assert(classColumnModel.ColumnName != null, "classColumnModel.ColumnName != null");

        if (coupledDataRow.TryGetValue(classColumnModel.ColumnName, out var value))
        {
            if (classColumnModel.ColumnName != fieldsInfo.TypeInfo.IDColumn)
            {
                Debug.Assert(classColumnModel.FormControl?.UserControlShowInDocumentTypes == true, "classColumnModel.FormControl?.UserControlShowInDocumentTypes == true");

                if (classColumnModel.FormControl?.UserControlForFile == true)
                {
                    // TODO tk: 2022-07-13 check if guid is available


                    // TODO tk: 2022-07-13 check grouped attachment
                }

                switch (classColumnModel.FormControlCodeName)
                {
                    case "DirectUploadControl" when classColumnModel.ColumnType == typeof(Guid):
                        target.SetValue(classColumnModel.ColumnName, value);
                        break;
                    case "DocumentAttachmentsControl":
                        target.SetValue(classColumnModel.ColumnName, value);
                        break;
                    default:
                        target.SetValue(classColumnModel.ColumnName, value);
                        break;
                }
            }
        }
        else
        {
            // TODO tk: 2022-07-13 column data expected   
        }
    }
}