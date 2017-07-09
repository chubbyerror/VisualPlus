namespace VisualPlus.Managers
{
    #region Namespace

    using System;

    #endregion

    internal class MathManager
    {
        #region Events

        /// <summary>Converts a degree to a radian.</summary>
        /// <param name="angle">The angle.</param>
        /// <returns>Returns radian.</returns>
        public static float DegreeToRadian(float angle)
        {
            return (float)((angle * Math.PI) / 180);
        }

        /// <summary>Gets the progress fraction.</summary>
        /// <param name="value">Current progress value.</param>
        /// <param name="total">Total bars.</param>
        /// <returns>Progress fraction.</returns>
        public static int GetFactor(double value, double total)
        {
            // Convert to decimal value
            double factor = value / 100;

            // Multiply by amount of bars
            factor = total * factor;

            // Round to fraction
            factor = Math.Round(factor, 0);

            return Convert.ToInt32(factor);
        }

        /// <summary>Converts a radian angle to a degree.</summary>
        /// <param name="angle">The angle.</param>
        /// <returns>Returns degree.</returns>
        public static float RadianToDegree(float angle)
        {
            return (float)(angle * (180.0 / Math.PI));
        }

        #endregion
    }
}