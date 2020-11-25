using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Helpers
{
    public static class JobSummaryQuestionExtensions
    {
        public static bool ShowOnTaskManagement(this Question question, bool showSensitiveData)
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
                (int)Questions.Shopping_List => true,
                (int)Questions.Prescription_PharmacyAddress => true,
                (int)Questions.SensitiveInformation => showSensitiveData,
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
                (int)Questions.Shopping_List => "Shopping List",
                (int)Questions.Prescription_PharmacyAddress => "Pharmacy Address",
                (int)Questions.SensitiveInformation => "Further Details (Restricted Access)",
                _ => question.Name
            };
        }

        public static int TaskManagementDisplayOrder(this Question question)
        {
            return question.Id switch
            {
                (int)Questions.Shopping_List => 1,
                (int)Questions.Prescription_PharmacyAddress => 1,
                (int)Questions.FaceMask_Amount => 1,
                (int)Questions.FaceMask_Recipient => 2,
                (int)Questions.FaceMask_Cost => 3,
                (int)Questions.FaceMask_SpecificRequirements => 4,
                (int)Questions.SupportRequesting => 10,
                (int)Questions.CommunicationNeeds => 98,
                (int)Questions.AnythingElseToTellUs => 99,
                (int)Questions.SensitiveInformation => 100,
                _ => 0
            };
        }
    }
}
