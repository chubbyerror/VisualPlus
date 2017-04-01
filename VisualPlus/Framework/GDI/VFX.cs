namespace VisualPlus.Framework.GDI
{
    using System;

    internal class AnimationLinear
    {
        #region ${0} Methods

        public static double CalculateProgress(double progress)
        {
            return progress;
        }

        #endregion
    }

    internal class AnimationEaseInOut
    {
        #region  ${0} Variables

        public static double Pi = Math.PI;
        public static double PiHalf = Math.PI / 2;

        #endregion

        #region ${0} Events

        private static double EaseInOut(double s)
        {
            return s - Math.Sin(s * 2 * Pi / (2 * Pi));
        }

        #endregion

        #region ${0} Methods

        public static double CalculateProgress(double progress)
        {
            return EaseInOut(progress);
        }

        #endregion
    }

    public static class AnimationEaseOut
    {
        #region ${0} Methods

        public static double CalculateProgress(double progress)
        {
            return -1 * progress * (progress - 2);
        }

        #endregion
    }

    public static class AnimationCustomQuadratic
    {
        #region ${0} Methods

        public static double CalculateProgress(double progress)
        {
            const double Boost = 0.6;
            return 1 - Math.Cos((Math.Max(progress, Boost) - Boost) * Math.PI / (2 - 2 * Boost));
        }

        #endregion
    }
}