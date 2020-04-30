using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HelpMyStreetFE.ViewComponents
{
    public class SiteHeaderViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        public SiteHeaderViewComponent(IUserService userService)
        {
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
                );
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            SiteHeaderViewModel viewModel = new SiteHeaderViewModel
            {
                isLoggedIn = ((HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated)
            };            
            if (viewModel.isLoggedIn)
            {
               
                var user = HttpContext.Session.GetObjectFromJson<User>("User");
                if(user == null)
                {
                    var id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    user = await _userService.GetUserAsync(id);
                    HttpContext.Session.SetObjectAsJson("User", user);
                }
                viewModel.AccountVM = GetAccountViewModel(user);
            }                                 
            return View(viewModel);
        }

        private AccountViewModel GetAccountViewModel(User user)
        {
            var viewModel = new AccountViewModel();

            if (user != null)
            {
                viewModel.Notifications = new List<NotificationModel>();
                var userDetails = GetUserDetails(user);
                viewModel.UserDetails = userDetails;
            }

            return viewModel;
        }

    }
}
