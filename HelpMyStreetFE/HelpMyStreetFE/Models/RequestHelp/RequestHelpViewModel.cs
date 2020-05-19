using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Validation;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Models.RequestHelp
{
	[ValidateRequestViewModel]
	public class RequestHelpViewModel
	{
		public HelpRequestViewModel HelpRequest { get; set; }
		public JobRequestViewModel JobRequest { get; set; }
	}
}
