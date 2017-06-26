namespace VisualPlus.Framework.Handlers
{
    #region Namespace

    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using VisualPlus.Controls;

    #endregion

    internal class ExceptionManager
    {
        #region Events

        /// <summary>Apply BackColor change on the container and it's child controls.</summary>
        /// <param name="container">The container control.</param>
        /// <param name="backgroundColor">The container backgroundColor.</param>
        public static void ApplyContainerBackColorChange(Control container, Color backgroundColor)
        {
            foreach (object control in container.Controls)
            {
                if (control != null)
                {
                    ((Control)control).BackColor = backgroundColor;
                }
            }
        }

        /// <summary>Returns a bool indicating whether the value is in range.</summary>
        /// <param name="sourceValue">The main value.</param>
        /// <param name="minimumValue">Minimum value.</param>
        /// <param name="maximumValue">Maximum value.</param>
        /// <returns>Bool value.</returns>
        public static bool ArgumentOutOfRangeException(int sourceValue, int minimumValue, int maximumValue)
        {
            if (sourceValue >= minimumValue && sourceValue <= maximumValue)
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

        /// <summary>Set's the container controls BackColor.</summary>
        /// <param name="control">Current control.</param>
        /// <param name="backgroundColor">Container background color.</param>
        /// <param name="onControlRemoved">Control removed?</param>
        public static void SetControlBackColor(Control control, Color backgroundColor, bool onControlRemoved)
        {
            Color backColor;

            if (onControlRemoved)
            {
                backColor = Color.Transparent;

                // Bug: The Control doesn't support transparent background
                if (control is VisualProgressIndicator)
                {
                    backColor = SystemColors.Control;
                }
            }
            else
            {
                backColor = backgroundColor;
            }

            control.BackColor = backColor;
        }

        #endregion
    }
}