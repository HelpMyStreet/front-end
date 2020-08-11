using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.ViewComponents
{
    public class JobListViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;
        private readonly IOptions<RequestSettings> _requestSettings;

        public JobListViewComponent(IRequestService requestService, IOptions<RequestSettings> requestSettings)
        {
            _requestService = requestService;
            _requestSettings = requestSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync(JobSet jobSet, int? groupId, Action emptyListCallback)
        {
            User user = HttpContext.Session.GetObjectFromJson<User>("User");

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            bool admin = false;
            IEnumerable<JobSummary> jobs = null;
            switch (jobSet)
            {
                case JobSet.GroupRequests:
                    jobs = await _requestService.GetGroupRequestsAsync(groupId.Value);
                    admin = true;
                    break;
                case JobSet.UserOpenRequests_MatchingCriteria:
                    jobs = (await _requestService.GetOpenJobsAsync(_requestSettings.Value.OpenRequestsRadius, _requestSettings.Value.MaxNonCriteriaOpenJobsToDisplay, user, HttpContext)).CriteriaJobs;
                    break;
                case JobSet.UserOpenRequests_NotMatchingCriteria:
                    jobs = (await _requestService.GetOpenJobsAsync(_requestSettings.Value.OpenRequestsRadius, _requestSettings.Value.MaxNonCriteriaOpenJobsToDisplay, user, HttpContext)).OtherJobs;
                    break;
                case JobSet.UserAcceptedRequests:
                    jobs = await _requestService.GetJobsForUserAsync(user.ID, HttpContext);
                    break;
                case JobSet.UserCompletedRequests:
                    jobs = await _requestService.GetJobsForUserAsync(user.ID, HttpContext);
                    break;
            }

            if (jobs.Count() == 0) { emptyListCallback.Invoke(); }

            var contactInformation = await _requestService.GetContactInformationForRequests(jobs.Select(a => a.JobID));

            var jobs2 = jobs.Select(a => new JobViewModel()
            {
                JobSummary = a,
                UserActingAsAdmin = admin,
                UserIsVerified = user.IsVerified ?? false,
                ContactInformation = contactInformation[a.JobID],
        });

            return View("JobList", jobs2);
        }
    }
}
