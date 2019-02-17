using System.Collections.Generic;
using System.Drawing;

namespace Pidzemka.Models
{
    public class MapData
    {
        public float StationsStrokeWidth { get; set; }

        public float StationsRadius { get; set; }

        public float MinimumScaleDistance { get; set; }

        public SizeF MapSize { get; set; }

        public IEnumerable<PointF> Stations { get; set; }
    }
}