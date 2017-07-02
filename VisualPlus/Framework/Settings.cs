namespace VisualPlus.Framework
{
    #region Namespace

    using System.Drawing;
    using System.Drawing.Text;

    using VisualPlus.Enums;
    using VisualPlus.Styles;

    #endregion

    internal class Settings
    {
        #region Variables

        public static readonly int MaximumAlpha = 255;
        public static readonly int MaximumBorderSize = 24;
        public static readonly int MaximumCheckBoxBorderRounding = 12;
        public static readonly int MaximumCheckBoxSize = 11;
        public static readonly int MaximumRounding = 30;
        public static readonly int MinimumAlpha = 1;
        public static readonly int MinimumBorderSize = 1;
        public static readonly int MinimumCheckBoxBorderRounding = 1;
        public static readonly int MinimumCheckBoxSize = 3;
        public static readonly int MinimumRounding = 1;

        #endregion

        #region Methods

        public struct DefaultValue
        {
            public const bool Animation = true;
            public const int BorderThickness = 1;
            public const bool BorderHoverVisible = true;
            public const BorderType BorderType = Enums.BorderType.Rounded;
            public const bool BorderVisible = true;
            public const bool TextVisible = true;
            public const float ProgressSize = 5F;
            public const bool TitleBoxVisible = true;
            public const bool HatchVisible = true;
            public const int BarAmount = 5;
            public const Styles.Style DefaultStyle = Styles.Style.Visual;
            public const float HatchSize = 2F;
            public const bool Moveable = false;
            public const bool WatermarkVisible = false;

            public static readonly IBorder Border = (IBorder)Styles.GetInterfaceObject(DefaultStyle);
            public static readonly ICheckmark Checkmark = (ICheckmark)Styles.GetInterfaceObject(DefaultStyle);
            public static readonly IControl Control = (IControl)Styles.GetInterfaceObject(DefaultStyle);
            public static readonly IControlState ControlState = (IControlState)Styles.GetInterfaceObject(DefaultStyle);
            public static readonly IFont Font = (IFont)Styles.GetInterfaceObject(DefaultStyle);
            public static readonly IProgress Progress = (IProgress)Styles.GetInterfaceObject(DefaultStyle);
            public static readonly ITab Tab = (ITab)Styles.GetInterfaceObject(DefaultStyle);
            public static readonly IWatermark Watermark = (IWatermark)Styles.GetInterfaceObject(DefaultStyle);

            public static readonly string WatermarkText = "Watermark text";
            public static TextRenderingHint TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            public static Font DefaultFont = new Font(Font.FontFamily, Font.FontSize, Font.FontStyle);

            public struct Rounding
            {
                public const int Default = 6;
                public const int BoxRounding = 3;
                public const int RoundedRectangle = 12;
                public const int ToggleBorder = 20;
                public const int ToggleButton = 18;
            }
        }

        #endregion
    }
}