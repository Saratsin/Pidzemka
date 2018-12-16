using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pidzemka.Models;
using Pidzemka.Models.Dto;

namespace Pidzemka.Managers
{
    public class RouteManager
    {
        private static readonly Lazy<RouteManager> lazyInstance = new Lazy<RouteManager>(() => new RouteManager());

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

            await Task.Run(() =>
            {
                var dataResourceName = $"Pidzemka.Resources.data_{cityName}.json";
                var mapSvgResourceName = $"Pidzemka.Resources.map_{cityName}.svg";

                var assembly = typeof(CrossApp).Assembly;

                using (var stream = assembly.GetManifestResourceStream(dataResourceName))
                using (var streamReader = new StreamReader(stream))
                {
                    var dataJsonString = streamReader.ReadToEnd();
                    var mapDto = JsonConvert.DeserializeObject<MapDto>(dataJsonString);
                    Map = new Map(mapDto);
                    LatestClosestStationId = Map.DefaultStationId;
                }

                using (var stream = assembly.GetManifestResourceStream(mapSvgResourceName))
                using (var streamReader = new StreamReader(stream))
                {
                    MapSvg = streamReader.ReadToEnd();
                }

            }).ConfigureAwait(false);

            isInitialized = true;
        }

        public int LatestClosestStationId { get; private set; }

        public Task<int> GetCurrentClosestStation()
        {
            return Task.FromResult(LatestClosestStationId);
        }

        public Route CreateDummyRoute()
        {
            return Map.FindRoute(310, 220);
        }
    }
}