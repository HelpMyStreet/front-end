using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Models.Community;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using System.Linq;
using HelpMyStreetFE.Services.Groups;

namespace HelpMyStreetFE.Repositories
{
    public class CommunityRepository : ICommunityRepository
    {
        private readonly IGroupService _groupService;
        private Dictionary<string, CommunityModel> Communities = new Dictionary<string, CommunityModel>()
        {
            {"hlp", new CommunityModel(){FriendlyName = "Healthy London Partnership", Latitude = 51.507602, Longitude = -0.127816, ReferenceName = "hlp", LinkURL = "/healthylondonpartnership", ZoomLevel = 10, DisplayOnMap = false, BannerLocation = "/img/community/hlp/hlp-banner.png"} },
            {"tankersley", new CommunityModel(){FriendlyName = "Tankersley & Pilley Community Helpers", Latitude = 53.498113, Longitude = -1.488587, ReferenceName = "tankersley", LinkURL = "/tankersley", ZoomLevel = 14, BannerLocation = "/img/community/tankersley/tankersley-st-peters-church.jpeg" } },
            {"ruddington", new CommunityModel(){FriendlyName = "Ruddington Community Response Team", Latitude = 52.8925, Longitude = -1.150, ReferenceName = "ruddington", LinkURL = "/ruddington", ZoomLevel = 14.6, BannerLocation = "/img/community/ruddington/banner.jpg", GeographicName = "Ruddington" } },
            {"ageuklsl", new CommunityModel() {FriendlyName = "Age UK Lincoln & South Lincolnshire", Latitude = 53.2304334, Longitude = -0.5435425, ReferenceName = "ageuklsl", LinkURL = "/ageuklsl", ZoomLevel = 9, DisplayOnMap = true, BannerLocation = "/img/community/ageUK/lsl/age-uk-lincoln-cathedral-banner.png"} },
            {"ageukwirral", new CommunityModel() {FriendlyName = "Age UK Wirral", Latitude = 53.37, Longitude = -3.05, ReferenceName = "ageukwirral", LinkURL = "/ageukwirral", ZoomLevel = 9, DisplayOnMap = true, BannerLocation = "/img/community/ageUK/wirral/age-uk-wirral-banner-narrow.png"} },
            {"balderton", new CommunityModel() {FriendlyName = "Balderton Community Support", Latitude = 53.0561082, Longitude = -0.8, ReferenceName = "balderton", LinkURL = "/balderton", ZoomLevel = 12, DisplayOnMap = true, BannerLocation = "/img/community/ageUK/notts/balderton/banner-narrow.jpg", GeographicName="Balderton" } },
            {"north-muskham", new CommunityModel() {FriendlyName = "North Muskham Community Support", Latitude = 53.120254, Longitude = -0.811079, ReferenceName = "north-muskham", LinkURL = "/north-muskham", ZoomLevel = 12, DisplayOnMap = true, BannerLocation = "", GeographicName="North Muskham" } },
            {"ftlos", new CommunityModel{FriendlyName="For the Love of Scrubs", DisplayOnMap = false } },
        };

        public CommunityRepository(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<CommunityViewModel> GetCommunity(string communityName, CancellationToken cancellationToken)
        {
            switch (communityName.Trim().ToLower())
            {
                case "tankersley":
                    return await GetTankersley(cancellationToken);
                case "ruddington":
                    return await GetRuddington(cancellationToken);
                case "ageuklsl":
                    return await GetAgeUKLSL(cancellationToken);
                case "ageukwirral":
                    return await GetAgeUKWirral(cancellationToken);
                case "hlp":
                    return await GetHLP(cancellationToken);
                case "ftlos":
                    return await GetFtLOS(cancellationToken);
                case "balderton":
                    return await GetBalderton(cancellationToken);
                case "north-muskham":
                    return await GetNorthMuskham(cancellationToken);
                default:
                    return null;
            }
        }

        public async Task<List<CommunityModel>> GetCommunities()
        {
            List<CommunityModel> returnCommunities = new List<CommunityModel>();
            foreach (var item in Communities){
                returnCommunities.Add(item.Value);
            }
            return returnCommunities;

        }

        public async Task<CommunityModel> GetCommunityDetailByKey(string key)
        {
            return Communities[key];
        }

        private async Task<CommunityViewModel> GetBalderton(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "Balderton" };
            CommunityModel communityModel = await GetCommunityDetailByKey("balderton");

            int groupId = await _groupService.GetGroupIdByKey("balderton", cancellationToken);
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.Latitude = communityModel.Latitude;
            communityViewModel.Longitude = communityModel.Longitude;
            communityViewModel.ZoomLevel = 13.5;

            communityViewModel.showFeedbackType = Models.Feedback.FeedbackMessageType.Other;
            communityViewModel.groupKey = "balderton";

            communityViewModel.CommunityName = communityModel.FriendlyName;

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Nick Fairfax",
                    Role = "Local Organiser",
                    Location = "",
                    ImageLocation = "/img/community/ageUK/notts/balderton/nick-fairfax.jpg"
                },
            };

            var carouselPath = "/img/community/ageUK/notts/balderton/carousel1";
            communityViewModel.CarouselImages1 = new List<string>
            {
                $"{carouselPath}/carousel-1.jpg",
                $"{carouselPath}/carousel-2.jpg",
                $"{carouselPath}/carousel-3.jpg",
                $"{carouselPath}/carousel-4.jpg",
                $"{carouselPath}/carousel-5.jpg",
                $"{carouselPath}/carousel-6.jpg",
                $"{carouselPath}/carousel-7.jpg",
                $"{carouselPath}/carousel-8.jpg",
            };

            return communityViewModel;
        }


        private async Task<CommunityViewModel> GetNorthMuskham(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel
            {
                groupKey = "north-muskham",
                View = "NorthMuskham",
            };

            CommunityModel communityModel = await GetCommunityDetailByKey(communityViewModel.groupKey);

            int groupId = await _groupService.GetGroupIdByKey(communityViewModel.groupKey, cancellationToken);
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.Latitude = communityModel.Latitude;
            communityViewModel.Longitude = communityModel.Longitude;
            communityViewModel.ZoomLevel = 14;

            communityViewModel.CommunityName = communityModel.FriendlyName;

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Nick Fairfax",
                    Role = "Local Organiser",
                    ImageLocation = "/img/community/ageUK/notts/balderton/nick-fairfax.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Rita Berkin",
                    Role = "Local Volunteer",
                    ImageLocation = "/img/community/ageUK/notts/north-muskham/rita-berkin.jpg"
                },
            };

            var carouselPath = "/img/community/ageUK/notts/north-muskham/carousel";
            communityViewModel.CarouselImages2 = new List<string>
            {
                $"{carouselPath}/view-over-trent.JPG",
                $"{carouselPath}/view-towards-sugar-factory.JPG",
            };

            return communityViewModel;
        }



        private async Task<CommunityViewModel> GetHLP(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "HLP" };
            CommunityModel communityModel = await GetCommunityDetailByKey("hlp");

            int groupId = await _groupService.GetGroupIdByKey("hlp", cancellationToken);

            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.Latitude = communityModel.Latitude;
            communityViewModel.Longitude = communityModel.Longitude;
            communityViewModel.ZoomLevel = communityModel.ZoomLevel;

            communityViewModel.CommunityName = communityModel.FriendlyName;
            communityViewModel.CommunityShortName = "Healthy London";


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

            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetTankersley(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "Tankersley" };
            CommunityModel communityModel = await GetCommunityDetailByKey("tankersley");

            int groupId = await _groupService.GetGroupIdByKey("tankersley", cancellationToken);
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.Latitude = communityModel.Latitude;
            communityViewModel.Longitude = communityModel.Longitude;
            communityViewModel.ZoomLevel = communityModel.ZoomLevel;

            communityViewModel.groupKey = "tankersley";

            communityViewModel.CommunityName = communityModel.FriendlyName;


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

            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetRuddington(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "Ruddington" };
            CommunityModel communityModel = await GetCommunityDetailByKey("ruddington");

            communityViewModel.showFeedbackType = Models.Feedback.FeedbackMessageType.Group;
            communityViewModel.groupKey = "ruddington";
            int groupId = await _groupService.GetGroupIdByKey(communityViewModel.groupKey, cancellationToken);
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.Latitude = communityModel.Latitude;
            communityViewModel.Longitude = communityModel.Longitude;
            communityViewModel.ZoomLevel = communityModel.ZoomLevel;

            communityViewModel.CommunityName = communityModel.FriendlyName;

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Ruddington Parish Council",
                    Role = "Proudly supported by the Parish Council",
                    Location = "",
                    ImageLocation = "/img/community/ruddington/RPC-logo.jpeg"
                },
                new CommunityVolunteer()
                {
                    Name = "Ruddington Village Centre Partnership",
                    Role = "Proudly supported by RVCP*",
                    Location = "",
                    ImageLocation = "/img/community/ruddington/RVCP.png"
                },
                new CommunityVolunteer()
                {
                    Name = "RUDDINGTON.info",
                    Role = "Promoting volunteer activities across the village",
                    Location = "",
                    ImageLocation = "/img/community/ruddington/ruddington-info-logo.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Han, Mark & Stella",
                    Role = "Market Volunteers",
                    Location = "",
                    ImageLocation = "/img/community/ruddington/HanMarkStella.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Leia",
                    Role = "Market Volunteer",
                    Location = "",
                    ImageLocation = "/img/community/ruddington/Leia.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Alex",
                    Role = "Community volunteer",
                    Location = "(photograph supplied by Pam Pearce)",
                    ImageLocation = "/img/community/ruddington/Alex.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Andrew",
                    Role = "Market Volunteer",
                    Location = "",
                    ImageLocation = "/img/community/ruddington/Andrew.jpg"
                },
            };

            var carouselPath = "/img/community/ruddington/carousel3";
            communityViewModel.CarouselImages3 = new List<string>
            {
                $"{carouselPath}/img1.jpeg",
                $"{carouselPath}/img2.jpeg",
                $"{carouselPath}/img3.jpeg",
                $"{carouselPath}/img4.jpeg",
                $"{carouselPath}/img5.jpeg",
            };

            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetAgeUKLSL(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "AgeUKLSL" };
            CommunityModel communityModel = await GetCommunityDetailByKey("ageuklsl");

            int groupId = await _groupService.GetGroupIdByKey("ageuklsl", cancellationToken);
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.Latitude = 52.95;
            communityViewModel.Longitude = -0.2;
            communityViewModel.ZoomLevel = communityModel.ZoomLevel;

            communityViewModel.showFeedbackType = Models.Feedback.FeedbackMessageType.Group;
            communityViewModel.groupKey = "ageuklsl";
            communityViewModel.CommunityName = communityModel.FriendlyName;
            communityViewModel.CommunityShortName = "Age UK LSL";



            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Nicki Lee",
                    Role = "Senior Volunteer Coordinator",
                    Location = "Lincoln & South Lincolnshire",
                    ImageLocation = "/img/community/ageuk/lsl/NL_cropped.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Amanda Wilson",
                    Role = "Engagement Coordinator",
                    Location = "Lincoln & South Lincolnshire",
                    ImageLocation = "/img/community/ageuk/lsl/AW_cropped.jpg"
                },
            };

            
            var carouselPath = "/img/community/ageUK/lsl/carousel1";
            communityViewModel.CarouselImages1 = new List<string>
            {
                $"{carouselPath}/img1.jpg",
                $"{carouselPath}/img2.jpg",
                $"{carouselPath}/img3.jpg",
                $"{carouselPath}/img4.jpg",
            };

            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetAgeUKWirral(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();

            int groupId = await _groupService.GetGroupIdByKey("ageukwirral", cancellationToken);
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.Latitude = 53.37;
            communityViewModel.Longitude = -3.05;
            communityViewModel.ZoomLevel = 11.15;

            communityViewModel.showFeedbackType = Models.Feedback.FeedbackMessageType.Group;
            communityViewModel.groupKey = "ageukwirral";

            communityViewModel.CommunityName = "Age UK Wirral";
            communityViewModel.CommunityShortName = "Age UK Wirral";

            communityViewModel.BannerImageLocation = "/img/community/ageUK/wirral/age-uk-wirral-banner.png";

            communityViewModel.Header = "In the Wirral, help is always available!";
            communityViewModel.HeaderHTML = @"
                    <p class='mt-sm mb-xs'>
                        Age UK Wirral are proud to have an amazing range of services for older people in our local communities. Our services are supported by volunteers who are here to help with:
                    </p>
                    <p class='mt-sm mb-xs'>
                        <ul class='tick-list mt-xs mb-sm compact-list'>
                            <li>Shopping for essentials</li>
                            <li>Collecting prescriptions</li>
                            <li>Staying warm and healthy in the chilly winter</li>
                            <li>Door-to-door transport for medical appointments</li>
                        </ul>
                    </p>";
            communityViewModel.CommunityVolunteersHeader = "Welcome from Age UK Wirral";
            communityViewModel.HeaderVolunteerButtonText = null;
            communityViewModel.HeaderButtonWidth = 12;

            communityViewModel.CommunityVolunteersTextHtml =
                 @"<p>
                    Age UK Wirral is an independent, local charity and we have been working in the local community to help 
                    older people, their families and carers for over 70 years. We have 700 dedicated staff and volunteers helping 
                    us to deliver services and activities for older people in the Wirral.
                </p>
                <p>
                     In March 2020 we launched our Covid-19 Emergency Response Services to support people with shopping, 
                     prescription collections and wellbeing support. In six months we have carried out over 3,000 shopping trips, 
                     collected over 500 prescriptions, lent 250 people tablets and data sim cards and made more than 35,000 wellbeing calls 
                     to isolated people in the community.  These services will continue to support the community and are vital to keep people safe, well and connected.
                </p>
                <p>
                     The services on this page are a selection of those available, focussing on areas where we need ad hoc 
                     volunteer assistance.
                </p>
                <p>
                    To find out more about our other services and the support we can provide, please go to our main website <a href=""www.ageukwirral.org.uk"">www.ageukwirral.org.uk</a>
                    or call us on 0151 482 3456.
                </p> 
                ";
            communityViewModel.ShowRequestHelp = false;

            communityViewModel.RequestHelpHeading = @"How can we help?";

            communityViewModel.RequestHelpText = @"We support older people, their families and carers. To find out more about our other services and the support we can provide, please go to our website or call us on 0151 482 3456.";
            communityViewModel.HeaderVolunteerButtonText = "Age UK Wirral relies on volunteers to help us offer vital services in the local community. Would you like to lend a hand?";

            communityViewModel.AllowJoinOurGroup = true;
            communityViewModel.JoinOurGroupButtonText = "Join Our Group";

            communityViewModel.AllowLeaveOurGroup = true;

            communityViewModel.ProvideHelpHeading = "Volunteer with us!";

            communityViewModel.ProvideHelpText_NotGroupMember = "Join us to help your neighbours. Just let us know when, where and how you can help. You can choose to help a little, or to help a lot! We’re grateful for every contribution.";
            communityViewModel.ProvideHelpText_GroupMember = "Thanks for being part of AgeUK Wirral.  Click below to view help requests in your area.";

            communityViewModel.ShowVisitWebsite = true;
            communityViewModel.VisitWebsiteHeading = "How can we help?";
            communityViewModel.VisitWebsiteText = "We support older people, their families and carers. To find out more about our other services and the support we can provide, please go to our website or call us on 0151 482 3456.";
            communityViewModel.VisitWebsiteButtonText = "Go to our website";
            communityViewModel.WebsiteUrl = "https://www.ageuk.org.uk/wirral/";
            communityViewModel.HeaderVisitWebsiteButtonText = "To ask for help, please go to our website or call us on 0151 482 3456.";

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Karen Giner",
                    Role = "Home & Communities Volunteer Coordinator",
                    ImageLocation = "/img/community/ageuk/wirral/KG.jpg"
                },
            };

            communityViewModel.UsefulLinksHtml =
                @"<p><a href=""https://www.ageuk.org.uk/wirral/"">Age UK Wirral Website</a> - Find out about our extensive range of services, donations, charity shops and further details about our organisation.</p>
                <p><a href=""https://www.facebook.com/ageukwirral/"">Age UK Wirral Facebook</a> - Facebook page of Age UK Wirral.</p>
                <p><a href=""https://www.wirralinfobank.co.uk/"">Wirral InfoBank</a> - The place where Wirral residents can find local community support services, online events and up-to-date advice and information about coronavirus (COVID-19).</p>
                <p><a href=""/pdf/ageUK/wirral/WirralVolunteerInstructions.pdf"">Volunteer Instructions</a> - Read our how-to guide (including frequently asked questions).</p>";
           
            var carouselPath = "/img/community/ageUK/wirral/carousel1";
            communityViewModel.CarouselImages1 = new List<string>
            {
                $"{carouselPath}/hands-round-mug.jpg",
                $"{carouselPath}/man-knocking-on-door-325x218.jpg",
                $"{carouselPath}/photo-1483385573908-0a2108937c4a.jpg",
                $"{carouselPath}/photo-1516862523118-a3724eb136d7.jpg",
                $"{carouselPath}/photo-1587040273238-9ba47c714796.jpg",
            };

            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetFtLOS(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "ForTheLoveOfScrubs" };
            communityViewModel.showFeedbackType = Models.Feedback.FeedbackMessageType.FaceCovering;
            communityViewModel.groupKey = "ftlos";

            int groupId = await _groupService.GetGroupIdByKey("ftlos", cancellationToken);
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.CommunityName = "For the Love of Scrubs";

            var carouselPath = "/img/community/fortheloveofscrubs";
            communityViewModel.CarouselImages1 = new List<string>
            {
                $"{carouselPath}/carousel1/1.jpeg",
                $"{carouselPath}/carousel1/2.jpeg",
                $"{carouselPath}/carousel1/3.jpeg",
                $"{carouselPath}/carousel1/4.jpeg",
                $"{carouselPath}/carousel1/5.jpeg",
            };
            communityViewModel.CarouselImages2 = new List<string>
            {
                $"{carouselPath}/carousel2/1.jpeg",
                $"{carouselPath}/carousel2/2.jpeg",
                $"{carouselPath}/carousel2/3.jpeg",
                $"{carouselPath}/carousel2/4.jpeg",
                $"{carouselPath}/carousel2/5.jpeg",
            };
            communityViewModel.CarouselImages3 = new List<string>
            {
                $"{carouselPath}/carousel3/A.png",
                $"{carouselPath}/carousel3/B.png",
                $"{carouselPath}/carousel3/C.png",
                $"{carouselPath}/carousel3/D.png",
                $"{carouselPath}/carousel3/E.png",
            };

            return communityViewModel;
        }
    }
}
