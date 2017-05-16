namespace VisualPlus.Framework.GDI
{
    #region Namespace

    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Enums;

    #endregion

    internal class GDI
    {
        #region Events

        /// <summary>Anchors the rectangle to an anchored alignment of the base rectangle.</summary>
        /// <param name="anchorStyle">Alignment style.</param>
        /// <param name="baseRectangle">Base rectangle.</param>
        /// <param name="anchorWidth">Anchor width.</param>
        /// <returns>Anchored rectangle.</returns>
        public static Rectangle ApplyAnchor(TabAlignment anchorStyle, Rectangle baseRectangle, int anchorWidth)
        {
            Point anchoredLocation;
            Size anchoredSize;

            switch (anchorStyle)
            {
                case TabAlignment.Top:
                    {
                        anchoredLocation = new Point(baseRectangle.X, baseRectangle.Y);
                        anchoredSize = new Size(baseRectangle.Width, anchorWidth);
                        break;
                    }

                case TabAlignment.Bottom:
                    {
                        anchoredLocation = new Point(baseRectangle.X, baseRectangle.Bottom - anchorWidth);
                        anchoredSize = new Size(baseRectangle.Width, anchorWidth);
                        break;
                    }

                case TabAlignment.Left:
                    {
                        anchoredLocation = new Point(baseRectangle.X, baseRectangle.Y);
                        anchoredSize = new Size(anchorWidth, baseRectangle.Height);
                        break;
                    }

                case TabAlignment.Right:
                    {
                        anchoredLocation = new Point(baseRectangle.Right - anchorWidth, baseRectangle.Y);
                        anchoredSize = new Size(anchorWidth, baseRectangle.Height);
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(anchorStyle), anchorStyle, null);
            }

            Rectangle anchoredRectangle = new Rectangle(anchoredLocation, anchoredSize);
            return anchoredRectangle;
        }

        /// <summary>Draws the text image relation.</summary>
        /// <param name="relation">The relation type.</param>
        /// <param name="imageRectangle">The image rectangle.</param>
        /// <param name="fontSize">The text size.</param>
        /// <param name="outerBounds">The outer bounds.</param>
        /// <param name="imagePoint">The image Point.</param>
        /// <returns>The <see cref="Point" />.</returns>
        public static Point ApplyTextImageRelation(TextImageRelation relation, Rectangle imageRectangle, SizeF fontSize, Rectangle outerBounds, bool imagePoint)
        {
            Point newPosition = new Point(0, 0);
            Point newImagePoint = new Point(0, 0);
            Point newTextPoint = new Point(0, 0);

            switch (relation)
            {
                case TextImageRelation.Overlay:
                    {
                        // Set center
                        newPosition.X = outerBounds.Width / 2;
                        newPosition.Y = outerBounds.Height / 2;

                        // Set image
                        newImagePoint.X = newPosition.X - imageRectangle.Width / 2;
                        newImagePoint.Y = newPosition.Y - imageRectangle.Height / 2;

                        // Set text
                        newTextPoint.X = newPosition.X - Convert.ToInt32(fontSize.Width) / 2;
                        newTextPoint.Y = newPosition.Y - Convert.ToInt32(fontSize.Height) / 2;
                        break;
                    }

                case TextImageRelation.ImageBeforeText:
                    {
                        // Set center
                        newPosition.Y = outerBounds.Height / 2;

                        // Set image
                        newImagePoint.X = newPosition.X + 4;
                        newImagePoint.Y = newPosition.Y - imageRectangle.Height / 2;

                        // Set text
                        newTextPoint.X = newImagePoint.X + imageRectangle.Width;
                        newTextPoint.Y = newPosition.Y - (int)fontSize.Height / 2;
                        break;
                    }

                case TextImageRelation.TextBeforeImage:
                    {
                        // Set center
                        newPosition.Y = outerBounds.Height / 2;

                        // Set text
                        newTextPoint.X = newPosition.X + 4;
                        newTextPoint.Y = newPosition.Y - (int)fontSize.Height / 2;

                        // Set image
                        newImagePoint.X = newTextPoint.X + (int)fontSize.Width;
                        newImagePoint.Y = newPosition.Y - imageRectangle.Height / 2;
                        break;
                    }

                case TextImageRelation.ImageAboveText:
                    {
                        // Set center
                        newPosition.X = outerBounds.Width / 2;

                        // Set image
                        newImagePoint.X = newPosition.X - imageRectangle.Width / 2;
                        newImagePoint.Y = newPosition.Y + 4;

                        // Set text
                        newTextPoint.X = newPosition.X - Convert.ToInt32(fontSize.Width) / 2;
                        newTextPoint.Y = newImagePoint.Y + imageRectangle.Height;
                        break;
                    }

                case TextImageRelation.TextAboveImage:
                    {
                        // Set center
                        newPosition.X = outerBounds.Width / 2;

                        // Set text
                        newTextPoint.X = newPosition.X - Convert.ToInt32(fontSize.Width) / 2;
                        newTextPoint.Y = newImagePoint.Y + 4;

                        // Set image
                        newImagePoint.X = newPosition.X - imageRectangle.Width / 2;
                        newImagePoint.Y = newPosition.Y + Convert.ToInt32(fontSize.Height) + 4;
                        break;
                    }
            }

            return imagePoint ? newImagePoint : newTextPoint;
        }

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

        /// <summary>Creates a gradient brush.</summary>
        /// <param name="colors">The colors.</param>
        /// <param name="positions">The positions.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="startPoint">Start position.</param>
        /// <param name="endPoint">End position.</param>
        /// <returns>The gradient brush.</returns>
        public static LinearGradientBrush CreateGradientBrush(Color[] colors, float[] positions, float angle, Point startPoint, Point endPoint)
        {
            LinearGradientBrush linearGradientBrush = new LinearGradientBrush(startPoint, endPoint, Color.Black, Color.Black);

            ColorBlend colorBlend = new ColorBlend
                {
                    Positions = positions,
                    Colors = colors
                };

            // Define brush color blend
            linearGradientBrush.InterpolationColors = colorBlend;
            linearGradientBrush.RotateTransform(angle);

            return linearGradientBrush;
        }

        /// <summary>Draws a border around the path.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="borderPath">The border path.</param>
        /// <param name="borderThickness">The border size.</param>
        /// <param name="color">The color.</param>
        public static void DrawBorder(Graphics graphics, GraphicsPath borderPath, float borderThickness, Color color)
        {
            Pen borderPen = new Pen(color, borderThickness);
            graphics.DrawPath(borderPen, borderPath);
        }

        /// <summary>Handles and draws a border on the control depending on it's current focus.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="controlState">The control state.</param>
        /// <param name="controlPath">The border path.</param>
        /// <param name="borderThickness">The border size.</param>
        /// <param name="borderColor">Normal border color.</param>
        /// <param name="borderHoverColor">Hover border color.</param>
        /// <param name="borderHoverVisible">Hover visible.</param>
        public static void DrawBorderType(Graphics graphics, ControlState controlState, GraphicsPath controlPath, float borderThickness, Color borderColor, Color borderHoverColor, bool borderHoverVisible)
        {
            if (controlState == ControlState.Hover && borderHoverVisible)
            {
                DrawBorder(graphics, controlPath, borderThickness, borderHoverColor);
            }
            else
            {
                DrawBorder(graphics, controlPath, borderThickness, borderColor);
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

        /// <summary>Draws the tick line.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="drawRect">The rectangle</param>
        /// <param name="tickFrequency">Tick frequency.</param>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        /// <param name="tickColor">The tick Color.</param>
        /// <param name="orientation">The orientation.</param>
        public static void DrawTickLine(Graphics graphics, RectangleF drawRect, int tickFrequency, int minimum, int maximum, Color tickColor, Orientation orientation)
        {
            // Check input value
            if (maximum == minimum)
            {
                return;
            }

            // Create the Pen for drawing Ticks
            Pen pen = new Pen(tickColor, 1);
            float tickFrequencySize;

            // Calculate tick number
            int tickCount = (maximum - minimum) / tickFrequency;
            if ((maximum - minimum) % tickFrequency == 0)
            {
                tickCount -= 1;
            }

            if (orientation == Orientation.Horizontal)
            {
                // Calculate tick's setting
                tickFrequencySize = drawRect.Width * tickFrequency / (maximum - minimum);

                // Draw each tick
                for (var i = 0; i <= tickCount; i++)
                {
                    graphics.DrawLine(pen, drawRect.Left + tickFrequencySize * i, drawRect.Top, drawRect.Left + tickFrequencySize * i, drawRect.Bottom);
                }

                // Draw last tick at Maximum
                graphics.DrawLine(pen, drawRect.Right, drawRect.Top, drawRect.Right, drawRect.Bottom);
            }
            else
            {
                // Calculate tick's setting
                tickFrequencySize = drawRect.Height * tickFrequency / (maximum - minimum);

                // Draw each tick
                for (var i = 0; i <= tickCount; i++)
                {
                    graphics.DrawLine(pen, drawRect.Left, drawRect.Bottom - tickFrequencySize * i, drawRect.Right, drawRect.Bottom - tickFrequencySize * i);
                }

                // Draw last tick at Maximum
                graphics.DrawLine(pen, drawRect.Left, drawRect.Top, drawRect.Right, drawRect.Top);
            }
        }

        /// <summary>Draws the tick text.</summary>
        /// <param name="graphics">Graphics controller.</param>
        /// <param name="drawRect">The rectangle</param>
        /// <param name="tickFrequency">Tick frequency.</param>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        /// <param name="foreColor">Fore color.</param>
        /// <param name="font">The font.</param>
        /// <param name="orientation">The orientation.</param>
        public static void DrawTickTextLine(Graphics graphics, RectangleF drawRect, int tickFrequency, int minimum, int maximum, Color foreColor, Font font, Orientation orientation)
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

        /// <summary>Flip the size by orientation.</summary>
        /// <param name="orientation">The orientation.</param>
        /// <param name="size">Current size.</param>
        /// <returns>New size.</returns>
        public static Size FlipOrientationSize(Orientation orientation, Size size)
        {
            Size newSize = new Size(0, 0);

            // Resize
            if (orientation == Orientation.Vertical)
            {
                if (size.Width > size.Height)
                {
                    newSize = new Size(size.Height, size.Width);
                }
            }
            else
            {
                if (size.Width < size.Height)
                {
                    newSize = new Size(size.Height, size.Width);
                }
            }

            return newSize;
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

        /// <summary>Measures the specified string when draw with the specified font.</summary>
        /// <param name="graphics">Graphics input.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The text Font.</param>
        /// <returns>Returns text size.</returns>
        public static Size GetTextSize(Graphics graphics, string text, Font font)
        {
            int width = Convert.ToInt32(graphics.MeasureString(text, font).Width);
            int height = Convert.ToInt32(graphics.MeasureString(text, font).Height);
            Size textSize = new Size(width, height);

            return textSize;
        }

        /// <summary>Initialize the graphics processor.</summary>
        /// <param name="e">Paint event.</param>
        /// <param name="compositingMode">Compositing mode.</param>
        /// <param name="compositingQuality">Compositing quality.</param>
        /// <param name="interpolationMode">Interpolation mode.</param>
        /// <param name="pixelOffsetMode">Pixel offset mode.</param>
        /// <param name="smoothingMode">Smoothing mode.</param>
        /// <param name="textRenderingHint">Text rendering hint.</param>
        /// <returns>Processed graphics.</returns>
        public static Graphics Initialize(PaintEventArgs e, CompositingMode compositingMode, CompositingQuality compositingQuality, InterpolationMode interpolationMode, PixelOffsetMode pixelOffsetMode, SmoothingMode smoothingMode, TextRenderingHint textRenderingHint)
        {
            Graphics graphics = e.Graphics;
            graphics.CompositingMode = compositingMode;
            graphics.CompositingQuality = compositingQuality;
            graphics.InterpolationMode = interpolationMode;
            graphics.PixelOffsetMode = pixelOffsetMode;
            graphics.SmoothingMode = smoothingMode;
            graphics.TextRenderingHint = textRenderingHint;
            return graphics;
        }

        #endregion
    }
}