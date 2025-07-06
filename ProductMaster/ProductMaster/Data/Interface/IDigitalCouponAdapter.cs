using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductMaster.Data
{
    /// <summary>
    /// Interface for all operation related to DigitalCoupon details
    /// </summary>
    public interface IDigitalCouponAdapter
    {
        /// <summary>
        /// Fetches DigitalCoupon details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<dynamic>> GetCouponDetails(FilterCriteria filterCriteria, string token);
    }
}
