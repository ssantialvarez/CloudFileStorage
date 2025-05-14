using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CloudFileStorage.Services.Interfaces;
using CloudFileStorage.Models.DTOs;

namespace CloudFileStorage.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }


        // GET: api/Users
        /// <summary>
        /// Gets a list of all users. Admin only.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns all users</response>
        /// <response code="401">Unauthorized. Login or register.</response>
        /// <response code="403">Forbidden. Admin Only.</response>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        // GET: api/Users/me
        /// <summary>
        /// Gets information of current user.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns all users</response>
        /// <response code="400">Bad request. Invalid credentials.</response>
        /// <response code="401">Unauthorized. Login or register.</response>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]      
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var response = await _userService.GetMeAsync();
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        // GET: api/Users/5
        /// <summary>
        /// Gets information of user by its id. Admin Only.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns all users</response>
        /// <response code="400">Bad request. Invalid credentials.</response>  
        /// <response code="401">Unauthorized. Login or register.</response>
        /// <response code="403">Forbidden. Admin Only.</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]      
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var response = await _userService.GetUserByIdAsync(id);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        // PUT: api/Users/me
        /// <summary>
        /// Updates user´s information.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns all users</response>
        /// <response code="400">Bad request. Invalid credentials.</response> 
        /// <response code="401">Unauthorized. Login or register.</response>
        [HttpPut("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]      
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PutUser([FromBody] UpdateUserRequest req)
        {
            try
            {
                var response = await _userService.UpdateUserAsync(req);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);           
            }
            
        }

        // DELETE: api/Users/5
        /// <summary>
        /// Deletes user by id. Admin only.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns all users</response>
        /// <response code="400">Bad request. Invalid credentials.</response> 
        /// <response code="401">Unauthorized. Login or register.</response>
        /// <response code="403">Forbidden. Admin Only.</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]      
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var response = await _userService.DeleteUserAsync(id);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);           
            }
        }

        // DELETE: api/Users/me
        /// <summary>
        /// Deletes current user.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns all users</response>
        /// <response code="400">Bad request. Invalid credentials.</response>       
        /// <response code="401">Unauthorized. Login or register.</response>
        [HttpDelete("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]       
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteUser()
        {
            try
            {
                var response = await _userService.DeleteOwnUserAsync();
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }
        
    }
}
