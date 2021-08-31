using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Services;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Users;
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
        private readonly IAuthService _authService;
        private readonly IGroupMemberService _groupMemberService;

        public SiteHeaderViewComponent(IUserService userService, IAuthService authService, IGroupMemberService groupMemberService)
        {
            _userService = userService;
            _authService = authService;
            _groupMemberService = groupMemberService;
        }

        public async Task<IViewComponentResult> InvokeAsync(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(cancellationToken);
            SiteHeaderViewModel viewModel = new SiteHeaderViewModel
            {
                isLoggedIn = (user != null)
            };            
            if (viewModel.isLoggedIn)
            {
                viewModel.AccountVM = await GetAccountViewModel(user, cancellationToken);
            }
            viewModel.loginPage = HttpContext.Request.Path.Value.ToLower().Contains("login");
            return View(viewModel);
        }

        private async Task<AccountViewModel> GetAccountViewModel(User user, CancellationToken cancellationToken)
        {
            var viewModel = new AccountViewModel();

            if (user != null)
            {
                viewModel.Notifications = new List<NotificationModel>();
                viewModel.UserDetails = await _userService.GetUserDetails(user, cancellationToken);
                viewModel.UserGroups = await _groupMemberService.GetUserGroupRoles(user.ID, cancellationToken);
            }

            return viewModel;
        }

    }
}
