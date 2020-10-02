using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace HelpMyStreetFE.Models.Reponses
{
    [DataContract(Name = "getCommunitiesResponse")]
    public class GetCommunitiesResponse
    {
        [DataMember(Name = "communityDetails")]
        [JsonPropertyName("communityDetails")]
        public IReadOnlyList<CommunityDetail> CommunityDetails { get; set; }
    }

    [DataContract(Name = "communityDetail")]
    public class CommunityDetail
    {
        [DataMember(Name = "refName")]
        [JsonPropertyName("refName")]
        public string ReferenceName { get; set; }

        [DataMember(Name = "friendlyName")]
        [JsonPropertyName("friendlyName")]
        public string FriendlyName { get; set; }

        [DataMember(Name = "linkURL")]
        [JsonPropertyName("linkURL")]
        public string LinkURL { get; set; }

        [DataMember(Name = "latitude")]
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [DataMember(Name ="displayOnMap")]
        [JsonPropertyName("displayOnMap")]
        public bool DisplayOnMap { get; set; }

        [DataMember(Name ="bannerLocation")]
        [JsonPropertyName("bannerLocation")]
        public string BannerLocation { get; set; }
    }
}