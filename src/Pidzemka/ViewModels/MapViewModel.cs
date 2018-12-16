using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FFImageLoading.Svg.Platform;
using Pidzemka.Common;
using Pidzemka.Managers;
using Pidzemka.ViewModels.Abstract;
using MvvmCross.Commands;
using System;
using System.Diagnostics;

namespace Pidzemka.ViewModels
{
    public class MapViewModel : BaseViewModel
    {
        private string buttonText = "Show route";

        public MapViewModel()
        {
            ButtonClickCommand = new MvxCommand(ButtonClick);
        }

        public override async Task Initialize()
        {
            await RouteManager.Instance.Initialize("kyiv").ConfigureAwait(false);
            await RaisePropertyChanged(nameof(MapSvgData)).ConfigureAwait(false);
        }

        public string MapSvgData => RouteManager.Instance.MapSvg;

        public SvgDataResolver MapRouteSvgResolver { get; } = new SvgDataResolver();

        public string ButtonText
        {
            get => buttonText;
            set => SetProperty(ref buttonText, value);
        }

        public ICommand ButtonClickCommand { get; }

        private void ButtonClick()
        {
            if (MapRouteSvgResolver.ReplaceStringMap == null)
            {
                try
                {
                    var route = RouteManager.Instance.CreateDummyRoute();

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