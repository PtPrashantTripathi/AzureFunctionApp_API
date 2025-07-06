using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductMaster.Data
{
    /// <summary>
    /// ProductPackaging Interface
    /// </summary>
    public interface IProductPackagingAdapter
    {
        /// <summary>
        /// Fetches ProductPackaging Details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<dynamic>> GetProductPackaging(FilterCriteria filterCriteria, string token);
    }
}
