using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreetFE.Specs.Context
{
    public class UserContext
    {
        public UserContext()
        {
            var dateComponent = DateTime.Now.ToString("yyMMddHHmm");
            var uniqueComponent =  Guid.NewGuid().ToString().Split('-')[0];

            Email = $"auto-testing+{dateComponent}+{uniqueComponent}@helpmystreet.org";
        }

        public string Email { get; set; }
    }
}
