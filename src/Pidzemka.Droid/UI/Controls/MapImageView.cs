using System;
using FFImageLoading.Views;
using Android.Runtime;
using Android.Graphics;
using System.Collections.Generic;
using PointF = System.Drawing.PointF;
using Android.Views;
using Pidzemka.Droid.Extensions;
using Pidzemka.Droid.Common;

namespace Pidzemka.Droid.UI.Controls
{
    [Register("Pidzemka.MapImageView")]
    public class MapImageView : ImageViewAsync
    {
        private enum TouchMode
        {
            None,
            Drag,
            Zoom
        }

        private const float FingersDistanceThreshold = 10f;

        private bool isDrawingFirstTime = true;

        private IEnumerable<PointF> stations;

        // These matrices will be used to move and zoom image
        private Matrix currentMatrix = new Matrix();
        private Matrix initialMatrix = new Matrix();

        private TouchMode currentTouchMode = TouchMode.None;

        // Remember some things for zooming
        private PointF touchStartPoint = new PointF();
        private PointF scalePoint = new PointF();
      
        float initialDistance = 1f;

        public MapImageView(Android.Content.Context context) : base(context)
        {
            Initialize();
        }

        public MapImageView(Android.Content.Context context, Android.Util.IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        public MapImageView(Android.Content.Context context, Android.Util.IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize();
        }

        protected MapImageView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Initialize();
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

        public float MinScaleDistance { get; set; } = 300f;

        public float MaxRelativeScreenOffset { get; set; } = 0.2f;

        protected float GetMinPossibleTranslateX(Matrix matrix)
        {
            var scaleX = matrix.GetValue(MatrixValue.ScaleX);
            var currentDrawableWidth = Drawable.IntrinsicWidth * scaleX;
            var minPossibleCurrentWidthOffset = Math.Max(0f, (Width - currentDrawableWidth) / 2f);
            var maxAbsoluteScreenWidthOffset = Math.Max(Width * MaxRelativeScreenOffset, minPossibleCurrentWidthOffset);

            var minPossibleTranslateX = Width - currentDrawableWidth - maxAbsoluteScreenWidthOffset;
            return minPossibleTranslateX;
        }

        protected float GetMinPossibleTranslateY(Matrix matrix)
        {
            var scaleY = matrix.GetValue(MatrixValue.ScaleY);
            var currentDrawableHeight = Drawable.IntrinsicHeight * scaleY;
            var minPossibleCurrentHeightOffset = Math.Max(0f, (Height - currentDrawableHeight) / 2f);
            var maxAbsoluteScreenHeightOffset = Math.Max(Height * MaxRelativeScreenOffset, minPossibleCurrentHeightOffset);

            var minPossibleTranslateY = Height - currentDrawableHeight - maxAbsoluteScreenHeightOffset;
            return minPossibleTranslateY;
        }

        protected float GetMaxPossibleTranslateX(Matrix matrix)
        {
            var scaleX = matrix.GetValue(MatrixValue.ScaleX);
            var currentDrawableWidth = Drawable.IntrinsicWidth * scaleX;
            var minPossibleCurrentWidthOffset = Math.Max(0f, (Width - currentDrawableWidth) / 2f);
            var maxAbsoluteScreenWidthOffset = Math.Max(Width * MaxRelativeScreenOffset, minPossibleCurrentWidthOffset);

            var maxPossibleTranslateX = maxAbsoluteScreenWidthOffset;
            return maxPossibleTranslateX;
        }

        protected float GetMaxPossibleTranslateY(Matrix matrix)
        {
            var scaleX = matrix.GetValue(MatrixValue.ScaleY);
            var currentDrawableHeight = Drawable.IntrinsicHeight * scaleX;
            var minPossibleCurrentHeightOffset = Math.Max(0f, (Height - currentDrawableHeight) / 2f);
            var maxAbsoluteScreenHeightOffset = Math.Max(Height * MaxRelativeScreenOffset, minPossibleCurrentHeightOffset);

            var maxPossibleTranslateY = maxAbsoluteScreenHeightOffset;
            return maxPossibleTranslateY;
        }

        protected void Initialize()
        {

        }

        protected override void OnDraw(Canvas canvas)
        {
            if (isDrawingFirstTime && Drawable != null)
            {
                var scale = Width / (float)Drawable.IntrinsicWidth;
                var scaledHeight = Drawable.IntrinsicHeight * scale;
                var startY = (Height - scaledHeight) / 2;

                currentMatrix.Set(initialMatrix);
                currentMatrix.PostScale(scale, scale);
                currentMatrix.PostTranslate(0, startY);

                ImageMatrix = currentMatrix;

                isDrawingFirstTime = false;
            }

            base.OnDraw(canvas);

            if (Drawable == null || StationCoordinates == null)
            {
                return;
            }

            var imageValues = new float[9];
            currentMatrix.GetValues(imageValues);

            var widthScale = imageValues[Matrix.MscaleX] * Drawable.IntrinsicWidth / RealWidth;
            var heightScale = imageValues[Matrix.MscaleY] * Drawable.IntrinsicHeight / RealHeight;

            var paint = new Paint
            {
                StrokeWidth = widthScale,
                StrokeCap = Paint.Cap.Round
            };

            foreach (var stationCoordinate in StationCoordinates)
            {
                var mappedStationCoordinate = MapPoint(stationCoordinate);

                var screenX = mappedStationCoordinate.X;
                var screenY = mappedStationCoordinate.Y;

                var radius = 8.5f * widthScale;

                paint.SetStyle(Paint.Style.Fill);
                paint.Color = Color.White;

                canvas.DrawCircle(screenX, screenY, radius, paint);

                paint.SetStyle(Paint.Style.Stroke);
                paint.Color = Color.Black;

                canvas.DrawCircle(screenX, screenY, radius - widthScale / 2, paint);
            }
        }

        private PointF MapPoint(PointF point)
        {
            var imageX = point.X / RealWidth * Drawable.IntrinsicWidth;
            var imageY = point.Y / RealHeight * Drawable.IntrinsicHeight;

            var pointArray = new[] { imageX, imageY };
            ImageMatrix.MapPoints(pointArray);
            var mappedPoint = new PointF(pointArray[0], pointArray[1]);
            return mappedPoint;
        }

        private float GetDistanceBetweenFingers(MotionEvent e)
        {
            var x = e.GetX(0) - e.GetX(1);
            var y = e.GetY(0) - e.GetY(1);
            return (float)Math.Sqrt(x * x + y * y);
        }

        private PointF GetMidPointBetweenFingers(MotionEvent e)
        {
            float x = (e.GetX(0) + e.GetX(1)) / 2f;
            float y = (e.GetY(0) + e.GetY(1)) / 2f;
            return new PointF(x, y);
        }


        public override bool OnTouchEvent(MotionEvent e)
        {
            // Handle touch events here...
            switch (e.Action & MotionEventActions.Mask)
            {
                case MotionEventActions.Down:
                    initialMatrix.Set(currentMatrix);
                    touchStartPoint = new PointF(e.GetX(), e.GetY());
                    currentTouchMode = TouchMode.Drag;
                    break;
                case MotionEventActions.PointerDown:
                    initialDistance = GetDistanceBetweenFingers(e);
                    if (initialDistance > FingersDistanceThreshold) 
                    {
                        initialMatrix.Set(currentMatrix);
                        scalePoint = GetMidPointBetweenFingers(e);
                        currentTouchMode = TouchMode.Zoom;
                    }
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                    currentTouchMode = TouchMode.None;
                    break;
                case MotionEventActions.Move when currentTouchMode == TouchMode.Drag:
                    currentMatrix.Set(initialMatrix);
                    (var transX, var transY) = GetTranslateValues(e.GetX(), e.GetY());
                    currentMatrix.PostTranslate(transX, transY);
                    break;
                case MotionEventActions.Move when currentTouchMode == TouchMode.Zoom:
                    var currentDistance = GetDistanceBetweenFingers(e);
                    if (currentDistance > FingersDistanceThreshold) 
                    {
                        currentMatrix.Set(initialMatrix);
                        var scale = GetScaleValue(currentDistance);
                        currentMatrix.PostScale(scale, scale, scalePoint.X, scalePoint.Y);
                        (var correctTransX, var correctTransY) = GetTranslateValues(touchStartPoint.X, touchStartPoint.Y);
                        currentMatrix.PostTranslate(correctTransX, correctTransY);
                    }
                    break;
            }

            ImageMatrix = currentMatrix;
            return true;
        }

        private (float translateX, float translateY) GetTranslateValues(float newX, float newY)
        {
            var matrixValues = currentMatrix.GetValues();
            var startTranslateX = matrixValues[MatrixValue.TransX];
            var startTranslateY = matrixValues[MatrixValue.TransY];

            var estimatedTranslateX = newX - touchStartPoint.X;
            var estimatedTranslateY = newY - touchStartPoint.Y;

            var totalTranslateX = startTranslateX + estimatedTranslateX;
            var totalTranslateY = startTranslateY + estimatedTranslateY;

            var minTranslateX = GetMinPossibleTranslateX(currentMatrix);
            var maxTranslateX = GetMaxPossibleTranslateX(currentMatrix);
            var minTranslateY = GetMinPossibleTranslateY(currentMatrix);
            var maxTranslateY = GetMaxPossibleTranslateY(currentMatrix);

            var normalizedTotalTranslateX = totalTranslateX.Normalize(minTranslateX, maxTranslateX);
            var normalizedTotalTranslateY = totalTranslateY.Normalize(minTranslateY, maxTranslateY);

            var translateX = normalizedTotalTranslateX - startTranslateX;
            var translateY = normalizedTotalTranslateY - startTranslateY;

            return (translateX, translateY);
        }

        private float GetScaleValue(float currentDistance)
        {
            var scale = currentDistance / initialDistance;
            var currentScaleX = currentMatrix.GetValue(MatrixValue.ScaleX);
            var currentScaleY = currentMatrix.GetValue(MatrixValue.ScaleY);
            var futureScaleX = currentScaleX * scale;
            var futureScaleY = currentScaleY * scale;
            var minScaleX = Width / (float)Drawable.IntrinsicWidth / 1.1f;
            var minScaleY = Height / (float)Drawable.IntrinsicHeight / 1.1f;

            var correctedFutureScaleX = Math.Max(futureScaleX, minScaleX);
            var correctedFutureScaleY = Math.Max(futureScaleY, minScaleY);
            var correctedScaleX = correctedFutureScaleX / currentScaleX;
            var correctedScaleY = correctedFutureScaleY / currentScaleY;

            var correctedScale = Math.Min(correctedScaleX, correctedScaleY);

            var maxScaleX = Width / MinScaleDistance / currentScaleX;
            var maxScaleY = Height / MinScaleDistance / currentScaleY;
            var maxScale = Math.Max(maxScaleX, maxScaleY);

            correctedScale = Math.Min(correctedScale, maxScale);

            return correctedScale;
        }
    }
}