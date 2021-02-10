using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.Stages
{
    public interface IRequestHelpStageViewModel
    {
        public string TemplateName { get; set; }
        public string FriendlyName { get; set; }
    }
}
