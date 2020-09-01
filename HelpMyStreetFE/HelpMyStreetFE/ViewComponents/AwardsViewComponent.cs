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
        private IAwardsRepository _awardsRepository;
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


        public AwardsViewComponent(IAwardsRepository awardsRepository)
        {
            _awardsRepository = awardsRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(List<JobSummary> jobs, User user)
        {
            var viewModel = new AwardsViewModel();
            var awards = await _awardsRepository.GetAwards();
            var predicates = new List<Object>() { user };
            var completedJobs = jobs.Where(x => x.JobStatus == JobStatuses.Done).Count();

            awards = awards.OrderBy(x => x.AwardValue).ToList();
            var relevantAwards = awards.Where(x => completedJobs >= x.AwardValue && x.SpecificPredicate(predicates));

            var listOfJobs = jobs.GroupBy(x => x.SupportActivity, x => x.JobID, (activity, jobID) => new { Activity = activity, Count = jobID.Count() });
            listOfJobs = listOfJobs.OrderByDescending(x => x.Count);

            var listArray = new List<string>();
            foreach (var result in listOfJobs)
            {
                if (result.Activity != SupportActivities.CommunityConnector)
                {
                    if (result.Count == 1) {
                        listArray.Add(result.Count + " " + friendlySupport[result.Activity]);
                    }
                    else {
                        listArray.Add(result.Count + " " + friendlySupports[result.Activity]);
                    }
                }
            }

            var listString = listArray.Count() > 0 ? ", including " + String.Join(", ", listArray) : " ";

            if (relevantAwards.Count() >= 1)
            {
                viewModel.Award = relevantAwards.LastOrDefault();
                viewModel.Award.AwardDescription = viewModel.Award.AwardDescription.Replace("{{count}}", completedJobs.ToString());
                viewModel.Award.AwardDescription = viewModel.Award.AwardDescription.Replace("{{list}}", listString);
                viewModel.NextAwardLevel = awards.Where(x => x.AwardValue > completedJobs).FirstOrDefault().AwardValue;
            }
            else
            {
                viewModel.NextAwardLevel = awards.FirstOrDefault().AwardValue;
            }

            viewModel.CurrentAwardLevel = completedJobs;
            return View(viewModel);
        }
    }
}
