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
    ///
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DigitalAvailableStoreItemAdapter : IDigitalAvailableStoreItemAdapter
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
        /// constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cosmosDbService"></param>
        public DigitalAvailableStoreItemAdapter(ILoggerAdapter logger, ICosmosDbService<dynamic> cosmosDbService)
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
        public async Task<List<dynamic>> GetDigitalAvailableStoreItem(FilterCriteria filterCriteria, string token)
        {
            try
            {
                var result = await _cosmosDbService.GetDataWithoutPartitionKey(collection, CreateQuery(filterCriteria), token, filterCriteria.Pagination);
                return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# ProductMaster/DigitalAvailableStoreItem Adapter failed! => Unable to fetch the ProductDetails . | Request# " + JsonConvert.SerializeObject(filterCriteria) + "| Response# " + ex.Message);
                throw ex;
            }
        }



        #endregion

        #region Private Methods
        /// <summary>
        /// Creates query for cosmos
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <returns></returns>
        private string CreateQuery(FilterCriteria filterCriteria)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT c.ItemSku, c.RowUpdatedTimestamp,c.UPCTypeName,");

                if (filterCriteria.StoreId != null)
                    query.Append($" ARRAY(SELECT VALUE t FROM t in c.StoreItems where t.StoreID IN ({string.Join(",", filterCriteria.StoreId.Select(itm => itm))})) AS StoreItems");
                else
                    query.Append(" c.StoreItems");

                query.Append(" FROM c");
                query.Append(" JOIN st IN c.StoreItems");
                query.Append(" WHERE  c.ItemSku <> ''");

                if (filterCriteria.ItemSku != null)
                    query.Append($" AND c.ItemSku IN ({string.Join(",", filterCriteria.ItemSku.Select(itm => "'" + itm + "'"))} )");

                if (filterCriteria.UPCTypeName != null)
                    query.Append($" AND c.UPCTypeName IN ({string.Join(",", filterCriteria.UPCTypeName.Select(itm => "'" + itm + "'"))})");

                if (filterCriteria.StoreId != null)
                    query.Append($" AND st.StoreID IN ({filterCriteria.StoreId.FirstOrDefault()})");

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
