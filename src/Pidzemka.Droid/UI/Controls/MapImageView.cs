using System;
using FFImageLoading.Views;
using Android.Runtime;
namespace Pidzemka.Droid.UI.Controls
{
    [Register("Pidzemka.MapImageView")]
    public class MapImageView : ImageViewAsync
    {
        public MapImageView(Android.Content.Context context) : base(context)
        {
        }

        public MapImageView(Android.Content.Context context, Android.Util.IAttributeSet attrs) : base(context, attrs)
        {
        }

        public MapImageView(Android.Content.Context context, Android.Util.IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }

        protected MapImageView(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
    }
}
