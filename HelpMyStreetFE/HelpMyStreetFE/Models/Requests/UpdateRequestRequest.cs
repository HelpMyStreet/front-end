namespace HelpMyStreetFE.Models.Requests
{
	public class UpdateRequestRequest
	{
		public int requestId;
		public bool onBehalfOfAnother;
		public bool healthOrWellbeingConcern;

		public SupportActivityRequest supportActivitiesRequired;
		public string furtherDetails;
		public string requestorFirstName;
		public string requestorLastName;
		public string requestorEmailAddress;
		public string requestorPhoneNumber;
	}

	public class SupportActivityRequest
	{
		public string[] supportActivities;
	}
}
