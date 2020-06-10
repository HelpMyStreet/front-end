﻿using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp.Stages;
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
        public List<string> Errors { get; set; } = new List<string>();
        public string ToJson() {             
                return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                });
       }

    }
}


