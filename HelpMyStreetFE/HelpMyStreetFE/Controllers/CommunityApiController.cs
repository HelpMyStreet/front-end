using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services.Groups;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Controllers
{
    [ApiController]
    [Route("api/community")]
    public class CommunityApiController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly ICommunityRepository _communityRepository;

        public CommunityApiController(IGroupService groupService, ICommunityRepository communityRepository)
        {
            _groupService = groupService ?? throw new ArgumentNullException(nameof(groupService));
            _communityRepository = communityRepository ?? throw new ArgumentNullException(nameof(communityRepository));
        }

        [Route("get-request-help-community-popup")]
        public async Task<IActionResult> GetRequestHelpCommunityPopup(string g, CancellationToken cancellationToken)
        {
            int groupId = Base64Utils.Base64DecodeToInt(g);
            var group = await _groupService.GetGroupById(groupId, cancellationToken);
            var community = await _communityRepository.GetCommunityDetailByKey(group.GroupKey);

            return PartialView("_RequestHelpCommunityPopup", community);
        }

        [Route("get-request-help-elsewhere-popup")]
        public async Task<IActionResult> GetRequestHelpElsewherePopup(string g, CancellationToken cancellationToken)
        {
            int groupId = Base64Utils.Base64DecodeToInt(g);
            var group = await _groupService.GetGroupById(groupId, cancellationToken);
            var community = await _communityRepository.GetCommunityDetailByKey(group.GroupKey);

            return PartialView("_RequestHelpElsewherePopup", community);
        }

        [AuthorizeAttributeNoRedirect]
        [Route("get-join-group-popup")]
        public async Task<IActionResult> GetJoinGroupPopup(string g, CancellationToken cancellationToken)
        {
            int groupId = Base64Utils.Base64DecodeToInt(g);
            var group = await _groupService.GetGroupById(groupId, cancellationToken);
            var community = await _communityRepository.GetCommunityDetailByKey(group.GroupKey);

            return PartialView("_JoinGroupPopup", community);
        }

        [AuthorizeAttributeNoRedirect]
        [Route("get-leave-group-popup")]
        public async Task<IActionResult> GetLeaveGroupPopup(string g, CancellationToken cancellationToken)
        {
            int groupId = Base64Utils.Base64DecodeToInt(g);
            var group = await _groupService.GetGroupById(groupId, cancellationToken);
            var community = await _communityRepository.GetCommunityDetailByKey(group.GroupKey);

            return PartialView("_LeaveGroupPopup", community);
        }
    }
}
