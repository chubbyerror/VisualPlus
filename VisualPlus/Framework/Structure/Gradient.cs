namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;

    #endregion

    [TypeConverter(typeof(GradientConverter))]
    public class Gradient
    {
        #region Variables

        private float angle;

        private Color[] colors;

        private float[] positions;

        #endregion

        #region Constructors

        public Gradient()
        {
            colors = new[]
                {
                    Color.Red,
                    Color.Green,
                    Color.Blue
                };

            positions = new[] { 0, 1 / 2f, 1 };
        }

        #endregion

        #region Properties

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Gradient.Angle)]
        public float Angle
        {
            get
            {
                return angle;
            }

            set
            {
                angle = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Gradient.Colors)]
        public Color[] Colors
        {
            get
            {
                return colors;
            }

            set
            {
                colors = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Gradient.Positions)]
        public float[] Positions
        {
            get
            {
                return positions;
            }

            set
            {
                positions = value;
            }
        }

        #endregion

        #region Events

        /// <summary>Creates a gradient brush.</summary>
        /// <param name="colors">The colors.</param>
        /// <param name="points">The points.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="positions">The positions.</param>
        /// <returns>Returns a custom gradient brush.</returns>
        public static LinearGradientBrush CreateGradientBrush(Color[] colors, Point[] points, float angle, float[] positions)
        {
            LinearGradientBrush linearGradientBrush = new LinearGradientBrush(points[0], points[1], Color.Black, Color.Black);

            ColorBlend colorBlend = new ColorBlend
                {
                    Positions = positions,
                    Colors = colors
                };

            linearGradientBrush.InterpolationColors = colorBlend;
            linearGradientBrush.RotateTransform(angle);

            return linearGradientBrush;
        }

        #endregion
    }

    public class GradientConverter : ExpandableObjectConverter
    {
        #region Events

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var stringValue = value as string;

            if (stringValue != null)
            {
                return new ObjectGradientWrapper(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Gradient gradient;
            object result;

            result = null;
            gradient = value as Gradient;

            if (gradient != null && destinationType == typeof(string))
            {
                // result = borderStyle.ToString();
                result = "Gradient Settings";
            }

            return result ?? base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    [TypeConverter(typeof(GradientConverter))]
    public class ObjectGradientWrapper
    {
        #region Constructors

        public ObjectGradientWrapper()
        {
        }

        public ObjectGradientWrapper(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public object Value { get; set; }

        #endregion
    }
}