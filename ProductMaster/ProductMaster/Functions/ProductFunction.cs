using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProductMaster.Data;
using ProductMaster.Functions.Base;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace ProductMaster
{
    /// <summary>
    /// ProductFunction class
    /// </summary>
    public class ProductFunction : BaseFunctions
    {
        #region Private Variables
        private readonly IProductAdapter _dataAdapter;
        #endregion

        #region Constructor
        /// <summary>
        /// ProductFunction contructor
        /// </summary>
        /// <param name="productAdapter"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        public ProductFunction(IProductAdapter productAdapter, ILoggerAdapter logger, IConfiguration config) : base(logger, config)
        {
            _dataAdapter = productAdapter;
        }
        #endregion

        #region Functions

        /// <summary>
        /// This Function fetches Product details
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("Product")]
        public async Task<IActionResult> Product(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Product")]
            [RequestBodyType(typeof(string), "Product")]HttpRequest req)
        {

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            try
            {
                //Validating Input Json
                string errorMessage = InputValidation(requestBody);

                JsonLoadSettings settings = new JsonLoadSettings { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error };
                JsonTextReader reader = new JsonTextReader(new StringReader(requestBody));
                JToken.ReadFrom(reader, settings);

                //Deserializing FilterCriteria
                var filterCriteria = JsonConvert.DeserializeObject<FilterCriteria>(requestBody);

                if (errorMessage == string.Empty)
                {
                    if (filterCriteria.Purpose == null || !(filterCriteria.Purpose.Trim().ToLower() == "product details"
                        || filterCriteria.Purpose.Trim().ToLower() == "all"))
                    {
                        errorMessage = "Please provide Valid Purpose either 'All' or 'Product Details'";
                    }
                    else if ((filterCriteria.ItemSku != null && filterCriteria.mProductId != null && filterCriteria.mRelatedProductGroupId != null
                            && filterCriteria.LegacyPrimaryKeyId > 0)
                            || (filterCriteria.mProductId == null && filterCriteria.ItemSku == null && filterCriteria.mRelatedProductGroupId == null
                            && filterCriteria.LegacyPrimaryKeyId <= 0))
                    {
                        errorMessage = "Please provide valid mProductId or ItemSku or mRelatedProductGroupId or LegacyPrimaryKeyId.";
                    }
                    else if (filterCriteria.Purpose.Trim().ToLower() == "product details")
                    {
                        if (filterCriteria.ItemSku != null || filterCriteria.UPCTypeName != null)
                            errorMessage = "API does not support ItemSku/UPCTypeName combination with purpose product details.";

                        else if (filterCriteria.mRelatedProductGroupId != null && filterCriteria.mRelatedProductGroupId.Count > 0)
                            errorMessage = "API does not support mRelatedProductGroupId combination with purpose product details";

                        else if (filterCriteria.LegacyPrimaryKeyId > 0)
                        {
                            if ((filterCriteria.mProductId != null && filterCriteria.mProductId.Count > 0)
                                || (filterCriteria.ItemSku != null && filterCriteria.ItemSku.Count > 0)
                                || (filterCriteria.mRelatedProductGroupId != null && filterCriteria.mRelatedProductGroupId.Count > 0))
                                errorMessage = "Please provide either a valid mProductId or ItemSku or mRelatedProductGroupId or LegacyPrimaryKeyId";
                            else if (filterCriteria.LegacyPrimaryKeyId <= 0)
                                errorMessage = "Please provide greater than zero value for LegacyPrimaryKeyId";
                        }

                        else if (filterCriteria.mProductId == null)
                        {
                            if (filterCriteria.mProductId == null && (filterCriteria.LegacyPrimaryKeyId == 0 || filterCriteria.LegacyPrimaryKeyId < 0))
                                errorMessage = "Please provide either valid mProductId or LegacyPrimaryKeyId";
                            if (filterCriteria.mProductId == null && filterCriteria.mRelatedProductGroupId != null
                                && (filterCriteria.LegacyPrimaryKeyId == 0 || filterCriteria.LegacyPrimaryKeyId < 0 || filterCriteria.LegacyPrimaryKeyId > 0))
                                errorMessage = "API does not support mRelatedProductGroupId combinations with purpose Product Details";
                        }

                        else if (filterCriteria.ProductStatusTypeName != null && filterCriteria.ProductStatusTypeName.Equals(""))
                            errorMessage = "Please provide valid ProductStatusTypeName data";

                        else if (filterCriteria.BrandCategoryId != null && filterCriteria.BrandCategoryId.Equals(""))
                            errorMessage = "Please provide valid BrandCategoryId data";

                        else if (filterCriteria.VendorId != 0 && filterCriteria.VendorId < 0)
                            errorMessage = "Please provide valid VendorId data";

                        else
                        {
                            if (filterCriteria.mProductId != null && filterCriteria.mProductId.Count > 0)
                            {
                                for (int i = 0; i < filterCriteria.mProductId.Count; i++)
                                {
                                    if (filterCriteria.mProductId[i] == null || filterCriteria.mProductId[i].Equals(""))
                                        errorMessage = "Please provide valid mProductId data";
                                }
                            }
                        }
                    }
                    else if (filterCriteria.Purpose.Trim().ToLower() == "all")
                    {
                        if (filterCriteria.ItemSku != null)
                        {
                            if (filterCriteria.ItemSku.Count == 0)
                                errorMessage = "Please provide valid ItemSku data";
                            else if (filterCriteria.ItemSku.Count > 0 && (filterCriteria.mProductId != null && filterCriteria.mProductId.Count > 0))
                                errorMessage = "Please provide either mProductId or ItemSku";
                            else
                            {
                                for (int i = 0; i < filterCriteria.ItemSku.Count; i++)
                                {
                                    if (filterCriteria.ItemSku[i] == null || filterCriteria.ItemSku[i].Equals(""))
                                        errorMessage = "Please provide valid ItemSku data";
                                }
                            }

                        }

                        //added search criteria mRelatedProductGroupId on 13-9-2022..
                        if (filterCriteria.mRelatedProductGroupId != null)
                        {
                            var mRelatedProductGrpValue = filterCriteria.mRelatedProductGroupId.Any(x => x.ToString().ToLower().StartsWith("l2-")
                                || x.ToString().ToLower().StartsWith("l3-") || x.ToString().ToLower().StartsWith("l4-"));

                            if (mRelatedProductGrpValue == true)
                            {
                                if (filterCriteria.mRelatedProductGroupId.Count > 1)
                                    errorMessage = "Multiple mRelatedProductGroupId search is not valid.Please provide single value.";

                                else if (filterCriteria.mRelatedProductGroupId.Count == 0)
                                    errorMessage = "Please provide atleast one mRelatedProductGroupId for search";

                                else if ((filterCriteria.mProductId != null && filterCriteria.mProductId.Count > 0)
                                    || (filterCriteria.ItemSku != null && filterCriteria.ItemSku.Count > 0)
                                    || (filterCriteria.LegacyPrimaryKeyId > 0 || filterCriteria.LegacyPrimaryKeyId < 0))
                                    errorMessage = "API does not support mRelatedProductGroupId combination of mProductId or ItemSku or LegacyPrimaryKeyId with purpose All";

                                else
                                {
                                    for (int i = 0; i < filterCriteria.mRelatedProductGroupId.Count; i++)
                                    {
                                        if (filterCriteria.mRelatedProductGroupId[i] == null || filterCriteria.mRelatedProductGroupId[i].Equals(""))
                                            errorMessage = "Please provide valid mRelatedProductGroupId";
                                        if (filterCriteria.mRelatedProductGroupId[i] != null &&
                                            (filterCriteria.mRelatedProductGroupId[i].ToString().ToLower().Contains("l4") || filterCriteria.mRelatedProductGroupId[i].ToString().ToLower().Contains("l5") || filterCriteria.mRelatedProductGroupId[i].ToString().ToLower().Contains("l6")))
                                            errorMessage = "Please provide mRelatedProductGroupLevels from L2 to L3";
                                    }
                                }
                            }
                            else
                                errorMessage = "Please provide valid mRelatedProductGroupId";

                        }

                        if (filterCriteria.LegacyPrimaryKeyId > 0)
                        {
                            if ((filterCriteria.mProductId != null && filterCriteria.mProductId.Count > 0)
                                || (filterCriteria.ItemSku != null && filterCriteria.ItemSku.Count > 0)
                                || (filterCriteria.mRelatedProductGroupId != null && filterCriteria.mRelatedProductGroupId.Count > 0))
                                errorMessage = "Please provide either a valid mProductId or ItemSku or mRelatedProductGroupId or LegacyPrimaryKeyId";
                            else if (filterCriteria.LegacyPrimaryKeyId <= 0)
                                errorMessage = "Please provide greater than zero value for LegacyPrimaryKeyId";
                        }

                        if (filterCriteria.mProductId != null)
                        {
                            if (filterCriteria.mProductId.Count == 0)
                                errorMessage = "Please provide atleast one mProductId for search";

                            for (int i = 0; i < filterCriteria.mProductId.Count; i++)
                            {
                                if (filterCriteria.mProductId[i] == null || filterCriteria.mProductId[i].Equals(""))
                                    errorMessage = "Please provide valid mProductId data";
                            }
                        }

                        if (filterCriteria.ProductStatusTypeName != null && filterCriteria.ProductStatusTypeName.Equals(""))
                            errorMessage = "Please provide valid ProductStatusTypeName data";

                        if (filterCriteria.BrandCategoryId != null && filterCriteria.BrandCategoryId.Equals(""))
                            errorMessage = "Please provide valid BrandCategoryId data";

                        if (filterCriteria.VendorId != 0 && filterCriteria.VendorId < 0)
                            errorMessage = "Please provide valid VendorId data";

                        if (filterCriteria.UPCTypeName != null)
                        {
                            for (int i = 0; i < filterCriteria.UPCTypeName.Count; i++)
                            {
                                if (filterCriteria.UPCTypeName[i] == null || filterCriteria.UPCTypeName[i].Equals(""))
                                    errorMessage = "Please provide valid UPCTypeName data";
                            }
                        }
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
                    _logger.LogEvent("HttpsRequest : ProductMaster/Product Calling", JsonConvert.SerializeObject(filterCriteria));

                string token = req.Headers.ContainsKey("Token") ? EncodingForBase64.DecodeBase64(req.Headers["Token"]) : null;
                var result = await _dataAdapter.GetProduct(filterCriteria, token);

                //Log HttpsResponse
                if (_logger != null)
                    _logger.LogEvent("HttpsResponse : ProductMaster/Product Executed :", JsonConvert.SerializeObject(result));
                if (_logger != null)
                    _logger.LogInformation("HttpsResponse : ProductMaster/Product Executed :", JsonConvert.SerializeObject(result));


                #region Calling Dynamic JSON method
                var output = DynamicMapping(result);
                #endregion

                return new OkObjectResult(output);

            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, $"Custom Message# ProductMaster/Product failed! => Unable to fetch the ProductDetails . | Request#  {requestBody} | Response# " + ex.Message);
                return new BadRequestObjectResult(new { error = ex.Message });
            }
        }
        #endregion
    }
}
