namespace VisualPlus.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Localization;

    /// <summary>The visual separator.</summary>
    [ToolboxBitmap(typeof(Control)), Designer(VSDesignerBinding.VisualSeparator)]
    public class VisualSeparator : Control
    {
        #region  ${0} Variables

        private Color lineColor = StylesManager.DefaultValue.Style.LineColor;
        private SplitterStyle separatorStyle = SplitterStyle.Horizontal;
        private Color shadowColor = StylesManager.DefaultValue.Style.ShadowColor;

        #endregion

        #region ${0} Properties

        public VisualSeparator()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;

            UpdateStyles();
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color Line
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

        [Category(Localize.Category.Behavior), Description(Localize.Description.SeparatorStyle)]
        public SplitterStyle SeparatorStyle
        {
            get
            {
                return separatorStyle;
            }

            set
            {
                separatorStyle = value;
                if (value == SplitterStyle.Horizontal)
                {
                    Height = 4;
                }
                else
                {
                    Width = 4;
                }

                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color Shadow
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

        #endregion

        #region ${0} Events

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            Point linePosition = new Point();
            Point lineSize = new Point();
            Point shadowPosition = new Point();
            Point shadowSize = new Point();

            // Get the line position and size
            switch (SeparatorStyle)
            {
                case SplitterStyle.Horizontal:
                    {
                        linePosition = new Point(0, 1);
                        lineSize = new Point(Width, 1);

                        shadowPosition = new Point(0, 2);
                        shadowSize = new Point(Width, 2);
                        break;
                    }

                case SplitterStyle.Vertical:
                    {
                        linePosition = new Point(1, 0);
                        lineSize = new Point(1, Height);

                        shadowPosition = new Point(2, 0);
                        shadowSize = new Point(2, Height);
                        break;
                    }
            }

            // Draw line
            graphics.DrawLine(new Pen(lineColor), linePosition, lineSize);

            // Draw line shadow
            graphics.DrawLine(new Pen(shadowColor), shadowPosition, shadowSize);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (SeparatorStyle == SplitterStyle.Horizontal)
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