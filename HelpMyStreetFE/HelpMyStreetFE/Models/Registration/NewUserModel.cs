namespace HelpMyStreetFE.Models.Registration
{
    public class NewUserModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string ReferringGroupId { get; set; }
        public string Source { get; set; }
    }
}
