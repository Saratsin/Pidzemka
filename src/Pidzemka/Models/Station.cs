using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Pidzemka.Extensions;

namespace Pidzemka.Models
{
    public class Station : IEquatable<Station>
    {
        public Station(uint id, PointF coordinates, double latitude, double longitude)
        {
            Id = id;
            Coordinates = coordinates;
            Latitude = latitude;
            Longitude = longitude;
        }

        public uint Id { get; }

        public PointF Coordinates { get; }

        public double Latitude { get; }

        public double Longitude { get; }

        public IReadOnlyDictionary<Station, TimeSpan> NearestStationsDistances { get; private set; }

        public void InitializeNearestStations(IDictionary<Station, TimeSpan> nearestStationDistances)
        {
            NearestStationsDistances = nearestStationDistances.AsReadOnly();
        }

        #region Equality & Hash code
        public override int GetHashCode()
        {
            return (int)Id;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Station other))
                return false;

            return Equals(other);
        }

        public bool Equals(Station other)
        {
            return Equals(this, other);
        }

        public static bool operator ==(Station left, Station right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Station left, Station right)
        {
            return !Equals(left, right);
        }

        private static bool Equals(Station left, Station right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left is null || right is null)
                return false;

            return left.Id == right.Id;
        }
        #endregion
    }
}
