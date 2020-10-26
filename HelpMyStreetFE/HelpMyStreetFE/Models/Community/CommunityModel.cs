using System;
namespace HelpMyStreetFE.Models.Community
{
    public class CommunityModel
    {
        public string ReferenceName { get; set; }
        public string FriendlyName { get; set; }
        public string LinkURL { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool DisplayOnMap { get; set; } = true;
        public string BannerLocation { get; set; }
        public double ZoomLevel { get; set; }
    }
}
