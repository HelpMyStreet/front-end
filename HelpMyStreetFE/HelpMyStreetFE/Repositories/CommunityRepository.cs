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
                case "hlp":
                    return GetHLP();
                default:
                    return null;
            }
        }
        
        private CommunityViewModel GetHLP()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();

            communityViewModel.Latitude = 51.507602;
            communityViewModel.Longitude = -0.127816;
            communityViewModel.ZoomLevel = 10;

            communityViewModel.CommunityName = "Healthy London Partnership";
            communityViewModel.CommunityShortName = "Healthy London";

            communityViewModel.BannerImageLocation = "/img/community/hlp/hlp-banner.png";

            communityViewModel.Header = "What are Community Connectors?";
            communityViewModel.DisableButtons = true;
            communityViewModel.SignUpLink = communityViewModel.SignUpLink + "/hlp";
            communityViewModel.HeaderHTML = @"
                    <p class='row sm12 text-left mt-sm mb-xs'>
                        Mental Health First Aid England is working with the new NHS Connect service to recruit volunteer 
                        Community Connectors as part of the nation-wide response to Covid-19. NHS Connect is a new digital
                        service that helps connect vulnerable people with the support they need.<br>
                    </p>
                    <p class='row sm12 text-left mt-sm mb-xs'>
                      We are looking for volunteers who combine an understanding of mental health problems with previous training 
                        and experience in one or more of these practical and ethical frameworks: coaching, motivational interviewing,
                        counselling or an accredited form of therapy.
                    </p>
                    <p class='row sm12 text-left mt-sm mb-xs'>This is your opportunity to sign up as a pioneer volunteer.</p>
                   <div class='row input sm12'>
                        <p class='mb-xs mt-sm'>Please confirm that:</p>
                       <form>
                            <div class='input input--checkbox'>
                                <label class='small'>
                                    <input type='checkbox' autocomplete='off' class='select-all' hidden />
                                    <span class='input--checkbox__checkbox'>
                                        <span class='mdi mdi-check'></span>
                                    </span>
                                    You pledge to commit 3-4 hours or your time per week (although no two weeks are alike) 
                                </label>
                            </div>
                            <div class='input input--checkbox'>
                                <label class='small'>
                                    <input type='checkbox' autocomplete='off' class='select-all' hidden />
                                    <span class='input--checkbox__checkbox'>
                                        <span class='mdi mdi-check'></span>
                                    </span>
                                    You have previous training and experience in one or more of these practical and ethical frameworks (coaching; motivational interviewing; counselling, or an accredited form of therapy)
                                </label>
                            </div>
                       </form>
                    </div>";                        
                        
                            
            communityViewModel.ShowRequestHelp = false;
            communityViewModel.HeaderVolunteerButtonText = null;
            communityViewModel.ShowHelpExampleCards = false;
            communityViewModel.CommunityVolunteersHeader = "Welcome from Healthy London Partnership";


            communityViewModel.CommunityVolunteersTextHtml =
            @"<p>We aim to make London the healthiest global city by working with our partners to improve Londoners' health and wellbeing so everyone can live healthier lives.</p>
            <p>Our partners include the NHS in London (Clinical Commissioning Groups, Health Education England, NHS England, NHS Digital, NHS Improvement, trusts and providers), the Greater London Authority, the Mayor of 
            London, Public Health England and London Councils.</p>";

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Volunteer Name",
                    Role = "Role",
                    Location = "Location",
                    ImageLocation = "/img/icons/anonymous-user.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Volunteer Name",
                    Role = "Role",
                    Location = "Location",
                    ImageLocation = "/img/icons/anonymous-user.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Volunteer Name",
                    Role = "Role",
                    Location = "Location",
                    ImageLocation = "/img/icons/anonymous-user.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Volunteer Name",
                    Role = "Role",
                    Location = "Location",
                    ImageLocation = "/img/icons/anonymous-user.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Volunteer Name",
                    Role = "Role",
                    Location = "Location",
                    ImageLocation = "/img/icons/anonymous-user.png"
                },
            };


            communityViewModel.UsefulLinksHtml = new List<string>()
            {
                @"<a href=""https://www.healthylondon.org/"">Healthy London Partnership</a> - for more information on the work we do.",                
                @"<a href=""https://mhfaengland.org/"">MHFA England</a> - to learn more about mental health training",
            };

            return communityViewModel;

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
            communityViewModel.HeaderHTML = @"
                    <p class='row sm12 text-left mt-sm mb-xs'>
                        In our community there’s always somebody here to help, there’s no need for anyone to struggle alone.
                        We’re the Tankersley &amp; Pilley Community Helpers, here to help with:
                    </p>
                    <ul class='tick-list mt-xs mb-sm compact-list'>
                        <li>Shopping for essentials</li>
                        <li>A friendly chat</li>
                        <li>Help at home</li>
                        <li>Cooking a hot meal</li>
                    </ul>";



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
            communityViewModel.ZoomLevel = 14;

            communityViewModel.CommunityName = "Age UK";

            communityViewModel.BannerImageLocation = "/img/community/ageUK/ageUKlogo.png";

            communityViewModel.Header = "Veterans need your help!";
            communityViewModel.HeaderBullets = null;
            communityViewModel.HideHeaderButtons = false;
            communityViewModel.HideHeaderHelpButton = true;
            communityViewModel.HeaderHTML = @"
                    <p class='row sm12 text-left mt-sm mb-xs'>
                        We are pleased to announce the start of our new 'Vitals For Veterans' project.
                    </p>
                    <p class='row sm12 text-left mt-sm mb-xs'>
                        We are aiming to deliver wellbeing packs to veterans across Lincoln & South Lincolnshire over the next 6 months because
                        <i>veterans should not be forgotten</i>.
                    </p>";
            communityViewModel.CommunityVolunteersHeader = "Welcome from Age UK Lincoln and South Lincolnshire";
            communityViewModel.HeaderVolunteerButtonText = "If you can help us get wellbeing packs to veterans in your area please sign up to volunteer below:";



            communityViewModel.CommunityVolunteersTextHtml =
 @"<p>Age UK Lincoln & South Lincolnshire is an independent, local charity and we have been working in the local community to help older people, their families and carers for over 61 years. We have over 200 dedicated staff and volunteers helping us to deliver services and activities for older people in Lincoln and across the county.</p>
<p>Supporting over 4,000 people aged 50 and over every week, our dedicated staff and volunteers deliver support services and activities across 17 departments for people countywide.</p>
<p>Our support is varied and extensive, 11,845 people attended activities with us in 2018/19, helping to eliminate social isolation and generating opportunities for engagement through clubs and groups. Our day centre had 4700 attendances offering support to vulnerable older people who are otherwise alone during the day and allows us to provide respite for carers.</p>
<p>To join us or to get in touch, email <a href = ""mailto: info@ageuklsl.org.uk"">info@ageuklsl.org.uk</a></p> 
";
            communityViewModel.ShowRequestHelp = false;

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
