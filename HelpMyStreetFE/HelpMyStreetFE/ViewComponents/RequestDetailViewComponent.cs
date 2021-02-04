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

        public async Task<IViewComponentResult> InvokeAsync(int requestId, User user, JobSet jobSet, CancellationToken cancellationToken, bool toPrint = false)
        {
            if (!jobSet.GroupAdminView())
            {
                throw new Exception($"Unexpected JobSet: {jobSet}");
            }

            var requestDetail = await _requestService.GetRequestDetailAsync(requestId, user.ID, cancellationToken);

            LocationDetails locationDetails = null;
            if (requestDetail.RequestSummary.Shift != null)
            {
                locationDetails = await _addressService.GetLocationDetails(requestDetail.RequestSummary.Shift.Location, cancellationToken);
            }

            var jobDetails = await Task.WhenAll(requestDetail.RequestSummary.JobSummaries.Select(async j => await _requestService.GetJobDetailsAsync(j.JobID, user.ID, jobSet.GroupAdminView(), cancellationToken)));

            var instructions = await _groupService.GetAllGroupSupportActivityInstructions(requestDetail.RequestSummary.ReferringGroupID, jobDetails.Select(j => j.JobSummary.SupportActivity).Distinct(), cancellationToken);
 
            RequestDetailViewModel requestDetailViewModel = new RequestDetailViewModel()
            {
                RequestDetail = requestDetail,
                LocationDetails = locationDetails,
                JobDetails = jobDetails,
                GroupSupportActivityInstructions = instructions,
            };

            return View("RequestDetail", requestDetailViewModel);
        }
    }
}
