namespace VisualPlus.Toolkit.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using VisualPlus.Enumerators;
    using VisualPlus.Properties;
    using VisualPlus.Structure;
    using VisualPlus.Toolkit.ActionList;
    using VisualPlus.Toolkit.VisualBase;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(TextBox))]
    [DefaultEvent("TextChanged")]
    [DefaultProperty("Text")]
    [Description("The Visual TextBox")]
    [Designer(typeof(VisualTextBoxTasks))]
    public class VisualTextBox : ContainedControlBase, IInputMethods
    {
        #region Variables

        private TextBox _textBox;
        private Border buttonBorder;
        private Color buttonColor;
        private Image buttonImage;
        private GraphicsPath buttonPath;
        private Rectangle buttonRectangle;
        private bool buttonVisible;
        private int buttonWidth;
        private Size iconSize;
        private Watermark watermark;
        private Panel waterMarkContainer;
        private int xValue;
        private int yValue;

        #endregion

        #region Constructors

        public VisualTextBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);

            // Contains another control
            SetStyle(ControlStyles.ContainerControl, true);

            // Cannot select this control, only the child and does not generate a click event
            SetStyle(ControlStyles.Selectable | ControlStyles.StandardClick, false);

            _textBox = new TextBox
                {
                    Size = GetInternalControlSize(Size, Border),
                    Location = GetInternalControlLocation(Border),
                    Text = string.Empty,
                    BorderStyle = BorderStyle.None,
                    TextAlign = HorizontalAlignment.Left,
                    Font = Font,
                    ForeColor = ForeColor,
                    BackColor = Background,
                    Multiline = false
                };

            buttonWidth = 19;
            buttonImage = Resources.Icon;
            iconSize = new Size(13, 13);

            watermark = new Watermark();

            buttonColor = StyleManager.ControlStyle.FlatButtonEnabled;

            buttonBorder = new Border();

            BackColor = Color.Transparent;
            AutoSize = false;
            Size = new Size(135, 25);

            _textBox.KeyDown += OnKeyDown;
            _textBox.Leave += OnLeave;
            _textBox.Enter += OnEnter;
            _textBox.GotFocus += OnEnter;
            _textBox.LostFocus += OnLeave;
            _textBox.MouseLeave += OnLeave;

            Controls.Add(_textBox);

            waterMarkContainer = null;

            if (watermark.Visible)
            {
                DrawWaterMark();
            }
        }

        public delegate void ButtonClickedEventHandler();

        public event ButtonClickedEventHandler ButtonClicked;

        #endregion

        #region Properties

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Appearance)]
        public Border ButtonBorder
        {
            get
            {
                return buttonBorder;
            }

            set
            {
                buttonBorder = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color ButtonColor
        {
            get
            {
                return buttonColor;
            }

            set
            {
                buttonColor = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Image)]
        public Image ButtonImage
        {
            get
            {
                return buttonImage;
            }

            set
            {
                buttonImage = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Visible)]
        public bool ButtonVisible
        {
            get
            {
                return buttonVisible;
            }

            set
            {
                buttonVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Size)]
        public int ButtonWidth
        {
            get
            {
                return buttonWidth;
            }

            set
            {
                buttonWidth = value;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(false)]
        [Description("Gets access to the contained control.")]
        public Control ContainedControl
        {
            get
            {
                return _textBox;
            }
        }

        public new Font Font
        {
            get
            {
                return base.Font;
            }

            set
            {
                _textBox.Font = value;
                base.Font = value;
            }
        }

        public new Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }

            set
            {
                _textBox.ForeColor = value;
                base.ForeColor = value;
            }
        }

        [Category(Localize.PropertiesCategory.Layout)]
        [Description(Localize.Description.Common.Size)]
        public Size IconSize
        {
            get
            {
                return iconSize;
            }

            set
            {
                iconSize = value;
                Invalidate();
            }
        }

        [Category("Behaviour")]
        [Description("Gets or sets a value indicating whether this is a multiline TextBox control.")]
        [DefaultValue(false)]
        public virtual bool MultiLine
        {
            get
            {
                return _textBox.Multiline;
            }

            set
            {
                _textBox.Multiline = value;
                Invalidate();
            }
        }

        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(false)]
        public new string Text
        {
            get
            {
                return _textBox.Text;
            }

            set
            {
                _textBox.Text = value;
                base.Text = value;

                if (watermark.Visible)
                {
                    // If the text of the text box is not empty.
                    if (_textBox.TextLength > 0)
                    {
                        // Remove the watermark
                        RemoveWaterMark();
                    }
                    else
                    {
                        // But if the text is empty, draw the watermark again.
                        DrawWaterMark();
                    }
                }
            }
        }

        [TypeConverter(typeof(WatermarkConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.PropertiesCategory.Behavior)]
        public Watermark Watermark
        {
            get
            {
                return watermark;
            }

            set
            {
                watermark = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        /// <summary>Appends text to the current text of a rich text box.</summary>
        /// <param name="text">The text to append to the current contents of the text box.</param>
        public void AppendText(string text)
        {
            _textBox.AppendText(text);
        }

        /// <summary>Clears all text from the text box control.</summary>
        public void Clear()
        {
            _textBox.Clear();
        }

        /// <summary>Clears information about the most recent operation from the undo buffer of the rich text box.</summary>
        public void ClearUndo()
        {
            _textBox.ClearUndo();
        }

        /// <summary>Copies the current selection in the text box to the Clipboard.</summary>
        public void Copy()
        {
            _textBox.Copy();
        }

        /// <summary>Moves the current selection in the text box to the Clipboard.</summary>
        public void Cut()
        {
            _textBox.Cut();
        }

        /// <summary>
        ///     Specifies that the value of the SelectionLength property is zero so that no characters are selected in the
        ///     control.
        /// </summary>
        public void DeselectAll()
        {
            _textBox.DeselectAll();
        }

        /// <summary>Retrieves the character that is closest to the specified location within the control.</summary>
        /// <param name="pt">The location from which to seek the nearest character.</param>
        /// <returns>The character at the specified location.</returns>
        public int GetCharFromPosition(Point pt)
        {
            return _textBox.GetCharFromPosition(pt);
        }

        /// <summary>Retrieves the index of the character nearest to the specified location.</summary>
        /// <param name="pt">The location to search.</param>
        /// <returns>The zero-based character index at the specified location.</returns>
        public int GetCharIndexFromPosition(Point pt)
        {
            return _textBox.GetCharIndexFromPosition(pt);
        }

        /// <summary>Retrieves the index of the first character of a given line.</summary>
        /// <param name="lineNumber">The line for which to get the index of its first character.</param>
        /// <returns>The zero-based character index in the specified line.</returns>
        public int GetFirstCharIndexFromLine(int lineNumber)
        {
            return _textBox.GetFirstCharIndexFromLine(lineNumber);
        }

        /// <summary>Retrieves the index of the first character of the current line.</summary>
        /// <returns>The zero-based character index in the current line.</returns>
        public int GetFirstCharIndexOfCurrentLine()
        {
            return _textBox.GetFirstCharIndexOfCurrentLine();
        }

        /// <summary>Retrieves the line number from the specified character position within the text of the RichTextBox control.</summary>
        /// <param name="index">The character index position to search.</param>
        /// <returns>The zero-based line number in which the character index is located.</returns>
        public int GetLineFromCharIndex(int index)
        {
            return _textBox.GetLineFromCharIndex(index);
        }

        /// <summary>Retrieves the location within the control at the specified character index.</summary>
        /// <param name="index">The index of the character for which to retrieve the location.</param>
        /// <returns>The location of the specified character.</returns>
        public Point GetPositionFromCharIndex(int index)
        {
            return _textBox.GetPositionFromCharIndex(index);
        }

        /// <summary>Replaces the current selection in the text box with the contents of the Clipboard.</summary>
        public void Paste()
        {
            _textBox.Paste();
        }

        /// <summary>Scrolls the contents of the control to the current caret position.</summary>
        public void ScrollToCaret()
        {
            _textBox.ScrollToCaret();
        }

        /// <summary>Selects a range of text in the control.</summary>
        /// <param name="start">The position of the first character in the current text selection within the text box.</param>
        /// <param name="length">The number of characters to select.</param>
        public void Select(int start, int length)
        {
            _textBox.Select(start, length);
        }

        /// <summary>Selects all text in the control.</summary>
        public void SelectAll()
        {
            _textBox.SelectAll();
        }

        /// <summary>Undoes the last edit operation in the text box.</summary>
        public void Undo()
        {
            _textBox.Undo();
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            if (watermark.Visible)
            {
                // If focused use focus color
                watermark.Brush = new SolidBrush(watermark.ActiveColor);

                // Don't draw watermark if contains text.
                if (_textBox.TextLength <= 0)
                {
                    RemoveWaterMark();
                    DrawWaterMark();
                }
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _textBox.Focus();
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);

            // Check if there is a watermark
            if (waterMarkContainer != null)
            {
                // if there is a watermark it should also be invalidated();
                waterMarkContainer.Invalidate();
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            if (watermark.Visible)
            {
                // If the user has written something and left the control
                if (_textBox.TextLength > 0)
                {
                    // Remove the watermark
                    RemoveWaterMark();
                }
                else
                {
                    // But if the user didn't write anything, then redraw the control.
                    Invalidate();
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            OnMouseClick(e);
            if (buttonVisible)
            {
                // Check if mouse in X position.
                if ((xValue > buttonRectangle.X) && (xValue < Width))
                {
                    // Determine the button middle separator by checking for the Y position.
                    if ((yValue > buttonRectangle.Y) && (yValue < Height))
                    {
                        ButtonClicked?.Invoke();
                    }
                }
            }

            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            xValue = e.Location.X;
            yValue = e.Location.Y;
            Invalidate();

            // IBeam cursor toggle
            if (e.X < buttonRectangle.X)
            {
                Cursor = Cursors.IBeam;
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;

            if (!_textBox.Multiline)
            {
                Height = GetTextBoxHeight();
            }

            buttonRectangle = new Rectangle(Width - buttonWidth, 0, buttonWidth, Height);

            graphics.SetClip(ControlGraphicsPath);

            if (_textBox.BackColor != Background)
            {
                _textBox.BackColor = Background;
            }

            _textBox.ForeColor = ForeColor;

            // Setup button
            if (!_textBox.Multiline && buttonVisible)
            {
                buttonPath = new GraphicsPath();
                buttonPath.AddRectangle(buttonRectangle);
                buttonPath.CloseAllFigures();

                // Buttons background
                graphics.FillPath(new SolidBrush(buttonColor), buttonPath);

                Size imageSize = new Size(iconSize.Width, iconSize.Height);
                Point imagePoint = new Point((buttonRectangle.X + (buttonRectangle.Width / 2)) - (imageSize.Width / 2), (buttonRectangle.Y + (buttonRectangle.Height / 2)) - (imageSize.Height / 2));

                Rectangle imageRectangle = new Rectangle(imagePoint, imageSize);

                graphics.SetClip(buttonPath);
                graphics.DrawImage(buttonImage, imageRectangle);
                graphics.SetClip(ControlGraphicsPath);

                Border.DrawBorderStyle(graphics, buttonBorder, MouseState, buttonPath);

                _textBox.Width = buttonRectangle.X - 10;
            }

            graphics.ResetClip();

            if (watermark.Visible)
            {
                DrawWaterMark();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (!_textBox.Multiline)
            {
                Height = GetTextBoxHeight();
            }

            _textBox.Location = GetInternalControlLocation(ControlBorder);
            _textBox.Size = GetInternalControlSize(Size, ControlBorder);
        }

        private void DrawWaterMark()
        {
            if ((waterMarkContainer == null) && (_textBox.TextLength <= 0))
            {
                waterMarkContainer = new Panel(); // Creates the new panel instance
                waterMarkContainer.Paint += WaterMarkContainer_Paint;
                waterMarkContainer.Invalidate();
                waterMarkContainer.Click += WaterMarkContainer_Click;
                Controls.Add(waterMarkContainer); // adds the control
                waterMarkContainer.BringToFront();
            }
        }

        private int GetTextBoxHeight()
        {
            if (_textBox.TextLength > 0)
            {
                return GDI.MeasureText(Text, Font).Height * 2;
            }
            else
            {
                return GDI.MeasureText("Hello World.", Font).Height * 2;
            }
        }

        private void OnEnter(object sender, EventArgs e)
        {
            MouseState = MouseStates.Hover;
        }

        private void OnKeyDown(object obj, KeyEventArgs e)
        {
            // Select all
            if (e.Control && (e.KeyCode == Keys.A))
            {
                _textBox.SelectAll();
                e.SuppressKeyPress = true;
            }

            // Copy
            if (e.Control && (e.KeyCode == Keys.C))
            {
                _textBox.Copy();
                e.SuppressKeyPress = true;
            }
        }

        private void OnLeave(object sender, EventArgs e)
        {
            if (!_textBox.Focused)
            {
                MouseState = MouseStates.Normal;
            }
        }

        private void RemoveWaterMark()
        {
            if (waterMarkContainer != null)
            {
                Controls.Remove(waterMarkContainer);
                waterMarkContainer = null;
            }
        }

        private void WaterMarkContainer_Click(object sender, EventArgs e)
        {
            Focus();
        }

        private void WaterMarkContainer_Paint(object sender, PaintEventArgs e)
        {
            // Configure the watermark
            waterMarkContainer.Location = new Point(2, 0);
            waterMarkContainer.Height = Height;
            waterMarkContainer.Width = Width;

            // Forces it to resize with the parent control
            waterMarkContainer.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            // Set color
            watermark.Brush = ContainsFocus ? new SolidBrush(watermark.ActiveColor) : new SolidBrush(watermark.InactiveColor);

            // Draws the string on the panel
            e.Graphics.DrawString(watermark.Text, watermark.Font, watermark.Brush, new PointF(-2f, 1f));
        }

        #endregion
    }
}