using System;
using FFImageLoading.Views;
using Android.Runtime;
using Android.Graphics;
using System.Collections.Generic;
using PointF = System.Drawing.PointF;

namespace Pidzemka.Droid.UI.Controls
{
    [Register("Pidzemka.MapImageView")]
    public class MapImageView : ImageViewAsync
    {
        private IEnumerable<PointF> stations;

        public MapImageView(Android.Content.Context context) : base(context)
        {
        }

        public MapImageView(Android.Content.Context context, Android.Util.IAttributeSet attrs) : base(context, attrs)
        {
        }

        public MapImageView(Android.Content.Context context, Android.Util.IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }

        protected MapImageView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public IEnumerable<PointF> StationCoordinates
        {
            get => stations;
            set
            {
                stations = value;
                Invalidate();
            }
        }

        public int RealWidth { get; set; }

        public int RealHeight { get; set; }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if(Drawable == null || StationCoordinates == null)
            {
                return;
            }

            var widthScale = Width / (float)RealWidth;
            var heightScale = Height / (float)RealHeight;

            var paint = new Paint
            {
                StrokeWidth = 2 * widthScale,
                StrokeCap = Paint.Cap.Round
            };

            foreach(var stationCoordinate in StationCoordinates)
            {
                var screenX = stationCoordinate.X * widthScale;
                var screenY = stationCoordinate.Y * heightScale;
                var radius = 8.5f * widthScale;

                paint.SetStyle(Paint.Style.Fill);
                paint.Color = Color.White;

                canvas.DrawCircle(screenX, screenY, radius, paint);

                paint.SetStyle(Paint.Style.Stroke);
                paint.Color = Color.Black;

                canvas.DrawCircle(screenX, screenY, radius, paint);
            }
        }
    }
}
