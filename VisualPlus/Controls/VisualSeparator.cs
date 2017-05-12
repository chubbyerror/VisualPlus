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
    using VisualPlus.Localization;

    #endregion

    /// <summary>The visual separator.</summary>
    [ToolboxBitmap(typeof(Control))]
    [Designer(VSDesignerBinding.VisualSeparator)]
    public sealed class VisualSeparator : Control
    {
        #region Variables

        private Point endPoint;
        private float gradientAngle = 90;
        private LinearGradientBrush gradientBrush;
        private float[] gradientPosition = { 0, 1 / 2f, 1 };
        private LinearGradientBrush gradientShadowBrush;
        private float[] gradientShadowPosition = { 0, 1 / 2f, 1 };

        private Color[] lineColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.LineColor),
                Settings.DefaultValue.Style.LineColor,
                ControlPaint.Light(Settings.DefaultValue.Style.LineColor)
            };

        private Rectangle lineRectangle;
        private Orientation separatorOrientation = Orientation.Horizontal;

        private Color[] shadowColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.ShadowColor),
                Settings.DefaultValue.Style.ShadowColor,
                ControlPaint.Light(Settings.DefaultValue.Style.ShadowColor)
            };

        private Rectangle shadowRectangle;
        private bool shadowVisible;
        private Point startPoint;

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
        }

        #endregion

        #region Properties

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Angle)]
        public float GradientAngle
        {
            get
            {
                return gradientAngle;
            }

            set
            {
                gradientAngle = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.GradientPosition)]
        public float[] GradientPosition
        {
            get
            {
                return gradientPosition;
            }

            set
            {
                gradientPosition = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.GradientPosition)]
        public float[] GradientShadowPosition
        {
            get
            {
                return gradientShadowPosition;
            }

            set
            {
                gradientShadowPosition = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color[] LineColor
        {
            get
            {
                return lineColor;
            }

            set
            {
                lineColor = value;
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

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color[] ShadowColor
        {
            get
            {
                return shadowColor;
            }

            set
            {
                shadowColor = value;
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

            // Get the line position and size
            switch (separatorOrientation)
            {
                case Orientation.Horizontal:
                    {
                        linePosition = new Point(0, 1);
                        lineSize = new Size(Width, 1);

                        shadowPosition = new Point(0, 2);
                        shadowSize = new Size(Width, 2);
                        break;
                    }

                case Orientation.Vertical:
                    {
                        linePosition = new Point(1, 0);
                        lineSize = new Size(1, Height);

                        shadowPosition = new Point(2, 0);
                        shadowSize = new Size(2, Height);
                        break;
                    }
            }

            // Create line rectangle
            lineRectangle = new Rectangle(linePosition, lineSize);

            startPoint = new Point(ClientRectangle.Width, 0);
            endPoint = new Point(ClientRectangle.Width, ClientRectangle.Width);

            // Create line brush
            gradientBrush = GDI.CreateGradientBrush(lineColor, gradientPosition, gradientAngle, startPoint, endPoint);

            // Draw line
            graphics.DrawRectangle(new Pen(gradientBrush), lineRectangle);

            if (shadowVisible)
            {
                // Create shadow rectangle
                shadowRectangle = new Rectangle(shadowPosition, shadowSize);

                // Create shadow brush
                gradientShadowBrush = GDI.CreateGradientBrush(shadowColor, gradientShadowPosition, gradientAngle, startPoint, endPoint);

                // Draw shadow
                graphics.DrawRectangle(new Pen(gradientShadowBrush), shadowRectangle);
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