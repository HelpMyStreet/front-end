﻿using System;
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
            string address = personalDetails.Address.AddressLine1 + "," + personalDetails.Address.Postcode;
            string gender = "Unknown";
            string underlyingMedicalConditions = "No";

            if(personalDetails.UnderlyingMedicalCondition.HasValue)
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
                personalDetails.MobilePhone,
                personalDetails.OtherPhone,
                personalDetails.DateOfBirth.Value.ToString("dd/MM/yyyy"),
                gender,
                underlyingMedicalConditions
                );
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //TODO: Check that the user is authenticated
            var id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            User user = await _userService.GetUserAsync(id);

            AccountViewModel viewModel = new AccountViewModel();

            if (user!=null)
            {
                var userDetails = GetUserDetails(user);

                List<NotificationModel> notifications = new List<NotificationModel>() 
                { 
                    new NotificationModel 
                    {
                        Id = Guid.NewGuid(), 
                        Title = "Good news " + user.UserPersonalDetails.FirstName +"!", 
                        Message = "Your account is all set up. From your profile you can claim local streets, search for local volunteers and update your details. Keep checking back and keep an eye on your email inbox for the latest updates to our service. We hope to be distributing requests for help very soon.", 
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