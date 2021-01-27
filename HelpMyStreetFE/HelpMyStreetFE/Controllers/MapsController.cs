using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using System.Collections.Generic;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.Community;
using HelpMyStreetFE.Services;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services.Users;


namespace HelpMyStreetFE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapsController : ControllerBase
    {

        private readonly IGoogleService _googleService;
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;
        private readonly ICommunityRepository _communityRepository;

        public MapsController(IGoogleService googleService, IUserService userService, IAddressService addressService, ICommunityRepository communityRepository)
        {
            _googleService = googleService;
            _userService = userService;
            _addressService = addressService;
            _communityRepository = communityRepository;
        }

        [HttpGet("js")]
        public async Task<ActionResult<string>> GetGoogleMapsJs()
        {
            string script = await _googleService.GetCachedGoogleMapScript();
            return await Task.FromResult(script);
        }

        [HttpGet("volunteerCoordinates")]
        public async Task<ActionResult<VolunteerCoordinatesResponse>> GetVolunteerCoordinates([FromQuery]double swLatitude, [FromQuery]double swLongitude, [FromQuery]double neLatitude, [FromQuery]double neLongitude, [FromQuery]int minDistanceBetweenInMetres)
        {
            VolunteerCoordinatesResponse volunteerCoordinatesResponse = await _userService.GetVolunteerCoordinates(swLatitude, swLongitude, neLatitude, neLongitude, minDistanceBetweenInMetres);
            return volunteerCoordinatesResponse;
        }

        [HttpGet("postcodeCoordinate")]
        public async Task<ActionResult<ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode>>> GetVolunteerCoordinates([FromQuery]string postcode)
        {
            ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode> volunteerCoordinatesResponse = await _addressService.GetPostcodeCoordinate(postcode);
            return volunteerCoordinatesResponse;
        }

        [HttpGet("getCommunities")]
        public async Task<ActionResult<GetCommunitiesResponse>> GetCommunities()
        {
            var communityData = await _communityRepository.GetCommunities();
            var communityDetails = new List<CommunityDetail>();
            foreach (CommunityModel community in communityData)
            {
                var communityDetail = new CommunityDetail()
                {
                    FriendlyName = community.FriendlyName,
                    Latitude = community.Pin_Latitude,
                    Longitude = community.Pin_Longitude,
                    LinkURL = community.LinkURL,
                    BannerLocation = community.BannerLocation,
                    DisplayOnMap = community.DisplayOnMap,
                    ZoomLevel = community.Pin_VisibilityZoomLevel,
                    GroupType = community.GroupType,
                };
                communityDetails.Add(communityDetail);
            }
            GetCommunitiesResponse gcr = new GetCommunitiesResponse() { CommunityDetails = communityDetails};
            return gcr;
        }

    }
}
