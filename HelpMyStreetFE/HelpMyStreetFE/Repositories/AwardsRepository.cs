using System;
using HelpMyStreetFE.Models.Awards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public class AwardsRepository : IAwardsRepository
    {

        public async Task<List<AwardsModel>> GetAwards()
        {
            var awardsList = new List<AwardsModel>()
            {
                new AwardsModel()
                {
                    AwardName = "Good Samaritan",
                    AwardValue = 1,
                    AwardDescription = "{{count}} requests completed so far{{list}} - keep up the good work!",
                    ImageLocation = "/img/awards/good-samaritan.png"
                },
                new AwardsModel()
                {
                    AwardName = "Helping Hand",
                    AwardValue = 2,
                    AwardDescription = "{{count}} requests completed so far{{lost}} - you're brill!",
                    ImageLocation = "/img/awards/helping-hand.png"
                },
                new AwardsModel()
                {
                    AwardName = "Top Neighbour",
                    AwardValue = 11,
                    AwardDescription = "{{count}} requests completed so far{{list}} - keep up the great work!",
                    ImageLocation = "/img/awards/top-neighbour.png"
                },
                new AwardsModel()
                {
                    AwardName = "Budding Humanitarian",
                    AwardValue = 21,
                    AwardDescription = "{{count}} requests completed so far{{list}} - you're awesome!",
                    ImageLocation = "/img/awards/budding-humanitarian.png"
                },
                new AwardsModel()
                {
                    AwardName = "Helping Hero",
                    AwardValue = 51,
                    AwardDescription = "{{count}} requests completed so far{{list}} - keep up the good work!",
                    ImageLocation = "/img/awards/helping-hero.png"
                },
                new AwardsModel()
                {
                    AwardName = "Volunteer Superstar",
                    AwardValue = 101,
                    AwardDescription = "{{count}} requests completed so far{{list}} - you're amazing!",
                    ImageLocation = "/img/awards/volunteer-superstar.png"
                }
            };

            return awardsList;
        }
    }
}
