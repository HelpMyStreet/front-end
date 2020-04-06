using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Reponses
{
    public class BaseResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool Ok { get; set; }
        public string Error { get; set; }
    }
}
