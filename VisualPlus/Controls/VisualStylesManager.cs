namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Text;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
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

        public bool Initialized;

        #endregion

        #region Variables

        private bool animation = Settings.DefaultValue.Animation;
        private int barAmount = Settings.DefaultValue.BarAmount;
        private bool borderHoverVisible = Settings.DefaultValue.BorderHoverVisible;
        private int borderRounding = Settings.DefaultValue.BorderRounding;
        private BorderShape borderShape = Settings.DefaultValue.BorderShape;
        private int borderThickness = Settings.DefaultValue.BorderThickness;
        private bool borderVisible = Settings.DefaultValue.BorderVisible;
        private Styles currentStyle = Settings.DefaultValue.DefaultStyle;
        private float hatchSize = Settings.DefaultValue.HatchSize;
        private bool hatchVisible = Settings.DefaultValue.HatchVisible;
        private float progressSize = Settings.DefaultValue.ProgressSize;
        private IStyle style = Settings.DefaultValue.Style;
        private Color styleColor = Settings.DefaultValue.Style.StyleColor;
        private TextRenderingHint textRenderingHint = Settings.DefaultValue.TextRenderingHint;
        private bool textVisible = Settings.DefaultValue.TextVisible;

        #endregion

        #region Constructors

        public VisualStylesManager()
        {
            Initialized = true;
        }

        public delegate void StyleChangedEventHandler();

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
                if (ExceptionHandler.ArgumentOutOfRangeException(value, Settings.MinimumBorderSize, Settings.MaximumBorderSize))
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
        [Description(Localize.Description.Style)]
        public Styles Style
        {
            get
            {
                return currentStyle;
            }

            set
            {
                currentStyle = value;

                // Change the style
                StyleChanged?.Invoke();
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

        #endregion

        #region Events

        public event StyleChangedEventHandler StyleChanged;

        #endregion

        #region Methods

        public interface IComponent : IDisposable
        {
            #region Properties

            ISite Site { get; set; }

            #endregion

            #region Events

            event EventHandler Disposed;

            #endregion
        }

        #endregion
    }
}