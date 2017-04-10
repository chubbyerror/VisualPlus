namespace VisualPlus.Components.Symbols
{
    using System.Drawing;

    public class Bars
    {
        #region ${0} Methods

        /// <summary>Draws bars.</summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="point">The point.</param>
        /// <param name="size">The size.</param>
        /// <param name="color">The color.</param>
        /// <param name="bars">The bars.</param>
        /// <param name="spacing">The spacing.</param>
        public static void DrawBars(Graphics graphics, Point point, Size size, Color color, int bars, int spacing)
        {
            // TODO: Add orientation, auto align in middle (to avoid drawing from top down since size can change depending on # bars.)
            var bump = spacing;
            for (var i = 0; i < bars; i++)
            {
                // Construct bar
                Pen linePen = new Pen(color, 2);

                // X , Y
                Point pt1 = new Point(point.X, point.Y + bump);

                // X , Y
                Point pt2 = new Point(point.X + size.Width, point.Y + bump);

                // Draw line bar
                graphics.DrawLine(linePen, pt1, pt2);

                // Prepare for next bar drawing
                bump = bump + spacing;
            }
        }

        #endregion
    }
}