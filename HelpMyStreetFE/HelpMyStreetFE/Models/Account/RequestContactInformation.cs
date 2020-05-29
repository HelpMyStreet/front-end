using HelpMyStreet.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Account
{
    public class RequestContactInformation
    {
        public int JobID { get; set; }
        public RequestPersonalDetails Requestor { get; set; }
        public RequestPersonalDetails Recipient { get; set; }
        public bool ForRequestor { get; set; }
        public void Deconstruct(out RequestPersonalDetails requestor, out RequestPersonalDetails recipient, out bool forRequestor)
        {
            requestor = Requestor;
            recipient = Recipient;
            forRequestor = ForRequestor;
        }
    }

}
