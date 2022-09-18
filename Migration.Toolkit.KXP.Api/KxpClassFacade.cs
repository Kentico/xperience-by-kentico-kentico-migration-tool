namespace Migration.Toolkit.KXP.Api;

using System.Diagnostics;
using CMS.DataEngine;

public class KxpClassFacade
{
    public KxpClassFacade(KxpApiInitializer kxpApiInitializer)
    {
        kxpApiInitializer.EnsureApiIsInitialized();
    }
    
    public DataClassInfo SetClass(DataClassInfo dataClassInfo)
    {
        // TODO tk: 2022-05-30  dataClassInfo.ClassConnectionString - check, might cause unexpected behavior
        DataClassInfoProvider.SetDataClassInfo(dataClassInfo);
        Debug.Assert(dataClassInfo.ClassID != 0, "dataClassInfo.ClassID != 0");
        return dataClassInfo;
    }

    public DataClassInfo GetClass(Guid classGuid)
    {
        return DataClassInfoProvider.GetDataClassInfo(classGuid);
    }
}