using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopApi.domain.Managers;
using OnlineShopApi.domain.Models.ViewModels;

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
        public ActionResult<UserVM> GetUser([FromBody] int? id)
        {
            var customerId = (id.HasValue) ? id.Value : 1;
            var doc = _manager.GetUser(customerId);
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
