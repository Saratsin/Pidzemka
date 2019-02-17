using Android.App;
using Android.Util;

namespace Pidzemka.Droid.Common
{
    public static class DisplayUtils
    {
        public static int DpToPx(float dp)
        {
            var displayMetrix = Application.Context.Resources.DisplayMetrics;
            var px = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, displayMetrix);
            return px;
        }
    }
}