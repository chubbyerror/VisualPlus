namespace VisualPlus.Controls
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Localization;

    /// <summary>The visual panel.</summary>
    // [ToolboxBitmap(typeof(Panel)), Designer(VSDesignerBinding.VisualPanel), Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    [ToolboxBitmap(typeof(Panel)), Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public partial class VisualPanel : Panel
    {
        #region  ${0} Variables

        private static BorderShape borderShape = StylesManager.DefaultValue.BorderShape;
        private Color backgroundColor = StylesManager.DefaultValue.Style.BackgroundColor(0);
        private Color borderColor = StylesManager.DefaultValue.Style.BorderColor(0);
        private Color borderHoverColor = StylesManager.DefaultValue.Style.BorderColor(1);
        private bool borderHoverVisible = StylesManager.DefaultValue.BorderHoverVisible;
        private int borderRounding = StylesManager.DefaultValue.BorderRounding;
        private int borderSize = StylesManager.DefaultValue.BorderSize;
        private bool borderVisible = StylesManager.DefaultValue.BorderVisible;
        private GraphicsPath controlGraphicsPath;
        private ControlState controlState = ControlState.Normal;

        #endregion

        #region ${0} Properties

        public VisualPanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint,
                true);

            Size = new Size(187, 117);
            Padding = new Padding(5, 5, 5, 5);
            DoubleBuffered = true;

            UpdateStyles();
        }

        [Category(Localize.Category.Appearance), Description(Localize.Description.ComponentColor)]
        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }

            set
            {
                backgroundColor = value;
                BackColorFix();
                Invalidate();
            }
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

        #endregion

        #region ${0} Events

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
            UpdateLocationPoints();

            // Draw background
            graphics.FillPath(new SolidBrush(backgroundColor), controlGraphicsPath);

            // Setup control border
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
            // Update paths
            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, borderShape, borderRounding);
        }

        #endregion

        #region ${0} Methods

        public virtual void BackColorFix()
        {
            foreach (object control in Controls)
            {
                if (control is VisualColorWheel)
                {
                    (control as VisualColorWheel).BackColor = backgroundColor;
                }

                if (control is VisualButton)
                {
                    (control as VisualButton).BackColor = backgroundColor;
                }

                if (control is VisualCheckBox)
                {
                    (control as VisualCheckBox).BackColor = backgroundColor;
                }

                if (control is VisualCircleProgressBar)
                {
                    (control as VisualCircleProgressBar).BackColor = backgroundColor;
                }

                if (control is VisualComboBox)
                {
                    (control as VisualComboBox).BackColor = backgroundColor;
                }

                if (control is VisualGroupBox)
                {
                    (control as VisualGroupBox).BackColor = backgroundColor;
                }

                if (control is VisualListBox)
                {
                    (control as VisualListBox).BackColor = backgroundColor;
                }

                if (control is VisualNumericUpDown)
                {
                    (control as VisualNumericUpDown).BackColor = backgroundColor;
                }

                if (control is VisualProgressBar)
                {
                    (control as VisualProgressBar).BackColor = backgroundColor;
                }

                if (control is VisualProgressIndicator)
                {
                    (control as VisualProgressIndicator).BackColor = backgroundColor;
                }

                if (control is VisualProgressSpinner)
                {
                    (control as VisualProgressSpinner).BackColor = backgroundColor;
                }

                if (control is VisualRadioButton)
                {
                    (control as VisualRadioButton).BackColor = backgroundColor;
                }

                if (control is VisualRichTextBox)
                {
                    (control as VisualRichTextBox).BackColor = backgroundColor;
                }

                if (control is VisualSeparator)
                {
                    (control as VisualSeparator).BackColor = backgroundColor;
                }

                if (control is VisualTabControl)
                {
                    (control as VisualTabControl).BackColor = backgroundColor;
                }

                if (control is VisualTextBox)
                {
                    (control as VisualTextBox).BackColor = backgroundColor;
                }

                if (control is VisualToggle)
                {
                    (control as VisualToggle).BackColor = backgroundColor;
                }

                if (control is VisualTrackBar)
                {
                    (control as VisualTrackBar).BackColor = backgroundColor;
                }

                if (control is VisualListView)
                {
                    (control as VisualListView).BackColor = backgroundColor;
                }

                if (control is VisualLabel)
                {
                    (control as VisualLabel).BackColor = backgroundColor;
                }
            }
        }

        #endregion
    }
}