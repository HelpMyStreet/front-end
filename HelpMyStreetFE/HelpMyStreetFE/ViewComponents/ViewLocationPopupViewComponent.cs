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

            var jobDetails = (await _requestService.GetJobSummaryAsync(jobId, cancellationToken));
            var canView = await _requestService.LogViewLocationEvent(user.ID, jobDetails.RequestID, jobId);

            ViewLocationViewModel viewLocationViewModel;

            if (canView)
            {

                var postCodeCoordinates = (await _addressService.GetPostcodeCoordinates(jobDetails.PostCode)).First();

                var distanceInMiles = await _addressService.GetDistanceBetweenPostcodes(jobDetails.PostCode, user.PostalCode, cancellationToken);

                viewLocationViewModel = new ViewLocationViewModel()
                {
                    IsAllowed = canView,
                    Coordinates = postCodeCoordinates,
                    Distance = distanceInMiles,
                    PostCode = jobDetails.PostCode,
                    encodedJobID = Base64Utils.Base64Encode(jobDetails.JobID)
                };
            } else
            {
                viewLocationViewModel = new ViewLocationViewModel()
                {
                    IsAllowed = canView,
                    Coordinates = null,
                    Distance = null,
                    PostCode = null,
                    encodedJobID = Base64Utils.Base64Encode(jobDetails.JobID)
                };
            }

            return View("ViewLocationPopup", viewLocationViewModel);
        }

    }
}
