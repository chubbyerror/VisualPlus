namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Localization;

    #endregion

    [TypeConverter(typeof(ExpanderConverter))]
    [Description("Expands the control.")]
    public class Expander
    {
        #region Variables

        private Size buttonSize;
        private Color color;
        private int contractedHeight;
        private Cursor cursor;
        private bool expanded;
        private Control hookedControl;
        private Alignment.Horizontal horizontal;
        private Size originalSize;
        private int spacing;
        private bool visible;

        #endregion

        #region Constructors

        public Expander(Control control, int contractHeight)
        {
            hookedControl = control;
            originalSize = hookedControl.Size;
            contractedHeight = contractHeight;

            horizontal = Enums.Alignment.Horizontal.Left;
            buttonSize = new Size(12, 10);
            color = Settings.DefaultValue.Control.FlatButtonEnabled;
            cursor = Cursors.Hand;
            expanded = true;
            spacing = 3;
            visible = true;
        }

        #endregion

        #region Properties

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Alignment)]
        public Alignment.Horizontal Alignment
        {
            get
            {
                return horizontal;
            }

            set
            {
                horizontal = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Size)]
        public Size ButtonSize
        {
            get
            {
                return buttonSize;
            }

            set
            {
                buttonSize = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Color)]
        public Color Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Expander.ContractedHeight)]
        public int ContractedHeight
        {
            get
            {
                return contractedHeight;
            }

            set
            {
                contractedHeight = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Cursor)]
        public Cursor Cursor
        {
            get
            {
                return cursor;
            }

            set
            {
                cursor = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Expander.Expanded)]
        public bool Expanded
        {
            get
            {
                return expanded;
            }

            set
            {
                expanded = value;
                hookedControl.Size = GetControlToggled();
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
                return originalSize;
            }

            set
            {
                originalSize = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Spacing)]
        public int Spacing
        {
            get
            {
                return spacing;
            }

            set
            {
                spacing = value;
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

        #endregion

        #region Events

        /// <summary>Draws the expander arrow.</summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="buttonPoint">The button location.</param>
        public void Draw(Graphics graphics, Point buttonPoint)
        {
            var points = new Point[3];
            if (expanded)
            {
                points[0].X = buttonPoint.X + spacing + (ButtonSize.Width / 2);
                points[0].Y = buttonPoint.Y + spacing;

                points[1].X = buttonPoint.X + spacing;
                points[1].Y = buttonPoint.Y + spacing + ButtonSize.Height;

                points[2].X = buttonPoint.X + spacing + ButtonSize.Width;
                points[2].Y = buttonPoint.Y + spacing + ButtonSize.Height;
            }
            else
            {
                points[0].X = buttonPoint.X + spacing;
                points[0].Y = buttonPoint.Y + spacing;

                points[1].X = buttonPoint.X + spacing + ButtonSize.Width;
                points[1].Y = buttonPoint.Y + spacing;

                points[2].X = buttonPoint.X + spacing + (ButtonSize.Width / 2);
                points[2].Y = buttonPoint.Y + spacing + ButtonSize.Height;
            }

            graphics.FillPolygon(new SolidBrush(color), points);
        }

        /// <summary>Retrieves the alignment point from the control.</summary>
        /// <param name="control">The parent control.</param>
        /// <returns>The expander button alignment point.</returns>
        public Point GetAlignmentPoint(Size control)
        {
            Point newPoint = new Point { Y = spacing };
            if (horizontal == Enums.Alignment.Horizontal.Left)
            {
                newPoint.X = spacing;
            }
            else
            {
                newPoint.X = control.Width - buttonSize.Width - (spacing * 2);
            }

            return newPoint;
        }

        /// <summary>Checks whether the mouse is on the button.</summary>
        /// <param name="mousePoint">The mouse location.</param>
        public void GetMouseOnButton(Point mousePoint)
        {
            MouseOnButton = GDI.IsMouseInBounds(mousePoint, new Rectangle(GetAlignmentPoint(originalSize), buttonSize));
        }

        /// <summary>Update the original size.</summary>
        /// <param name="control">The parent control.</param>
        public void UpdateOriginal(Size control)
        {
            if (expanded)
            {
                originalSize = control;
            }
        }

        /// <summary>Gets the toggle control size.</summary>
        /// <returns>New size.</returns>
        private Size GetControlToggled()
        {
            int height;

            if (!expanded)
            {
                height = contractedHeight;
                expanded = false;
            }
            else
            {
                height = originalSize.Height;
                expanded = true;
            }

            return new Size(originalSize.Width, height);
        }

        #endregion
    }

    public class ExpanderConverter : ExpandableObjectConverter
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
            Expander expander = value as Expander;

            if ((expander != null) && (destinationType == typeof(string)))
            {
                // result = borderStyle.ToString();
                result = "Expander Settings";
            }

            return result ?? base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    [TypeConverter(typeof(ExpanderConverter))]
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