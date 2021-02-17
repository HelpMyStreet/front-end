using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.RequestHelp.Enum;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HelpMyStreetFE.Models.Registration
{
    public class StepTwoFormModel
    {
        [BindProperty(Name = "first_name")]
        public string FirstName { get; set; }

        [BindProperty(Name = "last_name")]
        public string LastName { get; set; }

        [BindProperty(Name = "address_line_1")]
        public string AddressLine1 { get; set; }

        [BindProperty(Name = "address_line_2")]
        public string AddressLine2 { get; set; }

        [BindProperty(Name = "city")]
        public string City { get; set; }

        [BindProperty(Name = "county")]
        public string County { get; set; }

        [BindProperty(Name = "postcode")]
        public string Postcode { get; set; }

        [BindProperty(Name = "mobile_number")]
        public string MobilePhone { get; set; }

        [BindProperty(Name = "alt_number")]
        public string OtherPhone { get; set; }

        [BindProperty(Name = "dob")]
        public string DateOfBirth { get; set; }
    }
}
