using System;
namespace Pidzemka.Models.Dto
{
    public class NodeDto
    {
        public int StartStationId { get; set; }

        public int EndStationId { get; set; }

        public TimeSpan TimeDistance { get; set; }
    }
}
