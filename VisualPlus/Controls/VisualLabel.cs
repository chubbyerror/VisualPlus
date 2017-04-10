namespace VisualPlus.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using VisualPlus.Framework;
    using VisualPlus.Localization;

    [ToolboxBitmap(typeof(Label))]
    public class VisualLabel : Label
    {
        #region  ${0} Variables

        private Color hoverColor = Settings.DefaultValue.Style.BorderColor(1);
        private bool hoverVisible;

        #endregion

        #region ${0} Properties

        public VisualLabel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            UpdateStyles();

            BackColor = Color.Transparent;
        }

        [DefaultValue(Settings.DefaultValue.BorderHoverVisible), Category(Localize.Category.Behavior),
         Description(Localize.Description.BorderHoverVisible)]
        public bool BorderHoverVisible
        {
            get
            {
                return hoverVisible;
            }

            set
            {
                hoverVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.HoverColor)]
        public Color HoverColor
        {
            get
            {
                return hoverColor;
            }

            set
            {
                hoverColor = value;
                Invalidate();
            }
        }

        #endregion

        #region ${0} Events

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            if (hoverVisible)
            {
                ForeColor = hoverColor;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            ForeColor = Settings.DefaultValue.Style.ForeColor(0);
        }

        #endregion
    }
}