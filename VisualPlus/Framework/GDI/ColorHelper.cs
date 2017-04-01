namespace VisualPlus.Framework.GDI
{
    using System.Drawing;

    internal class ColorHelper
    {
        public static string ColorToHtml(Color color)
        {
            return ColorTranslator.ToHtml(color);
        }

        public static Color FromHtml(string withouthHash)
        {
            return ColorTranslator.FromHtml("#" + withouthHash);
        }
    }
}