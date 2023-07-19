using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Models.Community;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using System.Linq;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreet.Utils.Models;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Google.Apis.Util;
using HelpMyStreet.Utils.Extensions;
using System;
using HelpMyStreetFE.Services;

namespace HelpMyStreetFE.Repositories
{
    public class CommunityRepository : ICommunityRepository
    {
        private readonly IGroupService _groupService;
        private readonly INewsTickersService _newsTickerService;

        public CommunityRepository(IGroupService groupService, INewsTickersService newsTickerService)
        {
            _groupService = groupService;
            _newsTickerService = newsTickerService;
        }

        private async Task<CommunityViewModel> GetCommunity(Group group, CancellationToken cancellationToken, string language = null)
        {
            CommunityViewModel vm = ((Groups)group.GroupId) switch
            {
                Groups.Ruddington => GetRuddington(),
                Groups.AgeUKWirral => await GetAgeUKWirral(cancellationToken),
                Groups.Southwell => GetSouthwell(),
                _ => null,
            };

            vm.Group = group;

            return vm;

        }

        public async Task<CommunityViewModel> GetCommunity(int groupId, CancellationToken cancellationToken)
        {
            var group = await _groupService.GetGroupById(groupId, cancellationToken);
            return await GetCommunity(group, cancellationToken);
        }

        public async Task<CommunityViewModel> GetCommunity(string groupKey, string language, CancellationToken cancellationToken)
        {
            var group = await _groupService.GetGroupByKey(groupKey, cancellationToken);
            return await GetCommunity(group, cancellationToken, language);
        }

        public async Task<List<CommunityModel>> GetCommunities()
        {
            List<CommunityModel> returnCommunities = new List<CommunityModel>();
            var groups = await _groupService.GetGroupsWithMapDetails(MapLocation.HomePage, CancellationToken.None);

            if (groups != null)
            {
                groups
                    .ToList()
                    .ForEach(group =>
                        returnCommunities.Add(GetCommunityModel(group, MapLocation.HomePage))
                    );
            }
            return returnCommunities;

        }

        private CommunityModel GetCommunityModel(Group group, MapLocation mapLocation)
        {
            var communityModel = new CommunityModel()
            {
                FriendlyName = group.FriendlyName,
                GeographicName = group.GeographicName,
                GroupType = group.GroupType.GetString(),
                LinkURL = group.LinkURL,
                BannerLocation = $"/img/homepagemapbanner/{group.GroupKey}-banner.jpg",
                JoinGroupPopUpDetail = group.JoinGroupPopUpDetail
            };
            
            var maps = group.Maps.FirstOrDefault(x => x.MapLocation == mapLocation);

            if (maps != null)
            {
                communityModel.Pin_Latitude = maps.Latitude;
                communityModel.Pin_Longitude = maps.Longitude;
                communityModel.Pin_VisibilityZoomLevel = maps.ZoomLevel;
                communityModel.DisplayOnMap = true;
            }
            return communityModel;
        }

        private CommunityViewModel GetCommunityViewModel(Group group, string viewName, bool showRequestHelpPopup, bool showPopupOnSignUp)
        {
            var communityViewModel = new CommunityViewModel() 
            { 
                CommunityName = group.FriendlyName, 
                View = viewName, 
                CommunityShortName = group.ShortName, 
                ShowRequestHelpPopup = showRequestHelpPopup,
                ShowPopupOnSignUp = showPopupOnSignUp
            };
            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>() { };

            var maps = group.Maps.FirstOrDefault(x => x.MapLocation == MapLocation.Landing);

            if (maps != null)
            {
                communityViewModel.Map_CentreLatitude = maps.Latitude;
                communityViewModel.Map_CentreLongitude = maps.Longitude;
                communityViewModel.Map_ZoomLevel = maps.ZoomLevel;
            }
            return communityViewModel;
        }

        public CommunityModel GetCommunityDetailByKey(string key)
        {
            var group = _groupService.GetGroupByKey(key, CancellationToken.None).Result;

            if (group != null)
            {
                return GetCommunityModel(group, MapLocation.Landing);
            }
            else
            {
                return null;
            }
        }

        private CommunityViewModel GetCommunityViewModelByKey(string key, string viewName, bool showRequestHelpPopup = false, bool showPopupOnSignUp= false)
        {
            var group = _groupService.GetGroupByKey(key, CancellationToken.None).Result;

            if (group != null)
            {
                return GetCommunityViewModel(group, viewName, showRequestHelpPopup, showPopupOnSignUp);
            }
            else
            {
                return null;
            }
        }

        private CommunityViewModel GetSouthwell()
        {
            CommunityViewModel communityViewModel = GetCommunityViewModelByKey("southwell", "Southwell", true);

            var carouselPath = "/img/community/southwell/carousel";
            communityViewModel.CarouselImages = new List<List<string>>
            {
                new List<string>
                {
                    $"{carouselPath}/southwell-image-1.png",
                    $"{carouselPath}/southwell-image-2.png",
                    $"{carouselPath}/southwell-image-3.png",
                    $"{carouselPath}/southwell-image-4.png",
                    $"{carouselPath}/southwell-image-5.png",
                }
            };

            return communityViewModel;
        }
                
        private CommunityViewModel GetRuddington()
        {
            CommunityViewModel communityViewModel = GetCommunityViewModelByKey("ruddington", "Ruddington", true);
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

        private async Task<CommunityViewModel> GetAgeUKWirral(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = GetCommunityViewModelByKey("ageukwirral", "AgeUKWirral");
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

            communityViewModel.NewsTickerMessages = await _newsTickerService.GetNewsTickerMessages((int)Groups.AgeUKWirral, cancellationToken);

            return communityViewModel;
        }
    }
}
