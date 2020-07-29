using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.RequestHelp;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;

namespace HelpMyStreetFE
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


    public static class JobSummaryQuestionExtensions
    {
        public static bool ShowOnTaskManagement(this Question question)
        {
            return question.Id switch
            {
                (int)Questions.FaceMask_Recipient => true,
                (int)Questions.FaceMask_Cost => true,
                (int)Questions.SupportRequesting => true,
                (int)Questions.FaceMask_SpecificRequirements => true,
                _ => false
            };
        }

        public static string FriendlyName(this Question question)
        {
            return question.Id switch
            {
                (int)Questions.FaceMask_SpecificRequirements => "Request Description",
                (int)Questions.SupportRequesting => "Request Description",
                _ => question.Name
            };
        }

    }

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


    public static class HtmlHelpers
    {
        private const string ScriptsKey = "DelayedScripts";

        public static IDisposable BeginScripts(this IHtmlHelper helper)
        {
            return new ScriptBlock(helper.ViewContext);
        }

        public static HtmlString PageScripts(this IHtmlHelper helper)
        {
            return new HtmlString(string.Join(Environment.NewLine, GetPageScriptsList(helper.ViewContext.HttpContext)));
        }

        private static List<string> GetPageScriptsList(HttpContext httpContext)
        {
            var pageScripts = (List<string>)httpContext.Items[ScriptsKey];
            if (pageScripts == null)
            {
                pageScripts = new List<string>();
                httpContext.Items[ScriptsKey] = pageScripts;
            }
            return pageScripts;
        }

        private class ScriptBlock : IDisposable
        {
            private readonly TextWriter _originalWriter;
            private readonly StringWriter _scriptsWriter;

            private readonly ViewContext _viewContext;

            public ScriptBlock(ViewContext viewContext)
            {
                _viewContext = viewContext;
                _originalWriter = _viewContext.Writer;
                _viewContext.Writer = _scriptsWriter = new StringWriter();
            }

            public void Dispose()
            {
                _viewContext.Writer = _originalWriter;
                var pageScripts = GetPageScriptsList(_viewContext.HttpContext);
                pageScripts.Add(_scriptsWriter.ToString());
            }
        }
    }
}
