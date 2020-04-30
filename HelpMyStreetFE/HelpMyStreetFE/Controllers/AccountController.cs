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
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models;
using Microsoft.Extensions.Configuration;
using HelpMyStreetFE.Helpers;

namespace HelpMyStreetFE.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;
        private readonly IConfiguration _configuration;
        public AccountController(
            ILogger<AccountController> logger,
            IUserService userService,
            IAddressService addressService,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _userService = userService;
            _addressService = addressService;
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
            bool isStreetChampion = (user.StreetChampionRoleUnderstood.HasValue && user.StreetChampionRoleUnderstood.Value == true);
            bool isVerified = (user.IsVerified.HasValue && user.IsVerified.Value == true);
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
                underlyingMedicalConditions,
                user.ChampionPostcodes,
                isStreetChampion,
                isVerified
                ) ;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            LoginViewModel model = new LoginViewModel
            {
                FirebaseConfiguration = _configuration["Firebase:Configuration"]
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUser();

            string correctPage = RegistrationController.GetCorrectPage(user);

            if (correctPage.Length > 0)
            {
                //Registration journey is not complete
                return Redirect(correctPage);
            }

            var userDetails = GetUserDetails(user);

            //Assume the registration page has been fully completed
            var viewModel = GetAccountViewModel(user);

            viewModel.CurrentPage = MenuPage.UserDetails;
            viewModel.PageModel = userDetails;
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ComingSoon()
        {
            AccountViewModel viewModel = GetAccountViewModel(await GetCurrentUser());
            viewModel.Notifications = new List<NotificationModel>();
            viewModel.CurrentPage = MenuPage.ComingSoon;
            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Streets()
        {
            var currentUser = await GetCurrentUser();
            var viewModel = GetAccountViewModel(currentUser);
            viewModel.Notifications.Clear();
            viewModel.CurrentPage = MenuPage.MyStreets;
            var streetsViewModel = new StreetsViewModel();

            var friendlyPostcodes = await _addressService.GetFriendlyNames(viewModel.UserDetails.ChampionPostcodes);
            
            foreach (var postcode in viewModel.UserDetails.ChampionPostcodes)
            {
                Street street = new Street();                
                street.Name = postcode;
                if (friendlyPostcodes.Content != null)
                {
                    street.FriendlyName = friendlyPostcodes.Content.PostcodesResponse[HelpMyStreet.Utils.Utils.PostcodeFormatter.FormatPostcode(postcode)].FriendlyName;
                }
                var helpers = await _userService.GetHelpersByPostcode(postcode);            
                var champs = await _userService.GetChampionsByPostcode(postcode);                
                helpers.Users.AddRange(champs.Users);
                if (helpers.Users != null)
                {            
                    foreach (var helper in helpers.Users.GroupBy(x => x.ID).Select(g => g.First()).ToList())// de duping
                    {
                        if (helper.ID == currentUser.ID) continue;
                        if (!helper.IsVerified.HasValue || !helper.IsVerified.Value) continue;
                        bool isStreetChampion = (helper.StreetChampionRoleUnderstood.Value && helper.ChampionPostcodes.Contains(postcode));
                        street.Helpers.Add(new Helper
                        {
                            Name = helper.UserPersonalDetails.DisplayName,
                            PhoneNumber = helper.UserPersonalDetails.MobilePhone,
                            AlternatePhoneNumber = helper.UserPersonalDetails.OtherPhone,
                            Email = helper.UserPersonalDetails.EmailAddress,
                            SupportedActivites = helper.SupportActivities,
                            IsStreetChampion = isStreetChampion
                        }); 
                    }
                    streetsViewModel.Streets.Add(street);
                }
            }
            viewModel.PageModel = streetsViewModel;
            return View("Index", viewModel);
        }

        [HttpPut]
        public async Task<IActionResult> CloseNotification(Guid id)
        {
            //TODO: Pass this id to something that will stop that notification from being sent
            await Task.CompletedTask;
            return Ok();
        }

        private async Task<User> GetCurrentUser()
        {         
            var user = HttpContext.Session.GetObjectFromJson<User>("User");
            if (user == null)
            {
                var id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                user = await _userService.GetUserAsync(id);
                HttpContext.Session.SetObjectAsJson("User", user);
            }

            return user;
        }

        private AccountViewModel GetAccountViewModel(User user)
        {
            var viewModel = new AccountViewModel();

            if (user != null)
            {
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

                viewModel.Notifications = notifications;
                var userDetails = GetUserDetails(user);
                viewModel.UserDetails = userDetails;                
                
            }

            return viewModel;
        }
    }
}