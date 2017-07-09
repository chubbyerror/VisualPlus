namespace VisualPlus.Toolkit.VisualBase
{
    #region Namespace

    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using VisualPlus.Framework;
    using VisualPlus.Framework.Structure;

    #endregion

    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public sealed class RepeatButton : ButtonContentBase, IControlStates
    {
        #region Variables

        private Point textPoint = new Point(0, 0);

        #endregion

        #region Constructors

        public RepeatButton()
        {
            // TODO: Mini-replica of a VisualButton. To be used as buttons for VisualNumericUp/Down, VisualTrackBar and etc.
            AutoSize = false;
            BackColor = Color.Transparent;
            Margin = new Padding(4, 6, 4, 6);
            Padding = new Padding(0);
            Size = new Size(140, 45);
            MinimumSize = new Size(90, 25);
            DoubleBuffered = true;
        }

        #endregion

        #region Properties

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Border Border
        {
            get
            {
                return ControlBorder;
            }

            set
            {
                ControlBorder = value;
                Invalidate();
            }
        }

        [Description(Localize.Description.Common.ColorGradient)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient DisabledGradient
        {
            get
            {
                return ControlBrushCollection[3];
            }

            set
            {
                ControlBrushCollection[3] = value;
            }
        }

        [Description(Localize.Description.Common.ColorGradient)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient EnabledGradient
        {
            get
            {
                return ControlBrushCollection[0];
            }

            set
            {
                ControlBrushCollection[0] = value;
            }
        }

        [Description(Localize.Description.Common.ColorGradient)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient HoverGradient
        {
            get
            {
                return ControlBrushCollection[1];
            }

            set
            {
                ControlBrushCollection[1] = value;
            }
        }

        [Description(Localize.Description.Common.ColorGradient)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Gradient PressedGradient
        {
            get
            {
                return ControlBrushCollection[2];
            }

            set
            {
                ControlBrushCollection[2] = value;
            }
        }

        #endregion

        #region Events

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            DrawBackground(e.Graphics);
            graphics.DrawString(Text, Font, new SolidBrush(ForeColor), textPoint);
        }

        private void DrawBackground(Graphics graphics)
        {
            LinearGradientBrush controlGraphicsBrush = GDI.GetControlBrush(graphics, Enabled, MouseState, ControlBrushCollection, ClientRectangle);
            GDI.FillBackground(graphics, ControlGraphicsPath, controlGraphicsBrush);
            Border.DrawBorderStyle(graphics, Border, MouseState, ControlGraphicsPath);
        }

        #endregion
    }
}