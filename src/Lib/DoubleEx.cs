using System;

namespace Logitech
{
    public static class DoubleEx
    {
        #region Constants

        public const double DblEpsilon = 2.2204460492503131E-15;

        #endregion

        #region Queries

        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2) return true;
            var num1 = (Math.Abs(value1) + Math.Abs(value2) + 10.0)*DblEpsilon/10;
            var num2 = value1 - value2;
            return -num1 < num2 && num1 > num2;
        }

        public static bool GreaterThanOrClose(double value1, double value2)
        {
            return !(value1 <= value2) || AreClose(value1, value2);
        }

        public static bool IsZero(double value)
        {
            return Math.Abs(value) < DblEpsilon;
        }

        #endregion
    }
}