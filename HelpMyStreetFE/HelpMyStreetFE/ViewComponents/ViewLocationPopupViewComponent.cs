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
        private readonly IRequestCachingService _requestCachingService;

        public ViewLocationPopupViewComponent(
            IAuthService authService,
            IRequestService requestService,
            IAddressService addressService,
            IRequestCachingService requestCachingService,
            IJobCachingService jobCachingService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
            _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
            _jobCachingService = jobCachingService ?? throw new ArgumentNullException(nameof(jobCachingService));
            _requestCachingService = requestCachingService ?? throw new ArgumentNullException(nameof(requestCachingService));
        }

        public async Task<IViewComponentResult> InvokeAsync(int? jobId, int? requestId, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            var jobSummary = jobId.HasValue ? (await _jobCachingService.GetJobSummaryAsync(jobId.Value, cancellationToken)) : (await _requestCachingService.GetRequestSummaryAsync(requestId.Value, cancellationToken)).JobSummaries.First();
            var postCode = jobSummary.PostCode;
            var canView = await _requestService.LogViewLocationEvent(user.ID, jobSummary.RequestID, jobSummary.JobID);

            ViewLocationViewModel viewLocationViewModel;

            if (canView)
            {

                var postCodeCoordinates = (await _addressService.GetPostcodeCoordinates(postCode)).First();

                var distanceInMiles = await _addressService.GetDistanceBetweenPostcodes(postCode, user.PostalCode, cancellationToken);

                viewLocationViewModel = new ViewLocationViewModel()
                {
                    IsAllowed = canView,
                    Coordinates = postCodeCoordinates,
                    Distance = distanceInMiles,
                    PostCode = postCode,
                    encodedJobID = Base64Utils.Base64Encode(jobId.HasValue ? jobId.Value : requestId.Value)
                };
            } else
            {
                viewLocationViewModel = new ViewLocationViewModel()
                {
                    IsAllowed = canView,
                    Coordinates = null,
                    Distance = null,
                    PostCode = null,
                    encodedJobID = Base64Utils.Base64Encode(jobId.HasValue ? jobId.Value : requestId.Value)
                };
            }

            return View("ViewLocationPopup", viewLocationViewModel);
        }

    }
}
