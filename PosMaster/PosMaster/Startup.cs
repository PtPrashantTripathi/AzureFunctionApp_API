using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Dynatrace.OpenTelemetry.Instrumentation.AzureFunctions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PosMaster.Data;
using PosMaster.Data.Adapter;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Dynatrace.OpenTelemetry;
using OpenTelemetry.Trace;

[assembly: FunctionsStartup(typeof(PosMaster.Startup))]
namespace PosMaster
{
    /// <summary>
    /// StartUp class
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Startup : FunctionsStartup
    {
        /// <summary>The Name of the API</summary>
        public const string API_NAME = "PosMaster";
        /// <summary>The version of the API</summary>
        public const string API_VERSION = "v1";
        /// <summary>The description of the API</summary>
        public const string API_DESCRIPTION = "PosMaster Function";

        // CosmoDb contant feilds
        private const string CONFIG_COSMOS_URL = "CosmosURL";
        private const string CONFIG_COSMOS_WRITE_KEY = "CosmosWriteKey";
        private const string CONFIG_COSMOS_REGION = "CosmosRegion";

        /// <summary>
        /// Configure setting
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.Services.BuildServiceProvider().GetService<IConfiguration>();

            //Fetching cosmodb secrets
            var cosmosEndpointUri = Environment.GetEnvironmentVariable(CONFIG_COSMOS_URL);
            var cosmosKey = Environment.GetEnvironmentVariable(CONFIG_COSMOS_WRITE_KEY);
            var cosmosRegion = Environment.GetEnvironmentVariable(CONFIG_COSMOS_REGION);

            //COSMO client configuration
            CosmosClientOptions options = new CosmosClientOptions();
            options.ApplicationName = "PosMaster";
            options.ApplicationRegion = cosmosRegion;

            var CosmosClient = new CosmosClient(cosmosEndpointUri, cosmosKey, options);
            builder.Services.AddSingleton<CosmosClient>(CosmosClient);
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //Dependency injection
            builder.Services.AddTransient<ILoggerAdapter, LoggerAdapter>();
            builder.Services.AddTransient<ICosmosDbService<dynamic>, CosmosDbService<dynamic>>();
            builder.Services.AddTransient<IItemAdapter, ItemAdapter>();
            builder.Services.AddTransient<IPosAdapter, PosAdapter>();

            //HealthChecks implementation
            builder.Services.AddHealthChecks()
                .AddCheck<HealthCheckAdapter>("CosmosConn_Check");

            //Swagger implementation
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly(), opts =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                opts.XmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                opts.Documents = new[]
                {
                    new SwaggerDocument
                    {
                        Name = "v1",
                        Title = API_NAME,
                        Description = API_DESCRIPTION,
                        Version = API_VERSION
                    }
                };

            });
           
        }
    }
}