namespace HelpMyStreetFE.Models.Account.Jobs
{
    public enum OrderBy
    {
        Distance_Ascending = 1,
        DateRequested_Ascending = 2,
        DateRequested_Descending = 3,
        DateDue_Ascending = 4,
        DateDue_Descending = 5,
        DateStatusLastChanged_Ascending = 6,
        DateStatusLastChanged_Descending = 7,
        RequiringAdminAttention = 8,
        Emptiest = 9
    }
}
