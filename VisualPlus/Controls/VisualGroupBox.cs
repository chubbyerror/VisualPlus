namespace VisualPlus.Controls
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Localization;

    /// <summary>The visual GroupBox.</summary>
    [ToolboxBitmap(typeof(GroupBox)), Designer(VSDesignerBinding.VisualGroupBox),
     Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public sealed class VisualGroupBox : GroupBox
    {
        #region  ${0} Variables

        private static BorderShape borderShape = StylesManager.DefaultValue.BorderShape;
        private Color borderColor = StylesManager.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = StylesManager.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = StylesManager.DefaultValue.BorderHoverVisible;
        private int borderRounding = StylesManager.DefaultValue.BorderRounding;
        private int borderSize = StylesManager.DefaultValue.BorderSize;
        private bool borderVisible = StylesManager.DefaultValue.BorderVisible;
        private GraphicsPath controlGraphicsPath;
        private ControlState controlState = ControlState.Normal;
        private Color foreColor = StylesManager.DefaultValue.Style.ForeColor(0);
        private Color groupBoxColor = StylesManager.DefaultValue.Style.BackgroundColor(0);
        private Color textDisabledColor = StylesManager.DefaultValue.Style.TextDisabled;
        private Color titleBoxColor = StylesManager.DefaultValue.Style.BackgroundColor(1);
        private GraphicsPath titleBoxPath;
        private Rectangle titleBoxRectangle;
        private bool titleBoxVisible = StylesManager.DefaultValue.TitleBoxVisible;

        #endregion

        #region ${0} Properties

        public VisualGroupBox()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            ForeColor = StylesManager.DefaultValue.Style.ForeColor(0);
            Size = new Size(212, 104);
            MinimumSize = new Size(136, 50);
            Padding = new Padding(5, 28, 5, 5);

            UpdateStyles();
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.BorderColor)]
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }

            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.BorderHoverColor)]
        public Color BorderHoverColor
        {
            get
            {
                return borderHoverColor;
            }

            set
            {
                borderHoverColor = value;
                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.BorderHoverVisible), Category(Localize.Category.Behavior),
         Description(Localize.Description.BorderHoverVisible)]
        public bool BorderHoverVisible
        {
            get
            {
                return borderHoverVisible;
            }

            set
            {
                borderHoverVisible = value;
                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.BorderRounding), Category(Localize.Category.Layout),
         Description(Localize.Description.BorderRounding)]
        public int BorderRounding
        {
            get
            {
                return borderRounding;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, StylesManager.MinimumRounding, StylesManager.MaximumRounding))
                {
                    borderRounding = value;
                }

                UpdateLocationPoints();
                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.BorderShape), Category(Localize.Category.Appearance),
         Description(Localize.Description.ComponentShape)]
        public BorderShape BorderShape
        {
            get
            {
                return borderShape;
            }

            set
            {
                borderShape = value;
                controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.BorderSize), Category(Localize.Category.Layout),
         Description(Localize.Description.BorderSize)]
        public int BorderSize
        {
            get
            {
                return borderSize;
            }

            set
            {
                if (ExceptionHandler.ArgumentOutOfRangeException(value, StylesManager.MinimumBorderSize, StylesManager.MaximumBorderSize))
                {
                    borderSize = value;
                }

                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.BorderVisible), Category(Localize.Category.Behavior),
         Description(Localize.Description.BorderVisible)]
        public bool BorderVisible
        {
            get
            {
                return borderVisible;
            }

            set
            {
                borderVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color GroupBoxColor
        {
            get
            {
                return groupBoxColor;
            }

            set
            {
                groupBoxColor = value;
                BackColorFix();
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.TextColor)]
        public Color TextColor
        {
            get
            {
                return foreColor;
            }

            set
            {
                foreColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color TextDisabledColor
        {
            get
            {
                return textDisabledColor;
            }

            set
            {
                textDisabledColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color TitleBoxColor
        {
            get
            {
                return titleBoxColor;
            }

            set
            {
                titleBoxColor = value;
                Invalidate();
            }
        }

        [DefaultValue(StylesManager.DefaultValue.TitleBoxVisible), Category(Localize.Category.Behavior),
         Description(Localize.Description.TitleBoxVisible)]
        public bool TitleBoxVisible
        {
            get
            {
                return titleBoxVisible;
            }

            set
            {
                titleBoxVisible = value;
                Invalidate();
            }
        }

        #endregion

        #region ${0} Events

        private void BackColorFix()
        {
            foreach (object control in Controls)
            {
                if (control is VisualButton)
                {
                    (control as VisualButton).BackColor = groupBoxColor;
                }

                if (control is VisualCheckBox)
                {
                    (control as VisualCheckBox).BackColor = groupBoxColor;
                }

                if (control is VisualCircleProgressBar)
                {
                    (control as VisualCircleProgressBar).BackColor = groupBoxColor;
                }

                if (control is VisualComboBox)
                {
                    (control as VisualComboBox).BackColor = groupBoxColor;
                }

                if (control is VisualGroupBox)
                {
                    (control as VisualGroupBox).BackColor = groupBoxColor;
                }

                if (control is VisualListBox)
                {
                    (control as VisualListBox).BackColor = groupBoxColor;
                }

                if (control is VisualNumericUpDown)
                {
                    (control as VisualNumericUpDown).BackColor = groupBoxColor;
                }

                if (control is VisualProgressBar)
                {
                    (control as VisualProgressBar).BackColor = groupBoxColor;
                }

                if (control is VisualProgressIndicator)
                {
                    (control as VisualProgressIndicator).BackColor = groupBoxColor;
                }

                if (control is VisualProgressSpinner)
                {
                    (control as VisualProgressSpinner).BackColor = groupBoxColor;
                }

                if (control is VisualRadioButton)
                {
                    (control as VisualRadioButton).BackColor = groupBoxColor;
                }

                if (control is VisualRichTextBox)
                {
                    (control as VisualRichTextBox).BackColor = groupBoxColor;
                }

                if (control is VisualSeparator)
                {
                    (control as VisualSeparator).BackColor = groupBoxColor;
                }

                if (control is VisualTabControl)
                {
                    (control as VisualTabControl).BackColor = groupBoxColor;
                }

                if (control is VisualTextBox)
                {
                    (control as VisualTextBox).BackColor = groupBoxColor;
                }

                if (control is VisualToggle)
                {
                    (control as VisualToggle).BackColor = groupBoxColor;
                }

                if (control is VisualTrackBar)
                {
                    (control as VisualTrackBar).BackColor = groupBoxColor;
                }
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            BackColorFix();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            controlState = ControlState.Hover;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            controlState = ControlState.Normal;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.CompositingQuality = CompositingQuality.GammaCorrected;

            UpdateLocationPoints();

            // Set control state color
            foreColor = Enabled ? foreColor : textDisabledColor;

            // Draw the body of the GroupBoxColor
            graphics.FillPath(new SolidBrush(groupBoxColor), controlGraphicsPath);

            // Setup group box border
            if (borderVisible)
            {
                if (controlState == ControlState.Hover && borderHoverVisible)
                {
                    GDI.DrawBorder(graphics, controlGraphicsPath, borderSize, borderHoverColor);
                }
                else
                {
                    GDI.DrawBorder(graphics, controlGraphicsPath, borderSize, borderColor);
                }
            }

            // Draw the title box
            if (titleBoxVisible)
            {
                // Draw the background of the title box
                graphics.FillPath(new SolidBrush(titleBoxColor), titleBoxPath);

                // Setup title boxborder
                if (borderVisible)
                {
                    if (controlState == ControlState.Hover && borderHoverVisible)
                    {
                        GDI.DrawBorder(graphics, titleBoxPath, borderSize, borderHoverColor);
                    }
                    else
                    {
                        GDI.DrawBorder(graphics, titleBoxPath, borderSize, borderColor);
                    }
                }
            }

            // Draw the specified string from 'Text' property inside the title box
            StringFormat stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

            graphics.DrawString(Text, Font, new SolidBrush(foreColor), titleBoxRectangle, stringFormat);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLocationPoints();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateLocationPoints();
        }

        private void UpdateLocationPoints()
        {
            titleBoxRectangle = new Rectangle(0, 0, Width - 1, 25);

            // Determine type of border rounding to draw
            if (borderShape == BorderShape.Rounded)
            {
                titleBoxPath = GDI.DrawRoundedRectangle(titleBoxRectangle, borderRounding);
            }
            else
            {
                titleBoxPath = GDI.DrawRoundedRectangle(titleBoxRectangle, 1);
            }

            // Update paths
            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
        }

        #endregion
    }
}