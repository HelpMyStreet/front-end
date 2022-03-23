using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreetFE.Models.Account;
using System.Collections.Generic;
using System.Linq;
using System;

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
            var postCode = Location?.LocationDetails?.Address?.Postcode ?? "";
            var distance = Location?.Distance ?? 0.0;

            return Item switch
            {
                IEnumerable<JobDetail> jds when (jds.First().SupportActivity == SupportActivities.Accommodation) => "",
                IEnumerable<JobDetail> _ => $"{postCode.Split(" ")[0]}, {distance:0.#} miles away",
                JobSummary js when js.JobStatus == JobStatuses.Open || js.JobStatus == JobStatuses.New => $"{postCode.Split(" ")[0]}, {distance.ToString("0.#")} miles away",
                JobSummary js when (js.JobStatus == JobStatuses.InProgress || js.JobStatus == JobStatuses.Accepted) && js.SupportActivity.PersonalDetailsComponent(RequestRoles.Recipient).Contains(PersonalDetailsComponent.Postcode) => $"{postCode}",
                JobSummary js when (js.JobStatus == JobStatuses.InProgress || js.JobStatus == JobStatuses.Accepted) && !js.SupportActivity.PersonalDetailsComponent(RequestRoles.Recipient).Contains(PersonalDetailsComponent.Postcode) => $"{postCode.Split(" ")[0]}",
                ShiftJob _ => $"{Location.LocationDetails.Name}",
                RequestSummary rs when rs.RequestType == RequestType.Shift => Location.LocationDetails.Name,
                RequestSummary rs when rs.JobBasics.First().SupportActivity == SupportActivities.Accommodation => "",
                RequestSummary _ => postCode,
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
