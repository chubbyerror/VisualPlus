namespace VisualPlus.Toolkit.VisualBase
{
    #region Namespace

    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using VisualPlus.Delegates;
    using VisualPlus.Enumerators;
    using VisualPlus.Managers;
    using VisualPlus.Structure;

    #endregion

    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public abstract class VisualControlBase : Control
    {
        #region Variables

        private Color _foreColorDisabled;
        private MouseStates _mouseState;
        private StyleManager _styleManager;
        private TextRenderingHint _textRendererHint;

        #endregion

        #region Constructors

        protected VisualControlBase()
        {
            // Double buffering to reduce drawing flicker.
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            // Repaint entire control whenever resizing.
            SetStyle(ControlStyles.ResizeRedraw, true);

            // Drawn double buffered by default
            DoubleBuffered = true;
            ResizeRedraw = true;

            _mouseState = MouseStates.Normal;
            _styleManager = new StyleManager(Settings.DefaultValue.DefaultStyle);

            Font = _styleManager.Font;
            ForeColor = _styleManager.FontStyle.ForeColor;
            ForeColorDisabled = _styleManager.FontStyle.ForeColorDisabled;
            _textRendererHint = Settings.DefaultValue.TextRenderingHint;

            ControlBorder = new Border();

            ControlBrushCollection = new[]
                {
                    _styleManager.ControlStatesStyle.ControlEnabled,
                    _styleManager.ControlStatesStyle.ControlHover,
                    _styleManager.ControlStatesStyle.ControlPressed,
                    _styleManager.ControlStatesStyle.ControlDisabled
                };
        }

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description("Occours when the ForeColorDisabled property for the control has changed.")]
        public event ForeColorDisabledChangedEventHandler ForeColorDisabledChanged;

        [Category(Localize.EventsCategory.Mouse)]
        [Description("Occours when the MouseState of the control has changed.")]
        public event MouseStateChangedEventHandler MouseStateChanged;

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description("Occours when the TextRenderingHint property has changed.")]
        public event TextRenderingChangedEventHandler TextRenderingHintChanged;

        #endregion

        #region Properties

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.AutoSize)]
        public new bool AutoSize { get; set; }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color ForeColorDisabled
        {
            get
            {
                return _foreColorDisabled;
            }

            set
            {
                _foreColorDisabled = value;
                OnForeColorDisabledChanged();
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.MouseState)]
        public MouseStates MouseState
        {
            get
            {
                return _mouseState;
            }

            set
            {
                _mouseState = value;
                OnMouseStateChanged();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Strings.TextRenderingHint)]
        public TextRenderingHint TextRenderingHint
        {
            get
            {
                return _textRendererHint;
            }

            set
            {
                _textRendererHint = value;
                OnTextRenderingHintChanged();
                Invalidate();
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal Border ControlBorder { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal Gradient[] ControlBrushCollection { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal GraphicsPath ControlGraphicsPath { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal Point LastPosition { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal StyleManager StyleManager
        {
            get
            {
                return _styleManager;
            }

            set
            {
                _styleManager = value;
            }
        }

        #endregion

        #region Events

        /// <summary>Update the internal style manager and invalidate.</summary>
        /// <param name="style">The style.</param>
        public void UpdateTheme(Styles style)
        {
            _styleManager.UpdateStyle(style);
            Invalidate();
        }

        protected virtual void OnForeColorDisabledChanged()
        {
            ForeColorDisabledChanged?.Invoke();
        }

        protected virtual void OnMouseStateChanged()
        {
            MouseStateChanged?.Invoke();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = _textRendererHint;

            ForeColor = Enabled ? ForeColor : ForeColorDisabled;
        }

        protected virtual void OnTextRenderingHintChanged()
        {
            TextRenderingHintChanged?.Invoke();
        }

        #endregion
    }
}