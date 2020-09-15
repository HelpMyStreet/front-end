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
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.ViewComponents
{
    public class SiteHeaderViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        private readonly IAuthService _authService;
        public SiteHeaderViewComponent(IUserService userService, IGroupService groupService, IAuthService authService)
        {
            _userService = userService;
            _groupService = groupService;
            _authService = authService;
        }



        public async Task<IViewComponentResult> InvokeAsync(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            SiteHeaderViewModel viewModel = new SiteHeaderViewModel
            {
                isLoggedIn = (user != null)
            };            
            if (viewModel.isLoggedIn)
            {
                viewModel.AccountVM = await GetAccountViewModel(user, cancellationToken);
            }                                 
            return View(viewModel);
        }

        private async Task<AccountViewModel> GetAccountViewModel(User user, CancellationToken cancellationToken)
        {
            var viewModel = new AccountViewModel();

            if (user != null)
            {
                viewModel.Notifications = new List<NotificationModel>();
                var userDetails = _userService.GetUserDetails(user);
                viewModel.UserDetails = userDetails;
                viewModel.UserGroups = await _groupService.GetUserGroupRoles(user.ID, cancellationToken);
            }

            return viewModel;
        }

    }
}
