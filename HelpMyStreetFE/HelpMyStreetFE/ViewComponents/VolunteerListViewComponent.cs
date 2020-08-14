﻿using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Models.Account.Volunteers;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.ViewComponents
{
    public class VolunteerListViewComponent: ViewComponent
    {
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;

        public VolunteerListViewComponent(IGroupService groupService, IUserService userService)
        {
            _groupService = groupService;
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int groupId)
        {
            User user = HttpContext.Session.GetObjectFromJson<User>("User");

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }


            var groupMembers = await _groupService.GetGroupMembers(groupId);


            var getUserTasks = groupMembers.Select(async (userGroup) =>
            {
                return new VolunteerViewModel()
                {
                    Roles = userGroup.UserRoles,
                    User = await _userService.GetUserAsync(userGroup.UserId)
                };
            });

            VolunteerListViewModel volunteerListViewModel = new VolunteerListViewModel
            {
                Volunteers = (await Task.WhenAll(getUserTasks)).Where(v => v.User != null)
            };


            return View("VolunteerList", volunteerListViewModel);
        }
    }
}
