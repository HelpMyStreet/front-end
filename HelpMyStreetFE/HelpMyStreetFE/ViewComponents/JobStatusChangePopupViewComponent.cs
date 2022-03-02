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
    public class JobStatusChangePopupViewComponent : ViewComponent
    {
        private readonly IGroupMemberService _groupMemberService;
        private readonly IAuthService _authService;
        private readonly IJobCachingService _jobCachingService;
            private readonly IRequestCachingService _requestCachingService;
        private readonly IGroupService _groupService;

        public JobStatusChangePopupViewComponent(
            IGroupMemberService groupMemberService,
            IAuthService authService,
            IRequestCachingService requestCachingService,
            IJobCachingService jobCachingService,
            IGroupService groupService)
        {
            _groupMemberService = groupMemberService;
            _authService = authService;
            _jobCachingService = jobCachingService;
            _requestCachingService = requestCachingService;
            _groupService = groupService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int jobId, int requestId, JobStatuses targetStatus, CancellationToken cancellationToken)
        {
            JobBasic job = null;
            if (jobId > 0)
            {
                job = await _jobCachingService.GetJobBasicAsync(jobId, cancellationToken);
                requestId = job.RequestID;
            }
            var request = await _requestCachingService.GetRequestSummaryAsync(requestId, cancellationToken);
            var user = await _authService.GetCurrentUser(cancellationToken);

            bool userIsAdmin = await _groupMemberService.GetUserHasRole(user.ID, request.ReferringGroupID, GroupRoles.TaskAdmin, true, cancellationToken);
            
            JobStatusChangePopupViewModel vm = await BuildVm(request, job, targetStatus, cancellationToken);

            if (job == null)
            {
                return (request.JobBasics.SingleJobStatus(), targetStatus, userIsAdmin) switch
                {
                    (_, JobStatuses.Done, true) => View("Admin_MarkRequestAsDonePopup", vm),
                    (_, JobStatuses.Cancelled, true) => View("Admin_CancelRequestPopup", vm),
                    _ => throw new Exception($"Unhandled status/admin combination for request {requestId}, user {user.ID}: {request.JobBasics.SingleJobStatus()} -> {targetStatus} / admin:{userIsAdmin}")
                };
            }
            else
            {
                bool userIsAllocatedToTask = job.VolunteerUserID.GetValueOrDefault() == user.ID;

                return (job?.JobStatus, targetStatus, userIsAdmin, userIsAllocatedToTask) switch
                {
                    (JobStatuses.Open, JobStatuses.AppliedFor, _, _   ) => await ApplyRequestIfCredentialsSatisfied(vm, user, cancellationToken),
                    (JobStatuses.Open, JobStatuses.InProgress, _, _   ) => await AcceptRequestIfCredentialsSatisfied(vm, user, cancellationToken),
                    (JobStatuses.InProgress, JobStatuses.Done, _, true) => View("MarkAsCompletePopup", vm),
                    (JobStatuses.Accepted,   JobStatuses.Open, _, true) => View("CantDoPopup", vm),
                    (JobStatuses.InProgress, JobStatuses.Open, _, true) => View("CantDoPopup", vm),

                    (JobStatuses.New,        JobStatuses.Open, true, _    ) => View("Admin_ApproveRequestPopup", vm),
                    (JobStatuses.Done, JobStatuses.InProgress, true, _    ) => View("Admin_MarkAsInProgressPopup", vm),
                    (JobStatuses.InProgress, JobStatuses.Done, true, false) => View("Admin_MarkAsCompletePopup", vm),
                    (JobStatuses.Accepted,   JobStatuses.Open, true, false) => View("Admin_MarkAsOpenPopup", vm),
                    (JobStatuses.InProgress, JobStatuses.Open, true, false) => View("Admin_MarkAsOpenPopup", vm),
                    (JobStatuses.AppliedFor, JobStatuses.InProgress, true, _) => View("Admin_MarkAsInProgressFromAppliedForPopup", vm),
                    (_,                 JobStatuses.Cancelled, true, _    ) => View("Admin_CancelJobPopup", vm),

                    _ => throw new Exception($"Unhandled status/admin combination for job {jobId}, user {user.ID}: {job.JobStatus} -> {targetStatus} / admin:{userIsAdmin} / allocated to task:{userIsAllocatedToTask}")
                };
            }
        }

        private async Task<IViewComponentResult> AcceptRequestIfCredentialsSatisfied(JobStatusChangePopupViewModel vm, User user, CancellationToken cancellationToken)
        {
            var credentials = await _groupMemberService.GetAnnotatedGroupActivityCredentials(vm.JobBasic.ReferringGroupID, vm.JobBasic.SupportActivity, user.ID, user.ID, cancellationToken);

            if (!credentials.AreSatisfied)
            {
                vm.AnnotatedGroupActivityCredentialSets = credentials;
                return View("CredentialsRequiredPopup", vm);
            }
            else
            {
                return View("AcceptRequestPopup", vm);
            }
        }

        private async Task<IViewComponentResult> ApplyRequestIfCredentialsSatisfied(JobStatusChangePopupViewModel vm, User user, CancellationToken cancellationToken)
        {
            var credentials = await _groupMemberService.GetAnnotatedGroupActivityCredentials(vm.JobBasic.ReferringGroupID, vm.JobBasic.SupportActivity, user.ID, user.ID, cancellationToken);

            if (!credentials.AreSatisfied)
            {
                vm.AnnotatedGroupActivityCredentialSets = credentials;
                return View("CredentialsRequiredPopup", vm);
            }
            else
            {
                return View("ApplyRequestPopup", vm);
            }
        }

        private async Task<JobStatusChangePopupViewModel> BuildVm(RequestSummary request, JobBasic job, JobStatuses targetStatus, CancellationToken cancellationToken)
        {
            JobStatusChangePopupViewModel vm = new JobStatusChangePopupViewModel()
            {
                RequestSummary = request,
                RequestType = request.RequestType,
                JobBasic = job,
                TargetStatus = targetStatus,
            };

            if (job != null)
            {
                vm.GroupSupportActivityInstructions = await _groupService.GetGroupSupportActivityInstructions(request.ReferringGroupID, job.SupportActivity, cancellationToken);
            }

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
