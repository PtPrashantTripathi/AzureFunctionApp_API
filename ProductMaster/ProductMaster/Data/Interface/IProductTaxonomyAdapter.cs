using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductMaster.Data
{
    /// <summary>
    /// IProductTaxonomy Interface
    /// </summary>
    public interface IProductTaxonomyAdapter
    {
        /// <summary>
        /// Fetches ProductTaxonomy Details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<dynamic>> GetProductTaxonomy(FilterCriteria filterCriteria, string token);
    }
}
