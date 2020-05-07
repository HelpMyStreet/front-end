using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Services;

namespace HelpMyStreetFE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapsController : ControllerBase
    {

        private readonly IGoogleService _googleService;
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;

        public MapsController(IGoogleService googleService, IUserService userService, IAddressService addressService)
        {
            _googleService = googleService;
            _userService = userService;
            _addressService = addressService;
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

    }
}
