namespace VisualPlus.Framework.GDI
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

        private Color[] colors;
        private float[] positions;

        #endregion

        #region Constructors

        /// <summary>The gradient.</summary>
        /// <param name="colors">The gradients colors.</param>
        /// <param name="positions">The gradients positions.</param>
        /// <returns>The <see cref="Gradient" />.</returns>
        public Gradient(Color[] colors, float[] positions)
        {
            Colors = colors;
            Positions = positions;
        }

        #endregion

        #region Properties

        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(Color), "LightBlue")]
        public Color[] Colors
        {
            get
            {
                return colors;
            }

            set
            {
                colors = value;
                OnGradientChanged(EventArgs.Empty);
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(float), "0")]
        public float[] Positions
        {
            get
            {
                return positions;
            }

            set
            {
                positions = value;
                OnGradientChanged(EventArgs.Empty);
            }
        }

        #endregion

        #region Events

        public Gradient FromString(object value)
        {
            var values = ((string)value).Split(',');
            if (values.Length != 3)
            {
                throw new ArgumentException("Could not convert the value");
            }

            try
            {
                Gradient gradient = new Gradient(colors, positions);

                // Retrieve the colors.
                ColorConverter converter = new ColorConverter();
                gradient.Colors = (Color[])converter.ConvertFromString(values[0]);
                gradient.Positions = (float[])converter.ConvertFromString(values[1]);

                // gradient.ColorA = (Color)converter.ConvertFromString(values[0]);
                // gradient.ColorB = (Color)converter.ConvertFromString(values[1]);

                // Convert the name of the enumerated value into the corresponding
                // enumerated value (which is actually an integer constant).
                //gradient.GradientFillStyle = (LinearGradientMode)Enum.Parse(typeof(LinearGradientMode), values[2], true);

                return gradient;
            }
            catch (Exception)
            {
                throw new ArgumentException("Could not convert the value");
            }
        }

        public event EventHandler GradientChanged;

        private void OnGradientChanged(EventArgs e)
        {
            if (GradientChanged != null)
            {
                GradientChanged(this, e);
            }
        }

        #endregion
    }

    public class GradientConverter : ExpandableObjectConverter
    {
        #region Variables

        private TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));

        #endregion

        #region Events

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return ConvertFromString(value.ToString());
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }

        // public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        // {
        // if (destinationType == typeof(InstanceDescriptor) && value is GradientFill)
        // {
        // GradientFill gradient = (GradientFill)value;

        // // Specify the three-parameter (Color-Color-LinearGradientMode)  
        // // constructor. 
        // ConstructorInfo ctor = typeof(GradientFill).GetConstructor(new[] { typeof(Color), typeof(Color), typeof(LinearGradientMode) });
        // return new InstanceDescriptor(ctor, new object[] { gradient.ColorA, gradient.ColorB, gradient.GradientFillStyle });
        // }
        // else
        // {
        // return base.ConvertTo(context, culture, value, destinationType);
        // }
        // }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ConvertToString(value);
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public new string ConvertToString(object value)
        {
            Gradient fill = (Gradient)value;
            ColorConverter converter = new ColorConverter();

           // return string.Format("{0}, {1}", converter.ConvertToString(fill.Colors), converter.ConvertToString(fill.Positions));
            return "Gradient Options";
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            // Always force a new instance.    
            return true;
        }

        #endregion
    }
}