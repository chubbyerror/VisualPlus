namespace VisualPlus.Toolkit.VisualBase
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using VisualPlus.Delegates;
    using VisualPlus.Enumerators;
    using VisualPlus.EventArgs;
    using VisualPlus.Structure;

    #endregion

    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public abstract class SimpleBase : VisualControlBase
    {
        #region Variables

        private Color _backgroundColor;
        private Color _backgroundDisabledColor;

        #endregion

        #region Constructors

        protected SimpleBase()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            _backgroundColor = StyleManager.ControlStyle.Background(0);
            _backgroundDisabledColor = StyleManager.FontStyle.ForeColorDisabled;
        }

        [Category(Localize.EventsCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public event BackgroundChangedEventHandler BackgroundChanged;

        [Category(Localize.EventsCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public event BackgroundChangedEventHandler BackgroundDisabledChanged;

        #endregion

        #region Properties

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color Background
        {
            get
            {
                return _backgroundColor;
            }

            set
            {
                _backgroundColor = value;
                OnBackgroundChanged(new ColorEventArgs(_backgroundColor));
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color BackgroundDisabled
        {
            get
            {
                return _backgroundDisabledColor;
            }

            set
            {
                _backgroundDisabledColor = value;
                OnBackgroundDisabledChanged(new ColorEventArgs(_backgroundDisabledColor));
                Invalidate();
            }
        }

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

        #endregion

        #region Events

        protected virtual void OnBackgroundChanged(ColorEventArgs e)
        {
            BackgroundChanged?.Invoke(e);
        }

        protected virtual void OnBackgroundDisabledChanged(ColorEventArgs e)
        {
            BackgroundDisabledChanged?.Invoke(e);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            MouseState = MouseStates.Hover;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            MouseState = MouseStates.Normal;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Color stateColor = Enabled ? _backgroundColor : _backgroundDisabledColor;
            ControlGraphicsPath = Border.GetBorderShape(ClientRectangle, ControlBorder);

            Graphics graphics = e.Graphics;
            graphics.SetClip(ControlGraphicsPath);
            graphics.FillRectangle(new SolidBrush(stateColor), ClientRectangle);
            graphics.ResetClip();
            Border.DrawBorderStyle(graphics, ControlBorder, MouseState, ControlGraphicsPath);
        }

        #endregion
    }
}