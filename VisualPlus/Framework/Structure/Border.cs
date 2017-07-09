namespace VisualPlus.Framework.Structure
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using VisualPlus.Enums;
    using VisualPlus.Framework.Delegates;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Styles;

    #endregion

    [TypeConverter(typeof(BorderConverter))]
    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public class Border : Shape, IBorder
    {
        #region Variables

        private Color hoverColor;
        private bool hoverVisible;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Border" /> class.</summary>
        public Border()
        {
            StyleManager styleManager = new StyleManager(Settings.DefaultValue.DefaultStyle);

            hoverColor = styleManager.BorderStyle.HoverColor;
            hoverVisible = true;
        }

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description("Occours when the hvoer color has been changed.")]
        public event BorderHoverColorChangedEventHandler HoverColorChanged;

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description("Occours when the hover visible has been changed.")]
        public event BorderHoverVisibleChangedEventHandler HoverVisibleChanged;

        #endregion

        #region Properties

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Color)]
        public Color HoverColor
        {
            get
            {
                return hoverColor;
            }

            set
            {
                hoverColor = value;
                HoverColorChanged?.Invoke();
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description(Localize.Description.Common.Visible)]
        public bool HoverVisible
        {
            get
            {
                return hoverVisible;
            }

            set
            {
                hoverVisible = value;
                HoverVisibleChanged?.Invoke();
            }
        }

        #endregion

        #region Events

        /// <summary>Draws a border around the rectangle.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="borderRectangle">The rectangle.</param>
        /// <param name="borderThickness">The thickness.</param>
        /// <param name="color">The color.</param>
        public static void DrawBorder(Graphics graphics, Rectangle borderRectangle, float borderThickness, Color color)
        {
            using (GraphicsPath borderPath = new GraphicsPath())
            {
                borderPath.AddRectangle(borderRectangle);
                DrawBorder(graphics, borderPath, borderThickness, color);
            }
        }

        /// <summary>Draws a border around the path.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="borderPath">The path.</param>
        /// <param name="borderThickness">The thickness.</param>
        /// <param name="color">The color.</param>
        public static void DrawBorder(Graphics graphics, GraphicsPath borderPath, float borderThickness, Color color)
        {
            Pen borderPen = new Pen(color, borderThickness);
            graphics.DrawPath(borderPen, borderPath);
        }

        /// <summary>Draws the border style.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="border">The border type.</param>
        /// <param name="mouseState">The mouse state.</param>
        /// <param name="borderPath">The border path.</param>
        public static void DrawBorderStyle(Graphics graphics, Border border, MouseStates mouseState, GraphicsPath borderPath)
        {
            if (border.Visible)
            {
                if ((mouseState == MouseStates.Hover) && border.HoverVisible)
                {
                    DrawBorder(graphics, borderPath, border.Thickness, border.HoverColor);
                }
                else
                {
                    DrawBorder(graphics, borderPath, border.Thickness, border.Color);
                }
            }
        }

        /// <summary>Draws the border style.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="border">The border type.</param>
        /// <param name="mouseState">The mouse state.</param>
        /// <param name="borderRectangle">The border Rectangle.</param>
        public static void DrawBorderStyle(Graphics graphics, Border border, MouseStates mouseState, Rectangle borderRectangle)
        {
            GraphicsPath borderPath = new GraphicsPath();
            borderPath.AddRectangle(borderRectangle);

            DrawBorderStyle(graphics, border, mouseState, borderPath);
        }

        /// <summary>Get the border shape.</summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="border">The border.</param>
        /// <returns>Border graphics path.</returns>
        public static GraphicsPath GetBorderShape(Rectangle rectangle, Border border)
        {
            return GetBorderShape(rectangle, border.Type, border.Rounding);
        }

        /// <summary>Get the border shape.</summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="borderType">The shape.</param>
        /// <param name="borderRounding">The rounding.</param>
        /// <returns>The <see cref="GraphicsPath" />.</returns>
        public static GraphicsPath GetBorderShape(Rectangle rectangle, ShapeType borderType, int borderRounding)
        {
            Rectangle borderRectangle = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);

            GraphicsPath borderShape = new GraphicsPath();

            switch (borderType)
            {
                case ShapeType.Rectangle:
                    {
                        borderShape.AddRectangle(borderRectangle);
                        break;
                    }

                case ShapeType.Rounded:
                    {
                        borderShape.AddArc(borderRectangle.X, borderRectangle.Y, borderRounding, borderRounding, 180.0F, 90.0F);
                        borderShape.AddArc(borderRectangle.Right - borderRounding, borderRectangle.Y, borderRounding, borderRounding, 270.0F, 90.0F);
                        borderShape.AddArc(borderRectangle.Right - borderRounding, borderRectangle.Bottom - borderRounding, borderRounding, borderRounding, 0.0F, 90.0F);
                        borderShape.AddArc(borderRectangle.X, borderRectangle.Bottom - borderRounding, borderRounding, borderRounding, 90.0F, 90.0F);
                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(borderType), borderType, null);
                    }
            }

            borderShape.CloseAllFigures();
            return borderShape;
        }

        #endregion
    }

    public class BorderConverter : ExpandableObjectConverter
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
                return new ObjectShape2Wrapper(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Border border;
            object result;

            result = null;
            border = value as Border;

            if (border != null && destinationType == typeof(string))
            {
                // result = borderStyle.ToString();
                result = "Border Settings";
            }

            return result ?? base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }

    [TypeConverter(typeof(BorderConverter))]
    public class ObjectShape2Wrapper
    {
        #region Constructors

        public ObjectShape2Wrapper()
        {
        }

        public ObjectShape2Wrapper(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public object Value { get; set; }

        #endregion
    }
}