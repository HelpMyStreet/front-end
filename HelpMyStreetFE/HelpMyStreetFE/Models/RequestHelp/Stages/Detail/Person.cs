using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Detail
{
    public class Person
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string MobileNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public string Email { get; set; }

        public string Postcode { get; set; }
    }
}
