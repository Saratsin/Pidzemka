using Android.Graphics;
using Pidzemka.Droid.Common;
using Java.Util;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Pidzemka.Droid.Extensions
{
    public static class UIExtensions
    {
        public static Dictionary<MatrixValue, float> GetValues(this Matrix matrix)
        {
            var values = new float[9];
            matrix.GetValues(values);
            var dictionary = Enumerable.Range(0, 9).ToDictionary(
                index => (MatrixValue)index, 
                index => values[index]);

            return dictionary;
        }

        public static float GetValue(this Matrix matrix, MatrixValue valueName)
        {
            var values = new float[9];
            matrix.GetValues(values);
            var value = values[(int)valueName];

            return value;
        }

        public static float Normalize(this float value, float minValue, float maxValue)
        {
            if (value < minValue)
                return minValue;

            if (value > maxValue)
                return maxValue;

            return value;
        }
    }
}
