namespace VisualPlus.Toolkit.Bases
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.Structure;

    #endregion

    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public abstract class ControlBase : Control, IContainerStyle
    {
        #region Variables

        private Color foreColorDisabled;
        private MouseStates mouseState;
        private StyleManager styleManager;
        private TextRenderingHint textRendererHint;

        #endregion

        #region Constructors

        protected ControlBase()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            DefaultConstructor();

            // DoubleBuffered = true;
            mouseState = MouseStates.Normal;
            styleManager = new StyleManager();
            InitializeTheme();
        }

        public delegate void ForeColorDisabledChangedEventHandler();

        public delegate void MouseStateChangedEventHandler();

        public delegate void StyleManagerChangedEventHandler();

        public delegate void TextRenderingChangedEventHandler();

        [Category(Localize.EventsCategory.PropertyChanged)]
        public event ForeColorDisabledChangedEventHandler ForeColorDisabledChanged;

        [Category(Localize.EventsCategory.Mouse)]
        public event MouseStateChangedEventHandler MouseStateChanged;

        [Category(Localize.EventsCategory.Appearance)]
        public event StyleManagerChangedEventHandler StyleManagerChanged;

        [Category(Localize.EventsCategory.PropertyChanged)]
        public event TextRenderingChangedEventHandler TextRenderingHintChanged;

        #endregion

        #region Properties

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color ForeColorDisabled
        {
            get
            {
                return foreColorDisabled;
            }

            set
            {
                foreColorDisabled = value;
                OnForeColorDisabledChanged();
                Invalidate();
            }
        }

        public Point LastPosition { get; private set; }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.MouseState)]
        public MouseStates State
        {
            get
            {
                return mouseState;
            }

            set
            {
                mouseState = value;
                OnMouseStateChanged();
            }
        }

        [TypeConverter(typeof(StyleManagerConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public StyleManager StyleManager
        {
            get
            {
                return styleManager;
            }

            set
            {
                styleManager = value;
                Invalidate();
                OnStyleManagerChanged();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Strings.TextRenderingHint)]
        public TextRenderingHint TextRenderingHint
        {
            get
            {
                return textRendererHint;
            }

            set
            {
                textRendererHint = value;
                OnTextRenderingHintChanged();
                Invalidate();
            }
        }

        internal Border ControlBorder { get; set; }

        internal Gradient[] ControlBrushCollection { get; set; }

        internal IVisualStyle VisualStyleSheet { get; set; }

        #endregion

        #region Events

        protected virtual void OnForeColorDisabledChanged()
        {
            ForeColorDisabledChanged?.Invoke();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mouseState = MouseStates.Down;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            mouseState = MouseStates.Hover;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            mouseState = MouseStates.Normal;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            LastPosition = e.Location;
        }

        protected virtual void OnMouseStateChanged()
        {
            MouseStateChanged?.Invoke();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mouseState = MouseStates.Hover;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

            InitializeTheme();

            ForeColor = Enabled ? ForeColor : ForeColorDisabled;
        }

        /// <summary>Fires the OnStyleManagerChanged event.</summary>
        protected virtual void OnStyleManagerChanged()
        {
            StyleManagerChanged?.Invoke();
        }

        /// <summary>Fires the OnTextRenderingHintChanged event.</summary>
        protected virtual void OnTextRenderingHintChanged()
        {
            TextRenderingHintChanged?.Invoke();
        }

        private void DefaultConstructor()
        {
            Font = Settings.DefaultValue.DefaultFont;
            ForeColor = Settings.DefaultValue.Font.ForeColor;
            ForeColorDisabled = Settings.DefaultValue.Font.ForeColorDisabled;
            textRendererHint = Settings.DefaultValue.TextRenderingHint;

            ControlBorder = new Border();
            ControlBrushCollection = new[]
                {
                    Settings.DefaultValue.ControlStates.ControlEnabled,
                    Settings.DefaultValue.ControlStates.ControlHover,
                    Settings.DefaultValue.ControlStates.ControlPressed,
                    Settings.DefaultValue.ControlStates.ControlDisabled
                };
        }

        private void InitializeTheme()
        {
            if (styleManager.LockedStyle)
            {
                if (StyleManager.VisualStylesManager != null)
                {
                    VisualStyleSheet = styleManager.VisualStylesManager.VisualStylesInterface;

                    Font = new Font(VisualStyleSheet.FontStyle.FontFamily, VisualStyleSheet.FontStyle.FontSize, VisualStyleSheet.FontStyle.FontStyle);
                    ForeColor = VisualStyleSheet.FontStyle.ForeColor;
                    ForeColorDisabled = VisualStyleSheet.FontStyle.ForeColorDisabled;
                    textRendererHint = styleManager.VisualStylesManager.TextRenderingHint;

                    ControlBorder = new Border
                        {
                            Color = VisualStyleSheet.BorderStyle.Color,
                            HoverColor = VisualStyleSheet.BorderStyle.HoverColor,
                            HoverVisible = StyleManager.VisualStylesManager.BorderHoverVisible,
                            Rounding = StyleManager.VisualStylesManager.BorderRounding,
                            Type = StyleManager.VisualStylesManager.BorderType,
                            Thickness = StyleManager.VisualStylesManager.BorderThickness,
                            Visible = StyleManager.VisualStylesManager.BorderVisible
                        };

                    ControlBrushCollection[0] = VisualStyleSheet.ControlStatesStyle.ControlEnabled;
                    ControlBrushCollection[1] = VisualStyleSheet.ControlStatesStyle.ControlHover;
                    ControlBrushCollection[2] = VisualStyleSheet.ControlStatesStyle.ControlPressed;
                    ControlBrushCollection[3] = VisualStyleSheet.ControlStatesStyle.ControlDisabled;
                }
                else
                {
                    DefaultConstructor();

                    ControlBorder = new Border();

                    ControlBrushCollection[0] = Settings.DefaultValue.ControlStates.ControlEnabled;
                    ControlBrushCollection[1] = Settings.DefaultValue.ControlStates.ControlHover;
                    ControlBrushCollection[2] = Settings.DefaultValue.ControlStates.ControlPressed;
                    ControlBrushCollection[3] = Settings.DefaultValue.ControlStates.ControlDisabled;
                }
            }
        }

        #endregion
    }
}