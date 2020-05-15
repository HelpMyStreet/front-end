using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HelpMyStreet.Utils.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Models.RequestHelp
{
	public class RequestHelpViewModel
	{
		public HelpRequestViewModel HelpRequest { get; set; }
		public JobRequestViewModel JobRequest { get; set; }
	}
}
