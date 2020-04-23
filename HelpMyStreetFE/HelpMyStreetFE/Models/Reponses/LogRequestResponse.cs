using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Reponses
{
    public class LogRequestResponse
    {
        public bool HasContent { get; set; }
        public bool IsSuccessful { get; set; }
        public HelpMyStreet.Contracts.RequestService.Response.LogRequestResponse Content { get; set; }
    }
}
