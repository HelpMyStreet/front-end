using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)        
        {
            try
            {
                await _authService.LoginWithTokenAsync(loginRequest.token, HttpContext);
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError("an error occured in login", ex);
                return StatusCode(500);
            }            
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout(HttpContext);
            return RedirectToAction("Index", "Home");            
        }
    }
}
