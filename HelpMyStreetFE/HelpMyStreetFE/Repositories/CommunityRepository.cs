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
                case "ageuk":
                    return GetAgeUK();
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

            communityViewModel.RequestHelpText = @"We've got shoppers, showers and hot-meal makers; walkers, talkers and home-work helpers all ready and waiting to help you!";

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

        private CommunityViewModel GetAgeUK()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();

            communityViewModel.Latitude = 53.236571;
            communityViewModel.Longitude = -0.5398307;
            communityViewModel.ZoomLevel = 14;

            communityViewModel.CommunityName = "Age UK";

            communityViewModel.BannerImageLocation = "/img/community/ageUK/ageUKlogo.png";

            communityViewModel.Header = "Age UK, We're So Cool!";
            communityViewModel.HeaderSubtitle = "If you love Age, and the UK then this is totally the page for you! We got:";
            communityViewModel.HeaderBullets = new List<string>
            {
                "Dame Vera Lynn Recordings",
                "Beef Dripping Sandwiches",
                "A came first, then B, C would usually follow"
            };

            communityViewModel.CommunityVolunteersHeader = "Hi Guys - Welcome to our page";



            communityViewModel.CommunityVolunteersTextHtml =
 @"<p>Age UK - Vitals for Veterans is a new scheme from Age UK which provides PICU level Vital Sign monitoring for all veterans.</p>
<p>To join us or to get in touch, email <a href = ""mailto: v4v4u@helpmystreet.org"">tankersley@helpmystreet.org</a></p> 
";

            communityViewModel.RequestHelpHeading = @"How can we help?";

            communityViewModel.RequestHelpText = @"We've got shoppers, showers and hot-meal makers; walkers, talkers and home-work helpers all ready and waiting to help you!";

            communityViewModel.ProvideHelpHeading = "Volunteer with us!";

            communityViewModel.ProvideHelpText = "Join us to help your neighbours. Just let us know when, where and how you can help. You can choose to help a little, or to help a lot! We're grateful for every contribution.";

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Where",
                    Role = "",
                    Location = "",
                    ImageLocation = "/img/community/ageUK/ofrench.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Is",
                    Role = "",
                    Location = "",
                    ImageLocation = "/img/community/ageUK/ofrench.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Ollie",
                    Role = "",
                    Location = "",
                    ImageLocation = "/img/community/ageUK/ofrench.jpg"
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
