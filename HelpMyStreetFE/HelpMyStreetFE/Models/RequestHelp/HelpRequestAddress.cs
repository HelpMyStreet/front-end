using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp
{
	public class HelpRequestAddress
	{
		public string Addressline1 { get; set; }
		public string Addressline2 { get; set; }
		public string Locality { get; set; }
		public string Postcode { get; set; }
	}
}
