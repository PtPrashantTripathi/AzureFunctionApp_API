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
    public class PosGroupTest : TestBase
    {
        [TestMethod]
        public async Task Pos_Success()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { mPosGroupId = new List<string> { "12345" } };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            var header = new Dictionary<String, StringValues>();
            PosGroupFunction posFunction = new PosGroupFunction(new Mock<IPosGroupAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.PosGroup(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Pos_EmptyRequest()
        {
            // ****** Arrange ******
            var json = "";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            var header = new Dictionary<String, StringValues>();
            PosGroupFunction posFunction = new PosGroupFunction(new Mock<IPosGroupAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.PosGroup(req: HttpRequestSetup(query, "", header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }


        [TestMethod]
        public async Task Pos_InvalidList()
        {
            // ****** Arrange ******
            var json = "{}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosGroupFunction posFunction = new PosGroupFunction(new Mock<IPosGroupAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.PosGroup(req: HttpRequestSetup(query, body, header),new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Pos_AnyOneInput()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { mRelatedPosGroupId = new List<string> { "12345" }, mPosGroupId = new List<string> { "12345" }, };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            var header = new Dictionary<String, StringValues>();
            PosGroupFunction posFunction = new PosGroupFunction(new Mock<IPosGroupAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.PosGroup(req: HttpRequestSetup(query, body, header),new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }


        [TestMethod]
        public async Task Pos_Catch()
        {
            // ****** Arrange ******
            var json = "{\"mPosId\":[hello]}";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            var header = new Dictionary<String, StringValues>();
            PosGroupFunction posFunction = new PosGroupFunction(new Mock<IPosGroupAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.PosGroup(req: HttpRequestSetup(query, body, header),new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

    }
}
