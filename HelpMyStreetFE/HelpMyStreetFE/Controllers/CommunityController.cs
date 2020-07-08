using HelpMyStreetFE.Models.Community;
using HelpMyStreetFE.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Controllers
{
    public class CommunityController : Controller
    {
        private readonly ILogger<CommunityController> _logger;
        private readonly ICommunityRepository _communityRepository;

        public CommunityController(ILogger<CommunityController> logger, ICommunityRepository communityRepository)
        {
            _logger = logger;
            _communityRepository = communityRepository;
        }

        public async Task<IActionResult> Index(string communityName)
        {
            if (String.IsNullOrWhiteSpace(communityName))
            {
                return RedirectToAction(nameof(ErrorsController.Error404), "Errors");
            }

            CommunityViewModel communityViewModel = await _communityRepository.GetCommunity(communityName);

            if (communityViewModel == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            communityViewModel.IsLoggedIn = ((HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated);

            return View(communityViewModel);
        }


        public async Task<IActionResult> FaceMasks()
        {
            return RedirectPermanent("/for-the-love-of-scrubs");
        }
    }
}
