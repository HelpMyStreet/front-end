using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Reponses
{
    public class GetUserResponse : BaseResponse
    {
        public User User { get; set; }
    }
}
