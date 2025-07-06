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
    public class DigitalCouponItemTest : TestBase
    {
        [TestMethod]
        public async Task DigitalAvailableStore_Success()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { mCouponId = new List<int?> { 1111 }, CouponSourceId = new List<int?> { 2222 } };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            DigitalCouponItemFunction productFunction = new DigitalCouponItemFunction(new Mock<IDigitalCouponItemAdapter>().Object, Configuration, new Mock<ILoggerAdapter>().Object);
            var result = await productFunction.DigitalCouponItem(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task DigitalAvailableStore_Success1()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { ItemSku = new List<string> { "1111" } };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            DigitalCouponItemFunction productFunction = new DigitalCouponItemFunction(new Mock<IDigitalCouponItemAdapter>().Object, Configuration, new Mock<ILoggerAdapter>().Object);
            var result = await productFunction.DigitalCouponItem(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Success");
        }

        [TestMethod]
        public async Task DigitalAvailableStore_Anyone()
        {
            // ****** Arrange ******
            var filterCriteria = new FilterCriteria { mCouponId = new List<int?> { 1111 } };
            var query = new Dictionary<String, StringValues>();
            var body = JsonConvert.SerializeObject(filterCriteria);
            DigitalCouponItemFunction productFunction = new DigitalCouponItemFunction(new Mock<IDigitalCouponItemAdapter>().Object, Configuration, new Mock<ILoggerAdapter>().Object);
            var result = await productFunction.DigitalCouponItem(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task DigitalAvailableStore_EmptyRequest()
        {
            // ****** Arrange ******
            var json = "";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            DigitalCouponItemFunction productFunction = new DigitalCouponItemFunction(new Mock<IDigitalCouponItemAdapter>().Object, Configuration, new Mock<ILoggerAdapter>().Object);
            var result = await productFunction.DigitalCouponItem(req: HttpRequestSetup(query, body, null));
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
            DigitalCouponItemFunction productFunction = new DigitalCouponItemFunction(new Mock<IDigitalCouponItemAdapter>().Object, Configuration, new Mock<ILoggerAdapter>().Object);
            var result = await productFunction.DigitalCouponItem(req: HttpRequestSetup(query, body, null));
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
            DigitalCouponItemFunction productFunction = new DigitalCouponItemFunction(new Mock<IDigitalCouponItemAdapter>().Object, Configuration, new Mock<ILoggerAdapter>().Object);
            var result = await productFunction.DigitalCouponItem(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }

        [TestMethod]
        public async Task DigitalAvailableStore_Catch()
        {
            // ****** Arrange ******
            var json = "{\"ItemSku\":[hello]}";
            var query = new Dictionary<String, StringValues>();
            var body = json;
            DigitalCouponItemFunction productFunction = new DigitalCouponItemFunction(new Mock<IDigitalCouponItemAdapter>().Object, Configuration, new Mock<ILoggerAdapter>().Object);
            var result = await productFunction.DigitalCouponItem(req: HttpRequestSetup(query, body, null));
            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), "Failure");
        }
    }
}
