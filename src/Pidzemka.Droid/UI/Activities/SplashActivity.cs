using Android.App;
using Android.Content.PM;
using MvvmCross.Platforms.Android.Views;

namespace Pidzemka.Droid.UI.Activities
{
    [Activity(
        Label = "@string/ApplicationName"
        , MainLauncher = true
        , Icon = "@mipmap/ic_launcher"
        , Theme = "@style/Theme.Splash"
        , NoHistory = true
        , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : MvxSplashScreenActivity
    {
        public SplashActivity() : base(Resource.Layout.activity_splash)
        {
        }
    }
}