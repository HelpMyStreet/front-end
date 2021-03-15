using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models
{
    public class TileSelectorViewModel
    {
        public string Label { get; set; }
        public IEnumerable<IBasicTileViewModel> TileVMs { get; set; }
        public string FieldName { get; set; }
        public string TileView { get; set; } = "BasicTile";
    }
}
