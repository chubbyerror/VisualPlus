namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Toolkit.Delegates;

    #endregion

    [TypeConverter(typeof(ExpandableConverter))]
    [Description("Expandable component.")]
    public class Expandable
    {
        #region Variables

        private readonly StyleManager _styleManager = new StyleManager(Settings.DefaultValue.DefaultStyle);
        private Alignment.Horizontal _alignmentHorizontal;

        private Size _buttonSize;
        private Color _color;
        private int _contractedHeight;
        private Control _control;
        private Cursor _cursor;
        private bool _expanded;
        private Size _originalSize;
        private int _spacing;
        private bool _visible;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Expandable" /> class.</summary>
        /// <param name="control">The control.</param>
        /// <param name="contractedHeight">The contracted height.</param>
        public Expandable(Control control, int contractedHeight)
        {
            _control = control;
            _originalSize = _control.Size;
            _contractedHeight = contractedHeight;

            _alignmentHorizontal = Enums.Alignment.Horizontal.Left;
            _buttonSize = new Size(12, 10);
            _color = _styleManager.ControlStyle.FlatButtonEnabled;
            _cursor = Cursors.Hand;
            _expanded = true;
            _spacing = 3;
            _visible = true;

            _control.Click += ControlMouseClick;
            _control.MouseMove += ControlMouseMove;
            _control.Resize += ControlReSizeChanged;
            _control.SizeChanged += ControlReSizeChanged;

            _control.MouseHover += ControlMouseHover;
        }

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description(Localize.EventDescription.ToggleExpanderChanged)]
        public event ExpanderClickEventHandler ExpanderClick;

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description(Localize.EventDescription.ToggleExpanderChanged)]
        public event ExpanderContractedEventHandler ExpanderContracted;

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description(Localize.EventDescription.ToggleExpanderChanged)]
        public event ExpanderExpandedEventHandler ExpanderExpanded;

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description(Localize.EventDescription.ToggleExpanderChanged)]
        public event ExpanderToggledEventHandler ExpanderToggled;

        #endregion

        #region Properties

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Alignment)]
        public Alignment.Horizontal Alignment
        {
            get
            {
                return _alignmentHorizontal;
            }

            set
            {
                _alignmentHorizontal = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Size)]
        public Size ButtonSize
        {
            get
            {
                return _buttonSize;
            }

            set
            {
                _buttonSize = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Color)]
        public Color Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Expander.ContractedHeight)]
        public int ContractedHeight
        {
            get
            {
                return _contractedHeight;
            }

            set
            {
                _contractedHeight = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Cursor)]
        public Cursor Cursor
        {
            get
            {
                return _cursor;
            }

            set
            {
                _cursor = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Expander.Expanded)]
        public bool Expanded
        {
            get
            {
                return _expanded;
            }

            set
            {
                _expanded = value;
                _control.Size = GetControlToggled();
            }
        }

        [Browsable(false)]
        public bool MouseOnButton { get; private set; }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Size)]
        public Size OriginalSize
        {
            get
            {
                return _originalSize;
            }

            set
            {
                _originalSize = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Spacing)]
        public int Spacing
        {
            get
            {
                return _spacing;
            }

            set
            {
                _spacing = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Visible)]
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

        /// <summary>Draws the expander arrow.</summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="buttonPoint">The button location.</param>
        public void Draw(Graphics graphics, Point buttonPoint)
        {
            if (_visible)
            {
                var points = new Point[3];
                if (_expanded)
                {
                    points[0].X = buttonPoint.X + _spacing + (ButtonSize.Width / 2);
                    points[0].Y = buttonPoint.Y + _spacing;

                    points[1].X = buttonPoint.X + _spacing;
                    points[1].Y = buttonPoint.Y + _spacing + ButtonSize.Height;

                    points[2].X = buttonPoint.X + _spacing + ButtonSize.Width;
                    points[2].Y = buttonPoint.Y + _spacing + ButtonSize.Height;
                }
                else
                {
                    points[0].X = buttonPoint.X + _spacing;
                    points[0].Y = buttonPoint.Y + _spacing;

                    points[1].X = buttonPoint.X + _spacing + ButtonSize.Width;
                    points[1].Y = buttonPoint.Y + _spacing;

                    points[2].X = buttonPoint.X + _spacing + (ButtonSize.Width / 2);
                    points[2].Y = buttonPoint.Y + _spacing + ButtonSize.Height;
                }

                graphics.FillPolygon(new SolidBrush(_color), points);
            }
        }

        /// <summary>Retrieves the alignment point from the control.</summary>
        /// <param name="control">The parent control.</param>
        /// <returns>The expander button alignment point.</returns>
        public Point GetAlignmentPoint(Size control)
        {
            Point alignmentPoint = new Point { Y = _spacing };
            if (_alignmentHorizontal == Enums.Alignment.Horizontal.Left)
            {
                alignmentPoint.X = _spacing;
            }
            else
            {
                alignmentPoint.X = control.Width - _buttonSize.Width - (_spacing * 2);
            }

            return alignmentPoint;
        }

        private void ControlMouseClick(object sender, EventArgs e)
        {
            if (_visible && MouseOnButton)
            {
                ExpanderClick?.Invoke();

                if (_expanded)
                {
                    _expanded = false;
                }
                else
                {
                    _expanded = true;
                }

                ExpanderToggled?.Invoke();
                _control.Size = GetControlToggled();
            }
        }

        private void ControlMouseHover(object sender, EventArgs e)
        {
            if (_visible & MouseOnButton)
            {
                _control.Cursor = _cursor;
            }
            else if (_visible & !MouseOnButton)
            {
                _control.Cursor = Cursors.Default;
            }
        }

        private void ControlMouseMove(object sender, MouseEventArgs e)
        {
            MouseOnButton = GDI.IsMouseInBounds(e.Location, new Rectangle(GetAlignmentPoint(_originalSize), _buttonSize));
        }

        private void ControlReSizeChanged(object sender, EventArgs e)
        {
            if (_expanded)
            {
                _originalSize = _control.Size;
            }
        }

        /// <summary>Gets the toggle control size.</summary>
        /// <returns>New size.</returns>
        private Size GetControlToggled()
        {
            int height;

            if (!_expanded)
            {
                height = _contractedHeight;
                _expanded = false;
                ExpanderContracted?.Invoke();
            }
            else
            {
                height = _originalSize.Height;
                _expanded = true;
                ExpanderExpanded?.Invoke();
            }

            ExpanderToggled?.Invoke();
            return new Size(_originalSize.Width, height);
        }

        #endregion
    }

    public class ExpandableConverter : ExpandableObjectConverter
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
            Expandable expandable = value as Expandable;

            if ((expandable != null) && (destinationType == typeof(string)))
            {
                // result = borderStyle.ToString();
                result = "Expander Settings";
            }

            return result ?? base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    [TypeConverter(typeof(ExpandableConverter))]
    public class ObjectExpanderWrapper
    {
        #region Constructors

        public ObjectExpanderWrapper()
        {
        }

        public ObjectExpanderWrapper(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public object Value { get; set; }

        #endregion
    }
}