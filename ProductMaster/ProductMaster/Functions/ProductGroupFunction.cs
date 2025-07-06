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
using System.Linq;
using System.Threading.Tasks;

namespace ProductMaster
{
    /// <summary>
    /// ProductGroupFunction class
    /// </summary>
    public class ProductGroupFunction : BaseFunctions
    {
        #region Private Variables
        private readonly IProductGroupAdapter _productGroupAdapter;
        #endregion

        #region Constructor
        /// <summary>
        /// ProductGroupFunction contructor
        /// </summary>
        /// <param name="productGroupAdapter"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        public ProductGroupFunction(IProductGroupAdapter productGroupAdapter, ILoggerAdapter logger, IConfiguration config) : base(logger, config)
        {
            _productGroupAdapter = productGroupAdapter;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Fetches ProductGroup details
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("ProductGroup")]
        public async Task<IActionResult> ProductGroup(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ProductGroup")]
            [RequestBodyType(typeof(string), "ProductGroupId")]HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            try
            {
                //Validating Input Json
                string errorMessage = InputValidation(requestBody);

                var filterCriteria = new FilterCriteria();
                filterCriteria = JsonConvert.DeserializeObject<FilterCriteria>(requestBody);
                if (errorMessage == string.Empty)
                {
                    if (filterCriteria.mProductGroupId != null || filterCriteria.LegacyPrimaryKeyId != 0 || filterCriteria.mRelatedProductGroupId != null)
                    {
                        if (filterCriteria.LegacyPrimaryKeyId != 0 && filterCriteria.mRelatedProductGroupId != null && filterCriteria.mProductGroupId != null)
                            errorMessage = "Please provide mProductGroupId or LegacyPrimaryKeyId or mRelatedProductGroupId";
                        else if (filterCriteria.LegacyPrimaryKeyId != 0 && filterCriteria.mRelatedProductGroupId != null)
                            errorMessage = "Please provide either LegacyPrimaryKeyId or mRelatedProductGroupId";
                        else if (filterCriteria.LegacyPrimaryKeyId != 0 && filterCriteria.mProductGroupId != null)
                            errorMessage = "Please provide either LegacyPrimaryKeyId or mProductGroupId";
                        else if (filterCriteria.mProductGroupId != null && filterCriteria.mRelatedProductGroupId != null)
                            errorMessage = "Please provide either mProductGroupId or mRelatedProductGroupId";
                        else if (filterCriteria.mProductGroupId != null && filterCriteria.LegacyPrimaryKeyId != 0 && filterCriteria.mRelatedProductGroupId != null)
                            errorMessage = "Please provide either mProductGroupId or LegacyPrimaryKeyId or mRelatedProductGroupId";
                        else if (filterCriteria.mRelatedProductGroupId != null && filterCriteria.Purpose == null)
                            errorMessage = "Please provide Purpose for mRelatedProductGroupId";
                        else if (filterCriteria.mRelatedProductGroupId == null && filterCriteria.Purpose != null)
                            errorMessage = "Purpose only can use with mRelatedproductGroupId ";
                        else if (filterCriteria.mRelatedProductGroupId != null && filterCriteria.Purpose != null)
                        {
                            if (!(filterCriteria.Purpose.Trim().ToLower() == "all hierarchy" || filterCriteria.Purpose.Trim().ToLower() == "product details"))
                                errorMessage = "Purpose can be either All Hierarchy or Product Details";

                            if (filterCriteria.mRelatedProductGroupId.Count > 1 && filterCriteria.Purpose.Trim().ToLower() == "all hierarchy")
                                errorMessage = "API does not support bulk mRelatedProductGroupId combination with purpose all hierarchy. Please provide single mRelatedProductGroupId.";

                            if (filterCriteria.mRelatedProductGroupId.Count == 0)
                                errorMessage = "Please provide valid JSON data";

                            for (int i = 0; i < filterCriteria.mRelatedProductGroupId.Count; i++)
                            {
                                if (filterCriteria.mRelatedProductGroupId[i] == null || filterCriteria.mRelatedProductGroupId[i].Equals(""))
                                    errorMessage = "Please provide valid JSON data";
                            }

                        }
                        else if (filterCriteria.mProductGroupId != null && filterCriteria.LegacyPrimaryKeyId == 0 && filterCriteria.mRelatedProductGroupId == null)
                        {
                            if (filterCriteria.mProductGroupId.Count == 0)
                                errorMessage = "Please provide valid JSON data";

                            for (int i = 0; i < filterCriteria.mProductGroupId.Count; i++)
                            {
                                if (filterCriteria.mProductGroupId[i] == null || filterCriteria.mProductGroupId[i].Equals("") || !(filterCriteria.mProductGroupId[i].All(char.IsNumber)))
                                    errorMessage = "Please provide valid JSON data";
                            }

                        }
                        else
                            errorMessage = "";
                        /*else if (filterCriteria.LegacyPrimaryKeyId != null && filterCriteria.mRelatedProductGroupId == null && filterCriteria.mProductGroupId == null)
                        {
                            if (filterCriteria.LegacyPrimaryKeyId.Count > 1)
                                errorMessage = "API does not support bulk LegacyPrimaryKeyId combination. Please provide single LegacyPrimaryKeyId.";

                            for (int i = 0; i < filterCriteria.LegacyPrimaryKeyId.Count; i++)
                            {
                                if (filterCriteria.LegacyPrimaryKeyId[i] == null || filterCriteria.LegacyPrimaryKeyId[i].Equals(""))
                                    errorMessage = "Please provide valid JSON data";
                            }

                            if (filterCriteria.LegacyPrimaryKeyId.Count == 0)
                                errorMessage = "Please provide valid JSON data";

                            foreach (var item in filterCriteria.LegacyPrimaryKeyId)
                            {
                                bool isIsDigit = item.All(Char.IsNumber);
                                if (!isIsDigit)
                                    errorMessage = "Please provide valid JSON data";
                            }
                        }*/

                    }
                    else
                    {
                        errorMessage = "Please provide one Valid criteria";
                    }
                }
                //ErrorMessage
                if (errorMessage != string.Empty)
                {
                    if (_logger != null)
                        _logger.LogEvent("HttpsResponse : ProductMaster/ProductGroup BadRequest :" + errorMessage, requestBody);
                    return new BadRequestObjectResult(new { error = errorMessage });
                }

                //Log Httprequest
                if (_logger != null)
                    _logger.LogEvent("HttpsRequest : ProductMaster/ProductGroup Calling", JsonConvert.SerializeObject(filterCriteria));


                string token = GetTokeFromHeader(req);
                var result = await _productGroupAdapter.GetProductGroupId(filterCriteria, token);


                //Log HttpsResponse
                if (_logger != null)
                    _logger.LogEvent("HttpsResponse : ProductMaster/ProductGroup Executed :", JsonConvert.SerializeObject(result));
                if (_logger != null)
                    _logger.LogInformation("HttpsResponse : ProductMaster/ProductGroup Executed :", JsonConvert.SerializeObject(result));


                #region Calling Daynamic JSON method
                var output = DynamicMapping(result);
                #endregion

                return new OkObjectResult(output);

            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# ProductMaster/ProductGroup failed! => Unable to fetch the ProductDetails . | Request# " + requestBody + "| Response# " + ex.Message);
                return new BadRequestObjectResult(new { error = ex.Message });
            }
        }
        #endregion
    }
}
