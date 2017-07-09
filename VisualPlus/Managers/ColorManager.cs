namespace VisualPlus.Managers
{
    #region Namespace

    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    #endregion

    internal class ColorManager
    {
        #region Constructors

        public enum Brightness
        {
            /// <summary>Darker.</summary>
            Dark,

            /// <summary>Lighter.</summary>
            Light
        }

        #endregion

        #region Events

        public static double BlendColor(double foreColor, double backgroundColor, double alpha)
        {
            double result = backgroundColor + alpha * (foreColor - backgroundColor);
            if (result < 0.0)
            {
                result = 0.0;
            }

            if (result > 255)
            {
                result = 255;
            }

            return result;
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

        public static Color ColorFromHtml(string withoutHash)
        {
            return ColorTranslator.FromHtml("#" + withoutHash);
        }

        public static string ColorToHtml(Color color)
        {
            return ColorTranslator.ToHtml(color);
        }

        /// <summary>
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <returns></returns>
        public static Color CreateColorFromRGB(int red, int green, int blue)
        {
            // Corect Red element
            int r = red;
            if (r > 255)
            {
                r = 255;
            }

            if (r < 0)
            {
                r = 0;
            }

            // Corect Green element
            int g = green;
            if (g > 255)
            {
                g = 255;
            }

            if (g < 0)
            {
                g = 0;
            }

            // Correct Blue Element
            int b = blue;
            if (b > 255)
            {
                b = 255;
            }

            if (b < 0)
            {
                b = 0;
            }

            return Color.FromArgb(r, g, b);
        }

        /// <summary>Gets the color under the mouse.</summary>
        /// <returns>The color.</returns>
        public static Color CurrentPointerColor()
        {
            Point cursor = new Point();
            Native.GetCursorPos(ref cursor);
            return GetColorFromPosition(cursor);
        }

        /// <summary>Get the color from position.</summary>
        /// <param name="location">Cursor position.</param>
        /// <returns>The color.</returns>
        public static Color GetColorFromPosition(Point location)
        {
            using (Graphics graphicsDestination = Graphics.FromImage(screenPixel))
            {
                using (Graphics graphicsSource = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr handleContextSource = graphicsSource.GetHdc();
                    IntPtr handleContextDestination = graphicsDestination.GetHdc();
                    int retrieval = Native.BitBlt(handleContextDestination, 0, 0, 1, 1, handleContextSource, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                    graphicsDestination.ReleaseHdc();
                    graphicsSource.ReleaseHdc();
                }
            }

            return screenPixel.GetPixel(0, 0);
        }

        /// <summary>Returns a brightness difference in color.</summary>
        /// <param name="brightness">The color tint.</param>
        /// <param name="c">The color.</param>
        /// <param name="d">The byte.</param>
        /// <returns>The new color.</returns>
        public static Color GetColorTint(Brightness brightness, Color c, byte d)
        {
            Color newColor = new Color();
            byte r;
            byte g;
            byte b;

            switch (brightness)
            {
                case Brightness.Dark:
                    {
                        r = 0;
                        g = 0;
                        b = 0;

                        if (c.R > d)
                        {
                            r = (byte)(c.R - d);
                        }

                        if (c.G > d)
                        {
                            g = (byte)(c.G - d);
                        }

                        if (c.B > d)
                        {
                            b = (byte)(c.B - d);
                        }

                        newColor = Color.FromArgb(r, g, b);
                        break;
                    }

                case Brightness.Light:
                    {
                        r = 255;
                        g = 255;
                        b = 255;

                        if (c.R + d < 255)
                        {
                            r = (byte)(c.R + d);
                        }

                        if (c.G + d < 255)
                        {
                            g = (byte)(c.G + d);
                        }

                        if (c.B + d < 255)
                        {
                            b = (byte)(c.B + d);
                        }

                        newColor = Color.FromArgb(r, g, b);
                        break;
                    }
            }

            return newColor;
        }

        /// <summary>
        /// </summary>
        /// <param name="blendColor"></param>
        /// <param name="baseColor"></param>
        /// <param name="opacity"></param>
        /// <returns></returns>
        public static Color OpacityMix(Color blendColor, Color baseColor, int opacity)
        {
            int r1;
            int g1;
            int b1;
            int r2;
            int g2;
            int b2;
            int r3;
            int g3;
            int b3;
            r1 = blendColor.R;
            g1 = blendColor.G;
            b1 = blendColor.B;
            r2 = baseColor.R;
            g2 = baseColor.G;
            b2 = baseColor.B;
            r3 = (int)(r1 * ((float)opacity / 100) + r2 * (1 - (float)opacity / 100));
            g3 = (int)(g1 * ((float)opacity / 100) + g2 * (1 - (float)opacity / 100));
            b3 = (int)(b1 * ((float)opacity / 100) + b2 * (1 - (float)opacity / 100));
            return CreateColorFromRGB(r3, g3, b3);
        }

        /// <summary>
        /// </summary>
        /// <param name="ibase"></param>
        /// <param name="blend"></param>
        /// <returns></returns>
        public static int OverlayMath(int ibase, int blend)
        {
            double dbase;
            double dblend;
            dbase = (double)ibase / 255;
            dblend = (double)blend / 255;
            if (dbase < 0.5)
            {
                return (int)(2 * dbase * dblend * 255);
            }
            else
            {
                return (int)((1 - 2 * (1 - dbase) * (1 - dblend)) * 255);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="baseColor"></param>
        /// <param name="blendColor"></param>
        /// <param name="opacity"></param>
        /// <returns></returns>
        public static Color OverlayMix(Color baseColor, Color blendColor, int opacity)
        {
            int r1;
            int g1;
            int b1;
            int r2;
            int g2;
            int b2;
            int r3;
            int g3;
            int b3;
            r1 = baseColor.R;
            g1 = baseColor.G;
            b1 = baseColor.B;
            r2 = blendColor.R;
            g2 = blendColor.G;
            b2 = blendColor.B;
            r3 = OverlayMath(baseColor.R, blendColor.R);
            g3 = OverlayMath(baseColor.G, blendColor.G);
            b3 = OverlayMath(baseColor.B, blendColor.B);
            return OpacityMix(CreateColorFromRGB(r3, g3, b3), baseColor, opacity);
        }

        /// <summary>
        /// </summary>
        /// <param name="baseColor"></param>
        /// <param name="blendColor"></param>
        /// <param name="opacity"></param>
        /// <returns></returns>
        public static Color SoftLightMix(Color baseColor, Color blendColor, int opacity)
        {
            int r1;
            int g1;
            int b1;
            int r2;
            int g2;
            int b2;
            int r3;
            int g3;
            int b3;
            r1 = baseColor.R;
            g1 = baseColor.G;
            b1 = baseColor.B;
            r2 = blendColor.R;
            g2 = blendColor.G;
            b2 = blendColor.B;
            r3 = SoftLightMath(r1, r2);
            g3 = SoftLightMath(g1, g2);
            b3 = SoftLightMath(b1, b2);
            return OpacityMix(CreateColorFromRGB(r3, g3, b3), baseColor, opacity);
        }

        public static Color StepColor(Color color, int inputAlpha)
        {
            if (inputAlpha == 100)
            {
                return color;
            }

            byte a = color.A;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            float background;

            int alpha = Math.Min(inputAlpha, 200);
            alpha = Math.Max(alpha, 0);
            double doubleAlpha = (alpha - 100.0) / 100.0;

            if (doubleAlpha > 100)
            {
                // Blend with white
                background = 255.0F;

                // 0 = transparent fg; 1 = opaque fg
                doubleAlpha = 1.0F - doubleAlpha;
            }
            else
            {
                // Blend with black
                background = 0.0F;

                // 0 = transparent fg; 1 = opaque fg
                doubleAlpha = 1.0F + doubleAlpha;
            }

            r = (byte)BlendColor(r, background, doubleAlpha);
            g = (byte)BlendColor(g, background, doubleAlpha);
            b = (byte)BlendColor(b, background, doubleAlpha);

            return Color.FromArgb(a, r, g, b);
        }

        private static Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);

        /// <summary>
        /// </summary>
        /// <param name="ibase"></param>
        /// <param name="blend"></param>
        /// <returns></returns>
        private static int SoftLightMath(int ibase, int blend)
        {
            float dbase;
            float dblend;
            dbase = (float)ibase / 255;
            dblend = (float)blend / 255;
            if (dblend < 0.5)
            {
                return (int)((2 * dbase * dblend + Math.Pow(dbase, 2) * (1 - 2 * dblend)) * 255);
            }
            else
            {
                return (int)((Math.Sqrt(dbase) * (2 * dblend - 1) + 2 * dbase * (1 - dblend)) * 255);
            }
        }

        #endregion
    }
}