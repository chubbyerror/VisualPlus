namespace VisualPlus.Toolkit.Controls
{
    #region Namespace

    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Framework;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Toolkit.VisualBase;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Panel))]
    [DefaultEvent("Paint")]
    [DefaultProperty("Enabled")]
    [Description("The Visual Panel")]
    public sealed class VisualPanel : ContainerBase
    {
        #region Variables

        private Drag _drag;
        private Expandable _expander;

        #endregion

        #region Constructors

        public VisualPanel()
        {
            Size = new Size(187, 117);
            Padding = new Padding(5, 5, 5, 5);
            DoubleBuffered = true;

            _drag = new Drag(this, Settings.DefaultValue.Moveable);
            _expander = new Expandable(this, 22);
        }

        #endregion

        #region Properties

        [TypeConverter(typeof(DragConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Behavior)]
        public Drag Drag
        {
            get
            {
                return _drag;
            }

            set
            {
                _drag = value;
            }
        }

        [TypeConverter(typeof(ExpandableConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Behavior)]
        public Expandable Expandable
        {
            get
            {
                return _expander;
            }

            set
            {
                _expander = value;
            }
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

            _expander.Draw(graphics, _expander.GetAlignmentPoint(Size));
        }

        #endregion
    }
}