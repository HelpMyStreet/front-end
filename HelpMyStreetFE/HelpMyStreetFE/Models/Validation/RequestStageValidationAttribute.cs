using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Validation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    sealed public class RequestStageValidationAttribute : ValidationAttribute
    {
        public RequestStageValidationAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            List<string> errors = new List<string>();
            if (value is RequestHelpRequestStageViewModel)
            {
                var vm = (RequestHelpRequestStageViewModel)value;
                var task = vm.Tasks.Where(x => x.IsSelected).FirstOrDefault();
                if (task == null) errors.Add($"A Task must be selected");

                if (vm.Timeframes.Where(x => x.IsSelected).FirstOrDefault() == null) errors.Add($"A Timeframe must be selected");
                if (vm.Requestors.Where(x => x.IsSelected).FirstOrDefault() == null) errors.Add($"A Requestor must be selected");

                if (vm.Questions != null && vm.Questions.Questions != null)
                {
                    foreach (var q in vm.Questions.Questions)
                    {
                        if (q.Required && string.IsNullOrEmpty(q.Model)) errors.Add($"{q.DataValidationMessage}");
                    }
                }
            }
            else if (value is RequestHelpDetailStageViewModel)
            {
                var vm = (RequestHelpDetailStageViewModel)value;
                if (vm.Questions != null && vm.Questions.Questions != null)
                {
                    foreach (var q in vm.Questions.Questions)
                    {
                        if (q.Required && string.IsNullOrEmpty(q.Model)) errors.Add($"{q.DataValidationMessage}");
                    }
                }
            }

            if (errors.Count > 0)
            {
                ErrorMessage = string.Join(",", errors);
                return false;
            }
            return true;
        }
    }
}
