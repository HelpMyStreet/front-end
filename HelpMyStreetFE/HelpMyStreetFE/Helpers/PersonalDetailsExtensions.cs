using HelpMyStreet.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Helpers
{
    public static class PersonalDetailsExtensions
    {
        public static IEnumerable<string> PhoneNumbers(this UserPersonalDetails userPersonalDetails)
        {
            return (new[] { userPersonalDetails.MobilePhone, userPersonalDetails.OtherPhone }).Where(a => !string.IsNullOrEmpty(a));
        }

        public static IEnumerable<string> PhoneNumbers(this RequestPersonalDetails requestPersonalDetails)
        {
            return (new[] { requestPersonalDetails.MobileNumber, requestPersonalDetails.OtherNumber }).Where(a => !string.IsNullOrEmpty(a));
        }
    }
}
