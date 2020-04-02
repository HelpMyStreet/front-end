using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Account
{
    public class UserDetails //I expect this class to be depricated when we know how to differentiate users
    {
        public UserDetails(string initials)
        {
            Initials = initials;
        }

        public string Initials { get; set; }
    }
}
