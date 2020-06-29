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

        private CommunityViewModel GetAgeUK()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();

            communityViewModel.Latitude = 53.236571;
            communityViewModel.Longitude = -0.5398307;
            communityViewModel.ZoomLevel = 9;

            communityViewModel.CommunityName = "Age UK";

            communityViewModel.BannerImageLocation = "/img/community/ageUK/ageUKlogo.png";

            communityViewModel.Header = "Veterans need your help!";
            communityViewModel.HeaderSubtitle = "We are pleased to announce the start of our new 'Vitals For Veterans' project.<br><br>We are aiming to deliver wellbeing packs to veterans across Lincoln & South Lincolnshire over the next 6 months because <i>veterans should not be forgotten.</i><br>";
            communityViewModel.HeaderBullets = null;
            communityViewModel.HideHeaderButtons = false;
            communityViewModel.HideHeaderHelpButton = true;
            communityViewModel.HeaderVolunteerButtonText = "If you can help us get wellbeing packs to veterans in your area please sign up to volunteer below:";
            communityViewModel.CommunityVolunteersHeader = "Welcome from Age UK Lincoln and South Lincolnshire";



            communityViewModel.CommunityVolunteersTextHtml =
 @"<p>Age UK Lincoln & South Lincolnshire is an independent, local charity and we have been working in the local community to help older people, their families and carers for over 61 years. We have over 200 dedicated staff and volunteers helping us to deliver services and activities for older people in Lincoln and across the county.</p>
<p>Supporting over 4,000 people aged 50 and over every week, our dedicated staff and volunteers deliver support services and activities across 17 departments for people countywide.</p>
<p>Our support is varied and extensive, 11,845 people attended activities with us in 2018/19, helping to eliminate social isolation and generating opportunities for engagement through clubs and groups. Our day centre had 4700 attendances offering support to vulnerable older people who are otherwise alone during the day and allows us to provide respite for carers.</p>
<p>To join us or to get in touch, email <a href = ""mailto: info@ageuklsl.org.uk"">info@ageuklsl.org.uk</a></p> 
";
            communityViewModel.HideHelpPanel = true;

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Nicki Lee",
                    Role = "Senior Volunteer Coordinator",
                    Location = "Lincoln & South Lincolnshire",
                    ImageLocation = "/img/icons/anonymous-user.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Susan Kellit",
                    Role = "Head Of Charitable Services",
                    Location = "Lincoln & South Lincolnshire",
                    ImageLocation = "/img/icons/anonymous-user.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Melanie Meik",
                    Role = "Fundraising & Marketing Manager",
                    Location = "Lincoln & South Lincolnshire",
                    ImageLocation = "/img/icons/anonymous-user.png"
                },
            };


            communityViewModel.UsefulLinksHtml = new List<string>()
            {
                @"<a href=""https://www.ageuk.org.uk/lincolnsouthlincolnshire/"">Age UK Lincoln and South Lincolnshire main site</a>",
                @"<a href=""https://www.ageuk.org.uk/lincolnsouthlincolnshire/activities-and-events/vitals-for-veterans/"">Vitals for Veterans page</a>"
            };


            return communityViewModel;
        }
    }
}
