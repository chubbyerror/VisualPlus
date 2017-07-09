namespace VisualPlus.Extensibility
{
    #region Namespace

    using System.Drawing;
    using System.Drawing.Drawing2D;

    using VisualPlus.Structure;

    #endregion

    public static class GraphicsPathExtension
    {
        #region Events

        /// <summary>Converts the GraphicsPath to a border path.</summary>
        /// <param name="borderPath">The border path.</param>
        /// <param name="border">The border.</param>
        /// <returns>Converted border path.</returns>
        public static GraphicsPath ToBorderPath(this GraphicsPath borderPath, Border border)
        {
            return Border.GetBorderShape(borderPath.GetBounds().ToRectangle(), border.Type, border.Rounding);
        }

        /// <summary>Converts the Rectangle to a GraphicsPath.</summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>The graphics path.</returns>
        public static GraphicsPath ToGraphicsPath(this Rectangle rectangle)
        {
            GraphicsPath convertedPath = new GraphicsPath();
            convertedPath.AddRectangle(rectangle);
            return convertedPath;
        }

        #endregion
    }
}