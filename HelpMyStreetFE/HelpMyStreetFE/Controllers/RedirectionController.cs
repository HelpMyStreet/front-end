using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Controllers
{
    public class RedirectionController : Controller
    {
        private readonly ICommunicationService _communicationService;
        private readonly IAuthService _authService;

        public RedirectionController(ICommunicationService communicationService, IAuthService authService)
        {
            _communicationService = communicationService;
            _authService = authService;
        }

        [Route("link/{token}")]
        public async Task<IActionResult> Inbound (string token)
        {
            string destination = await _communicationService.GetLinkDestination(token);

            if (destination == null)
            {
                return Redirect("/link-expired"); 
            }

            _authService.PutSessionAuthorisedUrl(HttpContext, destination);

            return Redirect(destination);
        }
    }
}
