namespace VisualPlus.Toolkit.Components
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    #endregion

    /// <summary>The visualContainer component.</summary>
    [ToolboxItem(false)]
    [Description("The Visual Container Component")]
    public sealed partial class VisualContainer : ToolStripDropDown
    {
        #region Variables

        private ToolStripControlHost _controlHost;
        private bool _fade = true;
        private Control _userControl;

        #endregion

        #region Constructors

        public VisualContainer(Control contextControl)
        {
            if (contextControl == null)
            {
                throw new ArgumentNullException("No context control to load." + nameof(contextControl));
            }

            _userControl = contextControl;

            _fade = SystemInformation.IsMenuAnimationEnabled && SystemInformation.IsMenuFadeEnabled;

            // Setup control
            _controlHost = new ToolStripControlHost(contextControl)
                {
                    AutoSize = false
                };

            Padding = Margin = _controlHost.Padding = _controlHost.Margin = Padding.Empty;

            contextControl.Location = Point.Empty;

            Items.Add(_controlHost);

            contextControl.Disposed += delegate
                {
                    contextControl = null;

                    // Disposes after close.
                    Dispose(true);
                };
        }

        #endregion

        #region Events

        public void Show(Control control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            Show(control, control.ClientRectangle);
        }

        public void Show(Form form, Point point)
        {
            Show(form, new Rectangle(point, new Size(0, 0)));
        }

        protected override void OnOpened(EventArgs e)
        {
            _userControl.Focus();

            base.OnOpened(e);
        }

        protected override void OnOpening(CancelEventArgs e)
        {
            if (_userControl.IsDisposed || _userControl.Disposing)
            {
                e.Cancel = true;
                return;
            }

            base.OnOpening(e);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // Prevent ALT from closing it and allow ALT + MNEMONIC to work
            return ((keyData & Keys.Alt) != Keys.Alt) && base.ProcessDialogKey(keyData);
        }

        protected override void SetVisibleCore(bool visible)
        {
            double opacity = Opacity;
            if (visible && _fade)
            {
                Opacity = 0;
            }

            base.SetVisibleCore(visible);
            if (!visible || !_fade)
            {
                return;
            }

            for (var i = 1; i <= _frames; i++)
            {
                if (i > 1)
                {
                    Thread.Sleep(_frameDuration);
                }

                Opacity = (opacity * i) / _frames;
            }

            Opacity = opacity;
        }

        private const int _frameDuration = _totalDuration / _frames;
        private const int _frames = 5;
        private const int _totalDuration = 100;

        private void Show(Control control, Rectangle area)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            Point location = control.PointToScreen(new Point(area.Left, area.Top + area.Height));

            Rectangle screen = Screen.FromControl(control).WorkingArea;

            if (location.X + Size.Width > screen.Left + screen.Width)
            {
                location.X = (screen.Left + screen.Width) - Size.Width;
            }

            if (location.Y + Size.Height > screen.Top + screen.Height)
            {
                location.Y -= Size.Height + area.Height;
            }

            location = control.PointToClient(location);

            Show(control, location, ToolStripDropDownDirection.BelowRight);
        }

        #endregion
    }
}