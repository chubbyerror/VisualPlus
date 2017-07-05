namespace VisualPlus.Toolkit.Controls
{
    #region Namespace

    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Framework.Structure;
    using VisualPlus.Toolkit.VisualBase;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Panel))]
    [DefaultEvent("Paint")]
    [DefaultProperty("Enabled")]
    [Description("The Visual Panel")]
    public sealed class VisualPanel : ExpandableContainer
    {
        #region Constructors

        public VisualPanel()
        {
            Size = new Size(187, 117);
            Padding = new Padding(5, 5, 5, 5);
            DoubleBuffered = true;

            Expander = new Expandable(this, 22);
        }

        #endregion

        #region Events

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            ControlGraphicsPath = Border.GetBorderShape(ClientRectangle, Border.Type, Border.Rounding);
            graphics.FillPath(new SolidBrush(Background), ControlGraphicsPath);

            Border.DrawBorderStyle(graphics, Border, MouseState, ControlGraphicsPath);

            if (Expander.Visible)
            {
                Expander.Draw(graphics, Expander.GetAlignmentPoint(Size));
            }
        }

        #endregion
    }
}