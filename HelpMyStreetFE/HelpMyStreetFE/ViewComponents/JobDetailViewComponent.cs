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
using HelpMyStreetFE.Services;

namespace HelpMyStreetFE.ViewComponents
{
    public class JobDetailViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;
        private readonly IGroupService _groupService;
        private readonly IAddressService _addressService;

        public JobDetailViewComponent(IRequestService requestService, IGroupService groupService, IAddressService addressService)
        {
            _requestService = requestService;
            _groupService = groupService;
            _addressService = addressService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int jobId, User user, JobSet jobSet, CancellationToken cancellationToken, bool toPrint = false)
        {
            JobDetail jobDetails = jobSet.PrivilegedView() switch
            {
                true => await _requestService.GetJobDetailsAsync(jobId, user.ID, cancellationToken),
                false => await _requestService.GetJobAndRequestSummaryAsync(jobId, cancellationToken)
            };

            if (jobDetails == null)
            {
                throw new Exception($"Failed to retrieve job details for JobId {jobId}");
            }

            LocationDetails locationDetails = null;
            if (jobDetails.RequestSummary.Shift != null)
            {
                locationDetails = await _addressService.GetLocationDetails(jobDetails.RequestSummary.Shift.Location, cancellationToken);
            }

            JobDetailViewModel jobDetailViewModel = new JobDetailViewModel()
            {
                JobDetail = jobDetails,
                LocationDetails = locationDetails,
                UserActingAsAdmin = jobSet == JobSet.GroupRequests,
                GroupSupportActivityInstructions = await _groupService.GetGroupSupportActivityInstructions(jobDetails.JobSummary.ReferringGroupID, jobDetails.JobSummary.SupportActivity, cancellationToken),
                ToPrint = toPrint
            };

            return View("JobDetail", jobDetailViewModel);
        }
    }
}
