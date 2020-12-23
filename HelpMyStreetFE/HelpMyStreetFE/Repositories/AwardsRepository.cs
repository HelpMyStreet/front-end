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
using HelpMyStreetFE.Services.Groups;
using System.Threading;
using HelpMyStreet.Utils.Extensions;

namespace HelpMyStreetFE.Repositories
{
    public class AwardsRepository : IAwardsRepository
    {
        private readonly IRequestService _requestService;
        private readonly IUserService _userService;
        private readonly IGroupMemberService _groupMemberService;

        public AwardsRepository(IRequestService requestService, IUserService userService, IGroupMemberService groupMemberService)
        {
            _requestService = requestService;
            _userService = userService;
            _groupMemberService = groupMemberService;
        }

        public async Task<List<AwardsModel>> GetAwards()
        {
            var awardsList = new List<AwardsModel>()
            {
                new AwardsModel()
                {
                    AwardValue = 0,
                    AwardDescription = "Verify your ID in the My Profile tab so you can start accepting requests near you.",
                    ImageLocation = "/img/awards/round-placeholder.svg",
                    SpecificPredicate = u => {
                        foreach (Object o in u) {
                            if (o is bool){
                                bool isVerified = (bool)o;
                                return !isVerified;
                            }
                        }
                        return false; },
                },
                new AwardsModel()
                {
                    AwardValue = 0,
                    AwardDescription = "Find out what help is needed near you in the Open Requests tab.",
                    ImageLocation = "/img/awards/round-placeholder.svg",
                    SpecificPredicate = u => {
                        foreach (Object o in u) {
                            if (o is bool){
                                bool isVerified = (bool)o;
                                return isVerified;
                            }
                        }
                        return false; }
                },
                new AwardsModel()
                {
                    AwardValue = 1,
                    ImageLocation = "/img/awards/good-samaritan.png"
                },
                new AwardsModel()
                {
                    AwardValue = 5,
                    ImageLocation = "/img/awards/helping-hand.png"
                },
                new AwardsModel()
                {
                    AwardValue = 10,
                    ImageLocation = "/img/awards/top-neighbour.png"
                },
                new AwardsModel()
                {
                    AwardValue = 20,
                    ImageLocation = "/img/awards/budding-humanitarian.png"
                },
                new AwardsModel()
                {
                    AwardValue = 50,
                    ImageLocation = "/img/awards/helping-hero.png"
                },
                new AwardsModel()
                {
                    AwardValue = 100,
                    ImageLocation = "/img/awards/volunteer-superstar.png"
                }
            };

            return awardsList;
        }

        public async Task<CurrentAwardModel> GetAwardsByUserID(int userID, CancellationToken cancellationToken)
        {
            var jobs = await _requestService.GetJobsForUserAsync(userID, true, cancellationToken);
            var awards = await GetAwards();

            bool userIsVerified = await _groupMemberService.GetUserIsVerified(userID, cancellationToken);
            var predicates = new List<Object>() { userIsVerified };

            var completedJobs = jobs.Where(x => x.JobStatus == JobStatuses.Done);
            var relevantAward = awards.Where(x => completedJobs.Count() >= x.AwardValue && x.SpecificPredicate(predicates)).OrderBy(x => x.AwardValue).LastOrDefault();

            var completedJobDictionary = completedJobs.GroupBy(x => x.SupportActivity).ToDictionary(g => g.Key, g => g.Count());

            return new CurrentAwardModel
            {
                Award = relevantAward,
                CompletedJobCount = completedJobs.Count(),
                CompletedJobs = completedJobDictionary
            };
        }
    }
 }

