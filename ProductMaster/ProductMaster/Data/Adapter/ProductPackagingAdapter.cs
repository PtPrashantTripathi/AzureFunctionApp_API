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
    /// ProductPackagingAdapter class
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductPackagingAdapter : IProductPackagingAdapter
    {
        #region Constants
        static string collection = Constant.COSMOS_PRODUCTPACKAGING_COLLECTION;
        #endregion

        #region Private Members
        ILoggerAdapter _logger;
        ICosmosDbService<dynamic> _cosmosDbService;
        #endregion

        #region Contructors
        /// <summary>
        /// ProductPackagingAdapter constructor
        /// </summary>
        /// <param name="cosmosDbService"></param>
        /// <param name="logger"></param>
        public ProductPackagingAdapter(ILoggerAdapter logger, ICosmosDbService<dynamic> cosmosDbService)
        {
            _logger = logger;
            _cosmosDbService = cosmosDbService;
        }
        #endregion

        #region Method Implementation
        /// <summary>
        /// Fetches ProductPackaging details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetProductPackaging(FilterCriteria filterCriteria, string token)
        {
            try
            {
                var result = await _cosmosDbService.GetDataWithoutPartitionKey(collection, CreateQuery(filterCriteria), token, filterCriteria.Pagination);
                return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# ProductMaster/ProductPackaging Adapter failed! => Unable to fetch the Details . | Request# " + JsonConvert.SerializeObject(filterCriteria) + "| Response# " + ex.Message);
                throw ex;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Creates query for cosmos[New Optimized query fixes on duplicate records]
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <returns></returns>
        private string CreateQuery(FilterCriteria filterCriteria)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                bool flag = false;
                query.Append("SELECT c.id, c.mProductId, c.CasePackId, c.mVendorID");

                query.Append(", ARRAY(SELECT VALUE t FROM t in c.CasePackDetails");
                if (filterCriteria.ItemSku != null || filterCriteria.InnerPackId != null)
                {
                    query.Append($" WHERE");
                    if (filterCriteria.ItemSku != null)
                    {
                        query.Append($" t.ItemSKU IN ({string.Join(", ", filterCriteria.ItemSku.Select(itm => itm))})");
                        flag = true;
                    }
                    if (filterCriteria.InnerPackId != null)
                    {
                        if (flag) query.Append(" AND");
                        query.Append($" t.InnerPackId IN ({string.Join(", ", filterCriteria.InnerPackId.Select(itm => itm))})");
                    }
                    flag = false;
                }
                query.Append($") AS CasePackDetails");

                query.Append($", ARRAY(SELECT VALUE t FROM t in c.InnerPackDetails");
                if (filterCriteria.ItemCodeId != null || filterCriteria.InnerPackId != null)
                {
                    query.Append($" WHERE");
                    if (filterCriteria.ItemCodeId != null)
                    {
                        query.Append($" t.ItemCodeId IN ({string.Join(", ", filterCriteria.ItemCodeId.Select(itm => "'" + itm + "'"))})");
                        flag = true;
                    }
                    if (filterCriteria.InnerPackId != null)
                    {
                        if (flag) query.Append(" AND");
                        query.Append($" t.InnerPackId IN ({string.Join(", ", filterCriteria.InnerPackId.Select(itm => itm))})");
                    }
                    flag = false;
                }
                query.Append($") AS InnerPackDetails");

                query.Append(", c.VendorCosting, c.RowUpdateTimestamp");
                query.Append(" FROM c WHERE");

                if (filterCriteria.mProductId != null)
                {
                    query.Append($" c.mProductId IN({string.Join(", ", filterCriteria.mProductId.Select(itm => "'" + itm + "'"))})");
                    flag = true;
                }

                if (filterCriteria.CasePackId != null)
                {
                    if (flag) query.Append("AND");
                    query.Append($" c.CasePackId IN ({string.Join(", ", filterCriteria.CasePackId.Select(itm => "'" + itm + "'"))})");
                    flag = true;
                }

                if (filterCriteria.mVendorID != null)
                {
                    if (flag) query.Append(" AND");
                    query.Append($" c.mVendorID IN ({string.Join(", ", filterCriteria.mVendorID.Select(itm => itm))})");
                    flag = true;
                }

                if (filterCriteria.ItemSku != null || filterCriteria.InnerPackId != null)
                {
                    if (flag) query.Append(" AND");
                    flag = false;

                    query.Append($" EXISTS (SELECT VALUE t FROM t in c.CasePackDetails WHERE");
                    if (filterCriteria.ItemSku != null)
                    {
                        query.Append($" t.ItemSKU IN ({string.Join(", ", filterCriteria.ItemSku.Select(itm => itm))})");
                        flag = true;
                    }

                    if (filterCriteria.InnerPackId != null)
                    {
                        if (flag) query.Append(" AND");
                        query.Append($" t.InnerPackId IN ({string.Join(", ", filterCriteria.InnerPackId.Select(itm => itm))})");
                        flag = true;
                    }
                    query.Append(")");
                }

                if (filterCriteria.ItemCodeId != null || filterCriteria.InnerPackId != null)
                {
                    if (flag) query.Append(" AND");
                    flag = false;

                    query.Append($" EXISTS (SELECT VALUE t FROM t in c.InnerPackDetails WHERE");
                    if (filterCriteria.ItemCodeId != null)
                    {
                        query.Append($" t.ItemCodeId IN ({string.Join(", ", filterCriteria.ItemCodeId.Select(itm => "'" + itm + "'"))})");
                        flag = true;
                    }

                    if (filterCriteria.InnerPackId != null)
                    {
                        if (flag) query.Append(" AND");
                        query.Append($" t.InnerPackId IN ({string.Join(", ", filterCriteria.InnerPackId.Select(itm => itm))})");
                    }
                    query.Append(")");
                }
                query.Append(" ORDER BY c.mProductId");
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
