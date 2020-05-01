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
                var userDetails = _userService.GetUserDetails(user);
                viewModel.UserDetails = userDetails;
            }

            return viewModel;
        }

    }
}
