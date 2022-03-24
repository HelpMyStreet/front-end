using HelpMyStreetFE.Models.Account.Jobs;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Services;
using System.Linq;
using HelpMyStreet.Utils.Enums;
using System.Collections.Generic;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.EqualityComparers;
using HelpMyStreetFE.Services.Users;

namespace HelpMyStreetFE.ViewComponents
{
    public class JobDetailViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;
        private readonly IGroupService _groupService;
        private readonly IGroupMemberService _groupMemberService;
        private IEqualityComparer<JobBasic> _jobBasicEqualityComparer;
        private readonly IUserLocationService _userLocationService;

        public JobDetailViewComponent(
            IRequestService requestService,
            IGroupService groupService,
            IGroupMemberService groupMemberService,
            IUserLocationService userLocationService)
        {
            _userLocationService = userLocationService;
            _requestService = requestService;
            _groupService = groupService;
            _groupMemberService = groupMemberService;
            _jobBasicEqualityComparer = new JobBasicDedupeWithDate_EqualityComparer();
        }

        public async Task<IViewComponentResult> InvokeAsync(int jobId, User user, JobSet jobSet, CancellationToken cancellationToken, bool toPrint = false)
        {
            JobDetail jobDetails = await _requestService.GetJobAndRequestSummaryAsync(jobId, cancellationToken);

            if (jobSet.GroupAdminView() || jobSet.Equals(JobSet.UserMyShifts))
            {
                jobDetails = await _requestService.GetJobDetailsAsync(jobId, user.ID, jobSet.GroupAdminView(), cancellationToken);
            }

            if (jobDetails == null)
            {
                throw new Exception($"Failed to retrieve job details for JobId {jobId}");
            }

            var jobDetailViewModel = new JobDetailViewModel
            {
                JobDetail = new JobViewModel<JobDetail>
                {
                    Item = jobDetails,
                    User = user,
                    UserRole = jobSet.GroupAdminView() ? RequestRoles.GroupAdmin : RequestRoles.Volunteer,
                    UserHasRequiredCredentials = await _groupMemberService.GetUserHasCredentials(jobDetails.ReferringGroupID, jobDetails.SupportActivity, user.ID, user.ID, cancellationToken),
                },
                DuplicateJobs = jobDetails.RequestSummary.JobBasics.Where(j => _jobBasicEqualityComparer.Equals(j, jobDetails)),
                GroupSupportActivityInstructions = await _groupService.GetGroupSupportActivityInstructions(jobDetails.ReferringGroupID, jobDetails.SupportActivity, cancellationToken),
            };

            if (jobDetails.RequestSummary.Shift != null)
            {
                var userLocationDetails = await _userLocationService.GetLocationDetailsForUser(user, cancellationToken);
                jobDetailViewModel.JobDetail.Location = userLocationDetails.FirstOrDefault(l => l.Location.Equals(jobDetails.RequestSummary.Shift.Location));
            }

            string viewName = (jobSet, toPrint) switch
            {
                (JobSet.UserOpenRequests, false) => "JobDetail_OpenRequests",
                (JobSet.UserMyRequests, false) => "JobDetail_MyRequests",
                (JobSet.UserMyRequests, true) => "JobDetail_Print",
                (JobSet.UserOpenShifts, false) => "JobDetail_OpenShifts",
                (JobSet.UserMyShifts, false) => "JobDetail_MyShifts",
                (JobSet.GroupRequests, false) => "JobDetail_GroupRequests",
                (_, _) => throw new ArgumentException($"Unexpected JobSet value: {jobSet}", paramName: nameof(jobSet))
            };

            return View(viewName, jobDetailViewModel);
        }
    }
}
