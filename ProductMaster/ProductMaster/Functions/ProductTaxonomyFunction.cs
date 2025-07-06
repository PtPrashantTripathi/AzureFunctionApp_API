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
    /// ProductTaxonomyFunction class
    /// </summary>
    public class ProductTaxonomyFunction : BaseFunctions
    {
        #region Private Variables
        private readonly IProductTaxonomyAdapter _dataAdapter;

        #region Constructor
        /// <summary>
        /// UpcDetails Function contructor
        /// </summary>
        /// <param name="producttaxonomyadapter"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        public ProductTaxonomyFunction(IProductTaxonomyAdapter producttaxonomyadapter, ILoggerAdapter logger, IConfiguration config) : base(logger, config)
        {
            this._dataAdapter = producttaxonomyadapter;
        }
        #endregion

        /// <summary>
        /// Fetches ProductTaxonomy data
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("ProductTaxonomy")]
        public async Task<IActionResult> ProductTaxonomy(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ProductTaxonomy")]
             [RequestBodyType(typeof(string), "ProductId")] HttpRequest req)
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
                if (filterCriteria.mProductId == null && filterCriteria.mProductTaxonomyID == null)
                {
                    errorMessage = "Please provide atleast one valid criteria";
                    if (_logger != null)
                        _logger.LogEvent("HttpsResponse : ProductMaster/GetProductTaxonomy BadRequest :" + errorMessage, requestBody);
                    return new BadRequestObjectResult(new { error = errorMessage }); ;
                }

                //Log Httprequest
                if (_logger != null)
                    _logger.LogEvent("HttpsRequest : ProductMaster/GetProductTaxonomy Calling", JsonConvert.SerializeObject(filterCriteria));

                //Reading Headers if any
                string token = GetTokeFromHeader(req);

                var result = await _dataAdapter.GetProductTaxonomy(filterCriteria, token);


                //Log HttpsResponse
                if (_logger != null)
                    _logger.LogEvent("HttpsResponse : ProductMaster/GetProductTaxonomy Executed :", JsonConvert.SerializeObject(result));
                if (_logger != null)
                    _logger.LogInformation("HttpsResponse : ProductMaster/GetProductTaxonomy Executed :", JsonConvert.SerializeObject(result));


                #region Calling Daynamic JSON method
                var output = DynamicMapping(result);
                #endregion

                return new OkObjectResult(output);

            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# ProductMaster/GetProductTaxonomy failed! => Unable to fetch the ProductDetails . | Request# " + requestBody + "| Response# " + ex.Message);
                return new BadRequestObjectResult(new { error = ex.Message });
            }
        }
        #endregion
    }
}
