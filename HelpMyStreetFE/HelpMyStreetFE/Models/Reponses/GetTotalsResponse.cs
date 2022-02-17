using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Reponses
{
    public class GetTotalsResponse
    {
    }

    public class Rootobject
    {
        public object XAxisName { get; set; }
        public object YAxisName { get; set; }
        public Datapoint[] DataPoints { get; set; }
    }

    public class Datapoint
    {
        public string XAxis { get; set; }
        public string Series { get; set; }
        public int Value { get; set; }
        public object Children { get; set; }
    }

}
