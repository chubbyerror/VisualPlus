namespace VisualPlus.Controls
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Styles;

    /// <summary>The visual ContextMenuStrip.</summary>
    [ToolboxBitmap(typeof(ContextMenuStrip)), Designer(VSDesignerBinding.VisualContextMenu)]
    public class VisualContextMenu : ContextMenuStrip
    {
        #region  ${0} Variables

        public static bool animation = true;

        private static readonly IStyle style = new Visual();
        internal VFXManager effectsManager;
        internal Point effectsSource;

        private ToolStripItemClickedEventArgs _delayesArgs;

        #endregion

        #region ${0} Properties

        public VisualContextMenu()
        {
            Renderer = new VisualToolStripRender();

            effectsManager = new VFXManager(false)
                {
                    Increment = 0.07,
                    EffectType = EffectType.Linear
                };
            effectsManager.OnAnimationProgress += sender => Invalidate();
            effectsManager.OnAnimationFinished += sender => OnItemClicked(_delayesArgs);

            BackColor = style.BackgroundColor(0);
        }

        public delegate void ItemClickStart(object sender, ToolStripItemClickedEventArgs e);

        public event ItemClickStart OnItemClickStart;

        #endregion

        #region ${0} Events

        protected override void OnItemClicked(ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem != null && !(e.ClickedItem is ToolStripSeparator))
            {
                if (e == _delayesArgs)
                {
                    // The event has been fired manually because the args are the ones we saved for delay
                    base.OnItemClicked(e);
                }
                else
                {
                    // Interrupt the default on click, saving the args for the delay which is needed to display the animation
                    _delayesArgs = e;

                    // Fire custom event to trigger actions directly but keep cms open
                    OnItemClickStart?.Invoke(this, e);

                    // Start animation
                    effectsManager.StartNewAnimation(AnimationDirection.In);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            effectsSource = e.Location;
        }

        #endregion

        // private Color buttonNormal = style.ButtonNormalColor;
        // private Color buttonPressed = ControlPaint.Light(style.ButtonDownColor);
        // private Color itemHover = ControlPaint.Light(style.ButtonNormalColor);

        // private Color textDisabled = style.TextDisabled;
    }

    public sealed class VisualToolStripMenuItem : ToolStripMenuItem
    {
        #region ${0} Properties

        public VisualToolStripMenuItem()
        {
            AutoSize = false;
            Size = new Size(120, 30);
        }

        #endregion

        #region ${0} Events

        protected override ToolStripDropDown CreateDefaultDropDown()
        {
            ToolStripDropDown baseDropDown = base.CreateDefaultDropDown();
            if (DesignMode)
            {
                return baseDropDown;
            }

            VisualContextMenu defaultDropDown = new VisualContextMenu();
            defaultDropDown.Items.AddRange(baseDropDown.Items);

            return defaultDropDown;
        }

        #endregion
    }

    internal class VisualToolStripRender : ToolStripProfessionalRenderer
    {
        #region  ${0} Variables

        private static readonly IStyle style = new Visual();

        #endregion

        #region ${0} Events

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            const int ARROW_SIZE = 4;

            Point arrowMiddle = new Point(e.ArrowRectangle.X + e.ArrowRectangle.Width / 2, e.ArrowRectangle.Y + e.ArrowRectangle.Height / 2);

            SolidBrush arrowBrush = e.Item.Enabled ? new SolidBrush(Color.Black) : new SolidBrush(Color.Gray);
            using (GraphicsPath arrowPath = new GraphicsPath())
            {
                arrowPath.AddLines(
                    new[]
                        {
                            new Point(arrowMiddle.X - ARROW_SIZE, arrowMiddle.Y - ARROW_SIZE),
                            new Point(arrowMiddle.X, arrowMiddle.Y),
                            new Point(arrowMiddle.X - ARROW_SIZE, arrowMiddle.Y + ARROW_SIZE)
                        });
                arrowPath.CloseFigure();

                g.FillPath(arrowBrush, arrowPath);
            }
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            base.OnRenderImageMargin(e);
        }

        // public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;
        // public MouseState MouseState { get; set; }
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            Rectangle itemRect = GetItemRect(e.Item);
            Rectangle textRect = new Rectangle(24, itemRect.Y, itemRect.Width - (24 + 16), itemRect.Height);

            g.DrawString(
                e.Text,
                Control.DefaultFont,
                e.Item.Enabled ? new SolidBrush(Color.Black) : new SolidBrush(Color.Gray),
                textRect,
                new StringFormat { LineAlignment = StringAlignment.Center });
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.Clear(style.BackgroundColor(0));

            // Draw background
            Rectangle itemRect = GetItemRect(e.Item);

            graphics.FillRectangle(e.Item.Selected && e.Item.Enabled ? new SolidBrush(style.ItemHover(0)) : new SolidBrush(style.BackgroundColor(0)),
                itemRect);

            // Ripple animation
            VisualContextMenu toolStrip = e.ToolStrip as VisualContextMenu;
            if (toolStrip != null)
            {
                VFXManager animationManager = toolStrip.effectsManager;
                Point animationSource = toolStrip.effectsSource;
                if (toolStrip.effectsManager.IsAnimating() && e.Item.Bounds.Contains(animationSource))
                {
                    for (var i = 0; i < animationManager.GetAnimationCount(); i++)
                    {
                        double animationValue = animationManager.GetProgress(i);
                        SolidBrush rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - animationValue * 50), Color.Black));
                        var rippleSize = (int)(animationValue * itemRect.Width * 2.5);
                        graphics.FillEllipse(rippleBrush,
                            new Rectangle(animationSource.X - rippleSize / 2, itemRect.Y - itemRect.Height, rippleSize, itemRect.Height * 3));
                    }
                }
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            Graphics g = e.Graphics;

            g.FillRectangle(new SolidBrush(style.BackgroundColor(0)), e.Item.Bounds);
            g.DrawLine(
                new Pen(style.LineColor),
                new Point(e.Item.Bounds.Left, e.Item.Bounds.Height / 2),
                new Point(e.Item.Bounds.Right, e.Item.Bounds.Height / 2));
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawRectangle(
                new Pen(style.BorderColor(0)),
                new Rectangle(e.AffectedBounds.X, e.AffectedBounds.Y, e.AffectedBounds.Width - 1, e.AffectedBounds.Height - 1));
        }

        private Rectangle GetItemRect(ToolStripItem item)
        {
            return new Rectangle(0, item.ContentRectangle.Y, item.ContentRectangle.Width + 4, item.ContentRectangle.Height);
        }

        #endregion
    }
}