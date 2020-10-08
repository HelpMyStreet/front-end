using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Models.Email;
using Microsoft.Extensions.Options;

namespace HelpMyStreetFE.Services
{
    public class FilterService : IFilterService
    {
        private readonly IOptions<RequestSettings> _requestSettings;
        public FilterService(IOptions<RequestSettings> requestSettings)
        {
            _requestSettings = requestSettings;
        }

        public SortAndFilterSet GetDefaultSortAndFilterSet(JobSet jobSet, User user)
        {
            return jobSet switch
            {
                JobSet.GroupRequests => GroupRequestsDefaultSortAndFilterSet,
                JobSet.UserOpenRequests_MatchingCriteria => GetOpenRequestsMatchingCriteriaDefaultSortAndFilterSet(),
                JobSet.UserOpenRequests_NotMatchingCriteria => GetOpenRequestsNotMatchingCriteriaDefaultSortAndFilterSet(user),
                JobSet.UserAcceptedRequests => GetAcceptedRequestsDefaultSortAndFilterSet(),
                JobSet.UserCompletedRequests => GetCompletedRequestsDefaultSortAndFilterSet(),
                _ => throw new ArgumentException(message: $"Unexpected JobFilterRequest.JobSet value: {jobSet}", paramName: nameof(jobSet))
            };
        }

        private SortAndFilterSet GroupRequestsDefaultSortAndFilterSet = new SortAndFilterSet()
        {
            JobStatuses = new List<FilterField<JobStatuses>>()
                {
                    new FilterField<JobStatuses>() { Value = JobStatuses.Open, IsSelected = true },
                    new FilterField<JobStatuses>() { Value = JobStatuses.InProgress, IsSelected = true },
                    new FilterField<JobStatuses>() { Value = JobStatuses.Done },
                    new FilterField<JobStatuses>() { Value = JobStatuses.Cancelled },
                },
            OrderBy = new List<OrderByField>()
                {
                    new OrderByField() { Value = OrderBy.DateDue_Ascending, Label = "Help needed soonest", IsSelected = true },
                    new OrderByField() { Value = OrderBy.DateDue_Descending, Label = "Help needed least soon" },
                    new OrderByField() { Value = OrderBy.DateRequested_Descending, Label = "Requested last" },
                    new OrderByField() { Value = OrderBy.DateRequested_Ascending, Label = "Requested first" },
                    new OrderByField() { Value = OrderBy.DateStatusLastChanged_Descending, Label = "Updated most recently" },
                    new OrderByField() { Value = OrderBy.DateStatusLastChanged_Ascending, Label = "Updated least recently" },
                },
        };

        private SortAndFilterSet GetOpenRequestsMatchingCriteriaDefaultSortAndFilterSet()
        {
            SortAndFilterSet filterSet = new SortAndFilterSet()
            {
                OrderBy = new List<OrderByField>()
                    {
                        new OrderByField() { Value = OrderBy.DateDue_Ascending, Label = "Help needed soonest", IsSelected = true },
                        new OrderByField() { Value = OrderBy.Distance_Ascending, Label = "Closest to my address" },
                  },
            };

            return filterSet;
        }

        private SortAndFilterSet GetOpenRequestsNotMatchingCriteriaDefaultSortAndFilterSet(User user)
        {
            SortAndFilterSet filterSet = new SortAndFilterSet()
            {
                SupportActivities = new List<FilterField<SupportActivities>>()
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
                        new FilterField<SupportActivities>() { Value = SupportActivities.Other, IsSelected = true },

                        // The following are not currently on any request help form
                        //new FilterField<SupportActivities>() { Value = SupportActivities.MedicalAppointmentTransport, IsSelected = true },
                        //new FilterField<SupportActivities>() { Value = SupportActivities.PhoneCalls_Anxious, IsSelected = true },
                    },
                DueInNextXDays = new List<FilterField<int>>()
                    {
                        new FilterField<int> { Value = 1, Label = "Today" },
                        new FilterField<int> { Value = 7, Label = "This week" },
                        new FilterField<int> { Value = 14, Label = "Next 2 weeks" },
                        new FilterField<int> { Value = 999, Label = "Show all", IsSelected = true }
                    },
                OrderBy = new List<OrderByField>()
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
                filterSet.MaxDistanceInMiles = new List<FilterField<int>>()
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
                filterSet.MaxDistanceInMiles = new List<FilterField<int>>()
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
            SortAndFilterSet filterSet = new SortAndFilterSet()
            {
                OrderBy = new List<OrderByField>()
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
            SortAndFilterSet filterSet = new SortAndFilterSet()
            {
                OrderBy = new List<OrderByField>()
                    {
                        new OrderByField() { Value = OrderBy.DateStatusLastChanged_Descending, Label = "Completed last", IsSelected = true },
                        new OrderByField() { Value = OrderBy.DateStatusLastChanged_Ascending, Label = "Completed first" },
                    },
            };

            return filterSet;
        }
    }
}
