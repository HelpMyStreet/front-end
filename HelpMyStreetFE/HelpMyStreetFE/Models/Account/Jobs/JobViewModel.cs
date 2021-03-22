using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using System.Collections.Generic;
using System;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobViewModel<T>
    {
        public T Item { get; set; }
        public bool UserHasRequiredCredentials { get; set; }
        public RequestRoles UserRole { get; set; }
        public bool HighlightJob { get; set; }
        public LocationWithDistance Location { get; set; }
        public int? JobListGroupId { get; set; }
        public string ListLocalityDescription { get => GetLocalityInformation(); }

        private string GetLocalityInformation()
        {
            JobStatuses jstatus = new JobStatuses();
            string postCode = "";
            string distance = "";

            var personalDetailsComponents = new List<PersonalDetailsComponent>();

            switch (Item)
            {
                case JobSummary js:
                    jstatus = js.JobStatus;
                    postCode = js.PostCode;
                    distance = $"{Math.Round(js.DistanceInMiles, 1)}";
                    personalDetailsComponents = js.SupportActivity.PersonalDetailsComponent(RequestRoles.Recipient);
                    break;
                case ShiftJob sj:
                    jstatus = sj.JobStatus;
                    postCode = Location.LocationDetails.Name;
                    distance = $"{Math.Round(Location.Distance, 1)}";
                    break;
                case RequestSummary rs:
                    postCode = rs.PostCode;
                    distance = "";
                    break;
            }

            if (UserRole == RequestRoles.GroupAdmin)
            {
                return $"{postCode}";
            }
            else if (jstatus == JobStatuses.Open || jstatus == JobStatuses.New)
            {
                return $"{postCode.Split(" ")[0]}, {distance} miles away";
            }
            else if (jstatus == JobStatuses.Accepted || jstatus == JobStatuses.InProgress)
            {
                if (personalDetailsComponents.Contains(PersonalDetailsComponent.Postcode))
                {
                    return $"{postCode}";
                }
                else {
                    return $"{postCode.Split(" ")[0]}";
                }
            }

            return "";
        }
    }
    
}
