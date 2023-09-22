using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PosMaster.Data;
using PosMaster.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PosMaster.Functions.Base
{
    /// <summary>
    /// Base class for functions
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class BaseFunctions
    {
        /// <summary>
        /// Configuration object
        /// </summary>
        protected IConfiguration _config;

        /// <summary>
        /// Telemetry client
        /// </summary>
        protected ILoggerAdapter _logger { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public BaseFunctions(ILoggerAdapter logger, IConfiguration config)
        {
            //Configuration object
            _config = config;
            //Telemetry client for AppInsight logging 
            _logger = logger;
        }

        /// <summary>
        /// Check if date is in correct format or not
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        protected bool IsValidDateformat(string date)
        {
            string[] formats = { "yyyy-MM-dd HH:mm:ss.ffffff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd", "yyyy/MM/dd", "yyyy-M-d", "yyyy/M/dd", "yyyy-MM-dd hh:mm:ss.ffffff", "yyyy-MM-dd hh:mm:ss", "yyyy-MM-dd hh:mm" };
            if (!string.IsNullOrEmpty(date) && date != DateTime.MinValue.ToString())
            {
                DateTime parsedDate;
                return DateTime.TryParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
            }
            return true;
        }


        /// <summary>
        /// Check if json is in Valid format or not
        /// </summary>
        /// <param name="jsonstr"></param>
        /// <returns></returns>
        protected static bool IsValidJson(string jsonstr)
        {
            try
            {
                jsonstr = jsonstr.Trim();
                if ((jsonstr.StartsWith("{") && jsonstr.EndsWith("}")) || //For object
       (jsonstr.StartsWith("[") && jsonstr.EndsWith("]"))) //For array
                {
                    var token = JToken.Parse(jsonstr);
                    var obj = JsonConvert.DeserializeObject<FilterCriteria>(jsonstr);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Validate inputs are correct or not
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        protected string InputValidation(string requestBody)
        {

            if (!IsValidJson(requestBody))
                return "Please provide valid JSON data";

            if (string.IsNullOrEmpty(requestBody.Replace("\r\n", string.Empty).Trim()))
                return "Please provide valid JSON data";

            if (JsonConvert.DeserializeObject<JObject>(requestBody).Count == 0)
                return "Please provide valid JSON data";

            //Serializing Input json 
            var filterCriteria = JsonConvert.DeserializeObject<FilterCriteria>(requestBody);

            if (filterCriteria.Pagination != 0 && (filterCriteria.Pagination > 1000 || filterCriteria.Pagination < 100))
                return "Please provide pagination number between 100 to 1000.";

            var startTimeStamp = JObject.Parse(requestBody)["RowUpdatedStartDate"] == null ? "" : JObject.Parse(requestBody)["RowUpdatedStartDate"].ToString().Trim();
            var endTimeStamp = JObject.Parse(requestBody)["RowUpdatedEndDate"] == null ? "" : JObject.Parse(requestBody)["RowUpdatedEndDate"].ToString().Trim();

            if (!IsValidDateformat(startTimeStamp))
                return "Please provide date in yyyy-MM-dd format";
            if (!IsValidDateformat(endTimeStamp))
                return "Please provide date in yyyy-MM-dd format";

            if (!string.IsNullOrEmpty(startTimeStamp) && (!string.IsNullOrEmpty(endTimeStamp)))
            {
                if (Convert.ToDateTime(endTimeStamp) < Convert.ToDateTime(startTimeStamp))
                    return "RowUpdatedEndDate cannot be less than RowUpdatedStartDate";
            }
            return "";
        }

        /// <summary>
        /// Map to dynamic object ignoring null and empty arrays
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public List<dynamic> DynamicMapping(List<dynamic> result)
        {
            var serilaizeJson = JsonConvert.SerializeObject(result, Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = SerializeContractResolver.Instance
                });

            return JsonConvert.DeserializeObject<List<dynamic>>(serilaizeJson);
        }

        /// <summary>
        /// Reads Token from InputHeader 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string GetTokeFromHeader(HttpRequest request)
        {
            return request.Headers.ContainsKey("Token") ? EncodingForBase64.DecodeBase64(request.Headers["Token"]) : null;
        }
    }
}
