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
        [BindProperty(Name = "custom_distance")]            
        public float CustomDistance { get; set; }

        public bool HasCustomDistance
        {
            get
            {
                return VolunteerDistance == 999 && CustomDistance > 0;
            }
        }
    }
}
