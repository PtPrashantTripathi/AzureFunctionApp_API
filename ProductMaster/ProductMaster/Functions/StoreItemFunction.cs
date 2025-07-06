using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ProductMaster.Data;
using ProductMaster.Functions.Base;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProductMaster
{
    /// <summary>
    /// SotreItem Function class
    /// </summary>
    public class StoreItemFunction : BaseFunctions
    {
        #region Private Variables
        private readonly IStoreItemAdapter _dataAdapter;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storeItem"></param>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public StoreItemFunction(IStoreItemAdapter storeItem, ILoggerAdapter logger, IConfiguration config) : base(logger, config)
        {
            _dataAdapter = storeItem;
        }

        /// <summary>
        /// This Function fetches StoreItem details
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("StoreItem")]
        public async Task<HttpResponseMessage> StoreItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "StoreItem")]
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
                        _logger.LogEvent($"HttpsResponse : ProductMaster/StoreItem BadRequest :{errorMessage}", requestBody);

                    return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(errorMessage, Encoding.UTF8)
                    };
                }

                //Validating Empty Array
                string requestJson = requestBody.Replace("\n", string.Empty).Trim();
                if (requestJson.Contains("-") || requestJson.Contains("."))
                {
                    errorMessage = "Please provide valid JSON data";
                    if (_logger != null)
                        _logger.LogEvent($"HttpsResponse : ProductMaster/StoreItem BadRequest :{errorMessage}", requestBody);
                    return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(errorMessage, Encoding.UTF8)
                    };
                }
                var filterCriteria = JsonConvert.DeserializeObject<FilterCriteria>(requestBody);

                //both values are not Present
                if (filterCriteria.ItemSku == null && filterCriteria.StoreId == null)
                {
                    errorMessage = "Please provide a valid ItemSku or StoreID";
                    if (_logger != null)
                        _logger.LogEvent($"HttpsResponse : ProductMaster/StoreItem BadRequest :{errorMessage}", requestBody);
                    return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(errorMessage, Encoding.UTF8)
                    };
                }
                //both values are present
                if (filterCriteria.ItemSku != null && filterCriteria.StoreId != null)
                {
                    //any one of them haveing empty input
                    if (filterCriteria.ItemSku.Count == 0 || filterCriteria.StoreId.Count == 0)
                    {
                        errorMessage = "Please provide valid JSON data";
                        if (_logger != null)
                            _logger.LogEvent($"HttpsResponse : ProductMaster/StoreItem BadRequest :{errorMessage}", requestBody);
                        return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent(errorMessage, Encoding.UTF8)
                        };
                    }
                    else if (filterCriteria.StoreId.Count > 1 && filterCriteria.ItemSku.Count > 1)
                    {
                        errorMessage = "API does not support bulk StoreId and bulk ItemSKU combination.Please provide single StoreId and bulk ItemSKU or single ItemSKU and bulk StoreId combination.";
                        if (_logger != null)
                            _logger.LogEvent($"HttpsResponse : ProductMaster/StoreItem BadRequest :{errorMessage}", requestBody);

                        return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent(errorMessage, Encoding.UTF8)
                        };
                    }
                }

                //only item values present
                if (filterCriteria.ItemSku != null && filterCriteria.StoreId == null)
                {
                    // having empty value
                    if (filterCriteria.ItemSku.Count == 0)
                    {
                        errorMessage = "Please provide a valid ItemSku";
                        if (_logger != null)
                            _logger.LogEvent($"HttpsResponse : ProductMaster/StoreItem BadRequest :{errorMessage}", requestBody);
                        return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent(errorMessage, Encoding.UTF8)
                        };
                    }
                    //more then one
                    if (filterCriteria.ItemSku.Count > 1)
                    {
                        errorMessage = "Please provide only one ItemSku";
                        if (_logger != null)
                            _logger.LogEvent($"HttpsResponse : ProductMaster/StoreItem BadRequest :{errorMessage}", requestBody);
                        return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent(errorMessage, Encoding.UTF8)
                        };
                    }
                }

                //only store values present
                if (filterCriteria.StoreId != null && filterCriteria.ItemSku == null)
                {
                    //having empty value
                    if (filterCriteria.StoreId.Count == 0)
                    {
                        errorMessage = "Please provide a valid StoreID";
                        if (_logger != null)
                            _logger.LogEvent($"HttpsResponse : ProductMaster/StoreItem BadRequest :{errorMessage}", requestBody);
                        return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent(errorMessage, Encoding.UTF8)
                        };
                    }
                    if (filterCriteria.StoreId.Count == 1 && filterCriteria.StoreId[0] == null)
                    {
                        errorMessage = "Please provide a valid StoreID";
                        if (_logger != null)
                            _logger.LogEvent($"HttpsResponse : ProductMaster/StoreItem BadRequest :{errorMessage}", requestBody);
                        return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent(errorMessage, Encoding.UTF8)
                        };
                    }
                }

                //Log Httprequest
                if (_logger != null)
                    _logger.LogEvent("HttpsRequest : ProductMaster/StoreItem Calling", JsonConvert.SerializeObject(filterCriteria));

                //Reading Headers if any
                string token = GetTokeFromHeader(req);
                var result = await _dataAdapter.GetStoreItemDetails(filterCriteria, token);

                #region Calling Daynamic JSON method
                var output = DynamicMapping(result);
                #endregion

                //Log HttpsResponse
                if (_logger != null)
                    _logger.LogEvent("HttpsResponse : ProductMaster/StoreItem Executed :", JsonConvert.SerializeObject(result));
                if (_logger != null)
                    _logger.LogInformation("HttpsResponse : ProductMaster/StoreItem Executed :", JsonConvert.SerializeObject(result));

                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(output, Formatting.Indented), Encoding.UTF8)
                };

            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, $"Unable to fetch the ProductMaster/StoreItem seen exception  {ex.Message}");
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
