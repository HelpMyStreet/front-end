using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreetFE.Specs.Context
{
    public class UserContext
    {
        public UserContext()
        {
            Email = $"auto-testing+{Guid.NewGuid()}@helpmystreet.org";
        }

        public string Email { get; set; }
    }
}
