using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreetFE.Services.Users;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreet.Utils.Utils;

namespace HelpMyStreetFE.ViewComponents
{
    public class ViewLocationPopupViewComponent : ViewComponent
    {
        private readonly IAuthService _authService;
        private readonly IRequestService _requestService;
        private readonly IAddressService _addressService;
        private readonly IJobCachingService _jobCachingService;

        public ViewLocationPopupViewComponent(
            IAuthService authService,
            IRequestService requestService,
            IAddressService addressService,
            IJobCachingService jobCachingService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
            _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
            _jobCachingService = jobCachingService ?? throw new ArgumentNullException(nameof(jobCachingService));
        }

        public async Task<IViewComponentResult> InvokeAsync(int jobId, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            var jobSummary = await _jobCachingService.GetJobSummaryAsync(jobId, cancellationToken);
            var canView = await _requestService.LogViewLocationEvent(user.ID, jobSummary.RequestID, jobId);

            ViewLocationViewModel viewLocationViewModel;

            if (canView)
            {

                var postCodeCoordinates = (await _addressService.GetPostcodeCoordinates(jobSummary.PostCode)).First();

                var distanceInMiles = await _addressService.GetDistanceBetweenPostcodes(jobSummary.PostCode, user.PostalCode, cancellationToken);

                viewLocationViewModel = new ViewLocationViewModel()
                {
                    IsAllowed = canView,
                    Coordinates = postCodeCoordinates,
                    Distance = distanceInMiles,
                    PostCode = jobSummary.PostCode,
                    encodedJobID = Base64Utils.Base64Encode(jobSummary.JobID)
                };
            } else
            {
                viewLocationViewModel = new ViewLocationViewModel()
                {
                    IsAllowed = canView,
                    Coordinates = null,
                    Distance = null,
                    PostCode = null,
                    encodedJobID = Base64Utils.Base64Encode(jobSummary.JobID)
                };
            }

            return View("ViewLocationPopup", viewLocationViewModel);
        }

    }
}
