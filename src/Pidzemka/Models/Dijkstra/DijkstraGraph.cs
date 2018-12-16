using System;
using System.Collections.Generic;
using System.Linq;

namespace Pidzemka.Models.Dijkstra
{
    public static class DijkstraGraph
    {
        public static List<Station> FindShortestPath(List<Station> allStationsWithNearestStationsDistances, 
                                                     Station fromStation,
                                                     Station toStation)
        {
            var connections = allStationsWithNearestStationsDistances.ToDictionary(
                station => station,
                station => station.NearestStationsDistances.Keys.ToList());

            bool EqualityFunction(Station left, Station right) => left == right;

            double DistanceFunction(Station left, Station right) => left.NearestStationsDistances[right].TotalSeconds;

            return FindShortestPath(
                connections,
                fromStation,
                toStation,
                EqualityFunction,
                DistanceFunction);
        }

        /// <summary>
        /// Calculates the shortest route from a source node to a target node given a set of nodes and connections. 
        /// Will only work for graphs with non-negative path weights
        /// </summary>
        /// <param name="connections">All the nodes, as well as the list of their connections</param>
        /// <param name="sourceNode">The node to start from</param>
        /// <param name="targetNode">The node we should seek</param>
        /// <param name="equalsFunction">A function used for testing if two nodes are equal</param>
        /// <param name="distanceFunction">A function used for calculating the distance/weight between two nodes</param>
        /// <returns>
        /// An ordered list of nodes from source->target giving the shortest path from the source to the target node. 
        /// Returns null if no path is possible
        /// </returns>
        public static List<TNode> FindShortestPath<TNode>(IDictionary<TNode, List<TNode>> connections, 
                                                          TNode sourceNode, 
                                                          TNode targetNode, 
                                                          Func<TNode, TNode, bool> equalsFunction, 
                                                          Func<TNode, TNode, double> distanceFunction)
        {
            // Initialize values
            var distance = new Dictionary<TNode, double>();
            var previous = new Dictionary<TNode, TNode>();
            var localNodes = new List<TNode>();

            // For all nodes, copy it to our local list as well as set it's distance to null as it's unknown
            foreach (var node in connections.Keys)
            {
                localNodes.Add(node);
                distance.Add(node, double.PositiveInfinity);
            }

            // We know the distance from source->source is 0 by definition
            distance[sourceNode] = 0;

            while (localNodes.Count > 0)
            {
                // Return and remove best vertex (that is, connection with minimum distance
                var minNode = localNodes.OrderBy(n => distance[n]).First();
                localNodes.Remove(minNode);

                // Loop all connected nodes
                foreach (var neighbor in connections[minNode])
                {
                    // The positive distance between node and it's neighbor, added to the distance of the current node
                    var dist = distance[minNode] + distanceFunction(minNode, neighbor);

                    if (dist < distance[neighbor])
                    {
                        distance[neighbor] = dist;
                        previous[neighbor] = minNode;
                    }
                }

                // If we're at the target node, break
                if (equalsFunction(minNode, targetNode))
                {
                    break;
                }
            }

            // Construct a list containing the complete path. We'll start by looking 
            // at the previous node of the target and then making our way to the beginning.
            // We'll reverse it to get a source->target list instead of the other way around. 
            // The source node is manually added.
            var result = new List<TNode>();
            var target = targetNode;

            while (previous.ContainsKey(target))
            {
                result.Add(target);
                target = previous[target];
            }

            result.Add(sourceNode);
            result.Reverse();

            if (result.Count < 2)
            {
                return null;
            }

            return result;
        }
    }
}
