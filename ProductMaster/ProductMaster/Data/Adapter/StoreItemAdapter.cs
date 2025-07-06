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
    /// StoreItem Adapter class
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class StoreItemAdapter : IStoreItemAdapter
    {
        #region Constants
        static string collection = Constant.COSMOS_STOREITEM_COLLECTION;
        #endregion

        #region Private Members
        ILoggerAdapter _logger;
        ICosmosDbService<dynamic> _cosmosDbService;
        #endregion

        /// <summary>
        /// Consytructor
        /// </summary>
        /// <param name="cosmosDbService"></param>
        /// <param name="logger"></param>
        public StoreItemAdapter(ILoggerAdapter logger, ICosmosDbService<dynamic> cosmosDbService)
        {
            _logger = logger;
            _cosmosDbService = cosmosDbService;
        }

        /// <summary>
        /// Fetches storeitem details from CosmosDB
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetStoreItemDetails(FilterCriteria filterCriteria, string token)
        {
            try
            {
                var result = await _cosmosDbService.GetDataWithoutPartitionKey(collection, CreateQuery(filterCriteria), token, filterCriteria.Pagination);
                return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# ProductMaster/StoreItem Adapter failed! => Unable to fetch the ProductDetails . | Request# " + JsonConvert.SerializeObject(filterCriteria) + "| Response# " + ex.Message);
                throw ex;
            }
        }
        /// <summary>
        /// Creating dynamic query
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <returns></returns>
        private string CreateQuery(FilterCriteria filterCriteria)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT (IsDefined(c.ItemSKU) = true ? c.ItemSKU : null) as ItemSKU,");
                query.Append("(IsDefined(c.StoreId) = true ? c.StoreId : null) as StoreId,");
                query.Append("(IsDefined(c.LastReceiptQuantity) = true ? c.LastReceiptQuantity : null) as LastReceiptQuantity,");
                query.Append("(IsDefined(c.YearToDateReceiptQuantity) = true ? c.YearToDateReceiptQuantity : null) as YearToDateReceiptQuantity,");
                query.Append("(IsDefined(c.YearToDateSalesAmount) = true ? c.YearToDateSalesAmount : null) as YearToDateSalesAmount,");
                query.Append("(IsDefined(c.YearToDateSalesQuantity) = true ? c.YearToDateSalesQuantity : null) as YearToDateSalesQuantity,");
                query.Append("(IsDefined(c.StoreBalanceOnHandDecimalQuantity) = true ? c.StoreBalanceOnHandDecimalQuantity : null) as StoreBalanceOnHandDecimalQuantity,");
                query.Append("(IsDefined(c.ItemFirstReceiptCostAmount) = true ? c.ItemFirstReceiptCostAmount : null) as ItemFirstReceiptCostAmount,");
                query.Append("(IsDefined(c.ItemEndOfYearCostAmount) = true ? c.ItemEndOfYearCostAmount : null) as ItemEndOfYearCostAmount,");
                query.Append("(IsDefined(c.ItemBeginningOfYearCostAmount) = true ? c.ItemBeginningOfYearCostAmount : null) as ItemBeginningOfYearCostAmount,");
                query.Append("(IsDefined(c.ItemDistributionCenterHandlingAmount) = true ? c.ItemDistributionCenterHandlingAmount : null) as ItemDistributionCenterHandlingAmount,");
                query.Append("(IsDefined(c.ItemFreightAmount) = true ? c.ItemFreightAmount : null) as ItemFreightAmount,");
                query.Append("(IsDefined(c.ItemStoreHandlingAmount) = true ? c.ItemStoreHandlingAmount : null) as ItemStoreHandlingAmount,");
                query.Append("(IsDefined(c.ItemStorageChargeAmount) = true ? c.ItemStorageChargeAmount : null) as ItemStorageChargeAmount,");
                query.Append("(IsDefined(c.ItemFinancialCreditAmount) = true ? c.ItemFinancialCreditAmount : null) as ItemFinancialCreditAmount,");
                query.Append("(IsDefined(c.ItemFinancialChargeAmount) = true ? c.ItemFinancialChargeAmount : null) as ItemFinancialChargeAmount,");
                query.Append("(IsDefined(c.StoreItemInventoryAmount) = true ? c.StoreItemInventoryAmount : null) as StoreItemInventoryAmount,");
                query.Append("(IsDefined(c.ProductMeijerPerformanceCostAmount) = true ? c.ProductMeijerPerformanceCostAmount : null) as ProductMeijerPerformanceCostAmount,");
                query.Append("(IsDefined(c.ItemCubicFootageQuantity) = true ? c.ItemCubicFootageQuantity : null) as ItemCubicFootageQuantity,");
                query.Append("(IsDefined(c.StoreItemId) = true ? c.StoreItemId : null) as StoreItemId,");
                query.Append("(IsDefined(c.LegacyPrimaryKeyId) = true ? c.LegacyPrimaryKeyId : null) as LegacyPrimaryKeyId,");
                query.Append("(IsDefined(c.MeijerPerformanceRatingSystemCategoryId) = true ? c.MeijerPerformanceRatingSystemCategoryId : null) as MeijerPerformanceRatingSystemCategoryId,");
                query.Append("(IsDefined(c.ItemInventoryClearanceId) = true ? c.ItemInventoryClearanceId : null) as ItemInventoryClearanceId,");
                query.Append("(IsDefined(c.ItemRoundCountDate) = true ? c.ItemRoundCountDate : null) as ItemRoundCountDate,");
                query.Append("(IsDefined(c.ItemFirstReceiptDate) = true ? c.ItemFirstReceiptDate : null) as ItemFirstReceiptDate,");
                query.Append("(IsDefined(c.ItemFiscalYearNewId) = true ? c.ItemFiscalYearNewId : null) as ItemFiscalYearNewId,");
                query.Append("(IsDefined(c.PhysicalCountLastAdjustment) = true ? c.PhysicalCountLastAdjustment : null) as PhysicalCountLastAdjustment,");
                query.Append("(IsDefined(c.LastPhysicalInventoryDate) = true ? c.LastPhysicalInventoryDate : null) as LastPhysicalInventoryDate,");
                query.Append("(IsDefined(c.PointofSaleBillbackIndicator) = true ? c.PointofSaleBillbackIndicator : null) as PointofSaleBillbackIndicator,");
                query.Append("(IsDefined(c.ItemLastSalesDate) = true ? c.ItemLastSalesDate : null) as ItemLastSalesDate,");
                query.Append("(IsDefined(c.LastReceiptDate) = true ? c.LastReceiptDate : null) as LastReceiptDate,");
                query.Append("(IsDefined(c.LastReceiptCategory) = true ? c.LastReceiptCategory : null) as LastReceiptCategory,");
                query.Append("(IsDefined(c.ZeroBalanceonHandIndicator) = true ? c.ZeroBalanceonHandIndicator : null) as ZeroBalanceonHandIndicator,");
                query.Append("(IsDefined(c.mProductId) = true ? c.mProductId : null) as mProductId,");
                query.Append("(IsDefined(c.INV_SUFF_FLAG) = true ? c.INV_SUFF_FLAG : null) as INV_SUFF_FLAG,");
                query.Append("(IsDefined(c.OOS_CHECK_FLAG) = true ? c.OOS_CHECK_FLAG : null) as OOS_CHECK_FLAG,");
                query.Append("(IsDefined(c.IS_SELLABLE_FLAG) = true ? c.IS_SELLABLE_FLAG : null) as IS_SELLABLE_FLAG,");
                query.Append("(IsDefined(c.RowUpdateTimestamp) = true ? c.RowUpdateTimestamp : null) as RowUpdateTimestamp,");
                query.Append("(IsDefined(c.id) = true ? c.id : null) as id");
                query.Append(" FROM c WHERE");
                if (filterCriteria.StoreId != null)
                {
                    if (filterCriteria.ItemSku != null && filterCriteria.StoreId.Count == 1)
                        query.Append($" c.id IN ({string.Join(",", filterCriteria.ItemSku.Select(itm => "'" + itm + "-" + filterCriteria.StoreId[0] + "'"))})");
                    else if (filterCriteria.ItemSku != null && filterCriteria.StoreId.Count > 1)
                        query.Append($" c.id IN ({string.Join(",", filterCriteria.StoreId.Select(itm => "'" + filterCriteria.ItemSku[0] + "-" + itm + "'"))})");
                    else if (filterCriteria.ItemSku == null)
                        query.Append($" c.StoreId IN ({string.Join(",", filterCriteria.StoreId.Select(itm => itm))})");
                }
                else
                    query.Append(" c.id LIKE \'" + filterCriteria.ItemSku[0] + "-%\'");

                query.Append(" ORDER BY c.StoreId");

                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
