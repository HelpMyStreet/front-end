﻿using HelpMyStreetFE.Models.ContactForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models
{
    public class LoginViewModel
    {        
        public string FirebaseConfiguration { get; set; }
        public string SignUpURL { get; set; } = "/registration/step-one";
        public string Email { get; set; }
        public string LoginError { get; set; }
        public string EmailError { get; set; }
    }
}
