using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Account.Jobs;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Helpers;
using System.Collections.Generic;
using System.Linq;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Services;
using HelpMyStreetFE.Models.Account;

namespace HelpMyStreetFE.ViewComponents
{
    public class RequestDetailViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;
        private readonly IGroupService _groupService;
        private readonly IAddressService _addressService;
        public RequestDetailViewComponent(IRequestService requestService, IGroupService groupService, IAddressService addressService)
        {
            _requestService = requestService;
            _groupService = groupService;
            _addressService = addressService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int requestId, User user, JobSet jobSet, CancellationToken cancellationToken)
        {
            string viewName = "RequestDetail";

            RequestDetailViewModel requestDetailViewModel = new RequestDetailViewModel
            {
                User = user,
                UserRole = jobSet.GroupAdminView() ? RequestRoles.GroupAdmin : RequestRoles.Volunteer,
            };

            var requestDetail = await _requestService.GetRequestDetailAsync(requestId, user.ID, cancellationToken);

            // Temporary fix  TODO: Let users allocated to the task see RequestDetails
            if (requestDetail != null)
            {
                requestDetailViewModel.RequestSummary = requestDetail.RequestSummary;
                requestDetailViewModel.Requestor = requestDetail.Requestor;
                requestDetailViewModel.Recipient = requestDetail.Recipient;
            }
            else
            {
                requestDetailViewModel.RequestSummary = await _requestService.GetRequestSummaryAsync(requestId, cancellationToken);
            }

            requestDetailViewModel.GroupSupportActivityInstructions = await _groupService.GetAllGroupSupportActivityInstructions(requestDetailViewModel.RequestSummary.ReferringGroupID, requestDetail.RequestSummary.JobBasics.Select(j => j.SupportActivity).Distinct(), cancellationToken);


            switch (jobSet)
            {
                case JobSet.UserOpenRequests_MatchingCriteria:
                case JobSet.UserOpenRequests_NotMatchingCriteria:
                    requestDetailViewModel.JobsToShow = 
                        await _requestService.FilterAndDedupeOpenJobsForUser(requestDetailViewModel.RequestSummary.JobSummaries, user, cancellationToken);
                    viewName = "RequestDetail_OpenRequests";
                    break;
                case JobSet.UserMyRequests:
                    requestDetailViewModel.JobsToShow = requestDetail.RequestSummary.JobSummaries.GroupByDateAndActivity().OrderBy(j => j.Key)
                        .Select(g => g.Value.OrderByDescending(j => j.VolunteerUserID.Equals(user.ID)).ThenBy(j => j.JobStatus.UsualOrderOfProgression()).First());
                    viewName = "RequestDetail_MyRequests";
                    break;
                case JobSet.GroupRequests:
                case JobSet.GroupShifts:
                    var jobDetails = new List<JobDetail>();
                    foreach (var j in requestDetailViewModel.RequestSummary.JobBasics)
                    {
                        jobDetails.Add(await _requestService.GetJobDetailsAsync(j.JobID, user.ID, jobSet.GroupAdminView(), cancellationToken));
                    }
                    requestDetailViewModel.JobDetails = jobDetails;
                    break;
            }

            if (requestDetailViewModel.RequestSummary.Shift != null)
            {
                requestDetailViewModel.LocationDetails = await _addressService.GetLocationDetails(requestDetailViewModel.RequestSummary.Shift.Location, cancellationToken);
            }

            return View(viewName, requestDetailViewModel);
        }
    }
}
