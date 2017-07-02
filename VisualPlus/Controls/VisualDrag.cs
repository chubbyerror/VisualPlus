namespace VisualPlus.Controls
{
    #region Namespace

    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Windows.Forms;

    using VisualPlus.Framework;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Panel))]
    [DefaultEvent("MouseMove")]
    [DefaultProperty("Enabled")]
    [Description("The Visual Drag")]
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public sealed class VisualDrag : Control
    {
        #region Variables

        private Point _lastPos;
        private bool _movable = true;

        #endregion

        #region Constructors

        public VisualDrag()
        {
            Cursor = Cursors.Hand;
        }

        #endregion

        #region Properties

        [DefaultValue(true)]
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Toggle)]
        public bool Movable
        {
            get
            {
                return _movable;
            }

            set
            {
                _movable = value;
                Cursor = _movable ? Cursors.Hand : Cursors.Default;
            }
        }

        #endregion

        #region Events

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (_movable)
            {
                _lastPos = e.Location;
                Cursor = Cursors.SizeAll;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_movable && (e.Button == MouseButtons.Left))
            {
                Left += e.Location.X - _lastPos.X;
                Top += e.Location.Y - _lastPos.Y;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_movable)
            {
                Cursor = Cursors.Hand;
            }
        }

        #endregion
    }
}