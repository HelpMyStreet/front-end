using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Home
{
    public class ForgottonPasswordViewModel
    {
        public string FirebaseConfiguration { get; set; }
    }

    public class ResetPasswordViewModel {
        public string  ActionCode { get; set;}
        public string FirebaseConfiguration { get; set; }
    }
}
