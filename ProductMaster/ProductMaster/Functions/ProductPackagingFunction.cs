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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProductMaster
{
    /// <summary>
    /// ProductPackagingFunction class
    /// </summary>
    public class ProductPackagingFunction : BaseFunctions
    {
        #region Private Variables
        private readonly IProductPackagingAdapter _dataAdapter;
        #endregion

        #region Constructor
        /// <summary>
        /// ProductFunction contructor
        /// </summary>
        /// <param name="productAdapter"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        public ProductPackagingFunction(IProductPackagingAdapter productAdapter, ILoggerAdapter logger, IConfiguration config) : base(logger, config)
        {
            _dataAdapter = productAdapter;
        }
        #endregion

        #region Functions

        /// <summary>
        /// This Function fetches ProductPackaging details
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("ProductPackaging")]
        public async Task<IActionResult> ProductPackaging(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ProductPackaging")]
            [RequestBodyType(typeof(string), "Product")]HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            try
            {
                //Validating Input Json
                string errorMessage = InputValidation(requestBody);
                var filterCriteria = JsonConvert.DeserializeObject<FilterCriteria>(requestBody);

                if (errorMessage == string.Empty)
                {
                    if (filterCriteria.mProductId == null && filterCriteria.InnerPackId == null && filterCriteria.ItemCodeId == null)
                        errorMessage = "Please provide atlest mProductId or InnerPackId or ItemCodeId";

                    if (filterCriteria.mProductId != null)
                    {
                        if (filterCriteria.mProductId.Count == 0)
                            errorMessage = "Please provide valid mProductId";
                        else
                        {
                            foreach (string product in filterCriteria.mProductId)
                            {
                                if (product == null || !Regex.IsMatch(product, @"^\d+$"))
                                {
                                    errorMessage = "Please provide valid mProductId";
                                    break;
                                }
                            }
                        }
                    }
                    if (filterCriteria.InnerPackId != null && filterCriteria.InnerPackId.Count == 0)
                        errorMessage = "Please provide valid InnerPackId";

                    if (filterCriteria.ItemCodeId != null)
                    {
                        if (filterCriteria.ItemCodeId.Count == 0)
                            errorMessage = "Please provide valid ItemCodeId";
                        else
                        {
                            foreach (string item in filterCriteria.ItemCodeId)
                            {
                                if (item == null || !Regex.IsMatch(item, @"^\d+$"))
                                {
                                    errorMessage = "Please provide valid ItemCodeId";
                                    break;
                                }
                            }
                        }
                    }
                    if (filterCriteria.CasePackId != null)
                    {
                        if (filterCriteria.CasePackId.Count == 0)
                            errorMessage = "Please provide valid CasePackId";
                        else
                        {
                            foreach (string item in filterCriteria.CasePackId)
                            {
                                if (string.IsNullOrEmpty(item))
                                {
                                    errorMessage = "Please provide valid CasePackId";
                                    break;
                                }
                            }
                        }
                    }
                    if (filterCriteria.ItemSku != null)
                    {
                        if (filterCriteria.ItemSku.Count == 0)
                            errorMessage = "Please provide valid ItemSKU";
                        else
                        {
                            foreach (string item in filterCriteria.ItemSku)
                            {
                                if (item == null || !Regex.IsMatch(item, @"^\d+$"))
                                {
                                    errorMessage = "Please provide valid ItemSKU";
                                    break;
                                }
                            }
                        }
                    }
                    if (filterCriteria.mVendorID != null)
                    {
                        if (filterCriteria.mVendorID.Count == 0)
                            errorMessage = "Please provide valid mVendorID";
                        else
                        {
                            foreach (string item in filterCriteria.mVendorID)
                            {
                                if (item == null || !Regex.IsMatch(item, @"^\d+$"))
                                {
                                    errorMessage = "Please provide valid mVendorID";
                                    break;
                                }
                            }
                        }
                    }
                }

                if (errorMessage != string.Empty)
                {
                    if (_logger != null)
                        _logger.LogEvent("HttpsResponse : ProductMaster/ProductPackaging BadRequest :" + errorMessage, requestBody);
                    return new BadRequestObjectResult(new { error = errorMessage });
                }

                string token = req.Headers.ContainsKey("Token") ? EncodingForBase64.DecodeBase64(req.Headers["Token"]) : null;

                //Log Httprequest
                if (_logger != null)
                    _logger.LogEvent("HttpsRequest : ProductMaster/ProductPackaging Calling", JsonConvert.SerializeObject(filterCriteria));


                var result = await _dataAdapter.GetProductPackaging(filterCriteria, token);


                //Log HttpsResponse
                if (_logger != null)
                    _logger.LogEvent("HttpsResponse : ProductMaster/ProductPackaging Executed :", JsonConvert.SerializeObject(result));
                if (_logger != null)
                    _logger.LogInformation("HttpsResponse : ProductMaster/ProductPackaging Executed :", JsonConvert.SerializeObject(result));

                #region Calling Daynamic JSON method
                if (result != null && result.Count == 0)
                {
                    return new NoContentResult();
                }
                else
                {
                    var output = DynamicMapping(result);
                    return new OkObjectResult(output);
                }
                #endregion
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, $"Custom Message# ProductMaster/ProductPackaging failed! => Unable to fetch the ProductPackagingDetails . | Request#  {requestBody} | Response# " + ex.Message);
                return new BadRequestObjectResult(new { error = ex.Message });
            }
        }
        #endregion
    }
}
