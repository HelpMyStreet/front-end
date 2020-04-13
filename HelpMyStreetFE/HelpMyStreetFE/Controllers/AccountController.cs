using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Enums.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HelpMyStreetFE.Services;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IUserService _userService;
        public AccountController(
            ILogger<AccountController> logger,
            IUserService userService
            )
        {
            _logger = logger;
            _userService = userService;
        }

        private UserDetails GetUserDetails(HelpMyStreet.Utils.Models.User user)
        {
            var personalDetails = user.UserPersonalDetails;
            string initials = personalDetails.FirstName.Substring(0, 1) + personalDetails.LastName.Substring(0, 1);
            string address = personalDetails.Address.AddressLine1 + ", " + personalDetails.Address.Postcode;
            string streetChampion = string.Empty;
            string gender = "Unknown";
            string underlyingMedicalConditions = "No";

            if (user.ChampionPostcodes.Count > 0)
            {
                streetChampion = "Street Champion";
            }
            else if (user.IsVerified.HasValue && user.IsVerified.Value == true)
            {
                streetChampion = "Helper";
            }

            if (personalDetails.UnderlyingMedicalCondition.HasValue)
            {
                underlyingMedicalConditions = personalDetails.UnderlyingMedicalCondition.Value ? "Yes" : "No";
            }

            return new UserDetails(
                initials,
                personalDetails.DisplayName,
                personalDetails.FirstName,
                personalDetails.LastName,
                personalDetails.EmailAddress,
                address,
                streetChampion,
                personalDetails.MobilePhone,
                personalDetails.OtherPhone,
                personalDetails.DateOfBirth.Value.ToString("dd/MM/yyyy"),
                gender,
                underlyingMedicalConditions
                );
        }

        private string GetCorrectPage(int maxStep)
        {
            switch(maxStep)
            {
                case 1:
                    return "/registration/steptwo";
                case 2:
                    return "/registration/stepthree";
                case 3:
                    return "/registration/stepfour";
                case 4:
                    return "/registration/stepfive";
                default:
                    return string.Empty; //Registration journey is complete
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //TODO: Check that the user is authenticated
            var id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            User user = await _userService.GetUserAsync(id);

            //Assume the registration page has been fully completed
            string correctPage = string.Empty;

            if (user.RegistrationHistory.Count > 0)
            {
                int maxStep = user.RegistrationHistory.Max(a => a.Key); 
                correctPage = GetCorrectPage(maxStep);
            }
                

            if(correctPage.Length > 0)
            {
                //Registration journey is not complete
                return Redirect(correctPage);
            }
            
            //Assume the registration page has been fully completed
            AccountViewModel viewModel = new AccountViewModel();

            if (user != null)
            {
                var userDetails = GetUserDetails(user);

                List<NotificationModel> notifications = new List<NotificationModel>()
                {
                    new NotificationModel
                    {
                        Id = Guid.NewGuid(),
                        Title = "Good news " + user.UserPersonalDetails.FirstName +"!",
                        Message = "Your account is all set up. You will soon be able to update the personal and volunteering details on your profile page. Street Champions will be able to manage their streets, search for local volunteers, and handle requests for help. Keep an eye on your email inbox for the latest updates. Thanks for joining HelpMyStreet!",
                        Type = NotificationType.Success
                    }
                };
                viewModel.UserDetails = userDetails;
                viewModel.Notifications = notifications;
            }
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