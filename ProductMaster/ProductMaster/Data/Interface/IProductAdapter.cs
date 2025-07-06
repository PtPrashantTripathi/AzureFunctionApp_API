using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductMaster.Data
{
    /// <summary>
    /// ProductAdapter Interface
    /// </summary>
    public interface IProductAdapter
    {
        /// <summary>
        /// Fetches Product Details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<dynamic>> GetProduct(FilterCriteria filterCriteria, string token);
    }
}
