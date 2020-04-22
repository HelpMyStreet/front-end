using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Reponses
{
    public class LogRequestResponse : BaseResponse
    {
        public HelpMyStreet.Contracts.RequestService.Response.LogRequestResponse Content { get; set; }
    }
}
