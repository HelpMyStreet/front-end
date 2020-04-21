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
		[MaxLength(50)]
		[BindProperty(Name = "firstname")]
		public string FirstName { get; set; }

		[Required]
		[MaxLength(50)]
		[BindProperty(Name = "lastname")]
		public string LastName { get; set; }

		[Required]
		[EmailAddress]
		[BindProperty(Name = "email")]
		public string Email { get; set; }

		[BindProperty(Name = "phonenumber")]
		public string PhoneNumber { get; set; }

		[MaxLength(500)]
		[BindProperty(Name = "message")]
		public string Message { get; set; }

		[BindProperty(Name = "help-needed-array")]
		public List<SupportActivities> HelpNeeded { get; set; }

		[BindProperty(Name = "help_for_me")]
		public bool HelpForMe { get; set; }

		[BindProperty(Name = "health_concern")]
		public bool HealthConcern { get; set; }

		[BindProperty(Name = "address_line_1")]
		public string AddressLine1 { get; set; }

		[BindProperty(Name = "address_line_2")]
		public string AddressLine2 { get; set; }

		[BindProperty(Name = "city")]
		public string City { get; set; }

		[BindProperty(Name = "county")]
		public string County { get; set; }

		[BindProperty(Name = "postcode")]
		public string Postcode { get; set; }

		[BindProperty(Name = "requestId")]
		public int RequestId { get; set; }

		public bool HasErrors { get; set; }

	}
}
