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
using Microsoft.Extensions.Configuration;
using HelpMyStreetFE.Services;
using HelpMyStreet.Utils.Utils;
using System.Linq;
using System.Threading;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Users;

namespace HelpMyStreetFE.Controllers
{
    public class CommunityController : Controller
    {
        private readonly ILogger<CommunityController> _logger;
        private readonly ICommunityRepository _communityRepository;
        private readonly IWebHostEnvironment _env;
        private const string communityImageStore = "/img/community/";
        private readonly IConfiguration _configuration;
        private readonly IGroupService _groupService;
        private readonly IAuthService _authService;
        private readonly IGroupMemberService _groupMemberService;

        public CommunityController(ILogger<CommunityController> logger, ICommunityRepository communityRepository, IWebHostEnvironment env, IConfiguration configuration, IGroupService groupService, IAuthService authService, IGroupMemberService groupMemberService)
        {
            _env = env;
            _logger = logger;
            _communityRepository = communityRepository;
            _configuration = configuration;
            _groupService = groupService;
            _authService = authService;
            _groupMemberService = groupMemberService;
        }

        public async Task<IActionResult> Index(string communityName, CancellationToken cancellationToken)
        {
            if (String.IsNullOrWhiteSpace(communityName))
            {
                return RedirectToAction(nameof(ErrorsController.Error404), "Errors");
            }

            CommunityViewModel communityViewModel = await _communityRepository.GetCommunity(communityName, cancellationToken);

            if (communityViewModel == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (user != null)
            {
                communityViewModel.IsLoggedIn = true;
                communityViewModel.IsGroupMember = await _groupMemberService.GetUserHasRole(user.ID, communityViewModel.groupKey, GroupRoles.Member, cancellationToken);
            }
            
            string carousel1Path = _env.WebRootPath + communityImageStore + communityViewModel.HomeFolder + "/carousel1";
            string carousel2Path = _env.WebRootPath + communityImageStore + communityViewModel.HomeFolder + "/carousel2";
            string carousel3Path = _env.WebRootPath + communityImageStore + communityViewModel.HomeFolder + "/carousel3";

            if (Directory.Exists(carousel1Path))
            {
                communityViewModel.CarouselImages1 = Directory.EnumerateFiles(carousel1Path).OrderBy(x => x).Where(x => x.Contains("jpeg") || x.Contains("jpg") || x.Contains("png"));
            }
            if (Directory.Exists(carousel2Path))
            {
                communityViewModel.CarouselImages2 = Directory.EnumerateFiles(carousel2Path).OrderBy(x => x).Where(x => x.Contains("jpeg") || x.Contains("jpg") || x.Contains("png"));
            }
            if (Directory.Exists(carousel3Path))
            {
                communityViewModel.CarouselImages3 = Directory.EnumerateFiles(carousel3Path).OrderBy(x => x).Where(x => x.Contains("jpeg") || x.Contains("jpg") || x.Contains("png"));
            }

            return View(communityViewModel);
        }


        public async Task<IActionResult> FaceCoverings(CancellationToken cancellationToken)
        {
            var genericGetGroupByKeyResponse = await _groupService.GetGroupIdByKey("Generic", cancellationToken);
            var encodedGenericGroupId = Base64Utils.Base64Encode(genericGetGroupByKeyResponse);

            FaceCoveringsViewModel faceCoveringsViewModel = new FaceCoveringsViewModel()
            {
                GenericSignUpURL = $"/login/{encodedGenericGroupId}/face-masks",
                RequestHelpURL = $"/request-help/{encodedGenericGroupId}/face-masks",
            };

            return View(faceCoveringsViewModel);
        }

        public async Task<IActionResult> FaceMasks(CancellationToken cancellationToken)
        {
            return Redirect("/face-coverings");
        }
    }
}
