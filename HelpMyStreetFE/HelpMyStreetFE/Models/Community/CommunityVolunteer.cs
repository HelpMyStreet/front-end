namespace HelpMyStreetFE.Models.Community
{
    public class CommunityVolunteer
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string Location { get; set; }
        public bool IsLogo { get; set; } = false;
        public string ImageLocation { get; set; }
        public string LogoHyperLink { get; set; }
    }
}
