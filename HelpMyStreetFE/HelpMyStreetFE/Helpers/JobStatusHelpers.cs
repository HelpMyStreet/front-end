using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE
{
    public static class JobStatusHelpers
    {
        public static string FriendlyName(this JobStatuses status)
        {
            return status switch
            {
                JobStatuses.InProgress => "In Progress",
                _ => status.ToString()
            };
        }
    }
}
