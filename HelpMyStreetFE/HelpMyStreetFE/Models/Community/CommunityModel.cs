using System;
namespace HelpMyStreetFE.Models.Community
{
    public class CommunityModel
    {
        public string FriendlyName { get; set; }
        public string GeographicName { get; set; }
        public string LinkURL { get; set; }
        public double Pin_Latitude { get; set; }
        public double Pin_Longitude { get; set; }
        public bool DisplayOnMap { get; set; } = true;
        public string BannerLocation { get; set; }
        public double Pin_VisibilityZoomLevel { get; set; }
    }
}
