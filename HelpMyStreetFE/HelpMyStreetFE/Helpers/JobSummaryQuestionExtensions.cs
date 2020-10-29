using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Helpers
{
    public static class JobSummaryQuestionExtensions
    {
        public static bool ShowOnTaskManagement(this Question question)
        {
            return question.Id switch
            {
                (int)Questions.FaceMask_Amount => true,
                (int)Questions.FaceMask_Recipient => true,
                (int)Questions.FaceMask_Cost => true,
                (int)Questions.SupportRequesting => true,
                (int)Questions.FaceMask_SpecificRequirements => true,
                (int)Questions.CommunicationNeeds => true,
                (int)Questions.AnythingElseToTellUs => true,
                (int)Questions.Shopping => true,
                (int)Questions.Prescription => true,
                _ => false
            };
        }

        public static string FriendlyName(this Question question)
        {
            return question.Id switch
            {
                (int)Questions.FaceMask_SpecificRequirements => "Request Description",
                (int)Questions.SupportRequesting => "Request Description",
                (int)Questions.CommunicationNeeds => "Communication Needs",
                (int)Questions.AnythingElseToTellUs => "Further Details",
                _ => question.Name
            };
        }
    }
}
