using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class RequestContactInformation
    {
        public int JobID { get; set; }
        public RequestPersonalDetails Requestor { get; set; }
        public RequestPersonalDetails Recipient { get; set; }
        public RequestorType RequestorType { get; set; }
        public void Deconstruct(out RequestPersonalDetails requestor, out RequestPersonalDetails recipient, out RequestorType requestorType)
        {
            requestor = Requestor;
            recipient = Recipient;
            requestorType = RequestorType;
        }
    }

}
