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

        [AuthorizeAttributeNoRedirect]
        [Route("community/joinandgo/{groupIdEnc}")]
        public async Task<IActionResult> JoinAndGo(string groupIdEnc, CancellationToken cancellationToken)
        {
            var groupId = Int32.Parse(Base64Utils.Base64Decode(groupIdEnc));
            var group = (Groups)groupId;

            CommunityViewModel communityViewModel = await _communityRepository.GetCommunity(groupId, cancellationToken);

            if (communityViewModel == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            var user = await _authService.GetCurrentUser(cancellationToken);
            var outcome = await _groupMemberService.PostAssignRole(user.ID, groupId, GroupRoles.Member, -1, cancellationToken);
            communityViewModel.IsLoggedIn = true;
            communityViewModel.IsGroupMember = outcome == GroupPermissionOutcome.Success;

            return View(communityViewModel.View, communityViewModel);
        }

        public async Task<IActionResult> Index(string language, string groupKey, CancellationToken cancellationToken, bool suag = false)
        {
            if (String.IsNullOrWhiteSpace(groupKey))
            {
                return RedirectToAction(nameof(ErrorsController.Error404), "Errors");
            }

            var group = await _groupService.GetGroupByKey(groupKey, cancellationToken);
            CommunityViewModel communityViewModel = await _communityRepository.GetCommunity(groupKey, language, cancellationToken);

            if (communityViewModel == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            var user = await _authService.GetCurrentUser(cancellationToken);
            if (user != null)
            {
                communityViewModel.IsLoggedIn = true;
                communityViewModel.IsGroupMember = await _groupMemberService.GetUserHasRole(user.ID, group.GroupId, GroupRoles.Member, false, cancellationToken);
            }
            
            return View(communityViewModel.View, communityViewModel);
        }
    }
}
