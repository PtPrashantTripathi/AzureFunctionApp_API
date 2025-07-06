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
    public class ProductPackagingTest : TestBase
    {
        [TestMethod]
        public async Task ProductPackaging_EmptyRequest()
        {
            // ****** Arrange ******
            var filetrCriteria = new FilterCriteria();
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filetrCriteria);
            ProductPackagingFunction productFunction = new ProductPackagingFunction(new Mock<IProductPackagingAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.ProductPackaging(req: HttpRequestSetup(query, "", null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task ProductPackaging_catch()
        {
            // ****** Arrange ******
            var json = "{\"mProductId\":[hello]}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductPackagingFunction productFunction = new ProductPackagingFunction(new Mock<IProductPackagingAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.ProductPackaging(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }
        [TestMethod]
        public async Task ProductPackaging_mProductIdNull()
        {
            // ****** Arrange ******
            var json = "{\"CasePackId\":[\"0000029511-001-002\"]}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductPackagingFunction productFunction = new ProductPackagingFunction(new Mock<IProductPackagingAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.ProductPackaging(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task ProductPackaging_Emptyjson()
        {
            // ****** Arrange ******
            var json = "{}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductPackagingFunction productFunction = new ProductPackagingFunction(new Mock<IProductPackagingAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.ProductPackaging(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task ProductPackaging_mProductIdSuccess()
        {
            // ****** Arrange ******
            var json = "{\"mProductId\":[\"3827633\"]}";
            var query = new Dictionary<String, StringValues>();
            var body = json; //JsonConvert.SerializeObject(json);
            ProductPackagingFunction productFunction = new ProductPackagingFunction(new Mock<IProductPackagingAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.ProductPackaging(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

    }
}
