using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HelpMyStreetFE.Models.ContactForm
{
    public class ContactFormViewModel
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string MobileNumber { get; set; }
        public string OtherNumber { get; set; }

        public string Organisation { get; set; }
        public string Role { get; set; }

        [Required]
        [MaxLength(1024)]
        public string Message { get; set; }
    }
}
