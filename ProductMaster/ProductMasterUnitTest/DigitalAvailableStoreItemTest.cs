using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using ProductMaster.Data;
using ProductMasterUnitTest.Base;

namespace ProductMaster.UnitTest
{
    [TestClass]
    public class DigitalAvailableStoreItemTest : TestBase
    {
        [TestMethod]
        public async Task DigitalAvailableStore_Success()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { ItemSku = new List<string> { "12345" } };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            DigitalAvailableStoreItemFunction productFunction = new DigitalAvailableStoreItemFunction(new Mock<IDigitalAvailableStoreItemAdapter>().Object, Configuration, new Mock<ILoggerAdapter>().Object);
            var result = await productFunction.DigitalAvailableStoreItem(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task DigitalAvailableStore_EmptyRequest()
        {
            // ****** Arrange ******
            var json = "";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            DigitalAvailableStoreItemFunction productFunction = new DigitalAvailableStoreItemFunction(new Mock<IDigitalAvailableStoreItemAdapter>().Object, Configuration, new Mock<ILoggerAdapter>().Object);
            var result = await productFunction.DigitalAvailableStoreItem(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }


        [TestMethod]
        public async Task DigitalAvailableStore_InvalidList()
        {
            // ****** Arrange ******
            var json = "{}";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            DigitalAvailableStoreItemFunction productFunction = new DigitalAvailableStoreItemFunction(new Mock<IDigitalAvailableStoreItemAdapter>().Object, Configuration, new Mock<ILoggerAdapter>().Object);
            var result = await productFunction.DigitalAvailableStoreItem(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task DigitalAvailableStore_InvalidCriteria()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { mProductId = new List<string> { "12345" } };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            DigitalAvailableStoreItemFunction productFunction = new DigitalAvailableStoreItemFunction(new Mock<IDigitalAvailableStoreItemAdapter>().Object, Configuration, new Mock<ILoggerAdapter>().Object);
            var result = await productFunction.DigitalAvailableStoreItem(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task DigitalAvailableStore_Catch()
        {
            // ****** Arrange ******
            var json = "{\"mProductId\":[hello]}";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            DigitalAvailableStoreItemFunction productFunction = new DigitalAvailableStoreItemFunction(new Mock<IDigitalAvailableStoreItemAdapter>().Object, Configuration, new Mock<ILoggerAdapter>().Object);
            var result = await productFunction.DigitalAvailableStoreItem(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }
    }
}
