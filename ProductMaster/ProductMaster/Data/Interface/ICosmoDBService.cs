using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductMaster.Data
{
    /// <summary>
    /// CosmosDbService Interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICosmosDbService<T>
    {
        /// <summary>
        /// Connect and fetches data from cosmos with partition key
        /// </summary>
        /// <param name="Collection"></param>
        /// <param name="Query"></param>
        /// <param name="Token"></param>
        /// <param name="Pagination"></param>
        /// <returns></returns>
        Task<List<T>> GetDataWithoutPartitionKey(string Collection, string Query, string Token, int Pagination);
    }
}
