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
    /// OSAAlertsAdapter class
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class OSAAlertsAdapter : IOSAAlertsAdapter
    {

        # region Constants
        static readonly string collection = Constant.COSMOS_OSAALERTS_COLLECTION;
        #endregion

        #region Private Members
        private readonly ILoggerAdapter _logger;
        private readonly ICosmosDbService<dynamic> _cosmosDbService;
        #endregion

        #region Contructors
        /// <summary>
        /// OSAAlertsAdapter Constructor
        /// </summary>
        /// <param name="cosmosDbService"></param>
        /// <param name="logger"></param>
        public OSAAlertsAdapter(ILoggerAdapter logger, ICosmosDbService<dynamic> cosmosDbService)
        {
            _logger = logger;
            _cosmosDbService = cosmosDbService;
        }
        #endregion

        #region Method Implementation
        /// <summary>
        /// Fetches OSAAlerts details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetDetails(FilterCriteria filterCriteria, string token)
        {
            try
            {
                // Fetches maxTimestamp from OSAAlerts database
                string getMaxTimestampQuery = "SELECT max(c.RowUpdateTimestamp) as max from c";
                var maxTimestamp = await _cosmosDbService.GetDataWithoutPartitionKey(collection, getMaxTimestampQuery, null, 0);

                var result = await _cosmosDbService.GetDataWithoutPartitionKey(collection, CreateQuery(filterCriteria, maxTimestamp), token, filterCriteria.Pagination);
                return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# ProductMaster/OSAAlerts Adadpter failed! => Unable to fetch the OSAAlerts Details . | Request# " + JsonConvert.SerializeObject(filterCriteria) + "| Response# " + ex.Message);
                throw ex;
            }
        }
        #endregion

        #region CreateQuery Methods
        /// <summary>
        /// Creates query for cosmos
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="maxtimestamp"></param>
        /// <returns></returns>
        private string CreateQuery(FilterCriteria filterCriteria, List<dynamic> maxtimestamp)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT c.day_dt, c.ut_id, c.p_id, c.p_nm, c.L5_id, c.upc_id, c.boh, c.alert_type," +
                    " c.fcst, c.it_loc_asl_id, c.it_loc_sctn_id, c.it_loc_pstn_id, c.promo, c.oos_hits, c.dsd_flag," +
                    " c.alert_table_creation_ts, c.RowUpdateTimestamp");
                query.Append(" FROM c WHERE");
                query.Append($" c.RowUpdateTimestamp = '{maxtimestamp[0].max}'");
                if (filterCriteria.Purpose.Trim().ToLower() == "product details")
                {
                    if (filterCriteria.L5_id != null)
                    {
                        query.Append($" and c.L5_id IN({string.Join(",", filterCriteria.L5_id.Select(itm => "'" + itm + "'"))})");
                    }
                    if (filterCriteria.ut_id != null)
                    {
                        query.Append($" and c.ut_id IN({string.Join(",", filterCriteria.ut_id.Select(itm => itm))})");
                    }
                }
                query.Append(" ORDER BY c.ut_id");
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
