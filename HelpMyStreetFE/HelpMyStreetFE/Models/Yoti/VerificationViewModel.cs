using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Yoti
{
    public class VerificationViewModel
    {

        public YotiOptions YotiOptions { get; set; }
        public string EncodedUserID { get; set; }

        public string DisplayName { get; set; }

        public bool IsVerified { get; set; }
    }
}
