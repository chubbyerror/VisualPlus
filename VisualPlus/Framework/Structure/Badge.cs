namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.Windows.Forms;

    using VisualPlus.Framework.Delegates;
    using VisualPlus.Framework.Handlers;

    #endregion

    [TypeConverter(typeof(BadgeConverter))]
    [Description("Badge component.")]
    public class Badge
    {
        #region Variables

        private Color _backColor;
        private Point _badgePoint;
        private Border _border;
        private Control _control;
        private Font _font;
        private Color _foreColor;
        private int _value;
        private bool _visible;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Badge" /> class.</summary>
        /// <param name="control">The control.</param>
        /// <param name="badgePoint">The badge location.</param>
        public Badge(Control control, Point badgePoint)
        {
            StyleManager _styleManager = new StyleManager(Settings.DefaultValue.DefaultStyle);

            _control = control;

            _badgePoint = badgePoint;

            _value = 0;
            _visible = true;

            _font = _styleManager.Font;

            _backColor = Color.Red;
            _foreColor = Color.White;

            _border = new Border { Rounding = Settings.DefaultValue.Rounding.Default };
        }

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description("Occours when the value property has changed.")]
        public event BadgeValueChangedEventHandler ValueChanged;

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description("Occours when the value has been decreased.")]
        public event BadgeValueDecreasedEventHandler ValueDecreased;

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description("Occours when the value has been increased.")]
        public event BadgeValueIncreasedEventHandler ValueIncreased;

        #endregion

        #region Properties

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Color BackColor
        {
            get
            {
                return _backColor;
            }

            set
            {
                _backColor = value;
            }
        }

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Border Border
        {
            get
            {
                return _border;
            }

            set
            {
                _border = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Font Font
        {
            get
            {
                return _font;
            }

            set
            {
                _font = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Color ForeColor
        {
            get
            {
                return _foreColor;
            }

            set
            {
                _foreColor = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Point Location
        {
            get
            {
                return _badgePoint;
            }

            set
            {
                _badgePoint = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (_value != value)
                {
                    _value = value;
                    ValueChanged?.Invoke();
                }
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool Visible
        {
            get
            {
                return _visible;
            }

            set
            {
                _visible = value;
            }
        }

        #endregion

        #region Events

        /// <summary>Decrement from the value.</summary>
        /// <param name="value">Amount of value to decrement.</param>
        public void Decrement(int value)
        {
            if (Value > 0)
            {
                Value -= value;
                ValueDecreased?.Invoke();
                if (Value < 0)
                {
                    Value = 0;
                }
            }
            else
            {
                Value = 0;
            }

            _control.Invalidate();
        }

        /// <summary>Draws the badge.</summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="badgePoint">The badge location.</param>
        public void Draw(Graphics graphics, Point badgePoint)
        {
            if (_visible)
            {
                Size textSize = GDI.GetTextSize(graphics, _value.ToString(), _font);
                Rectangle shapeRectangle = new Rectangle(badgePoint, new Size(textSize.Width + 1, textSize.Height));
                Point textPoint = new Point((shapeRectangle.X + (shapeRectangle.Width / 2)) - (textSize.Width / 2), (shapeRectangle.Y + (shapeRectangle.Height / 2)) - (textSize.Height / 2));
                GraphicsPath shapePath = Border.GetBorderShape(shapeRectangle, _border.Type, _border.Rounding);

                graphics.FillPath(new SolidBrush(BackColor), shapePath);
                Border.DrawBorder(graphics, shapePath, _border.Thickness, _border.Color);
                graphics.DrawString(_value.ToString(), _font, new SolidBrush(_foreColor), textPoint);
            }
        }

        /// <summary>Increment to the value.</summary>
        /// <param name="value">Amount of value to increment.</param>
        public void Increment(int value)
        {
            Value += value;
            ValueIncreased?.Invoke();
            _control.Invalidate();
        }

        #endregion
    }

    public class BadgeConverter : ExpandableObjectConverter
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
                return new ObjectExpanderWrapper(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = null;
            Badge badge = value as Badge;

            if ((badge != null) && (destinationType == typeof(string)))
            {
                // result = borderStyle.ToString();
                result = "Badge Settings";
            }

            return result ?? base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    [TypeConverter(typeof(BadgeConverter))]
    public class ObjectBadgeWrapper
    {
        #region Constructors

        public ObjectBadgeWrapper()
        {
        }

        public ObjectBadgeWrapper(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public object Value { get; set; }

        #endregion
    }
}