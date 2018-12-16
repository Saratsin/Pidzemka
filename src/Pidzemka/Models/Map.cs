using System.Collections.Generic;
using System.Linq;
using Pidzemka.Extensions;
using Pidzemka.Models.Dijkstra;
using Pidzemka.Models.Dto;

namespace Pidzemka.Models
{
    public class Map
    {
        private readonly IReadOnlyDictionary<int, Station> stationsDictionary;

        public Map(MapDto dto)
        {
            DefaultStationId = dto.DefaultStationId;

            var nodes = dto.Nodes.Select(nodeDto => new Node(nodeDto)).ToList().AsReadOnly();

            stationsDictionary = dto.Stations
                                    .Select(stationDto => new Station(stationDto))
                                    .ToDictionary(station => station.Id, station => station)
                                    .AsReadOnly();

            foreach (var station in stationsDictionary.Values)
            {
                var stationNodes = nodes.Where(node => node.HasStation(station.Id))
                                        .Select(node => (OtherStationId: node.StartStationId != station.Id ?
                                                         node.StartStationId : node.EndStationId,
                                                         TimeDistance: node.TimeDistance));

                var nearestStationsWithDistances = stationNodes.ToDictionary(node => stationsDictionary[node.OtherStationId],
                                                                             node => node.TimeDistance);

                station.InitializeNearestStations(nearestStationsWithDistances);
            }
        }

        public int DefaultStationId { get; }

        public Route FindRoute(int fromStationId, int toStationId)
        {
            var allStationsWithNearestStationsDistances = stationsDictionary.Values.ToList();
            var fromStation = stationsDictionary[fromStationId];
            var toStation = stationsDictionary[toStationId];

            var routeStations = DijkstraGraph.FindShortestPath(allStationsWithNearestStationsDistances, fromStation, toStation);

            var routeNodes = new List<Node>(routeStations.Count - 1);

            for(int i = 0; i < routeStations.Count - 1; ++i)
            {
                var currentStation = routeStations[i];
                var nextStation = routeStations[i + 1];
                var timeDistance = currentStation.GetNearestStationDistance(nextStation.Id);
                var node = new Node(currentStation.Id, nextStation.Id, timeDistance);
                routeNodes.Add(node);
            }

            return new Route(routeStations, routeNodes);
        }
    }
}
