using MvvmCross.ViewModels;
using Pidzemka.ViewModels;

namespace Pidzemka
{
    public class CrossApp : MvxApplication
    {
        public override void Initialize()
        {
            RegisterAppStart<MapViewModel>();
        }
    }
}