using System;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Helpers
{
    public static class SupportActivityHelpers
    {
        public static string Icon(this SupportActivities activity)
        {
            return activity switch
            {
                SupportActivities.Shopping => "shopping.svg",
                SupportActivities.FaceMask => "face-covering.svg",
                SupportActivities.CheckingIn => "check-in.svg",
                SupportActivities.CollectingPrescriptions => "prescriptions.svg",
                SupportActivities.Errands => "Errands.svg",
                SupportActivities.DogWalking => "dog-walking.svg",
                SupportActivities.MealPreparation => "meal.svg",
                SupportActivities.PhoneCalls_Friendly => "friendlychat.svg",
                SupportActivities.PhoneCalls_Anxious => "supportivechat.svg",
                SupportActivities.HomeworkSupport => "homework.svg",
                SupportActivities.WellbeingPackage => "package.svg",
                SupportActivities.CommunityConnector => "friendlychat.svg",
                SupportActivities.ColdWeatherArmy => "snowflake.svg",
                SupportActivities.MealsToYourDoor => "mealstoyourdoor.svg",
                SupportActivities.MealtimeCompanion => "mealtimecompanion.svg",
                SupportActivities.VolunteerSupport => "volunteers.svg",
                SupportActivities.Transport => "transport.svg",
                SupportActivities.VaccineSupport => "vaccination.svg",
                SupportActivities.EmergencySupport => "emergency-support.svg",
                SupportActivities.BinDayAssistance => "bin-day-assistance.svg",
                SupportActivities.Covid19Help => "covid-19-support.svg",
                SupportActivities.DigitalSupport => "digital-support.svg",
                SupportActivities.PracticalSupport => "practical.svg",
                SupportActivities.InPersonBefriending => "befriending.svg",
                SupportActivities.BankStaffVaccinator => "vaccination.svg",
                SupportActivities.SkillShare => "puzzle-piece.svg",
                SupportActivities.Accommodation => "accommodation.svg",
                SupportActivities.AdvertisingRoles=> "advertisingroles.svg",
                _ => "question-mark.svg"
            };
        }

        public static string Class(this SupportActivities activity)
        {
            return activity switch
            {
                SupportActivities.Shopping => "blue",
                SupportActivities.FaceMask => "dark-blue",
                SupportActivities.CheckingIn => "light-purple",
                SupportActivities.CollectingPrescriptions => "blue",
                SupportActivities.Errands => "blue",
                SupportActivities.DogWalking => "dark-blue",
                SupportActivities.MealPreparation => "dark-blue",
                SupportActivities.PhoneCalls_Friendly => "light-purple",
                SupportActivities.PhoneCalls_Anxious => "light-purple",
                SupportActivities.HomeworkSupport => "dark-blue",
                SupportActivities.WellbeingPackage => "blue",
                SupportActivities.CommunityConnector => "light-purple",
                SupportActivities.ColdWeatherArmy => "blue",
                SupportActivities.MealsToYourDoor => "blue",
                SupportActivities.MealtimeCompanion => "light-purple",
                SupportActivities.EmergencySupport => "dark-blue",
                SupportActivities.PracticalSupport => "blue",
                SupportActivities.InPersonBefriending => "light-purple",
                SupportActivities.BankStaffVaccinator => "dark-blue",
                SupportActivities.Accommodation => "dark-blue",
                _ => "dark-blue",
            };
        }

        public static bool IncludeInDefaultSortAndFilterSet(this SupportActivities activity)
        {
            return activity switch
            {
                SupportActivities.Shopping => true,
                SupportActivities.FaceMask => true,
                SupportActivities.WellbeingPackage => true,
                SupportActivities.CheckingIn => true,
                SupportActivities.CollectingPrescriptions => true,
                SupportActivities.Errands => true,
                SupportActivities.MealPreparation => true,
                SupportActivities.PhoneCalls_Friendly => true,
                SupportActivities.HomeworkSupport => true,
                SupportActivities.DogWalking => true,
                SupportActivities.ColdWeatherArmy => true,
                SupportActivities.Transport => true,
                SupportActivities.MealsToYourDoor => true,
                SupportActivities.MealtimeCompanion => true,
                SupportActivities.EmergencySupport => true,
                SupportActivities.VolunteerSupport => true,
                SupportActivities.Covid19Help => true,
                SupportActivities.BinDayAssistance => true,
                SupportActivities.DigitalSupport => true,
                SupportActivities.InPersonBefriending => true,
                SupportActivities.PracticalSupport => true,
                SupportActivities.SkillShare => true,
                SupportActivities.Accommodation => false,
                SupportActivities.Other => true,

                SupportActivities.CommunityConnector => false,
                SupportActivities.VaccineSupport => false,
                SupportActivities.BankStaffVaccinator => false,

                SupportActivities.MedicalAppointmentTransport => false,
                SupportActivities.PhoneCalls_Anxious => false,
                SupportActivities.VolunteerInduction => false,
                SupportActivities.AdvertisingRoles => true,

                _ => throw new ArgumentException(message: $"Unexpected SupportActivities value: {activity}", paramName: nameof(activity))
            };
        }
    }
}
