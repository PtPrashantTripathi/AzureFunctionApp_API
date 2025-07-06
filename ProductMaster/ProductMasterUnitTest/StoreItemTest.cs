using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using ProductMaster.Data;
using ProductMasterUnitTest.Base;


namespace ProductMaster.UnitTest
{

    [TestClass]
    public class StoreItemTest : TestBase
    {
        [TestMethod]
        public async Task StoreItem_StoreIDSuccess()
        {
            // ****** Arrange ******
            var json = "{\"StoreID\":[\"19\"]}";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            StoreItemFunction storeItemFunction = new StoreItemFunction(new Mock<IStoreItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await storeItemFunction.StoreItem(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.OK);
            //Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }
        [TestMethod]
        public async Task StoreItem_StoreIDNegativeValidation()
        {
            // ****** Arrange ******
            var json = "{\"StoreID\":[\"-1\"]}";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            StoreItemFunction storeItemFunction = new StoreItemFunction(new Mock<IStoreItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await storeItemFunction.StoreItem(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task StoreItem_EmptyRequest()
        {
            // ****** Arrange ******
            var filetrCriteria = new FilterCriteria();
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filetrCriteria);
            StoreItemFunction storeItemFunction = new StoreItemFunction(new Mock<IStoreItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await storeItemFunction.StoreItem(req: HttpRequestSetup(query, "", null));
            //Assert
            //Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
            Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task StoreItem_catch()
        {
            // ****** Arrange ******
            var json = "{\"ItemSku\":[hello]}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            StoreItemFunction storeItemFunction = new StoreItemFunction(new Mock<IStoreItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await storeItemFunction.StoreItem(req: HttpRequestSetup(query, body, null));
            //Assert
            //Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
            Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task StoreItem_Emptyjson()
        {
            // ****** Arrange ******
            var json = "{}";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            StoreItemFunction storeItemFunction = new StoreItemFunction(new Mock<IStoreItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await storeItemFunction.StoreItem(req: HttpRequestSetup(query, body, null));
            //Assert
            //Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
            Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task StoreItem_InvalidInput()
        {
            // ****** Arrange ******
            var json = "{\"mProductId\":[\"3085189\"]\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            StoreItemFunction storeItemFunction = new StoreItemFunction(new Mock<IStoreItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await storeItemFunction.StoreItem(req: HttpRequestSetup(query, body, null));
            //Assert
            // Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
            Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.BadRequest);
        }
        [TestMethod]
        public async Task StoreItem_ItemSkuNullInput()
        {
            // ****** Arrange ******
            var json = "{\"ItemSku\":[],\"StoreId\":[232]}";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            StoreItemFunction storeItemFunction = new StoreItemFunction(new Mock<IStoreItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await storeItemFunction.StoreItem(req: HttpRequestSetup(query, body, null));
            //Assert
            // Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
            Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.BadRequest);
        }
        [TestMethod]
        public async Task StoreItem_ItemSkuStoreIdNullInput()
        {
            // ****** Arrange ******
            var json = "{\"ItemSku\":[],\"StoreId\":[]}";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            StoreItemFunction storeItemFunction = new StoreItemFunction(new Mock<IStoreItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await storeItemFunction.StoreItem(req: HttpRequestSetup(query, body, null));
            //Assert
            // Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
            Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task StoreItem_ItemSkuStoreIdBulkInput()
        {
            // ****** Arrange ******
            var json = "{\"ItemSku\":[\"21287500000\",\"21287500000\"],\"StoreId\":[\"232\",\"82\"]}";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            StoreItemFunction storeItemFunction = new StoreItemFunction(new Mock<IStoreItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await storeItemFunction.StoreItem(req: HttpRequestSetup(query, body, null));
            //Assert
            // Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
            Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task StoreItem_SingleItemSkuStoreIdNullInput()
        {
            // ****** Arrange ******
            var json = "{\"ItemSku\":[\"21287500000\"],\"StoreId\":[]}";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            StoreItemFunction storeItemFunction = new StoreItemFunction(new Mock<IStoreItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await storeItemFunction.StoreItem(req: HttpRequestSetup(query, body, null));
            //Assert
            // Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
            Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.BadRequest);
        }
        [TestMethod]
        public async Task StoreItem_StoreIdEmptyInput()
        {
            // ****** Arrange ******
            var json = "{\"StoreId\":[\"\"]}";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            StoreItemFunction storeItemFunction = new StoreItemFunction(new Mock<IStoreItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await storeItemFunction.StoreItem(req: HttpRequestSetup(query, body, null));
            //Assert
            // Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
            Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.BadRequest);
        }

    }
}
