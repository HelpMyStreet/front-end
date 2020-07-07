using HelpMyStreetFE.Models.RequestHelp.Enum;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Registration
{
    public class StepFourFormModel
    {
        [BindProperty(Name = "street_champion")]
        public bool ChampionRoleUnderstood { get; set; }
        [BindProperty(Name = "postcodes[]")]
        public List<string> ChampionPostcodes { get; set; } = new List<string>();
    }
}
