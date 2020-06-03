using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp.NewMVCForm.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.NewMVCForm.Models
{
    public class RequestHelpNewViewModel
    {
        public int CurrentStepIndex { get; set; }
        public IList<IRequestHelpStepsViewModel> Steps { get; set; }

        public string ToJson() {             
                return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                });
       }

    }
}


