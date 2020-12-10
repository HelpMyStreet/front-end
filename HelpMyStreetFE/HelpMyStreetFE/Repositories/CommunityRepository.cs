﻿using HelpMyStreet.Utils.Utils;
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
            {"hlp", new CommunityModel(){FriendlyName = "Healthy London Partnership", Pin_Latitude = 51.507602, Pin_Longitude = -0.127816, LinkURL = "/healthylondonpartnership", Pin_VisibilityZoomLevel = 10, DisplayOnMap = false, BannerLocation = "/img/community/hlp/hlp-banner.png"} },
            {"tankersley", new CommunityModel(){FriendlyName = "Tankersley & Pilley Community Helpers", Pin_Latitude = 53.498113, Pin_Longitude = -1.488587, LinkURL = "/tankersley", Pin_VisibilityZoomLevel = 14, BannerLocation = "/img/community/tankersley/tankersley-st-peters-church.jpeg" } },
            {"ruddington", new CommunityModel(){FriendlyName = "Ruddington Community Response Team", Pin_Latitude = 52.8925, Pin_Longitude = -1.150, LinkURL = "/ruddington", Pin_VisibilityZoomLevel = 14.6, BannerLocation = "/img/community/ruddington/banner.jpg", GeographicName = "Ruddington" } },
            {"ageuklsl", new CommunityModel() {FriendlyName = "Age UK Lincoln & South Lincolnshire", Pin_Latitude = 53.2304334, Pin_Longitude = -0.5435425, LinkURL = "/ageuklsl", Pin_VisibilityZoomLevel = 9, DisplayOnMap = true, BannerLocation = "/img/community/ageUK/lsl/age-uk-lincoln-cathedral-banner.png"} },
            {"ageukwirral", new CommunityModel() {FriendlyName = "Age UK Wirral", Pin_Latitude = 53.37, Pin_Longitude = -3.05, LinkURL = "/ageukwirral", Pin_VisibilityZoomLevel = 9, DisplayOnMap = true, BannerLocation = "/img/community/ageUK/wirral/age-uk-wirral-banner-narrow.png"} },
            {"balderton", new CommunityModel() {FriendlyName = "Balderton Community Support", Pin_Latitude = 53.0561082, Pin_Longitude = -0.8, LinkURL = "/balderton", Pin_VisibilityZoomLevel = 12, DisplayOnMap = true, BannerLocation = "/img/community/ageUK/notts/balderton/banner-narrow.jpg", GeographicName="Balderton" } },
            {"north-muskham", new CommunityModel() {FriendlyName = "North Muskham Community Support", Pin_Latitude = 53.120254, Pin_Longitude = -0.811079, LinkURL = "/north-muskham", Pin_VisibilityZoomLevel = 12, DisplayOnMap = true, BannerLocation = "", GeographicName="North Muskham" } },
            {"ftlos", new CommunityModel{FriendlyName="For the Love of Scrubs", DisplayOnMap = false } },
        };

        public CommunityRepository(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<CommunityViewModel> GetCommunity(string groupKey, CancellationToken cancellationToken)
        {
            switch (groupKey)
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
            communityViewModel.Map_CentreLatitude = communityModel.Pin_Latitude;
            communityViewModel.Map_CentreLongitude = communityModel.Pin_Longitude;
            communityViewModel.Map_ZoomLevel = 13.5;

            communityViewModel.groupKey = "balderton";

            communityViewModel.CommunityName = communityModel.FriendlyName;
            communityViewModel.ShowRequestHelpPopup = true;

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
            communityViewModel.CarouselImages = new List<List<string>>
            {
                new List<string>
                {
                    $"{carouselPath}/carousel-1.jpg",
                    $"{carouselPath}/carousel-2.jpg",
                    $"{carouselPath}/carousel-3.jpg",
                    $"{carouselPath}/carousel-4.jpg",
                    $"{carouselPath}/carousel-5.jpg",
                    $"{carouselPath}/carousel-6.jpg",
                    $"{carouselPath}/carousel-7.jpg",
                    $"{carouselPath}/carousel-8.jpg",
                }
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
            communityViewModel.Map_CentreLatitude = communityModel.Pin_Latitude;
            communityViewModel.Map_CentreLongitude = communityModel.Pin_Longitude;
            communityViewModel.Map_ZoomLevel = 14;

            communityViewModel.CommunityName = communityModel.FriendlyName;
            communityViewModel.ShowRequestHelpPopup = true;

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
            communityViewModel.CarouselImages = new List<List<string>>
            {
                new List<string>
                {
                    $"{carouselPath}/view-over-trent.JPG",
                    $"{carouselPath}/view-towards-sugar-factory.JPG",
                }
            };

            return communityViewModel;
        }



        private async Task<CommunityViewModel> GetHLP(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "HLP" };
            CommunityModel communityModel = await GetCommunityDetailByKey("hlp");

            int groupId = await _groupService.GetGroupIdByKey("hlp", cancellationToken);

            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.Map_CentreLatitude = communityModel.Pin_Latitude;
            communityViewModel.Map_CentreLongitude = communityModel.Pin_Longitude;
            communityViewModel.Map_ZoomLevel = communityModel.Pin_VisibilityZoomLevel;

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
            communityViewModel.Map_CentreLatitude = communityModel.Pin_Latitude;
            communityViewModel.Map_CentreLongitude = communityModel.Pin_Longitude;
            communityViewModel.Map_ZoomLevel = communityModel.Pin_VisibilityZoomLevel;

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

            communityViewModel.HelpExampleCards = new Models.HelpExampleCardsViewModel();

            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetRuddington(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "Ruddington" };
            CommunityModel communityModel = await GetCommunityDetailByKey("ruddington");

            communityViewModel.groupKey = "ruddington";
            int groupId = await _groupService.GetGroupIdByKey(communityViewModel.groupKey, cancellationToken);
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.Map_CentreLatitude = communityModel.Pin_Latitude;
            communityViewModel.Map_CentreLongitude = communityModel.Pin_Longitude;
            communityViewModel.Map_ZoomLevel = communityModel.Pin_VisibilityZoomLevel;

            communityViewModel.CommunityName = communityModel.FriendlyName;
            communityViewModel.ShowRequestHelpPopup = true;

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
            communityViewModel.CarouselImages = new List<List<string>>
            {
                new List<string>
                {
                    $"{carouselPath}/img1.jpeg",
                    $"{carouselPath}/img2.jpeg",
                    $"{carouselPath}/img3.jpeg",
                    $"{carouselPath}/img4.jpeg",
                    $"{carouselPath}/img5.jpeg",
                }
            };

            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetAgeUKLSL(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "AgeUKLSL" };
            CommunityModel communityModel = await GetCommunityDetailByKey("ageuklsl");

            int groupId = await _groupService.GetGroupIdByKey("ageuklsl", cancellationToken);
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.Map_CentreLatitude = 52.95;
            communityViewModel.Map_CentreLongitude = -0.2;
            communityViewModel.Map_ZoomLevel = communityModel.Pin_VisibilityZoomLevel;

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
            communityViewModel.CarouselImages = new List<List<string>>
            {
                new List<string>
                {
                    $"{carouselPath}/img1.jpg",
                    $"{carouselPath}/img2.jpg",
                    $"{carouselPath}/img3.jpg",
                    $"{carouselPath}/img4.jpg",
                }
            };

            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetAgeUKWirral(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "AgeUKWirral" };

            int groupId = await _groupService.GetGroupIdByKey("ageukwirral", cancellationToken);
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.Map_CentreLatitude = 53.37;
            communityViewModel.Map_CentreLongitude = -3.05;
            communityViewModel.Map_ZoomLevel = 11.15;

            communityViewModel.groupKey = "ageukwirral";

            communityViewModel.CommunityName = "Age UK Wirral";
            communityViewModel.CommunityShortName = "Age UK Wirral";


            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Karen Giner",
                    Role = "Home & Communities Volunteer Coordinator",
                    ImageLocation = "/img/community/ageuk/wirral/KG.jpg"
                },
            };

            var carouselPath = "/img/community/ageUK/wirral/carousel1";
            communityViewModel.CarouselImages = new List<List<string>>
            {
                new List<string>
                {
                    $"{carouselPath}/hands-round-mug.jpg",
                    $"{carouselPath}/man-knocking-on-door-325x218.jpg",
                    $"{carouselPath}/photo-1483385573908-0a2108937c4a.jpg",
                    $"{carouselPath}/photo-1516862523118-a3724eb136d7.jpg",
                    $"{carouselPath}/photo-1587040273238-9ba47c714796.jpg",
                },
            };

            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetFtLOS(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "ForTheLoveOfScrubs" };
            communityViewModel.groupKey = "ftlos";

            int groupId = await _groupService.GetGroupIdByKey("ftlos", cancellationToken);
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.CommunityName = "For the Love of Scrubs";

            var carouselPath = "/img/community/fortheloveofscrubs";
            communityViewModel.CarouselImages = new List<List<string>>
            {
                new List<string>
                {
                    $"{carouselPath}/carousel1/1.jpeg",
                    $"{carouselPath}/carousel1/2.jpeg",
                    $"{carouselPath}/carousel1/3.jpeg",
                    $"{carouselPath}/carousel1/4.jpeg",
                    $"{carouselPath}/carousel1/5.jpeg",
                },
                new List<string>
                {
                    $"{carouselPath}/carousel2/1.jpeg",
                    $"{carouselPath}/carousel2/2.jpeg",
                    $"{carouselPath}/carousel2/3.jpeg",
                    $"{carouselPath}/carousel2/4.jpeg",
                    $"{carouselPath}/carousel2/5.jpeg",
                },
                new List<string>
                {
                    $"{carouselPath}/carousel3/A.png",
                    $"{carouselPath}/carousel3/B.png",
                    $"{carouselPath}/carousel3/C.png",
                    $"{carouselPath}/carousel3/D.png",
                    $"{carouselPath}/carousel3/E.png",
                }
            };

            return communityViewModel;
        }
    }
}
