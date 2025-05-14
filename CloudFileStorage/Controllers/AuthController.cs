using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudFileStorage.Services.Interfaces;
using CloudFileStorage.Models.DTOs;

namespace CloudFileStorage.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        // POST: api/Auth/login
        // <summary> Logins user. </summary>
        // <returns> Successfully logged in user. </returns>
        // <response code="200">Returns access token</response>
        // <response code="400">Bad request. Invalid credentials.</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] AuthRequest req)
        {
            var response = await _authService.LoginAsync(req);
            if (!response.success) return BadRequest(response.message);
            return Ok(response);
        }

        // POST: api/Auth/register
        // <summary> Registers user. </summary>
        // <returns> Successfully registered user. </returns>
        // <response code="200">Returns access token</response>
        // <response code="400">Bad request. Invalid credentials.</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] AuthRequest req)
        {
            var response = await _authService.RegisterAsync(req);
            if (!response.success) return BadRequest(response.message);
            return Ok(response);
        }
    }
}
