namespace VisualPlus.Framework
{
    #region Namespace

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using VisualPlus.Framework.Structure;

    #endregion

    public static class Extensions
    {
        #region Events

        /// <summary>Returns the center point of the rectangle.</summary>
        /// <param name="rectangle">This rectangle.</param>
        /// <returns>Center point.</returns>
        public static Point Center(this Rectangle rectangle)
        {
            return new Point(rectangle.Left + (rectangle.Width / 2), rectangle.Top + (rectangle.Height / 2));
        }

        /// <summary>Filter the bitmap with GrayScale.</summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns>Filtered bitmap.</returns>
        public static Bitmap FilterGrayScale(this Bitmap bitmap)
        {
            Bitmap grayScale = new Bitmap(bitmap.Width, bitmap.Height);

            for (var y = 0; y < grayScale.Height; y++)
            {
                for (var x = 0; x < grayScale.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);

                    var gs = (int)((c.R * 0.3) + (c.G * 0.59) + (c.B * 0.11));

                    grayScale.SetPixel(x, y, Color.FromArgb(gs, gs, gs));
                }
            }

            return grayScale;
        }

        /// <summary>Check if the value is in range.</summary>
        /// <param name="value">The value.</param>
        /// <param name="minimum">The minimum value.</param>
        /// <param name="maximum">The maximum value.</param>
        /// <returns>Returns value.</returns>
        public static bool IsInRange(this int value, int minimum, int maximum)
        {
            return (value >= minimum) && (value <= maximum);
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

        /// <summary>Rounds a RectangleF to a Rectangle.</summary>
        /// <param name="rectangleF">The rectangleF.</param>
        /// <returns>Rounded rectangle.</returns>
        public static Rectangle ToRectangle(this RectangleF rectangleF)
        {
            return Rectangle.Round(rectangleF);
        }

        #endregion
    }

    public static class EnumExtensions
    {
        #region Events

        /// <summary>Returns the count length.</summary>
        /// <param name="enumerator">The enumerator.</param>
        /// <returns>The count length.</returns>
        public static int Count(this Enum enumerator)
        {
            return Enum.GetNames(enumerator.GetType()).Length;
        }

        /// <summary>Gets the enumerator index from the value.</summary>
        /// <param name="enumerator">The enumerator.</param>
        /// <param name="value">Value to search.</param>
        /// <returns>The value index.</returns>
        public static int GetIndexByValue(this Enum enumerator, string value)
        {
            try
            {
                var indexCount = (int)Enum.Parse(enumerator.GetType(), value);
                return indexCount;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        /// <summary>Gets the enumerator value from the index.</summary>
        /// <typeparam name="T">Type parameter.</typeparam>
        /// <param name="enumerator">The enumerator.</param>
        /// <param name="index">The index to search.</param>
        /// <returns>The value string.</returns>
        public static string GetValueByIndex<T>(this Enum enumerator, int index)
            where T : struct
        {
            Type type = typeof(T);
            if (type.IsEnum && Enum.IsDefined(enumerator.GetType(), index))
            {
                return Enum.GetName(enumerator.GetType(), index);
            }
            else
            {
                return null;
            }
        }

        /// <summary>Returns the string as an enumerator.</summary>
        /// <typeparam name="T">Type parameter.</typeparam>
        /// <param name="enumeratorString">The string.</param>
        /// <returns>The enumerator.</returns>
        public static Enum ToEnum<T>(this string enumeratorString)
            where T : struct
        {
            Type type = typeof(T);

            try
            {
                return (Enum)Enum.Parse(type, enumeratorString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>Converts enumerator to a list type.</summary>
        /// <typeparam name="T">Type parameter.</typeparam>
        /// <param name="enumerator">The enumerator.</param>
        /// <returns>Returns enumerated list.</returns>
        public static List<T> ToList<T>(this Enum enumerator)
            where T : struct
        {
            Type type = typeof(T);
            return !type.IsEnum ? null : Enum.GetValues(type).Cast<T>().ToList();
        }

        /// <summary>Returns the value.</summary>
        /// <param name="enumerator">The enumerator.</param>
        /// <returns>The enumerator description.</returns>
        public static string Value(this Enum enumerator)
        {
            try
            {
                DescriptionAttribute attribute = enumerator.GetType().GetField(enumerator.ToString()).GetCustomAttribute<DescriptionAttribute>(false);
                return attribute != null ? attribute.Description : enumerator.ToString();
            }
            catch
            {
                return string.Empty;
            }
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