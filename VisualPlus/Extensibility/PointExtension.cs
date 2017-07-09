namespace VisualPlus.Extensibility
{
    #region Namespace

    using System.Drawing;

    #endregion

    public static class PointExtension
    {
        #region Events

        /// <summary>Returns the center point of the rectangle.</summary>
        /// <param name="rectangle">This rectangle.</param>
        /// <returns>Center point.</returns>
        public static Point Center(this Rectangle rectangle)
        {
            return new Point(rectangle.Left + (rectangle.Width / 2), rectangle.Top + (rectangle.Height / 2));
        }

        #endregion
    }
}