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
    public class ItemTest : TestBase
    {

        [TestMethod]
        public async Task Item_ItemSkuSuccess()
        {
            // ****** Arrange ******
            var json = "{\"ItemSku\":[\"70882010802\"]}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ItemFunction upcFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await upcFunction.Item(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Item_EmptyRequest()
        {
            // ****** Arrange ******
            var filetrCriteria = new FilterCriteria();
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filetrCriteria);
            ItemFunction upcFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await upcFunction.Item(req: HttpRequestSetup(query, "", null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Item_catch()
        {
            // ****** Arrange ******
            var json = "{\"ItemSku\":[hello]}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ItemFunction upcFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await upcFunction.Item(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Item_Emptyjson()
        {
            // ****** Arrange ******
            var json = "{}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ItemFunction upcFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await upcFunction.Item(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Item_UpcTypeSuccess()
        {
            // ****** Arrange ******
            var filetrCriteria = new FilterCriteria { ItemSku = new List<string> { "12345" }, UPCTypeName = new List<string> { "12345" } };
            var query = new Dictionary<String, StringValues>();
            string body = JsonConvert.SerializeObject(filetrCriteria);
            ItemFunction upcFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await upcFunction.Item(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }


        [TestMethod]
        public async Task Item_RowUpdatedTimestampSuccess()
        {
            // ****** Arrange ******
            var json = "{\"ItemSku\":[\"70882010802\"],\"UPCTypeName\": [\"UPCA\"], \"RowUpdatedStartDate\" : \"2021-01-13\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ItemFunction upcFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await upcFunction.Item(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }
        [TestMethod]
        public async Task Item_AllInputNull()
        {
            // ****** Arrange ******
            var json = "{\"ItemSku\":[\"null\"],\"UPCTypeName\": [\"null\"], \"RowUpdatedStartDate\" : " + DateTime.MinValue + " }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ItemFunction upcFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await upcFunction.Item(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Item_RowUpdatedTimestampInvalidformat()
        {
            // ****** Arrange ******
            var json = "{\"ItemSku\":[\"70882010802\"],\"UPCTypeName\": [\"UPCA\"], \"RowUpdatedStartDate\" : \"13-01-2021\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ItemFunction upcFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await upcFunction.Item(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }
    }
}
