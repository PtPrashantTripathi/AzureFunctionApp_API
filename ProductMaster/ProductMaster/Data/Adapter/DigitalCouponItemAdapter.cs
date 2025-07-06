using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductMaster.Data
{
    [ExcludeFromCodeCoverage]
    class DigitalCouponItemAdapter : IDigitalCouponItemAdapter
    {
        #region Constants
        static string collection = Constant.COSMOS_ITEM_COLLECTION;
        #endregion

        #region Private Members
        ILoggerAdapter _logger;
        ICosmosDbService<dynamic> _cosmosDbService;
        #endregion

        #region Contructors
        /// <summary>
        /// DigitalCouponItem constructor
        /// </summary>
        /// <param name="cosmosDbService"></param>
        /// <param name="logger"></param>
        public DigitalCouponItemAdapter(ILoggerAdapter logger, ICosmosDbService<dynamic> cosmosDbService)
        {
            _logger = logger;
            _cosmosDbService = cosmosDbService;
        }
        #endregion

        #region Method Implementation
        /// <summary>
        /// Fetches Purchase and Offer item details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetDigitalCouponItem(FilterCriteria filterCriteria, string token)
        {
            try
            {
                var result = await _cosmosDbService.GetDataWithoutPartitionKey(collection, CreateQuery(filterCriteria), token, filterCriteria.Pagination);
                return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# ProductMaster/DigitalCouponItem Adapter failed! => Unable to fetch the ProductDetails . | Request# " + JsonConvert.SerializeObject(filterCriteria) + "| Response# " + ex.Message);
                throw ex;
            }
        }
        #endregion

        #region Private Methods
        private string CreateQuery(FilterCriteria filterCriteria)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT c.ItemSku,c.RowUpdatedTimestamp,c.UPCTypeName AS UniversalTypeCategory,");
                if (filterCriteria.mCouponId != null && filterCriteria.CouponSourceId != null)
                {
                    query.Append(" ARRAY(SELECT t.mCouponId, t.CouponName, t.CouponSourceId FROM t IN c.CouponPurchaseItems WHERE");
                    query.Append($" t.mCouponId IN({string.Join(",", filterCriteria.mCouponId.Select(itm => itm))})");
                    query.Append($" AND t.CouponSourceId IN ({string.Join(", ", filterCriteria.CouponSourceId.Select(itm => itm))})) AS CouponPurchaseItems,");

                    query.Append("ARRAY(SELECT t.mCouponId, t.CouponName, t.CouponSourceId FROM t IN c.CouponOfferItems where ");
                    query.Append($" t.mCouponId IN({string.Join(",", filterCriteria.mCouponId.Select(itm => itm))}) " +
                                    $" AND t.CouponSourceId IN ({string.Join(", ", filterCriteria.CouponSourceId.Select(itm => itm))})) AS CouponOfferItems");
                }
                else
                    query.Append("c.CouponPurchaseItems,c.CouponOfferItems");


                query.Append(" FROM c WHERE (EXISTS(SELECT VALUE t FROM t IN c.CouponPurchaseItems");

                if (filterCriteria.mCouponId != null && filterCriteria.CouponSourceId != null)
                {
                    query.Append($" WHERE t.mCouponId IN ({string.Join(", ", filterCriteria.mCouponId.Select(itm => itm))}) ");
                    query.Append($" AND t.CouponSourceId IN ({string.Join(", ", filterCriteria.CouponSourceId.Select(itm => itm))}))");
                }
                else
                    query.Append(")");

                query.Append(" OR EXISTS(SELECT VALUE t FROM t IN c.CouponOfferItems ");

                if (filterCriteria.mCouponId != null && filterCriteria.CouponSourceId != null)
                {
                    query.Append($" WHERE t.mCouponId IN ({string.Join(", ", filterCriteria.mCouponId.Select(itm => itm))}) ");
                    query.Append($" AND t.CouponSourceId IN ({string.Join(", ", filterCriteria.CouponSourceId.Select(itm => itm))})))");
                }
                else
                    query.Append("))");

                if (filterCriteria.ItemSku != null)
                    query.Append($" AND c.ItemSku IN ({string.Join(",", filterCriteria.ItemSku.Select(itm => "'" + itm + "'"))})");

                query.Append(" ORDER BY c.ItemSku");

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
