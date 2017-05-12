namespace VisualPlus.Components.Symbols
{
    using System.Drawing;

    public class Checkmark
    {
        #region ${0} Methods

        /// <summary>Draws a check mark.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="pointLocation">Checkmark location.</param>
        /// <param name="rectangleSize">Checkmark size.</param>
        /// <param name="color">The color.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="fontStyle">The font style.</param>
        public static void DrawCheckmark(Graphics graphics, Point pointLocation, Size rectangleSize, Color color, float fontSize, FontStyle fontStyle = FontStyle.Regular)
        {
            // Create shape
            Rectangle shape = new Rectangle(pointLocation, rectangleSize);

            // Construct font
            using (Font wing = new Font("Wingdings", fontSize, fontStyle))
            {
                // Draw 
                graphics.DrawString("ü", wing, new SolidBrush(color), shape);
            }
        }

        #endregion
    }
}