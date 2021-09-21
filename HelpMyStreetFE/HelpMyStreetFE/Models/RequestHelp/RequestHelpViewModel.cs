using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp.Stages;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp
{
    public class RequestHelpViewModel
    {
        public int CurrentStepIndex { get; set; }
        public IList<IRequestHelpStageViewModel> Steps { get; set; }
        public string Action { get; set; }
        public int ReferringGroupID { get; set; }
        public string Source { get; set; }
        public RequestHelpFormVariant RequestHelpFormVariant { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });
        }

        public int? SelectedSupportActivity()
        {
            var requestStage = (RequestHelpRequestStageViewModel)Steps.Where(s => s is RequestHelpRequestStageViewModel).First();
            var selectedTask = requestStage.Tasks.Where(t => t.IsSelected).FirstOrDefault();
            return selectedTask != null ? (int?)selectedTask.SupportActivity : null;
        }

        public string Language { get; set; }

    }
}


