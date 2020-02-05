using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLP.Social.DataService.common.Models;
using PLP.Social.DataService.domain.Managers;
using PLP.Social.DataService.domain.Models.ViewModels;
using OnlineShopApi.presentation.RequestModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopApi.presentation.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private IUserManager _manager;
        private IMapper _mapper;

        public UserController(IUserManager manager, IMapper mapper)
        {
            _manager = manager;
            _mapper = mapper;
        }

        // POST /users/authenticate
        [AllowAnonymous]
        [HttpPost("authenticate")]
        [ProducesResponseType(typeof(UserVM), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Authenticate(AuthenticateModel model)
        {
            var user = _manager.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new ErrorResponse(400, $"Username and/or password is incorrect."));

            return Ok(user);
        }

        // GET /users
        [HttpGet]
        [ProducesResponseType(typeof(List<UserVM>), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var users = _manager.GetAll();
            return Ok(users);
        }

        // POST /users
        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [ProducesResponseType(typeof(UserVM), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserVM>> CreateUser(NewUser newUser)
        {
            var user = _mapper.Map<UserVM>(newUser);
            var userVM = await _manager.CreateUser(user, newUser.Password);
            if (userVM == null)
                return BadRequest(new ErrorResponse(400, $"Username already exists."));
            return CreatedAtAction(nameof(Authenticate), new { username = newUser.Username }, newUser);
        }

        // PUT /users/password
        [HttpPut("password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePassword(UpdatePassword details)
        {
            bool isSuccessful = await _manager.UpdatePassword(details.Username, details.OldPassword, details.NewPassword);
            if (!isSuccessful)
                return BadRequest(new ErrorResponse(400, $"Username and/or password is incorrect."));
            return NoContent();
        }

        // PUT /users
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser(UserVM user)
        {
            bool isSuccessful = await _manager.UpdateUser(user);
            if (!isSuccessful)
                return BadRequest(new ErrorResponse(400, $"User with username '{user.Username}' not found"));
            return NoContent();
        }

        // DELETE /users
        [Authorize(Roles = Role.Admin)]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser(UserVM user)
        {
            await _manager.DeleteUser(user.Username);
            return NoContent();
        }
    }
}
