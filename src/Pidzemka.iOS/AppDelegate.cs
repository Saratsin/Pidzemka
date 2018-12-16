using Foundation;
using MvvmCross.Platforms.Ios.Core;

namespace Pidzemka.iOS
{
    [Register(nameof(AppDelegate))]
    public class AppDelegate : MvxApplicationDelegate<Setup, CrossApp>
    {
    }
}
