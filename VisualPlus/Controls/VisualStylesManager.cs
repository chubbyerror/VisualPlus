namespace VisualPlus.Controls
{
    #region Namespace

    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Text;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Styles;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Component))]
    [DefaultEvent("StyleChanged")]
    [Description("The visual style manager.")]
    public class VisualStylesManager : Component
    {
        #region Variables

        [Browsable(false)]
        public IBorder BorderStyle;

        [Browsable(false)]
        public ICheckmark CheckmarkStyle;

        [Browsable(false)]
        public IControl ControlStyle;

        [Browsable(false)]
        public IControlState ControlStateStyle;

        [Browsable(false)]
        public IFont FontStyle;

        [Browsable(false)]
        public IProgress ProgressStyle;

        [Browsable(false)]
        public ITab TabStyle;

        [Browsable(false)]
        public IWatermark WatermarkStyle;

        #endregion

        #region Variables

        private bool animation;
        private int barAmount;
        private bool borderHoverVisible;
        private int borderRounding;
        private int borderThickness;
        private BorderType borderType;
        private bool borderVisible;
        private float hatchSize;
        private bool hatchVisible;
        private float progressSize;
        private TextRenderingHint textRenderingHint;
        private bool textVisible;
        private Styles.Style visualStyle;
        private string watermarkText;
        private bool watermarkVisible;

        #endregion

        #region Constructors

        public VisualStylesManager()
        {
            // Load default style
            visualStyle = Settings.DefaultValue.DefaultStyle;

            // Load style
            LoadStyleSettings(visualStyle);

            // Load settings
            animation = Settings.DefaultValue.Animation;
            barAmount = Settings.DefaultValue.BarAmount;
            borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
            borderRounding = Settings.DefaultValue.Rounding.Default;
            borderThickness = Settings.DefaultValue.BorderThickness;
            borderType = Settings.DefaultValue.BorderType;
            borderVisible = Settings.DefaultValue.BorderVisible;
            hatchSize = Settings.DefaultValue.HatchSize;
            hatchVisible = Settings.DefaultValue.HatchVisible;
            progressSize = Settings.DefaultValue.ProgressSize;
            textRenderingHint = Settings.DefaultValue.TextRenderingHint;
            textVisible = Settings.DefaultValue.TextVisible;
            watermarkText = Settings.DefaultValue.WatermarkText;
            watermarkVisible = Settings.DefaultValue.WatermarkVisible;

            Initialized = true;
        }

        public delegate void StyleChangedEventHandler(Styles.Style newStyle);

        public event StyleChangedEventHandler StyleChanged;

        #endregion

        #region Properties

        [DefaultValue(Settings.DefaultValue.Animation)]
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Animation)]
        public bool Animation
        {
            get
            {
                return animation;
            }

            set
            {
                animation = value;
            }
        }

        [DefaultValue(Settings.DefaultValue.BarAmount)]
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Progressbar.Bars)]
        public int BarAmount
        {
            get
            {
                return barAmount;
            }

            set
            {
                barAmount = value;
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderHoverVisible)]
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Visible)]
        public bool BorderHoverVisible
        {
            get
            {
                return borderHoverVisible;
            }

            set
            {
                borderHoverVisible = value;
            }
        }

        [DefaultValue(Settings.DefaultValue.Rounding.Default)]
        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Border.Rounding)]
        public int BorderRounding
        {
            get
            {
                return borderRounding;
            }

            set
            {
                if (ExceptionManager.ArgumentOutOfRangeException(value, Settings.MinimumRounding, Settings.MaximumRounding))
                {
                    borderRounding = value;
                }
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderThickness)]
        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Border.Thickness)]
        public int BorderThickness
        {
            get
            {
                return borderThickness;
            }

            set
            {
                if (ExceptionManager.ArgumentOutOfRangeException(value, Settings.MinimumBorderSize, Settings.MaximumBorderSize))
                {
                    borderThickness = value;
                }
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderType)]
        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Type)]
        public BorderType BorderType
        {
            get
            {
                return borderType;
            }

            set
            {
                borderType = value;
            }
        }

        [DefaultValue(Settings.DefaultValue.BorderVisible)]
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Visible)]
        public bool BorderVisible
        {
            get
            {
                return borderVisible;
            }

            set
            {
                borderVisible = value;
            }
        }

        [Category(Localize.PropertiesCategory.Layout)]
        [DefaultValue(Settings.DefaultValue.HatchSize)]
        [Description(Localize.Description.Common.Size)]
        public float HatchSize
        {
            get
            {
                return hatchSize;
            }

            set
            {
                hatchSize = value;
            }
        }

        [DefaultValue(Settings.DefaultValue.HatchVisible)]
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Visible)]
        public bool HatchVisible
        {
            get
            {
                return hatchVisible;
            }

            set
            {
                hatchVisible = value;
            }
        }

        public bool Initialized { get; }

        [DefaultValue(Settings.DefaultValue.ProgressSize)]
        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Size)]
        public float ProgressSize
        {
            get
            {
                return progressSize;
            }

            set
            {
                progressSize = value;
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Strings.TextRenderingHint)]
        public TextRenderingHint TextRenderingHint
        {
            get
            {
                return textRenderingHint;
            }

            set
            {
                textRenderingHint = value;
            }
        }

        [DefaultValue(Settings.DefaultValue.TextVisible)]
        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Visible)]
        public bool TextVisible
        {
            get
            {
                return textVisible;
            }

            set
            {
                textVisible = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Type)]
        public Styles.Style VisualStyle
        {
            get
            {
                return visualStyle;
            }

            set
            {
                visualStyle = value;
                OnStyleChanged(visualStyle);
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("The watermark text.")]
        public string WatermarkText
        {
            get
            {
                return watermarkText;
            }

            set
            {
                watermarkText = value;
            }
        }

        [NotifyParentProperty(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Watermark visible toggle.")]
        public bool WatermarkVisible
        {
            get
            {
                return watermarkVisible;
            }

            set
            {
                watermarkVisible = value;
            }
        }

        #endregion

        #region Events

        protected virtual void OnStyleChanged(Styles.Style newStyle)
        {
            LoadStyleSettings(newStyle);

            StyleChangedEventHandler msc = VisualButton;
            msc += VisualCheckBox;
            msc(newStyle);

            StyleChanged?.Invoke(newStyle);
        }

        /// <summary>Loads the themes style.</summary>
        /// <param name="style">The style.</param>
        private void LoadStyleSettings(Styles.Style style)
        {
            BorderStyle = (IBorder)Styles.GetInterfaceObject(style);
            CheckmarkStyle = (ICheckmark)Styles.GetInterfaceObject(style);
            ControlStyle = (IControl)Styles.GetInterfaceObject(style);
            ControlStateStyle = (IControlState)Styles.GetInterfaceObject(style);
            FontStyle = (IFont)Styles.GetInterfaceObject(style);
            ProgressStyle = (IProgress)Styles.GetInterfaceObject(style);
            TabStyle = (ITab)Styles.GetInterfaceObject(style);
            WatermarkStyle = (IWatermark)Styles.GetInterfaceObject(style);
        }

        private void VisualButton(Styles.Style newStyle)
        {
        }

        private void VisualCheckBox(Styles.Style newStyle)
        {
            // Todo
        }

        #endregion
    }
}