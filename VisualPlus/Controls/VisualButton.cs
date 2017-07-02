namespace VisualPlus.Controls
{
    #region Namespace

    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using VisualPlus.Framework;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;

    using ButtonBase = VisualPlus.Controls.Bases.ButtonBase;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Button))]
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    [Description("The Visual Button")]
    [Designer(ControlManager.FilterProperties.VisualButton)]
    public sealed class VisualButton : ButtonBase
    {
        #region Variables

        private bool moveable = Settings.DefaultValue.Moveable;

        #endregion

        #region Constructors

        public VisualButton()
        {
            AutoSize = false;
            BackColor = Color.Transparent;
            Margin = new Padding(4, 6, 4, 6);
            Padding = new Padding(0);
            Size = new Size(140, 45);
            MinimumSize = new Size(90, 25);
            DoubleBuffered = true;
        }

        public delegate void ControlMovedEventHandler();

        public event ControlMovedEventHandler ControlMoved;

        #endregion

        #region Properties

        [DefaultValue(Settings.DefaultValue.Moveable)]
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Toggle)]
        public bool Moveable
        {
            get
            {
                return moveable;
            }

            set
            {
                moveable = value;
            }
        }

        #endregion

        #region Events

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            this.ToggleMove(moveable);
            if (moveable)
            {
                ControlMoved?.Invoke();
            }
        }

        #endregion
    }
}