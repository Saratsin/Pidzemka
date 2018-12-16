using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading;
using FFImageLoading.Work;
using MvvmCross.Binding.BindingContext;
using Pidzemka.Droid.UI.Activities.Abstract;
using Pidzemka.Droid.UI.Controls;
using Pidzemka.ViewModels;

namespace Pidzemka.Droid.UI.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class MapActivity : BaseActivity<MapViewModel>
    {
        protected override int LayoutResource => Resource.Layout.activity_map;

        private MapImageView mapImage;

        public MapActivity()
        {
        }

        public IDataResolver MapResolver
        {
            set => InvalidateMap();
        }

        public string MapSvgData
        {
            set => InvalidateMap();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            mapImage = FindViewById<MapImageView>(Resource.Id.map_image);

            var set = this.CreateBindingSet<MapActivity, MapViewModel>();

            set.Bind(this).For(nameof(MapResolver)).To(vm => vm.MapRouteSvgResolver);
            set.Bind(this).For(nameof(MapSvgData)).To(vm => vm.MapSvgData);
            set.Bind(mapImage).For(v => v.RealWidth).To(vm => vm.MapWidth);
            set.Bind(mapImage).For(v => v.RealHeight).To(vm => vm.MapHeight);
            set.Bind(mapImage).For(v => v.StationCoordinates).To(vm => vm.StationCoordinates);

            set.Apply();
        }

        private Task InvalidateMap()
        {
            if(ViewModel.MapSvgData == null || ViewModel.MapRouteSvgResolver == null)
            {
                return Task.FromResult(true);
            }

            return ImageService.Instance
                               .LoadString(ViewModel.MapSvgData)
                               .WithCustomDataResolver(ViewModel.MapRouteSvgResolver)
                               .IntoAsync(mapImage);
        }
    }
}