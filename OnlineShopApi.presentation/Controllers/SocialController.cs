using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLP.Social.DataService.domain.Managers;
using PLP.Social.DataService.domain.Models.ViewModels;
using OnlineShopApi.presentation.RequestModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopApi.presentation.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/sociallists")]
    [ApiController]
    public class SocialController : ControllerBase
    {
        private readonly ISocialManager _manager;

        public SocialController(ISocialManager manager)
        {
            _manager = manager;
        }

        // Get details for social media lists
        // GET: /api/sociallists/details
        [HttpGet("details")]
        [ProducesResponseType(typeof(SocialMediaListsDetailsVM), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SocialMediaListsDetailsVM), StatusCodes.Status404NotFound)]
        public ActionResult<SocialMediaListsDetailsVM> GetSocialListsDetails()
        {
            var doc = _manager.GetListsDetails();
            return Ok(doc);
        }

        // Create a new list of social media posts
        // POST: /api/sociallists/details/publish
        [HttpPost("details/publish")]
        [ProducesResponseType(typeof(SocialMediaListsDetailsVM), StatusCodes.Status204NoContent)]
        public IActionResult PublishAllSocialLists()
        {
            _manager.PublishAllLists();
            return NoContent();
        }

        // Create a new list of social media posts
        // POST: /api/sociallists/details
        [HttpPost("details")]
        [ProducesResponseType(typeof(SocialMediaListsDetailsVM), StatusCodes.Status201Created)]
        public async Task<ActionResult<SocialMediaListsDetailsVM>> CreateSocialListDetails(SocialMediaListsDetailsVM doc)
        {
            await _manager.CreateListsDetails(doc);
            return CreatedAtAction(nameof(GetSocialList), new { id = doc.Id }, doc);
        }

        // PUT: /api/sociallists/details
        [HttpPut("details")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateSocialListDetails(SocialMediaListsDetailsVM doc)
        {
            await _manager.UpdateListsDetails(doc);
            return NoContent();
        }

        // Get a single list of social media posts
        // GET: /api/sociallists/5dd69c3c17fce357dc82444e
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SocialListVM), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SocialListVM), StatusCodes.Status404NotFound)]
        public ActionResult<SocialListVM> GetSocialList(string id)
        {
            var doc = _manager.Get(id);
            if (doc == null)
                return NotFound(new ErrorResponse(404, $"List with id {id} could not be found."));
            return Ok(doc);
        }

        // Get all the social media lists
        // GET: /api/sociallists
        [HttpGet]
        [ProducesResponseType(typeof(SocialMediaListsVM), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SocialMediaListsVM), StatusCodes.Status404NotFound)]
        public ActionResult<SocialMediaListsVM> GetSocialLists()
        {
            var doc = _manager.Get();
            return Ok(doc);
        }

        // Create a new list of social media posts
        // POST: /api/sociallists
        [HttpPost]
        [ProducesResponseType(typeof(SocialListVM), StatusCodes.Status201Created)]
        public async Task<ActionResult<SocialListVM>> CreateSocialList(SocialListVM doc)
        {
            await _manager.Create(doc);
            return CreatedAtAction(nameof(GetSocialList), new { id = doc.Id }, doc);
        }

        // POST: /api/sociallists/5dd69c3c17fce357dc82444e/multipleitems
        [HttpPost("{id}/multipleitems")]
        [ProducesResponseType(typeof(MediaPostVM), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddItemsToSocialList(string id, List<MediaPostVM> posts)
        {
            await _manager.AddItemsToList(id, posts);
            return CreatedAtAction(nameof(GetSocialList), new { id = id }, posts);
        }

        // POST: /api/sociallists/5dd69c3c17fce357dc82444e/items
        [HttpPost("{id}/items")]
        [ProducesResponseType(typeof(MediaPostVM), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddItemToSocialList(string id, MediaPostVM doc)
        {
            await _manager.AddItemToList(id, doc);
            return CreatedAtAction(nameof(GetSocialList), new { id = id }, doc);
        }

        // POST: /api/sociallists/5dd69c9344750a67188ea1c2/publish
        [HttpPost("{id}/publish")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult PublishSocialList(string id)
        {
            var isPublished = _manager.Publish(id);
            if (isPublished == null)
                return NotFound(new ErrorResponse(404, $"List with id '{id}' could not be found."));
            else if (isPublished == false)
                return StatusCode(500);
            else
                return NoContent();
        }

        // Delete: /api/sociallists/5dd69c9344750a67188ea1c2/items/tw20191123
        [HttpDelete("{listId}/items/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteItemFromSocialList(string listId, string id)
        {
            await _manager.DeleteItemFromList(listId, id);
            return NoContent();
        }

        // PUT: /api/sociallists/5dd69c9344750a67188ea1c2/publish
        [HttpPut("{id}/publish")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAndPublishSocialList(string id, SocialListVM doc)
        {
            if (id != doc.Id) return BadRequest(new ErrorResponse(400, "Ids do not match"));
            await _manager.UpdateAndPublish(id, doc);
            return NoContent();
        }

        // PUT: /api/sociallists/5dd69c9344750a67188ea1c2
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSocialList(string id, SocialListVM doc)
        {
            if (id != doc.Id) return BadRequest(new ErrorResponse(400, "Ids do not match"));
            await _manager.Update(id, doc);
            return NoContent();
        }

        // DELETE: /api/sociallists/5dd69c9344750a67188ea1c2
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteSocialList(string id)
        {
            await _manager.Delete(id);
            return NoContent();
        }
    }
}
