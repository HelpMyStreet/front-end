using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models
{
    public class ValidUntilViewModel : BasicTileViewModel
    {
        public string Label { get; set; }
        public override string Value { get; set; }
        public bool ShowDatePicker { get; set; }
    }
}
