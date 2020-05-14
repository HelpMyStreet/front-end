using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp
{
	public class JobRequestViewModel
	{
		public SupportActivities SupportActivity { get; set; }
		public string Details { get; set; }
		public int DueDays { get; set; }
		public bool HealthCritical { get; set; }
	}

}
