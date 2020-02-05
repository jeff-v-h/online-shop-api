using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopApi.domain.Managers;
using OnlineShopApi.domain.Models.ViewModels;
using OnlineShopApi.presentation.RequestModels;
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
        [ProducesResponseType(typeof(ShopListVM), StatusCodes.Status200OK)]
        public ActionResult<ShopListVM> GetShopList([FromBody] string token)
        {
            var doc = _manager.Get();
            return Ok(doc);
        }

        //[HttpGet]
        //[HttpGet("sort")]
        //[ProducesResponseType(typeof(ShopListVM), StatusCodes.Status200OK)]
        //public ActionResult<ShopListVM> GetShopList([FromQuery] string sortOptions)
        //{
        //    var doc = _manager.Get(sortOptions);
        //    return Ok(doc);
        //}

        //[HttpPost("trolleyTotal")]
        //[ProducesResponseType(typeof(TrolleyTotalVM), StatusCodes.Status201Created)]
        //public async Task<ActionResult<TrolleyTotalVM>> CreateShopListDetails(Trolley doc)
        //{
        //    await _manager.CreateListsDetails(doc);
        //    return CreatedAtAction(nameof(GetShopList), new { id = doc.Id }, doc);
        //}
    }
}
