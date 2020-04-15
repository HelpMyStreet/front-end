using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HelpMyStreet.Utils.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Models.RequestHelp
{
    public class RequestHelpFormModel
    {
        [Required]
        [MaxLength(50)]
        [BindProperty(Name = "firstname")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        [BindProperty(Name = "lastname")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [BindProperty(Name = "email")]
        public string Email { get; set; }

        [BindProperty(Name = "phonenumber")]
        public string PhoneNumber { get; set; }

        [MaxLength(500)]
        [BindProperty(Name = "message")]
        public string Message { get; set; }

        [BindProperty(Name = "helpNeeded[]")]
        public List<SupportActivities> HelpNeeded { get; set; }

        [BindProperty(Name = "help_for_me")]
        public bool HelpForMe { get; set; }

        [BindProperty(Name = "health_concern")]
        public bool HealthConcern { get; set; }

    }
}
