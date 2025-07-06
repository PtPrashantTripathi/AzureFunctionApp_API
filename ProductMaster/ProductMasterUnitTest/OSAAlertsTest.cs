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
    public class OSAAlertsTest : TestBase
    {

        [TestMethod]
        public async Task OSAAlerts_PurposeALLSuccess()
        {
            // ****** Arrange ******
            var json = "{\"Purpose\":\"ALL\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ItemFunction OSAFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await OSAFunction.Item(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task OSAAlerts_EmptyRequest()
        {
            // ****** Arrange ******
            var filetrCriteria = new FilterCriteria();
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filetrCriteria);
            ItemFunction OSAFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await OSAFunction.Item(req: HttpRequestSetup(query, "", null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task OSAAlerts_PurposeIsNull()
        {
            // ****** Arrange ******
            var json = "{\"Purpose\":}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ItemFunction OSAFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await OSAFunction.Item(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task OSAAlerts_Emptyjson()
        {
            // ****** Arrange ******
            var json = "{}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ItemFunction OSAFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await OSAFunction.Item(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }


        [TestMethod]
        public async Task OSAAlerts_ProductDetailsSuccess()
        {
            // ****** Arrange ******
            var json = "{\"Purpose\":\"Product Details\",\"ut_id\": [30], \"L5_id\" : [\"L5-000059\"] }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ItemFunction OSAFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await OSAFunction.Item(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task OSAAlerts_ProductDetailsWithoutut_idSuccess()
        {
            // ****** Arrange ******
            var json = "{\"Purpose\":\"Product Details\", \"L5_id\" : [\"L5-000059\"] }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ItemFunction OSAFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await OSAFunction.Item(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task OSAAlerts_ProductDetailsWithoutL5_idSuccess()
        {
            // ****** Arrange ******
            var json = "{\"Purpose\":\"Product Details\", \"ut_id\": [30] }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ItemFunction OSAFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await OSAFunction.Item(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task OSAAlerts_ProductDetailsWithoutL5Andut_id()
        {
            // ****** Arrange ******
            var json = "{\"Purpose\":\"Product Details\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ItemFunction OSAFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await OSAFunction.Item(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Failure");
        }

        [TestMethod]
        public async Task OSAAlerts_ProductDetailsAllInputNull()
        {
            // ****** Arrange ******
            var json = "{\"Purpose\":\"Product Details\",\"ut_id\": [\"null\"], \"L5_id\" : [\"null\"]  }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ItemFunction OSAFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await OSAFunction.Item(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }
    }
}
