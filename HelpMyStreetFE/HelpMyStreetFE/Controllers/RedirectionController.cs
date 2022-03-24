using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Services;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Controllers
{
    public class RedirectionController : Controller
    {
        private readonly ICommunicationService _communicationService;
        private readonly IAuthService _authService;
        private readonly IRequestLocationService _requestLocationService;

        private readonly string LINK_EXPIRED_URL = "/link-expired";

        public RedirectionController(ICommunicationService communicationService, IAuthService authService, IRequestLocationService requestLocationService)
        {
            _communicationService = communicationService ?? throw new ArgumentNullException(nameof(communicationService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _requestLocationService = requestLocationService ?? throw new ArgumentNullException(nameof(requestLocationService));
        }

        [Route("link/{token}")]
        public async Task<IActionResult> Inbound(string token)
        {
            string destination = await _communicationService.GetLinkDestination(token);

            if (destination == null)
            {
                return Redirect(LINK_EXPIRED_URL);
            }

            _authService.PutSessionAuthorisedUrl(destination);

            return Redirect(destination);
        }

        [Route("link/j/{encodedJobId}")]
        [Authorize]
        public async Task<IActionResult> Job(string encodedJobId, CancellationToken cancellationToken)
        {

            int jobId = Base64Utils.Base64DecodeToInt(encodedJobId);
            User user = await _authService.GetCurrentUser(cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            var jobLocation = await _requestLocationService.LocateJob(jobId, user.ID, cancellationToken);
            encodedJobId = Base64Utils.Base64Encode(jobId);

            string destination = jobLocation?.JobSet switch
            {
                JobSet.UserOpenRequests => $"/account/open-requests/j/{encodedJobId}",
                JobSet.UserMyRequests => $"/account/my-requests/j/{encodedJobId}",
                JobSet.GroupRequests => $"/account/g/{jobLocation.GroupKey}/requests/j/{encodedJobId}",
                JobSet.UserOpenShifts => $"/account/open-shifts/j/{encodedJobId}",
                JobSet.UserMyShifts => $"/account/my-shifts/j/{encodedJobId}",
                JobSet.GroupShifts => $"/account/g/{jobLocation.GroupKey}/shifts/j/{encodedJobId}",
                _ => LINK_EXPIRED_URL,
            };

            return Redirect(destination);
        }

        [Route("link/r/{encodedRequestId}")]
        [Authorize]
        public async Task<IActionResult> RequestId(string encodedRequestId, CancellationToken cancellationToken)
        {

            int requestId = Base64Utils.Base64DecodeToInt(encodedRequestId);
            User user = await _authService.GetCurrentUser(cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            var jobLocation = await _requestLocationService.LocateRequest(requestId, user.ID, cancellationToken);
            encodedRequestId = Base64Utils.Base64Encode(requestId);

            string destination = jobLocation?.JobSet switch
            {
                JobSet.UserOpenRequests => $"/account/open-requests/r/{encodedRequestId}",
                JobSet.UserMyRequests => $"/account/my-requests/r/{encodedRequestId}",
                JobSet.GroupRequests => $"/account/g/{jobLocation.GroupKey}/requests/r/{encodedRequestId}",
                JobSet.UserOpenShifts => $"/account/open-shifts/r/{encodedRequestId}",
                JobSet.UserMyShifts => $"/account/my-shifts/r/{encodedRequestId}",
                JobSet.GroupShifts => $"/account/g/{jobLocation.GroupKey}/shifts/r/{encodedRequestId}",
                _ => LINK_EXPIRED_URL,
            };

            return Redirect(destination);
        }
    }
}
