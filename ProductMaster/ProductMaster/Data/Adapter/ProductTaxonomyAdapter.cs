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
    /// ProductTaxonomyAdapter class
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductTaxonomyAdapter : IProductTaxonomyAdapter
    {
        # region Constants
        static string collection = Constant.COSMOS_PRODUCTTAXONOMY_COLLECTION;
        #endregion

        #region Private Members
        ILoggerAdapter _logger;
        ICosmosDbService<dynamic> _cosmosDbService;


        #endregion

        #region Contructors
        /// <summary>
        /// ProductTaxonomyAdapter constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cosmosDbService"></param>
        public ProductTaxonomyAdapter(ILoggerAdapter logger, ICosmosDbService<dynamic> cosmosDbService)
        {
            _logger = logger;
            _cosmosDbService = cosmosDbService;
        }
        #endregion

        #region Method Implementation
        /// <summary>
        /// Fetches Upc ProductTaxamony details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetProductTaxonomy(FilterCriteria filterCriteria, string token)
        {
            try
            {
                var result = await _cosmosDbService.GetDataWithoutPartitionKey(collection, CreateQuery(filterCriteria), token, filterCriteria.Pagination);
                return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message#  ProductMaster/GetProductTaxonomy Adapter failed! => Unable to fetch the ProductTaxonomy details . | Request# " + JsonConvert.SerializeObject(filterCriteria) + "| Response# " + ex.Message);
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
            StringBuilder query = new StringBuilder();
            query.Append("SELECT c.mProductID , c.mTaxonomyMapId,c.RowUpdateTimestamp, c.TaxonomyLevels FROM c");

            if (filterCriteria.mProductTaxonomyID != null)
                query.Append(" JOIN pt in c.TaxonomyLevels");

            query.Append(" WHERE c.mProductID <> ''");

            if (filterCriteria.mProductId != null)
            {
                if (filterCriteria.mProductId != null)
                    query.Append($" AND c.mProductID IN({string.Join(",", filterCriteria.mProductId.Select(itm => "'" + itm + "'"))})");
            }
            if (filterCriteria.mProductTaxonomyID != null)
                query.Append($" AND pt.mProductTaxonomyID = {filterCriteria.mProductTaxonomyID}");

            query.Append(" ORDER BY c.mProductID");

            return query.ToString();
        }
        #endregion
    }
}
