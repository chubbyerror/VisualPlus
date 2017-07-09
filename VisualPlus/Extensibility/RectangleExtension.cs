namespace VisualPlus.Extensibility
{
    #region Namespace

    using System.Drawing;

    #endregion

    public static class RectangleExtension
    {
        #region Events

        /// <summary>Aligns the rectangle to the bottom.</summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="outerBounds">The outside rectangle.</param>
        /// <param name="spacing">The spacing.</param>
        /// <returns>Aligned rectangle.</returns>
        public static Rectangle AlignBottom(this Rectangle rectangle, Rectangle outerBounds, int spacing)
        {
            return new Rectangle(rectangle.X, outerBounds.Height - spacing - rectangle.Height, rectangle.Width, rectangle.Height);
        }

        /// <summary>Aligns the rectangle to the center.</summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="outerBounds">The outside rectangle.</param>
        /// <returns>Aligned rectangle.</returns>
        public static Rectangle AlignCenterX(this Rectangle rectangle, Rectangle outerBounds)
        {
            return new Rectangle((outerBounds.Width / 2) - (rectangle.Width / 2), rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>Aligns the rectangle to the center height.</summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="outerBounds">The outside rectangle.</param>
        /// <returns>Aligned rectangle.</returns>
        public static Rectangle AlignCenterY(this Rectangle rectangle, Rectangle outerBounds)
        {
            return new Rectangle(rectangle.X, (outerBounds.Height / 2) - (rectangle.Height / 2), rectangle.Width, rectangle.Height);
        }

        /// <summary>Aligns the rectangle to the left.</summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="outerBounds">The outside rectangle.</param>
        /// <param name="spacing">The spacing.</param>
        /// <returns>Aligned rectangle.</returns>
        public static Rectangle AlignLeft(this Rectangle rectangle, Rectangle outerBounds, int spacing)
        {
            return new Rectangle(outerBounds.X + spacing, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>Aligns the rectangle to the right.</summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="outerBounds">The outside rectangle.</param>
        /// <param name="spacing">The spacing.</param>
        /// <returns>Aligned rectangle.</returns>
        public static Rectangle AlignRight(this Rectangle rectangle, Rectangle outerBounds, int spacing)
        {
            return new Rectangle(outerBounds.Width - spacing - rectangle.Width, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>Aligns the rectangle to the top.</summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="outerBounds">The outside rectangle.</param>
        /// <param name="spacing">The spacing.</param>
        /// <returns>Aligned rectangle.</returns>
        public static Rectangle AlignTop(this Rectangle rectangle, Rectangle outerBounds, int spacing)
        {
            return new Rectangle(rectangle.X, outerBounds.Y + spacing, rectangle.Width, rectangle.Height);
        }

        /// <summary>Rounds a RectangleF to a Rectangle.</summary>
        /// <param name="rectangleF">The rectangleF.</param>
        /// <returns>Rounded rectangle.</returns>
        public static Rectangle ToRectangle(this RectangleF rectangleF)
        {
            return Rectangle.Round(rectangleF);
        }

        #endregion
    }
}