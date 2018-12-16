using System.Collections.Generic;
using System.Linq;
using Pidzemka.Common;

namespace Pidzemka.Models
{
    public class Route
    {
        public string MapSvgRouteRegex { get; }

        public IReadOnlyList<Station> Stations { get; }

        public IReadOnlyList<Node> LineParts { get; }

        public Route(IEnumerable<Station> stations, IEnumerable<Node> nodes)
        {
            Stations = stations.ToList().AsReadOnly();
            LineParts = nodes.ToList().AsReadOnly();
            MapSvgRouteRegex = GetMapSvgRouteRegex(LineParts);
        }

        private static string GetMapSvgRouteRegex(IEnumerable<Node> nodes)
        {
            var conditions = nodes.Select(value =>
            {
                return $"{value.StartStationId}_{value.EndStationId}";
            });

            var summaryCondition = string.Join("|", conditions);

            var subwayLineRegex = string.Format(Constants.SubwayRouteLinePartsRegexPattern, summaryCondition);

            return subwayLineRegex;
        }
    }
}
