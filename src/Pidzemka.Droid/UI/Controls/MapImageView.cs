using System;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using FFImageLoading.Views;
using Pidzemka.Droid.Common;
using Pidzemka.Droid.Extensions;
using Pidzemka.Models;
using PointF = System.Drawing.PointF;

namespace Pidzemka.Droid.UI.Controls
{
    /// <summary>
    /// Map image view.
    /// It's assumed that width of this view is always smaller than its height
    /// </summary>
    [Register("Pidzemka.MapImageView")]
    public class MapImageView : ImageViewAsync
    {
        private enum TouchMode
        {
            None,
            Drag,
            Zoom,
            DoubleTap,
            DoubleTapZoom,
            LongTap
        }

        private static readonly int fingersDistanceThreshold = 10;

        private static readonly int doubleTapZoomDistanceThreshold = DisplayUtils.DpToPx(32f);
        private static readonly TimeSpan doubleTouchDelay = TimeSpan.FromMilliseconds(200);

        private bool isDrawingFirstTime = true;
        private DateTimeOffset lastPointerDownActionTime = DateTimeOffset.MinValue;

        // These matrices will be used to move and zoom image
        private Matrix currentMatrix = new Matrix();
        private Matrix initialMatrix = new Matrix();

        private TouchMode currentTouchMode = TouchMode.None;

        // Remember some things for zooming
        private PointF initialTouchPoint = new PointF();
        private PointF currentScalePoint = new PointF();
      
        private float initialDistance = 1f;

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

        public MapData MapData { get; set; }

        public float MaxRelativeScreenOffset { get; set; } = 0.1f;

        protected float MinimumScale
        {
            get
            {
                var screenWidth = Width;
                var screenOffsetMultiplier = 1 / (1 - MaxRelativeScreenOffset * 2);
                var imageWidth = (float)Drawable.IntrinsicWidth;
                var imageWidthWithOffset = imageWidth * screenOffsetMultiplier;

                return screenWidth / imageWidthWithOffset;
            }
        }

        protected float MediumScale => (MinimumScale + MaximumScale) / 3f;

        protected float MaximumScale
        {
            get
            {
                var screenWidth = Width;
                var imageMinimumScaleDistance = ToImageValue(MapData.MinimumScaleDistance);

                var maximumScale = screenWidth / imageMinimumScaleDistance;
                return maximumScale;
            }
        }

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
            if (Drawable == null || MapData?.Stations == null)
            {
                return;
            }

            if (isDrawingFirstTime)
            {
                ScaleMatrix(currentMatrix, MinimumScale, currentScalePoint);
                ImageMatrix = currentMatrix;
                isDrawingFirstTime = false;
            }

            base.OnDraw(canvas);

            var imageValues = currentMatrix.GetValues();

            var stationStrokeWidth = ToImageValue(MapData.StationsStrokeWidth) * imageValues[MatrixValue.ScaleX];
            var stationRadius = ToImageValue(MapData.StationsRadius) * imageValues[MatrixValue.ScaleX];

            var paint = new Paint
            {
                StrokeWidth = stationStrokeWidth,
                StrokeCap = Paint.Cap.Round
            };

            foreach (var stationCoordinate in MapData.Stations)
            {
                var mappedStationCoordinate = MapPoint(stationCoordinate);

                var screenX = mappedStationCoordinate.X;
                var screenY = mappedStationCoordinate.Y;

                paint.SetStyle(Paint.Style.Fill);
                paint.Color = Color.White;

                canvas.DrawCircle(screenX, screenY, stationRadius, paint);

                paint.SetStyle(Paint.Style.Stroke);
                paint.Color = Color.Black;

                canvas.DrawCircle(screenX, screenY, stationRadius - stationStrokeWidth / 2f, paint);
            }
        }

        private PointF MapPoint(PointF point)
        {
            var imageX = ToImageValue(point.X);
            var imageY = ToImageValue(point.Y);

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
                    var currentTime = DateTimeOffset.Now;
                    var intervalFromLastTouch = currentTime - lastPointerDownActionTime;
                    var isDoubleTouch = intervalFromLastTouch < doubleTouchDelay;
                    if (isDoubleTouch)
                    {
                        currentTouchMode = TouchMode.DoubleTap;
                        currentScalePoint = initialTouchPoint;
                        break;
                    }
                    lastPointerDownActionTime = currentTime;
                    initialMatrix.Set(currentMatrix);
                    initialTouchPoint = new PointF(e.GetX(), e.GetY());
                    currentTouchMode = TouchMode.Drag;
                    break;
                case MotionEventActions.PointerDown:
                    initialDistance = GetDistanceBetweenFingers(e);
                    if (initialDistance > fingersDistanceThreshold) 
                    {
                        initialMatrix.Set(currentMatrix);
                        currentScalePoint = GetMidPointBetweenFingers(e);
                        currentTouchMode = TouchMode.Zoom;
                    }
                    break;
                case MotionEventActions.Up when currentTouchMode == TouchMode.DoubleTap:
                    var resetScale = GetClosestResetScale(currentMatrix);
                    ScaleMatrix(currentMatrix, resetScale, currentScalePoint);
                    currentTouchMode = TouchMode.None;
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                    currentTouchMode = TouchMode.None;
                    break;
                case MotionEventActions.Move when currentTouchMode == TouchMode.DoubleTap:
                    currentTouchMode = TouchMode.DoubleTapZoom;
                    break;
                case MotionEventActions.Move when currentTouchMode == TouchMode.DoubleTapZoom:
                    {
                        var distanceY = e.GetY() - initialTouchPoint.Y;
                        currentMatrix.Set(initialMatrix);

                        var rawScale = (distanceY + doubleTapZoomDistanceThreshold) / doubleTapZoomDistanceThreshold;

                        if (distanceY < 0)
                        {
                            rawScale = doubleTapZoomDistanceThreshold / (-distanceY + doubleTapZoomDistanceThreshold);
                        }

                        var scale = GetScaleValue(rawScale);
                        currentMatrix.PostScale(scale, scale, initialTouchPoint.X, initialTouchPoint.Y);
                        (var correctTransX, var correctTransY) = CorrectTranslateValues(initialTouchPoint.X, initialTouchPoint.Y);
                        currentMatrix.PostTranslate(correctTransX, correctTransY);
                        break;
                    }
                case MotionEventActions.Move when currentTouchMode == TouchMode.Drag:
                    currentMatrix.Set(initialMatrix);
                    (var transX, var transY) = GetTranslateValues(e.GetX(), e.GetY(), initialTouchPoint.X, initialTouchPoint.Y);
                    currentMatrix.PostTranslate(transX, transY);
                    break;
                case MotionEventActions.Move when currentTouchMode == TouchMode.Zoom:
                    var currentDistance = GetDistanceBetweenFingers(e);
                    if (currentDistance > fingersDistanceThreshold) 
                    {
                        currentMatrix.Set(initialMatrix);
                        var rawScale = currentDistance / initialDistance;
                        var scale = GetScaleValue(rawScale);
                        currentMatrix.PostScale(scale, scale, currentScalePoint.X, currentScalePoint.Y);
                        (var correctTransX, var correctTransY) = CorrectTranslateValues(initialTouchPoint.X, initialTouchPoint.Y);
                        currentMatrix.PostTranslate(correctTransX, correctTransY);
                    }
                    break;
            }

            ImageMatrix = currentMatrix;
            return true;
        }

        private void ScaleMatrix(Matrix matrix, float scale, PointF scalePoint)
        {
            var matrixValues = matrix.GetValues();
            var rawScale = scale / matrixValues[MatrixValue.ScaleX];
            var correctScale = GetScaleValue(rawScale);
            matrix.PostScale(correctScale, correctScale, scalePoint.X, scalePoint.Y);

            matrixValues = matrix.GetValues();
            (var transX, var transY) = CorrectTranslateValues(matrixValues[MatrixValue.TransX], matrixValues[MatrixValue.TransY]);
            currentMatrix.PostTranslate(transX, transY);
        }

        private (float correctedX, float correctedY) CorrectTranslateValues(float translateX, float translateY)
        {
            return GetTranslateValues(translateX, translateY, translateX, translateY);
        }

        private (float translateX, float translateY) GetTranslateValues(float newX, float newY, float oldX, float oldY)
        {
            var matrixValues = currentMatrix.GetValues();
            var startTranslateX = matrixValues[MatrixValue.TransX];
            var startTranslateY = matrixValues[MatrixValue.TransY];

            var estimatedTranslateX = newX - oldX;
            var estimatedTranslateY = newY - oldY;

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

        private float GetClosestResetScale(Matrix matrix)
        {
            var matrixValues = matrix.GetValues();
            var currentScale = Math.Max(matrixValues[MatrixValue.ScaleX], matrixValues[MatrixValue.ScaleY]);

            if (currentScale >= MaximumScale)
                return MinimumScale;

            if (currentScale >= MediumScale)
                return MaximumScale;

            return MediumScale;
        }

        private float GetScaleValue(float rawScale)
        {
            var currentScaleX = currentMatrix.GetValue(MatrixValue.ScaleX);
            var currentScaleY = currentMatrix.GetValue(MatrixValue.ScaleY);
            var futureScaleX = currentScaleX * rawScale;
            var futureScaleY = currentScaleY * rawScale;

            var correctedFutureScaleX = Math.Max(futureScaleX, MinimumScale);
            var correctedFutureScaleY = Math.Max(futureScaleY, MinimumScale);
            var correctedScaleX = correctedFutureScaleX / currentScaleX;
            var correctedScaleY = correctedFutureScaleY / currentScaleY;

            var correctedScale = Math.Min(correctedScaleX, correctedScaleY);

            var maxScaleX = MaximumScale / currentScaleX;
            var maxScaleY = MaximumScale / currentScaleY;
            var maxScale = Math.Max(maxScaleX, maxScaleY);

            correctedScale = Math.Min(correctedScale, maxScale);

            return correctedScale;
        }

        private float ToImageValue(float mapDataValue)
        {
            var mapDataWidth = MapData.MapSize.Width;
            var imageWidth = Drawable.IntrinsicWidth;
            var multiplier = imageWidth / mapDataWidth;

            var imageValue = mapDataValue * multiplier;
            return imageValue;
        }
    }
}