using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreetFE.Models.Account;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Linq;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobViewModel<T>
    {
        public T Item { get; set; }
        public User User { get; set; }
        public bool UserHasRequiredCredentials { get; set; }
        public RequestRoles UserRole { get; set; }
        public bool HighlightJob { get; set; }
        public LocationWithDistance Location { get; set; }
        public int? JobListGroupId { get; set; }
        public string ListLocalityDescription { get => GetLocalityInformation(); }
        public bool CanViewAddressPopup { get => GetPopupVisibility(); }

        private string GetLocalityInformation()
        {
            return Item switch
            {
                IEnumerable<JobDetail> jss => $"{jss.First().PostCode.Split(" ")[0]}, {Math.Round(jss.First().DistanceInMiles, 1)} miles away",
                JobSummary js when js.JobStatus == JobStatuses.Open || js.JobStatus == JobStatuses.New => $"{js.PostCode.Split(" ")[0]}, {Math.Round(js.DistanceInMiles, 1)} miles away",
                JobSummary js when (js.JobStatus == JobStatuses.InProgress || js.JobStatus == JobStatuses.Accepted) && js.SupportActivity.PersonalDetailsComponent(RequestRoles.Recipient).Contains(PersonalDetailsComponent.Postcode) => $"{js.PostCode}",
                JobSummary js when (js.JobStatus == JobStatuses.InProgress || js.JobStatus == JobStatuses.Accepted) && !js.SupportActivity.PersonalDetailsComponent(RequestRoles.Recipient).Contains(PersonalDetailsComponent.Postcode) => $"{js.PostCode.Split(" ")[0]}",
                ShiftJob sj => $"{Location.LocationDetails.Name}",
                RequestSummary rs => rs.PostCode,
                _ => "",
            };
        }

        private bool GetPopupVisibility()
        {
            return Item switch
            {
                IEnumerable<JobDetail> jss when jss.First().SupportActivity.PersonalDetailsComponent(RequestRoles.Recipient).Contains(PersonalDetailsComponent.Postcode) => true,
                JobSummary js when js.SupportActivity.PersonalDetailsComponent(RequestRoles.Recipient).Contains(PersonalDetailsComponent.Postcode) => true,
                ShiftJob sj => true,
                RequestSummary rs when rs.JobBasics.Select(jb => jb.SupportActivity.PersonalDetailsComponent(RequestRoles.Recipient).Contains(PersonalDetailsComponent.Postcode)).Any(pdc => pdc) => true,
                _ => false,
            };
        }
    }
    
}
