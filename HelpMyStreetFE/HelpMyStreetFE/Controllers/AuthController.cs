using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
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
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var fb = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("C:\\projects\\factor50-test-firebase-adminsdk-54f9y-450bc8e44a.json")
            });

            var auth = FirebaseAuth.GetAuth(fb);

            var decoded = await auth.VerifyIdTokenAsync(loginRequest.token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, decoded.Uid));

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return Ok();
        }
    }
}
