using HelpMyStreetFE.Models.Community;
using System.Collections.Generic;

namespace HelpMyStreetFE.Repositories
{
    public class CommunityRepository : ICommunityRepository
    {
        public CommunityViewModel GetCommunity(string communityName)
        {
            switch (communityName.Trim().ToLower())
            {
                case "tankersley":
                    return GetTankersley();     
                default:
                    return null;
            }
        }
        

        private CommunityViewModel GetTankersley()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();

            communityViewModel.Latitude = 53.498113;
            communityViewModel.Longitude = -1.488587;
            communityViewModel.ZoomLevel = 14;

            communityViewModel.CommunityName = "Tankersley & Pilley";

            communityViewModel.BannerImageLocation = "/img/community/tankersley/tankersley-st-peters-church.jpeg";

            communityViewModel.Header = "In Tankersley & Pilley, help is always available!";
            communityViewModel.HeaderSubtitle = "In our community there’s always somebody here to help, there’s no need for anyone to struggle alone. We’re the Tankersley & Pilley Community Helpers, here to help with:";
            communityViewModel.HeaderBullets = new List<string>
            {
                "Shopping for essentials",
                "A friendly chat",
                "Help at home",
                "Cooking a hot meal"
            };

            communityViewModel.CommunityVolunteersHeader = "Welcome from Tankersley & Pilley Community Helpers";
            


            communityViewModel.CommunityVolunteersTextHtml =
 @"<p>Pilley and Tankersley community helpers are here to help neighbours in need. We can help collecting shopping, running local errands or walking the dog.</p>
<p>To join us or to get in touch, email <a href = ""mailto: tankersley@helpmystreet.org"">tankersley@helpmystreet.org</a></p> 
";

            communityViewModel.RequestHelpHeading = @"How can we help?";

            communityViewModel.RequestHelpText = @"We've got shoppers, sewers and hot-meal makers; walkers, talkers and home-work helpers all ready and waiting to help you!";

            communityViewModel.ProvideHelpHeading = "Volunteer with us!";

            communityViewModel.ProvideHelpText = "Join us to help your neighbours. Just let us know when, where and how you can help. You can choose to help a little, or to help a lot! We're grateful for every contribution.";

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "David",
                    Role = "",
                    Location = "",
                    ImageLocation = "/img/community/tankersley/David.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Kate",
                    Role = "",
                    Location = "",
                    ImageLocation = "/img/community/tankersley/Kate.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Helen",
                    Role = "",
                    Location = "",
                    ImageLocation = "/img/community/tankersley/Helen.jpg"
                },
            };


            communityViewModel.UsefulLinksHtml = new List<string>()
            {
                @"<a href=""https://www.facebook.com/groups/958956387798343"">Piley & Tankersley Community Page (Facebook Group)</a>"
            };


            return communityViewModel;
        }
    }
}
