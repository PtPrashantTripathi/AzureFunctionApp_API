using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Polly;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ProductMaster.Data.Adapter
{
    /// <summary>
    /// CosmosDbService class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ExcludeFromCodeCoverage]
    public class CosmosDbService<T> : ICosmosDbService<T>
    {
        #region Private variables
        private Container _container;
        private CosmosClient _dbClient;
        private readonly HttpContext _context;
        private string Database = Constant.COSMOS_DB;
        private const string CONFIG_PAGINATION = "Pagination";
        ILoggerAdapter _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client"></param>
        /// <param name="contextAccessor"></param>
        /// <param name="logger"></param>
        public CosmosDbService(CosmosClient client, IHttpContextAccessor contextAccessor, ILoggerAdapter logger)
        {
            _dbClient = client;
            _context = contextAccessor.HttpContext;
            _logger = logger;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Connect and fetches data from cosmos with partition key
        /// </summary>
        /// /// <param name="Collection"></param>
        /// <param name="Query"></param>
        /// <param name="Token"></param>
        /// <param name="Pagination"></param>
        /// <returns></returns>
        public async Task<List<T>> GetDataWithoutPartitionKey(string Collection, string Query, string Token, int Pagination)
        {
            {
                try
                {
                    var retryPolicyException = Policy.Handle<Microsoft.Azure.Cosmos.CosmosException>(e => e.StatusCode == System.Net.HttpStatusCode.TooManyRequests).WaitAndRetryAsync(
                               retryCount: 3, // Retry 3 times
                               sleepDurationProvider: (retryAttempt, exception, context) =>
                               {
                                   var timeSpanToWait = (CosmosException)exception;
                                   return timeSpanToWait.RetryAfter ?? TimeSpan.FromSeconds(1); // if Retry has value wait for number mentioned else defautl of 2 Sec.
                               },
                               onRetryAsync: (exception, interval, retryAttempt, context) =>
                               {
                                   // logging necessary information
                                   _logger.LogError(exception, $"OnRetryAsync: Retry {retryAttempt} after interval of {interval}.");
                                   return Task.CompletedTask;
                               });

                    var results = new List<T>();
                    string continuostoken = Token != null ? Token : null;
                    int pagination = Pagination > 0 ? Pagination : Convert.ToInt32(Environment.GetEnvironmentVariable(CONFIG_PAGINATION));
                    this._container = _dbClient.GetContainer(Database, Collection);
                    FeedIterator<T> resultSet = this._container.GetItemQueryIterator<T>(
                    Query, continuostoken,
                    requestOptions: new QueryRequestOptions()
                    {
                        MaxItemCount = pagination,
                        ResponseContinuationTokenLimitInKb = 1
                    });

                    FeedResponse<T> response = null;

                    while (resultSet.HasMoreResults)
                    {
                        response = await retryPolicyException.ExecuteAsync(
                            () => resultSet.ReadNextAsync());
                        results.AddRange(response);
                        if (results.Count() >= pagination)
                            break;
                    }

                    var HasMoreResults = resultSet.HasMoreResults;
                    var ContinuationToken = resultSet.HasMoreResults ? EncodingForBase64.EncodeBase64(response.ContinuationToken) : null;

                    if (!string.IsNullOrEmpty(ContinuationToken))
                    {
                        if (_context.Response.Headers["Token"].ToString() == "")
                        {
                            _context.Response.Headers.TryAdd("Token", ContinuationToken);
                            _context.Response.Headers.Add("HasMoreResults", Convert.ToString(HasMoreResults));
                        }
                        //Old code..
                        //_context.Response.Headers.Add("Token", ContinuationToken);
                        //_context.Response.Headers.Add("HasMoreResults", Convert.ToString(HasMoreResults));
                    }

                    return results;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion
    }
}
