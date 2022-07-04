using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobDetail : JobSummary, IContainsLocation
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
            this.SpecificSupportActivity = jobSummary.SpecificSupportActivity;
        }

        public JobDetail(JobBasic jobBasic)
        {
            this.Archive = jobBasic.Archive;
            this.DateRequested = jobBasic.DateRequested;
            this.DateStatusLastChanged = jobBasic.DateStatusLastChanged;
            this.DistanceInMiles = jobBasic.DistanceInMiles;
            this.DueDate = jobBasic.DueDate;
            this.DueDateType = jobBasic.DueDateType;
            this.JobID = jobBasic.JobID;
            this.JobStatus = jobBasic.JobStatus;
            this.NotBeforeDate = jobBasic.NotBeforeDate;
            this.ReferringGroupID = jobBasic.ReferringGroupID;
            this.RequestID = jobBasic.RequestID;
            this.RequestType = jobBasic.RequestType;
            this.SupportActivity = jobBasic.SupportActivity;
            this.SuppressRecipientPersonalDetail = jobBasic.SuppressRecipientPersonalDetail;
            this.VolunteerUserID = jobBasic.VolunteerUserID;
            this.SpecificSupportActivity = jobBasic.SpecificSupportActivity;
        }

        public RequestSummary RequestSummary { get; set; }
        public RequestPersonalDetails Requestor { get; set; }
        public RequestPersonalDetails Recipient { get; set; }
        public List<EnrichedStatusHistory> JobStatusHistory { get; set; }
        public User CurrentVolunteer { get; set; }
        public bool RequiresApplicationToAccept
        {
            get
            {
                var requiresApplicationToAcceptQuestion =  this.Questions.Where(q => q.Id == (int)HelpMyStreet.Utils.Enums.Questions.RequiresApplicationToAccept).FirstOrDefault();
                if(requiresApplicationToAcceptQuestion==null)
                {
                    return false;
                }
                else
                {
                    return requiresApplicationToAcceptQuestion.Answer == "Yes";
                }
            }
        }
    }
}
