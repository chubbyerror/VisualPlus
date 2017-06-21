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
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;
    using VisualPlus.Styles;

    #endregion

    /// <summary>The visual Toggle.</summary>
    [ToolboxBitmap(typeof(Component))]
    [DefaultEvent("StyleChanged")]
    [Description("The visual style manager.")]
    public class VisualStylesManager : Component
    {
        #region Variables

        [Browsable(false)]
        public ITab TabStyle;

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
        public IWatermark WatermarkStyle;

        #endregion

        #region Variables

        private bool animation = Settings.DefaultValue.Animation;
        private int barAmount = Settings.DefaultValue.BarAmount;
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        private int borderRounding = Settings.DefaultValue.Rounding.Default;
        private BorderShape borderShape = Settings.DefaultValue.BorderShape;
        private int borderThickness = Settings.DefaultValue.BorderThickness;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private float hatchSize = Settings.DefaultValue.HatchSize;
        private bool hatchVisible = Settings.DefaultValue.HatchVisible;
        private float progressSize = Settings.DefaultValue.ProgressSize;
        private Color styleColor = Color.Green;
        private TextRenderingHint textRenderingHint = Settings.DefaultValue.TextRenderingHint;
        private bool textVisible = Settings.DefaultValue.TextVisible;
        private Styles visualStyle;
        private string watermarkText = Settings.DefaultValue.WatermarkText;
        private bool watermarkVisible = Settings.DefaultValue.WatermarkVisible;

        #endregion

        #region Constructors

        public VisualStylesManager()
        {
            // Load default style
            visualStyle = Settings.DefaultValue.DefaultStyle;

            BorderStyle = GetBorderStyle(visualStyle);
            ControlStyle = GetControlStyle(visualStyle);
            FontStyle = GetFontStyle(visualStyle);
            ProgressStyle = GetProgressStyle(visualStyle);
            WatermarkStyle = GetWatermarkStyle(visualStyle);

            Initialized = true;
        }

        public delegate void StyleChangedEventHandler(Styles newStyle);

        public event StyleChangedEventHandler StyleChanged;

        #endregion

        #region Properties

        [DefaultValue(Settings.DefaultValue.Animation)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.Animation)]
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
        [Description(Localize.Description.BarAmount)]
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
        [Description(Localize.Description.BorderHoverVisible)]
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
        [Description(Localize.Description.BorderRounding)]
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

        [DefaultValue(Settings.DefaultValue.BorderShape)]
        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentShape)]
        public BorderShape BorderShape
        {
            get
            {
                return borderShape;
            }

            set
            {
                borderShape = value;
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
                if (ExceptionManager.ArgumentOutOfRangeException(value, Settings.MinimumBorderSize, Settings.MaximumBorderSize))
                {
                    borderThickness = value;
                }
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
            }
        }

        [Category(Localize.Category.Layout)]
        [DefaultValue(Settings.DefaultValue.HatchSize)]
        [Description(Localize.Description.HatchSize)]
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
        [Description(Localize.Description.ComponentVisible)]
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
        [Description(Localize.Description.ProgressSize)]
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
        [Description(Localize.Description.StyleColor)]
        public Color StyleColor
        {
            get
            {
                return styleColor;
            }

            set
            {
                styleColor = value;
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.TextRenderingHint)]
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
        [Description(Localize.Description.TextVisible)]
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
        [Description(Localize.Description.Style)]
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