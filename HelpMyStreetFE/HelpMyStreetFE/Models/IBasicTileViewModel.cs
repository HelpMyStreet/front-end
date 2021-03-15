using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models
{
    public interface IBasicTileViewModel
    {
        public string ID { get; }
        public string Value { get; }
        public string Description { get; }
        public bool IsSelected { get; set; }
        public string DataType { get; }
    }
}
