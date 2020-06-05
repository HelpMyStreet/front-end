using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
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
                SupportActivities.FaceMask => "Face Covering",
                SupportActivities.CheckingIn => "Check in",
                SupportActivities.CollectingPrescriptions => "Prescriptions",
                SupportActivities.Errands => "Errands",
                SupportActivities.DogWalking => "Dog Walking",
                SupportActivities.MealPreparation => "A Meal",
                SupportActivities.PhoneCalls_Friendly => "Friendly chat",
                SupportActivities.PhoneCalls_Anxious => "Supportive chat",
                SupportActivities.HomeworkSupport => "Homework",
                SupportActivities.WellbeingPackage => "Wellbeing Package",
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
                _ => ""
            };
        }
    }

    public static class RequestHelpQuestionExtensions {
        
        public static string QuestionLocation(this RequestHelpQuestion question)
        {
            return question.ID switch
            {            
                4 => "pos2",
                5 => "pos3",
                3 => "pos3",
                _ => "pos1"
            };        
        }
        public static string Subtext(this RequestHelpQuestion question)
        {
            return question.ID switch
            {
                3 => "Remember they’re washable and reusable, so only request what you need between washes.",
                5 => "Volunteers are providing their time and skills free of charge.",
                _ => string.Empty
            };
        }

        public static string Placeholder(this RequestHelpQuestion question)
        {
            return question.ID switch
            {
                2 => "If you have very specific requirements, it may take longer to find a volunteer to help with your request. Please don’t include any personal information, such as name or address in this box. We’ll ask for that later.",
                1 => "Please don’t include any sensitive details that aren’t needed in order for us to help you",                
                _ => string.Empty
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
