using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Users;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Account.Volunteers;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreet.Utils.Utils;

namespace HelpMyStreetFE.ViewComponents
{
    public class ViewLocationPopupViewComponent : ViewComponent
    {
        private readonly IAuthService _authService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        private readonly IRequestService _requestService;
        private readonly IAddressService _addressService;

        public ViewLocationPopupViewComponent(IAddressService addressService, IRequestService requestService, IAuthService authService, IGroupMemberService groupMemberService, IUserService userService, IGroupService groupService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _groupMemberService = groupMemberService ?? throw new ArgumentNullException(nameof(groupMemberService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _groupService = groupService ?? throw new ArgumentNullException(nameof(groupService));
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
            _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
        }

        public async Task<IViewComponentResult> InvokeAsync(int jobId, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            ViewLocationViewModel viewLocationViewModel;

            //Awaiting better implementation of this in the backend

            JobSummary jobDetails;
            var jobSet = (await _requestService.LocateJob(jobId, user.ID, cancellationToken)).JobSet;
            if (jobSet == Enums.Account.JobSet.UserOpenRequests_MatchingCriteria || jobSet == Enums.Account.JobSet.UserOpenRequests_NotMatchingCriteria)
            {
                jobDetails = (await _requestService.GetOpenJobsAsync(user, true, cancellationToken)).Where(x => x.JobID == jobId).First();
            }
            else
            {
                jobDetails = (await _requestService.GetJobsForUserAsync(user.ID, true, cancellationToken)).Where(x => x.JobID == jobId).First();
            }

            var canView = await _requestService.LogViewLocationEvent(user.ID, jobDetails.RequestID, jobId);
            if (canView)
            {
                var postCodeCordinateResponse = await _addressService.GetPostcodeCoordinate(jobDetails.PostCode);
                var locationCoordinates = postCodeCordinateResponse.IsSuccessful ? postCodeCordinateResponse.Content.PostcodeCoordinates.First() : null;

                viewLocationViewModel = new ViewLocationViewModel()
                {
                    IsAllowed = true,
                    Coordinates = locationCoordinates,
                    Distance = jobDetails.DistanceInMiles,
                    PostCode = jobDetails.PostCode,
                    encodedJobID = Base64Utils.Base64Encode(jobDetails.JobID)
                };
            } else
            {
                viewLocationViewModel = new ViewLocationViewModel()
                {
                    IsAllowed = false,
                    Coordinates = null,
                    Distance = null,
                    PostCode = "",
                    encodedJobID = ""
                };
            }

            return View("ViewLocationPopup", viewLocationViewModel);
        }

    }
}
