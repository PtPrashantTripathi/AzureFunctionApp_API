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
    public class DigitalCouponFunction : BaseFunctions
    {
        #region Private Variables
        private readonly IDigitalCouponAdapter _dataAdapter;
        #endregion

        #region Constructor
        /// <summary>
        /// DigitalCouponFunction contructor
        /// </summary>
        /// <param name="digitalCouponAdapter"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        public DigitalCouponFunction(IDigitalCouponAdapter digitalCouponAdapter, ILoggerAdapter logger, IConfiguration config) : base(logger, config)
        {
            this._dataAdapter = digitalCouponAdapter;
        }
        #endregion

        #region FunctionsMethod
        /// <summary>
        /// This Function fetches DigitalCoupon
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("DigitalCoupon")]
        public async Task<IActionResult> DigitalCoupon(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "DigitalCoupon")]
            [RequestBodyType(typeof(string), "mCouponId")]HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            try
            {
                //Validating Input Json
                string errorMessage = InputValidation(requestBody);
                if (errorMessage != string.Empty)
                {
                    if (_logger != null)
                        _logger.LogEvent("HttpsResponse : ProductMaster/DigitalCoupon BadRequest :" + errorMessage, requestBody);
                    return new BadRequestObjectResult(new { error = errorMessage });
                }

                var filterCriteria = JsonConvert.DeserializeObject<FilterCriteria>(requestBody);
                if (filterCriteria.mCouponId == null && filterCriteria.CouponSourceId == null)
                {
                    errorMessage = "Please provide atleast one valid criteria";
                    if (_logger != null)
                        _logger.LogEvent("HttpsResponse : ProductMaster/DigitalCoupon BadRequest :" + errorMessage, requestBody);
                    return new BadRequestObjectResult(new { error = errorMessage });
                }

                //Log Httprequest
                if (_logger != null)
                    _logger.LogEvent("HttpsRequest : ProductMaster/DigitalCoupon Calling", JsonConvert.SerializeObject(filterCriteria));

                string token = GetTokeFromHeader(req);
                var result = await _dataAdapter.GetCouponDetails(filterCriteria, token);


                //Log HttpsResponse
                if (_logger != null)
                    _logger.LogEvent("HttpsResponse : ProductMaster/DigitalCoupon Executed :", JsonConvert.SerializeObject(result));
                if (_logger != null)
                    _logger.LogInformation("HttpsResponse : ProductMaster/DigitalCoupon Executed :", JsonConvert.SerializeObject(result));


                #region Calling Daynamic JSON method
                var output = DynamicMapping(result);
                #endregion

                return new OkObjectResult(output);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, $"Custom Message# ProductMaster/DigitalCoupon : Failed! => Unable to fetch the Product details. | Request# {JsonConvert.SerializeObject(requestBody)}| Response# " + ex.Message);
                return new BadRequestObjectResult(new { error = ex.Message });
            }
        }
        #endregion
    }
}
