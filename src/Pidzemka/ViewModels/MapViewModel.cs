using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using FFImageLoading.Svg.Platform;
using MvvmCross.Commands;
using Pidzemka.Common;
using Pidzemka.Managers;
using Pidzemka.Models;
using Pidzemka.ViewModels.Abstract;

namespace Pidzemka.ViewModels
{
    public class MapViewModel : BaseViewModel
    {
        private MapData mapData;
        private string mapSvgData;
        private string buttonText = "Show route";
        private string fromText;
        private string toText;

        public MapViewModel()
        {
            ButtonClickCommand = new MvxCommand(ButtonClick);

        }

        public override async Task Initialize()
        {
            await RouteManager.Instance.Initialize("kyiv").ConfigureAwait(false);

            MapData = new MapData
            {
                StationsStrokeWidth = 1f,
                StationsRadius = 8.5f,
                MinimumScaleDistance = 200f,
                MapSize = RouteManager.Instance.Map.Size,
                Stations = RouteManager.Instance.Map.StationsCoordinates
            };

            MapSvgData = RouteManager.Instance.MapSvg;
        }

        public MapData MapData
        {
            get => mapData;
            set => SetProperty(ref mapData, value);
        }

        public string MapSvgData
        {
            get => mapSvgData;
            private set => SetProperty(ref mapSvgData, value);
        }

        public SvgDataResolver MapRouteSvgResolver { get; } = new SvgDataResolver();

        public string ButtonText
        {
            get => buttonText;
            set => SetProperty(ref buttonText, value);
        }

        public string FromText
        {
            get => fromText;
            set
            {
                if(SetProperty(ref fromText, value))
                {

                }
            }
        }

        public string ToText
        {
            get => toText;
            set
            {
                if (SetProperty(ref toText, value))
                {

                }
            }
        }

        public ICommand ButtonClickCommand { get; }

        private void ButtonClick()
        {
            if (MapRouteSvgResolver.ReplaceStringMap == null)
            {
                try
                {
                    if(!int.TryParse(FromText, out var fromStationId) || 
                       !int.TryParse(ToText, out var toStationId))
                    {
                        ButtonText = "Error. Again";
                        return;
                    }

                    var route = RouteManager.Instance.Map.FindRoute(fromStationId, toStationId);

                    if(route == null)
                    {
                        ButtonText = "Error. Again";
                        return;
                    }

                    MapRouteSvgResolver.ReplaceStringMap = new Dictionary<string, string>
                    {
                        [$"regex:{route.MapSvgRouteRegex}"] = Constants.SubwayRouteLinePartsReplaceRegex
                    };
                    
                    ButtonText = "Clear route";
                    RaisePropertyChanged(nameof(MapRouteSvgResolver));
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            else
            {
                MapRouteSvgResolver.ReplaceStringMap = null;
                ButtonText = "Show route";
                RaisePropertyChanged(nameof(MapRouteSvgResolver));
            }
        }
    }
}