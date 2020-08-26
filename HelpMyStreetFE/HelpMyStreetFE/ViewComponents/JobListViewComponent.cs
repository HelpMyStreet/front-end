using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public async Task<IViewComponentResult> InvokeAsync(JobSet jobSet, int? groupId, JobFilterRequest jobFilterRequest, Action emptyListCallback, CancellationToken cancellationToken)
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
                    jobs = await _requestService.GetGroupRequestsAsync(groupId.Value, cancellationToken);
                    admin = true;
                    break;
                case JobSet.UserOpenRequests_MatchingCriteria:
                    jobs = (await _requestService.GetOpenJobsAsync(user, cancellationToken)).CriteriaJobs;
                    break;
                case JobSet.UserOpenRequests_NotMatchingCriteria:
                    jobs = (await _requestService.GetOpenJobsAsync(user, cancellationToken)).OtherJobs;
                    break;
                case JobSet.UserAcceptedRequests:
                    jobs = await _requestService.GetJobsForUserAsync(user.ID, cancellationToken);
                    break;
                case JobSet.UserCompletedRequests:
                    jobs = await _requestService.GetJobsForUserAsync(user.ID, cancellationToken);
                    break;
            }

            if (jobFilterRequest != null)
            {
                jobs = _requestService.FilterJobs(jobs, jobFilterRequest);
            }

            if (jobs.Count() == 0 && emptyListCallback != null) { emptyListCallback.Invoke(); }

            var jobs2 = jobs.Select(a => new JobViewModel()
            {
                JobSummary = a,
                UserActingAsAdmin = admin,
                UserIsVerified = user.IsVerified ?? false
            });

            return View("JobList", jobs2);
        }
    }
}
