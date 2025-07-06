using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ProductMaster.Data;
using ProductMaster.Functions.Base;
using System;
using System.IO;
using System.Threading.Tasks;


namespace ProductMaster
{
    /// <summary>
    /// OSA alerts Function class
    /// </summary>
    public class OSAAlertsFunction : BaseFunctions
    {
        #region Private Variables
        private readonly IOSAAlertsAdapter _dataAdapter;
        #endregion

        #region Constructor
        /// <summary>
        /// OSAAlerts Function Contructor
        /// </summary>
        /// <param name="OSAAlertsadapter"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        public OSAAlertsFunction(IOSAAlertsAdapter OSAAlertsadapter, ILoggerAdapter logger, IConfiguration config) : base(logger, config)
        {
            this._dataAdapter = OSAAlertsadapter;
        }
        #endregion

        #region Functions
        /// <summary>
        /// This Function fetches OSA Alert details
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("OSAAlerts")]
        public async Task<IActionResult> OSAAlerts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "OSAAlerts")]
            [RequestBodyType(typeof(string), "ut_id")] HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            try
            {
                //Validating Input Json
                string errorMessage = InputValidation(requestBody);

                //Serializing Input json
                var filterCriteria = JsonConvert.DeserializeObject<FilterCriteria>(requestBody);

                //filterCriteria Validation
                if (errorMessage == string.Empty)
                {
                    if (filterCriteria.Purpose == null || !(filterCriteria.Purpose.Trim().ToLower() == "product details" || filterCriteria.Purpose.Trim().ToLower() == "all"))
                    {
                        errorMessage = "Please provide Valid Purpose either 'All' or 'Product Details'";
                    }
                    else if (filterCriteria.Purpose.Trim().ToLower() == "all")
                    {
                        if (filterCriteria.ut_id != null || filterCriteria.L5_id != null)
                        {
                            errorMessage = "API does not support ut_id/L5_id combination with purpose ALL.";
                        }
                    }
                    else if (filterCriteria.Purpose.Trim().ToLower() == "product details")
                    {
                        if (filterCriteria.ut_id == null && filterCriteria.L5_id == null)
                        {
                            errorMessage = "Please provide atleast one valid ut_id or L5_id data.";
                        }
                    }
                }
                //errorMessage Logger
                if (errorMessage != string.Empty)
                {
                    if (_logger != null)
                        _logger.LogEvent("HttpsResponse : ProductMaster/GetOSAAlert BadRequest :" + errorMessage, requestBody);
                    return new BadRequestObjectResult(new { error = errorMessage });
                }
                //Log Httprequest
                if (_logger != null)
                    _logger.LogEvent("HttpsRequest : ProductMaster/OSAAlerts Calling", JsonConvert.SerializeObject(filterCriteria));

                //Reading Headers if any
                string token = GetTokeFromHeader(req);
                var result = await _dataAdapter.GetDetails(filterCriteria, token);

                //Log HttpsResponse
                if (_logger != null)
                    _logger.LogEvent("HttpsResponse : ProductMaster/OSAAlerts Executed :", JsonConvert.SerializeObject(result));
                if (_logger != null)
                    _logger.LogInformation("HttpsResponse : ProductMaster/OSAAlerts Executed :", JsonConvert.SerializeObject(result));


                #region Calling Daynamic JSON method
                var output = DynamicMapping(result);
                #endregion

                return new OkObjectResult(output);

            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# ProductMaster/OSAAlerts : Failed! => Unable to fetch the OSAAlerts details. | Request# " + JsonConvert.SerializeObject(requestBody) + "| Response# " + ex.Message);
                return new BadRequestObjectResult(new { error = ex.Message });
            }
        }
        #endregion
    }
}
