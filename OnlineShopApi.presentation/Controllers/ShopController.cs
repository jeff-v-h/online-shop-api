using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopApi.domain.Models.AppModels;
using OnlineShopApi.domain.Managers;
using OnlineShopApi.domain.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            return Ok(doc);
        }

        [HttpGet("sort")]
        [ProducesResponseType(typeof(List<ProductVM>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSortedProducts([FromQuery] SortOption sortOption)
        {
            var doc = await _manager.GetProductsAsync(sortOption);
            return Ok(doc);
        }

        [HttpPost("trolleyTotal")]
        [ProducesResponseType(typeof(TrolleyVM), StatusCodes.Status201Created)]
        public async Task<IActionResult> CalculateTrolley(TrolleyVM trolley)
        {
            var total = await _manager.CalculateTrolleyTotal(trolley);
            return Ok(total);
        }
    }
}
