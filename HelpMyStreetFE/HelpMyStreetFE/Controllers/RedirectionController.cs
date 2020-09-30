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

        public RedirectionController(ICommunicationService communicationService)
        {
            _communicationService = communicationService;
        }

        [Route("link/{token}")]
        public async Task<IActionResult> Inbound (string token)
        {
            string destination = await _communicationService.GetLinkDestination(token);

            if (destination == null)
            {
                return Redirect("/link-expired"); 
            }

            return Redirect(destination);
        }
    }
}
