using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductMaster.Data
{
    /// <summary>
    /// StoreItemAdapter Interface
    /// </summary>
    public interface IStoreItemAdapter
    {
        /// <summary>
        ///  Fetches StoreItem Details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<dynamic>> GetStoreItemDetails(FilterCriteria filterCriteria, string token);
    }
}
