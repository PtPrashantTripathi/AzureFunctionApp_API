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
    ///
    /// </summary>
    public class DigitalCouponItemFunction : BaseFunctions
    {
        #region Private Variables
        private readonly IDigitalCouponItemAdapter _dataAdapter;
        #endregion

        #region Constructor
        /// <summary>
        /// ProductFunction contructor
        /// </summary>
        /// <param name="digitalcoupon"></param>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public DigitalCouponItemFunction(IDigitalCouponItemAdapter digitalcoupon, IConfiguration config, ILoggerAdapter logger) : base(logger, config)
        {
            _dataAdapter = digitalcoupon;
        }
        #endregion

        #region Functions

        /// <summary>
        /// This Function fetches Product details
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("DigitalCouponItem")]
        public async Task<IActionResult> DigitalCouponItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "DigitalCouponItem")]
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
                        _logger.LogEvent("HttpsResponse : ProductMaster/Item BadRequest :" + errorMessage, requestBody);
                    return new BadRequestObjectResult(new { error = errorMessage });
                }

                var filterCriteria = JsonConvert.DeserializeObject<FilterCriteria>(requestBody);

                if (filterCriteria.ItemSku == null && (filterCriteria.CouponSourceId == null || filterCriteria.mCouponId == null))
                {
                    errorMessage = "Please provide valid criteria";
                    if (_logger != null)
                        _logger.LogEvent("HttpsResponse : ProductMaster/DigitalCouponItem BadRequest :" + errorMessage, requestBody);
                    return new BadRequestObjectResult(new { error = errorMessage });
                }

                //Log Httprequest
                if (_logger != null)
                    _logger.LogEvent("HttpsRequest : ProductMaster/DigitalCouponItem Calling", JsonConvert.SerializeObject(filterCriteria));

                //Reading Headers if any
                string token = GetTokeFromHeader(req);

                var result = await _dataAdapter.GetDigitalCouponItem(filterCriteria, token);


                //Log HttpsResponse
                if (_logger != null)
                    _logger.LogEvent("HttpsResponse : ProductMaster/DigitalCouponItem Executed :", JsonConvert.SerializeObject(result));
                if (_logger != null)
                    _logger.LogInformation("HttpsResponse : ProductMaster/DigitalCouponItem Executed :", JsonConvert.SerializeObject(result));


                #region Calling Daynamic JSON method
                var output = DynamicMapping(result);
                #endregion

                return new OkObjectResult(output);

            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# ProductMaster/DigitalCouponItem failed! => Unable to fetch the ProductDetails . | Request# " + requestBody + "| Response# " + ex.Message);
                return new BadRequestObjectResult(new { error = ex.Message });
            }
        }
        #endregion
    }
}
