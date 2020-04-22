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
using Microsoft.Extensions.Configuration;
using HelpMyStreetFE.Models;

namespace HelpMyStreetFE.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AccountController(
            ILogger<AccountController> logger,
            IUserService userService,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _userService = userService;
            _configuration = configuration;
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            BasePageViewModel model = new BasePageViewModel
            {
                FirebaseConfiguration = _configuration["Firebase:Configuration"]
            };
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //TODO: Check that the user is authenticated
            var id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            User user = await _userService.GetUserAsync(id);

            string correctPage = RegistrationController.GetCorrectPage(user);

            if (correctPage.Length > 0)
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
                        Message = "<p>Your account is all set up.</p>" +
                        " <div> Coming Soon: " +
                            "<ul style='margin-top:2px;'> " +
                                "<li>You will soon be able to update the personal and volunteering details on your profile page. </li>" +
                                "<li>Street Champions will be able to manage their streets, search for local volunteers, and handle requests for help.</li>" +
                            "</ul> " +
                        "</div>" +
                        "<p>Keep an eye on your email inbox for the latest updates. Thanks for joining HelpMyStreet!</p>",
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