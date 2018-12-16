using System.Collections.Generic;
using System.Linq;
using Pidzemka.Extensions;
using Dijkstra.NET.Extensions;
using Dijkstra.NET.Model;

namespace Pidzemka.Models
{
    public class Map
    {
        public Map(int defaultFromStationId, IDictionary<int, Station> stations, IEnumerable<LinePart> lineParts)
        {
            DefaultFromStationId = defaultFromStationId;
            Stations = stations.AsReadOnly();
            LineParts = lineParts.ToList().AsReadOnly();

            var graph = new Dijkstra.NET.Model.Graph<int, string>();
            var result = graph.Dijkstra(0, 1);

        }

        public int DefaultFromStationId { get; }

        public IReadOnlyDictionary<int, Station> Stations { get; }

        public IReadOnlyList<LinePart> LineParts { get; }

        public Route FindRoute(int fromStationId, int toStationId)
        {
            var graph = new Graph<Station, LinePart>();

        }
    }
}
