using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace HelpMyStreetFE.Models.Reponses
{
    [DataContract(Name = "volunteerCoordinatesResponse")]
    public class VolunteerCoordinatesResponse
    {
        [DataMember(Name = "volunteerCoordinates")]
        [JsonPropertyName("volunteerCoordinates")]
        public IReadOnlyList<VolunteerCoordinate> VolunteerCoordinates { get; set; }
    }

    [DataContract(Name = "volunteerCoordinate")]
    public class VolunteerCoordinate
    {
        [DataMember(Name = "pc")]
        [JsonPropertyName("pc")]
        public string Postcode { get; set; }

        [DataMember(Name = "lat")]
        [JsonPropertyName("lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lng")]
        [JsonPropertyName("lng")]
        public double Longitude { get; set; }
    }
}
