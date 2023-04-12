namespace Migration.Toolkit.KXP.Api;

using System.Diagnostics;
using CMS.DataEngine;
using CMS.FormEngine;
using Migration.Toolkit.Common.Enumerations;

public record CustomizedFieldInfo(string FieldName);

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

        // assert class form is well formed
        // var formInfo = FormHelper.GetFormInfo(dataClassInfo.ClassName, false);
        // var formElements = formInfo.GetFormElements(true, true);

        return dataClassInfo;
    }

    public DataClassInfo GetClass(Guid classGuid)
    {
        return DataClassInfoProvider.GetDataClassInfo(classGuid);
    }

    public DataClassInfo GetClass(string className)
    {
        return DataClassInfoProvider.GetDataClassInfo(className);
    }


    public IEnumerable<CustomizedFieldInfo> GetCustomizedFieldInfos(string className)
    {
        var dci = DataClassInfoProvider.GetDataClassInfo(className);
        if (Kx13SystemClass.Customizable.Contains(dci.ClassName)) //customizable class
        {
            var fi = new FormInfo(dci.ClassFormDefinition);
            foreach (var columnName in fi.GetColumnNames())
            {
                var field = fi.GetFormField(columnName);
                if (!field.System)
                {
                    yield return new CustomizedFieldInfo(columnName);
                }
            }
        }
        // if (dci.ClassShowAsSystemTable) // custom class
        // {
        //     yield break;
        // }

        yield break;
    }

    public IEnumerable<CustomizedFieldInfo> GetCustomizedFieldInfos(FormInfo formInfo)
    {
        foreach (var columnName in formInfo.GetColumnNames())
        {
            var field = formInfo.GetFormField(columnName);
            if (!field.System)
            {
                yield return new CustomizedFieldInfo(columnName);
            }
        }
    }

    public IEnumerable<CustomizedFieldInfo> GetCustomizedFieldInfosAll(string className)
    {
        var dci = DataClassInfoProvider.GetDataClassInfo(className);

        var fi = new FormInfo(dci.ClassFormDefinition);
        foreach (var columnName in fi.GetColumnNames())
        {
            var field = fi.GetFormField(columnName);
            if (!field.System)
            {
                yield return new CustomizedFieldInfo(columnName);
            }
        }
    }
}