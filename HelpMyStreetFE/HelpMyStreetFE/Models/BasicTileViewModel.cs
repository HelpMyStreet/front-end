﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models
{
    public abstract class BasicTileViewModel
    {
        public BasicTileViewModel()
        {
            HideTileWhen = new List<Tuple<string, string>>();
        }

        public int ID { get; set; }
        public abstract string Value { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; }
        public string DataType { get; protected set; }
        public List<Tuple<string, string>> HideTileWhen { get; private set; }

        public string HideTileWhenAttribute
        {
            get
            {
                return string.Join(",", HideTileWhen.Select(a => $"{a.Item1}:{a.Item2}"));
            }
        }
    }
}
