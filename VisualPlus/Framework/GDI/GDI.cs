namespace VisualPlus.Framework.GDI
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    using VisualPlus.Enums;
    using VisualPlus.Framework.Styles;

    internal class GDI
    {
        #region  ${0} Variables

        public static GraphicsPath GraphicPath = null;
        private static readonly IStyle Style = new Visual();

        #endregion

        #region ${0} Methods

        public static Color BlendColor(Color backgroundColor, Color frontColor, double blend)
        {
            double ratio = blend / 255d;
            double invRatio = 1d - ratio;
            var r = (int)(backgroundColor.R * invRatio + frontColor.R * ratio);
            var g = (int)(backgroundColor.G * invRatio + frontColor.G * ratio);
            var b = (int)(backgroundColor.B * invRatio + frontColor.B * ratio);
            return Color.FromArgb(r, g, b);
        }

        public static Color BlendColor(Color backgroundColor, Color frontColor)
        {
            return BlendColor(backgroundColor, frontColor, frontColor.A);
        }

        /// <summary>Calculates a 5 point star.</summary>
        /// <param name="originF"> The originF is the middle of the star.</param>
        /// <param name="outerRadius">Radius of the surrounding circle.</param>
        /// <param name="innerRadius">Radius of the circle for the "inner" points</param>
        /// <returns>10 PointF array.</returns>
        public static PointF[] Calculate5PointStar(PointF originF, float outerRadius, float innerRadius)
        {
            // Define some variables to avoid as much calculations as possible
            // conversions to radians
            const double Ang36 = Math.PI / 5.0; // 36Â° x PI/180
            const double Ang72 = 2.0 * Ang36; // 72Â° x PI/180

            // some sine and cosine values we need
            var sin36 = (float)Math.Sin(Ang36);
            var sin72 = (float)Math.Sin(Ang72);
            var cos36 = (float)Math.Cos(Ang36);
            var cos72 = (float)Math.Cos(Ang72);

            // Fill array with 10 originF points
            PointF[] pointsArray = { originF, originF, originF, originF, originF, originF, originF, originF, originF, originF };
            pointsArray[0].Y -= outerRadius; // top off the star, or on a clock this is 12:00 or 0:00 hours
            pointsArray[1].X += innerRadius * sin36;
            pointsArray[1].Y -= innerRadius * cos36; // 0:06 hours
            pointsArray[2].X += outerRadius * sin72;
            pointsArray[2].Y -= outerRadius * cos72; // 0:12 hours
            pointsArray[3].X += innerRadius * sin72;
            pointsArray[3].Y += innerRadius * cos72; // 0:18
            pointsArray[4].X += outerRadius * sin36;
            pointsArray[4].Y += outerRadius * cos36; // 0:24 

            // Phew! Glad I got that trig working.
            pointsArray[5].Y += innerRadius;

            // I use the symmetry of the star figure here
            pointsArray[6].X += pointsArray[6].X - pointsArray[4].X;
            pointsArray[6].Y = pointsArray[4].Y; // mirror point
            pointsArray[7].X += pointsArray[7].X - pointsArray[3].X;
            pointsArray[7].Y = pointsArray[3].Y; // mirror point
            pointsArray[8].X += pointsArray[8].X - pointsArray[2].X;
            pointsArray[8].Y = pointsArray[2].Y; // mirror point
            pointsArray[9].X += pointsArray[9].X - pointsArray[1].X;
            pointsArray[9].Y = pointsArray[1].Y; // mirror point

            return pointsArray;
        }

        public static GraphicsPath CreateRoundRect(float x, float y, float width, float height, float radius)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(x + radius, y, x + width - radius * 2, y);
            gp.AddArc(x + width - radius * 2, y, radius * 2, radius * 2, 270, 90);
            gp.AddLine(x + width, y + radius, x + width, y + height - radius * 2);
            gp.AddArc(x + width - radius * 2, y + height - radius * 2, radius * 2, radius * 2, 0, 90);
            gp.AddLine(x + width - radius * 2, y + height, x + radius, y + height);
            gp.AddArc(x, y + height - radius * 2, radius * 2, radius * 2, 90, 90);
            gp.AddLine(x, y + height - radius * 2, x, y + radius);
            gp.AddArc(x, y, radius * 2, radius * 2, 180, 90);
            gp.CloseFigure();
            return gp;
        }

        /// <summary>Draws a border around the path.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="borderPath">The border path.</param>
        /// <param name="borderSize">The border size.</param>
        /// <param name="color">The color.</param>
        public static void DrawBorder(Graphics graphics, GraphicsPath borderPath, float borderSize, Color color)
        {
            Pen borderPen = new Pen(color, borderSize);
            graphics.DrawPath(borderPen, borderPath);
        }

        /// <summary>Draws the rounded rectangle from specific values.</summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="curve">The curve.</param>
        /// <returns>The <see cref="GraphicsPath" />.</returns>
        public static GraphicsPath DrawRoundedRectangle(int x, int y, int width, int height, int curve)
        {
            Rectangle rectangleShape = new Rectangle(x, y, width, height);
            GraphicsPath graphicsPath = DrawRoundedRectangle(rectangleShape, curve);
            return graphicsPath;
        }

        /// <summary> Draws the rounded rectangle from a rectangle shape.</summary>
        /// <param name="rectangleShape">The rectangle shape.</param>
        /// <param name="curve">The curve.</param>
        /// <returns>The <see cref="GraphicsPath" />.</returns>
        public static GraphicsPath DrawRoundedRectangle(Rectangle rectangleShape, int curve)
        {
            GraphicsPath graphicPath = new GraphicsPath(FillMode.Winding);
            graphicPath.AddArc(rectangleShape.X, rectangleShape.Y, curve, curve, 180.0F, 90.0F);
            graphicPath.AddArc(rectangleShape.Right - curve, rectangleShape.Y, curve, curve, 270.0F, 90.0F);
            graphicPath.AddArc(rectangleShape.Right - curve, rectangleShape.Bottom - curve, curve, curve, 0.0F, 90.0F);
            graphicPath.AddArc(rectangleShape.X, rectangleShape.Bottom - curve, curve, curve, 90.0F, 90.0F);
            graphicPath.CloseFigure();
            return graphicPath;
        }

        /// <summary>Draws the hatch brush as an image and then converts it to a texture brush for scaling.</summary>
        /// <param name="hatchBrush">Hatch brush pattern.</param>
        /// <returns>Texture brush.</returns>
        public static TextureBrush DrawTextureUsingHatch(HatchBrush hatchBrush)
        {
            using (Bitmap bitmap = new Bitmap(8, 8))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.FillRectangle(hatchBrush, 0, 0, 8, 8);
                return new TextureBrush(bitmap);
            }
        }

        /// <summary>Fills the background with color.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="graphicsShape">The shape.</param>
        /// <param name="color1">Color 1.</param>
        /// <param name="color2">Color 2.</param>
        /// <param name="rotation">Gradient rotation.</param>
        /// <param name="gradient">Toggle gradient coloring.</param>
        public static void FillBackground(
            Graphics graphics,
            Rectangle rectangle,
            GraphicsPath graphicsShape,
            Color color1,
            Color color2,
            int rotation,
            bool gradient)
        {
            if (gradient)
            {
                // Fill gradient shape
                graphics.FillPath(new LinearGradientBrush(rectangle, color1, color2, rotation), graphicsShape);
            }
            else
            {
                graphics.FillPath(new SolidBrush(color1), graphicsShape);
            }
        }

        /// <summary>Draws the border shape.</summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="borderShape">The border Shape.</param>
        /// <param name="borderRounding">The border Rounding.</param>
        /// <returns>The <see cref="GraphicsPath" />.</returns>
        public static GraphicsPath GetBorderShape(Rectangle rectangle, BorderShape borderShape, int borderRounding)
        {
            Rectangle newRectangle = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);
            GraphicsPath newShape = new GraphicsPath();

            switch (borderShape)
            {
                case BorderShape.Rectangle:
                    {
                        newShape = new GraphicsPath();
                        newShape.AddRectangle(newRectangle);
                        newShape.CloseAllFigures();
                        break;
                    }

                case BorderShape.Rounded:
                    {
                        newShape = new GraphicsPath();
                        newShape.AddArc(newRectangle.X, newRectangle.Y, borderRounding, borderRounding, 180.0F, 90.0F);
                        newShape.AddArc(newRectangle.Right - borderRounding, newRectangle.Y, borderRounding, borderRounding, 270.0F, 90.0F);
                        newShape.AddArc(
                            newRectangle.Right - borderRounding,
                            newRectangle.Bottom - borderRounding,
                            borderRounding,
                            borderRounding,
                            0.0F,
                            90.0F);
                        newShape.AddArc(newRectangle.X, newRectangle.Bottom - borderRounding, borderRounding, borderRounding, 90.0F, 90.0F);
                        newShape.CloseAllFigures();
                        break;
                    }
            }

            return newShape;
        }

        #endregion
    }
}