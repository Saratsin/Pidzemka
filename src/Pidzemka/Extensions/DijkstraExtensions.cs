using System;
using Pidzemka.Models.Dijkstra;
using Pidzemka.Models;

namespace Pidzemka.Extensions
{
    public static class DijkstraExtensions
    {
        public static object Dijkstra(this PidzemkaGraph graph, uint from, uint to)
        {
            var result = graph.Dijkstra<Station, LinePart>(from, to);

            if(!result.IsFounded)
            {
                throw new InvalidOperationException("Wrong graph, you must come to every station from any station");
            }

            var pathStationIds = result.GetPath();


        }
    }
}
