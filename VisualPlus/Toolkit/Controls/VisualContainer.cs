namespace VisualPlus.Toolkit.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    #endregion

    [ToolboxItem(false)]
    [Description("The Visual Container")]
    public sealed partial class VisualContainer : ToolStripDropDown
    {
        #region Variables

        private bool m_fade = true;
        private ToolStripControlHost m_host;
        private Control mContextContainer;

        #endregion

        #region Constructors

        public VisualContainer(Control contextControl)
        {
            if (contextControl == null)
            {
                throw new ArgumentNullException("No context control to load." + nameof(contextControl));
            }

            mContextContainer = contextControl;

            m_fade = SystemInformation.IsMenuAnimationEnabled && SystemInformation.IsMenuFadeEnabled;

            // Setup control
            m_host = new ToolStripControlHost(contextControl)
                {
                    AutoSize = false
                };

            Padding = Margin = m_host.Padding = m_host.Margin = Padding.Empty;

            contextControl.Location = Point.Empty;

            Items.Add(m_host);

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
            mContextContainer.Focus();

            base.OnOpened(e);
        }

        protected override void OnOpening(CancelEventArgs e)
        {
            if (mContextContainer.IsDisposed || mContextContainer.Disposing)
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
            if (visible && m_fade)
            {
                Opacity = 0;
            }

            base.SetVisibleCore(visible);
            if (!visible || !m_fade)
            {
                return;
            }

            for (var i = 1; i <= frames; i++)
            {
                if (i > 1)
                {
                    Thread.Sleep(frameduration);
                }

                Opacity = (opacity * i) / frames;
            }

            Opacity = opacity;
        }

        private const int frameduration = totalduration / frames;
        private const int frames = 5;
        private const int totalduration = 100;

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