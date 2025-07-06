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
    public class ProductGroupTest : TestBase
    {
        [TestMethod]
        public async Task Product_Success()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { mProductGroupId = new List<string> { "12345" } };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            ProductGroupFunction productFunction = new ProductGroupFunction(new Mock<IProductGroupAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.ProductGroup(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Product_EmptyRequest()
        {
            // ****** Arrange ******
            var json = "";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            ProductGroupFunction productFunction = new ProductGroupFunction(new Mock<IProductGroupAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.ProductGroup(req: HttpRequestSetup(query, "", null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }


        [TestMethod]
        public async Task Product_InvalidList()
        {
            // ****** Arrange ******
            var json = "{}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductGroupFunction productFunction = new ProductGroupFunction(new Mock<IProductGroupAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.ProductGroup(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task Product_AnyOneInput()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { mRelatedProductGroupId = new List<string> { "12345" }, mProductGroupId = new List<string> { "12345" }, };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            ProductGroupFunction productFunction = new ProductGroupFunction(new Mock<IProductGroupAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.ProductGroup(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }


        [TestMethod]
        public async Task Product_Catch()
        {
            // ****** Arrange ******
            var json = "{\"mProductId\":[hello]}";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            ProductGroupFunction productFunction = new ProductGroupFunction(new Mock<IProductGroupAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.ProductGroup(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

    }
}
