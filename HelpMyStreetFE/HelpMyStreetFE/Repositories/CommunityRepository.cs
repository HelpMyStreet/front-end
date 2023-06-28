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
                Groups.Tankersley => GetTankersley(),
                Groups.Ruddington => GetRuddington(),
                Groups.AgeUKLSL => GetAgeUKLSL(),
                Groups.AgeUKWirral => await GetAgeUKWirral(cancellationToken),
                Groups.AgeUKNottsBalderton => GetBalderton(),
                Groups.AgeUKNottsNorthMuskham => GetNorthMuskham(),
                Groups.AgeUKSouthKentCoast => GetSouthKentCoast(),
                Groups.AgeUKFavershamAndSittingbourne => GetFavershameAndSittingBourne(),
                Groups.AgeUKNorthWestKent => GetNorthWestKent(),
                Groups.LincolnshireVolunteers => await GetLincolnshireVolunteers(cancellationToken),
                Groups.AgeConnectsCardiff => GetAgeConnectsCardiff(),
                Groups.MeadowsCommunityHelpers => GetMeadowsCommunityHelpers(),
                Groups.Southwell => GetSouthwell(),
                Groups.ApexBankStaff => GetApexBankStaff(),
                Groups.AgeUKMidMersey => GetAgeUKMidMersey(),
                Groups.BostonGNS => GetBostonGNS(language),
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

        private CommunityViewModel GetAgeConnectsCardiff()
        {
            CommunityViewModel communityViewModel = GetCommunityViewModelByKey("ageconnects-cardiff", "AgeConnectsCardiff", true, true);
            var carouselPath = "/img/community/ageconnectscardiff/carousel1";
            communityViewModel.CarouselImages = new List<List<string>>
            {
                new List<string>
                {
                    $"{carouselPath}/carousel-2.jpeg",
                    $"{carouselPath}/carousel-1.jpeg",
                    $"{carouselPath}/carousel-3.jpeg",
                    $"{carouselPath}/carousel-5.jpeg",
                    $"{carouselPath}/carousel-4.jpeg",
                }
            };

            return communityViewModel;
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

        private CommunityViewModel GetBalderton()
        {
            CommunityViewModel communityViewModel = GetCommunityViewModelByKey("balderton", "Balderton", true);
            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Nick Fairfax",
                    Role = "Local Volunteer",
                    Location = "",
                    ImageLocation = "/img/community/ageUK/notts/balderton/nick-fairfax.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Rita Berkin",
                    Role = "Local Volunteer",
                    Location = "",
                    ImageLocation = "/img/community/ageUK/notts/balderton/rita-birkin.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Kirstie Crook",
                    Role = "Local Volunteer",
                    Location = "",
                    ImageLocation = "/img/community/ageUK/notts/balderton/kirstie-crook.JPG"
                },
                new CommunityVolunteer()
                {
                    Name = "Richard Hartley",
                    Role = "Local Volunteer",
                    Location = "",
                    ImageLocation = "/img/community/ageUK/notts/balderton/richard-hartley-2.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Dawn Munslow",
                    Role = "Local Volunteer",
                    Location = "",
                    ImageLocation = "/img/community/ageUK/notts/balderton/dawn-munslow.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Steve Pollard",
                    Role = "Local Volunteer",
                    Location = "",
                    ImageLocation = "/img/community/ageUK/notts/balderton/steve-pollard.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Eunice Ray",
                    Role = "Local Volunteer",
                    Location = "",
                    ImageLocation = "/img/community/ageUK/notts/balderton/eunice-ray.jpg"
                }
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

        private CommunityViewModel GetNorthMuskham()
        {
            CommunityViewModel communityViewModel = GetCommunityViewModelByKey("north-muskham", "NorthMuskham", true);
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

        private CommunityViewModel GetSouthKentCoast()
        {
            CommunityViewModel communityViewModel = GetCommunityViewModelByKey("ageuk-southkentcoast", "SouthKentCoast", true);

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Debbie Barry",
                    Role = "Chief Executive Officer",
                    ImageLocation = "/img/community/ageuk/kent/southkentcoast/DB.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Mary Colley",
                    Role = "Volunteer Coordinator",
                    ImageLocation = "/img/community/ageuk/kent/southkentcoast/MC.jpg"
                },
            };

            var carouselPath = "/img/community/ageUK/kent/southkentcoast/carousel";
            communityViewModel.CarouselImages = new List<List<string>>
            {
                new List<string>
                {
                    $"{carouselPath}/SKC2.JPG",
                    $"{carouselPath}/SKC4.JPG",
                    $"{carouselPath}/SKC5.JPG",
                    $"{carouselPath}/SKC6.JPG",
                    $"{carouselPath}/SKC7.JPG",
                    $"{carouselPath}/SKC7a.JPG",
                }
            };

            return communityViewModel;
        }

        private CommunityViewModel GetMeadowsCommunityHelpers()
        {
            CommunityViewModel communityViewModel = GetCommunityViewModelByKey("meadows-community-helpers", "MeadowsComunityHelpers", true);
            communityViewModel.HelpExampleCards = new Models.HelpExampleCardsViewModel();
            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Notts County Foundation",
                    Role = "",
                    Location = "",
                    IsLogo = true,
                    ImageLocation = "/img/community/meadows/notts_county_foundation.png"
                },
                new CommunityVolunteer()
                {
                    Name = "The Bridges Community Trust",
                    Role = "",
                    Location = "",
                    IsLogo = true,
                    ImageLocation = "/img/community/meadows/the_bridges_trust.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Meadows Community Helpers",
                    Role = "",
                    Location = "",
                    IsLogo = true,
                    ImageLocation = "/img/community/meadows/meadows_community_helpers.png"
                }
            };

            var carouselPath = "/img/community/meadows/carousel";
            communityViewModel.CarouselImages = new List<List<string>>
            {
                new List<string>
                {
                    $"{carouselPath}/meadowscarousel1.JPG",
                    $"{carouselPath}/meadowscarousel2.JPEG",
                    $"{carouselPath}/meadowscarousel3.JPG",
                    $"{carouselPath}/meadowscarousel4.JPG"
                }
            };

            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetLincolnshireVolunteers(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = GetCommunityViewModelByKey("lincs-volunteers", "LincolnshireVolunteers", true);
            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Lincolnshire CVS",
                    Role = "",
                    Location = "",
                    IsLogo = true,
                    ImageLocation = "/img/community/vacc/lincolnshirevolunteers/LCVS-Master-New.jpg",
                    LogoHyperLink = "http://www.lincolnshirecvs.org.uk/",
                    ShowSmallLogo = true

                },
                new CommunityVolunteer()
                {
                    Name = "Voluntary Centre Services",
                    Role = "",
                    Location = "",
                    IsLogo = true,
                    ImageLocation = "/img/community/vacc/lincolnshirevolunteers/cropped-vcs-logo-new-site.png",
                    LogoHyperLink = "https://voluntarycentreservices.org.uk/",
                    ShowSmallLogo = true

                },
                new CommunityVolunteer()
                {
                    Name = "VET",
                    Role = "",
                    Location = "",
                    IsLogo = true,
                    ImageLocation = "/img/community/vacc/lincolnshirevolunteers/cropped-VET_Logo.png",
                    LogoHyperLink = "https://lvet.co.uk/",
                    ShowSmallLogo = true
                },
                new CommunityVolunteer()
                {
                    Name = "Age UK",
                    Role = "",
                    Location = "",
                    IsLogo = true,
                    ImageLocation = "/img/community/vacc/lincolnshirevolunteers/age-uk-lincoln--south-lincolnshire-logo-rgb.png",
                    LogoHyperLink = "https://www.ageuk.org.uk/lincolnsouthlincolnshire/",
                    ShowSmallLogo = true
                },
                new CommunityVolunteer()
                {
                    Name = "Local Resilience Forum",
                    Role = "",
                    Location = "",
                    IsLogo = true,
                    ImageLocation = "/img/community/vacc/lincolnshirevolunteers/lrf-logo.jpg",
                    LogoHyperLink = "https://lincolnshireresilienceforum.org/",
                    ShowSmallLogo = true
                },
                new CommunityVolunteer()
                {
                    Name = "NHS Lincolnshire",
                    Role = "",
                    Location = "",
                    IsLogo = true,
                    ImageLocation = "/img/community/vacc/lincolnshirevolunteers/nhs-lincolnshire.jpg",
                    LogoHyperLink = "https://www.lincolnshire.nhs.uk/",
                    ShowSmallLogo = true
                },
                new CommunityVolunteer()
                {
                    Name = "Lincolnshire County Council",
                    Role = "",
                    Location = "",
                    IsLogo = true,
                    ImageLocation = "/img/community/vacc/lincolnshirevolunteers/Lincolnshire-LOGO.png",
                    LogoHyperLink = "https://www.lincolnshire.gov.uk/",
                    ShowSmallLogo = true
                },
            };

            communityViewModel.NewsTickerMessages = await _newsTickerService.GetNewsTickerMessages((int)Groups.LincolnshireVolunteers, cancellationToken);

            return communityViewModel;
        }

        private CommunityViewModel GetApexBankStaff()
        {
            CommunityViewModel communityViewModel = GetCommunityViewModelByKey("apex-pcn-bank-staff", "ApexPCNBankStaff", true);
            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Dr Nick Smith",
                    Role = "Clinical Director",
                    Location = "",
                    IsLogo = true,
                    ImageLocation = "/img/community/vacc/apex-pcn-bank-staff/dr-smith.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Dr Rama Mark",
                    Role = "Clinical Director",
                    Location = "",
                    IsLogo = true,
                    ImageLocation = "/img/community/vacc/apex-pcn-bank-staff/rama.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Gary Burroughs",
                    Role = "PCN Manager",
                    Location = "",
                    IsLogo = true,
                    ImageLocation = "/img/community/vacc/apex-pcn-bank-staff/gary-burrows.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Fiona Roche",
                    Role = "Locality Lead",
                    Location = "",
                    IsLogo = true,
                    ImageLocation = "/img/icons/person-placeholder.png"
                }
            };

            return communityViewModel;
        }

        private CommunityViewModel GetFavershameAndSittingBourne()
        {
            CommunityViewModel communityViewModel = GetCommunityViewModelByKey("ageuk-favershamandsittingbourne", "FavershamAndSittingBourne", true);

            var carouselPath = "/img/community/ageUK/kent/favershamandsittingbourne/carousel";
            communityViewModel.CarouselImages = new List<List<string>>
            {
                new List<string>
                {
                    $"{carouselPath}/FS1.JPG",
                    $"{carouselPath}/FS2.JPG",
                    $"{carouselPath}/FS3.JPG",
                    $"{carouselPath}/FS4.JPG",
                    $"{carouselPath}/FS5.JPG",
                }
            };

            return communityViewModel;
        }

        private CommunityViewModel GetNorthWestKent()
        {
            CommunityViewModel communityViewModel = GetCommunityViewModelByKey("ageuknwkent", "NorthWestKent", true);
            var carouselPath = "/img/community/ageUK/kent/northwest/carousel";

            communityViewModel.CarouselImages = new List<List<string>>
            {
                new List<string>
                {
                    $"{carouselPath}/carousel-1.jpg",
                    $"{carouselPath}/carousel-2.jpg",
                    $"{carouselPath}/carousel-3.jpg",
                    $"{carouselPath}/carousel-4.jpg",
                    $"{carouselPath}/carousel-5.jpg",
                }
            };

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Kate Smith",
                    Role = "Joint Chief Executive",
                    Location = "",
                    ImageLocation = "/img/community/ageUK/kent/northwest/kate-smith.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Chriss Monks",
                    Role = "Joint Chief Executive",
                    Location = "",
                    ImageLocation = "/img/community/ageUK/kent/northwest/chriss-monks.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Kirsty Groves",
                    Role = "Community Services Administrator",
                    Location = "",
                    ImageLocation = "/img/community/ageUK/kent/northwest/kirsty-groves.jpg"
                }
            };
            return communityViewModel;
        }

        private CommunityViewModel GetTankersley()
        {
            CommunityViewModel communityViewModel = GetCommunityViewModelByKey("tankersley", "Tankersley",true);
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

        private CommunityViewModel GetAgeUKLSL()
        {
            CommunityViewModel communityViewModel = GetCommunityViewModelByKey("ageuklsl", "AgeUKLSL");
            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>();

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

        private CommunityViewModel GetAgeUKMidMersey()
        {
            CommunityViewModel communityViewModel = GetCommunityViewModelByKey("ageuk-midmersey", "AgeUKMidMersey");
            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Shelley Brown",
                    Role = "Chief Executive Officer",
                    ImageLocation = "/img/community/ageuk/midmersey/SB.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Judy Bebbington",
                    Role = "Volunteer Coordinator",
                    ImageLocation = "/img/icons/person-placeholder.png"
                },
            };

            var carouselPath = "/img/community/ageUK/midmersey/carousel1";
            communityViewModel.CarouselImages = new List<List<string>>
            {
                new List<string>
                {
                    $"{carouselPath}/carousel-1.png",
                    $"{carouselPath}/carousel-2.png",
                    $"{carouselPath}/carousel-3.png",
                    $"{carouselPath}/carousel-4.png",
                    $"{carouselPath}/carousel-5.jpg",
                },
            };

            return communityViewModel;
        }

        private CommunityViewModel GetBostonGNS(string pLanguage)
        {
            string language = "English";
            if(!string.IsNullOrEmpty(pLanguage))
            {
                language = pLanguage;
            }
            CommunityViewModel communityViewModel = GetCommunityViewModelByKey("boston", $"BostonGNS{language}");
            communityViewModel.Language = language;

            communityViewModel.Flags = new List<FlagViewModel>()
            {
                new FlagViewModel()
                {
                    Name = "English",
                    ImageLocation = "/img/community/bostongns/flags/united-kingdom-flag-small.png",
                    Language = "English"
                },
                new FlagViewModel()
                {
                    Name = "Polski",
                    ImageLocation = "/img/community/bostongns/flags/poland-flag-small.png",
                    Language = "Polish"
                },
                new FlagViewModel()
                {
                    Name = "Lietuvių",
                    ImageLocation = "/img/community/bostongns/flags/lithuania-flag-small.png",
                    Language = "Lithuanian"
                },
                new FlagViewModel()
                {
                    Name = "Русский",
                    ImageLocation = "/img/community/bostongns/flags/russia-flag-small.png",
                    Language = "Russian"
                },
                new FlagViewModel()
                {
                    Name = "Latviski",
                    ImageLocation = "/img/community/bostongns/flags/latvia-flag-small.png",
                    Language = "Latvian"
                },
                new FlagViewModel()
                {
                    Name = "Română",
                    ImageLocation = "/img/community/bostongns/flags/romania-flag-small.png",
                    Language = "Romanian"
                },
                new FlagViewModel()
                {
                    Name = "български",
                    ImageLocation = "/img/community/bostongns/flags/bulgaria-flag-small.png",
                    Language = "Bulgarian"
                },
                new FlagViewModel()
                {
                    Name = "Português",
                    ImageLocation = "/img/community/bostongns/flags/portugal-flag-small.png",
                    Language = "Portugese"
                }
            };

            communityViewModel.Flags.First(f => f.Language.Equals(language)).IsSelected = true;

            Dictionary<Tuple<string, string>, string> dict = new Dictionary<Tuple<string, string>, string>();
            
            dict.Add(new Tuple<string, string>("Bulgarian", "Supported by Boston Borough Council’s Empowering Healthy Communities Programme"), "Подкрепа по Програмата на Общинския Съвет на Бостън “Глас за Здрави Общности”");
            dict.Add(new Tuple<string, string>("Bulgarian", "A community project bringing people together"), "Общностен проект, който обединява хората");
            dict.Add(new Tuple<string, string>("Bulgarian", "Training and support provided by YMCA Lincolnshire"), "Обучение и подкрепа, осигурени от YMCA Линкълншър");
            dict.Add(new Tuple<string, string>("Bulgarian", "Proudly funded by The National Lottery Community Fund"), "Финансиран с гордост от Обществения Фонд на Националната лотария");

            dict.Add(new Tuple<string, string>("Lithuanian", "Supported by Boston Borough Council’s Empowering Healthy Communities Programme"),"Rėmėjas – Bostono rajono tarybos programa „Sveikų bendruomenių įgalinimas“");
            dict.Add(new Tuple<string, string>("Lithuanian", "A community project bringing people together"), "Bendruomenės projektas, vienijantis žmones");
            dict.Add(new Tuple<string, string>("Lithuanian", "Training and support provided by YMCA Lincolnshire"), "Apmokymus ir paramą tekia „YMCA Lincolnshire“");
            dict.Add(new Tuple<string, string>("Lithuanian", "Proudly funded by The National Lottery Community Fund"), "Finansavimą teikia Nacionalinės loterijos bendruomenės fondas");

            dict.Add(new Tuple<string, string>("Latvian", "Supported by Boston Borough Council’s Empowering Healthy Communities Programme"), "Atbalsta Bostonas rajona padomes programma \"Veselīgu kopienu stiprināšanai\"");
            dict.Add(new Tuple<string, string>("Latvian", "A community project bringing people together"), "Sabiedrības projekts, kas apvieno cilvēkus");
            dict.Add(new Tuple<string, string>("Latvian", "Training and support provided by YMCA Lincolnshire"), "YMCA Lincolnshire nodrošinātā apmācība un atbalsts");
            dict.Add(new Tuple<string, string>("Latvian", "Proudly funded by The National Lottery Community Fund"), "Ar lepnumu finansē Nacionālās loterijas kopienas fonds");

            dict.Add(new Tuple<string, string>("Polish", "Supported by Boston Borough Council’s Empowering Healthy Communities Programme"), "Wspierany przez program Rady Miasta Boston \"Empowering Healthy Communities\" (Wzmacnianie Zdrowych Społeczności).");
            dict.Add(new Tuple<string, string>("Polish", "A community project bringing people together"), "Projekt wspólnotowy zbliżający ludzi");
            dict.Add(new Tuple<string, string>("Polish", "Training and support provided by YMCA Lincolnshire"), "Szkolenie i wsparcie zapewnione przez YMCA Lincolnshire");
            dict.Add(new Tuple<string, string>("Polish", "Proudly funded by The National Lottery Community Fund"), "Dumnie finansowany przez Fundusz Społeczny Loterii Narodowej");

            dict.Add(new Tuple<string, string>("Portugese", "Supported by Boston Borough Council’s Empowering Healthy Communities Programme"), "Com o apoio do Programa Empowering Healthy Communities do Boston Borough Council");
            dict.Add(new Tuple<string, string>("Portugese", "A community project bringing people together"), "Um projeto comunitário que une as pessoas");
            dict.Add(new Tuple<string, string>("Portugese", "Training and support provided by YMCA Lincolnshire"), "Formação e apoio prestados pela YMCA Lincolnshire");
            dict.Add(new Tuple<string, string>("Portugese", "Proudly funded by The National Lottery Community Fund"), "Orgulhosamente financiado pelo Fundo Comunitário da Lotaria Nacional (The National Lottery Community Fund)");

            dict.Add(new Tuple<string, string>("Romanian", "Supported by Boston Borough Council’s Empowering Healthy Communities Programme"), "Susținute prin Programul „Empowering Healthy Communities” al Consiliului Districtual Boston");
            dict.Add(new Tuple<string, string>("Romanian", "A community project bringing people together"), "Un proiect comunitar care aduce oamenii împreună");
            dict.Add(new Tuple<string, string>("Romanian", "Training and support provided by YMCA Lincolnshire"), "Instruire și support oferite de Asociația Creștină pentru Tineret ( YMCA) din Lincolnshire");
            dict.Add(new Tuple<string, string>("Romanian", "Proudly funded by The National Lottery Community Fund"), "Finanțat cu mândrie de Fondul Comunitar al Loteriei Naționale");

            dict.Add(new Tuple<string, string>("Russian", "Supported by Boston Borough Council’s Empowering Healthy Communities Programme"), "При поддержке программы «Расширение возможностей здоровых сообществ» Совета Бостонского округа");
            dict.Add(new Tuple<string, string>("Russian", "A community project bringing people together"), "Общественный проект, объединяющий людей");
            dict.Add(new Tuple<string, string>("Russian", "Training and support provided by YMCA Lincolnshire"), "Обучение и поддержка, предоставляемые Христианской Ассоциацией молодых людей (YMCA)  в Линкольншире");
            dict.Add(new Tuple<string, string>("Russian", "Proudly funded by The National Lottery Community Fund"), "Финансируется Фондом сообществ Национальной лотереи");

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Location = GetTranslationForCommunityVolunteer(dict,language,"Supported by Boston Borough Council’s Empowering Healthy Communities Programme"),
                    IsLogo = true,
                    ImageLocation = "/img/community/bostongns/EHClogo.jpg"
                },
                new CommunityVolunteer()
                {
                    Location = GetTranslationForCommunityVolunteer(dict,language,"A community project bringing people together"),
                    IsLogo = true,
                    ImageLocation = "/img/community/bostongns/GNS.png"
                },
                new CommunityVolunteer()
                {
                    Location = GetTranslationForCommunityVolunteer(dict,language,"Training and support provided by YMCA Lincolnshire"),
                    IsLogo = true,
                    ImageLocation = "/img/community/bostongns/ymcalincs.png"
                },
                new CommunityVolunteer()
                {
                    Location = GetTranslationForCommunityVolunteer(dict,language,"Proudly funded by The National Lottery Community Fund"),
                    IsLogo = true,
                    ImageLocation = "/img/community/bostongns/NationalLotteryLogo.png"
                }
            };

            var carouselPath = "/img/community/bostongns/carousel";
            communityViewModel.CarouselImages = new List<List<string>>
            {
                new List<string>
                {
                    $"{carouselPath}/Picture4.jpg",
                    $"{carouselPath}/Signs.jpeg",
                    $"{carouselPath}/Picture5.png",
                    $"{carouselPath}/stump1.jpeg",
                    $"{carouselPath}/Picture6.jpg",                    
                },
            };

            return communityViewModel;
        }

        private string GetTranslationForCommunityVolunteer(Dictionary<Tuple<string, string>, string> dict, string language, string key)
        {
            return dict.GetValueOrDefault(new Tuple<string, string>(language, key), key);
        }
    }
}
