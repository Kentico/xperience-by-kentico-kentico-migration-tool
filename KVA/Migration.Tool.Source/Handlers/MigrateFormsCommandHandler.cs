using System.Collections.Immutable;
using System.Diagnostics;
using System.Xml.Linq;

using CMS.Base;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.OnlineForms;
using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Common.Services.BulkCopy;
using Migration.Tool.KXP.Api;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Handlers;

public class MigrateFormsCommandHandler(
    ILogger<MigrateFormsCommandHandler> logger,
    IEntityMapper<ICmsClass, DataClassInfo> dataClassMapper,
    IEntityMapper<ICmsForm, BizFormInfo> cmsFormMapper,
    KxpClassFacade kxpClassFacade,
    BulkDataCopyService bulkDataCopyService,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    ModelFacade modelFacade,
    ToolConfiguration configuration
)
    : IRequestHandler<MigrateFormsCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateFormsCommand request, CancellationToken cancellationToken)
    {
        var cmsClassForms = modelFacade.Select<ICmsClass>("ClassIsForm = 1", "ClassID");
        foreach (var ksClass in cmsClassForms)
        {
            protocol.FetchedSource(ksClass);

            var kxoDataClass = kxpClassFacade.GetClass(ksClass.ClassGUID);
            protocol.FetchedTarget(kxoDataClass);

            bool classSuccessFullySaved = MapAndSaveUsingKxoApi(ksClass, kxoDataClass);
            if (!classSuccessFullySaved)
            {
                continue;
            }

            var cmsForms = modelFacade.Select<ICmsForm>("FormClassID = @classId", "FormID", new SqlParameter("ClassID", ksClass.ClassID));

            foreach (var ksCmsForm in cmsForms)
            {
                protocol.FetchedSource(ksCmsForm);

                var kxoCmsForm = BizFormInfo.Provider.Get(ksCmsForm.FormGUID);

                protocol.FetchedTarget(kxoCmsForm);

                var mapped = cmsFormMapper.Map(ksCmsForm, kxoCmsForm);
                protocol.MappedTarget(mapped);

                if (mapped is { Success: true } result)
                {
                    (var cmsForm, bool newInstance) = result;
                    ArgumentNullException.ThrowIfNull(cmsForm, nameof(cmsForm));


                    try
                    {
                        int formItems = cmsForm.FormItems;
                        BizFormInfo.Provider.Set(cmsForm!);

                        // Update FormItems manually, as BizFormInfo provider always resets the value to 0
                        using (var conn = ConnectionHelper.GetConnection())
                        {
                            conn.DataConnection.ExecuteNonQuery("UPDATE [CMS_Form] SET FormItems=@formItems WHERE FormID=@formID", new QueryDataParameters() { { "formItems", formItems }, { "formID", cmsForm.FormID } }, QueryTypeEnum.SQLQuery, true);
                        }

                        logger.LogEntitySetAction(newInstance, cmsForm);

                        primaryKeyMappingContext.SetMapping<BizFormInfo>(
                            r => r.FormID,
                            ksClass.ClassID,
                            cmsForm.FormID
                        );
                    }
                    catch (Exception ex)
                    {
                        protocol.Append(HandbookReferences
                            .ErrorCreatingTargetInstance<BizFormInfo>(ex)
                            .NeedsManualAction()
                            .WithIdentityPrint(cmsForm)
                        );
                        logger.LogEntitySetError(ex, newInstance, cmsForm);

                        continue;
                    }
                }

                Debug.Assert(ksClass.ClassTableName != null, "kx13Class.ClassTableName != null");
                // var csi = new ClassStructureInfo(kx13Class.ClassXmlSchema, kx13Class.ClassXmlSchema, kx13Class.ClassTableName);

                XNamespace nsSchema = "http://www.w3.org/2001/XMLSchema";
                XNamespace msSchema = "urn:schemas-microsoft-com:xml-msdata";
                var xDoc = XDocument.Parse(ksClass.ClassXmlSchema);
                var autoIncrementColumns = xDoc.Descendants(nsSchema + "element")
                    .Where(x => x.Attribute(msSchema + "AutoIncrement")?.Value == "true")
                    .Select(x => x.Attribute("name")?.Value).ToImmutableHashSet();

                Debug.Assert(autoIncrementColumns.Count == 1, "autoIncrementColumns.Count == 1");

                var r = (ksClass.ClassTableName, ksClass.ClassGUID, autoIncrementColumns);
                logger.LogTrace("Class '{ClassGuild}' Resolved as: {Result}", ksClass.ClassGUID, r);

                // check if data is present in target tables
                if (bulkDataCopyService.CheckIfDataExistsInTargetTable(ksClass.ClassTableName))
                {
                    logger.LogWarning("Data exists in target coupled data table '{TableName}' - cannot migrate, skipping form data migration", r.ClassTableName);
                    protocol.Append(HandbookReferences
                        .DataMustNotExistInTargetInstanceTable(ksClass.ClassTableName)
                    );
                    continue;
                }

                var bulkCopyRequest = new BulkCopyRequest(
                    ksClass.ClassTableName, s => !autoIncrementColumns.Contains(s), _ => true,
                    20000
                );

                logger.LogTrace("Bulk data copy request: {Request}", bulkCopyRequest);
                bulkDataCopyService.CopyTableToTable(bulkCopyRequest);
            }
        }

        await GlobalizeBizFormFiles();

        return new GenericCommandResult();
    }

    private bool MapAndSaveUsingKxoApi(ICmsClass ksClass, DataClassInfo kxoDataClass)
    {
        var mapped = dataClassMapper.Map(ksClass, kxoDataClass);
        protocol.MappedTarget(mapped);

        if (mapped is { Success: true })
        {
            (var dataClassInfo, bool newInstance) = mapped;
            ArgumentNullException.ThrowIfNull(dataClassInfo, nameof(dataClassInfo));

            try
            {
                kxpClassFacade.SetClass(dataClassInfo);

                protocol.Success(ksClass, dataClassInfo, mapped);
                logger.LogEntitySetAction(newInstance, dataClassInfo);

                primaryKeyMappingContext.SetMapping<DataClassInfo>(
                    r => r.ClassID,
                    ksClass.ClassID,
                    dataClassInfo.ClassID
                );

                return true;
            }
            catch (Exception ex)
            {
                protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<DataClassInfo>(ex)
                    .NeedsManualAction()
                    .WithIdentityPrint(dataClassInfo)
                );
                logger.LogEntitySetError(ex, newInstance, dataClassInfo);
            }
        }

        return false;
    }

    #region Directory globalization

    private async Task GlobalizeBizFormFiles()
    {
        foreach (var cmsSite in modelFacade.SelectAll<ICmsSite>())
        {
            string globalBizformFiles = CMS.IO.Path.Combine(SystemContext.WebApplicationPhysicalPath, "BizFormFiles");
            string siteBizFormFiles = CMS.IO.Path.Combine(configuration.KxCmsDirPath, cmsSite.SiteName, "bizformfiles");
            if (CMS.IO.Directory.Exists(siteBizFormFiles))
            {
                Debug.WriteLine($"Copying site bizformfiles from '{siteBizFormFiles}' to global bizformfiles '{globalBizformFiles}'");
                try
                {
                    var source = CMS.IO.DirectoryInfo.New(siteBizFormFiles);
                    var target = CMS.IO.DirectoryInfo.New(globalBizformFiles);
                    CopyAll(source, target);
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Moving site bizformfiles failed with {ex}");
                }
            }
            else
            {
                Debug.WriteLine($"Directory '{siteBizFormFiles}' not exists");
            }
        }
    }

    internal static void CopyAll(CMS.IO.DirectoryInfo source, CMS.IO.DirectoryInfo target)
    {
        var stack = new Stack<(CMS.IO.DirectoryInfo source, CMS.IO.DirectoryInfo target)>();
        stack.Push((source, target));

        while (stack.Count > 0)
        {
            var (s, t) = stack.Pop();
            if (string.Equals(s.FullName, t.FullName, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            // Check if the target directory exists, if not, create it.
            if (!CMS.IO.Directory.Exists(t.FullName))
            {
                CMS.IO.Directory.CreateDirectory(t.FullName);
            }

            // Copy each file into it's new directory.
            foreach (var fi in s.GetFiles())
            {
                Debug.WriteLine($@"Moving {t.FullName}\{fi.Name}");
                fi.CopyTo(CMS.IO.Path.Combine(t.FullName, fi.Name), true);
                // fi.Delete();
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in s.GetDirectories())
            {
                var nextTargetSubDir = t.CreateSubdirectory(diSourceSubDir.Name);
                stack.Push((diSourceSubDir, nextTargetSubDir));
            }
        }

        // missing overload in source.Delete(true);, replaced with:
        // CMS.IO.Directory.Delete(source.FullName, true);
    }

    #endregion
}
