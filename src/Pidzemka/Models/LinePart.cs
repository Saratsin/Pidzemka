using System;

namespace Pidzemka.Models
{
    public class LinePart : IEquatable<LinePart>
    {
        public LinePart(uint startStationId, uint endStationId, TimeSpan timeDistance)
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

        public uint StartStationId { get; }

        public uint EndStationId { get; }
        
        public TimeSpan TimeDistance { get; }

        public bool HasStation(uint stationId)
        {
            return stationId == StartStationId || stationId == EndStationId;
        }

        #region Equality logic
        public override bool Equals(object obj)
        {
            if (!(obj is LinePart other))
            {
                return false;
            }

            return Equals(other);
        }

        public bool Equals(LinePart other)
        {
            return Equals(this, other);
        }

        public override int GetHashCode()
        {
            var hash = 17;

            hash = hash * 31 + (int)StartStationId;
            hash = hash * 31 + (int)EndStationId;

            return hash;
        }

        public static bool operator ==(LinePart left, LinePart right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LinePart left, LinePart right)
        {
            return !Equals(left, right);
        }

        private static bool Equals(LinePart left, LinePart right)
        {
            return left.StartStationId == right.StartStationId && left.EndStationId == right.EndStationId;
        }
        #endregion
    }
}