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

        private const string Juice = "juice";
        private const string Shampoo = "shampoo";
        private const string Meat = "meat";
        private const string Wine = "wine";

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

        #region Tests for Calculation of Trolley
        [Fact]
        public async void CalculateTrolleyAsync_ReturnsDecimal_WhenTrolleyProvided()
        {
            _mockService.Setup(x => x.CalculateTrolleyTotal(It.IsAny<Trolley>()))
                .ReturnsAsync((decimal)40.5);

            var result = await _manager.CalculateTrolleyTotalAsync(GetTrolley());

            Assert.IsType<decimal>(result);
        }

        [Fact]
        public void CalculateTrolley_GetsCorrectValue_WithSingleItemSpecial()
        {
            var result = _manager.CalculateTrolleyTotal(GetTrolley());

            Assert.Equal(20.9000067M, result);
        }

        [Fact]
        public void CalculateTrolley_GetsCorrectValue_WithMultipleItemSpecials()
        {
            var result = _manager.CalculateTrolleyTotal(GetTrolleyWithMultipleItemSpecial());

            Assert.Equal(67.6000067M, result);
        }

        [Fact]
        public void CalculateTrolley_GetsValue_WhenItemHasFurtherReductionsIfMoreBought()
        {
            var result = _manager.CalculateTrolleyTotal(GetTrolleySameItemSpecialDiffQuantity());

            Assert.Equal(67.4000067M, result);
        }

        [Fact]
        public void CalculateTrolley_GetsValue_WhenItemAcrossSpecials()
        {
            var result = _manager.CalculateTrolleyTotal(GetTrolleyWithItemAcrossSpecials());

            Assert.Equal(66.7000067M, result);
        }
        #endregion

        #region Trolleys to test against
        private TrolleyVM GetTrolley()
        {
            return new TrolleyVM
            {
                Products = new List<ProductBaseVM>
                {
                    new ProductBaseVM
                    {
                        Name = Juice,
                        Price = 3.5M
                    },
                    new ProductBaseVM
                    {
                        Name = Shampoo,
                        Price = 5.0000067M
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
                                Name = Juice,
                                Quantity = 2
                            }
                        },
                        Total = 6.2M
                    }
                },
                Quantities = new List<ProductQuantityVM>
                {
                    new ProductQuantityVM
                    {
                        Name = Juice,
                        Quantity = 5
                    },
                    new ProductQuantityVM
                    {
                        Name = Shampoo,
                        Quantity = 1
                    }
                }
            };
        }

        private TrolleyVM GetTrolleyWithMultipleItemSpecial()
        {
            var trolley = GetTrolley();

            trolley.Products.Add(new ProductBaseVM
            {
                Name = Meat,
                Price = 10
            });
            trolley.Products.Add(new ProductBaseVM
            {
                Name = Wine,
                Price = 11.7M
            });

            trolley.Specials.Add(new SpecialVM
            {
                Quantities = new List<ProductQuantityVM>
                {
                    new ProductQuantityVM
                    {
                        Name = Meat,
                        Quantity = 3
                    },
                    new ProductQuantityVM
                    {
                        Name = Wine,
                        Quantity = 1
                    }
                },
                Total = 35
            });

            trolley.Quantities.Add(new ProductQuantityVM
            {
                Name = Meat,
                Quantity = 3
            });
            trolley.Quantities.Add(new ProductQuantityVM
            {
                Name = Wine,
                Quantity = 2
            });


            return trolley;
        }

        private TrolleyVM GetTrolleySameItemSpecialDiffQuantity()
        {
            var trolley = GetTrolleyWithMultipleItemSpecial();

            trolley.Specials.Add(new SpecialVM
            {
                Quantities = new List<ProductQuantityVM>
                {
                    new ProductQuantityVM
                    {
                        Name = Juice,
                        Quantity = 3
                    },
                },
                Total = 9.5M
            });

            return trolley;
        }

        private TrolleyVM GetTrolleyWithItemAcrossSpecials()
        {
            var trolley = GetTrolleySameItemSpecialDiffQuantity();

            trolley.Specials.Add(new SpecialVM
            {
                Quantities = new List<ProductQuantityVM>
                {
                    new ProductQuantityVM
                    {
                        Name = Wine,
                        Quantity = 2
                    },
                },
                Total = 16
            });

            return trolley;
        }
        #endregion
    }
}
