using System.Diagnostics;
using System.Text;
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

using Migration.Toolkit.Common;
using Migration.Toolkit.KX13.Models;

public record CmsTreeMapperSource(KX13M.CmsTree CmsTree, string CultureCode);

public class TreeNodeMapper: EntityMapperBase<CmsTreeMapperSource, TreeNode>
{
    private const string CLASS_FIELD_CONTROL_NAME = "controlname";
    private readonly ILogger<TreeNodeMapper> _logger;
    private readonly CoupledDataService _coupledDataService;
    private readonly ClassService _classService;
    private readonly AttachmentMigrator _attachmentMigrator;
    private readonly CmsRelationshipService _relationshipService;

    public TreeNodeMapper(
        ILogger<TreeNodeMapper> logger, 
        PrimaryKeyMappingContext pkContext, 
        CoupledDataService coupledDataService, 
        IProtocol protocol, 
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
            _logger.LogTrace("Assertion failed, entity key mismatch");
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
        // target.NodeLinkedNodeID = ;
        // target.NodeOriginalNodeID = ;
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
        // TODO target.DocumentsOnPath
        // target.DocumentCheckedOutWhen = sourceDocument.DocumentCheckedOutWhen;
        // target.DocumentLastVersionName = sourceDocument.DocumentLastVersionNumber;
        // target.DocumentNodeID = sourceDocument.DocumentNodeId;
        // target.DocumentWorkflowActionStatus = sourceDocument.DocumentWorkflowActionStatus;
        target.DocumentGUID = sourceDocument.DocumentGuid.GetValueOrDefault();
        // target.DocumentWorkflowStepID = sourceDocument.DocumentWorkflowStepId;

        if (mappingHelper.TranslateRequiredId<KX13M.CmsUser>(u => u.UserId, sourceDocument.DocumentCreatedByUserId, out var createdByUserId))
        {
            target.SetValue(nameof(target.DocumentCreatedByUserID), createdByUserId);
        }

        if (mappingHelper.TranslateRequiredId<KX13M.CmsUser>(u => u.UserId, sourceDocument.DocumentModifiedByUserId, out var modifiedByUserId))
        {
            target.SetValue(nameof(target.DocumentModifiedByUserID), modifiedByUserId);
        }

        // target.DocumentPublishedVersionHistoryID = sourceDocument.DocumentPublishedVersionHistoryId;
        // target.DocumentCheckedOutByUserID = sourceDocument.DocumentCreatedByUserId;
        target.DocumentWorkflowCycleGUID = sourceDocument.DocumentWorkflowCycleGuid.GetValueOrDefault();
        // target.CoupledClassIDColumn = 
        
        var formInfo = new FormInfo(cmsTree.NodeClass.ClassFormDefinition);
        var fieldsInfo = new DocumentFieldsInfo(cmsTree.NodeClass.ClassName);
        var columnNames = formInfo.GetColumnNames();

        if (cmsTree.NodeClass.ClassIsCoupledClass)
        {
            MapCoupledDataFieldValues(target, cmsTree, fieldsInfo, sourceDocument, columnNames, formInfo);
        }

        return target;
    }

    private void MapCoupledDataFieldValues(TreeNode target, CmsTree cmsTree, DocumentFieldsInfo fieldsInfo, CmsDocument sourceDocument, List<string> columnNames,
        FormInfo formInfo)
    {
        Debug.Assert(cmsTree.NodeClass.ClassTableName != null, "cmsTree.NodeClass.ClassTableName != null");

        var coupledDataRow = _coupledDataService.GetSourceCoupledDataRow(cmsTree.NodeClass.ClassTableName, fieldsInfo.TypeInfo.IDColumn,
            sourceDocument.DocumentForeignKeyValue);

        foreach (var columnName in columnNames)
        {
            var field = formInfo.GetFormField(columnName);
            var controlName = field.Settings[CLASS_FIELD_CONTROL_NAME]?.ToString()?.ToLowerInvariant();

            Debug.Assert(coupledDataRow != null, nameof(coupledDataRow) + " != null");

            if (coupledDataRow.TryGetValue(columnName, out var value))
            {
                if (controlName != null)
                {
                    switch (_classService.GetFormControlDefinition(controlName))
                    {
                        case { UserControlForFile: true }:
                        {
                            // attachment - migrate attachment from field and leave link to it
                            if (value is Guid attachmentGuid)
                            {
                                var (success, _, mediaFileInfo, mediaLibraryInfo) = _attachmentMigrator.MigrateAttachment(attachmentGuid);
                                if (success)
                                {
                                    target.SetValue(columnName, string.Format(Resources.Attachment_MovedToLibrary, mediaLibraryInfo?.LibraryFolder, mediaFileInfo?.FilePath));
                                }
                            }
                            else if (value is string attachmentGuidStr && Guid.TryParse(attachmentGuidStr, out attachmentGuid))
                            {
                                var (success, _, mediaFileInfo, mediaLibraryInfo) = _attachmentMigrator.MigrateAttachment(attachmentGuid);
                                if (success)
                                {
                                    target.SetValue(columnName, string.Format(Resources.Attachment_MovedToLibrary, mediaLibraryInfo?.LibraryFolder, mediaFileInfo?.FilePath));
                                }
                            }

                            break;
                        }
                        case { UserControlForDocAttachments: true }:
                        {
                            // attachment - migrate attachment from field and leave link to it
                            var result = new StringBuilder();
                            foreach (var (success, canContinue, mediaFileInfo, mediaLibraryInfo) in _attachmentMigrator.MigrateGroupedAttachments(
                                         sourceDocument.DocumentId, field.Guid, field.Name))
                            {
                                if (!canContinue) break;
                                if (success)
                                {
                                    result.AppendLine(string.Format(Resources.AttachmentGrouped_ModevToLibrary, mediaLibraryInfo?.LibraryFolder, mediaFileInfo?.FilePath));
                                }
                            }

                            target.SetValue(columnName, result.ToString());
                            break;
                        }
                        case { UserControlForDocRelationships: true }:
                        {
                            // relation to other document
                            var result = new StringBuilder();
                            foreach (var nodeRelationship in _relationshipService.GetNodeRelationships(cmsTree.NodeId))
                            {
                                result.AppendLine(string.Format(Resources.Document_Relationship_NotSupported, nodeRelationship.RightNode?.NodeAliasPath));
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
}