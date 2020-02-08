using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineShopApi.domain.Managers;
using OnlineShopApi.domain.Models.AppModels;
using OnlineShopApi.domain.Models.ViewModels;
using OnlineShopApi.presentation.Controllers;
using System;
using System.Collections.Generic;
using Xunit;

namespace OnlineShopApi.presentation.test
{
    public class ShopControllerTests
    {
        private readonly Mock<IShopManager> _mockManager;
        private ShopController _controller;

        public ShopControllerTests()
        {
            _mockManager = new Mock<IShopManager>();
            _controller = new ShopController(_mockManager.Object);
        }

        [Fact]
        public void GetUser_ReturnsOk_WithUser()
        {
            // Arrange
            _mockManager.Setup(x => x.GetUser(null)).Returns(new UserVM());

            // Act
            IActionResult result = _controller.GetUser();

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<UserVM>(objectResult.Value);
        }

        [Fact]
        public void GetUser_ReturnsNotFound_WhenNullResponse()
        {
            UserVM user = null;
            _mockManager.Setup(x => x.GetUser(null)).Returns(user);

            IActionResult result = _controller.GetUser();

            var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Theory]
        [InlineData("Low")]
        [InlineData("High")]
        [InlineData("Ascending")]
        [InlineData("Descending")]
        [InlineData("Recommended")]
        public async void GetSortedProducts_ReturnsOk_WithListOfProducts(string sortOption)
        {
            var option = (SortOption)Enum.Parse(typeof(SortOption), sortOption);
            var products = new List<ProductVM>();
            _mockManager.Setup(x => x.GetProductsAsync(It.IsAny<SortOption>()))
                .ReturnsAsync(products);

            IActionResult result = await _controller.GetSortedProducts(option);

            var objectResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<ProductVM>>(objectResult.Value);
        }

        [Theory]
        [InlineData("Low")]
        [InlineData("Recommended")]
        public async void GetSortedProducts_ReturnsNotFound_WhenNullResponse(string sortOption)
        {
            var option = (SortOption)Enum.Parse(typeof(SortOption), sortOption);
            List<ProductVM> products = null;
            _mockManager.Setup(x => x.GetProductsAsync(It.IsAny<SortOption>()))
                .ReturnsAsync(products);

            IActionResult result = await _controller.GetSortedProducts(option);

            var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async void CalculateTrolley_ReturnsOk_WithDecimal()
        {
            _mockManager.Setup(x => x.CalculateTrolleyTotal(It.IsAny<TrolleyVM>()))
                .ReturnsAsync((decimal)40.5);

            IActionResult result = await _controller.CalculateTrolley(GetTrolley());

            var objectResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<decimal>(objectResult.Value);
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
