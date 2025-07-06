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
    public class ProductTest : TestBase
    {
        [TestMethod]
        public async Task Product_Success()
        {
            // ****** Arrange ******
            var json = "{\"mProductId\":[\"3681761\"],\"Purpose\":\"Product Details\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Product_EmptyRequest()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria();
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, "", null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Product_EmptyInput()
        {
            // ****** Arrange ******
            var json = "{}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Product_RowUpdatedTimestampSuccess()
        {
            // ****** Arrange ******
            var json = "{\"mProductId\":[\"3085189\"], \"RowUpdatedStartDate\" : \"2021-01-13\",\"Purpose\":\"Product Details\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Product_RowUpdatedTimestampInvalidformat()
        {
            // ****** Arrange ******
            var json = "{\"mProductId\":[\"3085189\"], \"RowUpdatedStartDate\" : \"13-01-2021\",\"Purpose\":\"Product Details\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Product_BrandIdSuccess()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { mProductId = new List<string> { "12345" }, BrandCategoryId = "1234", Purpose = "Product Details" };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Product_ProductStatusTypeNameSuccess()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { mProductId = new List<string> { "12345" }, BrandCategoryId = "1234", ProductStatusTypeName = "2345", Purpose = "Product Details" };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Product_Catch()
        {
            // ****** Arrange ******
            var json = "{\"mProductId\":[hello]}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Product_AnyOneInput()
        {
            // ****** Arrange ******
            var json = "{\"mProductId\":[\"3085189\"], \"ItemSku\" : [\"88465018232\"], \"mRelatedProductGroupId\" : [\"L2-015946\"], \"LegacyPrimaryKey\" : 24, \"Purpose\":\"All\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        //Unit Testing for Merged API with mRelatedProductId
        [TestMethod]
        public async Task Product_ProductItemSkuInput()
        {
            // ****** Arrange ******
            var json = "{\"mProductId\":[\"3085189\"], \"ItemSku\" : [\"88465018232\"],\"Purpose\":\"All\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Product_ProductRelatedProductGroupInput()
        {
            // ****** Arrange ******
            var json = "{\"mProductId\":[\"3085189\"], \"mRelatedProductGroupId\" : [\"L2-015946\"],\"Purpose\":\"All\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Product_ItemSkuRelatedProductGroupInput()
        {
            // ****** Arrange ******
            var json = "{\"ItemSku\" : [\"88465018232\"], \"mRelatedProductGroupId\" : [\"L2-015946\"],\"Purpose\":\"All\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Product_ItemSkuSuccess()
        {
            // ****** Arrange ******
            var json = "{\"ItemSku\" : [\"88465018232\"], \"Purpose\":\"All\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Product_ItemSkuInvalidPurpose()
        {
            // ****** Arrange ******
            var json = "{\"ItemSku\" : [\"88465018232\"], \"Purpose\":\"Product Details\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Product_RelatedProductGroupSuccess()
        {
            // ****** Arrange ******
            var json = "{ \"mRelatedProductGroupId\" : [\"L2-015946\"], \"Purpose\":\"All\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Product_RelatedProductGroupInvalidPurpose()
        {
            // ****** Arrange ******
            var json = "{ \"mRelatedProductGroupId\" : [\"L2-015946\"], \"Purpose\":\"Product Details\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Product_RelatedProductGroupInvalidlevel()
        {
            // ****** Arrange ******
            var json = "{\"mRelatedProductGroupId\" : [\"L6-000009\"],\"Purpose\":\"All\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Product_RelatedProductGroupMultiInput()
        {
            // ****** Arrange ******
            var json = "{\"mRelatedProductGroupId\" : [\"L2-015946\", \"L3-004146\"],\"Purpose\":\"All\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        //Unit Testing for Merged API with LegacyPrimaryKeyId
        [TestMethod]
        public async Task Product_LegacyPrimaryKeySuccess()
        {
            // ****** Arrange ******
            var json = "{\"LegacyPrimaryKeyId\" : 24,\"Purpose\":\"All\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Product_LegacyPrimaryKeyZero()
        {
            // ****** Arrange ******
            var json = "{\"LegacyPrimaryKeyId\" : 0, \"Purpose\":\"All\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Product_LegacyPrimaryKeyPurpose()
        {
            // ****** Arrange ******
            var json = "{\"LegacyPrimaryKeyId\" : 24,\"Purpose\":\"Product Details\" }";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }



        [TestMethod]
        public async Task Product_ProductGroupSearchSuccess()
        {
            // ****** Arrange ******
            var json = "{\"mRelatedProductGroupId\":[\"L2-017286\"],\"Purpose\":\"All\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Product_ProductGroupSearchMultiple()
        {
            // ****** Arrange ******
            var json = "{\"mRelatedProductGroupId\":[\"L2-017286\",\"L3-004315\"],\"Purpose\":\"All\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Product_ProductGroupSearchEmpty()
        {
            // ****** Arrange ******
            var json = "{\"mRelatedProductGroupId\":[],\"Purpose\":\"All\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Product_LegacyPrimaryKeyIdSearch_Success()
        {
            // ****** Arrange ******
            var json = "{\"LegacyPrimaryKeyId\":2,\"Purpose\":\"Product Details\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Product_LegacyPrimaryKeyIdZero()
        {
            // ****** Arrange ******
            var json = "{\"LegacyPrimaryKeyId\":0,\"Purpose\":\"Product Details\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Product_LegacyPrimaryKeyId_PurposeAll()
        {
            // ****** Arrange ******
            var json = "{\"LegacyPrimaryKeyId\":2,\"Purpose\":\"All\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Product_LegacyPrimaryKeyIdZero_PurposeAll()
        {
            // ****** Arrange ******
            var json = "{\"LegacyPrimaryKeyId\":0,\"Purpose\":\"All\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Product_LegacyPrimaryKeyId_DuplicateProperty()
        {
            // ****** Arrange ******
            var json = "{\"LegacyPrimaryKeyId\":2,\"Purpose\":\"All\",\"Purpose\":\"Product Details\"}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductFunction productFunction = new ProductFunction(new Mock<IProductAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.Product(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }
    }
}
