using Dynatrace.OpenTelemetry.Instrumentation.AzureFunctions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PosMaster.Data;
using PosMaster.Functions.Base;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace PosMaster
{
    /// <summary>
    /// HealthCheck class
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HealthCheck : BaseFunctions
    {
        #region Constants
        private readonly HealthCheckService _healthCheck;
        #endregion

        #region Constructor
        /// <summary>
        /// HealthCheck Function contructor
        /// </summary>
        /// <param name="healthCheck"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        public HealthCheck(HealthCheckService healthCheck, ILoggerAdapter logger, IConfiguration config) : base(logger, config)
        {
            _healthCheck = healthCheck;
        }
        #endregion

        #region Functions Method
        /// <summary>
        /// This Function fetches health diagnostics details
        /// </summary>
        /// <param name="req"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [FunctionName("HealthCheck")]
        public async Task<IActionResult> Health(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "HealthCheck")] HttpRequest req, ExecutionContext ctx)
        {
            AzureFunctionsInstrumentation.AddIncomingHttpAzureFunctionCallInfo(Activity.Current, ctx);
            try
            {
                var status = await _healthCheck.CheckHealthAsync();
                if (status.Status == HealthStatus.Healthy)
                {
                    return new OkObjectResult(Enum.GetName(typeof(HealthStatus), status.Status));
                }
                else
                {
                    return new ObjectResult(Enum.GetName(typeof(HealthStatus), status.Status)) { StatusCode = 500 };
                }
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# PosMaster/HealthCheck : Failed! => Unable to fetch the HealthCheck details. | Error Message# " + ex.Message);
                return new BadRequestObjectResult(new
                {
                    error = ex.Message
                });
            }
        }
        #endregion
    }
}