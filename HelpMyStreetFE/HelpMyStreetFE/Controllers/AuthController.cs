using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Controllers
{
    public class LoginRequest
    {
        public string token { get; set; }
    }
        
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            await _authService.LoginViaToken(loginRequest.token, HttpContext);

            return Ok();
        }
    }
}
