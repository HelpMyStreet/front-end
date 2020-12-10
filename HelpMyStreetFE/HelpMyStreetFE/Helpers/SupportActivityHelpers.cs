using System;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Helpers
{
    public static class SupportActivityHelpers
    {
        public static string Sentences(this SupportActivities activity, int count)
        {
            if (count == 1)
            {
                return activity switch
                {
                    SupportActivities.Shopping => "1 shopping trip",
                    SupportActivities.FaceMask => "1 request for face coverings",
                    SupportActivities.CheckingIn => "1 person checked on",
                    SupportActivities.CollectingPrescriptions => "1 prescription collected",
                    SupportActivities.Errands => "1 errand run",
                    SupportActivities.DogWalking => "1 dog walked",
                    SupportActivities.MealPreparation => "1 meal prepared",
                    SupportActivities.PhoneCalls_Friendly => "1 friendly chat",
                    SupportActivities.PhoneCalls_Anxious => "1 supportive chat",
                    SupportActivities.HomeworkSupport => "1 homework assignment",
                    SupportActivities.WellbeingPackage => "1 wellbeing package delivered",
                    SupportActivities.CommunityConnector => "1 community connector task",
                    SupportActivities.MedicalAppointmentTransport => "1 person transported to a medical appointment",
                    SupportActivities.ColdWeatherArmy => "1 cold weather army task completed",
                    SupportActivities.Other => "1 other task completed",
                    _ => throw new ArgumentException(message: $"Unexpected SupportActivity: {activity}", paramName: nameof(activity))
                };
            }
            else
            {
                return activity switch
                {
                    SupportActivities.Shopping => $"{count} shopping trips",
                    SupportActivities.FaceMask => $"{count} requests for face coverings",
                    SupportActivities.CheckingIn => $"{count} people checked on",
                    SupportActivities.CollectingPrescriptions => $"{count} prescriptions collected",
                    SupportActivities.Errands => $"{count} errands run",
                    SupportActivities.DogWalking => $"{count} dogs walked",
                    SupportActivities.MealPreparation => $"{count} meals prepared",
                    SupportActivities.PhoneCalls_Friendly => $"{count} friendly chats",
                    SupportActivities.PhoneCalls_Anxious => $"{count} supportive chats",
                    SupportActivities.HomeworkSupport => $"{count} homework assignments",
                    SupportActivities.WellbeingPackage => $"{count} wellbeing packages delivered",
                    SupportActivities.CommunityConnector => $"{count} community connector tasks",
                    SupportActivities.MedicalAppointmentTransport => $"{count} people transported to medical appointments",
                    SupportActivities.ColdWeatherArmy => $"{count} cold weather army tasks completed",
                    SupportActivities.Other => $"{count} other tasks completed",
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
                _ => "dark-blue",
            };
        }
    }
}
