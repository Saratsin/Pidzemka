using System.Collections.Generic;

namespace Pidzemka.Models.Dto
{
    public class MapDto
    {
        public int DefaultStationId { get; set; }

        public List<StationDto> Stations { get; set; }

        public List<NodeDto> Nodes { get; set; }
    }
}