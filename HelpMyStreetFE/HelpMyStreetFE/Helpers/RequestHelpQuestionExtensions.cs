using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp;

namespace HelpMyStreetFE.Helpers
{
    public static class RequestHelpQuestionExtensions
    {
        public static string Class(this RequestHelpQuestion question)
        {
            return question.ID switch
            {
                (int)Questions.FaceMask_Amount => "small-width",
                _ => ""
            };
        }

        public static int? Max(this RequestHelpQuestion question)
        {
            return question.ID switch
            {
                (int)Questions.FaceMask_Amount => 200,
                _ => null
            };
        }

        public static string DataValidationMessage(this RequestHelpQuestion question)
        {
            return question.ID switch
            {
                (int)Questions.FaceMask_Amount => @"Please enter a number between 1 and 200. We can only accept requests for up to 200 face coverings using this form, if you need more email For the Love of Scrubs at fortheloveofscrubs@outlook.com",
                _ => question.DataValidationMessage
            };
        }
    }
}
