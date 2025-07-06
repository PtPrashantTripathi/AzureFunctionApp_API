using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductMaster.Data
{
    /// <summary>
    /// DigitalCouponItemAdapter interface
    /// </summary>
    public interface IDigitalCouponItemAdapter
    {
        /// <summary>
        /// GetDigitalCouponItem Interface
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<dynamic>> GetDigitalCouponItem(FilterCriteria filterCriteria, string token);
    }
}
