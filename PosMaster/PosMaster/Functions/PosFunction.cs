using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Dynatrace.OpenTelemetry.Instrumentation.AzureFunctions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PosMaster.Data;
using PosMaster.Functions.Base;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace PosMaster
{
    /// <summary>
    /// PosFunction class
    /// </summary>
    public class PosFunction : BaseFunctions
    {
        #region Private Variables
        private readonly IPosAdapter _dataAdapter;
        #endregion

        #region Constructor
        /// <summary>
        /// PosFunction contructor
        /// </summary>
        /// <param name="posAdapter"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        public PosFunction(IPosAdapter posAdapter, ILoggerAdapter logger, IConfiguration config) : base(logger, config)
        {
            _dataAdapter = posAdapter;
        }
        #endregion

        #region Functions

        /// <summary>
        /// This Function fetches Pos details
        /// </summary>
        /// <param name="req"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [FunctionName("Pos")]
        public async Task<IActionResult> Pos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Pos")]
            [RequestBodyType(typeof(string), "Pos")]HttpRequest req, ExecutionContext ctx)
        {
            AzureFunctionsInstrumentation.AddIncomingHttpAzureFunctionCallInfo(Activity.Current, ctx);
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
                    if (filterCriteria.Purpose == null || !(filterCriteria.Purpose.Trim().ToLower() == "pos details"
                        || filterCriteria.Purpose.Trim().ToLower() == "all"))
                    {
                        errorMessage = "Please provide Valid Purpose either 'All' or 'Pos Details'";
                    }
                    else if ((filterCriteria.Sku != null && filterCriteria.mPosId != null && filterCriteria.mRelatedPosGroupId != null
                            && filterCriteria.LegacyPrimaryKeyId > 0)
                            || (filterCriteria.mPosId == null && filterCriteria.Sku == null && filterCriteria.mRelatedPosGroupId == null
                            && filterCriteria.LegacyPrimaryKeyId <= 0))
                    {
                        errorMessage = "Please provide valid mPosId or Sku or mRelatedPosGroupId or LegacyPrimaryKeyId.";
                    }
                    else if (filterCriteria.Purpose.Trim().ToLower() == "pos details")
                    {
                        if (filterCriteria.Sku != null || filterCriteria.UPCTypeName != null)
                            errorMessage = "API does not support Sku/UPCTypeName combination with purpose pos details.";

                        else if (filterCriteria.mRelatedPosGroupId != null && filterCriteria.mRelatedPosGroupId.Count > 0)
                            errorMessage = "API does not support mRelatedPosGroupId combination with purpose pos details";

                        else if (filterCriteria.LegacyPrimaryKeyId > 0)
                        {
                            if ((filterCriteria.mPosId != null && filterCriteria.mPosId.Count > 0)
                                || (filterCriteria.Sku != null && filterCriteria.Sku.Count > 0)
                                || (filterCriteria.mRelatedPosGroupId != null && filterCriteria.mRelatedPosGroupId.Count > 0))
                                errorMessage = "Please provide either a valid mPosId or Sku or mRelatedPosGroupId or LegacyPrimaryKeyId";
                            else if (filterCriteria.LegacyPrimaryKeyId <= 0)
                                errorMessage = "Please provide greater than zero value for LegacyPrimaryKeyId";
                        }

                        else if (filterCriteria.mPosId == null)
                        {
                            if (filterCriteria.mPosId == null && (filterCriteria.LegacyPrimaryKeyId == 0 || filterCriteria.LegacyPrimaryKeyId < 0))
                                errorMessage = "Please provide either valid mPosId or LegacyPrimaryKeyId";
                            if (filterCriteria.mPosId == null && filterCriteria.mRelatedPosGroupId != null
                                && (filterCriteria.LegacyPrimaryKeyId == 0 || filterCriteria.LegacyPrimaryKeyId < 0 || filterCriteria.LegacyPrimaryKeyId > 0))
                                errorMessage = "API does not support mRelatedPosGroupId combinations with purpose Pos Details";
                        }

                        else if (filterCriteria.PosStatusTypeName != null && filterCriteria.PosStatusTypeName.Equals(""))
                            errorMessage = "Please provide valid PosStatusTypeName data";

                        else if (filterCriteria.BrandCategoryId != null && filterCriteria.BrandCategoryId.Equals(""))
                            errorMessage = "Please provide valid BrandCategoryId data";

                        else if (filterCriteria.VendorId != 0 && filterCriteria.VendorId < 0)
                            errorMessage = "Please provide valid VendorId data";

                        else
                        {
                            if (filterCriteria.mPosId != null && filterCriteria.mPosId.Count > 0)
                            {
                                for (int i = 0; i < filterCriteria.mPosId.Count; i++)
                                {
                                    if (filterCriteria.mPosId[i] == null || filterCriteria.mPosId[i].Equals(""))
                                        errorMessage = "Please provide valid mPosId data";
                                }
                            }
                        }
                    }
                    else if (filterCriteria.Purpose.Trim().ToLower() == "all")
                    {
                        if (filterCriteria.Sku != null)
                        {
                            if (filterCriteria.Sku.Count == 0)
                                errorMessage = "Please provide valid Sku data";
                            else if (filterCriteria.Sku.Count > 0 && (filterCriteria.mPosId != null && filterCriteria.mPosId.Count > 0))
                                errorMessage = "Please provide either mPosId or Sku";
                            else
                            {
                                for (int i = 0; i < filterCriteria.Sku.Count; i++)
                                {
                                    if (filterCriteria.Sku[i] == null || filterCriteria.Sku[i].Equals(""))
                                        errorMessage = "Please provide valid Sku data";
                                }
                            }

                        }

                        //added search criteria mRelatedPosGroupId on 13-9-2022..
                        if (filterCriteria.mRelatedPosGroupId != null)
                        {
                            var mRelatedPosGrpValue = filterCriteria.mRelatedPosGroupId.Any(x => x.ToString().ToLower().StartsWith("l2-")
                                || x.ToString().ToLower().StartsWith("l3-") || x.ToString().ToLower().StartsWith("l4-"));

                            if (mRelatedPosGrpValue == true)
                            {
                                if (filterCriteria.mRelatedPosGroupId.Count > 1)
                                    errorMessage = "Multiple mRelatedPosGroupId search is not valid.Please provide single value.";

                                else if (filterCriteria.mRelatedPosGroupId.Count == 0)
                                    errorMessage = "Please provide atleast one mRelatedPosGroupId for search";

                                else if ((filterCriteria.mPosId != null && filterCriteria.mPosId.Count > 0)
                                    || (filterCriteria.Sku != null && filterCriteria.Sku.Count > 0)
                                    || (filterCriteria.LegacyPrimaryKeyId > 0 || filterCriteria.LegacyPrimaryKeyId < 0))
                                    errorMessage = "API does not support mRelatedPosGroupId combination of mPosId or Sku or LegacyPrimaryKeyId with purpose All";

                                else
                                {
                                    for (int i = 0; i < filterCriteria.mRelatedPosGroupId.Count; i++)
                                    {
                                        if (filterCriteria.mRelatedPosGroupId[i] == null || filterCriteria.mRelatedPosGroupId[i].Equals(""))
                                            errorMessage = "Please provide valid mRelatedPosGroupId";
                                        if (filterCriteria.mRelatedPosGroupId[i] != null &&
                                            (filterCriteria.mRelatedPosGroupId[i].ToString().ToLower().Contains("l4") || filterCriteria.mRelatedPosGroupId[i].ToString().ToLower().Contains("l5") || filterCriteria.mRelatedPosGroupId[i].ToString().ToLower().Contains("l6")))
                                            errorMessage = "Please provide mRelatedPosGroupLevels from L2 to L3";
                                    }
                                }
                            }
                            else
                                errorMessage = "Please provide valid mRelatedPosGroupId";

                        }

                        if (filterCriteria.LegacyPrimaryKeyId > 0)
                        {
                            if ((filterCriteria.mPosId != null && filterCriteria.mPosId.Count > 0)
                                || (filterCriteria.Sku != null && filterCriteria.Sku.Count > 0)
                                || (filterCriteria.mRelatedPosGroupId != null && filterCriteria.mRelatedPosGroupId.Count > 0))
                                errorMessage = "Please provide either a valid mPosId or Sku or mRelatedPosGroupId or LegacyPrimaryKeyId";
                            else if (filterCriteria.LegacyPrimaryKeyId <= 0)
                                errorMessage = "Please provide greater than zero value for LegacyPrimaryKeyId";
                        }

                        if (filterCriteria.mPosId != null)
                        {
                            if (filterCriteria.mPosId.Count == 0)
                                errorMessage = "Please provide atleast one mPosId for search";

                            for (int i = 0; i < filterCriteria.mPosId.Count; i++)
                            {
                                if (filterCriteria.mPosId[i] == null || filterCriteria.mPosId[i].Equals(""))
                                    errorMessage = "Please provide valid mPosId data";
                            }
                        }

                        if (filterCriteria.PosStatusTypeName != null && filterCriteria.PosStatusTypeName.Equals(""))
                            errorMessage = "Please provide valid PosStatusTypeName data";

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
                        _logger.LogEvent("HttpsResponse : PosMaster/PosGroup BadRequest :" + errorMessage, requestBody);
                    return new BadRequestObjectResult(new { error = errorMessage });
                }

                //Log Httprequest
                if (_logger != null)
                    _logger.LogEvent("HttpsRequest : PosMaster/Pos Calling", JsonConvert.SerializeObject(filterCriteria));

                string token = req.Headers.ContainsKey("Token") ? EncodingForBase64.DecodeBase64(req.Headers["Token"]) : null;
                var result = await _dataAdapter.GetPos(filterCriteria, token);

                //Log HttpsResponse
                if (_logger != null)
                    _logger.LogEvent("HttpsResponse : PosMaster/Pos Executed :", JsonConvert.SerializeObject(result));
                if (_logger != null)
                    _logger.LogInformation("HttpsResponse : PosMaster/Pos Executed :", JsonConvert.SerializeObject(result));


                #region Calling Dynamic JSON method
                var output = DynamicMapping(result);
                #endregion

                return new OkObjectResult(output);

            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, $"Custom Message# PosMaster/Pos failed! => Unable to fetch the PosDetails . | Request#  {requestBody} | Response# " + ex.Message);
                return new BadRequestObjectResult(new { error = ex.Message });
            }
        }
        #endregion
    }
}
