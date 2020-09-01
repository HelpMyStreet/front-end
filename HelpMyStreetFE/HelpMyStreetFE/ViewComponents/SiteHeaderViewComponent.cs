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
        private readonly IGroupService _groupService;
        public SiteHeaderViewComponent(IUserService userService, IGroupService groupService)
        {
            _userService = userService;
            _groupService = groupService;
        }

 

        public async Task<IViewComponentResult> InvokeAsync()
        {
            SiteHeaderViewModel viewModel = new SiteHeaderViewModel
            {
                isLoggedIn = ((HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated)
            };            
            if (viewModel.isLoggedIn)
            {
                var id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var user = HttpContext.Session.GetObjectFromJson<User>("User");
                if(user == null || user.ID != id)
                {               
                    user = await _userService.GetUserAsync(id);
                    HttpContext.Session.SetObjectAsJson("User", user);
                }
                viewModel.AccountVM = await GetAccountViewModel(user);
            }                                 
            return View(viewModel);
        }

        private async Task<AccountViewModel> GetAccountViewModel(User user)
        {
            var viewModel = new AccountViewModel();

            if (user != null)
            {
                viewModel.Notifications = new List<NotificationModel>();
                var userDetails = _userService.GetUserDetails(user);
                viewModel.UserDetails = userDetails;
                viewModel.UserGroups = await _groupService.GetUserGroupRoles(user.ID);
            }

            return viewModel;
        }

    }
}
