using System;
using System.Collections.Generic;
using System.Linq;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Account.Jobs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Users;

namespace HelpMyStreetFE.Services.Requests
{
    public class FilterService : IFilterService
    {
        private IAddressService _addressService;
        private IUserLocationService _userLocationService;
        

        public FilterService(IAddressService addressService, IUserLocationService userLocationService)
        {
            _addressService = addressService;
            _userLocationService = userLocationService;
        }

        public async Task<SortAndFilterSet> GetDefaultSortAndFilterSet(JobSet jobSet, int? groupId, List<JobStatuses> jobStatuses, User user, CancellationToken cancellationToken)
        {
            return jobSet switch
            {
                JobSet.GroupRequests => GetGroupRequestsDefaultSortAndFilterSet(jobStatuses),
                JobSet.UserOpenRequests_MatchingCriteria => GetOpenRequestsMatchingCriteriaDefaultSortAndFilterSet(),
                JobSet.UserOpenRequests_NotMatchingCriteria => GetOpenRequestsNotMatchingCriteriaDefaultSortAndFilterSet(user),
                JobSet.UserMyRequests => GetMyRequestsDefaultSortAndFilterSet(),

                JobSet.GroupShifts => await GetGroupShiftsFilterSet(groupId.Value, user, jobStatuses, cancellationToken),
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

            var userLocations = await _userLocationService.GetLocationDetailsForUser(user, cancellationToken);
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

        private async Task<SortAndFilterSet> GetGroupShiftsFilterSet(int groupId, User user, List<JobStatuses> jobStatuses, CancellationToken cancellationToken)
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

            var locationDetails = await _addressService.GetLocationDetailsForGroup(groupId, true, cancellationToken);

            if (locationDetails.Count() > 0)
            {
                filterSet.Locations = locationDetails.OrderBy(ld => ld.ShortName).Select(ld => new FilterField<Location>()
                {
                    Value = ld.Location,
                    IsSelected = true,
                    Label = ld.ShortName
                });
            }

            if (jobStatuses != null && jobStatuses.Count > 0)
            {
                filterSet.JobStatuses.Where(js => jobStatuses.Contains(js.Value)).ForEach(s => s.IsSelected = true);
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

        private SortAndFilterSet GetGroupRequestsDefaultSortAndFilterSet(List<JobStatuses> jobStatuses)
        {
            SortAndFilterSet filterSet = new SortAndFilterSet
            {

                JobStatuses = new List<FilterField<JobStatuses>>
                    {
                        new FilterField<JobStatuses>() { Value = JobStatuses.New },
                        new FilterField<JobStatuses>() { Value = JobStatuses.Open },
                        new FilterField<JobStatuses>() { Value = JobStatuses.AppliedFor},
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

            if (jobStatuses != null && jobStatuses.Count > 0)
            {
                filterSet.JobStatuses.Where(js => jobStatuses.Contains(js.Value)).ForEach(s => s.IsSelected = true);
            }
            else
            {
                filterSet.JobStatuses.Where(js => js.Value == JobStatuses.New).First().IsSelected = true;
                filterSet.JobStatuses.Where(js => js.Value == JobStatuses.Open).First().IsSelected = true;
                filterSet.JobStatuses.Where(js => js.Value == JobStatuses.InProgress).First().IsSelected = true;
                filterSet.JobStatuses.Where(js => js.Value == JobStatuses.AppliedFor).First().IsSelected = true;
            }

            if (jobStatuses.Count() > 0 && jobStatuses.All(s => s.Complete()))
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
                        new FilterField<SupportActivities>() { Value = SupportActivities.EmergencySupport, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.VolunteerSupport, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.Covid19Help, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.BinDayAssistance, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.DigitalSupport, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.InPersonBefriending, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.PracticalSupport, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = SupportActivities.SkillShare, IsSelected = true },
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

            filterSet.MaxDistanceInMiles = new List<FilterField<int>>
            {
                new FilterField<int> { Value = 0, Label = "My street only" },
                new FilterField<int> { Value = 1, Label = "Within 1 mile" },
                new FilterField<int> { Value = 5, Label = "Within 5 miles" },
                new FilterField<int> { Value = 10, Label = "Within 10 miles", IsSelected = true },
                new FilterField<int> { Value = 999, Label = "Show all"},
            };

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

        public IEnumerable<ShiftJob> SortAndFilterShiftJobs(IEnumerable<ShiftJob> jobs, JobFilterRequest jfr)
        {
            var jobsToDisplay = jobs.Where(
                j => (jfr.JobStatuses == null || jfr.JobStatuses.Contains(j.JobStatus))
                    && (jfr.SupportActivities == null || jfr.SupportActivities.Contains(j.SupportActivity))
                    && (jfr.Locations == null || jfr.Locations.Count() == 0 || jfr.Locations.Contains(j.Location))
                    && (jfr.DueInNextXDays == null || j.StartDate.ToUKFromUTCTime() <= DateTime.Now.Date.AddDays(jfr.DueInNextXDays.Value))
                    && (jfr.PartsOfDay == null || jfr.PartsOfDay.Where(pod => pod.CheckStartTimeWithin(j.StartDate.ToUKFromUTCTime())).Count() > 0)
                    );

            return jfr.OrderBy switch
            {
                OrderBy.DateDue_Ascending =>
                   jobsToDisplay.OrderByDescending(j => Highlight(j, jfr)).ThenBy(j => j.StartDate),
                OrderBy.DateDue_Descending =>
                    jobsToDisplay.OrderByDescending(j => Highlight(j, jfr)).ThenByDescending(j => j.StartDate),
                OrderBy.DateRequested_Descending =>
                  jobsToDisplay.OrderByDescending(j => Highlight(j, jfr)).ThenByDescending(j => j.DateRequested),
                _ => throw new ArgumentException(message: $"Unexpected OrderByField value: {jfr.OrderBy}", paramName: nameof(jfr.OrderBy)),
            };
        }

        public async Task<IEnumerable<RequestSummary>> SortAndFilterRequests(IEnumerable<RequestSummary> requests, JobFilterRequest jfr, int? userId, CancellationToken cancellationToken)
        {
           var requestsWithDistances = await Task.WhenAll(requests.Select(async r => { r.DistanceInMiles = (await _userLocationService.GetLocationWithDistanceForCurrentUser(r, cancellationToken)).Distance; return r; }));

            var requestsToDisplay = requestsWithDistances.Where(
                r => (jfr.SupportActivities == null || r.JobBasics.Where(js => jfr.SupportActivities.Contains(js.SupportActivity)).Count() > 0)
                    && (jfr.JobStatuses == null || r.JobBasics.Where(js => (!userId.HasValue || js.VolunteerUserID.Equals(userId.Value)) && jfr.JobStatuses.Contains(js.JobStatus)).Count() > 0)
                    && (jfr.Locations == null || jfr.Locations.Count() == 0 || jfr.Locations.Contains(r.Shift.Location))
                    && (jfr.DueInNextXDays == null || r.Shift.StartDate.ToUKFromUTCTime() <= DateTime.Now.Date.AddDays(jfr.DueInNextXDays.Value))
                    && (jfr.PartsOfDay == null || jfr.PartsOfDay.Where(pod => pod.CheckStartTimeWithin(r.Shift.StartDate.ToUKFromUTCTime())).Count() > 0)
                    && (jfr.DueAfter == null || (userId.HasValue ? r.NextDueDate(userId.Value) : r.NextDueDate().ToUKFromUTCTime()) >= jfr.DueAfter?.Date)
                    && (jfr.DueBefore == null || (userId.HasValue ? r.NextDueDate(userId.Value) : r.NextDueDate().ToUKFromUTCTime()) <= jfr.DueBefore?.Date)
                    && (jfr.RequestedAfter == null || r.DateRequested.Date.ToUKFromUTCTime() >= jfr.RequestedAfter?.Date)
                    && (jfr.RequestedBefore == null) || r.DateRequested.Date.ToUKFromUTCTime() <= jfr.RequestedBefore?.Date);

            return jfr.OrderBy switch
            {
                OrderBy.DateDue_Ascending when userId.HasValue =>
                    requestsToDisplay.OrderByDescending(r => Highlight(r, jfr)).ThenBy(r => r.NextDueDate(userId.Value)),
                OrderBy.DateDue_Descending when userId.HasValue =>
                    requestsToDisplay.OrderByDescending(r => Highlight(r, jfr)).ThenByDescending(r => r.NextDueDate(userId.Value)),
                OrderBy.DateDue_Ascending when !userId.HasValue =>
                   requestsToDisplay.OrderByDescending(r => Highlight(r, jfr)).ThenBy(r => r.NextDueDate()),
                OrderBy.DateDue_Descending when !userId.HasValue =>
                    requestsToDisplay.OrderByDescending(r => Highlight(r, jfr)).ThenByDescending(r => r.NextDueDate()),
                OrderBy.Emptiest =>
                    requestsToDisplay.OrderByDescending(r => Highlight(r, jfr)).ThenByDescending(r => r.JobBasics.Where(js => js.JobStatus == JobStatuses.Open).Count()).ThenBy(r => r.NextDueDate()),
                OrderBy.DateRequested_Ascending =>
                    requestsToDisplay.OrderByDescending(r => Highlight(r, jfr)).ThenBy(r => r.DateRequested),
                OrderBy.DateRequested_Descending =>
                    requestsToDisplay.OrderByDescending(r => Highlight(r, jfr)).ThenByDescending(r => r.DateRequested),
                OrderBy.RequiringAdminAttention =>
                    requestsToDisplay.OrderByDescending(r => Highlight(r, jfr)).ThenByDescending(r => r.RequiringAdminAttentionScore()).ThenBy(r => r.NextDueDate()),
                OrderBy.DateStatusLastChanged_Ascending when userId.HasValue =>
                    requestsToDisplay.OrderByDescending(r => Highlight(r, jfr)).ThenBy(r => r.JobSummaries.Where(j => j.VolunteerUserID.Equals(userId.Value)).Min(j => j.DateStatusLastChanged)),
                OrderBy.DateStatusLastChanged_Descending when userId.HasValue =>
                    requestsToDisplay.OrderByDescending(r => Highlight(r, jfr)).ThenByDescending(r => r.JobSummaries.Where(j => j.VolunteerUserID.Equals(userId.Value)).Max(j => j.DateStatusLastChanged)),
                OrderBy.DateStatusLastChanged_Ascending when !userId.HasValue =>
                    requestsToDisplay.OrderByDescending(r => Highlight(r, jfr)).ThenBy(r => r.JobSummaries.Min(j => j.DateStatusLastChanged)),
                OrderBy.DateStatusLastChanged_Descending when !userId.HasValue =>
                    requestsToDisplay.OrderByDescending(r => Highlight(r, jfr)).ThenByDescending(r => r.JobSummaries.Max(j => j.DateStatusLastChanged)),
                _ => throw new ArgumentException(message: $"Unexpected OrderByField value: {jfr.OrderBy}", paramName: nameof(jfr.OrderBy)),
            };
        }

        public async Task<IEnumerable<IEnumerable<JobSummary>>> SortAndFilterOpenJobs(IEnumerable<IEnumerable<JobSummary>> jobs, JobFilterRequest jfr, CancellationToken cancellationToken)
        {
            var jobswithDistances = await Task.WhenAll(jobs.Select(async j => await Task.WhenAll(j.Select(async jd => { jd.DistanceInMiles = await _userLocationService.GetDistanceFromPostcodeForCurrentUser(jd.PostCode, cancellationToken); return jd; }))));

            var jobsToDisplay = jobswithDistances.Where(
                js => (jfr.JobStatuses == null || js.Where(js => jfr.JobStatuses.Contains(js.JobStatus)).Count() > 0)
                    && (jfr.SupportActivities == null || js.Where(j => jfr.SupportActivities.Contains(j.SupportActivity)).Count() > 0)
                    && (jfr.MaxDistanceInMiles == null || js.First().DistanceInMiles <= jfr.MaxDistanceInMiles)
                    && (jfr.DueInNextXDays == null || js.Any(j =>  j.JobStatus.Equals(JobStatuses.Open) && j.DueDate.ToUKFromUTCTime().Date <= DateTime.Now.Date.AddDays(jfr.DueInNextXDays.Value)))
                    && (jfr.RequestedAfter == null || js.First().DateRequested.ToUKFromUTCTime().Date >= jfr.RequestedAfter?.Date)
                    && (jfr.RequestedBefore == null) || js.First().DateRequested.ToUKFromUTCTime().Date <= jfr.RequestedBefore?.Date);


            return jfr.OrderBy switch
            {
                OrderBy.DateDue_Ascending =>
                    jobsToDisplay.OrderByDescending(js => Highlight(js, jfr)).ThenBy(js => js.Where(j => j.JobStatus.Equals(JobStatuses.Open)).Min(j => j.DueDate)),
                OrderBy.Distance_Ascending =>
                    jobsToDisplay.OrderByDescending(js => Highlight(js, jfr)).ThenBy(js => js.First().DistanceInMiles),
                _ => throw new ArgumentException(message: $"Unexpected OrderByField value: {jfr.OrderBy}", paramName: nameof(jfr.OrderBy)),
            };
        }

        private bool Highlight(JobBasic job, JobFilterRequest jfr)
        {
            return job.JobID.Equals(jfr.HighlightJobId) || job.RequestID.Equals(jfr.HighlightRequestId);
        }

        private bool Highlight(RequestSummary requestSummary, JobFilterRequest jfr)
        {
            return requestSummary.JobBasics.Exists(j => j.JobID.Equals(jfr.HighlightJobId)) || requestSummary.RequestID.Equals(jfr.HighlightRequestId);
        }

        private bool Highlight(IEnumerable<JobBasic> jobs, JobFilterRequest jfr)
        {
            return jobs.Where(j => j.JobID.Equals(jfr.HighlightJobId) || j.RequestID.Equals(jfr.HighlightRequestId)).Count() > 0;
        }
    }
}
