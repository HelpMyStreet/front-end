using HelpMyStreet.Utils.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Registration
{
    public class StepThreeFormModel
    {
        [BindProperty(Name = "volunteer[]")]
        public List<SupportActivities> VolunteerOptions { get; set; }
        [BindProperty(Name = "volunteer_distance")]
        public float VolunteerDistance { get; set; }
        [BindProperty(Name = "volunteer_phone_contact")]
        public bool VolunteerPhoneContact { get; set; }
        [BindProperty(Name = "volunteer_medical_condition")]
        public bool VolunteerMedicalCondition { get; set; }
    }
}
