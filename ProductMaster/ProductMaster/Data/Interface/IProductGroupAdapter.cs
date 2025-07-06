using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductMaster.Data
{
    /// <summary>
    /// ProductGroupAdapter interface
    /// </summary>
    public interface IProductGroupAdapter
    {
        /// <summary>
        ///  Fetches ProductGroup details and GetPrimaryKeyandProductHierarchy
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<dynamic>> GetProductGroupId(FilterCriteria filterCriteria, string token);

    }
}
