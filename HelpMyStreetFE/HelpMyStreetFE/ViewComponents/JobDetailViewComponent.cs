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
            JobDetailViewModel jobDetailViewModel = new JobDetailViewModel()
            {
                JobDetail = await _requestService.GetJobDetailsAsync(JobID, UserId),
                UserIsVerified = (await _userService.GetUserAsync(UserId)).IsVerified ?? false,
            };

            return View("JobDetail", jobDetailViewModel);
        }
    }
}
