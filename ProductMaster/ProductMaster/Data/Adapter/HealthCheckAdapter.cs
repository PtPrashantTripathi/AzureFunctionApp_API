using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace ProductMaster.Data
{
    /// <summary>
    /// HealthCheckAdapter class
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HealthCheckAdapter : IHealthCheck
    {
        #region Private Variables
        ILoggerAdapter _logger;
        CosmosClient _cosmosClient;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cosmosClient"></param>
        public HealthCheckAdapter(ILoggerAdapter logger, CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;
        }

        #region Method Implementation
        /// <summary>
        /// This Function fetches Cosmos connectivity health diagnostics details
        /// </summary>
        /// <returns>HealthCheckResult</returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _cosmosClient.ReadAccountAsync().ConfigureAwait(false);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# ProductMaster/HealthCheckAdapter Cosmos connectivity failed! => Unable to fetch the HealthCheck . | Response# " + ex.Message);
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
        #endregion
    }
}
