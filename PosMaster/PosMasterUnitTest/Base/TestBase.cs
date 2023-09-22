using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PosMasterUnitTest.Base
{
    [ExcludeFromCodeCoverage]
    public class TestBase
    {
        /// <summary>
        /// Configuration Instance
        /// </summary>
        private IConfiguration _config;

        /// <summary>
        /// Configuration Public property        
        /// </summary>
        protected IConfiguration Configuration
        {
            get
            {
                if (_config == null)
                {
                    var builder = new ConfigurationBuilder();
                    _config = builder.Build();
                }

                return _config;
            }
        }
        /// <summary>
        /// Cosmos Client
        /// </summary>
        Microsoft.Azure.Cosmos.CosmosClient _CosmosClient;
        /// <summary>
        /// Cosmos Client Public property        
        /// </summary>
        protected Microsoft.Azure.Cosmos.CosmosClient CosmosClient
        {
            get
            {
                if (_CosmosClient == null)
                {
                    var fakeCosmosClient = new Mock<Microsoft.Azure.Cosmos.CosmosClient>();
                    var fakeDatabases = new Mock<Microsoft.Azure.Cosmos.Database>();
                    var fakeContainer = new Mock<Microsoft.Azure.Cosmos.Container>();
                    fakeDatabases.Setup(c => c.GetContainer(It.IsAny<string>())).Returns(fakeContainer.Object);
                    fakeCosmosClient.Setup(c => c.GetDatabase(It.IsAny<string>())).Returns(fakeDatabases.Object);
                    _CosmosClient = fakeCosmosClient.Object;
                }

                return _CosmosClient;
            }
        }
        protected HttpRequest HttpRequestSetup(Dictionary<String, StringValues> query, string body, Dictionary<String, StringValues> header)
        {
            var reqMock = new Mock<HttpRequest>();
            reqMock.Setup(req => req.Query).Returns(new QueryCollection(query));
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(body);
            writer.Flush();
            stream.Position = 0;
            reqMock.Setup(req => req.Body).Returns(stream);
            // turn our request string into a byte stream
            byte[] postBytes = Encoding.UTF8.GetBytes(body);
            // this is important - make sure you specify type this way
            reqMock.Setup(req => req.ContentType).Returns("application/json; charset=UTF-8");
            reqMock.Setup(req => req.Headers).Returns(new HeaderDictionary(header));
            reqMock.Setup(req => req.ContentLength).Returns(postBytes.Length);
            return reqMock.Object;
        }
    }
}
