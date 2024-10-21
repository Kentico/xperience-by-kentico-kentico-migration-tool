﻿using CMS.ContentEngine;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;
using Migration.Tool.Common.Enumerations;
using Migration.Tool.KXP.Api;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source;
using Migration.Tool.Source.Auxiliary;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Services;
using Migration.Tool.Source.Services.Model;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Extensions.DefaultMigrations;

public class WidgetFileMigration(ILogger<WidgetFileMigration> logger, EntityIdentityFacade entityIdentityFacade, ModelFacade modelFacade, IAssetFacade assetFacade, KxpMediaFileFacade mediaFileFacade, ToolConfiguration configuration) : IWidgetPropertyMigration
{
    private const string MigratedComponent = Kx13FormComponents.Kentico_MediaFilesSelector;

    public int Rank => 100_000;

    public bool ShallMigrate(WidgetPropertyMigrationContext context, string propertyName)
        => MigratedComponent.Equals(context.EditingFormControlModel?.FormComponentIdentifier, StringComparison.InvariantCultureIgnoreCase);

    public Task<WidgetPropertyMigrationResult> MigrateWidgetProperty(string key, JToken? value, WidgetPropertyMigrationContext context)
    {
        (int siteId, _) = context;

        var refsToMedia = new List<object>();
        if (value?.ToObject<List<MediaFilesSelectorItem>>() is { Count: > 0 } mediaSelectorItems)
        {
            foreach (var mediaSelectorItem in mediaSelectorItems)
            {
                if (configuration.MigrateMediaToMediaLibrary)
                {
                    if (entityIdentityFacade.Translate<IMediaFile>(mediaSelectorItem.FileGuid, siteId) is var (_, identity) && mediaFileFacade.GetMediaFile(identity) is not null)
                    {
                        refsToMedia.Add(new Kentico.Components.Web.Mvc.FormComponents.MediaFilesSelectorItem { FileGuid = identity });
                    }
                    else
                    {
                        logger.LogError("Media file not found, media guid {MediaGuid}", mediaSelectorItem.FileGuid);
                    }
                }
                else
                {
                    var sourceMediaFile = modelFacade.SelectWhere<IMediaFile>("FileGUID = @mediaFileGuid AND FileSiteID = @fileSiteID", new SqlParameter("mediaFileGuid", mediaSelectorItem.FileGuid), new SqlParameter("fileSiteID", siteId))
                        .FirstOrDefault();
                    if (sourceMediaFile != null)
                    {
                        var (ownerContentItemGuid, _) = assetFacade.GetRef(sourceMediaFile);
                        refsToMedia.Add(new ContentItemReference { Identifier = ownerContentItemGuid });
                    }
                    else
                    {
                        logger.LogError("Media file not found, media guid {MediaGuid}", mediaSelectorItem.FileGuid);
                    }
                }
            }

            var resultAsJToken = JToken.FromObject(refsToMedia);
            return Task.FromResult(new WidgetPropertyMigrationResult(resultAsJToken));
        }
        else
        {
            logger.LogError("Failed to parse '{ComponentName}' json {Json}", MigratedComponent, value?.ToString() ?? "<null>");

            // leave value as it is
            return Task.FromResult(new WidgetPropertyMigrationResult(value));
        }
    }
}
