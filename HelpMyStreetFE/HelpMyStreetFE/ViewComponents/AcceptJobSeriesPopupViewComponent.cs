using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.ViewComponents
{
    public class AcceptJobSeriesPopupViewComponent : ViewComponent
    {
        private readonly IGroupMemberService _groupMemberService;
        private readonly IAuthService _authService;
        private readonly IRequestService _requestService;
        private readonly IGroupService _groupService;

        public AcceptJobSeriesPopupViewComponent(IGroupMemberService groupMemberService, IAuthService authService, IRequestService requestService, IGroupService groupService)
        {
            _groupMemberService = groupMemberService;
            _authService = authService;
            _requestService = requestService;
            _groupService = groupService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int requestId, int stage, CancellationToken cancellationToken)
        {
            var request = await _requestService.GetRequestSummaryAsync(requestId, cancellationToken);
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            AcceptJobSeriesPopupViewModel vm = await BuildVm(request, user, cancellationToken);

            return stage switch
            {
                1 => View("Stage1_Instructions", vm),
                2 => View("Stage2_SelectDates", vm),
                _ => throw new Exception($"Unexpected stage: {stage}")
            };
        }

        //private async Task<IViewComponentResult> AcceptRequestIfCredentialsSatisfied(JobStatusChangePopupViewModel vm, User user, CancellationToken cancellationToken)
        //{
        //    var credentials = await _groupMemberService.GetAnnotatedGroupActivityCredentials(vm.JobSummary.ReferringGroupID, vm.JobSummary.SupportActivity, user.ID, user.ID, cancellationToken);

        //    if (credentials.AreSatisfied)
        //    {
        //        return View("AcceptRequestPopup", vm);
        //    }
        //    else
        //    {
        //        vm.AnnotatedGroupActivityCredentialSets = credentials;
        //        return View("CredentialsRequiredPopup", vm);
        //    }
        //}

        private async Task<AcceptJobSeriesPopupViewModel> BuildVm(RequestSummary request, User user, CancellationToken cancellationToken)
        {
            var vm = new AcceptJobSeriesPopupViewModel()
            {
                RequestSummary = request,
                OpenJobsForUser = await _requestService.FilterAndDedupeOpenJobsForUser(request.JobSummaries, user, cancellationToken),
                GroupSupportActivityInstructions = await _groupService.GetGroupSupportActivityInstructions(request.ReferringGroupID, request.JobSummaries.First().SupportActivity, cancellationToken),
                RequestType = request.RequestType,
            };

            if (request.ReferringGroupID == (int)Groups.Generic)
            {
                vm.ReferringGroup = "HelpMyStreet.org";
            }
            else
            {
                var group = await _groupService.GetGroupById(request.ReferringGroupID, cancellationToken);
                vm.ReferringGroup = group.GroupName;
            }

            return vm;
        }
    }
}
