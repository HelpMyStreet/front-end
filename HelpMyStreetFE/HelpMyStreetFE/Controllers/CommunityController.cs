using HelpMyStreetFE.Models.Community;
using HelpMyStreetFE.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Web;
using System.Threading.Tasks;
using System.IO;

namespace HelpMyStreetFE.Controllers
{
    public class CommunityController : Controller
    {
        private readonly ILogger<CommunityController> _logger;
        private readonly ICommunityRepository _communityRepository;
        private readonly IWebHostEnvironment _env;
        private const string communityImageStore = "/img/community/";

        public CommunityController(ILogger<CommunityController> logger, ICommunityRepository communityRepository, IWebHostEnvironment env)
        {
            _env = env;
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
            
            string carousel1Path = _env.WebRootPath + communityImageStore + communityViewModel.HomeFolder + "/carousel1";
            string carousel2Path = _env.WebRootPath + communityImageStore + communityViewModel.HomeFolder + "/carousel2";

            if (Directory.Exists(carousel1Path))
            {
                communityViewModel.CarouselImages1 = Directory.EnumerateFiles(carousel1Path);
            }
            if (Directory.Exists(carousel2Path))
            {
                communityViewModel.CarouselImages2 = Directory.EnumerateFiles(carousel2Path);
            }

            return View(communityViewModel);
        }


        public async Task<IActionResult> FaceMasks()
        {
            return RedirectPermanent("/for-the-love-of-scrubs");
        }
    }
}
