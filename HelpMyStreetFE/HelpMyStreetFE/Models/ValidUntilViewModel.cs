using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models
{
    public class ValidUntilViewModel
    {
        public string ID { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public bool ShowDatePicker { get; set; }
        public bool IsSelected { get; set; }
    }
}
