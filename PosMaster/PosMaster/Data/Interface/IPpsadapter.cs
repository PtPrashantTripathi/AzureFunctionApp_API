using System.Collections.Generic;
using System.Threading.Tasks;

namespace PosMaster.Data
{
    /// <summary>
    /// PosAdapter Interface
    /// </summary>
    public interface IPosAdapter
    {
        /// <summary>
        /// Fetches Pos Details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<dynamic>> GetPos(FilterCriteria filterCriteria, string token);
    }
}
