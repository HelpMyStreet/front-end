using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Validation
{
    public class ValidationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public static ValidationResponse Failed(string message) => new ValidationResponse { Success = false, Message = message };
    }
}
