using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp
{

	public class HelpRequestViewModel
	{
		public bool ForRequestor { get; set; }
		public bool ReadPrivacyNotice { get; set; }
		public bool AcceptedTerms { get; set; }
		public HelpRequestPersonDetails Requestor { get; set; }
		public HelpRequestPersonDetails Recipient { get; set; }
		public bool ConsentForContact { get; set; }
		public string SpecialCommunicationNeeds { get; set; }
		public string OtherDetails { get; set; }

	}
}
