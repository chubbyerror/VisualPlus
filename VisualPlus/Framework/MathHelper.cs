namespace VisualPlus.Framework
{
    using System;

    internal class MathHelper
    {
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
    }
}