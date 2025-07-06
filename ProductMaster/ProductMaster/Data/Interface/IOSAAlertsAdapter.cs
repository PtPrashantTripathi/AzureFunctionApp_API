using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductMaster.Data
{
    /// <summary>
    /// Interface for all operation related to OSA alerts details
    /// </summary>
    public interface IOSAAlertsAdapter
    {
        /// <summary>
        /// Fetches OSAAlerts Details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<dynamic>> GetDetails(FilterCriteria filterCriteria, string token);
    }
}
