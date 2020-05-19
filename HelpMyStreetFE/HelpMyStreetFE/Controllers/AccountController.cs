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
using Microsoft.Extensions.Options;
using HelpMyStreetFE.Models.Yoti;
using HelpMyStreetFE.Repositories;

namespace HelpMyStreetFE.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;
        private readonly IConfiguration _configuration;
        private readonly IOptions<YotiOptions> _yotiOptions;
        private readonly IRequestService _requestService;
        public AccountController(
            ILogger<AccountController> logger,
            IUserService userService,
            IAddressService addressService,
            IConfiguration configuration,
            IOptions<YotiOptions> yotiOptions,
            IRequestService requestService
            )
        {
            _logger = logger;
            _userService = userService;
            _addressService = addressService;
            _configuration = configuration;
            _yotiOptions = yotiOptions;
            _requestService = requestService;
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

            var userDetails = _userService.GetUserDetails(user);

            //Assume the registration page has been fully completed
            var viewModel = GetAccountViewModel<UserDetails>(user);

            viewModel.CurrentPage = MenuPage.UserDetails;
            viewModel.PageModel = userDetails;
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ComingSoon()
        {
            var viewModel = GetAccountViewModel<dynamic>(await GetCurrentUser());
            viewModel.Notifications = new List<NotificationModel>();
            viewModel.CurrentPage = MenuPage.ComingSoon;
            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Streets()
        {
            var currentUser = await GetCurrentUser();
            var viewModel = GetAccountViewModel<StreetsViewModel>(currentUser);
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

        [HttpGet]
        public async Task<IActionResult> OpenRequests()
        {
            var currentUser = await GetCurrentUser();
            var viewModel = GetAccountViewModel<List<Job>>(currentUser);
            viewModel.CurrentPage = MenuPage.OpenRequests;
            
            //viewModel.PageModel = await _requestService.GetOpenJobs("", 0.0);
            viewModel.PageModel = await _requestService.GetJobsByFilterAsync(currentUser.PostalCode, 20);

            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AcceptedRequests()
        {
            var currentUser = await GetCurrentUser();
            var viewModel = GetAccountViewModel<List<Job>>(currentUser);
            viewModel.CurrentPage = MenuPage.AcceptedRequests;
            viewModel.PageModel = await _requestService.GetJobsAllocatedToUserAsync(currentUser.ID);
            //viewModel.PageModel = new List<JobSummary>
            //{
            //    new JobSummary {
            //        UniqueIdentifier = Guid.NewGuid(),
            //        IsHealthCritical = true,
            //        DueDate = new DateTime(2020, 05, 22),
            //        SupportActivity = SupportActivities.Shopping,
            //        PostCode = "AB1 2CD",
            //        DistanceInMiles = 1.23
            //    },
            //    new JobSummary {
            //        UniqueIdentifier = Guid.NewGuid(),
            //        IsHealthCritical = false,
            //        DueDate = new DateTime(2020, 05, 22),
            //        SupportActivity = SupportActivities.CollectingPrescriptions,
            //        PostCode = "AB1 2CD",
            //        DistanceInMiles = 1.23
            //    },
            //    new JobSummary {
            //        UniqueIdentifier = Guid.NewGuid(),
            //        IsHealthCritical = true,
            //        DueDate = new DateTime(2020, 05, 22),
            //        SupportActivity = SupportActivities.Other,
            //        PostCode = "AB1 2CD",
            //        DistanceInMiles = 1.23
            //    },
            //};

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
             var id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
             var  user = await _userService.GetUserAsync(id);
             HttpContext.Session.SetObjectAsJson("User", user);   
             return user;
        }

        private AccountViewModel GetAccountViewModel<TPageModel>(User user)
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
                var userDetails = _userService.GetUserDetails(user);
                viewModel.Notifications = notifications;
                viewModel.VerificationViewModel = new Models.Yoti.VerificationViewModel
                {
                    YotiOptions = _yotiOptions.Value,
                    EncodedUserID = Base64Helpers.Base64Encode(user.ID.ToString()),
                    DisplayName = userDetails.DisplayName,
                    IsStreetChampion = userDetails.IsStreetChampion,
                    IsVerified = userDetails.IsVerified,
                    
                };
       
                viewModel.UserDetails = userDetails;                
                
            }

            return viewModel;
        }
    }
}
