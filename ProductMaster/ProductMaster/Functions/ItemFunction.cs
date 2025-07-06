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
    /// Item Function class
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
        /// This Function fetches Item details
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("Item")]
        public async Task<IActionResult> Item(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Item")]
            [RequestBodyType(typeof(string), "ItemSku")]HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            try
            {
                //Validating Input Json
                string errorMessage = InputValidation(requestBody);
                if (errorMessage != string.Empty)
                {
                    if (_logger != null)
                        _logger.LogEvent("HttpsResponse : ProductMaster/Item BadRequest :" + errorMessage, requestBody);
                    return new BadRequestObjectResult(new { error = errorMessage });
                }


                //Serializing Input json
                var filterCriteria = JsonConvert.DeserializeObject<FilterCriteria>(requestBody);

                //if (filterCriteria.ItemSku == null && filterCriteria.RowUpdatedStartDate == DateTime.MinValue && filterCriteria.UPCTypeName == null)
                //{
                //    errorMessage = "Please provide atleast one valid criteria";
                //    if (_logger != null)
                //        _logger.LogEvent("HttpsResponse : ProductMaster/Item BadRequest :" + errorMessage, requestBody);
                //    return new BadRequestObjectResult(new { error = errorMessage });
                //}

                //Log Httprequest
                if (_logger != null)
                    _logger.LogEvent("HttpsRequest : ProductMaster/Item Calling", JsonConvert.SerializeObject(filterCriteria));

                //Reading Headers if any
                string token = GetTokeFromHeader(req);

                //Calling adapter class
                var result = await _dataAdapter.GetDetails(filterCriteria, token);


                //Log HttpsResponse
                if (_logger != null)
                    _logger.LogEvent("HttpsResponse : ProductMaster/Item Executed :", JsonConvert.SerializeObject(result));
                if (_logger != null)
                    _logger.LogInformation("HttpsResponse : ProductMaster/Item Executed :", JsonConvert.SerializeObject(result));


                return new OkObjectResult(DynamicMapping(result));
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# ProductMaster/Item : Failed! => Unable to fetch the Product details. | Request# " + JsonConvert.SerializeObject(requestBody) + "| Response# " + ex.Message);
                return new BadRequestObjectResult(new { error = ex.Message });
            }
        }
        #endregion
    }
}
