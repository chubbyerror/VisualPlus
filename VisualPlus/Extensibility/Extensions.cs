namespace VisualPlus.Extensibility
{
    #region Namespace

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Forms;

    #endregion

    public static class Extensions
    {
        #region Properties

        /// <summary>Get the design mode state.</summary>
        public static bool IsInDesignMode
        {
            get
            {
                bool isInDesignMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime) || Debugger.IsAttached;

                if (!isInDesignMode)
                {
                    using (Process process = Process.GetCurrentProcess())
                    {
                        return process.ProcessName.ToLowerInvariant().Contains("devenv");
                    }
                }

                return isInDesignMode;
            }
        }

        #endregion

        #region Events

        /// <summary>Gets a boolean determining whether the object holds any value or is empty/null.</summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>The object elements exist.</returns>
        public static bool AnyOrNotNull<T>(this IEnumerable<T> source)
        {
            return (source != null) && source.Any();
        }

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

        public static void ScrollDown(this Panel p, int pos)
        {
            // pos passed in should be positive
            using (Control c = new Control { Parent = p, Height = 1, Top = p.ClientSize.Height + pos })
            {
                p.ScrollControlIntoView(c);
            }
        }

        /// <summary>Scroll to the bottom of the panel.</summary>
        /// <param name="panel">The panel.</param>
        public static void ScrollToBottom(this Panel panel)
        {
            using (Control c = new Control { Parent = panel, Dock = DockStyle.Bottom })
            {
                panel.ScrollControlIntoView(c);
                c.Parent = null;
            }
        }

        public static void ScrollUp(this Panel p, int pos)
        {
            // pos passed in should be negative
            using (Control c = new Control { Parent = p, Height = 1, Top = pos })
            {
                p.ScrollControlIntoView(c);
            }
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