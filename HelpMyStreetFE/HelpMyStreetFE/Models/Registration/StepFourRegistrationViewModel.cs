using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Registration
{
    public class PostCodeDetail
    {
        public string Postcode { get; set; }
        public string StreetName { get; set; }
        public int DistanceInMetres { get; set; }
        public string FriendlyName { get; set; }
    }
}
