using System;
using Pidzemka.Models;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
namespace Pidzemka.Managers
{
    public class RouteManager
    {
        private static readonly Lazy<RouteManager> lazyInstance = new Lazy<RouteManager>(() =>
        {
            return new RouteManager();
        });

        private bool isInitialized;

        private RouteManager()
        {
        }

        public static RouteManager Instance => lazyInstance.Value;

        public string MapSvg { get; private set; }

        public Map Map { get; private set; }

        public async Task Initialize(string cityName)
        {
            if (isInitialized)
            {
                return;
            }

            var dataResourceName = $"Pidzemka.Resources.data_{cityName}.json";
            var mapSvgResourceName = $"Pidzemka.Resources.map_{cityName}.svg";
         
            var assembly = typeof(CrossApp).Assembly;

            using (var stream = assembly.GetManifestResourceStream(dataResourceName))
            using (var streamReader = new StreamReader(stream))
            {
                await Task.Run(() =>
                {
                    var dataJsonString = streamReader.ReadToEnd();
                    Map = JsonConvert.DeserializeObject<Map>(dataJsonString);
                    LatestClosestStationId = Map.DefaultFromStationId;

                }).ConfigureAwait(false);
            }

            using (var stream = assembly.GetManifestResourceStream(mapSvgResourceName))
            using (var streamReader = new StreamReader(stream))
            {
                await Task.Run(() =>
                {
                    MapSvg = streamReader.ReadToEnd();

                }).ConfigureAwait(false);
            }

            isInitialized = true;
        }

        public int LatestClosestStationId { get; private set; }

        public Task<int> GetCurrentClosestStation()
        {
            return Task.FromResult(LatestClosestStationId);
        }

        public Route CreateDummyRoute()
        {
            var lineParts = new List<LinePart>
            {
                new LinePart(310, 311, TimeSpan.Zero),
                new LinePart(311, 312, TimeSpan.Zero),
                new LinePart(312, 314, TimeSpan.Zero),
                new LinePart(314, 119, TimeSpan.Zero),
                new LinePart(119, 118, TimeSpan.Zero),
                new LinePart(118, 117, TimeSpan.Zero),
                new LinePart(117, 116, TimeSpan.Zero),
            };

            return new Route(new List<Station>(), lineParts);
        }
    }
}