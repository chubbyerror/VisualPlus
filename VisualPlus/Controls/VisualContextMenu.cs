namespace VisualPlus.Controls
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Framework;

    public class VisualContextMenuStrip : ContextMenuStrip
    {
        public sealed class VisualToolStripMenuItem : ToolStripMenuItem
        {
            #region ${0} Properties

            public VisualToolStripMenuItem()
            {
                AutoSize = false;
                Size = new Size(160, 30);
            }

            #endregion

            #region ${0} Events

            protected override ToolStripDropDown CreateDefaultDropDown()
            {
                if (DesignMode)
                {
                    return base.CreateDefaultDropDown();
                }

                VisualContextMenuStrip defaultDropDown = new VisualContextMenuStrip();
                defaultDropDown.Items.AddRange(base.CreateDefaultDropDown().Items);
                return defaultDropDown;
            }

            #endregion
        }

        public sealed class VisualToolStripRender : ToolStripProfessionalRenderer
        {
            #region ${0} Events

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                int ArrowX = e.ArrowRectangle.X + e.ArrowRectangle.Width / 2;
                int ArrowY = e.ArrowRectangle.Y + e.ArrowRectangle.Height / 2;
                Point[] ArrowPoints =
                    {
                        new Point(ArrowX - 5, ArrowY - 5),
                        new Point(ArrowX, ArrowY),
                        new Point(ArrowX - 5, ArrowY + 5)
                    };
                if (e.Item.Enabled)
                {
                    e.Graphics.FillPolygon(new SolidBrush(StylesManager.DefaultValue.Style.DropDownButtonColor), ArrowPoints);
                }
                else
                {
                    e.Graphics.FillPolygon(new SolidBrush(StylesManager.DefaultValue.Style.ItemDisableBackgroundColor), ArrowPoints);
                }
            }

            protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
            {
                // Allow to add images to ToolStrips
                // MyBase.OnRenderImageMargin(e) 
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                Rectangle textRect = new Rectangle(25, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Width - (24 + 16),
                    e.Item.ContentRectangle.Height - 4);
                using (Font F = new Font("Microsoft Sans Serif", 10, FontStyle.Regular))
                {
                    using (Brush solidBrush = e.Item.Enabled
                                         ? new SolidBrush(StylesManager.DefaultValue.Style.ForeColor(0))
                                         : new SolidBrush(StylesManager.DefaultValue.Style.TextDisabled))
                    {
                        using (StringFormat stringFormat = new StringFormat { LineAlignment = StringAlignment.Center })
                        {
                            e.Graphics.DrawString(e.Text, F, solidBrush, textRect);
                        }
                    }
                }
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                e.Graphics.InterpolationMode = InterpolationMode.High;
                e.Graphics.Clear(StylesManager.DefaultValue.Style.BackgroundColor(0));
                Rectangle R = new Rectangle(0, e.Item.ContentRectangle.Y - 2, e.Item.ContentRectangle.Width + 4, e.Item.ContentRectangle.Height + 3);

                e.Graphics.FillRectangle(
                    e.Item.Selected && e.Item.Enabled
                        ? new SolidBrush(Color.FromArgb(130, StylesManager.DefaultValue.Style.BackgroundColor(1)))
                        : new SolidBrush(StylesManager.DefaultValue.Style.BackgroundColor(0)), R);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawLine(new Pen(Color.FromArgb(200, StylesManager.DefaultValue.Style.BorderColor(0)), 1),
                    new Point(e.Item.Bounds.Left, e.Item.Bounds.Height / 2), new Point(e.Item.Bounds.Right - 5, e.Item.Bounds.Height / 2));
            }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                base.OnRenderToolStripBackground(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.InterpolationMode = InterpolationMode.High;
                e.Graphics.Clear(StylesManager.DefaultValue.Style.BackgroundColor(0));
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                e.Graphics.InterpolationMode = InterpolationMode.High;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle hoverBorder = new Rectangle(e.AffectedBounds.X, e.AffectedBounds.Y, e.AffectedBounds.Width, e.AffectedBounds.Height - 1);
                GraphicsPath borderPath = new GraphicsPath();
                borderPath.AddRectangle(hoverBorder);
                borderPath.CloseAllFigures();

                e.Graphics.SetClip(borderPath);
                e.Graphics.DrawPath(new Pen(StylesManager.DefaultValue.Style.BorderColor(0)), borderPath);
                e.Graphics.ResetClip();
            }

            #endregion
        }

        #region  ${0} Variables

        private ToolStripItemClickedEventArgs clickedEventArgs;

        #endregion

        #region ${0} Properties

        public VisualContextMenuStrip()
        {
            Renderer = new VisualToolStripRender();
            BackColor = StylesManager.DefaultValue.Style.BackgroundColor(0);
        }

        public delegate void ClickedEventHandler(object sender);

        public event ClickedEventHandler Clicked;

        #endregion

        #region ${0} Events

        protected override void OnItemClicked(ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem != null && !(e.ClickedItem is ToolStripSeparator))
            {
                if (ReferenceEquals(e, clickedEventArgs))
                {
                    OnItemClicked(e);
                }
                else
                {
                    clickedEventArgs = e;
                    if (Clicked != null)
                    {
                        Clicked(this);
                    }
                }
            }
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            Cursor = Cursors.Hand;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Cursor = Cursors.Hand;
            Invalidate();
        }

        #endregion
    }
}