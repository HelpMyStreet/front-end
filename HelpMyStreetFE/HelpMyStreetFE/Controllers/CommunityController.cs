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

            CommunityViewModel communityViewModel = _communityRepository.GetCommunity(communityName);

            if (communityViewModel == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            communityViewModel.IsLoggedIn = ((HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated);

            return View(communityViewModel);
        }


        public async Task<IActionResult> FaceMasks()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();
            communityViewModel.Header = "Help My Street's Community Sew";
            communityViewModel.RequestHelpHeading = "Do you need a face covering?";
            communityViewModel.RequestHelpText = "If you need face coverings for yourself, your family or your organisation request help now, wheter 1 or 100 we'll do our best to help.";
            communityViewModel.ProvideHelpHeading = "Volunteer with us!";
            communityViewModel.ProvideHelpText = "If you're a home-sewer and able to make face coverings as a donation or the cost of materials, join us to help your neighbours. You can choose to help a little, or to help a lot! We're grateful for every contribution";

            communityViewModel.UsefulLinksHtml = new List<string>
            {
                "<a href=''> The Big Community Sew </a>",
                "<a href=''> Downloadable florence fitted facemask pattern from freesewing.org</a>",
                "<a href=''> Video: Pleated facemask with fabric ties from jenny Doan or missouri star quit company</a>",
            };


            if (User.Identity.IsAuthenticated)
            {
                communityViewModel.IsLoggedIn = true;
            }
            return View(communityViewModel);
        }
    }
}
