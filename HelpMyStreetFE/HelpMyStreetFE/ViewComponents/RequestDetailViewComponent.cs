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

            RequestDetailViewModel vm = new RequestDetailViewModel
            {
                User = user,
                UserRole = jobSet.GroupAdminView() ? RequestRoles.GroupAdmin : RequestRoles.Volunteer,
            };

            if (jobSet.PrivilegedView())
            {
                var requestDetail = await _requestService.GetRequestDetailAsync(requestId, user.ID, cancellationToken);

                vm.RequestSummary = requestDetail.RequestSummary;
                vm.Requestor = requestDetail.Requestor;
                vm.Recipient = requestDetail.Recipient;
            }
            else
            {
                vm.RequestSummary = await _requestService.GetRequestSummaryAsync(requestId, cancellationToken);
            }

            vm.GroupSupportActivityInstructions = await _groupService.GetAllGroupSupportActivityInstructions(vm.RequestSummary.ReferringGroupID, vm.RequestSummary.JobBasics.Select(j => j.SupportActivity).Distinct(), cancellationToken);


            switch (jobSet)
            {
                case JobSet.UserOpenRequests_MatchingCriteria:
                case JobSet.UserOpenRequests_NotMatchingCriteria:
                    vm.JobsToShow = 
                        await _requestService.FilterAndDedupeOpenJobsForUser(vm.RequestSummary.JobSummaries, user, cancellationToken);
                    viewName = "RequestDetail_OpenRequests";
                    break;
                case JobSet.UserMyRequests:
                    vm.JobsToShow = vm.RequestSummary.JobSummaries.GroupByDateAndActivity().OrderBy(j => j.Key)
                        .Select(g => g.Value.OrderByDescending(j => j.VolunteerUserID.Equals(user.ID)).ThenBy(j => j.JobStatus.UsualOrderOfProgression()).First());
                    viewName = "RequestDetail_MyRequests";
                    break;
                case JobSet.GroupRequests:
                case JobSet.GroupShifts:
                    var jobDetails = new List<JobDetail>();
                    foreach (var j in vm.RequestSummary.JobBasics)
                    {
                        jobDetails.Add(await _requestService.GetJobDetailsAsync(j.JobID, user.ID, jobSet.GroupAdminView(), cancellationToken));
                    }
                    vm.JobDetails = jobDetails;
                    break;
            }

            if (vm.RequestSummary.Shift != null)
            {
                vm.LocationDetails = await _addressService.GetLocationDetails(vm.RequestSummary.Shift.Location, cancellationToken);
            }

            return View(viewName, vm);
        }
    }
}
