namespace VisualPlus.Extensibility
{
    #region Namespace

    using System.Drawing;
    using System.Windows.Forms;

    #endregion

    public static class StringExtension
    {
        #region Events

        /// <summary>Provides the size, in pixels, of the specified text when drawn with the specified font.</summary>
        /// <param name="text">The text to measure.</param>
        /// <param name="font">The font to apply to the measured text.</param>
        /// <returns>Measured text size.</returns>
        public static Size MeasureText(this string text, Font font)
        {
            return TextRenderer.MeasureText(text, font);
        }

        #endregion
    }
}