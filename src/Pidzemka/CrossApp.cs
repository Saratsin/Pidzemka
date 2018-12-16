using MvvmCross.ViewModels;
using Pidzemka.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Pidzemka.JsonConverters;

namespace Pidzemka
{
    public class CrossApp : MvxApplication
    {
        public override void Initialize()
        {
            RegisterAppStart<MapViewModel>();

            JsonConvert.DefaultSettings = CreateJsonSettings;
        }

        private JsonSerializerSettings CreateJsonSettings()
        {
            return new JsonSerializerSettings
            {
                Converters =
                {
                    new LinePartJsonConverter(),
                    new StationJsonConverter(),
                    new MapJsonConverter()
                }
            };
        }
    }
}