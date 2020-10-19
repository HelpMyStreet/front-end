using System;
using HelpMyStreetFE.Models.Awards;
using HelpMyStreetFE.Helpers;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelpMyStreetFE.Services;
using System.Linq;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Services.Users;

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

        public Dictionary<string, Func<Dictionary<string, Object>, string, string>> DescriptionModifiers = new Dictionary<string, Func<Dictionary<string, Object>, string, string>>()
        {
            {"default",
                (attributes, description) => {
                    if (attributes.ContainsKey("countOfJobsCompleted") && attributes.ContainsKey("listOfJobsCompleted"))
                    {
                        var count = (int)attributes["countOfJobsCompleted"];
                        var list = attributes["listOfJobsCompleted"];
                        var desc = description;
                        desc = count == 1 ? desc.Replace("requests", "request") : desc;
                        return desc.Replace("{{count}}",count.ToString()).Replace("{{list}}",list.ToString());
                    }
                    else
                    {
                        return description;
                    }
                }

            }
        };

        public async Task<List<AwardsModel>> GetAwards()
        {
            var awardsList = new List<AwardsModel>()
            {
                new AwardsModel()
                {
                    AwardName = "Ready to start helping?",
                    AwardValue = 0,
                    AwardDescription = "Verify your ID in the My Profile tab so you can start accepting requests near you.",
                    ImageLocation = "/img/awards/round-placeholder.svg",
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
                    AwardDescription = "Find out what help is needed near you in the Open Requests tab.",
                    ImageLocation = "/img/awards/round-placeholder.svg",
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
                    DescriptionModifier = DescriptionModifiers["default"],
                    ImageLocation = "/img/awards/good-samaritan.png"
                },
                new AwardsModel()
                {
                    AwardName = "Helping Hand",
                    AwardValue = 2,
                    AwardDescription = "{{count}} requests completed so far{{list}} - you're brill!",
                    DescriptionModifier = DescriptionModifiers["default"],
                    ImageLocation = "/img/awards/helping-hand.png"
                },
                new AwardsModel()
                {
                    AwardName = "Top Neighbour",
                    AwardValue = 5,
                    AwardDescription = "{{count}} requests completed so far{{list}} - keep up the great work!",
                    DescriptionModifier = DescriptionModifiers["default"],
                    ImageLocation = "/img/awards/top-neighbour.png"
                },
                new AwardsModel()
                {
                    AwardName = "Budding Humanitarian",
                    AwardValue = 10,
                    AwardDescription = "{{count}} requests completed so far{{list}} - you're awesome!",
                    DescriptionModifier = DescriptionModifiers["default"],
                    ImageLocation = "/img/awards/budding-humanitarian.png"
                },
                new AwardsModel()
                {
                    AwardName = "Helping Hero",
                    AwardValue = 20,
                    AwardDescription = "{{count}} requests completed so far{{list}} - keep up the good work!",
                    DescriptionModifier = DescriptionModifiers["default"],
                    ImageLocation = "/img/awards/helping-hero.png"
                },
                new AwardsModel()
                {
                    AwardName = "Volunteer Superstar",
                    AwardValue = 50,
                    AwardDescription = "{{count}} requests completed so far{{list}} - you're amazing!",
                    DescriptionModifier = DescriptionModifiers["default"],
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
                var attributes = new Dictionary<string, Object>()
                {
                    {"countOfJobsCompleted",  completedJobs},
                    {"listOfJobsCompleted", listString }
                };
                returnAward.Award.AwardAttributes = attributes;
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

