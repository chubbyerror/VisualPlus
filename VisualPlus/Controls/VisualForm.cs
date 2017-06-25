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
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;
    using VisualPlus.Properties;

    #endregion

    [ToolboxItem(false)]
    [ToolboxBitmap(typeof(Form))]
    [Description("The Visual Form")]
    [Designer(ControlManager.FilterProperties.VisualForm)]
    public class VisualForm : Form
    {
        #region Variables
        private readonly MouseState mouseState;
        [Category(Localize.Category.Appearance)]
        public MouseStates MouseState
        {
            get
            {
                return mouseState.State;
            }

            set
            {
                mouseState.State = value;
                Invalidate();
            }
        }

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
        private Border border = new Border();
        private Color buttonBackHoverColor = Settings.DefaultValue.Control.ControlHover.Colors[0];
        private Color buttonBackPressedColor = Settings.DefaultValue.Control.ControlPressed.Colors[0];
        private Color buttonColor = Settings.DefaultValue.Control.FlatButtonEnabled;
        private Size buttonSize = new Size(25, 25);
        private ButtonState buttonState = ButtonState.None;
        // private ControlState controlState = ControlState.Normal;
        private bool headerMouseDown;
        private Rectangle maxButtonBounds;
        private bool maximized;
        private Rectangle minButtonBounds;
        private Point previousLocation;
        private Size previousSize;
        private ResizeDirection resizeDir;
        private Rectangle statusBarBounds;
        private Size titleTextSize;
        private VisualImage vsImage = new VisualImage(Resources.Icon, new Size(16, 16));
        private Color windowBarColor = Settings.DefaultValue.Control.Background(1);
        private int windowBarHeight = 30;
        private Rectangle xButtonBounds;

        #endregion

        #region Constructors

        public VisualForm()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            FormBorderStyle = FormBorderStyle.None;
            Sizable = true;
            mouseState = new MouseState(this);
            // Padding-Left: 5 for icon
            Padding = new Padding(5, 0, 0, 0);

            border.Thickness = 3;
            border.Type = BorderType.Rectangle;

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

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Border Border
        {
            get
            {
                return border;
            }

            set
            {
                border = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color ButtonBackHoverColor
        {
            get
            {
                return buttonBackHoverColor;
            }

            set
            {
                buttonBackHoverColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color ButtonBackPressedColor
        {
            get
            {
                return buttonBackPressedColor;
            }

            set
            {
                buttonBackPressedColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color ButtonColor
        {
            get
            {
                return buttonColor;
            }

            set
            {
                buttonColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.Common.Size)]
        public Size ButtonSize
        {
            get
            {
                return buttonSize;
            }

            set
            {
                buttonSize = value;
                Invalidate();
            }
        }

        public new Image Icon
        {
            get
            {
                return vsImage.Image;
            }

            set
            {
                vsImage.Image = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public VisualImage Image
        {
            get
            {
                return vsImage;
            }

            set
            {
                vsImage = value;
                Invalidate();
            }
        }

        public bool Sizable { get; set; }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color WindowBarColor
        {
            get
            {
                return windowBarColor;
            }

            set
            {
                windowBarColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Layout)]
        [Description(Localize.Description.Common.Size)]
        public int WindowBarHeight
        {
            get
            {
                return windowBarHeight;
            }

            set
            {
                windowBarHeight = value;
                Invalidate();
            }
        }

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

        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int WM_RBUTTONDOWN = 0x0204;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (DesignMode)
            {
                return;
            }

            UpdateButtons(e);

            if ((e.Button == MouseButtons.Left) && !maximized)
            {
                ResizeForm(resizeDir);
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            mouseState.State = MouseStates.Hover;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (DesignMode)
            {
                return;
            }

            buttonState = ButtonState.None;
            mouseState.State = MouseStates.Normal;
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

                if ((e.Location.X < border.Thickness) && (e.Location.Y > Height - border.Thickness) && !isChildUnderMouse && !maximized)
                {
                    resizeDir = ResizeDirection.BottomLeft;
                    Cursor = Cursors.SizeNESW;
                }
                else if ((e.Location.X < border.Thickness) && !isChildUnderMouse && !maximized)
                {
                    resizeDir = ResizeDirection.Left;
                    Cursor = Cursors.SizeWE;
                }
                else if ((e.Location.X > Width - border.Thickness) && (e.Location.Y > Height - border.Thickness) && !isChildUnderMouse && !maximized)
                {
                    resizeDir = ResizeDirection.BottomRight;
                    Cursor = Cursors.SizeNWSE;
                }
                else if ((e.Location.X > Width - border.Thickness) && !isChildUnderMouse && !maximized)
                {
                    resizeDir = ResizeDirection.Right;
                    Cursor = Cursors.SizeWE;
                }
                else if ((e.Location.Y > Height - border.Thickness) && !isChildUnderMouse && !maximized)
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
            graphics.Clear(BackColor);
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            // Title box
            graphics.FillRectangle(new SolidBrush(windowBarColor), statusBarBounds);

            DrawButtons(graphics);
            DrawIcon(graphics);
            DrawTitle(graphics);
            DrawBorder(graphics);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            minButtonBounds = new Rectangle(Width - Padding.Right - (3 * buttonSize.Width), (Padding.Top + (windowBarHeight / 2)) - (buttonSize.Height / 2), buttonSize.Width, buttonSize.Height);

            maxButtonBounds = new Rectangle(Width - Padding.Right - (2 * buttonSize.Width), (Padding.Top + (windowBarHeight / 2)) - (buttonSize.Height / 2), buttonSize.Width, buttonSize.Height);

            xButtonBounds = new Rectangle(Width - Padding.Right - buttonSize.Width, (Padding.Top + (windowBarHeight / 2)) - (buttonSize.Height / 2), buttonSize.Width, buttonSize.Height);

            statusBarBounds = new Rectangle(0, 0, Width, windowBarHeight);
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
            else if ((m.Msg == WM_MOUSEMOVE) && maximized &&
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
                        Location = mousePoint.X < previousSize.Width / 2 ? new Point(Cursor.Position.X - mousePoint.X, Cursor.Position.Y - mousePoint.Y) : new Point(Cursor.Position.X - (previousSize.Width / 2), Cursor.Position.Y - mousePoint.Y);
                    }
                    else
                    {
                        Location = Width - mousePoint.X < previousSize.Width / 2 ? new Point(((Cursor.Position.X - previousSize.Width) + Width) - mousePoint.X, Cursor.Position.Y - mousePoint.Y) : new Point(Cursor.Position.X - (previousSize.Width / 2), Cursor.Position.Y - mousePoint.Y);
                    }

                    Size = previousSize;
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
            else if ((m.Msg == WM_LBUTTONDOWN) &&
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

        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;

        private const int MONITOR_DEFAULTTONEAREST = 2;

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

        private void DrawBorder(Graphics graphics)
        {
            if (border.Visible)
            {
                GraphicsPath clientPath = new GraphicsPath();
                clientPath.AddRectangle(ClientRectangle);

                GDI.DrawBorderType(graphics, mouseState.State, clientPath, border.Thickness, border.Color, border.HoverColor, border.HoverVisible);
            }
        }

        private void DrawButtons(Graphics graphics)
        {
            // Determine whether or not we even should be drawing the buttons.
            bool showMin = MinimizeBox && ControlBox;
            bool showMax = MaximizeBox && ControlBox;
            SolidBrush hoverBrush = new SolidBrush(buttonBackHoverColor);
            SolidBrush downBrush = new SolidBrush(buttonBackPressedColor);

            // When MaximizeButton == false, the minimize button will be painted in its place
            DrawMinimizeOverMaximizeButton(graphics, showMin, showMax, hoverBrush, downBrush);

            using (Pen formButtonsPen = new Pen(buttonColor, 2))
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
                        y + (int)(minButtonBounds.Height * 0.66));
                }

                // Maximize button
                if (showMax)
                {
                    graphics.DrawRectangle(
                        formButtonsPen,
                        maxButtonBounds.X + (int)(maxButtonBounds.Width * 0.33),
                        maxButtonBounds.Y + (int)(maxButtonBounds.Height * 0.36),
                        (int)(maxButtonBounds.Width * 0.39),
                        (int)(maxButtonBounds.Height * 0.31));
                }

                // Close button
                if (ControlBox)
                {
                    graphics.DrawLine(
                        formButtonsPen,
                        xButtonBounds.X + (int)(xButtonBounds.Width * 0.33),
                        xButtonBounds.Y + (int)(xButtonBounds.Height * 0.33),
                        xButtonBounds.X + (int)(xButtonBounds.Width * 0.66),
                        xButtonBounds.Y + (int)(xButtonBounds.Height * 0.66));

                    graphics.DrawLine(
                        formButtonsPen,
                        xButtonBounds.X + (int)(xButtonBounds.Width * 0.66),
                        xButtonBounds.Y + (int)(xButtonBounds.Height * 0.33),
                        xButtonBounds.X + (int)(xButtonBounds.Width * 0.33),
                        xButtonBounds.Y + (int)(xButtonBounds.Height * 0.66));
                }
            }
        }

        private void DrawIcon(Graphics graphics)
        {
            vsImage.Point = new Point(Padding.Left, (statusBarBounds.Height / 2) - (vsImage.Size.Height / 2));
            VisualImage.DrawImage(graphics, vsImage.Border, vsImage.Point, vsImage.Image, vsImage.Size, vsImage.Visible);
        }

        private void DrawMinimizeOverMaximizeButton(Graphics graphics, bool showMin, bool showMax, SolidBrush hoverBrush, SolidBrush downBrush)
        {
            if ((buttonState == ButtonState.MinOver) && showMin)
            {
                graphics.FillRectangle(hoverBrush, showMax ? minButtonBounds : maxButtonBounds);
            }

            if ((buttonState == ButtonState.MinDown) && showMin)
            {
                graphics.FillRectangle(downBrush, showMax ? minButtonBounds : maxButtonBounds);
            }

            if ((buttonState == ButtonState.MaxOver) && showMax)
            {
                graphics.FillRectangle(hoverBrush, maxButtonBounds);
            }

            if ((buttonState == ButtonState.MaxDown) && showMax)
            {
                graphics.FillRectangle(downBrush, maxButtonBounds);
            }

            if ((buttonState == ButtonState.XOver) && ControlBox)
            {
                graphics.FillRectangle(hoverBrush, xButtonBounds);
            }

            if ((buttonState == ButtonState.XDown) && ControlBox)
            {
                graphics.FillRectangle(downBrush, xButtonBounds);
            }
        }

        private void DrawTitle(Graphics graphics)
        {
            titleTextSize = GDI.GetTextSize(graphics, Text, Font);
            Rectangle textRectangle = new Rectangle(5 + vsImage.Size.Width + 5, (windowBarHeight / 2) - (titleTextSize.Height / 2), Width, titleTextSize.Height);
            graphics.DrawString(Text, Font, new SolidBrush(ForeColor), textRectangle, new StringFormat { LineAlignment = StringAlignment.Center });
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

            if ((e.Button == MouseButtons.Left) && !up)
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

                    if ((oldState == ButtonState.MinDown) && up)
                    {
                        WindowState = FormWindowState.Minimized;
                    }
                }
                else if (showMin && showMax && minButtonBounds.Contains(e.Location))
                {
                    buttonState = ButtonState.MinOver;

                    if ((oldState == ButtonState.MinDown) && up)
                    {
                        WindowState = FormWindowState.Minimized;
                    }
                }
                else if (MaximizeBox && ControlBox && maxButtonBounds.Contains(e.Location))
                {
                    buttonState = ButtonState.MaxOver;

                    if ((oldState == ButtonState.MaxDown) && up)
                    {
                        MaximizeWindow(!maximized);
                    }
                }
                else if (ControlBox && xButtonBounds.Contains(e.Location))
                {
                    buttonState = ButtonState.XOver;

                    if ((oldState == ButtonState.XDown) && up)
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

            #region Constructors

            public static event MouseEventHandler MouseMove;

            #endregion

            #region Events

            private const int WM_MOUSEMOVE = 0x0200;

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