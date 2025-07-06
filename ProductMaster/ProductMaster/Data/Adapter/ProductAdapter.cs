using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductMaster.Data
{
    /// <summary>
    /// ProductAdapter class
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductAdapter : IProductAdapter
    {
        # region Constants
        static readonly string itemcollection = Constant.COSMOS_ITEM_COLLECTION;
        static readonly string productcollection = Constant.COSMOS_PRODUCT_COLLECTION;
        static readonly string productgroupcollection = Constant.COSMOS_PRODUCTGROUP_COLLECTION;
        #endregion

        #region Private Members
        ILoggerAdapter _logger;
        ICosmosDbService<dynamic> _cosmosDbService;
        #endregion

        #region Contructors
        /// <summary>
        /// ProductAdapter constructor
        /// </summary>
        /// <param name="cosmosDbService"></param>
        /// <param name="logger"></param>
        public ProductAdapter(ILoggerAdapter logger, ICosmosDbService<dynamic> cosmosDbService)
        {
            _logger = logger;
            _cosmosDbService = cosmosDbService;
        }
        #endregion

        #region Method Implementation
        /// <summary>
        /// Fetches Product details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetProduct(FilterCriteria filterCriteria, string token)
        {
            try
            {
                List<dynamic> ItemResult = null;
                List<dynamic> productGroupResult = null;

                if (filterCriteria.Purpose.Trim().ToLower() == "all")
                {
                    if (filterCriteria.mRelatedProductGroupId != null)
                    {
                        productGroupResult = await _cosmosDbService.GetDataWithoutPartitionKey(productgroupcollection, CreateProductGroupQuery(filterCriteria, ItemResult), token, filterCriteria.Pagination);

                        if (productGroupResult != null && productGroupResult.Count > 0)
                            ItemResult = await _cosmosDbService.GetDataWithoutPartitionKey(itemcollection, CreateItemQuery(filterCriteria, productGroupResult), null, filterCriteria.Pagination);
                    }
                    else
                    {
                        ItemResult = await _cosmosDbService.GetDataWithoutPartitionKey(itemcollection, CreateItemQuery(filterCriteria, productGroupResult), token, filterCriteria.Pagination);

                        if (ItemResult != null && ItemResult.Count > 0)
                            productGroupResult = await _cosmosDbService.GetDataWithoutPartitionKey(productgroupcollection, CreateProductGroupQuery(filterCriteria, ItemResult), null, filterCriteria.Pagination);
                    }

                    if (ItemResult != null && ItemResult.Count > 0)
                    {
                        //merging ProductGroup and Item together..
                        if (productGroupResult != null && productGroupResult.Count > 0)
                        {
                            for (int i = 0; i < ItemResult.Count; i++)
                            {
                                foreach (var prodgroup in productGroupResult)
                                {
                                    string mprodgroupid = prodgroup.mProductGroupId;
                                    if (ItemResult[i].mProductId.ToString() == mprodgroupid)
                                    {
                                        ItemResult[i].Merge(prodgroup);
                                        break;
                                    }
                                }
                            }
                        }

                        var ProductResult = await _cosmosDbService.GetDataWithoutPartitionKey(productcollection, CreateQuery(filterCriteria, ItemResult), null, filterCriteria.Pagination);

                        if (ProductResult.Count > 0)
                        {
                            //joining operation between ProductResult & ItemResult
                            for (int i = 0; i < ItemResult.Count; i++)
                            {
                                foreach (var prod in ProductResult)
                                {
                                    string improd = prod.mProductId;
                                    if (ItemResult[i].mProductId.ToString() == improd)
                                    {
                                        ItemResult[i].Merge(prod);
                                        break;
                                    }
                                }
                            }
                            return ItemResult;
                        }
                        else
                            return ProductResult; //an empty result
                    }
                    else
                        return ItemResult; //an empty result
                }
                else
                {
                    var result = await _cosmosDbService.GetDataWithoutPartitionKey(productcollection, CreateQuery(filterCriteria, null), token, filterCriteria.Pagination);
                    return result;
                }
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# ProductMaster/Product Adapter failed! => Unable to fetch the ProductDetails . | Request# " + JsonConvert.SerializeObject(filterCriteria) + "| Response# " + ex.Message);
                throw ex;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Creates query for cosmos
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="ItemResult"></param>
        /// <returns></returns>
        private string CreateQuery(FilterCriteria filterCriteria, List<dynamic> ItemResult)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT c.mproductId AS mProductId,c.AdditionalOrderLimitQuantity,c.AdvertisingPriceMinimumAmount,c.AllocationAggregateRoundingName,c.BuyerId,c.BuyerNoteText,c.BuyerStartingRetailPriceAmount," +
                "c.CigarettePromotionDescription,c.CouponFamilyCodeId,c.CouponAlternateFamilyCodeId,c.DrugStatusTypeID,c.FlavorName,c.HazardTypeId,c.id,c.InitialBuyQuantity,c.InventoryDecimalPrecisionQuantity," +
                    "c.LegacyPrimaryKeyId,c.LegacyProductCategoryId,c.ManufacturerItemId,c.ManufacturerPrePriceAmount,c.MaximumStorageTemperature,c.mBrandEquivalentId,c.MinimumStorageTemperature,c.mProductGroupLevel2Id," +
                    "c.NonScanReasonCodeTypeName,c.NualScore,c.OrderbookSequenceId,c.OrderProductId,c.PharmaceuticalIngredientId,c.PharmacyCentralFillOrderLimitQuantity,c.PharmacyDrugStrengthUnitOfMeasureDescription," +
                    "c.PharmacyProductTotalStrengthUnitOfMeasureDescription,c.PlannedAbandonmentDate,c.ProductAddOnProgramId1,c.ProductAddOnProgramId2,c.ProductContainerEachQuantity,c.ProductEstimatedDeliveryCostAmount," +
                    "c.ProductEthyleneSensitiveName,c.ProductExpectedFirstDeliveryDate,c.ProductExpectedLossQuantity,c.ProductFirstOrderDate,c.ProductFirstShipDate,c.ProductGroupLevel2Id,c.VendorId,c.VendorNote,c.ProductId," +
                    "c.ProductInventoryDecimalPrecisionQuantity,c.ProductLabelPriceTypeName,c.ProductNM,c.ProductPsuedophedarineInGramsQuantity,c.ProductSignDescription,c.ProductSizeDescription,c.ProductSizeUnitOfMeasure," +
                    "c.ProductStockTypeName,c.ProductTargetGrossMarginPercent,c.ProductUnitOfMeasureConversionQuantity,c.ProductUnitOfMeasureSizeQuantity,c.ProductVariableWeightId,c.PublicationPerYearQuantity," +
                    "c.RandomWeightCaseCountQuantity,c.ReportCodeName,c.RetailCopyfromProductId,c.ReturnDispositionName,c.RowUpdateTimestamp,c.runid,c.ScentName,c.SeasonalUsageName,c.SportsTeamName,c.SuggestedRetailPrice," +
                    "c.TargetName,c.TaxCodeContainerCategory,c.TrademarkP,c.UnitOfMeasureName,c.Color,c.ColorType,c.Display,c.DisplayType,c.Event,c.EventType,c.Features,c.Flags,c.Flavor,c.FlavorType," +
                    "c.FootwearProductHeelType,c.FootwearProductStyle,c.JewelryProductMaterialCategory,c.JewelryProductMaterialType,c.LocationType,c.NonScanReasonCodeType,c.Package,c.PackageType,c.PackagingType," +
                    "c.PGMorRole,c.ProductCharacteristicType,c.ProductCustomizedCharacteristic,c.ProductCustomizedFeatures,c.ProductDescriptionType,c.ProductDisplay,c.ProductFeatureType,c.ProductName,c.ProductShrinkage," +
                    "c.Regulation,c.RegulationCategory,c.RelatedBrand,c.ReportCode,c.PharmaceuticalIngredient,c.IngredientType,c.ReturnDisposition,c.ReturnShipmentType,c.Scent,c.ScentType,c.SeasonalUsage,c.SeasonalUsageType," +
                    "c.ShrinkageType,c.SportsTeam,c.SportsTeamType,c.StorageCondition,c.StoreType,c.StockKeepingUnitCategory,c.Threshold,c.ThresholdType,c.Trademark,c.TrademarkType,c.VendorStatusType,");

                if (filterCriteria.BrandCategoryId != null)
                    query.Append($" ARRAY(SELECT VALUE t FROM t in c.Brand where t.BrandCategoryId = '{filterCriteria.BrandCategoryId}' ) AS Brand, ");
                else
                    query.Append("c.Brand,");

                if (filterCriteria.ProductStatusTypeName != null)
                    query.Append($" ARRAY(SELECT VALUE t FROM t in c.ProductStatus where t.ProductStatusTypeName = '{filterCriteria.ProductStatusTypeName}' ) AS ProductStatus,");
                else
                    query.Append("c.ProductStatus,");

                query.Append("c.VendorType FROM c");

                if (filterCriteria.BrandCategoryId != null)
                    query.Append($" JOIN br in c.Brand ");
                if (filterCriteria.ProductStatusTypeName != null)
                    query.Append($" JOIN ps in c.ProductStatus ");

                query.Append(" WHERE");
                if (ItemResult != null)
                    query.Append($" c.mproductId IN ({string.Join(",", ItemResult.Select(itm => "'" + itm.mProductId + "'"))})");
                else
                {
                    if (filterCriteria.mProductId != null)
                        query.Append($" c.mproductId IN({string.Join(",", filterCriteria.mProductId.Select(itm => "'" + itm + "'"))})");
                    if (filterCriteria.LegacyPrimaryKeyId > 0)
                        query.Append($" c.LegacyPrimaryKeyId  = {filterCriteria.LegacyPrimaryKeyId}");
                }

                if (filterCriteria.BrandCategoryId != null)
                    query.Append($" AND br.BrandCategoryId = '{filterCriteria.BrandCategoryId}'");
                if (filterCriteria.ProductStatusTypeName != null)
                    query.Append($" AND ps.ProductStatusTypeName = '{filterCriteria.ProductStatusTypeName}'");
                if (filterCriteria.VendorId != 0)
                    query.Append($" AND c.VendorId  = {filterCriteria.VendorId}");

                if (filterCriteria.RowUpdatedStartDate > DateTime.MinValue)
                {
                    if (filterCriteria.RowUpdatedEndDate == DateTime.MinValue) //will search for only a particular date
                        filterCriteria.RowUpdatedEndDate = filterCriteria.RowUpdatedStartDate;
                    query.Append($" AND(c.RowUpdateTimestamp BETWEEN '{filterCriteria.RowUpdatedStartDate:yyyy-MM-ddT00:00:00}' AND '{filterCriteria.RowUpdatedEndDate:yyyy-MM-ddT23:59:59}')");
                }

                query.Append(" ORDER BY c.mproductId");

                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region CreateItemQuery Methods
        private string CreateItemQuery(FilterCriteria filterCriteria, List<dynamic> productGroupResult)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT c.ItemSku,c.ActiveInventoryItemIndicator,c.ActiveItemInventoryPeriodNumberOfDays,c.AssetIndicator,c.BarCodeId,c.CasePackItemIndicator," +
                    "c.CoManagedInventoryItemIndicator,c.CommodityTypeId,c.CompetitiveBidsRequiredForMaximumAmountRange,c.CompetitiveBidsRequiredForMinimumAmountRange," +
                    "c.ComponentPartIdentifier,c.ConsignmentItemIndicator,c.CopyrightNumber,c.DepreciationMethodId,c.DescriptiveLabelText," +
                    "c.DocumentEventTypeId,c.DocumentId," +
                    "c.EligibleForExportIndicator,c.EngineeringReferenceNumber,c.EurobrandIndicator,c.EuropeanArticleNumber,c.FamilyGroupId,c.GlobalTradeItemNumber,c.GradeLabelText," +
                    "c.HazardousMaterialIndicator,c.HazardSeverityCategoryId,c.id,c.InformationLabelText,c.InitialRetailPrice,c.InStockOutOfStockIndicator,c.ItemCodeCheckDigitId," +
                    "c.ItemConversionQuantity,c.ItemDescription,c.ItemElectronicProductCode,c.ItemHeight,c.ItemIdentificationMethodId,c.ItemImageActiveDate,c.ItemImageApprovalDate," +
                    "c.ItemImageDefaultIdentifier,c.ItemImageExpirationDate,c.ItemLength,c.ItemName,c.ItemNote,c.ItemPointOfSaleDiscountAmount,c.ItemPointOfSaleDiscountPercent," +
                    "c.ItemPrimaryIndicator,c.ItemSafetyClassificationId,c.ItemTypeId,c.ItemUnitOfMeasureSizeCategory,c.ItemUnitOfMeasureSizeQuantity,c.ItemVolume,c.ItemVolumeUomId," +
                    "c.ItemWeight,c.ItemWidth,c.JdaProtFlg,c.LegacyPrimaryKeyId,c.LegacyProductCategoryId,c.LifedItemIndicator,c.ListPrice,c.LotSizeQuantity,c.LwhUomId,c.MarginAmount," +
                    "c.MaximumStorageHumidity,c.MaximumStorageHumidityUomId,c.MaximumStorageTemperature,c.MaximumStorageTemperatureUomId,c.MinimumOrderQuantity,c.MinimumStockQuantity," +
                    "c.MinimumStorageHumidity,c.MinimumStorageHumidityUomId,c.MinimumStorageTemperature,c.MinimumStorageTemperatureUomId,c.mProductId,c.PluCode,c.ProductFeatureValue," +
                    "c.ProductItemIndicator,c.PutAwayRulesStatement,c.RepairableItemIndicator,c.RepairPeriod,c.RotableItemIndicator,c.RowUpdatedTimestamp,c.SecurityLevelId," +
                    "c.SerializedItemIndicator,c.ShelfLifeDays,c.SkuDflFlg,c.SoleSourceItemIndicator,c.SoleSourceSupplierName,c.StandardCost,c.StandardQuantityUnitOfMeasureId," +
                    "c.StandardStorageUnitTypeId,c.StandardWholesalePrice,c.StoreMixedIndicator,c.StoreToExistingItemIndicator,c.StoreToExistingLotIndicator,c.TargetReturnPrice," +
                    "c.TrackSerialNumberIndicator,c.UlControlNumber,c.UlFileNumber,c.UnitOfMeasureCompareCategory,c.UnitOfMeasureName,c.UniversalProductCode,c.UPCTypeName,c.VendorId," +
                    "c.VerbalBidsRequiredForMaximumAmountRange,c.VerbalBidsRequiredForMinimumAmountRange,c.WarningInformationText,c.WeightUomId,c.WrittenBidsRequiredForMaximumAmountRange," +
                    "c.WrittenBidsRequiredForMinimumAmountRange,c.CustomerFacingAttributes,c.ItemDocuments," +
                    "c.GoldenRecordId, c.ContainsLEDIndicator, c.SustainableCertificate, c.ThreadCount, c.NumberOfPieces," +
                    "c.SustainableCertifiedName, c.SustainableCertifiedDescription, c.ProductSizeRangeSubTypeName," +
                    "c.ProductSizeRangeSubTypeDescription, c.ProductSizeRangeCodeName, c.ProductSizeRangeCodeDescription, c.Fabric, c.Footwear, c.ItemSpecification");
                query.Append(" FROM c WHERE");

                //if mRelatedProductGroupId is inputted
                if (productGroupResult != null)
                    query.Append($" c.mProductId IN ({string.Join(",", productGroupResult.Select(itm => itm.mProductGroupId))})");

                //if LegacyPrimaryKeyId is inputted
                if (filterCriteria.LegacyPrimaryKeyId != 0)
                    query.Append($" c.LegacyPrimaryKeyId = {filterCriteria.LegacyPrimaryKeyId}");

                //if mProductId is inputted
                if (filterCriteria.mProductId != null)
                    query.Append($" c.mProductId IN ({string.Join(",", filterCriteria.mProductId.Select(itm => itm))})");

                //if ItemSku is inputted
                if (filterCriteria.ItemSku != null)
                {
                    query.Append($" c.ItemSku IN ({string.Join(",", filterCriteria.ItemSku.Select(itm => "'" + itm + "'"))})");

                    if (filterCriteria.RowUpdatedStartDate > DateTime.MinValue)
                    {
                        if (filterCriteria.RowUpdatedEndDate == DateTime.MinValue) //will search for only a particular date
                            filterCriteria.RowUpdatedEndDate = filterCriteria.RowUpdatedStartDate;

                        query.Append($" AND (c.RowUpdatedTimestamp BETWEEN '{filterCriteria.RowUpdatedStartDate.ToString("yyyy-MM-dd 00:00:00")}' AND '{filterCriteria.RowUpdatedEndDate.ToString("yyyy-MM-dd 23:59:59")}')");
                    }
                }

                if (filterCriteria.UPCTypeName != null)
                    query.Append($" AND c.UPCTypeName IN ({string.Join(",", filterCriteria.UPCTypeName.Select(itm => "'" + itm + "'"))})");

                query.Append(" ORDER BY c.mProductId");

                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CreateProductGroupQuery Methods
        private string CreateProductGroupQuery(FilterCriteria filterCriteria, List<dynamic> ItemResult)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT c.mProductGroupId,");
                query.Append(" ARRAY(SELECT t.mRelatedProductGroupId, t.ProductGroupName from t in c.ParentLevel_2) ParentLevel_2,");
                query.Append(" ARRAY(SELECT t.mRelatedProductGroupId, t.ProductGroupName from t in c.ParentLevel_3) ParentLevel_3,");
                query.Append(" ARRAY(SELECT t.mRelatedProductGroupId, t.ProductGroupName from t in c.ParentLevel_4) ParentLevel_4,");
                query.Append(" ARRAY(SELECT t.mRelatedProductGroupId, t.ProductGroupName from t in c.ParentLevel_5) ParentLevel_5,");
                query.Append(" ARRAY(SELECT t.mRelatedProductGroupId, t.ProductGroupName from t in c.ParentLevel_6) ParentLevel_6,");
                query.Append(" c.MPRSHierarchy[0].LegacyPrimaryKeyId, c.MPRSHierarchy[0].LegacyPrimaryKeyText, c.MPRSHierarchy[0].LegacyMPRSCategoryId, c.MPRSHierarchy[0].LegacyMPRSCategoryText,");
                query.Append(" c.ProductCategory[0].LegacyProductCategoryId, c.ProductCategory[0].LegacyProductCategoryText");
                query.Append(" FROM c WHERE");

                if (ItemResult != null)
                {
                    query.Append($" c.mProductGroupId IN ({string.Join(",", ItemResult.Select(itm => "'" + itm.mProductId + "'"))})");
                }
                else
                {
                    query.Append($" (EXISTS (SELECT VALUE t FROM t in c.ParentLevel_{filterCriteria.mRelatedProductGroupId[0][1]} WHERE");
                    query.Append($" t.mRelatedProductGroupId = '{filterCriteria.mRelatedProductGroupId[0]}'))");
                }

                query.Append(" ORDER BY c.mProductGroupId");

                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
