using System;
using System.Collections.Generic;
using System.Drawing;
using Pidzemka.Extensions;
using Pidzemka.Models.Dto;
using System.Linq;

namespace Pidzemka.Models
{
    public class Station : IEquatable<Station>
    {
        public Station(StationDto dto)
        {
            Id = dto.Id;
            MapCoordinates = dto.MapCoordinates;
            Latitude = dto.Latitude;
            Longitude = dto.Longitude;
        }

        public int Id { get; }

        public PointF MapCoordinates { get; }

        public double Latitude { get; }

        public double Longitude { get; }

        public IReadOnlyDictionary<Station, TimeSpan> NearestStationsDistances { get; private set; }

        public TimeSpan GetNearestStationDistance(int stationId)
        {
            var key = NearestStationsDistances.Keys.First(station => station.Id == stationId);

            return NearestStationsDistances[key];
        }

        public void InitializeNearestStations(IDictionary<Station, TimeSpan> nearestStationsDistances)
        {
            this.NearestStationsDistances = nearestStationsDistances.AsReadOnly();
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
