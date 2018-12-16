using System;
using Pidzemka.Models.Dto;

namespace Pidzemka.Models
{
    public class Node : IEquatable<Node>
    {
        public Node(int startStationId, int endStationId, TimeSpan timeDistance)
        {
            if (startStationId < endStationId)
            {
                StartStationId = startStationId;
                EndStationId = endStationId;
            }
            else
            {
                StartStationId = endStationId;
                EndStationId = startStationId;
            }

            TimeDistance = timeDistance;
        }

        public Node(NodeDto dto) 
            : this(dto.StartStationId, dto.EndStationId, dto.TimeDistance)
        {
        }

        public int StartStationId { get; }

        public int EndStationId { get; }
        
        public TimeSpan TimeDistance { get; }

        public bool HasStation(int stationId)
        {
            return stationId == StartStationId || stationId == EndStationId;
        }

        #region Equality logic
        public override bool Equals(object obj)
        {
            if (!(obj is Node other))
            {
                return false;
            }

            return Equals(other);
        }

        public bool Equals(Node other)
        {
            return Equals(this, other);
        }

        public override int GetHashCode()
        {
            var hash = 17;

            hash = hash * 31 + StartStationId;
            hash = hash * 31 + EndStationId;

            return hash;
        }

        public static bool operator ==(Node left, Node right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Node left, Node right)
        {
            return !Equals(left, right);
        }

        private static bool Equals(Node left, Node right)
        {
            return left.StartStationId == right.StartStationId && left.EndStationId == right.EndStationId;
        }
        #endregion
    }
}