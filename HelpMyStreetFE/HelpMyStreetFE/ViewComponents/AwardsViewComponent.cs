using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Awards;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HelpMyStreetFE.Services.Users;

namespace HelpMyStreetFE.ViewComponents
{
    public class AwardsViewComponent : ViewComponent
    {
        private IAwardsRepository _awardsRepository;
        private IUserService _userService;

        public AwardsViewComponent(IAwardsRepository awardsRepository, IUserService userService)
        {
            _awardsRepository = awardsRepository;
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int userID, System.Threading.CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserAsync(userID, cancellationToken);
            var currentAward = await _awardsRepository.GetAwardsByUserID(userID, cancellationToken);
            var viewModel = new AwardsViewModel
            {
                CurrentAward = currentAward,
                User = user
            };
            return View(viewModel);
        }
    }
}
