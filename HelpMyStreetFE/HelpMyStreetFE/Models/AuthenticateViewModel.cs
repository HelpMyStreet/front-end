using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models
{
    public class AuthenticateViewModel
    {
        public string Token { get; set; }
        public string EncodedUserID { get; set; }
        public bool Mobile { get; set; }
    }
}
