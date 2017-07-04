namespace VisualPlus.Toolkit.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.Handlers;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Styles;
    using VisualPlus.Toolkit.Bases;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(RichTextBox))]
    [DefaultEvent("TextChanged")]
    [DefaultProperty("Text")]
    [Description("The Visual RichTextBox")]
    [Designer(ControlManager.FilterProperties.VisualRichTextBox)]
    public sealed class VisualRichTextBox : InputFieldBase
    {
        #region Variables

        public RichTextBox RichObject = new RichTextBox();

        #endregion

        #region Variables

        private Color backgroundColor;
        private Color backgroundDisabledColor;
        private GraphicsPath controlGraphicsPath;

        #endregion

        #region Constructors

        public VisualRichTextBox()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);


            CreateRichTextBox();
            Controls.Add(RichObject);
            BackColor = Color.Transparent;
            Size = new Size(150, 100);
            //WordWrap = true;
            //AutoWordSelection = false;
            //BorderStyle = BorderStyle.None;
            TextChanged += TextBoxTextChanged;
            UpdateStyles();

            ConfigureStyleManager();
        }

        #endregion

        #region Properties

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }

            set
            {
                backgroundColor = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color BackgroundDisabledColor
        {
            get
            {
                return backgroundDisabledColor;
            }

            set
            {
                backgroundDisabledColor = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Border Border
        {
            get
            {
                return ControlBorder;
            }

            set
            {
                ControlBorder = value;
                Invalidate();
            }
        }


        [Category(Localize.PropertiesCategory.Appearance)]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(false)]
        public override string Text
        {
            get
            {
                return RichObject.Text;
            }

            set
            {
                RichObject.Text = value;
                Invalidate();
            }
        }


        #endregion

        #region Events

        public void CreateRichTextBox()
        {
            // BackColor = Color.Transparent;
            RichTextBox rtb = RichObject;
            rtb.BackColor = backgroundColor;
            rtb.ForeColor = ForeColor;
            rtb.Size = new Size(Width - 10, 100);
            rtb.Location = new Point(7, 5);
            rtb.Text = string.Empty;
            rtb.BorderStyle = BorderStyle.None;
            rtb.Font = Font;
            rtb.Multiline = true;
        }

        public void TextBoxTextChanged(object s, EventArgs e)
        {
            RichObject.Text = Text;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            State = MouseStates.Hover;
            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            RichObject.Font = Font;
            Invalidate();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            RichObject.ForeColor = ForeColor;
            Invalidate();
        }

        protected override void OnLeave(EventArgs e)
        {
          //  base.OnLeave(e);
            State = MouseStates.Normal;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            if (StyleManager.LockedStyle)
            {
                ConfigureStyleManager();
            }

            Color controlTempColor = Enabled ? backgroundColor : backgroundDisabledColor;

            RichObject.BackColor = controlTempColor;
            RichObject.ForeColor = ForeColor;

            controlGraphicsPath = Border.GetBorderShape(ClientRectangle, ControlBorder.Type, ControlBorder.Rounding);
            graphics.FillPath(new SolidBrush(controlTempColor), controlGraphicsPath);

            if (ControlBorder.Visible)
            {
                if ((State == MouseStates.Hover) && ControlBorder.HoverVisible)
                {
                    Border.DrawBorder(graphics, controlGraphicsPath, ControlBorder.Thickness, ControlBorder.HoverColor);
                }
                else
                {
                    Border.DrawBorder(graphics, controlGraphicsPath, ControlBorder.Thickness, ControlBorder.Color);
                }
            }

            graphics.SetClip(controlGraphicsPath);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RichObject.Size = new Size(Width - 13, Height - 11);
        }

        private void ConfigureStyleManager()
        {
            if (StyleManager.VisualStylesManager != null)
            {
                IControl controlStyle = StyleManager.VisualStylesManager.VisualStylesInterface.ControlStyle;

                backgroundColor = controlStyle.Background(0);
                backgroundDisabledColor = controlStyle.FlatButtonDisabled;

            }
            else
            {


                backgroundColor = Settings.DefaultValue.Control.Background(3);
                backgroundDisabledColor = Settings.DefaultValue.Control.FlatButtonDisabled;

            }
        }

        #endregion
    }
}