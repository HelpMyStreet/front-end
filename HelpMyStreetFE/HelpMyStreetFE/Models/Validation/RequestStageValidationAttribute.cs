using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreetFE.Helpers;
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
            if (value is RequestHelpRequestStageViewModel vm)
            {
                var task = vm.Tasks.Where(x => x.IsSelected).FirstOrDefault();
                if (task == null) errors.Add($"A Task must be selected");

                RequestHelpTimeViewModel selectedTimeframe = null;

                if (vm.Timeframes.Where(x => x.IsSelected).FirstOrDefault() == null)
                {
                    errors.Add($"A Timeframe must be selected");
                }
                else
                {
                    selectedTimeframe = vm.Timeframes.Where(x => x.IsSelected).First();

                    if (selectedTimeframe.DueDateType.HasDate())
                    {
                        if (selectedTimeframe.StartTime.Equals(DateTime.MinValue)) errors.Add("A date must be specified");
                        if (selectedTimeframe.DueDateType.HasEndTime() || selectedTimeframe.DueDateType.HasEndDate())
                        {
                            if (selectedTimeframe.EndTime.Equals(DateTime.MinValue)) errors.Add("An end time must be specified");
                        }
                    }
                }

                if (vm.Frequencies.Where(x => x.IsSelected).FirstOrDefault() == null)
                {
                    errors.Add("A frequency must be selected");
                }
                else
                {
                    var selectedFrequency = vm.Frequencies.Where(x => x.IsSelected).First();
                    if (!selectedFrequency.Frequency.Equals(Frequency.Once) && !selectedFrequency.Frequency.Equals(Frequency.Ongoing))
                    {
                        var occurrences = vm.Occurrences;
                        if (occurrences == null || occurrences > selectedFrequency.Frequency.MaxOccurrences())
                        {
                            errors.Add("Valid number of occurrences must be supplied");
                        }
                    }
                }

                if (vm.Requestors.Where(x => x.IsSelected).FirstOrDefault() == null) errors.Add($"A Requestor must be selected");

                if (vm.Questions != null && vm.Questions.Questions != null)
                {
                    foreach (var q in vm.Questions.Questions)
                    {
                        if (q.Required && string.IsNullOrEmpty(q.Model)) errors.Add($"{q.DataValidationMessage}");
                    }
                }
            }
            else if (value is RequestHelpDetailStageViewModel rsvm)
            {
                if (rsvm.Questions != null && rsvm.Questions.Questions != null)
                {
                    foreach (var q in rsvm.Questions.Questions)
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
