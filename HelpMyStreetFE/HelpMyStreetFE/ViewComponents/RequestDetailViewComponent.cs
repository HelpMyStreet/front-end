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
            var instructions = await _groupService.GetAllGroupSupportActivityInstructions(requestDetail.RequestSummary.ReferringGroupID, jobDetails.Select(j => j.SupportActivity).Distinct(), cancellationToken);
 
            RequestDetailViewModel requestDetailViewModel = new RequestDetailViewModel()
            {
                RequestDetail = requestDetail,
                JobDetails = showJobList ? jobDetails : new List<JobDetail>(),
                GroupSupportActivityInstructions = instructions,
            };

            if (requestDetail.RequestSummary.Shift != null)
            {
                requestDetailViewModel.LocationDetails = await _addressService.GetLocationDetails(requestDetail.RequestSummary.Shift.Location, cancellationToken);
            }

            return View("RequestDetail", requestDetailViewModel);
        }
    }
}
