using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudFileStorage.Models;
using CloudFileStorage.Services.Interfaces;

namespace CloudFileStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        // POST: api/Auth/login
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest req)
        {
            var response = await _authService.LoginAsync(req);
            if (!response.success) return BadRequest(response.message);
            return Ok(response);
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequest req)
        {
            var response = await _authService.RegisterAsync(req);
            if (!response.success) return BadRequest(response.message);
            return Ok(response);
        }
    }
}
