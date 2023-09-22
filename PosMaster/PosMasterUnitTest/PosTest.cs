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
    public class PosTest : TestBase
    {
        [TestMethod]
        public async Task Pos_Success()
        {
            // ****** Arrange ******
            var json = "{\"mPosId\":[\"3681761\"],\"Purpose\":\"Pos Details\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Pos_EmptyRequest()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria();
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, "", header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Pos_EmptyInput()
        {
            // ****** Arrange ******
            var json = "{}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Pos_RowUpdatedTimestampSuccess()
        {
            // ****** Arrange ******
            var json = "{\"mPosId\":[\"3085189\"], \"RowUpdatedStartDate\" : \"2021-01-13\",\"Purpose\":\"Pos Details\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Pos_RowUpdatedTimestampInvalidformat()
        {
            // ****** Arrange ******
            var json = "{\"mPosId\":[\"3085189\"], \"RowUpdatedStartDate\" : \"13-01-2021\",\"Purpose\":\"Pos Details\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Pos_BrandIdSuccess()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { mPosId = new List<string> { "12345" }, BrandCategoryId = "1234", Purpose = "Pos Details" };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Pos_PosStatusTypeNameSuccess()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { mPosId = new List<string> { "12345" }, BrandCategoryId = "1234", PosStatusTypeName = "2345", Purpose = "Pos Details" };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Pos_Catch()
        {
            // ****** Arrange ******
            var json = "{\"mPosId\":[hello]}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Pos_AnyOneInput()
        {
            // ****** Arrange ******
            var json = "{\"mPosId\":[\"3085189\"], \"Sku\" : [\"88465018232\"], \"mRelatedPosGroupId\" : [\"L2-015946\"], \"LegacyPrimaryKey\" : 24, \"Purpose\":\"All\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        //Unit Testing for Merged API with mRelatedPosId
        [TestMethod]
        public async Task Pos_PosSkuInput()
        {
            // ****** Arrange ******
            var json = "{\"mPosId\":[\"3085189\"], \"Sku\" : [\"88465018232\"],\"Purpose\":\"All\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Pos_PosRelatedPosGroupInput()
        {
            // ****** Arrange ******
            var json = "{\"mPosId\":[\"3085189\"], \"mRelatedPosGroupId\" : [\"L2-015946\"],\"Purpose\":\"All\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Pos_SkuRelatedPosGroupInput()
        {
            // ****** Arrange ******
            var json = "{\"Sku\" : [\"88465018232\"], \"mRelatedPosGroupId\" : [\"L2-015946\"],\"Purpose\":\"All\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Pos_SkuSuccess()
        {
            // ****** Arrange ******
            var json = "{\"Sku\" : [\"88465018232\"], \"Purpose\":\"All\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Pos_SkuInvalidPurpose()
        {
            // ****** Arrange ******
            var json = "{\"Sku\" : [\"88465018232\"], \"Purpose\":\"Pos Details\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Pos_RelatedPosGroupSuccess()
        {
            // ****** Arrange ******
            var json = "{ \"mRelatedPosGroupId\" : [\"L2-015946\"], \"Purpose\":\"All\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Pos_RelatedPosGroupInvalidPurpose()
        {
            // ****** Arrange ******
            var json = "{ \"mRelatedPosGroupId\" : [\"L2-015946\"], \"Purpose\":\"Pos Details\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Pos_RelatedPosGroupInvalidlevel()
        {
            // ****** Arrange ******
            var json = "{\"mRelatedPosGroupId\" : [\"L6-000009\"],\"Purpose\":\"All\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Pos_RelatedPosGroupMultiInput()
        {
            // ****** Arrange ******
            var json = "{\"mRelatedPosGroupId\" : [\"L2-015946\", \"L3-004146\"],\"Purpose\":\"All\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        //Unit Testing for Merged API with LegacyPrimaryKeyId
        [TestMethod]
        public async Task Pos_LegacyPrimaryKeySuccess()
        {
            // ****** Arrange ******
            var json = "{\"LegacyPrimaryKeyId\" : 24,\"Purpose\":\"All\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Pos_LegacyPrimaryKeyZero()
        {
            // ****** Arrange ******
            var json = "{\"LegacyPrimaryKeyId\" : 0, \"Purpose\":\"All\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Pos_LegacyPrimaryKeyPurpose()
        {
            // ****** Arrange ******
            var json = "{\"LegacyPrimaryKeyId\" : 24,\"Purpose\":\"Pos Details\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }



        [TestMethod]
        public async Task Pos_PosGroupSearchSuccess()
        {
            // ****** Arrange ******
            var json = "{\"mRelatedPosGroupId\":[\"L2-017286\"],\"Purpose\":\"All\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Pos_PosGroupSearchMultiple()
        {
            // ****** Arrange ******
            var json = "{\"mRelatedPosGroupId\":[\"L2-017286\",\"L3-004315\"],\"Purpose\":\"All\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Pos_PosGroupSearchEmpty()
        {
            // ****** Arrange ******
            var json = "{\"mRelatedPosGroupId\":[],\"Purpose\":\"All\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Pos_LegacyPrimaryKeyIdSearch_Success()
        {
            // ****** Arrange ******
            var json = "{\"LegacyPrimaryKeyId\":2,\"Purpose\":\"Pos Details\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Pos_LegacyPrimaryKeyIdZero()
        {
            // ****** Arrange ******
            var json = "{\"LegacyPrimaryKeyId\":0,\"Purpose\":\"Pos Details\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Pos_LegacyPrimaryKeyId_PurposeAll()
        {
            // ****** Arrange ******
            var json = "{\"LegacyPrimaryKeyId\":2,\"Purpose\":\"All\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Pos_LegacyPrimaryKeyIdZero_PurposeAll()
        {
            // ****** Arrange ******
            var json = "{\"LegacyPrimaryKeyId\":0,\"Purpose\":\"All\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Pos_LegacyPrimaryKeyId_DuplicateProperty()
        {
            // ****** Arrange ******
            var json = "{\"LegacyPrimaryKeyId\":2,\"Purpose\":\"All\",\"Purpose\":\"Pos Details\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            var header = new Dictionary<String, StringValues>();
            PosFunction posFunction = new PosFunction(new Mock<IPosAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await posFunction.Pos(req: HttpRequestSetup(query, body, header), new Mock<ExecutionContext>().Object);
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }
    }
}
