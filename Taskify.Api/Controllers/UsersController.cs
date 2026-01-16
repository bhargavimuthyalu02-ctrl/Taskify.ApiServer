using Microsoft.AspNetCore.Mvc;
using Taskify.Service;
using Taskify.Api.Controllers;
using Taskify.Api.Controllers.DTOs;

namespace Taskify.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = await _userService.RegisterAsync(dto.UserEmail, dto.Password, dto.Role ?? "User");
            return Ok(new { user.Id, user.UserEmail });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _userService.LoginAsync(dto.UserEmail, dto.Password);
            if (token == null) return Unauthorized();
            return Ok(new { token });
        }
    }

 
}
