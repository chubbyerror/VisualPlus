namespace VisualPlus.Framework
{
    #region Namespace

    using System;
    using System.Drawing.Text;

    using VisualPlus.Enums;
    using VisualPlus.Framework.Structure;
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

        #region Events

        /// <summary>Gets the style information.</summary>
        /// <param name="styles">Input the style.</param>
        /// <returns>The new style interface.</returns>
        public static IStyle GetStyleSheet(Styles styles)
        {
            IStyle style;

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

        #endregion

        #region Methods

        public struct DefaultValue
        {
            public const bool Animation = true;
            public const int BorderThickness = 1;
            public const bool BorderHoverVisible = true;
            public const int BorderRounding = 6;
            public const BorderShape BorderShape = Structure.BorderShape.Rounded;
            public const bool BorderVisible = true;
            public const bool TextVisible = true;
            public const float ProgressSize = 5F;
            public const bool TitleBoxVisible = true;
            public const bool HatchVisible = true;
            public const int BarAmount = 5;
            public const Styles DefaultStyle = Styles.Visual;
            public const float HatchSize = 2F;
            public const bool Moveable = false;
            public const bool WatermarkVisible = false;
            public static readonly string WatermarkText = "Watermark text";
            public static readonly IStyle Style = new Visual();
            public static TextRenderingHint TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        }

        #endregion
    }
}