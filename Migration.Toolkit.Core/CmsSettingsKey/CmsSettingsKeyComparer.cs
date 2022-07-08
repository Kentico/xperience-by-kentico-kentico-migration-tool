
using Migration.Toolkit.Common;

namespace Migration.Toolkit.Core.CmsSettingsKey;

public class CmsSettingsKeyComparer: IDataEqualityComparer<Migration.Toolkit.KX13.Models.CmsSettingsKey, Migration.Toolkit.KXO.Models.CmsSettingsKey>
{
    public bool DataEquals(Migration.Toolkit.KX13.Models.CmsSettingsKey? source, Migration.Toolkit.KXO.Models.CmsSettingsKey? target)
    {
        if (source is null && target is null) return true;
        if (source is null) return false;
        if (target is null) return false;

        return source.KeyId == target.KeyId &&
               source.KeyName == target.KeyName &&
               source.KeyDisplayName == target.KeyDisplayName &&
               source.KeyDescription == target.KeyDescription &&
               source.KeyValue == target.KeyValue &&
               source.KeyType == target.KeyType &&
               source.KeyCategoryId == target.KeyCategoryId &&
               source.SiteId == target.SiteId &&
               source.KeyGuid == target.KeyGuid &&
               source.KeyLastModified == target.KeyLastModified &&
               source.KeyOrder == target.KeyOrder &&
               source.KeyDefaultValue == target.KeyDefaultValue &&
               source.KeyValidation == target.KeyValidation &&
               source.KeyEditingControlPath == target.KeyEditingControlPath &&
               source.KeyIsGlobal == target.KeyIsGlobal &&
               source.KeyIsCustom == target.KeyIsCustom &&
               source.KeyIsHidden == target.KeyIsHidden &&
               source.KeyFormControlSettings == target.KeyFormControlSettings &&
               source.KeyExplanationText == target.KeyExplanationText;


        // TODO tk: 2022-05-08 deps
        // public virtual CmsSettingsCategory? KeyCategory { get; set; }
        //
        //
        // public virtual CmsSite? Site { get; set; }
    }
}