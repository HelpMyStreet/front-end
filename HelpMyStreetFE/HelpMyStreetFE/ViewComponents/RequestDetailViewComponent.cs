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

        public async Task<IViewComponentResult> InvokeAsync(int requestId, User user, JobSet jobSet, CancellationToken cancellationToken, bool toPrint = false, bool showJobList = true)
        {
            //if (!jobSet.GroupAdminView())
            //{
            //    throw new Exception($"Unexpected JobSet: {jobSet}");
            //}


            switch (jobSet)
            {
                case JobSet.UserOpenRequests_MatchingCriteria:
                case JobSet.UserOpenRequests_NotMatchingCriteria:
                    return await InvokeAsync_OpenRequests(requestId, user, cancellationToken);
                case JobSet.UserMyRequests:
                    return await InvokeAsync_MyRequests(requestId, user, cancellationToken);
            }


            var requestDetail = await _requestService.GetRequestDetailAsync(requestId, user.ID, cancellationToken);

            // Temporary fix  TODO: Let users allocated to the task see RequestDetails
            if (requestDetail == null)
            {
                requestDetail = new HelpMyStreet.Contracts.RequestService.Response.GetRequestDetailsResponse
                {
                    RequestSummary = await _requestService.GetRequestSummaryAsync(requestId, cancellationToken)
                };
            }


            var jobDetails = new List<JobDetail>();

            if (showJobList)
            {
                foreach (var j in requestDetail.RequestSummary.JobBasics)
                {
                    if (jobSet.GroupAdminView() || j.VolunteerUserID == user.ID)
                    {
                        jobDetails.Add(await _requestService.GetJobDetailsAsync(j.JobID, user.ID, jobSet.GroupAdminView(), cancellationToken));
                    }
                    else
                    {
                        //JobDetail jobDetail = (JobDetail)await _requestService.GetJobSummaryAsync(j.JobID, cancellationToken);
                        //jobDetail.RequestSummary = requestDetail.RequestSummary;

                        //jobDetails.Add(jobDetail);

                        jobDetails.Add(new JobDetail(await _requestService.GetJobSummaryAsync(j.JobID, cancellationToken))
                        {
                            //JobSummary = await _requestService.GetJobSummaryAsync(j.JobID, cancellationToken),
                            RequestSummary = requestDetail.RequestSummary,
                        });
                    }
                }
            }

            var instructions = await _groupService.GetAllGroupSupportActivityInstructions(requestDetail.RequestSummary.ReferringGroupID, requestDetail.RequestSummary.JobBasics.Select(j => j.SupportActivity).Distinct(), cancellationToken);
 
            RequestDetailViewModel requestDetailViewModel = new RequestDetailViewModel()
            {
                RequestDetail = requestDetail,
                JobDetails = jobDetails,
                UserRole = jobSet.GroupAdminView() ? RequestRoles.GroupAdmin : RequestRoles.Volunteer,
                GroupSupportActivityInstructions = instructions,
            };

            if (requestDetail.RequestSummary.Shift != null)
            {
                requestDetailViewModel.LocationDetails = await _addressService.GetLocationDetails(requestDetail.RequestSummary.Shift.Location, cancellationToken);
            }

            return View("RequestDetail", requestDetailViewModel);
        }

        private async Task<IViewComponentResult> InvokeAsync_OpenRequests(int requestId, User user, CancellationToken cancellationToken)
        {
            var requestSummary = await _requestService.GetRequestSummaryAsync(requestId, cancellationToken);

            var instructions = await _groupService.GetAllGroupSupportActivityInstructions(requestSummary.ReferringGroupID,
                requestSummary.JobBasics.Select(j => j.SupportActivity).Distinct(), cancellationToken);

            RequestDetailViewModel requestDetailViewModel = new RequestDetailViewModel()
            {
                JobsToShow = await _requestService.FilterAndDedupeOpenJobsForUser(requestSummary.JobSummaries, user, cancellationToken),
                UserRole = RequestRoles.Volunteer,
                GroupSupportActivityInstructions = instructions,
            };

            return View("RequestDetail_OpenRequests", requestDetailViewModel);
        }

        private async Task<IViewComponentResult> InvokeAsync_MyRequests(int requestId, User user, CancellationToken cancellationToken)
        {
            var requestDetail = await _requestService.GetRequestDetailAsync(requestId, user.ID, cancellationToken);
            //var requestSummary = await _requestService.GetRequestSummaryAsync(requestId, cancellationToken);

            var instructions = await _groupService.GetAllGroupSupportActivityInstructions(requestDetail.RequestSummary.ReferringGroupID,
                requestDetail.RequestSummary.JobBasics.Select(j => j.SupportActivity).Distinct(), cancellationToken);

            RequestDetailViewModel requestDetailViewModel = new RequestDetailViewModel()
            {
                RequestDetail = requestDetail,
                User = user,
                JobsToShow = requestDetail.RequestSummary.JobSummaries.GroupByDateAndActivity().OrderBy(j => j.Key)
                    .Select(g => g.Value.OrderByDescending(j => j.VolunteerUserID.Equals(user.ID)).ThenBy(j => j.JobStatus.UsualOrderOfProgression()).First()),
                UserRole = RequestRoles.Volunteer,
                GroupSupportActivityInstructions = instructions,
            };

            return View("RequestDetail_MyRequests", requestDetailViewModel);
        }
    }
}
