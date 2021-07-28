using System;
using HelpMyStreet.Contracts.AddressService.Response;
namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class ViewLocationViewModel
    {
        public double? Distance { get; set; }
        public string PostCode { get; set; }
        public bool IsAllowed { get; set; }
        public string encodedJobID { get; set; }
        public PostcodeCoordinate Coordinates { get; set; }
    }
}
