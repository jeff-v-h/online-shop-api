using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopApi.domain.Models.AppModels;
using OnlineShopApi.domain.Managers;
using OnlineShopApi.domain.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineShopApi.presentation.RequestModels;

namespace OnlineShopApi.presentation.Controllers
{
    [Produces("application/json")]
    [Route("api/shop")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly IShopManager _manager;

        public ShopController(IShopManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        [HttpGet("user")]
        [ProducesResponseType(typeof(UserVM), StatusCodes.Status200OK)]
        public IActionResult GetUser()
        {
            var doc = _manager.GetUser(null);
            if (doc == null) return NotFound(new ErrorResponse(404, $"User was not found."));
            return Ok(doc);
        }

        [HttpGet("sort")]
        [ProducesResponseType(typeof(List<ProductVM>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSortedProducts([FromQuery] SortOption sortOption)
        {
            var products = await _manager.GetProductsAsync(sortOption);
            if (products == null)
            {
                var response = "Products";
                if (sortOption == SortOption.Recommended) response += " or shopping history";
                response += " could not be found";
                return NotFound(new ErrorResponse(404, response));
            }
            return Ok(products);
        }

        [HttpPost("trolleyTotal")]
        [ProducesResponseType(typeof(TrolleyVM), StatusCodes.Status201Created)]
        public IActionResult CalculateTrolley(TrolleyVM trolley)
        {
            var total = _manager.CalculateTrolleyTotal(trolley);
            return Ok(total);
        }
    }
}
