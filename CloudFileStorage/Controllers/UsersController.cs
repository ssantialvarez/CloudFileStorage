using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CloudFileStorage.Services.Interfaces;
using CloudFileStorage.Models.DTOs;

namespace CloudFileStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        // GET: api/Users/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var response = await _userService.GetMeAsync();
            return Ok(response);
        }

        // GET: api/Users/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var response = await _userService.GetUserByIdAsync(id);
            return Ok(response);
        }

        // PUT: api/Users/me
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("me")]
        public async Task<IActionResult> PutUser([FromBody] UpdateUserRequest req)
        {
            var response = await _userService.UpdateUserAsync(req);
            return Ok(response);
        }

        // DELETE: api/Users/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var response = await _userService.DeleteUserAsync(id);
            return Ok(response);
        }
        
    }
}
