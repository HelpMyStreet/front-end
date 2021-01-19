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
            {"hlp", new CommunityModel(){FriendlyName = "Healthy London Partnership", Pin_Latitude = 51.507602, Pin_Longitude = -0.127816, LinkURL = "/healthylondonpartnership", Pin_VisibilityZoomLevel = 10, DisplayOnMap = false, BannerLocation = "/img/community/hlp/hlp-banner.png"} },
            {"tankersley", new CommunityModel(){FriendlyName = "Tankersley & Pilley Community Helpers", Pin_Latitude = 53.498113, Pin_Longitude = -1.488587, LinkURL = "/tankersley", Pin_VisibilityZoomLevel = 14, BannerLocation = "/img/community/tankersley/tankersley-st-peters-church.jpeg" } },
            {"ruddington", new CommunityModel(){FriendlyName = "Ruddington Community Response Team", Pin_Latitude = 52.8925, Pin_Longitude = -1.150, LinkURL = "/ruddington", Pin_VisibilityZoomLevel = 14.6, BannerLocation = "/img/community/ruddington/banner.jpg", GeographicName = "Ruddington" } },
            {"ageuklsl", new CommunityModel() {FriendlyName = "Age UK Lincoln & South Lincolnshire", Pin_Latitude = 53.2304334, Pin_Longitude = -0.5435425, LinkURL = "/ageuklsl", Pin_VisibilityZoomLevel = 9, DisplayOnMap = true, BannerLocation = "/img/community/ageUK/lsl/age-uk-lincoln-cathedral-banner.png"} },
            {"ageukwirral", new CommunityModel() {FriendlyName = "Age UK Wirral", Pin_Latitude = 53.37, Pin_Longitude = -3.05, LinkURL = "/ageukwirral", Pin_VisibilityZoomLevel = 9, DisplayOnMap = true, BannerLocation = "/img/community/ageUK/wirral/age-uk-wirral-banner-narrow.png"} },
            {"balderton", new CommunityModel() {FriendlyName = "Balderton Community Support", Pin_Latitude = 53.0561082, Pin_Longitude = -0.8, LinkURL = "/balderton", Pin_VisibilityZoomLevel = 12, DisplayOnMap = true, BannerLocation = "/img/community/ageUK/notts/balderton/banner-narrow.jpg", GeographicName="Balderton" } },
            {"north-muskham", new CommunityModel() {FriendlyName = "North Muskham Community Support", Pin_Latitude = 53.120254, Pin_Longitude = -0.811079, LinkURL = "/north-muskham", Pin_VisibilityZoomLevel = 12, DisplayOnMap = true, BannerLocation = "/img/community/ageUK/notts/north-muskham/north-muskham-banner.png", GeographicName="North Muskham" } },
            {"ageuk-southkentcoast", new CommunityModel() {FriendlyName = "Age UK South Kent Coast", Pin_Latitude = 51.15670694376801, Pin_Longitude = 1.2906096124741184, LinkURL = "/southkentcoast", Pin_VisibilityZoomLevel = 12, DisplayOnMap = true, BannerLocation = "/img/community/ageUK/kent/southkentcoast/banner.jpg", GeographicName="Deal or Folkestone" } },
            {"ageuk-favershamandsittingbourne", new CommunityModel() {FriendlyName = "Age UK Faversham & Sittingbourne", Pin_Latitude = 51.32681418199929, Pin_Longitude = 0.8065864663737088, LinkURL = "/favershamandsittingbourne", Pin_VisibilityZoomLevel = 12, DisplayOnMap = true, BannerLocation = "/img/community/ageUK/kent/favershamandsittingbourne/banner.jpg", GeographicName="Faversham or Sittingbourne" } },
            {"ageuknwkent", new CommunityModel() {FriendlyName = "Age UK North West Kent", Pin_Latitude = 51.40020276537333, Pin_Longitude = 0.2950217005371014, LinkURL = "/northwestkent", Pin_VisibilityZoomLevel = 11, DisplayOnMap = true, BannerLocation = "/img/community/ageUK/kent/northwest/banner.jpg", GeographicName="North West Kent (Dartford, Swanley or Gravesend)" } },
            {"lincolnshirevolunteers", new CommunityModel() {FriendlyName = "Lincolnshire Volunteers", Pin_Latitude = 53.2304334, Pin_Longitude = -0.5435425, Pin_VisibilityZoomLevel = 9, DisplayOnMap = false, BannerLocation = "/imb/community/vacc/lincolnshire/banner.png", LinkURL = "/lincolnshirevolunteers"} },
            {"ftlos", new CommunityModel{FriendlyName="For the Love of Scrubs", DisplayOnMap = false } },

        };

        public CommunityRepository(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<CommunityViewModel> GetCommunity(string groupKey, CancellationToken cancellationToken)
        {
            if (groupKey == "lincolnshirevolunteers")
            {
                CommunityViewModel vm = GetLincolnshireVolunteers();
                vm.groupKey = groupKey;
                vm.EncodedGroupId = "test";
                return vm;
            }
            else
            {
                int groupId = await _groupService.GetGroupIdByKey(groupKey, cancellationToken);

                CommunityViewModel vm = ((Groups)groupId) switch
                {
                    Groups.Tankersley => GetTankersley(),
                    Groups.Ruddington => GetRuddington(),
                    Groups.AgeUKLSL => GetAgeUKLSL(),
                    Groups.AgeUKWirral => GetAgeUKWirral(),
                    Groups.HLP => GetHLP(),
                    Groups.FTLOS => GetFtLOS(),
                    Groups.AgeUKNottsBalderton => GetBalderton(),
                    Groups.AgeUKNottsNorthMuskham => GetNorthMuskham(),
                    Groups.AgeUKSouthKentCoast => GetSouthKentCoast(),
                    Groups.AgeUKFavershamAndSittingbourne => GetFavershameAndSittingBourne(),
                    Groups.AgeUKNorthWestKent => GetNorthWestKent(),
                    _ => null,
                };

                vm.EncodedGroupId = Base64Utils.Base64Encode(groupId);
                vm.groupKey = groupKey;

                return vm;
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

        public CommunityModel GetCommunityDetailByKey(string key)
        {
            return Communities[key];
        }

        private CommunityViewModel GetBalderton()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "Balderton" };
            CommunityModel communityModel = GetCommunityDetailByKey("balderton");

            communityViewModel.Map_CentreLatitude = communityModel.Pin_Latitude;
            communityViewModel.Map_CentreLongitude = communityModel.Pin_Longitude;
            communityViewModel.Map_ZoomLevel = 13.5;

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


        private CommunityViewModel GetNorthMuskham()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel
            {
                View = "NorthMuskham",
            };

            CommunityModel communityModel = GetCommunityDetailByKey("north-muskham");

            communityViewModel.Map_CentreLatitude = communityModel.Pin_Latitude;
            communityViewModel.Map_CentreLongitude = communityModel.Pin_Longitude;
            communityViewModel.Map_ZoomLevel = 14;

            communityViewModel.CommunityName = communityModel.FriendlyName;
            communityViewModel.ShowRequestHelpPopup = true;

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
            CommunityViewModel communityViewModel = new CommunityViewModel
            {
                View = "SouthKentCoast",
            };

            CommunityModel communityModel = GetCommunityDetailByKey("ageuk-southkentcoast");

            communityViewModel.Map_CentreLatitude = communityModel.Pin_Latitude;
            communityViewModel.Map_CentreLongitude = communityModel.Pin_Longitude;
            communityViewModel.Map_ZoomLevel = 11;

            communityViewModel.CommunityName = "Age UK South Kent Coast";
            communityViewModel.ShowRequestHelpPopup = true;

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

        private CommunityViewModel GetLincolnshireVolunteers()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel
            {
                View = "LincolnshireVolunteers",
            };

            CommunityModel communityModel = GetCommunityDetailByKey("lincolnshirevolunteers");

            communityViewModel.Map_CentreLatitude = communityModel.Pin_Latitude;
            communityViewModel.Map_CentreLongitude = communityModel.Pin_Longitude;
            communityViewModel.Map_ZoomLevel = 11;

            communityViewModel.CommunityName = "Lincolnshire Volunteers";
            communityViewModel.ShowRequestHelpPopup = true;


            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Lincolnshire CVS",
                    Role = "",
                    Location = "",
                    ImageLocation = "/img/community/vacc/linconshirevolunteers/LCVS-Master-New.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Voluntary Centre Services",
                    Role = "",
                    Location = "",
                    ImageLocation = "/img/community/vacc/linconshirevolunteers/cropped-vcs-logo-new-site.png"
                },
                new CommunityVolunteer()
                {
                    Name = "VET",
                    Role = "",
                    Location = "",
                    ImageLocation = "/img/community/vacc/linconshirevolunteers/cropped-VET_Logo.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Age UK",
                    Role = "",
                    Location = "",
                    ImageLocation = "/img/community/vacc/linconshirevolunteers/age-uk-lincoln--south-lincolnshire-logo-rgb.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Local Resilience Forum",
                    Role = "",
                    Location = "",
                    ImageLocation = "/img/community/vacc/linconshirevolunteers/lrf-logo.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "NHS Lincolnshire",
                    Role = "",
                    Location = "",
                    ImageLocation = "/img/community/vacc/linconshirevolunteers/nhs-lincolnshire.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Lincolnshire County Council",
                    Role = "",
                    Location = "",
                    ImageLocation = "/img/community/vacc/linconshirevolunteers/Lincolnshire-LOGO.png"
                },
            };

            var carouselPath = "";
            communityViewModel.CarouselImages = new List<List<string>>
            {
                new List<string>
                {
                }
            };

            return communityViewModel;
        }

        private CommunityViewModel GetFavershameAndSittingBourne()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel
            {
                View = "FavershamAndSittingBourne",
            };

            CommunityModel communityModel = GetCommunityDetailByKey("ageuk-favershamandsittingbourne");

            communityViewModel.Map_CentreLatitude = communityModel.Pin_Latitude;
            communityViewModel.Map_CentreLongitude = communityModel.Pin_Longitude;
            communityViewModel.Map_ZoomLevel = 12;

            communityViewModel.CommunityName = "Age UK Faversham and Sittingbourne";
            communityViewModel.ShowRequestHelpPopup = true;

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
            CommunityViewModel communityViewModel = new CommunityViewModel
            {
                View = "NorthWestKent",
            };

            CommunityModel communityModel = GetCommunityDetailByKey("ageuknwkent");

            communityViewModel.Map_CentreLatitude = communityModel.Pin_Latitude;
            communityViewModel.Map_CentreLongitude = communityModel.Pin_Longitude;
            communityViewModel.Map_ZoomLevel = 12;

            communityViewModel.CommunityName = "Age UK North West Kent";
            communityViewModel.ShowRequestHelpPopup = true;

            var carouselPath = "/img/community/ageUK/kent/northwest/carousel";
            communityViewModel.CarouselImages = new List<List<string>>();

            return communityViewModel;
        }

        private CommunityViewModel GetHLP()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "HLP" };
            CommunityModel communityModel = GetCommunityDetailByKey("hlp");

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

        private CommunityViewModel GetTankersley()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "Tankersley" };
            CommunityModel communityModel = GetCommunityDetailByKey("tankersley");

            communityViewModel.Map_CentreLatitude = communityModel.Pin_Latitude;
            communityViewModel.Map_CentreLongitude = communityModel.Pin_Longitude;
            communityViewModel.Map_ZoomLevel = communityModel.Pin_VisibilityZoomLevel;


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

        private CommunityViewModel GetRuddington()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "Ruddington" };
            CommunityModel communityModel = GetCommunityDetailByKey("ruddington");

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

        private CommunityViewModel GetAgeUKLSL()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "AgeUKLSL" };
            CommunityModel communityModel = GetCommunityDetailByKey("ageuklsl");

            communityViewModel.Map_CentreLatitude = 52.95;
            communityViewModel.Map_CentreLongitude = -0.2;
            communityViewModel.Map_ZoomLevel = communityModel.Pin_VisibilityZoomLevel;

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

        private CommunityViewModel GetAgeUKWirral()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "AgeUKWirral" };

            communityViewModel.Map_CentreLatitude = 53.37;
            communityViewModel.Map_CentreLongitude = -3.05;
            communityViewModel.Map_ZoomLevel = 11.15;

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

        private CommunityViewModel GetFtLOS()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel { View = "ForTheLoveOfScrubs" };

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
