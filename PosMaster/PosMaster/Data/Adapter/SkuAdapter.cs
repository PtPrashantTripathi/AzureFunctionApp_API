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
    /// ItemDetailsAdapter class
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ItemAdapter : IItemAdapter
    {

        # region Constants
        static readonly string collection = Constant.COSMOS_ITEM_COLLECTION;
        #endregion

        #region Private Members
        ILoggerAdapter _logger;
        ICosmosDbService<dynamic> _cosmosDbService;
        #endregion

        #region Contructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cosmosDbService"></param>
        /// <param name="logger"></param>
        public ItemAdapter(ILoggerAdapter logger, ICosmosDbService<dynamic> cosmosDbService)
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
        public async Task<List<dynamic>> GetDetails(FilterCriteria filterCriteria, string token)
        {
            try
            {
                var result = await _cosmosDbService.GetDataWithoutPartitionKey(collection, CreateQuery(filterCriteria), token, filterCriteria.Pagination);
                return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# PosMaster/Sku Adadpter failed! => Unable to fetch the Sku Details . | Request# " + JsonConvert.SerializeObject(filterCriteria) + "| Response# " + ex.Message);
                throw ex;
            }
        }
        #endregion

        #region CreateQuery Methods
        private string CreateQuery(FilterCriteria filterCriteria)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT c.Sku, c.ActiveInventoryItemIndicator, c.ActiveItemInventoryPeriodNumberOfDays," +
" c.AssetIndicator, c.BarCodeId, c.CasePackItemIndicator, c.CoManagedInventoryItemIndicator," +
" c.CommodityTypeId, c.CompetitiveBidsRequiredForMaximumAmountRange," +
" c.CompetitiveBidsRequiredForMinimumAmountRange, c.ComponentPartIdentifier," +
" c.ConsignmentItemIndicator, c.CopyrightNumber, c.DepreciationMethodId, c.DescriptiveLabelText," +
" c.DocumentEventTypeId, c.DocumentId, c.EligibleForExportIndicator, c.EngineeringReferenceNumber," +
" c.EurobrandIndicator, c.EuropeanArticleNumber, c.FamilyGroupId, c.GlobalTradeItemNumber," +
" c.GradeLabelText, c.HazardousMaterialIndicator, c.HazardSeverityCategoryId, c.id," +
" c.InformationLabelText, c.InitialRetailPrice, c.InStockOutOfStockIndicator, c.ItemCodeCheckDigitId," +
" c.ItemConversionQuantity, c.ItemDescription, c.ItemElectronicPosCode, c.ItemHeight, " +
" c.ItemIdentificationMethodId, c.ItemImageActiveDate, c.ItemImageApprovalDate, c.ItemImageDefaultIdentifier," +
" c.ItemImageExpirationDate, c.ItemLength, c.ItemName, c.ItemNote, c.ItemPointOfSaleDiscountAmount," +
" c.ItemPointOfSaleDiscountPercent, c.ItemPrimaryIndicator, c.ItemSafetyClassificationId, c.ItemTypeId," +
" c.ItemUnitOfMeasureSizeCategory, c.ItemUnitOfMeasureSizeQuantity, c.ItemVolume, c.ItemVolumeUomId," +
" c.ItemWeight, c.ItemWidth, c.JdaProtFlg, c.LegacyPrimaryKeyId, c.LegacyPosCategoryId," +
" c.LifedItemIndicator, c.ListPrice, c.LotSizeQuantity, c.LwhUomId, c.MarginAmount," +
" c.MaximumStorageHumidity, c.MaximumStorageHumidityUomId, c.MaximumStorageTemperature," +
" c.MaximumStorageTemperatureUomId, c.MinimumOrderQuantity, c.MinimumStockQuantity," +
" c.MinimumStorageHumidity, c.MinimumStorageHumidityUomId, c.MinimumStorageTemperature," +
" c.MinimumStorageTemperatureUomId, c.mPosId, c.PluCode, c.PosFeatureValue," +
" c.PosItemIndicator, c.PutAwayRulesStatement, c.RepairableItemIndicator," +
" c.RepairPeriod, c.RotableItemIndicator, c.RowUpdatedTimestamp, c.SecurityLevelId," +
" c.SerializedItemIndicator, c.ShelfLifeDays, c.SkuDflFlg, c.SoleSourceItemIndicator," +
" c.SoleSourceSupplierName, c.StandardCost, c.StandardQuantityUnitOfMeasureId, c.StandardStorageUnitTypeId," +
" c.StandardWholesalePrice, c.StoreMixedIndicator, c.StoreToExistingItemIndicator, c.StoreToExistingLotIndicator," +
" c.TargetReturnPrice, c.TrackSerialNumberIndicator, c.UlControlNumber, c.UlFileNumber," +
" c.UnitOfMeasureCompareCategory, c.UnitOfMeasureName, c.UniversalPosCode, c.UPCTypeName," +
" c.VendorId, c.VerbalBidsRequiredForMaximumAmountRange, c.VerbalBidsRequiredForMinimumAmountRange," +
" c.WarningInformationText, c.WeightUomId, c.WrittenBidsRequiredForMaximumAmountRange," +
" c.WrittenBidsRequiredForMinimumAmountRange, c.CustomerFacingAttributes, c.ItemDocuments," +
" c.GoldenRecordId, c.ContainsLEDIndicator, c.SustainableCertificate, c.ThreadCount, c.NumberOfPieces," +
" c.SustainableCertifiedName, c.SustainableCertifiedDescription, c.PosSizeRangeSubTypeName," +
" c.PosSizeRangeSubTypeDescription, c.PosSizeRangeCodeName, c.PosSizeRangeCodeDescription," +
" c.Fabric, c.Footwear, c.ItemSpecification");
                query.Append(" FROM c");
                query.Append(" WHERE c.Sku <> '' ");

                if (filterCriteria.Sku != null)
                {
                    query.Append($" AND c.Sku IN ({string.Join(",", filterCriteria.Sku.Select(itm => "'" + itm + "'"))})");
                }

                if (filterCriteria.RowUpdatedStartDate > DateTime.MinValue)
                {
                    if (filterCriteria.RowUpdatedEndDate == DateTime.MinValue) //will search for only a particular date
                        filterCriteria.RowUpdatedEndDate = filterCriteria.RowUpdatedStartDate;

                    query.Append($" AND (c.RowUpdatedTimestamp BETWEEN '{filterCriteria.RowUpdatedStartDate.ToString("yyyy-MM-dd 00:00:00")}' AND '{filterCriteria.RowUpdatedEndDate.ToString("yyyy-MM-dd 23:59:59")}')");
                }

                if (filterCriteria.UPCTypeName != null)
                    query.Append($" AND c.UPCTypeName IN ({string.Join(",", filterCriteria.UPCTypeName.Select(itm => "'" + itm + "'"))})");

                query.Append(" ORDER BY c.Sku");

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
