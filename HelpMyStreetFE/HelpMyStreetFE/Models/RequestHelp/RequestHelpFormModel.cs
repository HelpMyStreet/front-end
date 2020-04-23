using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HelpMyStreet.Utils.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Models.RequestHelp
{
	public class RequestHelpFormModel
	{
		[Required]
		[BindProperty(Name = "firstname")]
		public string FirstName { get; set; }

		[BindProperty(Name = "lastname")]
		public string LastName { get; set; }

		[Required]
		[EmailAddress]
		[BindProperty(Name = "email")]
		public string Email { get; set; }

		[BindProperty(Name = "phonenumber")]
		public string PhoneNumber { get; set; }

		[BindProperty(Name = "message")]
		public string Message { get; set; }

		[Required]
		[BindProperty(Name = "help-needed-array")]
		public List<SupportActivities> HelpNeeded { get; set; }

		[BindProperty(Name = "on_behalf_of_another")]
		public bool OnBehalfOfAnother { get; set; }

		[BindProperty(Name = "health_concern")]
		public bool HealthConcern { get; set; }

		[BindProperty(Name = "postcode")]
		public string Postcode { get; set; }

		[BindProperty(Name = "requestId")]
		public int RequestId { get; set; }

		public bool HasErrors { get; set; }

	}
}
