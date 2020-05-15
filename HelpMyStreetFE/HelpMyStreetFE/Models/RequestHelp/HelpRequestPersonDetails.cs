using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp
{
	public class HelpRequestPersonDetails
	{
		public string Firstname { get; set; }
		public string Lastname { get; set; }
		public string Email { get; set; }
		public string Mobile { get; set; }
		public string AltNumber { get; set; }
		public HelpRequestAddress Address { get; set; }
	}
}
