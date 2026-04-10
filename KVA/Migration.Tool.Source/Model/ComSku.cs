// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Tool.Common;

namespace Migration.Tool.Source.Model;

public partial interface IComSku : ISourceModel<IComSku>
{
    int SKUID { get; }
    Guid SKUGUID { get; }
    int? SKUOptionCategoryID { get; }
    int? SKUOrder { get; }
    int? SKUSiteID { get; }
    string SKUName { get; }
    string? SKUNumber { get; }
    decimal SKUPrice { get; }
    decimal? SKURetailPrice { get; }
    int? SKUDepartmentID { get; }
    int? SKUBrandID { get; }
    int? SKUManufacturerID { get; }
    int? SKUSupplierID { get; }
    int? SKUCollectionID { get; }
    int? SKUTaxClassID { get; }
    string? SKUImagePath { get; }
    string? SKUShortDescription { get; }
    string? SKUDescription { get; }
    string? SKUProductType { get; }
    string? SKUCustomData { get; }
    DateTime? SKUCreated { get; }
    DateTime SKULastModified { get; }
    Guid? SKUMembershipGUID { get; }
    string? SKUValidity { get; }
    int? SKUValidFor { get; }
    DateTime? SKUValidUntil { get; }
    int? SKUEproductFilesCount { get; }
    string? SKUBundleInventoryType { get; }
    int? SKUBundleItemsCount { get; }
    DateTime? SKUInStoreFrom { get; }
    int? SKUPublicStatusID { get; }
    int? SKUInternalStatusID { get; }
    bool SKUEnabled { get; }
    bool? SKUNeedsShipping { get; }
    double? SKUWeight { get; }
    double? SKUHeight { get; }
    double? SKUWidth { get; }
    double? SKUDepth { get; }
    string? SKUTrackInventory { get; }
    bool? SKUSellOnlyAvailable { get; }
    int? SKUAvailableItems { get; }
    int? SKUReorderAt { get; }
    int? SKUAvailableInDays { get; }
    int? SKUMinItemsInOrder { get; }
    int? SKUMaxItemsInOrder { get; }
    int? SKUParentSKUID { get; }

    static string ISourceModel<IComSku>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => ComSkuK11.GetPrimaryKeyName(version),
        { Major: 12 } => ComSkuK12.GetPrimaryKeyName(version),
        { Major: 13 } => ComSkuK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<IComSku>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => ComSkuK11.IsAvailable(version),
        { Major: 12 } => ComSkuK12.IsAvailable(version),
        { Major: 13 } => ComSkuK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<IComSku>.TableName => "COM_SKU";
    static string ISourceModel<IComSku>.GuidColumnName => "SKUGUID"; //assumtion, class Guid column doesn't change between versions
    static IComSku ISourceModel<IComSku>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => ComSkuK11.FromReader(reader, version),
        { Major: 12 } => ComSkuK12.FromReader(reader, version),
        { Major: 13 } => ComSkuK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record ComSkuK11(int SKUID, Guid SKUGUID, int? SKUOptionCategoryID, int? SKUOrder, int? SKUSiteID, string SKUName, string? SKUNumber, decimal SKUPrice, decimal? SKURetailPrice, int? SKUDepartmentID, int? SKUBrandID, int? SKUManufacturerID, int? SKUSupplierID, int? SKUCollectionID, int? SKUTaxClassID, string? SKUImagePath, string? SKUShortDescription, string? SKUDescription, string? SKUProductType, string? SKUCustomData, DateTime? SKUCreated, DateTime SKULastModified, Guid? SKUMembershipGUID, string? SKUValidity, int? SKUValidFor, DateTime? SKUValidUntil, int? SKUEproductFilesCount, string? SKUBundleInventoryType, int? SKUBundleItemsCount, DateTime? SKUInStoreFrom, int? SKUPublicStatusID, int? SKUInternalStatusID, bool SKUEnabled, bool? SKUNeedsShipping, double? SKUWeight, double? SKUHeight, double? SKUWidth, double? SKUDepth, string? SKUTrackInventory, bool? SKUSellOnlyAvailable, int? SKUAvailableItems, int? SKUReorderAt, int? SKUAvailableInDays, int? SKUMinItemsInOrder, int? SKUMaxItemsInOrder, string? SKUConversionName, string? SKUConversionValue, int? SKUParentSKUID) : IComSku, ISourceModel<ComSkuK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "SKUID";
    public static string TableName => "COM_SKU";
    public static string GuidColumnName => "SKUGUID";
    static ComSkuK11 ISourceModel<ComSkuK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SKUID"), reader.Unbox<Guid>("SKUGUID"), reader.Unbox<int?>("SKUOptionCategoryID"), reader.Unbox<int?>("SKUOrder"), reader.Unbox<int?>("SKUSiteID"), reader.Unbox<string>("SKUName"), reader.Unbox<string?>("SKUNumber"), reader.Unbox<decimal>("SKUPrice"), reader.Unbox<decimal?>("SKURetailPrice"), reader.Unbox<int?>("SKUDepartmentID"), reader.Unbox<int?>("SKUBrandID"), reader.Unbox<int?>("SKUManufacturerID"), reader.Unbox<int?>("SKUSupplierID"), reader.Unbox<int?>("SKUCollectionID"), reader.Unbox<int?>("SKUTaxClassID"), reader.Unbox<string?>("SKUImagePath"), reader.Unbox<string?>("SKUShortDescription"), reader.Unbox<string?>("SKUDescription"), reader.Unbox<string?>("SKUProductType"), reader.Unbox<string?>("SKUCustomData"), reader.Unbox<DateTime?>("SKUCreated"), reader.Unbox<DateTime>("SKULastModified"), reader.Unbox<Guid?>("SKUMembershipGUID"), reader.Unbox<string?>("SKUValidity"), reader.Unbox<int?>("SKUValidFor"), reader.Unbox<DateTime?>("SKUValidUntil"), reader.Unbox<int?>("SKUEproductFilesCount"), reader.Unbox<string?>("SKUBundleInventoryType"), reader.Unbox<int?>("SKUBundleItemsCount"), reader.Unbox<DateTime?>("SKUInStoreFrom"), reader.Unbox<int?>("SKUPublicStatusID"), reader.Unbox<int?>("SKUInternalStatusID"), reader.Unbox<bool>("SKUEnabled"), reader.Unbox<bool?>("SKUNeedsShipping"), reader.Unbox<double?>("SKUWeight"), reader.Unbox<double?>("SKUHeight"), reader.Unbox<double?>("SKUWidth"), reader.Unbox<double?>("SKUDepth"), reader.Unbox<string?>("SKUTrackInventory"), reader.Unbox<bool?>("SKUSellOnlyAvailable"), reader.Unbox<int?>("SKUAvailableItems"), reader.Unbox<int?>("SKUReorderAt"), reader.Unbox<int?>("SKUAvailableInDays"), reader.Unbox<int?>("SKUMinItemsInOrder"), reader.Unbox<int?>("SKUMaxItemsInOrder"), reader.Unbox<string?>("SKUConversionName"), reader.Unbox<string?>("SKUConversionValue"), reader.Unbox<int?>("SKUParentSKUID")
        );
    public static ComSkuK11 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SKUID"), reader.Unbox<Guid>("SKUGUID"), reader.Unbox<int?>("SKUOptionCategoryID"), reader.Unbox<int?>("SKUOrder"), reader.Unbox<int?>("SKUSiteID"), reader.Unbox<string>("SKUName"), reader.Unbox<string?>("SKUNumber"), reader.Unbox<decimal>("SKUPrice"), reader.Unbox<decimal?>("SKURetailPrice"), reader.Unbox<int?>("SKUDepartmentID"), reader.Unbox<int?>("SKUBrandID"), reader.Unbox<int?>("SKUManufacturerID"), reader.Unbox<int?>("SKUSupplierID"), reader.Unbox<int?>("SKUCollectionID"), reader.Unbox<int?>("SKUTaxClassID"), reader.Unbox<string?>("SKUImagePath"), reader.Unbox<string?>("SKUShortDescription"), reader.Unbox<string?>("SKUDescription"), reader.Unbox<string?>("SKUProductType"), reader.Unbox<string?>("SKUCustomData"), reader.Unbox<DateTime?>("SKUCreated"), reader.Unbox<DateTime>("SKULastModified"), reader.Unbox<Guid?>("SKUMembershipGUID"), reader.Unbox<string?>("SKUValidity"), reader.Unbox<int?>("SKUValidFor"), reader.Unbox<DateTime?>("SKUValidUntil"), reader.Unbox<int?>("SKUEproductFilesCount"), reader.Unbox<string?>("SKUBundleInventoryType"), reader.Unbox<int?>("SKUBundleItemsCount"), reader.Unbox<DateTime?>("SKUInStoreFrom"), reader.Unbox<int?>("SKUPublicStatusID"), reader.Unbox<int?>("SKUInternalStatusID"), reader.Unbox<bool>("SKUEnabled"), reader.Unbox<bool?>("SKUNeedsShipping"), reader.Unbox<double?>("SKUWeight"), reader.Unbox<double?>("SKUHeight"), reader.Unbox<double?>("SKUWidth"), reader.Unbox<double?>("SKUDepth"), reader.Unbox<string?>("SKUTrackInventory"), reader.Unbox<bool?>("SKUSellOnlyAvailable"), reader.Unbox<int?>("SKUAvailableItems"), reader.Unbox<int?>("SKUReorderAt"), reader.Unbox<int?>("SKUAvailableInDays"), reader.Unbox<int?>("SKUMinItemsInOrder"), reader.Unbox<int?>("SKUMaxItemsInOrder"), reader.Unbox<string?>("SKUConversionName"), reader.Unbox<string?>("SKUConversionValue"), reader.Unbox<int?>("SKUParentSKUID")
        );
};
public partial record ComSkuK12(int SKUID, Guid SKUGUID, int? SKUOptionCategoryID, int? SKUOrder, int? SKUSiteID, string SKUName, string? SKUNumber, decimal SKUPrice, decimal? SKURetailPrice, int? SKUDepartmentID, int? SKUBrandID, int? SKUManufacturerID, int? SKUSupplierID, int? SKUCollectionID, int? SKUTaxClassID, string? SKUImagePath, string? SKUShortDescription, string? SKUDescription, string? SKUProductType, string? SKUCustomData, DateTime? SKUCreated, DateTime SKULastModified, Guid? SKUMembershipGUID, string? SKUValidity, int? SKUValidFor, DateTime? SKUValidUntil, int? SKUEproductFilesCount, string? SKUBundleInventoryType, int? SKUBundleItemsCount, DateTime? SKUInStoreFrom, int? SKUPublicStatusID, int? SKUInternalStatusID, bool SKUEnabled, bool? SKUNeedsShipping, double? SKUWeight, double? SKUHeight, double? SKUWidth, double? SKUDepth, string? SKUTrackInventory, bool? SKUSellOnlyAvailable, int? SKUAvailableItems, int? SKUReorderAt, int? SKUAvailableInDays, int? SKUMinItemsInOrder, int? SKUMaxItemsInOrder, string? SKUConversionName, string? SKUConversionValue, int? SKUParentSKUID) : IComSku, ISourceModel<ComSkuK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "SKUID";
    public static string TableName => "COM_SKU";
    public static string GuidColumnName => "SKUGUID";
    static ComSkuK12 ISourceModel<ComSkuK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SKUID"), reader.Unbox<Guid>("SKUGUID"), reader.Unbox<int?>("SKUOptionCategoryID"), reader.Unbox<int?>("SKUOrder"), reader.Unbox<int?>("SKUSiteID"), reader.Unbox<string>("SKUName"), reader.Unbox<string?>("SKUNumber"), reader.Unbox<decimal>("SKUPrice"), reader.Unbox<decimal?>("SKURetailPrice"), reader.Unbox<int?>("SKUDepartmentID"), reader.Unbox<int?>("SKUBrandID"), reader.Unbox<int?>("SKUManufacturerID"), reader.Unbox<int?>("SKUSupplierID"), reader.Unbox<int?>("SKUCollectionID"), reader.Unbox<int?>("SKUTaxClassID"), reader.Unbox<string?>("SKUImagePath"), reader.Unbox<string?>("SKUShortDescription"), reader.Unbox<string?>("SKUDescription"), reader.Unbox<string?>("SKUProductType"), reader.Unbox<string?>("SKUCustomData"), reader.Unbox<DateTime?>("SKUCreated"), reader.Unbox<DateTime>("SKULastModified"), reader.Unbox<Guid?>("SKUMembershipGUID"), reader.Unbox<string?>("SKUValidity"), reader.Unbox<int?>("SKUValidFor"), reader.Unbox<DateTime?>("SKUValidUntil"), reader.Unbox<int?>("SKUEproductFilesCount"), reader.Unbox<string?>("SKUBundleInventoryType"), reader.Unbox<int?>("SKUBundleItemsCount"), reader.Unbox<DateTime?>("SKUInStoreFrom"), reader.Unbox<int?>("SKUPublicStatusID"), reader.Unbox<int?>("SKUInternalStatusID"), reader.Unbox<bool>("SKUEnabled"), reader.Unbox<bool?>("SKUNeedsShipping"), reader.Unbox<double?>("SKUWeight"), reader.Unbox<double?>("SKUHeight"), reader.Unbox<double?>("SKUWidth"), reader.Unbox<double?>("SKUDepth"), reader.Unbox<string?>("SKUTrackInventory"), reader.Unbox<bool?>("SKUSellOnlyAvailable"), reader.Unbox<int?>("SKUAvailableItems"), reader.Unbox<int?>("SKUReorderAt"), reader.Unbox<int?>("SKUAvailableInDays"), reader.Unbox<int?>("SKUMinItemsInOrder"), reader.Unbox<int?>("SKUMaxItemsInOrder"), reader.Unbox<string?>("SKUConversionName"), reader.Unbox<string?>("SKUConversionValue"), reader.Unbox<int?>("SKUParentSKUID")
        );
    public static ComSkuK12 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SKUID"), reader.Unbox<Guid>("SKUGUID"), reader.Unbox<int?>("SKUOptionCategoryID"), reader.Unbox<int?>("SKUOrder"), reader.Unbox<int?>("SKUSiteID"), reader.Unbox<string>("SKUName"), reader.Unbox<string?>("SKUNumber"), reader.Unbox<decimal>("SKUPrice"), reader.Unbox<decimal?>("SKURetailPrice"), reader.Unbox<int?>("SKUDepartmentID"), reader.Unbox<int?>("SKUBrandID"), reader.Unbox<int?>("SKUManufacturerID"), reader.Unbox<int?>("SKUSupplierID"), reader.Unbox<int?>("SKUCollectionID"), reader.Unbox<int?>("SKUTaxClassID"), reader.Unbox<string?>("SKUImagePath"), reader.Unbox<string?>("SKUShortDescription"), reader.Unbox<string?>("SKUDescription"), reader.Unbox<string?>("SKUProductType"), reader.Unbox<string?>("SKUCustomData"), reader.Unbox<DateTime?>("SKUCreated"), reader.Unbox<DateTime>("SKULastModified"), reader.Unbox<Guid?>("SKUMembershipGUID"), reader.Unbox<string?>("SKUValidity"), reader.Unbox<int?>("SKUValidFor"), reader.Unbox<DateTime?>("SKUValidUntil"), reader.Unbox<int?>("SKUEproductFilesCount"), reader.Unbox<string?>("SKUBundleInventoryType"), reader.Unbox<int?>("SKUBundleItemsCount"), reader.Unbox<DateTime?>("SKUInStoreFrom"), reader.Unbox<int?>("SKUPublicStatusID"), reader.Unbox<int?>("SKUInternalStatusID"), reader.Unbox<bool>("SKUEnabled"), reader.Unbox<bool?>("SKUNeedsShipping"), reader.Unbox<double?>("SKUWeight"), reader.Unbox<double?>("SKUHeight"), reader.Unbox<double?>("SKUWidth"), reader.Unbox<double?>("SKUDepth"), reader.Unbox<string?>("SKUTrackInventory"), reader.Unbox<bool?>("SKUSellOnlyAvailable"), reader.Unbox<int?>("SKUAvailableItems"), reader.Unbox<int?>("SKUReorderAt"), reader.Unbox<int?>("SKUAvailableInDays"), reader.Unbox<int?>("SKUMinItemsInOrder"), reader.Unbox<int?>("SKUMaxItemsInOrder"), reader.Unbox<string?>("SKUConversionName"), reader.Unbox<string?>("SKUConversionValue"), reader.Unbox<int?>("SKUParentSKUID")
        );
};
public partial record ComSkuK13(int SKUID, Guid SKUGUID, int? SKUOptionCategoryID, int? SKUOrder, int? SKUSiteID, string SKUName, string? SKUNumber, decimal SKUPrice, decimal? SKURetailPrice, int? SKUDepartmentID, int? SKUBrandID, int? SKUManufacturerID, int? SKUSupplierID, int? SKUCollectionID, int? SKUTaxClassID, string? SKUImagePath, string? SKUShortDescription, string? SKUDescription, string? SKUProductType, string? SKUCustomData, DateTime? SKUCreated, DateTime SKULastModified, Guid? SKUMembershipGUID, string? SKUValidity, int? SKUValidFor, DateTime? SKUValidUntil, int? SKUEproductFilesCount, string? SKUBundleInventoryType, int? SKUBundleItemsCount, DateTime? SKUInStoreFrom, int? SKUPublicStatusID, int? SKUInternalStatusID, bool SKUEnabled, bool? SKUNeedsShipping, double? SKUWeight, double? SKUHeight, double? SKUWidth, double? SKUDepth, string? SKUTrackInventory, bool? SKUSellOnlyAvailable, int? SKUAvailableItems, int? SKUReorderAt, int? SKUAvailableInDays, int? SKUMinItemsInOrder, int? SKUMaxItemsInOrder, int? SKUParentSKUID) : IComSku, ISourceModel<ComSkuK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "SKUID";
    public static string TableName => "COM_SKU";
    public static string GuidColumnName => "SKUGUID";
    static ComSkuK13 ISourceModel<ComSkuK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SKUID"), reader.Unbox<Guid>("SKUGUID"), reader.Unbox<int?>("SKUOptionCategoryID"), reader.Unbox<int?>("SKUOrder"), reader.Unbox<int?>("SKUSiteID"), reader.Unbox<string>("SKUName"), reader.Unbox<string?>("SKUNumber"), reader.Unbox<decimal>("SKUPrice"), reader.Unbox<decimal?>("SKURetailPrice"), reader.Unbox<int?>("SKUDepartmentID"), reader.Unbox<int?>("SKUBrandID"), reader.Unbox<int?>("SKUManufacturerID"), reader.Unbox<int?>("SKUSupplierID"), reader.Unbox<int?>("SKUCollectionID"), reader.Unbox<int?>("SKUTaxClassID"), reader.Unbox<string?>("SKUImagePath"), reader.Unbox<string?>("SKUShortDescription"), reader.Unbox<string?>("SKUDescription"), reader.Unbox<string?>("SKUProductType"), reader.Unbox<string?>("SKUCustomData"), reader.Unbox<DateTime?>("SKUCreated"), reader.Unbox<DateTime>("SKULastModified"), reader.Unbox<Guid?>("SKUMembershipGUID"), reader.Unbox<string?>("SKUValidity"), reader.Unbox<int?>("SKUValidFor"), reader.Unbox<DateTime?>("SKUValidUntil"), reader.Unbox<int?>("SKUEproductFilesCount"), reader.Unbox<string?>("SKUBundleInventoryType"), reader.Unbox<int?>("SKUBundleItemsCount"), reader.Unbox<DateTime?>("SKUInStoreFrom"), reader.Unbox<int?>("SKUPublicStatusID"), reader.Unbox<int?>("SKUInternalStatusID"), reader.Unbox<bool>("SKUEnabled"), reader.Unbox<bool?>("SKUNeedsShipping"), reader.Unbox<double?>("SKUWeight"), reader.Unbox<double?>("SKUHeight"), reader.Unbox<double?>("SKUWidth"), reader.Unbox<double?>("SKUDepth"), reader.Unbox<string?>("SKUTrackInventory"), reader.Unbox<bool?>("SKUSellOnlyAvailable"), reader.Unbox<int?>("SKUAvailableItems"), reader.Unbox<int?>("SKUReorderAt"), reader.Unbox<int?>("SKUAvailableInDays"), reader.Unbox<int?>("SKUMinItemsInOrder"), reader.Unbox<int?>("SKUMaxItemsInOrder"), reader.Unbox<int?>("SKUParentSKUID")
        );
    public static ComSkuK13 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SKUID"), reader.Unbox<Guid>("SKUGUID"), reader.Unbox<int?>("SKUOptionCategoryID"), reader.Unbox<int?>("SKUOrder"), reader.Unbox<int?>("SKUSiteID"), reader.Unbox<string>("SKUName"), reader.Unbox<string?>("SKUNumber"), reader.Unbox<decimal>("SKUPrice"), reader.Unbox<decimal?>("SKURetailPrice"), reader.Unbox<int?>("SKUDepartmentID"), reader.Unbox<int?>("SKUBrandID"), reader.Unbox<int?>("SKUManufacturerID"), reader.Unbox<int?>("SKUSupplierID"), reader.Unbox<int?>("SKUCollectionID"), reader.Unbox<int?>("SKUTaxClassID"), reader.Unbox<string?>("SKUImagePath"), reader.Unbox<string?>("SKUShortDescription"), reader.Unbox<string?>("SKUDescription"), reader.Unbox<string?>("SKUProductType"), reader.Unbox<string?>("SKUCustomData"), reader.Unbox<DateTime?>("SKUCreated"), reader.Unbox<DateTime>("SKULastModified"), reader.Unbox<Guid?>("SKUMembershipGUID"), reader.Unbox<string?>("SKUValidity"), reader.Unbox<int?>("SKUValidFor"), reader.Unbox<DateTime?>("SKUValidUntil"), reader.Unbox<int?>("SKUEproductFilesCount"), reader.Unbox<string?>("SKUBundleInventoryType"), reader.Unbox<int?>("SKUBundleItemsCount"), reader.Unbox<DateTime?>("SKUInStoreFrom"), reader.Unbox<int?>("SKUPublicStatusID"), reader.Unbox<int?>("SKUInternalStatusID"), reader.Unbox<bool>("SKUEnabled"), reader.Unbox<bool?>("SKUNeedsShipping"), reader.Unbox<double?>("SKUWeight"), reader.Unbox<double?>("SKUHeight"), reader.Unbox<double?>("SKUWidth"), reader.Unbox<double?>("SKUDepth"), reader.Unbox<string?>("SKUTrackInventory"), reader.Unbox<bool?>("SKUSellOnlyAvailable"), reader.Unbox<int?>("SKUAvailableItems"), reader.Unbox<int?>("SKUReorderAt"), reader.Unbox<int?>("SKUAvailableInDays"), reader.Unbox<int?>("SKUMinItemsInOrder"), reader.Unbox<int?>("SKUMaxItemsInOrder"), reader.Unbox<int?>("SKUParentSKUID")
        );
};
