namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Controls.Bases;
    using VisualPlus.Framework;
    using VisualPlus.Framework.Structure;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Panel))]
    [DefaultEvent("Paint")]
    [DefaultProperty("Enabled")]
    [Description("The Visual Panel")]
    public sealed class VisualPanel : ContainerBase
    {
        #region Variables

        private GraphicsPath controlGraphicsPath;
        private Expander expander;

        #endregion

        #region Constructors

        public VisualPanel()
        {
            Size = new Size(187, 117);
            Padding = new Padding(5, 5, 5, 5);
            DoubleBuffered = true;

            expander = new Expander(this, 22);
        }

        [Description("Occours when the expander toggle has changed.")]
        public delegate void ToggleChangedEventHandler();

        public event ToggleChangedEventHandler ToggleExpanderChanged;

        #endregion

        #region Properties

        [TypeConverter(typeof(ExpanderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Expander Expander
        {
            get
            {
                return expander;
            }

            set
            {
                expander = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (expander.MouseOnButton)
            {
                expander.Expanded = !expander.Expanded;
                ToggleExpanderChanged?.Invoke();
            }
            else
            {
                Focus();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (expander.Visible)
            {
                expander.GetMouseOnButton(e.Location);
                Cursor = expander.MouseOnButton ? expander.Cursor : Cursors.Default;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            controlGraphicsPath = Border.GetBorderShape(ClientRectangle, Border.Type, Border.Rounding);
            graphics.FillPath(new SolidBrush(Background), controlGraphicsPath);

            Border.DrawBorderStyle(graphics, Border, State, controlGraphicsPath);

            if (expander.Visible)
            {
                Point buttonPoint = expander.GetAlignmentPoint(Size);
                expander.Draw(graphics, buttonPoint);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            expander?.UpdateOriginal(Size);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            expander?.UpdateOriginal(Size);
        }

        #endregion
    }
}