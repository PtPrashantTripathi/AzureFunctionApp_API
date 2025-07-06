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
    public class DigitalCouponTest : TestBase
    {
        [TestMethod]
        public async Task DigitalCoupon_Success()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { mCouponId = new List<int?> { 1111 } };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            DigitalCouponFunction productFunction = new DigitalCouponFunction(new Mock<IDigitalCouponAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.DigitalCoupon(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }


        [TestMethod]
        public async Task DigitalAvailableStore_EmptyRequest()
        {
            // ****** Arrange ******
            var json = "";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            DigitalCouponFunction productFunction = new DigitalCouponFunction(new Mock<IDigitalCouponAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.DigitalCoupon(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }


        [TestMethod]
        public async Task DigitalAvailableStore_InvalidList()
        {
            // ****** Arrange ******
            var json = "{}";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            DigitalCouponFunction productFunction = new DigitalCouponFunction(new Mock<IDigitalCouponAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.DigitalCoupon(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task DigitalAvailableStore_InvalidCriteria()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { mProductId = new List<string> { "12345" } };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            DigitalCouponFunction productFunction = new DigitalCouponFunction(new Mock<IDigitalCouponAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.DigitalCoupon(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task DigitalAvailableStore_Catch()
        {
            // ****** Arrange ******
            var json = "{\"mProductId\":[hello]}";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            DigitalCouponFunction productFunction = new DigitalCouponFunction(new Mock<IDigitalCouponAdapter>().Object, new Mock<ILoggerAdapter>().Object, Configuration);
            var result = await productFunction.DigitalCoupon(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }
    }
}
