using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.ViewComponents
{
    public class JobStatusChangePopupViewComponent : ViewComponent
    {
        private readonly IGroupMemberService _groupMemberService;
        private readonly IAuthService _authService;
        private readonly IRequestService _requestService;
        private readonly IGroupService _groupService;

        public JobStatusChangePopupViewComponent(IGroupMemberService groupMemberService, IAuthService authService, IRequestService requestService, IGroupService groupService)
        {
            _groupMemberService = groupMemberService;
            _authService = authService;
            _requestService = requestService;
            _groupService = groupService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int jobId, JobStatuses targetStatus, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            var job = await _requestService.GetJobSummaryAsync(jobId, cancellationToken);

            bool userIsAdmin = await _groupMemberService.GetUserHasRole(user.ID, job.ReferringGroupID, GroupRoles.TaskAdmin, cancellationToken);
            bool userIsAllocatedToTask = job.VolunteerUserID.GetValueOrDefault() == user.ID;

            JobStatusChangePopupViewModel vm = await BuildVm(job, targetStatus, cancellationToken);

            return (job.JobStatus, targetStatus, userIsAdmin, userIsAllocatedToTask) switch
            {
                (JobStatuses.Open, JobStatuses.InProgress, _, _   ) => await AcceptRequestIfCredentialsSatisfied(vm, user, cancellationToken),
                (JobStatuses.InProgress, JobStatuses.Done, _, true) => View("MarkAsCompletePopup", vm),
                (JobStatuses.InProgress, JobStatuses.Open, _, true) => View("CantDoPopup", vm),

                (JobStatuses.Done, JobStatuses.InProgress, true, _    ) => View("Admin_MarkAsInProgressPopup", vm),
                (JobStatuses.InProgress, JobStatuses.Done, true, false) => View("Admin_MarkAsCompletePopup", vm),
                (JobStatuses.InProgress, JobStatuses.Open, true, false) => View("Admin_MarkAsOpenPopup", vm),
                (_,                 JobStatuses.Cancelled, true, _    ) => View("Admin_CancelRequestPopup", vm),

                _ => throw new Exception($"Unhandled status/admin combination: {job.JobStatus} -> {vm.TargetStatus} / admin:{userIsAdmin} / allocated to task:{userIsAllocatedToTask}")
            };
        }

        private async Task<IViewComponentResult> AcceptRequestIfCredentialsSatisfied(JobStatusChangePopupViewModel vm, User user, CancellationToken cancellationToken)
        {
            var credentials = await _groupMemberService.GetAnnotatedGroupActivityCredentials(vm.JobSummary.ReferringGroupID, vm.JobSummary.SupportActivity, user.ID, user.ID, cancellationToken);

            if (credentials.AreSatisfied)
            {
                return View("AcceptRequestPopup", vm);
            }
            else
            {
                vm.AnnotatedGroupActivityCredentialSets = credentials;
                return View("CredentialsRequiredPopup", vm);
            }
        }

        private async Task<JobStatusChangePopupViewModel> BuildVm(JobSummary job, JobStatuses targetStatus, CancellationToken cancellationToken)
        {
            JobStatusChangePopupViewModel vm = new JobStatusChangePopupViewModel() { JobSummary = job, TargetStatus = targetStatus };

            if (job.ReferringGroupID != (int)Groups.Generic)
            {
                var group = await _groupService.GetGroupById(job.ReferringGroupID, cancellationToken);
                vm.ReferringGroup = group.GroupName;
            }

            return vm;
        }
    }
}
