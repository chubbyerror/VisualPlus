namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Localization;
    using VisualPlus.Properties;

    #endregion

    [ToolboxBitmap(typeof(Form))]
    public class VisualForm : Form
    {
        #region Variables

        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_MOUSEMOVE = 0x0200;

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int WM_RBUTTONDOWN = 0x0204;

        #endregion

        #region Variables

        private const int ACTION_BAR_HEIGHT = 40;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;

        private const int MONITOR_DEFAULTTONEAREST = 2;

        private int STATUS_BAR_BUTTON_WIDTH = controlBarHeight;

        private const uint TPM_LEFTALIGN = 0x0000;
        private const uint TPM_RETURNCMD = 0x0100;

        private const int WM_SYSCOMMAND = 0x0112;
        private const int WMSZ_BOTTOM = 6;
        private const int WMSZ_BOTTOMLEFT = 7;
        private const int WMSZ_BOTTOMRIGHT = 8;
        private const int WMSZ_LEFT = 1;
        private const int WMSZ_RIGHT = 2;

        private const int WMSZ_TOP = 3;
        private const int WMSZ_TOPLEFT = 4;
        private const int WMSZ_TOPRIGHT = 5;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int WS_SYSMENU = 0x00080000;
        private static int controlBarHeight = 24;

        private readonly Dictionary<int, int> _resizingLocationsToCmd = new Dictionary<int, int>
            {
                { HTTOP, WMSZ_TOP },
                { HTTOPLEFT, WMSZ_TOPLEFT },
                { HTTOPRIGHT, WMSZ_TOPRIGHT },
                { HTLEFT, WMSZ_LEFT },
                { HTRIGHT, WMSZ_RIGHT },
                { HTBOTTOM, WMSZ_BOTTOM },
                { HTBOTTOMLEFT, WMSZ_BOTTOMLEFT },
                { HTBOTTOMRIGHT, WMSZ_BOTTOMRIGHT }
            };

        private readonly Cursor[] resizeCursors = { Cursors.SizeNESW, Cursors.SizeWE, Cursors.SizeNWSE, Cursors.SizeWE, Cursors.SizeNS };

        private Rectangle actionBarBounds;
        private Color borderColor = Settings.DefaultValue.Style.BorderColor(0);
        private int borderRounding = Settings.DefaultValue.BorderRounding;
        private BorderShape borderShape = Settings.DefaultValue.BorderShape;
        private int borderThickness = Settings.DefaultValue.BorderThickness;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private ButtonState buttonState = ButtonState.None;
        private Color controlBoxItems = Settings.DefaultValue.Style.ForeColor(0);
        private ControlState controlState = ControlState.Normal;
        private int form_padding = 2;
        private Color formTitle = Settings.DefaultValue.Style.ForeColor(0);
        private bool headerMouseDown;
        private Color headerTitle = Settings.DefaultValue.Style.ButtonNormalColor;

        private Image icon = Resources.Icon;
        private bool iconBorder;
        private GraphicsPath iconGraphicsPath;
        private Point iconPoint = new Point(0, 0);
        private Rectangle iconRectangle;
        private Size iconSize = new Size(16, 16);
        private Rectangle maxButtonBounds;
        private bool maximized;
        private Rectangle minButtonBounds;
        private Point previousLocation;
        private Size previousSize;
        private ResizeDirection resizeDir;
        private Rectangle statusBarBounds;

        private Size titleTextSize;
        private Rectangle xButtonBounds;

        #endregion

        #region Constructors

        public VisualForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            Sizable = true;
            DoubleBuffered = true;

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            Text = "Visual Form";

            // This enables the form to trigger the MouseMove event even when mouse is over another control
            Application.AddMessageFilter(new MouseMessageFilter());
            MouseMessageFilter.MouseMove += OnGlobalMouseMove;
        }

        public enum ButtonState
        {
            /// <summary>The x over.</summary>
            XOver,

            /// <summary>The max over.</summary>
            MaxOver,

            /// <summary>The min over.</summary>
            MinOver,

            /// <summary>The x down.</summary>
            XDown,

            /// <summary>The max down.</summary>
            MaxDown,

            /// <summary>The min down.</summary>
            MinDown,

            /// <summary>None.</summary>
            None
        }

        #endregion

        #region Properties

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.BorderColor)]
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }

            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderRounding)]
        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.BorderRounding)]
        public int BorderRounding
        {
            get
            {
                return borderRounding;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, Settings.MinimumRounding, Settings.MaximumRounding))
                {
                    borderRounding = value;
                }

                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderThickness)]
        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.BorderThickness)]
        public int BorderThickness
        {
            get
            {
                return borderThickness;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, Settings.MinimumBorderSize, Settings.MaximumBorderSize))
                {
                    borderThickness = value;
                }

                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.BorderVisible)]
        public bool BorderVisible
        {
            get
            {
                return borderVisible;
            }

            set
            {
                borderVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.ComponentSize)]
        public int ControlBarHeight
        {
            get
            {
                return controlBarHeight;
            }

            set
            {
                controlBarHeight = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Icon)]
        public new Image Icon
        {
            get
            {
                return icon;
            }

            set
            {
                icon = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.BorderVisible)]
        public bool IconBorder
        {
            get
            {
                return iconBorder;
            }

            set
            {
                iconBorder = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.IconSize)]
        public Size IconSize
        {
            get
            {
                return iconSize;
            }

            set
            {
                iconSize = value;
                Invalidate();
            }
        }

        public bool Sizable { get; set; }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams par = base.CreateParams;

                // WS_SYSMENU: Trigger the creation of the system menu
                // WS_MINIMIZEBOX: Allow minimizing from taskbar
                par.Style = par.Style | WS_MINIMIZEBOX | WS_SYSMENU; // Turn on the WS_MINIMIZEBOX style flag
                return par;
            }
        }

        #endregion

        #region Events

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(HandleRef hmonitor, [In] [Out] MonitorInfo info);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (DesignMode)
            {
                return;
            }

            UpdateButtons(e);

            if (e.Button == MouseButtons.Left && !maximized)
            {
                ResizeForm(resizeDir);
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (DesignMode)
            {
                return;
            }

            buttonState = ButtonState.None;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (DesignMode)
            {
                return;
            }

            if (Sizable)
            {
                // True if the mouse is hovering over a child control
                bool isChildUnderMouse = GetChildAtPoint(e.Location) != null;

                if (e.Location.X < borderThickness && e.Location.Y > Height - borderThickness && !isChildUnderMouse && !maximized)
                {
                    resizeDir = ResizeDirection.BottomLeft;
                    Cursor = Cursors.SizeNESW;
                }
                else if (e.Location.X < borderThickness && !isChildUnderMouse && !maximized)
                {
                    resizeDir = ResizeDirection.Left;
                    Cursor = Cursors.SizeWE;
                }
                else if (e.Location.X > Width - borderThickness && e.Location.Y > Height - borderThickness && !isChildUnderMouse && !maximized)
                {
                    resizeDir = ResizeDirection.BottomRight;
                    Cursor = Cursors.SizeNWSE;
                }
                else if (e.Location.X > Width - borderThickness && !isChildUnderMouse && !maximized)
                {
                    resizeDir = ResizeDirection.Right;
                    Cursor = Cursors.SizeWE;
                }
                else if (e.Location.Y > Height - borderThickness && !isChildUnderMouse && !maximized)
                {
                    resizeDir = ResizeDirection.Bottom;
                    Cursor = Cursors.SizeNS;
                }
                else
                {
                    resizeDir = ResizeDirection.None;

                    // Only reset the cursor when needed, this prevents it from flickering when a child control changes the cursor to its own needs
                    if (((IList)resizeCursors).Contains(Cursor))
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }

            UpdateButtons(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (DesignMode)
            {
                return;
            }

            UpdateButtons(e, true);

            base.OnMouseUp(e);
            ReleaseCapture();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            graphics.Clear(BackColor);

            // sub-header
            // g.FillRectangle(new SolidBrush(Color.Blue), _actionBarBounds);

            // Draw border
            if (borderVisible)
            {
                using (Pen borderPen = new Pen(borderColor, borderThickness))
                {
                    graphics.DrawLine(borderPen, new Point(0, actionBarBounds.Bottom), new Point(0, Height - 2));
                    graphics.DrawLine(borderPen, new Point(Width - 1, actionBarBounds.Bottom), new Point(Width - 1, Height - 2));
                    graphics.DrawLine(borderPen, new Point(0, Height - 1), new Point(Width - 1, Height - 1));
                }
            }

            // Title box
            graphics.FillRectangle(new SolidBrush(headerTitle), statusBarBounds);

            // Determine whether or not we even should be drawing the buttons.
            bool showMin = MinimizeBox && ControlBox;
            bool showMax = MaximizeBox && ControlBox;
            SolidBrush hoverBrush = new SolidBrush(Settings.DefaultValue.Style.ButtonHoverColor);
            SolidBrush downBrush = new SolidBrush(Settings.DefaultValue.Style.ButtonDownColor);

            // When MaximizeButton == false, the minimize button will be painted in its place
            if (buttonState == ButtonState.MinOver && showMin)
            {
                graphics.FillRectangle(hoverBrush, showMax ? minButtonBounds : maxButtonBounds);
            }

            if (buttonState == ButtonState.MinDown && showMin)
            {
                graphics.FillRectangle(downBrush, showMax ? minButtonBounds : maxButtonBounds);
            }

            if (buttonState == ButtonState.MaxOver && showMax)
            {
                graphics.FillRectangle(hoverBrush, maxButtonBounds);
            }

            if (buttonState == ButtonState.MaxDown && showMax)
            {
                graphics.FillRectangle(downBrush, maxButtonBounds);
            }

            if (buttonState == ButtonState.XOver && ControlBox)
            {
                graphics.FillRectangle(hoverBrush, xButtonBounds);
            }

            if (buttonState == ButtonState.XDown && ControlBox)
            {
                graphics.FillRectangle(downBrush, xButtonBounds);
            }

            using (Pen formButtonsPen = new Pen(controlBoxItems, 2))
            {
                // Minimize button.
                if (showMin)
                {
                    int x = showMax ? minButtonBounds.X : maxButtonBounds.X;
                    int y = showMax ? minButtonBounds.Y : maxButtonBounds.Y;

                    graphics.DrawLine(
                        formButtonsPen,
                        x + (int)(minButtonBounds.Width * 0.33),
                        y + (int)(minButtonBounds.Height * 0.66),
                        x + (int)(minButtonBounds.Width * 0.66),
                        y + (int)(minButtonBounds.Height * 0.66)
                    );
                }

                // Maximize button
                if (showMax)
                {
                    graphics.DrawRectangle(
                        formButtonsPen,
                        maxButtonBounds.X + (int)(maxButtonBounds.Width * 0.33),
                        maxButtonBounds.Y + (int)(maxButtonBounds.Height * 0.36),
                        (int)(maxButtonBounds.Width * 0.39),
                        (int)(maxButtonBounds.Height * 0.31)
                    );
                }

                // Close button
                if (ControlBox)
                {
                    graphics.DrawLine(
                        formButtonsPen,
                        xButtonBounds.X + (int)(xButtonBounds.Width * 0.33),
                        xButtonBounds.Y + (int)(xButtonBounds.Height * 0.33),
                        xButtonBounds.X + (int)(xButtonBounds.Width * 0.66),
                        xButtonBounds.Y + (int)(xButtonBounds.Height * 0.66)
                    );

                    graphics.DrawLine(
                        formButtonsPen,
                        xButtonBounds.X + (int)(xButtonBounds.Width * 0.66),
                        xButtonBounds.Y + (int)(xButtonBounds.Height * 0.33),
                        xButtonBounds.X + (int)(xButtonBounds.Width * 0.33),
                        xButtonBounds.Y + (int)(xButtonBounds.Height * 0.66));
                }
            }

            iconPoint = new Point(5, statusBarBounds.Height / 2 - iconSize.Height / 2);
            iconRectangle = new Rectangle(iconPoint, iconSize);
            iconGraphicsPath = new GraphicsPath();
            iconGraphicsPath.AddRectangle(iconRectangle);
            iconGraphicsPath.CloseAllFigures();

            titleTextSize = GDI.GetTextSize(graphics, Text, Font);

            if (ShowIcon)
            {
                if (icon != null)
                {
                    // Update point
                    iconRectangle.Location = iconPoint;

                    // Draw icon border
                    if (iconBorder)
                    {
                        graphics.DrawPath(new Pen(borderColor), iconGraphicsPath);
                    }

                    // Draw icon
                    graphics.DrawImage(Icon, iconRectangle);
                }
            }
            
            // Form title
            graphics.DrawString(Text, Font, new SolidBrush(formTitle), new Rectangle(5 + iconSize.Width + 5, statusBarBounds.X + statusBarBounds.Height / 2 - titleTextSize.Height / 2, Width, titleTextSize.Height), new StringFormat { LineAlignment = StringAlignment.Center });
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            minButtonBounds = new Rectangle(Width - form_padding / 2 - 3 * STATUS_BAR_BUTTON_WIDTH, 0, STATUS_BAR_BUTTON_WIDTH, controlBarHeight);
            maxButtonBounds = new Rectangle(Width - form_padding / 2 - 2 * STATUS_BAR_BUTTON_WIDTH, 0, STATUS_BAR_BUTTON_WIDTH, controlBarHeight);
            xButtonBounds = new Rectangle(Width - form_padding / 2 - STATUS_BAR_BUTTON_WIDTH, 0, STATUS_BAR_BUTTON_WIDTH, controlBarHeight);
            statusBarBounds = new Rectangle(0, 0, Width, controlBarHeight);

            // _actionBarBounds = new Rectangle(0, STATUS_BAR_HEIGHT, Width, ACTION_BAR_HEIGHT);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (DesignMode || IsDisposed)
            {
                return;
            }

            if (m.Msg == WM_LBUTTONDBLCLK)
            {
                MaximizeWindow(!maximized);
            }
            else if (m.Msg == WM_MOUSEMOVE && maximized &&
                     (statusBarBounds.Contains(PointToClient(Cursor.Position)) || actionBarBounds.Contains(PointToClient(Cursor.Position))) &&
                     !(minButtonBounds.Contains(PointToClient(Cursor.Position)) || maxButtonBounds.Contains(PointToClient(Cursor.Position)) || xButtonBounds.Contains(PointToClient(Cursor.Position))))
            {
                if (headerMouseDown)
                {
                    maximized = false;
                    headerMouseDown = false;

                    Point mousePoint = PointToClient(Cursor.Position);
                    if (mousePoint.X < Width / 2)
                    {
                        Location = mousePoint.X < previousSize.Width / 2 ? new Point(Cursor.Position.X - mousePoint.X, Cursor.Position.Y - mousePoint.Y) : new Point(Cursor.Position.X - previousSize.Width / 2, Cursor.Position.Y - mousePoint.Y);
                    }
                    else
                    {
                        Location = Width - mousePoint.X < previousSize.Width / 2 ? new Point(Cursor.Position.X - previousSize.Width + Width - mousePoint.X, Cursor.Position.Y - mousePoint.Y) : new Point(Cursor.Position.X - previousSize.Width / 2, Cursor.Position.Y - mousePoint.Y);
                    }

                    Size = previousSize;
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
            else if (m.Msg == WM_LBUTTONDOWN &&
                     (statusBarBounds.Contains(PointToClient(Cursor.Position)) || actionBarBounds.Contains(PointToClient(Cursor.Position))) &&
                     !(minButtonBounds.Contains(PointToClient(Cursor.Position)) || maxButtonBounds.Contains(PointToClient(Cursor.Position)) || xButtonBounds.Contains(PointToClient(Cursor.Position))))
            {
                if (!maximized)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
                else
                {
                    headerMouseDown = true;
                }
            }
            else if (m.Msg == WM_RBUTTONDOWN)
            {
                Point cursorPos = PointToClient(Cursor.Position);

                if (statusBarBounds.Contains(cursorPos) && !minButtonBounds.Contains(cursorPos) &&
                    !maxButtonBounds.Contains(cursorPos) && !xButtonBounds.Contains(cursorPos))
                {
                    // Show default system menu when right clicking titlebar
                    int id = TrackPopupMenuEx(GetSystemMenu(Handle, false), TPM_LEFTALIGN | TPM_RETURNCMD, Cursor.Position.X, Cursor.Position.Y, Handle, IntPtr.Zero);

                    // Pass the command as a WM_SYSCOMMAND message
                    SendMessage(Handle, WM_SYSCOMMAND, id, 0);
                }
            }
            else if (m.Msg == WM_NCLBUTTONDOWN)
            {
                // This re-enables resizing by letting the application know when the
                // user is trying to resize a side. This is disabled by default when using WS_SYSMENU.
                if (!Sizable)
                {
                    return;
                }

                byte bFlag = 0;

                // Get which side to resize from
                if (_resizingLocationsToCmd.ContainsKey((int)m.WParam))
                {
                    bFlag = (byte)_resizingLocationsToCmd[(int)m.WParam];
                }

                if (bFlag != 0)
                {
                    SendMessage(Handle, WM_SYSCOMMAND, 0xF000 | bFlag, (int)m.LParam);
                }
            }
            else if (m.Msg == WM_LBUTTONUP)
            {
                headerMouseDown = false;
            }
        }

        private void MaximizeWindow(bool maximize)
        {
            if (!MaximizeBox || !ControlBox)
            {
                return;
            }

            maximized = maximize;

            if (maximize)
            {
                IntPtr monitorHandle = MonitorFromWindow(Handle, MONITOR_DEFAULTTONEAREST);
                MonitorInfo monitorInfo = new MonitorInfo();
                GetMonitorInfo(new HandleRef(null, monitorHandle), monitorInfo);
                previousSize = Size;
                previousLocation = Location;
                Size = new Size(monitorInfo.rcWork.Width(), monitorInfo.rcWork.Height());
                Location = new Point(monitorInfo.rcWork.left, monitorInfo.rcWork.top);
            }
            else
            {
                Size = previousSize;
                Location = previousLocation;
            }
        }

        private void OnGlobalMouseMove(object sender, MouseEventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }

            // Convert to client position and pass to Form.MouseMove
            Point clientCursorPos = PointToClient(e.Location);
            MouseEventArgs newE = new MouseEventArgs(MouseButtons.None, 0, clientCursorPos.X, clientCursorPos.Y, 0);
            OnMouseMove(newE);
        }

        private void ResizeForm(ResizeDirection direction)
        {
            if (DesignMode)
            {
                return;
            }

            int dir = -1;
            switch (direction)
            {
                case ResizeDirection.BottomLeft:
                    dir = HTBOTTOMLEFT;
                    break;
                case ResizeDirection.Left:
                    dir = HTLEFT;
                    break;
                case ResizeDirection.Right:
                    dir = HTRIGHT;
                    break;
                case ResizeDirection.BottomRight:
                    dir = HTBOTTOMRIGHT;
                    break;
                case ResizeDirection.Bottom:
                    dir = HTBOTTOM;
                    break;
            }

            ReleaseCapture();
            if (dir != -1)
            {
                SendMessage(Handle, WM_NCLBUTTONDOWN, dir, 0);
            }
        }

        private void UpdateButtons(MouseEventArgs e, bool up = false)
        {
            if (DesignMode)
            {
                return;
            }

            ButtonState oldState = buttonState;
            bool showMin = MinimizeBox && ControlBox;
            bool showMax = MaximizeBox && ControlBox;

            if (e.Button == MouseButtons.Left && !up)
            {
                if (showMin && !showMax && maxButtonBounds.Contains(e.Location))
                {
                    buttonState = ButtonState.MinDown;
                }
                else if (showMin && showMax && minButtonBounds.Contains(e.Location))
                {
                    buttonState = ButtonState.MinDown;
                }
                else if (showMax && maxButtonBounds.Contains(e.Location))
                {
                    buttonState = ButtonState.MaxDown;
                }
                else if (ControlBox && xButtonBounds.Contains(e.Location))
                {
                    buttonState = ButtonState.XDown;
                }
                else
                {
                    buttonState = ButtonState.None;
                }
            }
            else
            {
                if (showMin && !showMax && maxButtonBounds.Contains(e.Location))
                {
                    buttonState = ButtonState.MinOver;

                    if (oldState == ButtonState.MinDown && up)
                    {
                        WindowState = FormWindowState.Minimized;
                    }
                }
                else if (showMin && showMax && minButtonBounds.Contains(e.Location))
                {
                    buttonState = ButtonState.MinOver;

                    if (oldState == ButtonState.MinDown && up)
                    {
                        WindowState = FormWindowState.Minimized;
                    }
                }
                else if (MaximizeBox && ControlBox && maxButtonBounds.Contains(e.Location))
                {
                    buttonState = ButtonState.MaxOver;

                    if (oldState == ButtonState.MaxDown && up)
                    {
                        MaximizeWindow(!maximized);
                    }
                }
                else if (ControlBox && xButtonBounds.Contains(e.Location))
                {
                    buttonState = ButtonState.XOver;

                    if (oldState == ButtonState.XDown && up)
                    {
                        Close();
                    }
                }
                else
                {
                    buttonState = ButtonState.None;
                }
            }

            if (oldState != buttonState)
            {
                Invalidate();
            }
        }

        #endregion

        #region Methods

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        public class MonitorInfo
        {
            #region Variables

            public int cbSize = Marshal.SizeOf(typeof(MonitorInfo));
            public int dwFlags = 0;
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szDevice = new char[32];

            #endregion
        }

        public class MouseMessageFilter : IMessageFilter
        {
            #region Variables

            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == WM_MOUSEMOVE)
                {
                    if (MouseMove != null)
                    {
                        int x = MousePosition.X, y = MousePosition.Y;

                        MouseMove(null, new MouseEventArgs(MouseButtons.None, 0, x, y, 0));
                    }
                }

                return false;
            }

            #endregion

            #region Variables

            private const int WM_MOUSEMOVE = 0x0200;

            #endregion

            #region Events

            public static event MouseEventHandler MouseMove;

            #endregion
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public int Width()
            {
                return right - left;
            }

            public int Height()
            {
                return bottom - top;
            }
        }

        internal enum ResizeDirection
        {
            BottomLeft,
            Left,
            Right,
            BottomRight,
            Bottom,
            None
        }

        private enum ControlBoxAlignment
        {
            /// <summary>The bottom.</summary>
            Bottom,

            /// <summary>The center.</summary>
            Center,

            /// <summary>The top.</summary>
            Top
        }

        #endregion
    }
}