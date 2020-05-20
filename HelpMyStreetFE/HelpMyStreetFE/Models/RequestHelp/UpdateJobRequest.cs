using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp
{
    public class UpdateJobRequest
    {
        [Required]
        public string JobID { get; set; }
    }
}
