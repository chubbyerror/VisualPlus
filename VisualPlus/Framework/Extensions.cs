namespace VisualPlus.Framework
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;

    #endregion

    public static class Extensions
    {
        #region Events

        /// <summary>Returns the center point of the rectangle.</summary>
        /// <param name="rect">This rectangle.</param>
        /// <returns>Center point.</returns>
        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        }

        /// <summary>Check if the value is in range.</summary>
        /// <param name="value">The value.</param>
        /// <param name="minimum">The minimum value.</param>
        /// <param name="maximum">The maximum value.</param>
        /// <returns>Returns value.</returns>
        public static bool IsInRange(this int value, int minimum, int maximum)
        {
            return value >= minimum && value <= maximum;
        }

        /// <summary>Limits the number exclusively to only what is in range.</summary>
        /// <param name="value">The value.</param>
        /// <param name="inclusiveMinimum">The minimum.</param>
        /// <param name="inclusiveMaximum">The maximum.</param>
        /// <returns>Returns value.</returns>
        public static int LimitToRange(this int value, int inclusiveMinimum, int inclusiveMaximum)
        {
            if (value < inclusiveMinimum)
            {
                return inclusiveMinimum;
            }

            if (value > inclusiveMaximum)
            {
                return inclusiveMaximum;
            }

            return value;
        }

        #endregion
    }

    public static class RectangleAlignment
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
            return new Rectangle(outerBounds.Width / 2 - rectangle.Width / 2, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>Aligns the rectangle to the center height.</summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="outerBounds">The outside rectangle.</param>
        /// <returns>Aligned rectangle.</returns>
        public static Rectangle AlignCenterY(this Rectangle rectangle, Rectangle outerBounds)
        {
            return new Rectangle(rectangle.X, outerBounds.Height / 2 - rectangle.Height / 2, rectangle.Width, rectangle.Height);
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

        #endregion
    }

    public class CustomNumberTypeConverter : TypeConverter
    {
        #region Events

        public override bool CanConvertFrom(
            ITypeDescriptorContext context,
            Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value)
        {
            if (value is string)
            {
                var s = (string)value;
                return int.Parse(s, NumberStyles.AllowThousands, culture);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((int)value).ToString("N0", culture);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }
}