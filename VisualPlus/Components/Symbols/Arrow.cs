namespace VisualPlus.Components.Symbols
{
    using System.Drawing;

    public class Arrow
    {
        #region ${0} Methods

        /// <summary>Draws a arrow.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="pointLocation">Arrow location.</param>
        /// <param name="rectangleSize">Arrow size.</param>
        /// <param name="color">The color.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="fontStyle">The font style.</param>
        public static void DrawArrow(Graphics graphics, Point pointLocation, Size rectangleSize, Color color, float fontSize, FontStyle fontStyle = FontStyle.Regular)
        {
            // Create shape
            Rectangle shape = new Rectangle(pointLocation, rectangleSize);

            graphics.DrawString(
                "6",
                new Font("Marlett", fontSize, fontStyle),
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