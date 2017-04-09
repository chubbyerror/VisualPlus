namespace VisualPlus.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Drawing.Text;
    using VisualPlus.Framework;
    using VisualPlus.Localization;

    [ToolboxBitmap(typeof(Label))]
    public class VisualLabel : Label
    {
        #region  ${0} Variables

        private Color hoverColor = StylesManager.DefaultValue.Style.BorderColor(1);
        private bool hoverVisible;
        private TextRenderingHint textRendererHint = TextRenderingHint.AntiAlias;
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

        [DefaultValue(StylesManager.DefaultValue.BorderHoverVisible), Category(Localize.Category.Behavior),
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

        [Category(Localize.Category.Appearance), Description("Visual Text Renderer.")]
        public TextRenderingHint TextRendering { get { return textRendererHint; } set { textRendererHint = value; Invalidate(); } }

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
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = textRendererHint;
            base.OnPaint(e);
        }

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
            ForeColor = StylesManager.DefaultValue.Style.ForeColor(0);
        }

        #endregion
    }
}