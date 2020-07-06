namespace HelpMyStreetFE.Models.Registration
{
    public class NewUserModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public int ReferringGroupId { get; set; }
    }
}
