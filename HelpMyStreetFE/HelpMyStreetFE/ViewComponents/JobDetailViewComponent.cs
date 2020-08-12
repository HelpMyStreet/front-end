using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.ViewComponents
{
    public class JobDetailViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;
        private readonly IUserService _userService;
        public JobDetailViewComponent(IRequestService requestService, IUserService userService)
        {
            _requestService = requestService;
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int JobID, int UserId)
        {
            var jobDetails = await _requestService.GetJobDetailsAsync(JobID);
            var userDetails = await _userService.GetUserAsync(UserId);

            JobDetailViewModel jobDetailViewModel = new JobDetailViewModel()
            {
                UserIsVerified = userDetails.IsVerified ?? false,
                Recipient = jobDetails.Recipient,
                Requestor = jobDetails.Requestor,
                RequestorType = jobDetails.RequestorType,
                JobSummary = new HelpMyStreet.Utils.Models.JobSummary()
                {
                    JobID = jobDetails.JobID,
                    SupportActivity = jobDetails.SupportActivity,
                    SpecialCommunicationNeeds = jobDetails.SpecialCommunicationNeeds,
                    DateStatusLastChanged = jobDetails.DateStatusLastChanged,
                    Details = jobDetails.Details,
                    DueDate = jobDetails.DueDate,
                    DueDays = jobDetails.DueDays,
                    JobStatus = jobDetails.JobStatus,
                    OtherDetails = jobDetails.OtherDetails,
                    PostCode = jobDetails.PostCode,
                    RecipientOrganisation = jobDetails.OrganisationName,

                    //TODO: Need actual questions here 
                    Questions = new List<HelpMyStreet.Utils.Models.Question>(),
                }
            };

            return View("JobDetail", jobDetailViewModel);
        }
    }
}
