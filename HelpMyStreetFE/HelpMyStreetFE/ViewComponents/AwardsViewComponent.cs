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
namespace HelpMyStreetFE.ViewComponents
{
    public class AwardsViewComponent : ViewComponent
    {
        private IRequestService _requestService;
        private IAwardsRepository _awardsRepository;
        private IUserService _userService;

        private Dictionary<SupportActivities, string> friendlySupport = new Dictionary<SupportActivities, string>()
        {
        { SupportActivities.Shopping, "shopping trip" },
        { SupportActivities.CollectingPrescriptions, "prescription collected" },
        { SupportActivities.Errands, "errand run" },
        { SupportActivities.DogWalking, "dog walked" },
        { SupportActivities.MealPreparation, "meal prepared" },
        { SupportActivities.PhoneCalls_Friendly, "friendly chat" },
        { SupportActivities.PhoneCalls_Anxious, "supportive chat" },
        { SupportActivities.HomeworkSupport, "homework assignment" },
        { SupportActivities.CheckingIn, "check in" },
        { SupportActivities.Other, "other task" },
        { SupportActivities.FaceMask, "face covering sent" },
        { SupportActivities.WellbeingPackage, "wellbeing package" },
        { SupportActivities.CommunityConnector, "Community Connectors" },
        };
        private Dictionary<SupportActivities, string> friendlySupports = new Dictionary<SupportActivities, string>()
        {
        { SupportActivities.Shopping, "shopping trips" },
        { SupportActivities.CollectingPrescriptions, "prescriptions collected" },
        { SupportActivities.Errands, "errands run" },
        { SupportActivities.DogWalking, "dogs walked" },
        { SupportActivities.MealPreparation, "meals prepared" },
        { SupportActivities.PhoneCalls_Friendly, "friendly chats" },
        { SupportActivities.PhoneCalls_Anxious, "supportive chats" },
        { SupportActivities.HomeworkSupport, "homework assignments" },
        { SupportActivities.CheckingIn, "check ins" },
        { SupportActivities.Other, "other tasks" },
        { SupportActivities.FaceMask, "face coverings sent" },
        { SupportActivities.WellbeingPackage, "wellbeing packages" },
        { SupportActivities.CommunityConnector, "Community Connectors" },
        };


        public AwardsViewComponent(IAwardsRepository awardsRepository, IRequestService requestService, IUserService userService)
        {
            _requestService = requestService;
            _awardsRepository = awardsRepository;
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int userID, System.Threading.CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserAsync(userID, cancellationToken);
            var currentAward = await _awardsRepository.GetAwardsByUserID(userID, cancellationToken);
            var viewModel = new AwardsViewModel();

            viewModel.CurrentAward = currentAward;
            viewModel.User = user;
            return View(viewModel);
        }
    }
}
