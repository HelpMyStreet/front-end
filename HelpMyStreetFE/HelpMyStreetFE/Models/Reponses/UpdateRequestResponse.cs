using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Reponses
{
    public class UpdateRequestResponseContent
    {

    }

    public class UpdateRequestResponse
    {
        public bool HasContent { get; set; }
        public bool IsSuccessful { get; set; }
        public UpdateRequestResponseContent Content { get; set; }
    }
}
