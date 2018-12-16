using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Input;
using FFImageLoading.Svg.Platform;
using MvvmCross.Commands;
using Pidzemka.Common;
using Pidzemka.Managers;
using Pidzemka.ViewModels.Abstract;

namespace Pidzemka.ViewModels
{
    public class MapViewModel : BaseViewModel
    {
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
            await RaisePropertyChanged(nameof(StationCoordinates)).ConfigureAwait(false);
            await RaisePropertyChanged(nameof(MapWidth)).ConfigureAwait(false);
            await RaisePropertyChanged(nameof(MapHeight)).ConfigureAwait(false);
            await RaisePropertyChanged(nameof(MapSvgData)).ConfigureAwait(false);
        }

        public string MapSvgData => RouteManager.Instance.MapSvg;

        public int MapWidth => RouteManager.Instance.Map.Width;

        public int MapHeight => RouteManager.Instance.Map.Height;

        public IEnumerable<PointF> StationCoordinates => RouteManager.Instance.Map.StationsCoordinates;

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