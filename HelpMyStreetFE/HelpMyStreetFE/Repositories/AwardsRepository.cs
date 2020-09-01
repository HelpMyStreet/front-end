using System;
using HelpMyStreetFE.Models.Awards;
using HelpMyStreet.Utils.Models;
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
                    AwardValue = 11,
                    AwardDescription = "{{count}} requests completed so far{{list}} - keep up the great work!",
                    DescriptionModifier = (count, list, description) => {return description.Replace("{{count}}",count.ToString()).Replace("{{list}}",list); },
                    ImageLocation = "/img/awards/top-neighbour.png"
                },
                new AwardsModel()
                {
                    AwardName = "Budding Humanitarian",
                    AwardValue = 21,
                    AwardDescription = "{{count}} requests completed so far{{list}} - you're awesome!",
                    DescriptionModifier = (count, list, description) => {return description.Replace("{{count}}",count.ToString()).Replace("{{list}}",list); },
                    ImageLocation = "/img/awards/budding-humanitarian.png"
                },
                new AwardsModel()
                {
                    AwardName = "Helping Hero",
                    AwardValue = 51,
                    AwardDescription = "{{count}} requests completed so far{{list}} - keep up the good work!",
                    DescriptionModifier = (count, list, description) => {return description.Replace("{{count}}",count.ToString()).Replace("{{list}}",list); },
                    ImageLocation = "/img/awards/helping-hero.png"
                },
                new AwardsModel()
                {
                    AwardName = "Volunteer Superstar",
                    AwardValue = 101,
                    AwardDescription = "{{count}} requests completed so far{{list}} - you're amazing!",
                    DescriptionModifier = (count, list, description) => {return description.Replace("{{count}}",count.ToString()).Replace("{{list}}",list); },
                    ImageLocation = "/img/awards/volunteer-superstar.png"
                }
            };

            return awardsList;
        }
    }
}
