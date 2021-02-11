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
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading;
using HelpMyStreetFE.Services.Groups;

namespace HelpMyStreetFE.Services.Requests
{
    public class FilterService : IFilterService
    {
        private readonly IOptions<RequestSettings> _requestSettings;
        private IAddressService _addressService;
        private IGroupService _groupService;

        public FilterService(IOptions<RequestSettings> requestSettings, IAddressService addressService, IGroupService groupService)
        {
            _requestSettings = requestSettings;
            _addressService = addressService;
            _groupService = groupService;
        }

        public async Task<SortAndFilterSet> GetDefaultSortAndFilterSet(JobSet jobSet, int? groupId, JobStatuses? jobStatus, User user, CancellationToken cancellationToken)
        {
            return jobSet switch
            {
                JobSet.GroupRequests => GetGroupRequestsDefaultSortAndFilterSet(jobStatus),
                JobSet.UserOpenRequests_MatchingCriteria => GetOpenRequestsMatchingCriteriaDefaultSortAndFilterSet(),
                JobSet.UserOpenRequests_NotMatchingCriteria => GetOpenRequestsNotMatchingCriteriaDefaultSortAndFilterSet(user),
                JobSet.UserMyRequests => GetMyRequestsDefaultSortAndFilterSet(),

                JobSet.GroupShifts => await GetGroupShiftsFilterSet(groupId.Value, user, jobStatus, cancellationToken),
                JobSet.UserOpenShifts => await GetShiftsFilterSet(user, jobSet, cancellationToken),
                JobSet.UserMyShifts => await GetShiftsFilterSet(user, jobSet, cancellationToken),
                _ => throw new ArgumentException(message: $"Unexpected JobFilterRequest.JobSet value: {jobSet}", paramName: nameof(jobSet))
            };
        }

        private async Task<SortAndFilterSet> GetShiftsFilterSet(User user, JobSet jobSet, CancellationToken cancellationToken)
        {
            SortAndFilterSet filterSet = new SortAndFilterSet
            {
                PartOfDay = new List<FilterField<PartOfDay>>()
                {
                    new FilterField<PartOfDay>() {Value = PartOfDay.Morning, Label = "Morning shifts", IsSelected = true},
                    new FilterField<PartOfDay>() {Value = PartOfDay.Afternoon, Label = "Afternoon shifts", IsSelected = true},
                    new FilterField<PartOfDay>() {Value = PartOfDay.Night, Label = "Night shifts", IsSelected = true}
                },
                OrderBy = new List<OrderByField>
                {
                    new OrderByField() {Value = OrderBy.DateDue_Ascending, Label = "Soonest", IsSelected = true},
                    new OrderByField() {Value = OrderBy.DateDue_Descending, Label = "Least soon"},
                },
                DueInNextXDays = new List<FilterField<int>>
                {
                    new FilterField<int> { Value = 1, Label = "Today" },
                    new FilterField<int> { Value = 7, Label = "This week" },
                    new FilterField<int> { Value = 14, Label = "Next 2 weeks" },
                    new FilterField<int> { Value = 999, Label = "Show all", IsSelected = true }
                },
            };

            var userLocations = await _addressService.GetLocationDetailsForUser(user, cancellationToken);
            filterSet.Locations = userLocations.Select(l => new FilterField<Location>
            {
                Value = l.Location,
                IsSelected = true,
                Label = $"{l.LocationDetails.ShortName} ({l.Distance:0.0} miles)"
            });

            if (jobSet == JobSet.UserMyShifts)
            {
                filterSet.JobStatuses = new List<FilterField<JobStatuses>>
                {
                    new FilterField<JobStatuses>() { Value = JobStatuses.Accepted, IsSelected = true },
                    new FilterField<JobStatuses>() { Value = JobStatuses.InProgress, IsSelected = true },
                    new FilterField<JobStatuses>() { Value = JobStatuses.Done },
                };
            }
            else if (jobSet == JobSet.UserOpenShifts)
            {
                filterSet.OrderBy = filterSet.OrderBy.Append(new OrderByField { Value = OrderBy.DateRequested_Descending, Label = "Most recently added" });
            }

            return filterSet;
        }

        private async Task<SortAndFilterSet> GetGroupShiftsFilterSet(int groupId, User user, JobStatuses? jobStatus, CancellationToken cancellationToken)
        {
            SortAndFilterSet filterSet = new SortAndFilterSet
            {
                PartOfDay = new List<FilterField<PartOfDay>>()
                {
                    new FilterField<PartOfDay>() {Value = PartOfDay.Morning, Label = "Morning shifts", IsSelected = true},
                    new FilterField<PartOfDay>() {Value = PartOfDay.Afternoon, Label = "Afternoon shifts", IsSelected = true},
                    new FilterField<PartOfDay>() {Value = PartOfDay.Night, Label = "Night shifts", IsSelected = true}
                },
                OrderBy = new List<OrderByField>
                {
                    new OrderByField { Value = OrderBy.DateDue_Ascending, Label = "Soonest", IsSelected = true },
                    new OrderByField { Value = OrderBy.DateDue_Descending, Label = "Least soon" },
                    new OrderByField { Value = OrderBy.Emptiest, Label = "Most unfilled slots" },
                    new OrderByField { Value = OrderBy.DateRequested_Descending, Label = "Most recently added" }
                },
                DueInNextXDays = new List<FilterField<int>>
                {
                    new FilterField<int> { Value = 1, Label = "Today" },
                    new FilterField<int> { Value = 7, Label = "This week" },
                    new FilterField<int> { Value = 14, Label = "Next 2 weeks" },
                    new FilterField<int> { Value = 999, Label = "Show all", IsSelected = true }
                },
                JobStatuses = new List<FilterField<JobStatuses>>
                {
                    new FilterField<JobStatuses>() { Value = JobStatuses.New },
                    new FilterField<JobStatuses>() { Value = JobStatuses.Open },
                    new FilterField<JobStatuses>() { Value = JobStatuses.Accepted },
                    new FilterField<JobStatuses>() { Value = JobStatuses.InProgress },
                    new FilterField<JobStatuses>() { Value = JobStatuses.Done },
                    new FilterField<JobStatuses>() { Value = JobStatuses.Cancelled },
                },
            };

            var locations = await _groupService.GetGroupLocations(groupId, true);

            if (locations.Count() > 0)
            {
                var locationDetails = await _addressService.GetLocationDetails(locations, cancellationToken);
                filterSet.Locations = locationDetails.OrderBy(ld => ld.ShortName).Select(ld => new FilterField<Location>()
                {
                    Value = ld.Location,
                    IsSelected = true,
                    Label = ld.ShortName
                });
            }

            if (jobStatus != null)
            {
                filterSet.JobStatuses.Where(js => js.Value == jobStatus).First().IsSelected = true;
            }
            else
            {
                filterSet.JobStatuses.Where(js => js.Value == JobStatuses.New).First().IsSelected = true;
                filterSet.JobStatuses.Where(js => js.Value == JobStatuses.Open).First().IsSelected = true;
                filterSet.JobStatuses.Where(js => js.Value == JobStatuses.Accepted).First().IsSelected = true;
                filterSet.JobStatuses.Where(js => js.Value == JobStatuses.InProgress).First().IsSelected = true;
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

        private SortAndFilterSet GetMyRequestsDefaultSortAndFilterSet()
        {
            SortAndFilterSet filterSet = new SortAndFilterSet
            {
                JobStatuses = new List<FilterField<JobStatuses>>
                {
                    new FilterField<JobStatuses>() { Value = JobStatuses.InProgress, IsSelected = true },
                    new FilterField<JobStatuses>() { Value = JobStatuses.Done },
                },
                OrderBy = new List<OrderByField>
                {
                    new OrderByField() { Value = OrderBy.DateDue_Ascending, Label = "Help needed soonest", IsSelected = true },
                    new OrderByField() { Value = OrderBy.DateStatusLastChanged_Descending, Label = "Updated most recently" },
                    new OrderByField() { Value = OrderBy.DateStatusLastChanged_Ascending, Label = "Updated least recently" },
                },
            };

            return filterSet;
        }

        public IEnumerable<JobBasic> SortAndFilterJobs(IEnumerable<JobBasic> jobs, JobFilterRequest jfr)
        {
            if (jfr.JobStatuses != null) { jobs = jobs.Where(j => jfr.JobStatuses.Contains(j.JobStatus)); }
            if (jfr.SupportActivities != null) { jobs = jobs.Where(j => jfr.SupportActivities.Contains(j.SupportActivity)); }

            if (jobs.GetType() == typeof(IEnumerable<ShiftJob>))
            {
                if (jfr.Locations != null && jfr.Locations.Count() > 0) { jobs = jobs.Where(j => jfr.Locations.Contains(((ShiftJob)j).Location)); }
                if (jfr.DueInNextXDays != null) { jobs = jobs.Where(j => ((ShiftJob)j).StartDate <= DateTime.Now.Date.AddDays(jfr.DueInNextXDays.Value)); }
                if (jfr.PartsOfDay != null) { jobs = jobs.Where(j => jfr.PartsOfDay.Where(pod => pod.CheckStartTimeWithin(((ShiftJob)j).StartDate)).Count() > 0); }

                return jfr.OrderBy switch
                {
                    OrderBy.DateDue_Ascending =>
                       jobs.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenBy(j => ((ShiftJob)j).StartDate),
                    OrderBy.DateDue_Descending =>
                        jobs.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenByDescending(j => ((ShiftJob)j).StartDate),
                    OrderBy.DateRequested_Descending =>
                      jobs.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenByDescending(j => ((ShiftJob)j).DateRequested),
                    _ => throw new ArgumentException(message: $"Unexpected OrderByField value: {jfr.OrderBy}", paramName: nameof(jfr.OrderBy)),
                };
            }

            if (jobs.GetType() == typeof(IEnumerable<JobHeader>))
            {
                if (jfr.MaxDistanceInMiles != null) { jobs = jobs.Where(j => ((JobHeader)j).DistanceInMiles <= jfr.MaxDistanceInMiles); }
                if (jfr.DueInNextXDays != null) { jobs = jobs.Where(j => ((JobHeader)j).DueDate.Date <= DateTime.Now.Date.AddDays(jfr.DueInNextXDays.Value)); }
                if (jfr.DueAfter != null) { jobs = jobs.Where(j => ((JobHeader)j).DueDate.Date >= jfr.DueAfter?.Date); }
                if (jfr.DueBefore != null) { jobs = jobs.Where(j => ((JobHeader)j).DueDate.Date <= jfr.DueBefore?.Date); }
                if (jfr.RequestedAfter != null) { jobs = jobs.Where(j => ((JobHeader)j).DateRequested.Date >= jfr.RequestedAfter?.Date); }
                if (jfr.RequestedBefore != null) { jobs = jobs.Where(j => ((JobHeader)j).DateRequested.Date <= jfr.RequestedBefore?.Date); }

                return jfr.OrderBy switch
                {
                    OrderBy.RequiringAdminAttention =>
                        jobs.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenByDescending(j => ((JobHeader)j).RequiringAdminAttentionScore()).ThenBy(j => ((JobHeader)j).DueDate),
                    OrderBy.DateDue_Ascending =>
                        jobs.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenBy(j => ((JobHeader)j).DueDate),
                    OrderBy.DateDue_Descending =>
                        jobs.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenByDescending(j => ((JobHeader)j).DueDate),
                    OrderBy.DateRequested_Ascending =>
                        jobs.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenBy(j => ((JobHeader)j).DateRequested),
                    OrderBy.DateRequested_Descending =>
                        jobs.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenByDescending(j => ((JobHeader)j).DateRequested),
                    OrderBy.DateStatusLastChanged_Ascending =>
                        jobs.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenBy(j => ((JobHeader)j).DateStatusLastChanged),
                    OrderBy.DateStatusLastChanged_Descending =>
                        jobs.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenByDescending(j => ((JobHeader)j).DateStatusLastChanged),
                    OrderBy.Distance_Ascending =>
                        jobs.OrderByDescending(j => j.JobID.Equals(jfr.HighlightJobId)).ThenBy(j => ((JobHeader)j).DistanceInMiles),
                    _ => throw new ArgumentException(message: $"Unexpected OrderByField value: {jfr.OrderBy}", paramName: nameof(jfr.OrderBy)),
                };
            }

            return jobs;
        }

        public IEnumerable<RequestSummary> SortAndFilterJobs(IEnumerable<RequestSummary> jobs, JobFilterRequest jfr)
        {
            var jobsToDisplay = jobs.Where(
                j => (jfr.SupportActivities == null || j.JobSummaries.Where(js => jfr.SupportActivities.Contains(js.SupportActivity)).Count() > 0)
                    && (jfr.JobStatuses == null || j.JobSummaries.Where(js => jfr.JobStatuses.Contains(js.JobStatus)).Count() > 0)
                    && (jfr.Locations == null || jfr.Locations.Count() == 0 || jfr.Locations.Contains(j.Shift.Location))
                    && (jfr.DueInNextXDays == null || j.Shift.StartDate <= DateTime.Now.Date.AddDays(jfr.DueInNextXDays.Value))
                    && (jfr.PartsOfDay == null || jfr.PartsOfDay.Where(pod => pod.CheckStartTimeWithin(j.Shift.StartDate)).Count() > 0)
                    );

            return jfr.OrderBy switch
            {
                OrderBy.DateDue_Ascending =>
                   jobsToDisplay.OrderBy(j => j.Shift.StartDate),
                OrderBy.DateDue_Descending =>
                    jobsToDisplay.OrderByDescending(j => j.Shift.StartDate),
                OrderBy.Emptiest =>
                    jobsToDisplay.OrderByDescending(j => j.JobSummaries.Where(js => js.JobStatus == JobStatuses.Open).Count()).ThenBy(j => j.Shift.StartDate),
                OrderBy.DateRequested_Descending =>
                    jobsToDisplay.OrderByDescending(j => j.DateRequested),
                _ => throw new ArgumentException(message: $"Unexpected OrderByField value: {jfr.OrderBy}", paramName: nameof(jfr.OrderBy)),
            };
        }
    }
}
