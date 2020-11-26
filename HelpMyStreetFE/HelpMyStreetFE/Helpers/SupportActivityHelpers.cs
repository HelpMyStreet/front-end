using System;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Helpers
{
    public static class SupportActivityHelpers
    {
        public static string Sentences(this SupportActivities activity, bool pleural)
        {
            if (pleural)
            {
                return activity switch
                {
                    SupportActivities.Shopping => "shopping trips",
                    SupportActivities.FaceMask => "requests for face coverings",
                    SupportActivities.CheckingIn => "people checked on",
                    SupportActivities.CollectingPrescriptions => "prescriptions collected",
                    SupportActivities.Errands => "errands run",
                    SupportActivities.DogWalking => "dogs walked",
                    SupportActivities.MealPreparation => "meals prepared",
                    SupportActivities.PhoneCalls_Friendly => "friendly chats",
                    SupportActivities.PhoneCalls_Anxious => "supportive chats",
                    SupportActivities.HomeworkSupport => "homework assignments",
                    SupportActivities.WellbeingPackage => "wellbeing packages sent",
                    SupportActivities.CommunityConnector => "community connector tasks",
                    SupportActivities.MedicalAppointmentTransport => "medical appointments transported",
                    SupportActivities.ColdWeatherArmy => "cold weather army tasks completed",
                    SupportActivities.Other => "other tasks",
                    _ => throw new ArgumentException(message: $"Unexpected SupportActivity: {activity}", paramName: nameof(activity))
                };
            } else
            {
                return activity switch
                {
                    SupportActivities.Shopping => "shopping trip",
                    SupportActivities.FaceMask => "request for face coverings",
                    SupportActivities.CheckingIn => "person checked on",
                    SupportActivities.CollectingPrescriptions => "prescription collected",
                    SupportActivities.Errands => "errand run",
                    SupportActivities.DogWalking => "dog walked",
                    SupportActivities.MealPreparation => "meal prepared",
                    SupportActivities.PhoneCalls_Friendly => "friendly chat",
                    SupportActivities.PhoneCalls_Anxious => "supportive chat",
                    SupportActivities.HomeworkSupport => "homework assignment",
                    SupportActivities.WellbeingPackage => "wellbeing package sent",
                    SupportActivities.CommunityConnector => "community connector task",
                    SupportActivities.MedicalAppointmentTransport => "medical appointment transported",
                    SupportActivities.ColdWeatherArmy => "cold weather army task completed",
                    SupportActivities.Other => "other task",
                    _ => throw new ArgumentException(message: $"Unexpected SupportActivity: {activity}", paramName: nameof(activity))
                };
            }
        }

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
                SupportActivities.CommunityConnector => "phone-green.svg",
                SupportActivities.ColdWeatherArmy => "snowflake.svg",
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
                SupportActivities.WellbeingPackage => "light-purple",
                SupportActivities.CommunityConnector => "green",
                SupportActivities.ColdWeatherArmy => "blue",
                _ => ""
            };
        }
    }
}
