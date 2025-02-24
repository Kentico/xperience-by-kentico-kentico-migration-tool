using System.Diagnostics;
using CMS.DataEngine;
using CMS.FormEngine;
using Kentico.Xperience.UMT.Model;

namespace Migration.Tool.KXP.Api;

public record CustomizedFieldInfo(string FieldName);

public class KxpClassFacade
{
    public KxpClassFacade(KxpApiInitializer kxpApiInitializer) => kxpApiInitializer.EnsureApiIsInitialized();

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

    public DataClassInfo GetClass(Guid classGuid) => DataClassInfoProvider.GetDataClassInfo(classGuid);

    public DataClassInfo GetClass(string className) => DataClassInfoProvider.GetDataClassInfo(className);


    public IEnumerable<CustomizedFieldInfo> GetCustomizedFieldInfos(string className)
    {
        var dci = DataClassInfoProvider.GetDataClassInfo(className);
        var fi = new FormInfo(dci.ClassFormDefinition);
        foreach (string? columnName in fi.GetColumnNames())
        {
            var field = fi.GetFormField(columnName);
            if (!field.System)
            {
                yield return new CustomizedFieldInfo(columnName);
            }
        }
    }

    public IEnumerable<CustomizedFieldInfo> GetCustomizedFieldInfos(FormInfo formInfo)
    {
        foreach (string? columnName in formInfo.GetColumnNames())
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
        foreach (string? columnName in fi.GetColumnNames())
        {
            var field = fi.GetFormField(columnName);
            if (!field.System)
            {
                yield return new CustomizedFieldInfo(columnName);
            }
        }
    }

    /// <summary>
    /// Gets all classes in XbyK, filtered by content-type type
    /// </summary>
    public IEnumerable<DataClassInfo> GetClasses(string contentTypeType) => DataClassInfoProvider.GetClasses().WhereEquals(nameof(DataClassInfo.ClassContentTypeType), contentTypeType);
}
