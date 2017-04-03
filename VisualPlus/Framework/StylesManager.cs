namespace VisualPlus.Framework
{
    using System.Drawing;

    using VisualPlus.Enums;
    using VisualPlus.Framework.Styles;

    public enum Style
    {
        // Note: If you implement more styles or your own styles, just add enumeration here.

        /// <summary>Visual Style.</summary>
        Visual = 0,

        /// <summary>Black and Yellow Style.</summary>
        BlackAndYellow = 1
    }

    internal class StylesManager
    {
        #region  ${0} Variables

        public static readonly Color MainColor = Color.Green;
        public static readonly int MaximumBorderSize = 24;
        public static readonly int MaximumCheckBoxBorderRounding = 12;

        public static readonly int MaximumCheckBoxSize = 11;

        public static readonly int MaximumRounding = 30;

        public static readonly int MinimumBorderSize = 1;

        public static readonly int MinimumCheckBoxBorderRounding = 1;
        public static readonly int MinimumCheckBoxSize = 3;
        public static readonly int MinimumRounding = 1;

        #endregion

        #region ${0} Structures

        public struct DefaultValue
        {
            public const bool Animation = true;
            public const int BorderSize = 1;
            public const bool BorderHoverVisible = true;
            public const int BorderRounding = 5;
            public const BorderShape BorderShape = Enums.BorderShape.Rounded;
            public const bool BorderVisible = true;
            public const bool TextVisible = true;
            public const float ProgressSize = 5F;
            public const bool TitleBoxVisible = true;
            public const bool HatchVisible = true;
            public const int BarAmount = 5;
            public const float HatchSize = 2F;

            public static readonly IStyle Style = new Visual();

            public static Color TextColor = Color.Black;
        }

        #endregion
    }
}