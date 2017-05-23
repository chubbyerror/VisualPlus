namespace VisualPlus.Components.Symbols
{
    #region Namespace

    using System.Drawing;

    #endregion

    public class Arrow
    {
        #region Events

        /// <summary>Draws a arrow.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="pointLocation">Arrow location.</param>
        /// <param name="rectangleSize">Arrow size.</param>
        /// <param name="color">The color.</param>
        /// <param name="fontSize">The font size.</param>
        public static void DrawArrow(Graphics graphics, Point pointLocation, Size rectangleSize, Color color, float fontSize)
        {
            // Create shape
            Rectangle shape = new Rectangle(pointLocation, rectangleSize);

            // Draw arrow
            graphics.DrawString(
                "6",
                new Font("Marlett", fontSize, FontStyle.Regular),
                new SolidBrush(color),
                shape,
                new StringFormat
                    {
                        LineAlignment = StringAlignment.Center,
                        Alignment = StringAlignment.Far
                    });
        }

        #endregion
    }
}