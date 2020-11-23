using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Models.ContactForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models
{
    public class SiteHeaderViewModel
    {        
        public bool isLoggedIn { get; set; }
        public bool loginPage { get; set; }
        public AccountViewModel AccountVM { get; set; }
    }
}
