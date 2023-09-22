using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Dynatrace.OpenTelemetry.Instrumentation.AzureFunctions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PosMaster.Data;
using PosMaster.Functions.Base;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;


namespace PosMaster
{
    /// <summary>
    /// Sku Function class
    /// </summary>
    public class ItemFunction : BaseFunctions
    {
        #region Private Variables
        private readonly IItemAdapter _dataAdapter;
        #endregion

        #region Constructor
        /// <summary>
        /// GetItemFunction contructor
        /// </summary>
        /// <param name="itemAdapter"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        public ItemFunction(IItemAdapter itemAdapter, ILoggerAdapter logger, IConfiguration config) : base(logger, config)
        {
            _dataAdapter = itemAdapter;
        }
        #endregion

        #region FunctionsMethod
        /// <summary>
        /// This Function fetches Sku details
        /// </summary>
        /// <param name="req"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [FunctionName("Sku")]
        public async Task<IActionResult> Sku(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Sku")]
            [RequestBodyType(typeof(string), "Sku")]HttpRequest req, ExecutionContext ctx)
        {
            AzureFunctionsInstrumentation.AddIncomingHttpAzureFunctionCallInfo(Activity.Current, ctx);
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            try
            {
                //Validating Input Json
                string errorMessage = InputValidation(requestBody);
                if (errorMessage != string.Empty)
                {
                    if (_logger != null)
                        _logger.LogEvent("HttpsResponse : PosMaster/Sku BadRequest :" + errorMessage, requestBody);
                    return new BadRequestObjectResult(new { error = errorMessage });
                }


                //Serializing Input json 
                var filterCriteria = JsonConvert.DeserializeObject<FilterCriteria>(requestBody);

                //if (filterCriteria.Sku == null && filterCriteria.RowUpdatedStartDate == DateTime.MinValue && filterCriteria.UPCTypeName == null)
                //{
                //    errorMessage = "Please provide atleast one valid criteria";
                //    if (_logger != null)
                //        _logger.LogEvent("HttpsResponse : PosMaster/Sku BadRequest :" + errorMessage, requestBody);
                //    return new BadRequestObjectResult(new { error = errorMessage });
                //}

                //Log Httprequest
                if (_logger != null)
                    _logger.LogEvent("HttpsRequest : PosMaster/Sku Calling", JsonConvert.SerializeObject(filterCriteria));

                //Reading Headers if any
                string token = GetTokeFromHeader(req);

                //Calling adapter class
                var result = await _dataAdapter.GetDetails(filterCriteria, token);


                //Log HttpsResponse
                if (_logger != null)
                    _logger.LogEvent("HttpsResponse : PosMaster/Sku Executed :", JsonConvert.SerializeObject(result));
                if (_logger != null)
                    _logger.LogInformation("HttpsResponse : PosMaster/Sku Executed :", JsonConvert.SerializeObject(result));


                return new OkObjectResult(DynamicMapping(result));
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# PosMaster/Sku : Failed! => Unable to fetch the Pos details. | Request# " + JsonConvert.SerializeObject(requestBody) + "| Response# " + ex.Message);
                return new BadRequestObjectResult(new { error = ex.Message });
            }
        }
        #endregion
    }
}


