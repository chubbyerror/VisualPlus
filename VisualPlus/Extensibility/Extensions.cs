namespace VisualPlus.Extensibility
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Globalization;

    #endregion

    public static class Extensions
    {
        #region Events

        /// <summary>Gets a bool value determining whether the object has the following method.</summary>
        /// <param name="objectToCheck">Object to check.</param>
        /// <param name="methodName">Method name to search.</param>
        /// <returns>Returns bool value.</returns>
        public static bool HasMethod(this object objectToCheck, string methodName)
        {
            return objectToCheck.GetType().GetMethod(methodName) != null;
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

        #endregion
    }

    public class CustomNumberTypeConverter : TypeConverter
    {
        #region Events

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var s = (string)value;
                return int.Parse(s, NumberStyles.AllowThousands, culture);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
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