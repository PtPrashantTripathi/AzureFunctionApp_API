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
    /// DigitalAvailableStoreItemFunction class
    /// </summary>
    public class DigitalAvailableStoreItemFunction : BaseFunctions
    {
        #region Private Variables
        private readonly IDigitalAvailableStoreItemAdapter _dataAdapter;
        #endregion

        #region Constructor
        /// <summary>
        /// DigitalAvailableStoreItemFunction Constructor
        /// </summary>
        /// <param name="digitalStore"></param>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public DigitalAvailableStoreItemFunction(IDigitalAvailableStoreItemAdapter digitalStore, IConfiguration config, ILoggerAdapter logger) : base(logger, config)
        {
            _dataAdapter = digitalStore;
        }
        #endregion

        #region Functions

        /// <summary>
        /// This Function fetches Product details
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("DigitalAvailableStoreItem")]
        public async Task<IActionResult> DigitalAvailableStoreItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "DigitalAvailableStoreItem")]
            [RequestBodyType(typeof(string), "Product")]HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            try
            {
                //Validating Input Json
                string errorMessage = InputValidation(requestBody);
                if (errorMessage != string.Empty)
                {
                    if (_logger != null)
                        _logger.LogEvent($"HttpsResponse : ProductMaster/DigitalAvailableStoreItem BadRequest :{errorMessage}", requestBody);
                    return new BadRequestObjectResult(new { error = errorMessage });
                }

                var filterCriteria = JsonConvert.DeserializeObject<FilterCriteria>(requestBody);
                if (filterCriteria.ItemSku == null && filterCriteria.UPCTypeName == null && filterCriteria.StoreId == null)
                {
                    errorMessage = "Please provide atleast one valid criteria";
                    if (_logger != null)
                        _logger.LogEvent($"HttpsResponse : ProductMaster/DigitalAvailableStoreItem BadRequest :{errorMessage}", requestBody);
                    return new BadRequestObjectResult(new { error = errorMessage });
                }

                //Log Httprequest
                if (_logger != null)
                    _logger.LogEvent("HttpsRequest : ProductMaster/DigitalAvailableStoreItem Calling", JsonConvert.SerializeObject(filterCriteria));

                //Reading Headers if any
                string token = GetTokeFromHeader(req);

                var result = await _dataAdapter.GetDigitalAvailableStoreItem(filterCriteria, token);


                //Log HttpsResponse
                if (_logger != null)
                    _logger.LogEvent("HttpsResponse : ProductMaster/DigitalAvailableStoreItem Executed :", JsonConvert.SerializeObject(result));
                if (_logger != null)
                    _logger.LogInformation("HttpsResponse : ProductMaster/DigitalAvailableStoreItem Executed :", JsonConvert.SerializeObject(result));


                #region Dynamic JSON Serilization
                var output = DynamicMapping(result);
                #endregion

                return new OkObjectResult(output);

            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, $"Custom Message# ProductMaster/DigitalAvailableStoreItem failed! => Unable to fetch the ProductDetails . | Request# {requestBody} | Response# " + ex.Message);
                return new BadRequestObjectResult(new { error = ex.Message });
            }
        }
        #endregion
    }
}
