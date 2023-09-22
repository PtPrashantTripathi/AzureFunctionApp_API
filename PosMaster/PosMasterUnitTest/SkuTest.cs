using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using PosMaster.Data;
using PosMasterUnitTest.Base;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace PosMaster.UnitTest
{
    [TestClass]
    public class ItemTest : TestBase
    {

        [TestMethod]
        public async Task Item_SkuSuccess()
        {
            // ****** Arrange ******
            var json = "{\"Sku\":[\"70882010802\"]}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            ItemFunction upcFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await upcFunction.Sku(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
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
            var header = new Dictionary<String, StringValues>();
            ItemFunction upcFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await upcFunction.Sku(req: HttpRequestSetup(query, "", header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Item_catch()
        {
            // ****** Arrange ******
            var json = "{\"Sku\":[hello]}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            ItemFunction upcFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await upcFunction.Sku(req: HttpRequestSetup(query, body, header),new Mock<ExecutionContext>().Object);
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
            var header = new Dictionary<String, StringValues>();
            ItemFunction upcFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await upcFunction.Sku(req: HttpRequestSetup(query, body, header),new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Item_UpcTypeSuccess()
        {
            // ****** Arrange ******
            var filetrCriteria = new FilterCriteria { Sku = new List<string> { "12345" }, UPCTypeName = new List<string> { "12345" } };
            var query = new Dictionary<String, StringValues>();
            string body = JsonConvert.SerializeObject(filetrCriteria);
            var header = new Dictionary<String, StringValues>();
            ItemFunction upcFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await upcFunction.Sku(req: HttpRequestSetup(query, body, header),new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }


        [TestMethod]
        public async Task Item_RowUpdatedTimestampSuccess()
        {
            // ****** Arrange ******
            var json = "{\"Sku\":[\"70882010802\"],\"UPCTypeName\": [\"UPCA\"], \"RowUpdatedStartDate\" : \"2021-01-13\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            ItemFunction upcFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await upcFunction.Sku(req: HttpRequestSetup(query, body, header),new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }
        [TestMethod]
        public async Task Item_AllInputNull()
        {
            // ****** Arrange ******
            var json = "{\"Sku\":[\"null\"],\"UPCTypeName\": [\"null\"], \"RowUpdatedStartDate\" : " + DateTime.MinValue + " }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            ItemFunction upcFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await upcFunction.Sku(req: HttpRequestSetup(query, body, header),new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Item_RowUpdatedTimestampInvalidformat()
        {
            // ****** Arrange ******
            var json = "{\"Sku\":[\"70882010802\"],\"UPCTypeName\": [\"UPCA\"], \"RowUpdatedStartDate\" : \"13-01-2021\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            ItemFunction upcFunction = new ItemFunction(new Mock<IItemAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await upcFunction.Sku(req: HttpRequestSetup(query, body, header),new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }
    }
}
