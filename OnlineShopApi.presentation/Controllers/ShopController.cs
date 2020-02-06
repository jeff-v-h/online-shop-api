using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopApi.common;
using OnlineShopApi.domain.Managers;
using OnlineShopApi.domain.Models.ViewModels;
using OnlineShopApi.presentation.RequestModels;
using System.Collections.Generic;

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
        public ActionResult<UserVM> GetUser()
        {
            var doc = _manager.GetUser(null);
            return Ok(doc);
        }

        [HttpGet("sort")]
        [ProducesResponseType(typeof(List<ProductVM>), StatusCodes.Status200OK)]
        public ActionResult<List<ProductVM>> GetSortedProducts([FromQuery] SortOption sortOption)
        {
            var doc = _manager.GetProducts(sortOption);
            return Ok(doc);
        }

        //[HttpPost("trolleyTotal")]
        //[ProducesResponseType(typeof(TrolleyTotalVM), StatusCodes.Status201Created)]
        //public async Task<ActionResult<TrolleyTotalVM>> CreateShopListDetails(Trolley doc)
        //{
        //    await _manager.CreateListsDetails(doc);
        //    return CreatedAtAction(nameof(GetShopList), new { id = doc.Id }, doc);
        //}
    }
}
