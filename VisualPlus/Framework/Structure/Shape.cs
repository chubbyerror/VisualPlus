namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;

    using VisualPlus.Framework.GDI;
    using VisualPlus.Properties;

    #endregion

    [TypeConverter(typeof(ShapeConverter))]
    public class Shape
    {
        #region Variables

        private Border border;

        private bool centerImage;

        // iShape < -- >
        private Gradient disabledGradient;
        private Gradient enabledGradient;
        private Gradient hoverGradient;
        private Gradient pressedGradient;

        private Rectangle rectangle;

        private VisualBitmap visualBitmap;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Shape"/> class.</summary>
        /// <param name="shapeRectangle">The shape Rectangle.</param>
        public Shape(Rectangle shapeRectangle)
        {
            border = new Border();
            disabledGradient = Settings.DefaultValue.Control.ControlDisabled;
            enabledGradient = Settings.DefaultValue.Control.ControlEnabled;
            hoverGradient = Settings.DefaultValue.Control.ControlHover;
            pressedGradient = Settings.DefaultValue.Control.ControlPressed;

            centerImage = true;
            rectangle = shapeRectangle;

            visualBitmap = new VisualBitmap(Resources.Icon, new Size(25, 25));
        }

        #endregion

        #region Properties

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Border Border
        {
            get
            {
                return border;
            }

            set
            {
                border = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Toggle)]
        public bool CenteredImage
        {
            get
            {
                return centerImage;
            }

            set
            {
                centerImage = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.ColorGradient)]
        public Gradient DisabledGradient
        {
            get
            {
                return disabledGradient;
            }

            set
            {
                disabledGradient = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.ColorGradient)]
        public Gradient EnabledGradient
        {
            get
            {
                return enabledGradient;
            }

            set
            {
                enabledGradient = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.ColorGradient)]
        public Gradient HoverGradient
        {
            get
            {
                return hoverGradient;
            }

            set
            {
                hoverGradient = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public VisualBitmap BackgroundImage
        {
            get
            {
                return visualBitmap;
            }

            set
            {
                visualBitmap = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.ColorGradient)]
        public Gradient PressedGradient
        {
            get
            {
                return pressedGradient;
            }

            set
            {
                pressedGradient = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Size)]
        public Rectangle Rectangle
        {
            get
            {
                return rectangle;
            }

            set
            {
                rectangle = value;
            }
        }

        #endregion

        #region Events

        /// <summary>Draws the background.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="shape">The shape.</param>
        /// <param name="linearGradientBrush">The brush.</param>
        /// <param name="backgroundPath">The path.</param>
        public static void DrawBackground(Graphics graphics, Shape shape, Brush linearGradientBrush, GraphicsPath backgroundPath)
        {
            if (shape.BackgroundImage.Visible)
            {
                Point tempLocation = shape.CenteredImage ? new Point((shape.Rectangle.X + (shape.Rectangle.Width / 2)) - (shape.BackgroundImage.Size.Width / 2), (shape.Rectangle.Y + (shape.Rectangle.Height / 2)) - (shape.BackgroundImage.Size.Height / 2)) : shape.BackgroundImage.Point;
                graphics.DrawImage(shape.BackgroundImage.Image, new Rectangle(tempLocation, shape.BackgroundImage.Size));
            }
            else
            {
                graphics.FillPath(linearGradientBrush, backgroundPath);
            }
        }
        
        /// <summary>Draws the background.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="shape">The shape.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="enabled">The enabled state.</param>
        public static void DrawBackground(Graphics graphics, Shape shape, Rectangle rectangle, bool enabled)
        {
            GraphicsPath backgroundPath = new GraphicsPath();

            Gradient backgroundGradient = enabled ? shape.EnabledGradient : shape.DisabledGradient;
            shape.Rectangle = new Rectangle(new Point(0, (rectangle.Height / 2) - (shape.Rectangle.Height / 2)), shape.Rectangle.Size);
            backgroundPath = Border.GetBorderShape(shape.Rectangle, shape.Border.Type, shape.Border.Rounding);

            var boxGradientPoints = GDI.GetGradientPoints(rectangle);
            LinearGradientBrush backgroundGradientBrush = Gradient.CreateGradientBrush(backgroundGradient.Colors, boxGradientPoints, backgroundGradient.Angle, backgroundGradient.Positions);
            
            if (shape.BackgroundImage.Visible)
            {
                Point tempLocation = shape.CenteredImage ? new Point((shape.Rectangle.X + (shape.Rectangle.Width / 2)) - (shape.BackgroundImage.Size.Width / 2), (shape.Rectangle.Y + (shape.Rectangle.Height / 2)) - (shape.BackgroundImage.Size.Height / 2)) : shape.BackgroundImage.Point;
                graphics.DrawImage(shape.BackgroundImage.Image, new Rectangle(tempLocation, shape.BackgroundImage.Size));
            }
            else
            {
                graphics.FillPath(backgroundGradientBrush, backgroundPath);
            }
        }

        #endregion
    }

    public class ShapeConverter : ExpandableObjectConverter
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
                return new ObjectShapeWrapper(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Shape shape;
            object result;

            result = null;
            shape = value as Shape;

            if ((shape != null) && (destinationType == typeof(string)))
            {
                result = "Shape Settings";
            }

            return result ?? base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    [TypeConverter(typeof(ShapeConverter))]
    public class ObjectShapeWrapper
    {
        #region Constructors

        public ObjectShapeWrapper()
        {
        }

        public ObjectShapeWrapper(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public object Value { get; set; }

        #endregion
    }
}