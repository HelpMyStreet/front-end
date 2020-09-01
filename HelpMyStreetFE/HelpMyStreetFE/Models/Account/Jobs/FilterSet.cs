using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class FilterSet
    {
        public IEnumerable<FilterField<SupportActivities>> SupportActivities { get; set; }
        public IEnumerable<FilterField<JobStatuses>> JobStatuses { get; set; }
        public IEnumerable<FilterField<int>> MaxDistanceInMiles { get; set; }
        public IEnumerable<FilterField<int>> DueInNextXDays { get; set; }

        public static class DefaultFilterSet
        {
            public static FilterSet GroupRequestsDefaultFilterSet = new FilterSet()
            {
                JobStatuses = new List<FilterField<JobStatuses>>()
                {
                    new FilterField<JobStatuses>() { Value = HelpMyStreet.Utils.Enums.JobStatuses.Cancelled, IsSelected = true },
                    new FilterField<JobStatuses>() { Value = HelpMyStreet.Utils.Enums.JobStatuses.Done, IsSelected = true },
                    new FilterField<JobStatuses>() { Value = HelpMyStreet.Utils.Enums.JobStatuses.InProgress, IsSelected = true },
                    new FilterField<JobStatuses>() { Value = HelpMyStreet.Utils.Enums.JobStatuses.Open, IsSelected = true }
                },
                DueInNextXDays = new List<FilterField<int>>()
                {
                    new FilterField<int>{Value = 1, Label = "Today" },
                    new FilterField<int>{Value = 7, Label = "This week"},
                    new FilterField<int>{Value = 14, Label = "Next 2 weeks"},
                    new FilterField<int>{Value = 999, Label = "Show all", IsSelected = true}
                },
            };

            public static FilterSet GetOpenRequestsDefaultFilterSet(User user)
            {
                FilterSet filterSet = new FilterSet()
                {
                    SupportActivities = new List<FilterField<SupportActivities>>()
                    {
                        new FilterField<SupportActivities>() { Value = HelpMyStreet.Utils.Enums.SupportActivities.CheckingIn, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = HelpMyStreet.Utils.Enums.SupportActivities.WellbeingPackage, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = HelpMyStreet.Utils.Enums.SupportActivities.Shopping, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = HelpMyStreet.Utils.Enums.SupportActivities.MealPreparation, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = HelpMyStreet.Utils.Enums.SupportActivities.Errands, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = HelpMyStreet.Utils.Enums.SupportActivities.DogWalking, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = HelpMyStreet.Utils.Enums.SupportActivities.HomeworkSupport, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = HelpMyStreet.Utils.Enums.SupportActivities.Other, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = HelpMyStreet.Utils.Enums.SupportActivities.PhoneCalls_Friendly, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = HelpMyStreet.Utils.Enums.SupportActivities.MedicalAppointmentTransport, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = HelpMyStreet.Utils.Enums.SupportActivities.CollectingPrescriptions, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = HelpMyStreet.Utils.Enums.SupportActivities.FaceMask, IsSelected = true },
                        new FilterField<SupportActivities>() { Value = HelpMyStreet.Utils.Enums.SupportActivities.WellbeingPackage, IsSelected = true },
                    },
                    DueInNextXDays = new List<FilterField<int>>()
                    {
                        new FilterField<int> { Value = 1, Label = "Today" },
                        new FilterField<int> { Value = 7, Label = "This week" },
                        new FilterField<int> { Value = 14, Label = "Next 2 weeks" },
                        new FilterField<int> { Value = 999, Label = "Show all", IsSelected = true }
                    },
                    MaxDistanceInMiles = new List<FilterField<int>>()
                    {
                        new FilterField<int> { Value = 0, Label = "My street only" },
                        new FilterField<int> { Value = 1, Label = "Within 1 mile" },
                        new FilterField<int> { Value = 5, Label = "within 5 miles" },
                        new FilterField<int> { Value = 10, Label = "within 10 miles" },
                        new FilterField<int> { Value = 20, Label = "within 20 miles", IsSelected = true },
                    },
                };

                if (user.SupportActivities.Contains(HelpMyStreet.Utils.Enums.SupportActivities.CommunityConnector))
                {
                    ((List<FilterField<SupportActivities>>)filterSet.SupportActivities)
                        .Insert(0, new FilterField<SupportActivities>() { Value = HelpMyStreet.Utils.Enums.SupportActivities.CommunityConnector, IsSelected = true });
                }

                return filterSet;
            }
        }
    }
}
