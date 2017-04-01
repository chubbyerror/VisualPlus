namespace VisualPlus
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Framework;
    using VisualPlus.Framework.Styles;

    /// <summary>The visual ProgressSpinner.</summary>
    [ToolboxBitmap(typeof(ProgressBar)), Designer(VSDesignerBinding.VisualProgressIndicator)]
    public class VisualProgressSpinner : Control
    {
        #region  ${0} Variables

        private static IStyle style = new Visual();

        private readonly Timer timer;
        private float angle = 270;

        private int maximum = 100;

        private int minimum;
        private int progress;

        private bool progressVisible = true;

        private bool reverse;

        private float speed = 1f;

        #endregion

        #region ${0} Properties

        public VisualProgressSpinner()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            MinimumSize = new Size(16, 16);
            Width = 16;
            Height = 16;
            Value = -1;
            timer = new Timer { Interval = 20, Enabled = true };
        }

        [DefaultValue(0), Description("The maximum progress value.")]
        public int Maximum
        {
            get
            {
                return maximum;
            }

            set
            {
                if (value <= minimum)
                {
                    throw new ArgumentOutOfRangeException("Maximum value must be > Minimum.");
                }

                maximum = value;
                if (progress > maximum)
                {
                    progress = maximum;
                }

                Refresh();
            }
        }

        [DefaultValue(0), Description("The minimum progress value.")]
        public int Minimum
        {
            get
            {
                return minimum;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Minimum value must be >= 0.");
                }

                if (value >= maximum)
                {
                    throw new ArgumentOutOfRangeException("Minimum value must be < Maximum.");
                }

                minimum = value;
                if (progress != -1 && progress < minimum)
                {
                    progress = minimum;
                }

                Refresh();
            }
        }

        [DefaultValue(true), Description("Specifies whether the progress spinner should be visible at all progress values.")]
        public bool ProgressVisible
        {
            get
            {
                return progressVisible;
            }

            set
            {
                progressVisible = value;
                Refresh();
            }
        }

        [DefaultValue(false), Description("Specifies whether the progress spinner should spin anti-clockwise.")]
        public bool Reverse
        {
            get
            {
                return reverse;
            }

            set
            {
                reverse = value;
                Refresh();
            }
        }

        [DefaultValue(1f), Description("The speed factor. 1 is the original speed, less is slower, greater is faster.")]
        public float Speed
        {
            get
            {
                return speed;
            }

            set
            {
                if (value <= 0 || value > 10)
                {
                    throw new ArgumentOutOfRangeException("Speed value must be > 0 and <= 10.");
                }

                speed = value;
            }
        }

        [DefaultValue(0), Description("The current progress value. Set -1 to indicate that the current progress is unknown.")]
        public int Value
        {
            get
            {
                return progress;
            }

            set
            {
                if (value != -1 && (value < minimum || value > maximum))
                {
                    throw new ArgumentOutOfRangeException("Progress value must be -1 or between Minimum and Maximum.");
                }

                progress = value;
                Refresh();
            }
        }

        #endregion

        #region ${0} Events

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            timer.Enabled = Enabled;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!DesignMode)
            {
                timer.Tick += (s, a) =>
                    {
                        angle += speed * (reverse ? -6f : 6f);
                        Invalidate();
                    };
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (Pen forePen = new Pen(style.ProgressColor, (float)Width / 5))
            {
                var padding = (int)Math.Ceiling((float)Width / 10);

                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

                // Draw spinner pie
                if (progress != -1)
                {
                    /* We have a progress value, draw a solid arc line
                     * angle is the back end of the line.
                     * angle +/- progress is the front end of the line
                     */
                    float sweepAngle;
                    float progFrac = (progress - minimum) / (float)(maximum - minimum);

                    if (progressVisible)
                    {
                        sweepAngle = 30 + 300f * progFrac;
                    }
                    else
                    {
                        sweepAngle = 360f * progFrac;
                    }

                    if (reverse)
                    {
                        sweepAngle = -sweepAngle;
                    }

                    e.Graphics.DrawArc(forePen, padding, padding, Width - 2 * padding - 1, Height - 2 * padding - 1, angle, sweepAngle);
                }
                else
                {
                    /* No progress value, draw a gradient arc line
                     * angle is the opaque front end of the line
                     * angle +/- 180° is the transparent tail end of the line
                     */
                    const int maxOffset = 180;
                    for (var offset = 0; offset <= maxOffset; offset += 15)
                    {
                        int alpha = 290 - offset * 290 / maxOffset;

                        if (alpha > 255)
                        {
                            alpha = 255;
                        }
                        else if (alpha < 0)
                        {
                            alpha = 0;
                        }

                        Color col = Color.FromArgb(alpha, forePen.Color);
                        using (Pen gradPen = new Pen(col, forePen.Width))
                        {
                            float startAngle = angle + (offset - (progressVisible ? 30 : 0)) * (reverse ? 1 : -1);
                            float sweepAngle = 15 * (reverse ? 1 : -1); // draw in reverse direction
                            e.Graphics.DrawArc(gradPen, padding, padding, Width - 2 * padding - 1, Height - 2 * padding - 1, startAngle, sweepAngle);
                        }
                    }
                }
            }
        }

        #endregion

        #region ${0} Methods

        /// <summary>Resets the progress spinner's status.</summary>
        public void Reset()
        {
            progress = minimum;
            angle = 270;
            Refresh();
        }

        #endregion
    }
}