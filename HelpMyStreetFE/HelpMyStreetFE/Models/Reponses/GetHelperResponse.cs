using HelpMyStreet.Utils.Models;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Reponses
{
    public class GetHelperResponse : BaseResponse
    {
        public List<User> Users { get; set; }
    }
}
