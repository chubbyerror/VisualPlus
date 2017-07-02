namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;

    using VisualPlus.Styles;

    #endregion

    [TypeConverter(typeof(WatermarkConverter))]
    [Description("The watermark class.")]
    public class Watermark : IWatermark
    {
        #region Variables

        [Browsable(false)]
        public SolidBrush Brush;

        #endregion

        #region Variables

        private Color activeColor;
        private Font font;
        private Color inactiveColor;
        private string text;
        private bool visible;

        #endregion

        #region Constructors

        public Watermark()
        {
            activeColor = Settings.DefaultValue.Watermark.ActiveColor;
            font = new Font(Settings.DefaultValue.Font.FontFamily, Settings.DefaultValue.Font.FontSize, Settings.DefaultValue.Font.FontStyle);
            inactiveColor = Settings.DefaultValue.Watermark.InactiveColor;
            text = Settings.DefaultValue.WatermarkText;
            visible = Settings.DefaultValue.WatermarkVisible;

            Brush = new SolidBrush(inactiveColor);
        }

        #endregion

        #region Properties

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Strings.Font)]
        public Font Font
        {
            get
            {
                return font;
            }

            set
            {
                font = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Strings.Text)]
        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Visible)]
        public bool Visible
        {
            get
            {
                return visible;
            }

            set
            {
                visible = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Color)]
        public Color ActiveColor
        {
            get
            {
                return activeColor;
            }

            set
            {
                activeColor = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Color)]
        public Color InactiveColor
        {
            get
            {
                return inactiveColor;
            }

            set
            {
                inactiveColor = value;
            }
        }

        #endregion

        #region Events

        public static void DrawWatermark(Graphics graphics, Rectangle textBoxRectangle, StringFormat stringFormat, Watermark watermark)
        {
            if (watermark.Visible)
            {
                graphics.DrawString(watermark.Text, watermark.Font, watermark.Brush, textBoxRectangle, stringFormat);
            }
        }

        #endregion
    }

    public class WatermarkConverter : ExpandableObjectConverter
    {
        #region Events

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var stringValue = value as string;

            if (stringValue != null)
            {
                return new ObjectWatermarkWrapper(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Watermark watermark;
            object result;

            result = null;
            watermark = value as Watermark;

            if ((watermark != null) && (destinationType == typeof(string)))
            {
                result = "Watermark Settings";
            }

            return result ?? base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    [TypeConverter(typeof(WatermarkConverter))]
    public class ObjectWatermarkWrapper
    {
        #region Constructors

        public ObjectWatermarkWrapper()
        {
        }

        public ObjectWatermarkWrapper(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public object Value { get; set; }

        #endregion
    }
}