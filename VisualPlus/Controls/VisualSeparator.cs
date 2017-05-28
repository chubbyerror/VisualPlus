namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Control))]
    [DefaultEvent("Click")]
    [DefaultProperty("Enabled")]
    [Description("The Visual Separator")]
    [Designer(DesignManager.VisualSeparator)]
    public sealed class VisualSeparator : Control
    {
        #region Variables

        private readonly Color[] lineColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.LineColor),
                Settings.DefaultValue.Style.LineColor,
                ControlPaint.Light(Settings.DefaultValue.Style.LineColor)
            };

        private readonly Color[] shadowColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.ShadowColor),
                Settings.DefaultValue.Style.ShadowColor,
                ControlPaint.Light(Settings.DefaultValue.Style.ShadowColor)
            };

        private Gradient lineGradient = new Gradient();

        private Rectangle lineRectangle;
        private Orientation separatorOrientation = Orientation.Horizontal;

        private Gradient shadowGradient = new Gradient();

        private Rectangle shadowRectangle;
        private bool shadowVisible;

        #endregion

        #region Constructors

        public VisualSeparator()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;

            UpdateStyles();

            float[] gradientPosition = { 0, 1 / 2f, 1 };
            float angle = 90;

            lineGradient.Angle = angle;
            lineGradient.Colors = lineColor;
            lineGradient.Positions = gradientPosition;

            shadowGradient.Angle = angle;
            shadowGradient.Colors = shadowColor;
            shadowGradient.Positions = gradientPosition;
        }

        #endregion

        #region Properties

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient Line
        {
            get
            {
                return lineGradient;
            }

            set
            {
                lineGradient = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.SeparatorStyle)]
        public Orientation Orientation
        {
            get
            {
                return separatorOrientation;
            }

            set
            {
                separatorOrientation = value;

                if (separatorOrientation == Orientation.Horizontal)
                {
                    if (Width < Height)
                    {
                        int temp = Width;
                        Width = Height;
                        Height = temp;
                    }
                }
                else
                {
                    // Vertical
                    if (Width > Height)
                    {
                        int temp = Width;
                        Width = Height;
                        Height = temp;
                    }
                }

                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient Shadow
        {
            get
            {
                return shadowGradient;
            }

            set
            {
                shadowGradient = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentVisible)]
        public bool ShadowVisible
        {
            get
            {
                return shadowVisible;
            }

            set
            {
                shadowVisible = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            Point linePosition = new Point();
            Size lineSize = new Size();
            Point shadowPosition = new Point();
            Size shadowSize = new Size();
            Point[] gradientPoints = { };

            switch (separatorOrientation)
            {
                case Orientation.Horizontal:
                    {
                        linePosition = new Point(0, 1);
                        lineSize = new Size(Width, 1);

                        shadowPosition = new Point(0, 2);
                        shadowSize = new Size(Width, 2);

                        gradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Width } };
                        break;
                    }

                case Orientation.Vertical:
                    {
                        linePosition = new Point(1, 0);
                        lineSize = new Size(1, Height);

                        shadowPosition = new Point(2, 0);
                        shadowSize = new Size(2, Height);

                        gradientPoints = new[] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };
                        break;
                    }
            }

            lineRectangle = new Rectangle(linePosition, lineSize);

            LinearGradientBrush lineBrush = GDI.CreateGradientBrush(lineGradient.Colors, gradientPoints, lineGradient.Angle, lineGradient.Positions);
            graphics.DrawRectangle(new Pen(lineBrush), lineRectangle);

            if (shadowVisible)
            {
                shadowRectangle = new Rectangle(shadowPosition, shadowSize);
                LinearGradientBrush shadowBrush = GDI.CreateGradientBrush(lineGradient.Colors, gradientPoints, lineGradient.Angle, lineGradient.Positions);
                graphics.DrawRectangle(new Pen(shadowBrush), shadowRectangle);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (separatorOrientation == Orientation.Horizontal)
            {
                Height = 4;
            }
            else
            {
                Width = 4;
            }
        }

        #endregion
    }
}