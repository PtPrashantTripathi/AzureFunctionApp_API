using System.Collections.Generic;
using System.Threading.Tasks;

namespace PosMaster.Data
{
    /// <summary>
    /// Interface for all operation related to Sku details
    /// </summary>
    public interface IItemAdapter
    {
        /// <summary>
        /// Fetches Sku details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// /// <param name="token"></param>
        /// <returns></returns>
        Task<List<dynamic>> GetDetails(FilterCriteria filterCriteria, string token);
    }
}
