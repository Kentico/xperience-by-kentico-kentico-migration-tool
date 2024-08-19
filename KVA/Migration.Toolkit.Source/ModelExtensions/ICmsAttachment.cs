using System.Collections.Frozen;
using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;

public partial interface ICmsAttachment : ISourceGuidEntity
{
    (Guid EntityGuid, int? SiteId) ISourceGuidEntity.GetIdentity() => (AttachmentGUID, AttachmentSiteID);

    static FrozenDictionary<Guid, int[]> ISourceGuidEntity.Load(ModelFacade modelFacade) =>
        modelFacade
            .Select("SELECT AttachmentGUID, AttachmentSiteID FROM CMS_Attachment", (reader, _) => new { AttachmentGUID = reader.Unbox<Guid>("AttachmentGUID"), AttachmentSiteID = reader.Unbox<int>("AttachmentSiteID") })
            .ToLookup(x => x.AttachmentGUID)
            .ToFrozenDictionary(
                x => x.Key,
                x => x.Select(z => z.AttachmentSiteID).ToArray()
            );

    static Guid ISourceGuidEntity.NewGuidNs => new("78ACF349-948E-4DB8-BD3C-F91D1D072122");
}
