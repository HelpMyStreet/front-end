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
        private Dictionary<SupportActivities, string> friendlySupports = new Dictionary<SupportActivities, string>()
        {
        { SupportActivities.Shopping, "Shopping trips" },
        { SupportActivities.CollectingPrescriptions, "Prescriptions" },
        { SupportActivities.Errands, "Errands" },
        { SupportActivities.DogWalking, "Dogs Walked" },
        { SupportActivities.MealPreparation, "Prepared Meals" },
        { SupportActivities.PhoneCalls_Friendly, "Friendly Chats" },
        { SupportActivities.PhoneCalls_Anxious, "Supportive Chats" },
        { SupportActivities.HomeworkSupport, "Homework Assignments" },
        { SupportActivities.CheckingIn, "Check ins" },
        { SupportActivities.Other, "Other Tasks" },
        { SupportActivities.FaceMask, "Face Coverings" },
        { SupportActivities.WellbeingPackage, "Wellbeing Packages" },
        { SupportActivities.CommunityConnector, "Community Connectors" },
        };
    

    public AwardsViewComponent(IAwardsRepository awardsRepository)
        {
            _awardsRepository = awardsRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(List<JobSummary> jobs)
        {
            var viewModel = new AwardsViewModel();
            var awards = await _awardsRepository.GetAwards();
            var completedJobs = jobs.Where(x => x.JobStatus == JobStatuses.Done).Count();
            awards = awards.OrderBy(x => x.AwardValue).ToList();
            var relevantAwards = awards.Where(x => completedJobs > x.AwardValue);

            var listOfJobs = jobs.GroupBy(x => x.SupportActivity, x => x.JobID, (activity, jobID) => new { Activity = activity, Count = jobID.Count() });

            var listArray = new List<string>();
            foreach (var result in listOfJobs)
            {
                if (result.Activity != SupportActivities.CommunityConnector)
                {
                    listArray.Add(result.Count + " " + friendlySupports[result.Activity]);
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
