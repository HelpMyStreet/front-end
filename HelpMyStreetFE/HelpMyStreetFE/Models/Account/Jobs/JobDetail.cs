using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobDetail : JobSummary
    {
        public JobDetail(JobSummary jobSummary)
        {
            this.Archive = jobSummary.Archive;
            this.ConsentForContact = jobSummary.ConsentForContact;
            this.DateRequested = jobSummary.DateRequested;
            this.DateStatusLastChanged = jobSummary.DateStatusLastChanged;
            this.Details = jobSummary.Details;
            this.DistanceInMiles = jobSummary.DistanceInMiles;
            this.DueDate = jobSummary.DueDate;
            this.DueDateType = jobSummary.DueDateType;
            this.DueDays = jobSummary.DueDays;
            this.Groups = jobSummary.Groups;
            this.IsHealthCritical = jobSummary.IsHealthCritical;
            this.JobID = jobSummary.JobID;
            this.JobStatus = jobSummary.JobStatus;
            this.NotBeforeDate = jobSummary.NotBeforeDate;
            this.PostCode = jobSummary.PostCode;
            this.Questions = jobSummary.Questions;
            this.RecipientOrganisation = jobSummary.RecipientOrganisation;
            this.Reference = jobSummary.Reference;
            this.ReferringGroupID = jobSummary.ReferringGroupID;
            this.RequestID = jobSummary.RequestID;
            this.RequestorDefinedByGroup = jobSummary.RequestorDefinedByGroup;
            this.RequestorType = jobSummary.RequestorType;
            this.RequestType = jobSummary.RequestType;
            this.SupportActivity = jobSummary.SupportActivity;
            this.SuppressRecipientPersonalDetail = jobSummary.SuppressRecipientPersonalDetail;
            this.VolunteerUserID = jobSummary.VolunteerUserID;
        }

        public RequestSummary RequestSummary { get; set; }
        public RequestPersonalDetails Requestor { get; set; }
        public RequestPersonalDetails Recipient { get; set; }
        public List<EnrichedStatusHistory> JobStatusHistory { get; set; }
        public User CurrentVolunteer { get; set; }
    }
}
