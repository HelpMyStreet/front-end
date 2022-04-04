using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp.Enum;
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
    }
}
