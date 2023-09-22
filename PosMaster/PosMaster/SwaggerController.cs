using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace PosMaster
{
    /// <summary>
    /// Swagger class
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class SwaggerController
    {
        /// <summary>
        /// Swagger json endpoint
        /// </summary>
        /// <param name="req"></param>
        /// <param name="swashBuckleClient"></param>
        /// <returns></returns>
        [SwaggerIgnore]
        [FunctionName("Swagger")]
        public static Task<HttpResponseMessage> Run(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Swagger/json")] HttpRequestMessage req,
           [SwashBuckleClient] ISwashBuckleClient swashBuckleClient)
        {
            return Task.FromResult(swashBuckleClient.CreateSwaggerJsonDocumentResponse(req));
        }

        /// <summary>
        /// Swagger UI route
        /// </summary>
        /// <param name="req"></param>
        /// <param name="swashBuckleClient"></param>
        /// <returns></returns>
        [SwaggerIgnore]
        [FunctionName("SwaggerUi")]
        public static Task<HttpResponseMessage> Run2(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Swagger/ui")] HttpRequestMessage req,
            [SwashBuckleClient] ISwashBuckleClient swashBuckleClient)
        {
            return Task.FromResult(swashBuckleClient.CreateSwaggerUIResponse(req, "swagger/json"));
        }
    }
}
