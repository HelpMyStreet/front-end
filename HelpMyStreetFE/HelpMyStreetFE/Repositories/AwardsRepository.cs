using System;
using HelpMyStreetFE.Models.Awards;
using HelpMyStreetFE.Helpers;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelpMyStreetFE.Services;
using System.Linq;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Repositories
{
    public class AwardsRepository : IAwardsRepository
    {
        private IRequestService _requestService;
        private IUserService _userService;

        public AwardsRepository(IRequestService requestService, IUserService userService)
        {
            _requestService = requestService;
            _userService = userService;
        }

        public async Task<List<AwardsModel>> GetAwards()
        {
            var awardsList = new List<AwardsModel>()
            {
                new AwardsModel()
                {
                    AwardName = "Ready to start helping?",
                    AwardValue = 0,
                    AwardDescription = "<a href='/account/verify'>Verify your ID</a> and see what help is needed near you in the Open Requests tab.",
                    ImageLocation = "/img/awards/question.svg",
                    SpecificPredicate = u => {
                        foreach (Object o in u) {
                            if (o is User){
                                User x = (User)o;
                                return !x.IsVerified.GetValueOrDefault(false);
                            }
                        }
                        return false; },
                },
                new AwardsModel()
                {
                    AwardName = "Ready to start helping?",
                    AwardValue = 0,
                    AwardDescription = "Find out what help is needed near you in the <a href='/account/open-requests'>open requests</a> tab!",
                    ImageLocation = "/img/awards/question.svg",
                    SpecificPredicate = u => {
                        foreach (Object o in u) {
                            if (o is User){
                                User x = (User)o;
                                return x.IsVerified.GetValueOrDefault(false);
                            }
                        }
                        return false; }
                },
                new AwardsModel()
                {
                    AwardName = "Good Samaritan",
                    AwardValue = 1,
                    AwardDescription = "{{count}} requests completed so far{{list}} - keep up the good work!",
                    DescriptionModifier = (count, list, description) => {return description.Replace("{{count}}",count.ToString()).Replace("{{list}}",list); },
                    ImageLocation = "/img/awards/good-samaritan.png"
                },
                new AwardsModel()
                {
                    AwardName = "Helping Hand",
                    AwardValue = 2,
                    AwardDescription = "{{count}} requests completed so far{{list}} - you're brill!",
                    DescriptionModifier = (count, list, description) => {return description.Replace("{{count}}",count.ToString()).Replace("{{list}}",list); },
                    ImageLocation = "/img/awards/helping-hand.png"
                },
                new AwardsModel()
                {
                    AwardName = "Top Neighbour",
                    AwardValue = 5,
                    AwardDescription = "{{count}} requests completed so far{{list}} - keep up the great work!",
                    DescriptionModifier = (count, list, description) => {return description.Replace("{{count}}",count.ToString()).Replace("{{list}}",list); },
                    ImageLocation = "/img/awards/top-neighbour.png"
                },
                new AwardsModel()
                {
                    AwardName = "Budding Humanitarian",
                    AwardValue = 10,
                    AwardDescription = "{{count}} requests completed so far{{list}} - you're awesome!",
                    DescriptionModifier = (count, list, description) => {return description.Replace("{{count}}",count.ToString()).Replace("{{list}}",list); },
                    ImageLocation = "/img/awards/budding-humanitarian.png"
                },
                new AwardsModel()
                {
                    AwardName = "Helping Hero",
                    AwardValue = 20,
                    AwardDescription = "{{count}} requests completed so far{{list}} - keep up the good work!",
                    DescriptionModifier = (count, list, description) => {return description.Replace("{{count}}",count.ToString()).Replace("{{list}}",list); },
                    ImageLocation = "/img/awards/helping-hero.png"
                },
                new AwardsModel()
                {
                    AwardName = "Volunteer Superstar",
                    AwardValue = 50,
                    AwardDescription = "{{count}} requests completed so far{{list}} - you're amazing!",
                    DescriptionModifier = (count, list, description) => {return description.Replace("{{count}}",count.ToString()).Replace("{{list}}",list); },
                    ImageLocation = "/img/awards/volunteer-superstar.png"
                }
            };

            return awardsList;
        }

        public async Task<CurrentAwardModel> GetAwardsByUserID(int userID, System.Threading.CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserAsync(userID, cancellationToken);
            var jobs = await _requestService.GetJobsForUserAsync(userID, true, cancellationToken);
            var viewModel = new AwardsViewModel();
            var awards = await GetAwards();

            var predicates = new List<Object>() { user };

            jobs = jobs.Where(x => x.JobStatus == JobStatuses.Done);
            var completedJobs = jobs.Count();

            awards = awards.OrderBy(x => x.AwardValue).ToList();
            var relevantAwards = awards.Where(x => completedJobs >= x.AwardValue && x.SpecificPredicate(predicates));

            var listOfJobs = jobs.GroupBy(x => x.SupportActivity, x => x.JobID, (activity, jobID) => new { Activity = activity, Count = jobID.Count() });
            listOfJobs = listOfJobs.OrderByDescending(x => x.Count);

            var listArray = new List<string>();
            foreach (var result in listOfJobs)
            {
                var friendlyName = SupportActivityHelpers.Sentences(result.Activity, (result.Count > 1));
                listArray.Add(result.Count + " " + friendlyName);
            }

            var listString = listArray.Count() > 0 ? ", including " + String.Join(", ", listArray) : " ";

            var returnAward = new CurrentAwardModel();

            if (relevantAwards.Count() >= 1)
            {

                var higherAwards = awards.Where(x => x.AwardValue > completedJobs);
                returnAward.Award = relevantAwards.LastOrDefault();
                returnAward.Award.JobCount = completedJobs;
                returnAward.Award.JobDetail = listString;
                if (higherAwards.Count() != 0)
                {
                    
                    returnAward.NextAwardLevel = awards.Where(x => x.AwardValue > completedJobs).FirstOrDefault().AwardValue;
                } else
                {
                    returnAward.NextAwardLevel = 0;
                }
            }
            else
            {
                returnAward.NextAwardLevel = awards.FirstOrDefault().AwardValue;
            }

            returnAward.CurrentAwardLevel = completedJobs;
            return returnAward;
        }
    }
 }

