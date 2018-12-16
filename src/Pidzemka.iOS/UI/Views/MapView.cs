using System;

using UIKit;
using Pidzemka.iOS.UI.Views.Abstract;
using Pidzemka.ViewModels;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using FFImageLoading;
using FFImageLoading.Svg.Platform;

namespace Pidzemka.iOS.UI.Views
{
    [MvxRootPresentation]
    public partial class MapView : BaseView<MapViewModel>
    {
        public MapView() : base(nameof(MapView), null)
        {
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            var assemblyName = typeof(CrossApp).Assembly.FullName;
            var assemblyNameUri = Uri.EscapeUriString(assemblyName);

            var background = await ImageService.Instance
                                               .LoadString(ViewModel.MapSvgData)
                                               .WithCustomDataResolver(ViewModel.MapRouteSvgResolver)
                                               .IntoAsync(MapImageView);
        }
    }
}

