using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using OnlineShopApi.data.Models;
using OnlineShopApi.data.Repositories;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;

namespace OnlineShopApi.data.test
{
    public class WooliesServiceTests
    {
        private readonly WooliesService _service; // use this private variable for for OK status responses only
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly MockHttpMessageHandler _mockHttp;
        private const string Host = "http://test.com";
        private const string Token = "123";

        public WooliesServiceTests()
        {
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "Host")]).Returns(Host);
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "Token")]).Returns(Token);

            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(a => a.GetSection(It.Is<string>(s => s == "WooliesService"))).Returns(mockConfSection.Object);
            _mockHttp = new MockHttpMessageHandler();

            var httpClient = ConfigureHttpClient(_mockHttp);
            _service = new WooliesService(_mockConfig.Object, httpClient);
        }
        
        // Only for OK Status responses
        private HttpClient ConfigureHttpClient(MockHttpMessageHandler mockHttp)
        {
            var history = JsonConvert.SerializeObject(new List<ShopperHistory>());
            var products = JsonConvert.SerializeObject(new List<Product>());
            var trolleyTotal = JsonConvert.SerializeObject(50.78);

            mockHttp.When($"{Host}/api/resource/shopperHistory?token={Token}").Respond("application/json", history);
            mockHttp.When($"{Host}/api/resource/products?token={Token}").Respond("application/json", products);
            mockHttp.When($"{Host}/api/resource/trolleyCalculator?token={Token}").Respond("application/json", trolleyTotal);

            // Use real http client with mocked handler
            return new HttpClient(mockHttp);
        }

        [Theory]
        [InlineData(1)]
        public void GetUser_ReturnsUser_ById(int id)
        {
            var result = _service.GetUser(id);

            Assert.NotNull(result);
            Assert.IsType<User>(result);
        }

        [Fact]
        public async void GetShopperHistory_ReturnsListOfShoppingHistory()
        {
            var result = await _service.GetShopperHistoryAsync();

            Assert.NotNull(result);
            Assert.IsType<List<ShopperHistory>>(result);
        }

        [Fact]
        public async void GetShopperHistory_ReturnNull_WhenNotFound()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{Host}/api/resource/shopperHistory?token={Token}").Respond(HttpStatusCode.NotFound);

            var httpClient = new HttpClient(mockHttp);
            var service = new WooliesService(_mockConfig.Object, httpClient);

            var result = await service.GetShopperHistoryAsync();

            Assert.Null(result);
        }

        [Fact]
        public async void GetProducts_ReturnsListOfProducts()
        {
            var result = await _service.GetProductsAsync();

            Assert.NotNull(result);
            Assert.IsType<List<Product>>(result);
        }

        [Fact]
        public async void GetProducts_ReturnNull_WhenNotFound()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{Host}/api/resource/products?token={Token}").Respond(HttpStatusCode.NotFound);

            var httpClient = new HttpClient(mockHttp);
            var service = new WooliesService(_mockConfig.Object, httpClient);

            var result = await service.GetProductsAsync();

            Assert.Null(result);
        }

        [Fact]
        public async void CalculatTrolley_ReturnsDecimal()
        {
            var result = await _service.CalculateTrolleyTotal(new Trolley());

            Assert.IsType<decimal>(result);
        }
    }
}
