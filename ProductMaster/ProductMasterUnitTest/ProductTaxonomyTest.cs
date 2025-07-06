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
    public class ProductTaxonomyTest : TestBase
    {
        [TestMethod]
        public async Task Product_Success()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { mProductId = new List<string> { "12345" } };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            ProductTaxonomyFunction productFunction = new ProductTaxonomyFunction(new Mock<IProductTaxonomyAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.ProductTaxonomy(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task Product_InvalidList()
        {
            // ****** Arrange ******
            var json = "{}";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            ProductTaxonomyFunction productFunction = new ProductTaxonomyFunction(new Mock<IProductTaxonomyAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.ProductTaxonomy(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }


        [TestMethod]
        public async Task Product_InvalidCriteria()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { ItemSku = new List<string> { "12345" } };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            ProductTaxonomyFunction productFunction = new ProductTaxonomyFunction(new Mock<IProductTaxonomyAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.ProductTaxonomy(req: HttpRequestSetup(query, body, null));
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
            ProductTaxonomyFunction productFunction = new ProductTaxonomyFunction(new Mock<IProductTaxonomyAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.ProductTaxonomy(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }
    }
}
