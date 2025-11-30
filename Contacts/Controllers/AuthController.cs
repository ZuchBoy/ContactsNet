using Contacts.Interfaces;
using Contacts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel contact)
        {
            var result = await _authService.Register(contact);
            if (!result)
                return BadRequest();

            return Ok();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
			var result = await _authService.Login(login) ?? "";
            if (string.IsNullOrEmpty(result))
                return Unauthorized();

            return Ok( new { Token = result });
        }
    }
}
