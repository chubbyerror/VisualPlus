namespace VisualPlus.Managers
{
    #region Namespace

    using System;

    #endregion

    internal class ExceptionManager
    {
        #region Events

        /// <summary>Returns a bool indicating whether the value is in range.</summary>
        /// <param name="sourceValue">The main value.</param>
        /// <param name="minimumValue">Minimum value.</param>
        /// <param name="maximumValue">Maximum value.</param>
        /// <returns>Bool value.</returns>
        public static bool ArgumentOutOfRangeException(int sourceValue, int minimumValue, int maximumValue)
        {
            if ((sourceValue >= minimumValue) && (sourceValue <= maximumValue))
            {
                // Value in range
                return true;
            }
            else
            {
                // Value not in range
                throw new ArgumentOutOfRangeException("The value (" + sourceValue + ") must be in range of " + minimumValue + " to " + maximumValue + ".");
            }
        }

        #endregion
    }
}