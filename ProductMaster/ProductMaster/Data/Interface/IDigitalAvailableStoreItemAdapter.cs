using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductMaster.Data
{
    /// <summary>
    /// DigitalAvailableStoreItemAdapter Interface
    /// </summary>
    public interface IDigitalAvailableStoreItemAdapter
    {
        /// <summary>
        /// Fetches DigitalAvailableStoreItem Details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<dynamic>> GetDigitalAvailableStoreItem(FilterCriteria filterCriteria, string token);
    }
}
