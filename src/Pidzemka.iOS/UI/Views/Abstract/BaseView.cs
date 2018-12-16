using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;

namespace Pidzemka.iOS.UI.Views.Abstract
{
    public abstract class BaseView<TViewModel> : MvxViewController<TViewModel> where TViewModel : class, IMvxViewModel
    {
        protected BaseView(System.IntPtr handle) : base(handle)
        {
        }

        public BaseView(string nibName, Foundation.NSBundle bundle) : base(nibName, bundle)
        {
        }
    }
}