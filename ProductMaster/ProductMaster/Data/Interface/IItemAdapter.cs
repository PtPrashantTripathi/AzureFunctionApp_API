using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductMaster.Data
{
    /// <summary>
    /// Interface for all operation related to Item details
    /// </summary>
    public interface IItemAdapter
    {
        /// <summary>
        /// Fetches Item details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// /// <param name="token"></param>
        /// <returns></returns>
        Task<List<dynamic>> GetDetails(FilterCriteria filterCriteria, string token);
    }
}
