using System.Drawing;

namespace Pidzemka.Models.Dto
{
    public class StationDto
    {
        public int Id { get; set; }

        public PointF MapCoordinates { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}