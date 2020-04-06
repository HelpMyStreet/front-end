using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Registration
{
    public class PostCodeDetail
    {
        public string Postcode { get; set; }
        public string StreetName { get; set; }
        public int ChampionCount { get; set; }
    }

    public class StepFourRegistrationViewModel : RegistrationViewModel
    {
        public List<PostCodeDetail> NearbyPostCodes { get; set; }
        public PostCodeDetail UsersPostCode { get; set; }
        public bool LocalAvailability { get; set; }
    }
}
