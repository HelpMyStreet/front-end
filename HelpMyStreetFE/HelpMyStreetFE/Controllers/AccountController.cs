using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Enums.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace HelpMyStreetFE.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //TODO: Check that the user is authenticated

            //TODO: Actually get user details
            var mockDetails = new UserDetails("KG");
            var mockNotifications = new List<NotificationModel>() { new NotificationModel { Id = Guid.NewGuid(), Title = "Good new John!", Message = "Your account is all set up. From your profile you can claim local streets, search for local volunteers and update your details. Keep checking back and keep an eye on your email inbox for the latest updates to our service. We hope to be distributing requests for help very soon.", Type = NotificationType.Success } };
            var viewModel = new AccountViewModel() { UserDetails = mockDetails, Notifications = mockNotifications };
            return View(viewModel);
        }

        [HttpPut]
        public async Task<IActionResult> CloseNotification(Guid id)
        {
            //TODO: Pass this id to something that will stop that notification from being sent
            await Task.CompletedTask;
            return Ok();
        }
    }
}