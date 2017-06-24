namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Text;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Localization;
    using VisualPlus.Styles;

    #endregion

    [ToolboxItem(false)]
    [ToolboxBitmap(typeof(Component))]
    [DefaultEvent("StyleChanged")]
    [Description("The visual style manager.")]
    public class VisualStylesManager : Component
    {
        #region Variables

        [Browsable(false)]
        public IBorder BorderStyle;

        [Browsable(false)]
        public IControl ControlStyle;

        [Browsable(false)]
        public IFont FontStyle;

        public bool Initialized;

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
        private Styles visualStyle;
        private string watermarkText;
        private bool watermarkVisible;

        #endregion

        #region Constructors

        public VisualStylesManager()
        {
            // Load default style
            visualStyle = Settings.DefaultValue.DefaultStyle;

            // Load settings
            BorderStyle = GetBorderStyle(visualStyle);
            ControlStyle = GetControlStyle(visualStyle);
            FontStyle = GetFontStyle(visualStyle);
            ProgressStyle = GetProgressStyle(visualStyle);
            TabStyle = GetTabStyle(visualStyle);
            WatermarkStyle = GetWatermarkStyle(visualStyle);

            // Apply settings
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

        public delegate void StyleChangedEventHandler(Styles newStyle);

        public event StyleChangedEventHandler StyleChanged;

        #endregion

        #region Properties

        [DefaultValue(Settings.DefaultValue.Animation)]
        [Category(Localize.Category.Behavior)]
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
        [Category(Localize.Category.Behavior)]
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
        [Category(Localize.Category.Behavior)]
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
        [Category(Localize.Category.Layout)]
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
        [Category(Localize.Category.Layout)]
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
        [Category(Localize.Category.Appearance)]
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
        [Category(Localize.Category.Behavior)]
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

        [Category(Localize.Category.Layout)]
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
        [Category(Localize.Category.Behavior)]
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

        [DefaultValue(Settings.DefaultValue.ProgressSize)]
        [Category(Localize.Category.Layout)]
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

        [Category(Localize.Category.Appearance)]
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
        [Category(Localize.Category.Appearance)]
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
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Common.Type)]
        public Styles VisualStyle
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

        protected virtual void OnStyleChanged(Styles newStyle)
        {
            BorderStyle = GetBorderStyle(newStyle);
            ControlStyle = GetControlStyle(newStyle);
            FontStyle = GetFontStyle(newStyle);
            ProgressStyle = GetProgressStyle(newStyle);
            WatermarkStyle = GetWatermarkStyle(newStyle);
            TabStyle = GetTabStyle(newStyle);

            StyleChangedEventHandler msc = VisualButton;
            msc += VisualCheckBox;
            msc(newStyle);

            StyleChanged?.Invoke(newStyle);
        }

        private static IBorder GetBorderStyle(Styles styles)
        {
            IBorder style;

            switch (styles)
            {
                case Styles.Visual:
                    {
                        style = new Visual();
                        break;
                    }

                case Styles.BlackAndYellow:
                    {
                        style = new BlackAndYellow();
                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
            }

            return style;
        }

        private static IControl GetControlStyle(Styles styles)
        {
            IControl style;

            switch (styles)
            {
                case Styles.Visual:
                    {
                        style = new Visual();
                        break;
                    }

                case Styles.BlackAndYellow:
                    {
                        style = new BlackAndYellow();
                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
            }

            return style;
        }

        private static IFont GetFontStyle(Styles styles)
        {
            IFont style;

            switch (styles)
            {
                case Styles.Visual:
                    {
                        style = new Visual();
                        break;
                    }

                case Styles.BlackAndYellow:
                    {
                        style = new BlackAndYellow();
                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
            }

            return style;
        }

        private static IProgress GetProgressStyle(Styles styles)
        {
            IProgress style;

            switch (styles)
            {
                case Styles.Visual:
                    {
                        style = new Visual();
                        break;
                    }

                case Styles.BlackAndYellow:
                    {
                        style = new BlackAndYellow();
                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
            }

            return style;
        }

        private static ITab GetTabStyle(Styles styles)
        {
            ITab style;

            switch (styles)
            {
                case Styles.Visual:
                    {
                        style = new Visual();
                        break;
                    }

                case Styles.BlackAndYellow:
                    {
                        style = new BlackAndYellow();
                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
            }

            return style;
        }

        private static IWatermark GetWatermarkStyle(Styles styles)
        {
            IWatermark style;

            switch (styles)
            {
                case Styles.Visual:
                    {
                        style = new Visual();
                        break;
                    }

                case Styles.BlackAndYellow:
                    {
                        style = new BlackAndYellow();
                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
            }

            return style;
        }

        private void VisualButton(Styles newStyle)
        {
        }

        private void VisualCheckBox(Styles newStyle)
        {
            // Todo
        }

        #endregion
    }
}