using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Helpers
{
    public static class SupportActivityHelpers
    {
        public static string FriendlyName(this SupportActivities activity)
        {
            return activity switch
            {
                SupportActivities.Shopping => "Shopping",
                SupportActivities.FaceMask => "Homemade Face Coverings",
                SupportActivities.CheckingIn => "Check in",
                SupportActivities.CollectingPrescriptions => "Prescriptions",
                SupportActivities.Errands => "Errands",
                SupportActivities.DogWalking => "Dog Walking",
                SupportActivities.MealPreparation => "A Meal",
                SupportActivities.PhoneCalls_Friendly => "Friendly chat",
                SupportActivities.PhoneCalls_Anxious => "Supportive chat",
                SupportActivities.HomeworkSupport => "Homework",
                SupportActivities.WellbeingPackage => "Wellbeing Package",
                SupportActivities.CommunityConnector => "Community Connector",
                _ => "Other"
            };
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
                SupportActivities.WellbeingPackage => "vitalsforveterans.png",
                SupportActivities.CommunityConnector => "communityconnector.png",
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
                _ => ""
            };
        }
    }
}
