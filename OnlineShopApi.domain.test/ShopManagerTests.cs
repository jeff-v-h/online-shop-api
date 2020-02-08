using AutoMapper;
using Moq;
using OnlineShopApi.data.Models;
using OnlineShopApi.data.Repositories;
using OnlineShopApi.domain.Managers;
using OnlineShopApi.domain.Models.AppModels;
using OnlineShopApi.domain.Models.ModelMappers;
using OnlineShopApi.domain.Models.ViewModels;
using System;
using System.Collections.Generic;
using Xunit;

namespace OnlineShopApi.domain.test
{
    public class ShopManagerTests
    {
        private readonly Mock<IWooliesService> _mockService;
        private ShopManager _manager;

        public ShopManagerTests()
        {
            _mockService = new Mock<IWooliesService>();
            var myProfile = new ShopProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);
            _manager = new ShopManager(mapper, _mockService.Object);
        }

        [Fact]
        public void GetUser_ReturnsUserVM_WhenUserReturned()
        {
            _mockService.Setup(x => x.GetUser(It.IsAny<int>())).Returns(new User());

            var result = _manager.GetUser(111);

            Assert.NotNull(result);
            Assert.IsType<UserVM>(result);
        }

        [Fact]
        public void GetUser_ReturnsNull_WhenNullReturnedFromService()
        {
            User user = null;
            _mockService.Setup(x => x.GetUser(It.IsAny<int>())).Returns(user);

            var result = _manager.GetUser(111);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("Low")]
        [InlineData("High")]
        [InlineData("Ascending")]
        [InlineData("Descending")]
        [InlineData("Recommended")]
        public async void GetProductsAsync_ReturnsListProductVMs_WhenProductsReturned(string sortOption)
        {
            var option = (SortOption)Enum.Parse(typeof(SortOption), sortOption);
            var products = new List<Product>();
            var shopHistory = new List<ShopperHistory>();
            _mockService.Setup(x => x.GetProductsAsync()).ReturnsAsync(products);
            _mockService.Setup(x => x.GetShopperHistoryAsync()).ReturnsAsync(shopHistory);

            var result = await _manager.GetProductsAsync(option);

            Assert.NotNull(result);
            Assert.IsType<List<ProductVM>>(result);
        }

        [Theory]
        [InlineData("Low")]
        [InlineData("High")]
        [InlineData("Ascending")]
        [InlineData("Descending")]
        public async void GetProductsAsync_DoesNotCallShopperHistory_WhenNotRecommendedOption(string sortOption)
        {
            var option = (SortOption)Enum.Parse(typeof(SortOption), sortOption);
            var products = new List<Product>();
            var shopHistory = new List<ShopperHistory>();
            _mockService.Setup(x => x.GetProductsAsync()).ReturnsAsync(products);
            _mockService.Setup(x => x.GetShopperHistoryAsync()).ReturnsAsync(shopHistory);

            var result = await _manager.GetProductsAsync(option);

            _mockService.Verify(x => x.GetShopperHistoryAsync(), Times.Never);
        }

        [Fact]
        public async void GetProductsAsync_CallsShopperHistory_WhenRecommendedOption()
        {
            var option = (SortOption)Enum.Parse(typeof(SortOption), "Recommended");
            var products = new List<Product>();
            var shopHistory = new List<ShopperHistory>();
            _mockService.Setup(x => x.GetProductsAsync()).ReturnsAsync(products);
            _mockService.Setup(x => x.GetShopperHistoryAsync()).ReturnsAsync(shopHistory);

            var result = await _manager.GetProductsAsync(option);

            _mockService.Verify(x => x.GetShopperHistoryAsync(), Times.Once);
        }

        [Theory]
        [InlineData("Low")]
        [InlineData("Recommended")]
        public async void GetProductsAsync_ReturnsNull_WhenNullProductsReturned(string sortOption)
        {
            var option = (SortOption)Enum.Parse(typeof(SortOption), sortOption);
            List<Product> products = null;
            var shopHistory = new List<ShopperHistory>();
            _mockService.Setup(x => x.GetProductsAsync()).ReturnsAsync(products);
            _mockService.Setup(x => x.GetShopperHistoryAsync()).ReturnsAsync(shopHistory);

            var result = await _manager.GetProductsAsync(option);

            Assert.Null(result);
        }

        [Fact]
        public async void GetProductsAsync_ReturnsNull_WhenNullHistoryReturned()
        {
            var option = (SortOption)Enum.Parse(typeof(SortOption), "Recommended");
            var products = new List<Product>();
            List<ShopperHistory> shopHistory = null;
            _mockService.Setup(x => x.GetProductsAsync()).ReturnsAsync(products);
            _mockService.Setup(x => x.GetShopperHistoryAsync()).ReturnsAsync(shopHistory);

            var result = await _manager.GetProductsAsync(option);

            Assert.Null(result);
        }

        [Fact]
        public async void CalculateTrolley_ReturnsDecimal_WhenTrolleyProvided()
        {
            _mockService.Setup(x => x.CalculateTrolleyTotal(It.IsAny<Trolley>()))
                .ReturnsAsync((decimal)40.5);

            var result = await _manager.CalculateTrolleyTotal(GetTrolley());

            Assert.IsType<decimal>(result);
        }

        private TrolleyVM GetTrolley()
        {
            var name = "item 1";
            return new TrolleyVM
            {
                Products = new List<ProductBaseVM>
                {
                    new ProductBaseVM
                    {
                        Name = name,
                        Price = 10.5
                    }
                },
                Specials = new List<SpecialVM>
                {
                    new SpecialVM
                    {
                        Quantities = new List<ProductQuantityVM>
                        {
                            new ProductQuantityVM
                            {
                                Name = name,
                                Quantity = 3
                            }
                        },
                        Total = 30
                    }
                },
                Quantities = new List<ProductQuantityVM>
                {
                    new ProductQuantityVM
                    {
                        Name = name,
                        Quantity = 4
                    }
                }
            };
        }
    }
}
