namespace VisualPlus.Framework.GDI
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Enums;

    internal class GDI
    {
        #region  ${0} Variables

        public static GraphicsPath GraphicPath = null;

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

        /// <summary>
        /// </summary>
        /// <param name="g"></param>
        /// <param name="drawRectF"></param>
        /// <param name="drawColor"></param>
        /// <param name="orientation"></param>
        public static void DrawAquaPill(Graphics g, RectangleF drawRectF, Color drawColor, Orientation orientation)
        {
            Color color1;
            Color color2;
            Color color3;
            Color color4;
            Color color5;
            LinearGradientBrush gradientBrush;
            ColorBlend colorBlend = new ColorBlend();

            color1 = ColorHelper.OpacityMix(Color.White, ColorHelper.SoftLightMix(drawColor, Color.Black, 100), 40);
            color2 = ColorHelper.OpacityMix(Color.White, ColorHelper.SoftLightMix(drawColor, ColorHelper.CreateColorFromRGB(64, 64, 64), 100), 20);
            color3 = ColorHelper.SoftLightMix(drawColor, ColorHelper.CreateColorFromRGB(128, 128, 128), 100);
            color4 = ColorHelper.SoftLightMix(drawColor, ColorHelper.CreateColorFromRGB(192, 192, 192), 100);
            color5 = ColorHelper.OverlayMix(ColorHelper.SoftLightMix(drawColor, Color.White, 100), Color.White, 75);

            colorBlend.Colors = new[] { color1, color2, color3, color4, color5 };
            colorBlend.Positions = new[] { 0, 0.25f, 0.5f, 0.75f, 1 };
            if (orientation == Orientation.Horizontal)
            {
                gradientBrush = new LinearGradientBrush(new Point((int)drawRectF.Left, (int)drawRectF.Top - 1),
                    new Point((int)drawRectF.Left, (int)drawRectF.Top + (int)drawRectF.Height + 1), color1, color5);
            }
            else
            {
                gradientBrush = new LinearGradientBrush(new Point((int)drawRectF.Left - 1, (int)drawRectF.Top),
                    new Point((int)drawRectF.Left + (int)drawRectF.Width + 1, (int)drawRectF.Top), color1, color5);
            }

            gradientBrush.InterpolationColors = colorBlend;
            FillPill(gradientBrush, drawRectF, g);

            color2 = Color.White;
            colorBlend.Colors = new[] { color2, color3, color4, color5 };
            colorBlend.Positions = new[] { 0, 0.5f, 0.75f, 1 };
            if (orientation == Orientation.Horizontal)
            {
                gradientBrush = new LinearGradientBrush(new Point((int)drawRectF.Left + 1, (int)drawRectF.Top),
                    new Point((int)drawRectF.Left + 1, (int)drawRectF.Top + (int)drawRectF.Height - 1), color2, color5);
            }
            else
            {
                gradientBrush = new LinearGradientBrush(new Point((int)drawRectF.Left, (int)drawRectF.Top + 1),
                    new Point((int)drawRectF.Left + (int)drawRectF.Width - 1, (int)drawRectF.Top + 1), color2, color5);
            }

            gradientBrush.InterpolationColors = colorBlend;
            FillPill(gradientBrush, RectangleF.Inflate(drawRectF, -3, -3), g);
        }

        /// <summary>
        /// </summary>
        /// <param name="g"></param>
        /// <param name="drawRectF"></param>
        /// <param name="drawColor"></param>
        /// <param name="orientation"></param>
        public static void DrawAquaPillSingleLayer(Graphics g, RectangleF drawRectF, Color drawColor, Orientation orientation)
        {
            Color color1;
            Color color2;
            Color color3;
            Color color4;
            LinearGradientBrush gradientBrush;
            ColorBlend colorBlend = new ColorBlend();

            color1 = drawColor;
            color2 = ControlPaint.Light(color1);
            color3 = ControlPaint.Light(color2);
            color4 = ControlPaint.Light(color3);

            colorBlend.Colors = new[] { color1, color2, color3, color4 };
            colorBlend.Positions = new[] { 0, 0.25f, 0.65f, 1 };

            if (orientation == Orientation.Horizontal)
            {
                gradientBrush = new LinearGradientBrush(new Point((int)drawRectF.Left, (int)drawRectF.Top),
                    new Point((int)drawRectF.Left, (int)drawRectF.Top + (int)drawRectF.Height), color1, color4);
            }
            else
            {
                gradientBrush = new LinearGradientBrush(new Point((int)drawRectF.Left, (int)drawRectF.Top),
                    new Point((int)drawRectF.Left + (int)drawRectF.Width, (int)drawRectF.Top), color1, color4);
            }

            gradientBrush.InterpolationColors = colorBlend;

            FillPill(gradientBrush, drawRectF, g);
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

        /// <summary>Handles and draws a border on the control depending on it's current focus.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="controlState">The control state.</param>
        /// <param name="controlPath">The border path.</param>
        /// <param name="borderSize">The border size.</param>
        /// <param name="borderColor">Normal border color.</param>
        /// <param name="borderHoverColor">Hover border color.</param>
        /// <param name="hoverVisible">Hover visible.</param>
        public static void DrawBorderType(Graphics graphics, ControlState controlState, GraphicsPath controlPath, float borderSize, Color borderColor,
                                          Color borderHoverColor, bool borderHoverVisible)
        {
            if (controlState == ControlState.Hover && borderHoverVisible)
            {
                DrawBorder(graphics, controlPath, borderSize, borderHoverColor);
            }
            else
            {
                DrawBorder(graphics, controlPath, borderSize, borderColor);
            }
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

        /// <summary>
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="drawRect"></param>
        /// <param name="tickFrequency"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="tickColor"></param>
        /// <param name="orientation"></param>
        public static void DrawTickLine(Graphics graphics, RectangleF drawRect, int tickFrequency, int minimum, int maximum, Color tickColor,
                                        Orientation orientation)
        {
            // Check input value
            if (maximum == minimum)
            {
                return;
            }

            // Create the Pen for drawing Ticks
            Pen pen = new Pen(tickColor, 1);
            float tickFrequencySize;

            // Caculate tick number
            int tickCount = (maximum - minimum) / tickFrequency;
            if ((maximum - minimum) % tickFrequency == 0)
            {
                tickCount -= 1;
            }

            if (orientation == Orientation.Horizontal)
            {
                // Calculate tick's setting
                tickFrequencySize = drawRect.Width * tickFrequency / (maximum - minimum);

                // ===============================================================

                // Draw each tick
                for (var i = 0; i <= tickCount; i++)
                {
                    graphics.DrawLine(pen, drawRect.Left + tickFrequencySize * i, drawRect.Top, drawRect.Left + tickFrequencySize * i, drawRect.Bottom);
                }

                // Draw last tick at Maximum
                graphics.DrawLine(pen, drawRect.Right, drawRect.Top, drawRect.Right, drawRect.Bottom);

                // ===============================================================
            }
            else
            {
                // Orientation.Vertical
                // Calculate tick's setting
                tickFrequencySize = drawRect.Height * tickFrequency / (maximum - minimum);

                // ===============================================================

                // Draw each tick
                for (var i = 0; i <= tickCount; i++)
                {
                    graphics.DrawLine(pen, drawRect.Left, drawRect.Bottom - tickFrequencySize * i, drawRect.Right,
                        drawRect.Bottom - tickFrequencySize * i);
                }

                // Draw last tick at Maximum
                graphics.DrawLine(pen, drawRect.Left, drawRect.Top, drawRect.Right, drawRect.Top);

                // ===============================================================
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="drawRect"></param>
        /// <param name="tickFrequency"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="foreColor"></param>
        /// <param name="font"></param>
        /// <param name="orientation"></param>
        public static void DrawTickTextLine(Graphics graphics, RectangleF drawRect, int tickFrequency, int minimum, int maximum, Color foreColor,
                                            Font font, Orientation orientation)
        {
            // Check input value
            if (maximum == minimum)
            {
                return;
            }

            // Calculate tick number
            int tickCount = (maximum - minimum) / tickFrequency;
            if ((maximum - minimum) % tickFrequency == 0)
            {
                tickCount -= 1;
            }

            // Prepare for drawing Text
            StringFormat stringFormat = new StringFormat
                {
                    FormatFlags = StringFormatFlags.NoWrap,
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    HotkeyPrefix = HotkeyPrefix.Show
                };

            Brush brush = new SolidBrush(foreColor);
            string text;
            float tickFrequencySize;

            if (orientation == Orientation.Horizontal)
            {
                // Calculate tick's setting
                tickFrequencySize = drawRect.Width * tickFrequency / (maximum - minimum);

                // Draw each tick text
                for (var i = 0; i <= tickCount; i++)
                {
                    text = Convert.ToString(minimum + tickFrequency * i, 10);
                    graphics.DrawString(text, font, brush, drawRect.Left + tickFrequencySize * i, drawRect.Top + drawRect.Height / 2, stringFormat);
                }

                // Draw last tick text at Maximum
                text = Convert.ToString(maximum, 10);
                graphics.DrawString(text, font, brush, drawRect.Right, drawRect.Top + drawRect.Height / 2, stringFormat);
            }
            else
            {
                // Orientation.Vertical
                // Calculate tick's setting
                tickFrequencySize = drawRect.Height * tickFrequency / (maximum - minimum);

                // Draw each tick text
                for (var i = 0; i <= tickCount; i++)
                {
                    text = Convert.ToString(minimum + tickFrequency * i, 10);
                    graphics.DrawString(text, font, brush, drawRect.Left + drawRect.Width / 2, drawRect.Bottom - tickFrequencySize * i, stringFormat);
                }

                // Draw last tick text at Maximum
                text = Convert.ToString(maximum, 10);
                graphics.DrawString(text, font, brush, drawRect.Left + drawRect.Width / 2, drawRect.Top, stringFormat);
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

        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        /// <param name="rect"></param>
        /// <param name="g"></param>
        public static void FillPill(Brush b, RectangleF rect, Graphics g)
        {
            if (rect.Width > rect.Height)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.FillEllipse(b, new RectangleF(rect.Left, rect.Top, rect.Height, rect.Height));
                g.FillEllipse(b, new RectangleF(rect.Left + rect.Width - rect.Height, rect.Top, rect.Height, rect.Height));

                float w = rect.Width - rect.Height;
                float l = rect.Left + rect.Height / 2;
                g.FillRectangle(b, new RectangleF(l, rect.Top, w, rect.Height));
                g.SmoothingMode = SmoothingMode.Default;
            }
            else if (rect.Width < rect.Height)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.FillEllipse(b, new RectangleF(rect.Left, rect.Top, rect.Width, rect.Width));
                g.FillEllipse(b, new RectangleF(rect.Left, rect.Top + rect.Height - rect.Width, rect.Width, rect.Width));

                float t = rect.Top + rect.Width / 2;
                float h = rect.Height - rect.Width;
                g.FillRectangle(b, new RectangleF(rect.Left, t, rect.Width, h));
                g.SmoothingMode = SmoothingMode.Default;
            }
            else if (rect.Width == rect.Height)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.FillEllipse(b, rect);
                g.SmoothingMode = SmoothingMode.Default;
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