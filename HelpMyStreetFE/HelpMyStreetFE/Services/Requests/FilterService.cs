using System;
using System.Collections.Generic;
using System.Linq;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Services.Users;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services.Requests
{
    public class FilterService : IFilterService
    {
        private readonly IOptions<RequestSettings> _requestSettings;
        private IRequestService _requestService;
        private IAddressService _addressService;
        private IUserService _userService;

        public FilterService(IOptions<RequestSettings> requestSettings, IRequestService requestService, IAddressService addressService, IUserService userService)
        {
            _requestService = requestService;
            _requestSettings = requestSettings;
            _addressService = addressService;
            _userService = userService;
        }

        public async Task<SortAndFilterSet> GetDefaultSortAndFilterSet(JobSet jobSet, JobStatuses? jobStatus, User user)
        {
            return jobSet switch
            {
                JobSet.GroupRequests => GetGroupRequestsDefaultSortAndFilterSet(jobStatus),
                JobSet.UserOpenRequests_MatchingCriteria => GetOpenRequestsMatchingCriteriaDefaultSortAndFilterSet(),
                JobSet.UserOpenRequests_NotMatchingCriteria => GetOpenRequestsNotMatchingCriteriaDefaultSortAndFilterSet(user),
                JobSet.UserAcceptedRequests => GetAcceptedRequestsDefaultSortAndFilterSet(),
                JobSet.UserCompletedRequests => GetCompletedRequestsDefaultSortAndFilterSet(),

                JobSet.GroupShifts => await GetGroupShiftsFilterSet(user, jobStatus),
                JobSet.UserOpenShifts => await GetShiftsFilterSet(user),
                JobSet.UserMyShifts => await GetShiftsFilterSet(user),
                _ => throw new ArgumentException(message: $"Unexpected JobFilterRequest.JobSet value: {jobSet}", paramName: nameof(jobSet))
            };
        }

        private async Task<SortAndFilterSet> GetShiftsFilterSet(User user)
        {
            var locations = await _userService.GetLocations(user.ID, new System.Threading.CancellationToken());
            var locationDetails = await _addressService.GetLocationDetails(locations);
            SortAndFilterSet filterSet = new SortAndFilterSet
            {
                Locations = locationDetails.Select(ld => new FilterField<Location>() { Value = ld.Location, IsSelected = true, Label = ld.ShortName }),
                PartOfDay = new List<FilterField<PartOfDay>>()
                {
                    new FilterField<PartOfDay>() {Value = PartOfDay.Morning, Label = "Morning Shifts", IsSelected = true},
                    new FilterField<PartOfDay>() {Value = PartOfDay.Afternoon, Label = "Afternoon Shifts", IsSelected = true},
                    new FilterField<PartOfDay>() {Value = PartOfDay.Night, Label = "Night Shifts", IsSelected = true}
                },
                OrderBy = new List<OrderByField>
                {
                    new OrderByField() {Value = OrderBy.DateDue_Ascending, Label = "Soonest"},
                    new OrderByField() {Value = OrderBy.DateDue_Descending, Label = "Furthest Away"},
                    
                },
                DueInNextXDays = new List<FilterField<int>>
                    {
                        new FilterField<int> { Value = 1, Label = "Today" },
                        new FilterField<int> { Value = 7, Label = "This week" },
                        new FilterField<int> { Value = 14, Label = "Next 2 weeks" },
                        new FilterField<int> { Value = 999, Label = "Show all", IsSelected = true }
                    },

            };


            return filterSet;
        }

        private async Task<SortAndFilterSet> GetGroupShiftsFilterSet(User user, JobStatuses? jobStatus)
        {
            var filterSet = await GetShiftsFilterSet(user);

            filterSet.OrderBy.Append(new OrderByField() { Value = OrderBy.Emptiest, Label = "Most unfilled shifts" });

            filterSet.JobStatuses = new List<FilterField<JobStatuses>>
                    {
                        new FilterField<JobStatuses>() { Value = JobStatuses.New },
                        new FilterField<JobStatuses>() { Value = JobStatuses.Open },
                        new FilterField<JobStatuses>() { Value = JobStatuses.InProgress },
                        new FilterField<JobStatuses>() { Value = JobStatuses.Done },
                        new FilterField<JobStatuses>() { Value = JobStatuses.Cancelled },
                    };



            if (jobStatus != null)
            {
                filterSet.JobStatuses.Where(js => js.Value == jobStatus).First().IsSelected = true;
            }
            else
            {
                filterSet.JobStatuses.Where(js => js.Value == JobStatuses.New).First().IsSelected = true;
                filterSet.JobStatuses.Where(js => js.Value == JobStatuses.Open).First().IsSelected = true;
                filterSet.JobStatuses.Where(js => js.Value == JobStatuses.InProgress).First().IsSelected = true;
            }

            if (jobStatus == JobStatuses.Cancelled || jobStatus == JobStatuses.Done)
            {
                filterSet.OrderBy.Where(ob => ob.Value == OrderBy.DateStatusLastChanged_Descending).First().IsSelected = true;
            }
            else
            {
                filterSet.OrderBy.Where(ob => ob.Value == OrderBy.RequiringAdminAttention).First().IsSelected = true;
            }

            return filterSet;
        }

        private SortAndFilterSet GetGroupRequestsDefaultSortAndFilterSet(JobStatuses? jobStatus)
        {
            SortAndFilterSet filterSet = new SortAndFilterSet
            {

                JobStatuses = new List<FilterField<JobStatuses>>
                    {
                        new FilterField<JobStatuses>() { Value = JobStatuses.New },
                        new FilterField<JobStatuses>() { Value = JobStatuses.Open },
                        new FilterField<JobStatuses>() { Value = JobStatuses.InProgress },
                        new FilterField<JobStatuses>() { Value = JobStatuses.Done },
                        new FilterField<JobStatuses>() { Value = JobStatuses.Cancelled },
                    },
                OrderBy = new List<OrderByField>
                    {
                        new OrderByField() { Value = OrderBy.RequiringAdminAttention, Label = "Requiring attention" },
                        new OrderByField() { Value = OrderBy.DateDue_Ascending, Label = "Help needed soonest" },
                        new OrderByField() { Value = OrderBy.DateDue_Descending, Label = "Help needed least soon" },
                        new OrderByField() { Value = OrderBy.DateRequested_Descending, Label = "Requested last" },
                        new OrderByField() { Value = OrderBy.DateRequested_Ascending, Label = "Requested first" },
                        new OrderByField() { Value = OrderBy.DateStatusLastChanged_Descending, Label = "Updated most recently" },
                        new OrderByField() { Value = OrderBy.DateStatusLastChanged_Ascending, Label = "Updated least recently" },
                    },
            };

            if (jobStatus != null)
            {
                filterSet.JobStatuses.Where(js => js.Value == jobStatus).First().IsSelected = true;
            }
            else
            {
                filterSet.JobStatuses.Where(js => js.Value == JobStatuses.New).First().IsSelected = true;
                filterSet.JobStatuses.Where(js => js.Value == JobStatuses.Open).First().IsSelected = true;
                filterSet.JobStatuses.Where(js => js.Value == JobStatuses.InProgress).First().IsSelected = true;
            }

            if (jobStatus == JobStatuses.Cancelled || jobStatus == JobStatuses.Done)
            {
                filterSet.OrderBy.Where(ob => ob.Value == OrderBy.DateStatusLastChanged_Descending).First().IsSelected = true;
            }
            else
            {
                filterSet.OrderBy.Where(ob => ob.Value == OrderBy.RequiringAdminAttention).First().IsSelected = true;
            }

            return filterSet;
        }

        private SortAndFilterSet GetOpenRequestsMatchingCriteriaDefaultSortAndFilterSet()
        {
            SortAndFilterSet filterSet = new SortAndFilterSet
            {
                OrderBy = new List<OrderByField>
                    {
                        new OrderByField() { Value = OrderBy.DateDue_Ascending, Label = "Help needed soonest", IsSelected = true },
                        new OrderByField() { Value = OrderBy.Distance_Ascending, Label = "Closest to my address" },
                    },
            };

            return filterSet;
        }

        private SortAndFilterSet GetOpenRequestsNotMatchingCriteriaDefaultSortAndFilterSet(User user)
        {
            SortAndFilterSet filterSet = new SortAndFilterSet
            {
                SupportActivities = new List<FilterField<SupportActivities>>
                    {
                        new FilterField<SupportActivities>() { Value = SupportActivities.Shopping, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.FaceMask, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.WellbeingPackage, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.CheckingIn, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.CollectingPrescriptions, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.Errands, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.MealPreparation, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.PhoneCalls_Friendly, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.HomeworkSupport, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.DogWalking, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.ColdWeatherArmy, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.Transport, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.MealsToYourDoor, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.MealtimeCompanion, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.Other, IsSelected = true },

                        // The following are not currently on any request help form
                        //new FilterField<SupportActivities>() { Value = SupportActivities.MedicalAppointmentTransport, IsSelected = true },
                        //new FilterField<SupportActivities>() { Value = SupportActivities.PhoneCalls_Anxious, IsSelected = true },
                    },
                DueInNextXDays = new List<FilterField<int>>
                    {
                        new FilterField<int> { Value = 1, Label = "Today" },
                        new FilterField<int> { Value = 7, Label = "This week" },
                        new FilterField<int> { Value = 14, Label = "Next 2 weeks" },
                        new FilterField<int> { Value = 999, Label = "Show all", IsSelected = true }
                    },
                OrderBy = new List<OrderByField>
                    {
                        new OrderByField() { Value = OrderBy.DateDue_Ascending, Label = "Help needed soonest", IsSelected = true },
                        new OrderByField() { Value = OrderBy.Distance_Ascending, Label = "Closest to my address" },
                    },
            };

            if (user.SupportActivities.Contains(SupportActivities.CommunityConnector))
            {
                ((List<FilterField<SupportActivities>>)filterSet.SupportActivities)
                    .Insert(0, new FilterField<SupportActivities>() { Value = SupportActivities.CommunityConnector, IsSelected = true });
            }

            if (user.SupportActivities.Intersect(_requestSettings.Value.NationalSupportActivities).Count() > 0)
            {
                filterSet.MaxDistanceInMiles = new List<FilterField<int>>
                    {
                        new FilterField<int> { Value = 0, Label = "My street only" },
                        new FilterField<int> { Value = 1, Label = "Within 1 mile" },
                        new FilterField<int> { Value = 5, Label = "Within 5 miles" },
                        new FilterField<int> { Value = 10, Label = "Within 10 miles" },
                        new FilterField<int> { Value = 20, Label = "Within 20 miles" },
                        new FilterField<int> { Value = 999, Label = "Show all", IsSelected = true },
                    };
            }
            else
            {
                filterSet.MaxDistanceInMiles = new List<FilterField<int>>
                    {
                        new FilterField<int> { Value = 0, Label = "My street only" },
                        new FilterField<int> { Value = 1, Label = "Within 1 mile" },
                        new FilterField<int> { Value = 5, Label = "Within 5 miles" },
                        new FilterField<int> { Value = 10, Label = "Within 10 miles" },
                        new FilterField<int> { Value = 20, Label = "Within 20 miles", IsSelected = true },
                    };
            }

            return filterSet;
        }

        private SortAndFilterSet GetAcceptedRequestsDefaultSortAndFilterSet()
        {
            SortAndFilterSet filterSet = new SortAndFilterSet
            {
                OrderBy = new List<OrderByField>
                    {
                        new OrderByField() { Value = OrderBy.DateDue_Ascending, Label = "Help needed soonest", IsSelected = true },
                        new OrderByField() { Value = OrderBy.DateStatusLastChanged_Ascending, Label = "Accepted first" },
                        new OrderByField() { Value = OrderBy.DateStatusLastChanged_Descending, Label = "Accepted last" },
                    },
            };

            return filterSet;
        }

        private SortAndFilterSet GetCompletedRequestsDefaultSortAndFilterSet()
        {
            SortAndFilterSet filterSet = new SortAndFilterSet
            {
                OrderBy = new List<OrderByField>
                    {
                        new OrderByField() { Value = OrderBy.DateStatusLastChanged_Descending, Label = "Completed last", IsSelected = true },
                        new OrderByField() { Value = OrderBy.DateStatusLastChanged_Ascending, Label = "Completed first" },
                    },
            };

            return filterSet;
        }

        public IEnumerable<ShiftJob> SortAndFilterJobs(IEnumerable<ShiftJob> jobs, JobFilterRequest jfr)
        {


            var jobsToDisplay = jobs.Where(
                j => (jfr.JobStatuses == null || jfr.JobStatuses.Contains(j.JobStatus))
                    && (jfr.SupportActivities == null || jfr.SupportActivities.Contains(j.SupportActivity))
                    && (jfr.Locations == null || jfr.Locations.Contains(j.Location))
                    && (jfr.DueInNextXDays == null || j.StartDate <= DateTime.Now.Date.AddDays(jfr.DueInNextXDays.Value))
                    && (jfr.PartsOfDay == null || jfr.PartsOfDay.Where(pod => pod.CheckStartTimeWithin(j.StartDate)).Count() > 0)
                    );

            return jfr.OrderBy switch
            {
                 OrderBy.DateDue_Ascending =>
                    jobsToDisplay.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenBy(j => j.StartDate),
                OrderBy.DateDue_Descending =>
                    jobsToDisplay.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenByDescending(j => j.StartDate)
                _ => throw new ArgumentException(message: $"Unexpected OrderByField value: {jfr.OrderBy}", paramName: nameof(jfr.OrderBy)),
            };
        }

        public IEnumerable<RequestSummary> SortAndFilterJobs(IEnumerable<RequestSummary> jobs, JobFilterRequest jfr)
        {
            var jobsToDisplay = jobs.Where(
                j => (jfr.SupportActivities == null || j.JobSummaries.Where(js => jfr.SupportActivities.Contains(js.SupportActivity)).Count() > 0)
                    && (jfr.JobStatuses == null || j.JobSummaries.Where(js => jfr.JobStatuses.Contains(js.JobStatus)).Count() > 0)
                    && (jfr.Locations == null || j.JobSummaries.Where(js => { var sj = (ShiftJob)js; return jfr.Locations.Contains(sj.Location); }).Count() > 0)
                    && (jfr.DueInNextXDays == null || j.Shift.StartDate <= DateTime.Now.Date.AddDays(jfr.DueInNextXDays.Value))
                    && (jfr.PartsOfDay == null || jfr.PartsOfDay.Where(pod => pod.CheckStartTimeWithin(j.Shift.StartDate)).Count() > 0 )
                    );

            return jfr.OrderBy switch
            {
                OrderBy.DateDue_Ascending =>
                   jobsToDisplay.OrderBy(j => j.Shift.StartDate),
                OrderBy.DateDue_Descending =>
                    jobsToDisplay.OrderByDescending(j => j.Shift.StartDate),
                OrderBy.Emptiest =>
                    jobsToDisplay.OrderBy(j => j.JobSummaries.Where(js => js.JobStatus == JobStatuses.Done).Count()).ThenBy(j => j.Shift.StartDate),
                _ => throw new ArgumentException(message: $"Unexpected OrderByField value: {jfr.OrderBy}", paramName: nameof(jfr.OrderBy)),
            };
        }

        public IEnumerable<JobHeader> SortAndFilterJobs(IEnumerable<JobHeader> jobs, JobFilterRequest jfr)
        {
            var jobsToDisplay = jobs.Where(
                j => (jfr.JobStatuses == null || jfr.JobStatuses.Contains(j.JobStatus))
                    && (jfr.SupportActivities == null || jfr.SupportActivities.Contains(j.SupportActivity))
                    && (jfr.MaxDistanceInMiles == null || j.DistanceInMiles <= jfr.MaxDistanceInMiles)
                    && (jfr.DueInNextXDays == null || j.DueDate.Date <= DateTime.Now.Date.AddDays(jfr.DueInNextXDays.Value))
                    && (jfr.DueAfter == null || j.DueDate.Date >= jfr.DueAfter?.Date)
                    && (jfr.DueBefore == null || j.DueDate.Date <= jfr.DueBefore?.Date)
                    && (jfr.RequestedAfter == null || j.DateRequested.Date >= jfr.RequestedAfter?.Date)
                    && (jfr.RequestedBefore == null) || j.DateRequested.Date <= jfr.RequestedBefore?.Date);

            return jfr.OrderBy switch
            {
                OrderBy.RequiringAdminAttention =>
                    jobsToDisplay.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenByDescending(j => j.RequiringAdminAttentionScore()).ThenBy(j => j.DueDate),
                OrderBy.DateDue_Ascending =>
                    jobsToDisplay.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenBy(j => j.DueDate),
                OrderBy.DateDue_Descending =>
                    jobsToDisplay.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenByDescending(j => j.DueDate),
                OrderBy.DateRequested_Ascending =>
                    jobsToDisplay.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenBy(j => j.DateRequested),
                OrderBy.DateRequested_Descending =>
                    jobsToDisplay.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenByDescending(j => j.DateRequested),
                OrderBy.DateStatusLastChanged_Ascending =>
                    jobsToDisplay.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenBy(j => j.DateStatusLastChanged),
                OrderBy.DateStatusLastChanged_Descending =>
                    jobsToDisplay.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenByDescending(j => j.DateStatusLastChanged),
                OrderBy.Distance_Ascending =>
                    jobsToDisplay.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenBy(j => j.DistanceInMiles),
                _ => throw new ArgumentException(message: $"Unexpected OrderByField value: {jfr.OrderBy}", paramName: nameof(jfr.OrderBy)),
            };
        }
    }
}
