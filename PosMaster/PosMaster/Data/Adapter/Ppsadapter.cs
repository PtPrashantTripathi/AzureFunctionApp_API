using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosMaster.Data
{
    /// <summary>
    /// PosAdapter class
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PosAdapter : IPosAdapter
    {
        # region Constants
        static readonly string itemcollection = Constant.COSMOS_ITEM_COLLECTION;
        static readonly string poscollection = Constant.COSMOS_POS_COLLECTION;
        static readonly string posgroupcollection = Constant.COSMOS_POSGROUP_COLLECTION;
        #endregion

        #region Private Members
        ILoggerAdapter _logger;
        ICosmosDbService<dynamic> _cosmosDbService;
        #endregion

        #region Contructors
        /// <summary>
        /// PosAdapter constructor
        /// </summary>
        /// <param name="cosmosDbService"></param>
        /// <param name="logger"></param>
        public PosAdapter(ILoggerAdapter logger, ICosmosDbService<dynamic> cosmosDbService)
        {
            _logger = logger;
            _cosmosDbService = cosmosDbService;
        }
        #endregion

        #region Method Implementation
        /// <summary>
        /// Fetches Pos details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetPos(FilterCriteria filterCriteria, string token)
        {
            try
            {
                List<dynamic> ItemResult = null;
                List<dynamic> posGroupResult = null;

                if (filterCriteria.Purpose.Trim().ToLower() == "all")
                {
                    if (filterCriteria.mRelatedPosGroupId != null)
                    {
                        posGroupResult = await _cosmosDbService.GetDataWithoutPartitionKey(posgroupcollection, CreatePosGroupQuery(filterCriteria, ItemResult), token, filterCriteria.Pagination);

                        if (posGroupResult != null && posGroupResult.Count > 0)
                            ItemResult = await _cosmosDbService.GetDataWithoutPartitionKey(itemcollection, CreateItemQuery(filterCriteria, posGroupResult), null, filterCriteria.Pagination);
                    }
                    else
                    {
                        ItemResult = await _cosmosDbService.GetDataWithoutPartitionKey(itemcollection, CreateItemQuery(filterCriteria, posGroupResult), token, filterCriteria.Pagination);

                        if (ItemResult != null && ItemResult.Count > 0)
                            posGroupResult = await _cosmosDbService.GetDataWithoutPartitionKey(posgroupcollection, CreatePosGroupQuery(filterCriteria, ItemResult), null, filterCriteria.Pagination);
                    }

                    if (ItemResult != null && ItemResult.Count > 0)
                    {
                        //merging PosGroup and Sku together..
                        if (posGroupResult != null && posGroupResult.Count > 0)
                        {
                            for (int i = 0; i < ItemResult.Count; i++)
                            {
                                foreach (var prodgroup in posGroupResult)
                                {
                                    string mprodgroupid = prodgroup.mPosGroupId;
                                    if (ItemResult[i].mPosId.ToString() == mprodgroupid)
                                    {
                                        ItemResult[i].Merge(prodgroup);
                                        break;
                                    }
                                }
                            }
                        }

                        var PosResult = await _cosmosDbService.GetDataWithoutPartitionKey(poscollection, CreateQuery(filterCriteria, ItemResult), null, filterCriteria.Pagination);

                        if (PosResult.Count > 0)
                        {
                            //joining operation between PosResult & ItemResult
                            for (int i = 0; i < ItemResult.Count; i++)
                            {
                                foreach (var prod in PosResult)
                                {
                                    string improd = prod.mPosId;
                                    if (ItemResult[i].mPosId.ToString() == improd)
                                    {
                                        ItemResult[i].Merge(prod);
                                        break;
                                    }
                                }
                            }
                            return ItemResult;
                        }
                        else
                            return PosResult; //an empty result
                    }
                    else
                        return ItemResult; //an empty result
                }
                else
                {
                    var result = await _cosmosDbService.GetDataWithoutPartitionKey(poscollection, CreateQuery(filterCriteria, null), token, filterCriteria.Pagination);
                    return result;
                }
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# PosMaster/Pos Adapter failed! => Unable to fetch the PosDetails . | Request# " + JsonConvert.SerializeObject(filterCriteria) + "| Response# " + ex.Message);
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
                query.Append("SELECT c.mposId AS mPosId,c.AdditionalOrderLimitQuantity,c.AdvertisingPriceMinimumAmount,c.AllocationAggregateRoundingName,c.BuyerId,c.BuyerNoteText,c.BuyerStartingRetailPriceAmount," +
                "c.CigarettePromotionDescription,c.CouponFamilyCodeId,c.CouponAlternateFamilyCodeId,c.DrugStatusTypeID,c.FlavorName,c.HazardTypeId,c.id,c.InitialBuyQuantity,c.InventoryDecimalPrecisionQuantity," +
                    "c.LegacyPrimaryKeyId,c.LegacyPosCategoryId,c.ManufacturerItemId,c.ManufacturerPrePriceAmount,c.MaximumStorageTemperature,c.mBrandEquivalentId,c.MinimumStorageTemperature,c.mPosGroupLevel2Id," +
                    "c.NonScanReasonCodeTypeName,c.NualScore,c.OrderbookSequenceId,c.OrderPosId,c.PharmaceuticalIngredientId,c.PharmacyCentralFillOrderLimitQuantity,c.PharmacyDrugStrengthUnitOfMeasureDescription," +
                    "c.PharmacyPosTotalStrengthUnitOfMeasureDescription,c.PlannedAbandonmentDate,c.PosAddOnProgramId1,c.PosAddOnProgramId2,c.PosContainerEachQuantity,c.PosEstimatedDeliveryCostAmount," +
                    "c.PosEthyleneSensitiveName,c.PosExpectedFirstDeliveryDate,c.PosExpectedLossQuantity,c.PosFirstOrderDate,c.PosFirstShipDate,c.PosGroupLevel2Id,c.VendorId,c.VendorNote,c.PosId," +
                    "c.PosInventoryDecimalPrecisionQuantity,c.PosLabelPriceTypeName,c.PosNM,c.PosPsuedophedarineInGramsQuantity,c.PosSignDescription,c.PosSizeDescription,c.PosSizeUnitOfMeasure," +
                    "c.PosStockTypeName,c.PosTargetGrossMarginPercent,c.PosUnitOfMeasureConversionQuantity,c.PosUnitOfMeasureSizeQuantity,c.PosVariableWeightId,c.PublicationPerYearQuantity," +
                    "c.RandomWeightCaseCountQuantity,c.ReportCodeName,c.Retaupc_locopyfromPosId,c.ReturnDispositionName,c.RowUpdateTimestamp,c.runid,c.ScentName,c.SeasonalUsageName,c.SportsTeamName,c.SuggestedRetailPrice," +
                    "c.TargetName,c.TaxCodeContainerCategory,c.TrademarkP,c.UnitOfMeasureName,c.Color,c.ColorType,c.Display,c.DisplayType,c.Event,c.EventType,c.Features,c.Flags,c.Flavor,c.FlavorType," +
                    "c.FootwearPosHeelType,c.FootwearPosStyle,c.JewelryPosMaterialCategory,c.JewelryPosMaterialType,c.LocationType,c.NonScanReasonCodeType,c.Package,c.PackageType,c.PackagingType," +
                    "c.PGMorRole,c.PosCharacteristicType,c.PosCustomizedCharacteristic,c.PosCustomizedFeatures,c.PosDescriptionType,c.PosDisplay,c.PosFeatureType,c.PosName,c.PosShrinkage," +
                    "c.Regulation,c.RegulationCategory,c.RelatedBrand,c.ReportCode,c.PharmaceuticalIngredient,c.IngredientType,c.ReturnDisposition,c.ReturnShipmentType,c.Scent,c.ScentType,c.SeasonalUsage,c.SeasonalUsageType," +
                    "c.ShrinkageType,c.SportsTeam,c.SportsTeamType,c.StorageCondition,c.StoreType,c.StockKeepingUnitCategory,c.Threshold,c.ThresholdType,c.Trademark,c.TrademarkType,c.VendorStatusType,");

                if (filterCriteria.BrandCategoryId != null)
                    query.Append($" ARRAY(SELECT VALUE t FROM t in c.Brand where t.BrandCategoryId = '{filterCriteria.BrandCategoryId}' ) AS Brand, ");
                else
                    query.Append("c.Brand,");

                if (filterCriteria.PosStatusTypeName != null)
                    query.Append($" ARRAY(SELECT VALUE t FROM t in c.PosStatus where t.PosStatusTypeName = '{filterCriteria.PosStatusTypeName}' ) AS PosStatus,");
                else
                    query.Append("c.PosStatus,");

                query.Append("c.VendorType FROM c");

                if (filterCriteria.BrandCategoryId != null)
                    query.Append($" JOIN br in c.Brand ");
                if (filterCriteria.PosStatusTypeName != null)
                    query.Append($" JOIN ps in c.PosStatus ");

                query.Append(" WHERE");
                if (ItemResult != null)
                    query.Append($" c.mposId IN ({string.Join(",", ItemResult.Select(itm => "'" + itm.mPosId + "'"))})");
                else
                {
                    if (filterCriteria.mPosId != null)
                        query.Append($" c.mposId IN({string.Join(",", filterCriteria.mPosId.Select(itm => "'" + itm + "'"))})");
                    if (filterCriteria.LegacyPrimaryKeyId > 0)
                        query.Append($" c.LegacyPrimaryKeyId  = {filterCriteria.LegacyPrimaryKeyId}");
                }

                if (filterCriteria.BrandCategoryId != null)
                    query.Append($" AND br.BrandCategoryId = '{filterCriteria.BrandCategoryId}'");
                if (filterCriteria.PosStatusTypeName != null)
                    query.Append($" AND ps.PosStatusTypeName = '{filterCriteria.PosStatusTypeName}'");
                if (filterCriteria.VendorId != 0)
                    query.Append($" AND c.VendorId  = {filterCriteria.VendorId}");

                if (filterCriteria.RowUpdatedStartDate > DateTime.MinValue)
                {
                    if (filterCriteria.RowUpdatedEndDate == DateTime.MinValue) //will search for only a particular date
                        filterCriteria.RowUpdatedEndDate = filterCriteria.RowUpdatedStartDate;
                    query.Append($" AND(c.RowUpdateTimestamp BETWEEN '{filterCriteria.RowUpdatedStartDate:yyyy-MM-ddT00:00:00}' AND '{filterCriteria.RowUpdatedEndDate:yyyy-MM-ddT23:59:59}')");
                }

                query.Append(" ORDER BY c.mposId");

                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region CreateItemQuery Methods
        private string CreateItemQuery(FilterCriteria filterCriteria, List<dynamic> posGroupResult)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT c.Sku,c.ActiveInventoryItemIndicator,c.ActiveItemInventoryPeriodNumberOfDays,c.AssetIndicator,c.BarCodeId,c.CasePackItemIndicator," +
                    "c.CoManagedInventoryItemIndicator,c.CommodityTypeId,c.CompetitiveBidsRequiredForMaximumAmountRange,c.CompetitiveBidsRequiredForMinimumAmountRange," +
                    "c.ComponentPartIdentifier,c.ConsignmentItemIndicator,c.CopyrightNumber,c.DepreciationMethodId,c.DescriptiveLabelText," +
                    "c.DocumentEventTypeId,c.DocumentId," +
                    "c.EligibleForExportIndicator,c.EngineeringReferenceNumber,c.EurobrandIndicator,c.EuropeanArticleNumber,c.FamilyGroupId,c.GlobalTradeItemNumber,c.GradeLabelText," +
                    "c.HazardousMaterialIndicator,c.HazardSeverityCategoryId,c.id,c.InformationLabelText,c.InitialRetailPrice,c.InStockOutOfStockIndicator,c.ItemCodeCheckDigitId," +
                    "c.ItemConversionQuantity,c.ItemDescription,c.ItemElectronicPosCode,c.ItemHeight,c.ItemIdentificationMethodId,c.ItemImageActiveDate,c.ItemImageApprovalDate," +
                    "c.ItemImageDefaultIdentifier,c.ItemImageExpirationDate,c.ItemLength,c.ItemName,c.ItemNote,c.ItemPointOfSaleDiscountAmount,c.ItemPointOfSaleDiscountPercent," +
                    "c.ItemPrimaryIndicator,c.ItemSafetyClassificationId,c.ItemTypeId,c.ItemUnitOfMeasureSizeCategory,c.ItemUnitOfMeasureSizeQuantity,c.ItemVolume,c.ItemVolumeUomId," +
                    "c.ItemWeight,c.ItemWidth,c.JdaProtFlg,c.LegacyPrimaryKeyId,c.LegacyPosCategoryId,c.LifedItemIndicator,c.ListPrice,c.LotSizeQuantity,c.LwhUomId,c.MarginAmount," +
                    "c.MaximumStorageHumidity,c.MaximumStorageHumidityUomId,c.MaximumStorageTemperature,c.MaximumStorageTemperatureUomId,c.MinimumOrderQuantity,c.MinimumStockQuantity," +
                    "c.MinimumStorageHumidity,c.MinimumStorageHumidityUomId,c.MinimumStorageTemperature,c.MinimumStorageTemperatureUomId,c.mPosId,c.PluCode,c.PosFeatureValue," +
                    "c.PosItemIndicator,c.PutAwayRulesStatement,c.RepairableItemIndicator,c.RepairPeriod,c.RotableItemIndicator,c.RowUpdatedTimestamp,c.SecurityLevelId," +
                    "c.SerializedItemIndicator,c.ShelfLifeDays,c.SkuDflFlg,c.SoleSourceItemIndicator,c.SoleSourceSupplierName,c.StandardCost,c.StandardQuantityUnitOfMeasureId," +
                    "c.StandardStorageUnitTypeId,c.StandardWholesalePrice,c.StoreMixedIndicator,c.StoreToExistingItemIndicator,c.StoreToExistingLotIndicator,c.TargetReturnPrice," +
                    "c.TrackSerialNumberIndicator,c.UlControlNumber,c.UlFileNumber,c.UnitOfMeasureCompareCategory,c.UnitOfMeasureName,c.UniversalPosCode,c.UPCTypeName,c.VendorId," +
                    "c.VerbalBidsRequiredForMaximumAmountRange,c.VerbalBidsRequiredForMinimumAmountRange,c.WarningInformationText,c.WeightUomId,c.WrittenBidsRequiredForMaximumAmountRange," +
                    "c.WrittenBidsRequiredForMinimumAmountRange,c.CustomerFacingAttributes,c.ItemDocuments," +
                    "c.GoldenRecordId, c.ContainsLEDIndicator, c.SustainableCertificate, c.ThreadCount, c.NumberOfPieces," +
                    "c.SustainableCertifiedName, c.SustainableCertifiedDescription, c.PosSizeRangeSubTypeName," +
                    "c.PosSizeRangeSubTypeDescription, c.PosSizeRangeCodeName, c.PosSizeRangeCodeDescription, c.Fabric, c.Footwear, c.ItemSpecification");
                query.Append(" FROM c WHERE");

                //if mRelatedPosGroupId is inputted
                if (posGroupResult != null)
                    query.Append($" c.mPosId IN ({string.Join(",", posGroupResult.Select(itm => itm.mPosGroupId))})");

                //if LegacyPrimaryKeyId is inputted
                if (filterCriteria.LegacyPrimaryKeyId != 0)
                    query.Append($" c.LegacyPrimaryKeyId = {filterCriteria.LegacyPrimaryKeyId}");

                //if mPosId is inputted
                if (filterCriteria.mPosId != null)
                    query.Append($" c.mPosId IN ({string.Join(",", filterCriteria.mPosId.Select(itm => itm))})");

                //if Sku is inputted
                if (filterCriteria.Sku != null)
                {
                    query.Append($" c.Sku IN ({string.Join(",", filterCriteria.Sku.Select(itm => "'" + itm + "'"))})");

                    if (filterCriteria.RowUpdatedStartDate > DateTime.MinValue)
                    {
                        if (filterCriteria.RowUpdatedEndDate == DateTime.MinValue) //will search for only a particular date
                            filterCriteria.RowUpdatedEndDate = filterCriteria.RowUpdatedStartDate;

                        query.Append($" AND (c.RowUpdatedTimestamp BETWEEN '{filterCriteria.RowUpdatedStartDate.ToString("yyyy-MM-dd 00:00:00")}' AND '{filterCriteria.RowUpdatedEndDate.ToString("yyyy-MM-dd 23:59:59")}')");
                    }
                }

                if (filterCriteria.UPCTypeName != null)
                    query.Append($" AND c.UPCTypeName IN ({string.Join(",", filterCriteria.UPCTypeName.Select(itm => "'" + itm + "'"))})");

                query.Append(" ORDER BY c.mPosId");

                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CreatePosGroupQuery Methods
        private string CreatePosGroupQuery(FilterCriteria filterCriteria, List<dynamic> ItemResult)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT c.mPosGroupId,");
                query.Append(" ARRAY(SELECT t.mRelatedPosGroupId, t.PosGroupName from t in c.ParentLevel_2) ParentLevel_2,");
                query.Append(" ARRAY(SELECT t.mRelatedPosGroupId, t.PosGroupName from t in c.ParentLevel_3) ParentLevel_3,");
                query.Append(" ARRAY(SELECT t.mRelatedPosGroupId, t.PosGroupName from t in c.ParentLevel_4) ParentLevel_4,");
                query.Append(" ARRAY(SELECT t.mRelatedPosGroupId, t.PosGroupName from t in c.ParentLevel_5) ParentLevel_5,");
                query.Append(" ARRAY(SELECT t.mRelatedPosGroupId, t.PosGroupName from t in c.ParentLevel_6) ParentLevel_6,");
                query.Append(" c.MPRSHierarchy[0].LegacyPrimaryKeyId, c.MPRSHierarchy[0].LegacyPrimaryKeyText, c.MPRSHierarchy[0].LegacyMPRSCategoryId, c.MPRSHierarchy[0].LegacyMPRSCategoryText,");
                query.Append(" c.PosCategory[0].LegacyPosCategoryId, c.PosCategory[0].LegacyPosCategoryText");
                query.Append(" FROM c WHERE");

                if (ItemResult != null)
                {
                    query.Append($" c.mPosGroupId IN ({string.Join(",", ItemResult.Select(itm => "'" + itm.mPosId + "'"))})");
                }
                else
                {
                    query.Append($" (EXISTS (SELECT VALUE t FROM t in c.ParentLevel_{filterCriteria.mRelatedPosGroupId[0][1]} WHERE");
                    query.Append($" t.mRelatedPosGroupId = '{filterCriteria.mRelatedPosGroupId[0]}'))");
                }

                query.Append(" ORDER BY c.mPosGroupId");

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

