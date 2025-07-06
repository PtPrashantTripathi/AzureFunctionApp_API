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
    /// Fetches ProductGroup details
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductGroupAdapter : IProductGroupAdapter
    {
        #region Constants
        static string mProductGroupCollection = Constant.COSMOS_PRODUCTGROUP_COLLECTION;
        static string relatedGroupCollection = Constant.COSMOS_RELATEDPRODUCTGROUP_COLLECTION;
        #endregion

        #region Private Members
        ILoggerAdapter _logger;
        ICosmosDbService<dynamic> _cosmosDbService;
        #endregion

        #region Contructors
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cosmosDbService"></param>
        public ProductGroupAdapter(ILoggerAdapter logger, ICosmosDbService<dynamic> cosmosDbService)
        {
            _logger = logger;
            _cosmosDbService = cosmosDbService;
        }
        #endregion

        #region Method Implementation
        /// <summary>
        /// Fetches ProductGroupId details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetProductGroupId(FilterCriteria filterCriteria, string token)
        {
            try
            {
                var Collection = string.Empty;
                if (filterCriteria.mRelatedProductGroupId != null && filterCriteria.Purpose.Trim().ToLower() == "product details")
                    Collection = relatedGroupCollection;
                else
                    Collection = mProductGroupCollection;
                var result = await _cosmosDbService.GetDataWithoutPartitionKey(Collection, CreateQuery(filterCriteria), token, filterCriteria.Pagination);
                return result;
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
        /// <returns></returns>
        private string CreateQuery(FilterCriteria filterCriteria)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                if (filterCriteria.mProductGroupId != null)
                {
                    query.Append("SELECT c.ItemDetails, c.MDSCategory, c.ProductName,c.ParentLevel_2, c.ParentLevel_3, c.ParentLevel_4, c.ParentLevel_5, c.ParentLevel_6, ");
                    query.Append("c.mProductGroupId, c.MDSDepartment, c.MPRSHierarchy , c.RowUpdateTimestamp, c.ProductCategory, c.id ");
                    query.Append(" FROM c Where");
                    query.Append($" c.mProductGroupId IN({string.Join(",", filterCriteria.mProductGroupId.Select(itm => "'" + itm + "'"))})");
                    query.Append(" ORDER BY c.mProductGroupId");
                }
                else if (filterCriteria.LegacyPrimaryKeyId != 0)
                {
                    query.Append($"SELECT TOP 1 t.LegacyPrimaryKeyId,t.LegacyPrimaryKeyText FROM t in c.MPRSHierarchy where t.LegacyPrimaryKeyId IN ({filterCriteria.LegacyPrimaryKeyId})");
                }
                else if (filterCriteria.mRelatedProductGroupId != null)
                {
                    if (filterCriteria.Purpose.Trim().ToLower() == "product details")
                    {
                        query.Append("SELECT c.ItemDetails,c.ProductGroupId,c.ProductGroupLevel,c.ProductGroupName,c.mProductGroupId,c.LegacyMPRSCategoryText,c.LegacyProductCategoryId,");
                        query.Append("c.RelatedProductGroupId,c.RowUpdateTimestamp,c.LegacyMPRSCategoryId,c.LegacyPrimaryKeyId,c.id,c.mRelatedProductGroupId,c.LegacyPrimaryKeyText, ");
                        query.Append("c.LegacyProductCategoryText ");
                        query.Append(" FROM c Where");
                        query.Append($" c.mRelatedProductGroupId IN({string.Join(",", filterCriteria.mRelatedProductGroupId.Select(itm => "'" + itm + "'"))})");
                        query.Append(" ORDER BY c.mRelatedProductGroupId");
                    }
                    else if (filterCriteria.Purpose.Trim().ToLower() == "all hierarchy")
                    {
                        query.Append("SELECT c.ItemDetails, c.MDSCategory, c.ProductName,c.ParentLevel_2, c.ParentLevel_3, c.ParentLevel_4, c.ParentLevel_5, c.ParentLevel_6, ");
                        query.Append("c.mProductGroupId, c.MDSDepartment, c.MPRSHierarchy , c.RowUpdateTimestamp, c.ProductCategory, c.id ");
                        query.Append($" FROM c WHERE(EXISTS (SELECT VALUE t FROM t in c.ParentLevel_{filterCriteria.mRelatedProductGroupId[0][1]} WHERE");
                        query.Append($" t.mRelatedProductGroupId = '{filterCriteria.mRelatedProductGroupId[0]}'))");
                        query.Append(" ORDER BY c.mProductGroupId");
                    }
                }

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
