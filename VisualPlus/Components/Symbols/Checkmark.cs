namespace VisualPlus.Components.Symbols
{
    #region Namespace

    using System;
    using System.Drawing;
    using System.IO;

    #endregion

    public class Checkmark
    {
        #region Variables

        public static char CheckChar = '✔';

        public static Image CheckImage = Image.FromStream(new MemoryStream(Convert.FromBase64String(GetBase64CheckImage())));

        #endregion

        #region Events

        /// <summary>Draws a checkmark.</summary>
        /// <param name="graphics">Graphics processor.</param>
        /// <param name="font">The font.</param>
        /// <param name="color">The color.</param>
        /// <param name="point">The location.</param>
        /// <param name="checkMark">The character.</param>
        public static void DrawCheckMark(Graphics graphics, Font font, Brush color, PointF point, char checkMark)
        {
            graphics.DrawString(checkMark.ToString(), font, color, point);
        }

        public static string GetBase64CheckImage()
        {
            return
                "iVBORw0KGgoAAAANSUhEUgAAABMAAAAQCAYAAAD0xERiAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAEySURBVDhPY/hPRUBdw/79+/efVHz77bf/X37+wRAn2bDff/7+91l+83/YmtsYBpJs2ITjz/8rTbrwP2Dlrf9XXn5FkSPJsD13P/y3nHsVbNjyy28w5Ik27NWXX//TNt8DG1S19zFWNRiGvfzy8//ccy9RxEB4wvFnYIMMZl7+//brLwx5EEYx7MP33/9dF18Ha1py8RVcHBR7mlMvgsVXX8X0Hgwz/P379z8yLtz5AKxJdcpFcBj9+v3nf/CqW2Cx5E13UdSiYwzDvv36/d9/BUSzzvRL/0t2PQSzQd57+vEHilp0jGEYCJ9+8hnuGhiee+4Vhjp0jNUwEN566/1/m/mQZJC/48H/zz9+YVWHjHEaBsKgwAZ59eH771jl0TFew0D48osvWMWxYYKGEY///gcAqiuA6kEmfEMAAAAASUVORK5CYII=";
        }

        #endregion
    }
}