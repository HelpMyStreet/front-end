using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Validation
{
    public class ValidationRequest
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}
