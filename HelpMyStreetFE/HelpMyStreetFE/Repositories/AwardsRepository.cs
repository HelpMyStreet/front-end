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
                    AwardName = "Volunteer Newbie",
                    AwardValue = 1,
                    AwardDescription = "Great work on your first completed task!"
                },
                new AwardsModel()
                {
                    AwardName = "Volunteer Pro",
                    AwardValue = 5,
                    AwardDescription = "Nice one, thanks for all your help!"
                },
                new AwardsModel()
                {
                    AwardName = "Volunteer Champion",
                    AwardValue = 10,
                    AwardDescription = "You are a volunteer champion, my friend!"
                },
                new AwardsModel()
                {
                    AwardName = "Volunteer Superstar",
                    AwardValue = 15,
                    AwardDescription = "A helping superstar, that is what you are!"
                },
                new AwardsModel()
                {
                    AwardName = "VolunteerMeister 3000",
                    AwardValue = 25,
                    AwardDescription = "Wowsers, that's a lot of completed requests! Thanks!"
                },
                new AwardsModel()
                {
                    AwardName = "Volunterosaurus Rex",
                    AwardValue = 40,
                    AwardDescription = "Is that a glass of water I see shaking?"
                },
                new AwardsModel()
                {
                    AwardName = "Volunteer Hero",
                    AwardValue = 40,
                    AwardDescription = "You... can... take... my breath away..."
                }
            };

            return awardsList;
        }
    }
}
