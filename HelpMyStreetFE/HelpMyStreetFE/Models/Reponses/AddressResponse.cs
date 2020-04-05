using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HelpMyStreetFE.Models.Reponses
{
    public class AddressResponse
    {
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressLine3 { get; set; }
        public string locality { get; set; }
        public string postcode { get; set; }
    }
}
