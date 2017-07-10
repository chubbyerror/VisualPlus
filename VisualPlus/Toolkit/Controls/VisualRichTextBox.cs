namespace VisualPlus.Toolkit.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Windows.Forms;

    using VisualPlus.Enumerators;
    using VisualPlus.Managers;
    using VisualPlus.Structure;
    using VisualPlus.Toolkit.VisualBase;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(RichTextBox))]
    [DefaultEvent("TextChanged")]
    [DefaultProperty("Text")]
    [Description("The Visual RichTextBox")]
    [Designer(ControlManager.FilterProperties.VisualRichTextBox)]
    public sealed class VisualRichTextBox : InputFieldBase, IInputMethods
    {
        #region Variables

        public RichTextBox InternalRichTextBox = new RichTextBox();

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
            Controls.Add(InternalRichTextBox);
            BackColor = Color.Transparent;
            Size = new Size(150, 100);

            // WordWrap = true;
            // AutoWordSelection = false;
            // BorderStyle = BorderStyle.None;
            TextChanged += TextBoxTextChanged;
            UpdateStyles();

            backgroundColor = StyleManager.ControlStyle.Background(3);
            backgroundDisabledColor = StyleManager.ControlStyle.FlatButtonDisabled;
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
                return InternalRichTextBox.Text;
            }

            set
            {
                InternalRichTextBox.Text = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        /// <summary>Appends text to the current text of a rich text box.</summary>
        /// <param name="text">The text to append to the current contents of the text box.</param>
        public void AppendText(string text)
        {
            InternalRichTextBox.AppendText(text);
        }

        /// <summary>Determines whether you can paste information from the Clipboard in the specified data format.</summary>
        /// <param name="clipFormat">One of the System.Windows.Forms.DataFormats.Format values.</param>
        /// <returns>true if you can paste data from the Clipboard in the specified data format; otherwise, false.</returns>
        public bool CanPaste(DataFormats.Format clipFormat)
        {
            return InternalRichTextBox.CanPaste(clipFormat);
        }

        /// <summary>Clears all text from the text box control.</summary>
        public void Clear()
        {
            InternalRichTextBox.Clear();
        }

        /// <summary>Clears information about the most recent operation from the undo buffer of the rich text box.</summary>
        public void ClearUndo()
        {
            InternalRichTextBox.ClearUndo();
        }

        /// <summary>Copies the current selection in the text box to the Clipboard.</summary>
        public void Copy()
        {
            InternalRichTextBox.Copy();
        }

        public void CreateRichTextBox()
        {
            // BackColor = Color.Transparent;
            RichTextBox rtb = InternalRichTextBox;
            rtb.BackColor = backgroundColor;
            rtb.ForeColor = ForeColor;
            rtb.Size = new Size(Width - 10, 100);
            rtb.Location = new Point(7, 5);
            rtb.Text = string.Empty;
            rtb.BorderStyle = BorderStyle.None;
            rtb.Font = Font;
            rtb.Multiline = true;
        }

        /// <summary>Moves the current selection in the text box to the Clipboard.</summary>
        public void Cut()
        {
            InternalRichTextBox.Cut();
        }

        /// <summary>
        ///     Specifies that the value of the SelectionLength property is zero so that no characters are selected in the
        ///     control.
        /// </summary>
        public void DeselectAll()
        {
            InternalRichTextBox.DeselectAll();
        }

        /// <summary>Searches the text in a RichTextBox control for a string.</summary>
        /// <param name="str">The text to locate in the control.</param>
        /// <returns>
        ///     The location within the control where the search text was found or -1 if the search string is not found or an
        ///     empty search string is specified in the str parameter.
        /// </returns>
        public int Find(string str)
        {
            return InternalRichTextBox.Find(str);
        }

        /// <summary>Searches the text of a RichTextBox control for the first instance of a character from a list of characters.</summary>
        /// <param name="characterSet">The array of characters to search for.</param>
        /// <returns>
        ///     The location within the control where the search characters were found or -1 if the search characters are not
        ///     found or an empty search character set is specified in the char parameter.
        /// </returns>
        public int Find(char[] characterSet)
        {
            return InternalRichTextBox.Find(characterSet);
        }

        /// <summary>
        ///     Searches the text of a RichTextBox control, at a specific starting point, for the first instance of a
        ///     character from a list of characters.
        /// </summary>
        /// <param name="characterSet">The array of characters to search for.</param>
        /// <param name="start">The location within the control's text at which to begin searching.</param>
        /// <returns>The location within the control where the search characters are found.</returns>
        public int Find(char[] characterSet, int start)
        {
            return InternalRichTextBox.Find(characterSet, start);
        }

        /// <summary>Searches the text in a RichTextBox control for a string with specific options applied to the search.</summary>
        /// <param name="str">The text to locate in the control.</param>
        /// <param name="options">A bit wise combination of the RichTextBoxFinds values.</param>
        /// <returns>The location within the control where the search text was found.</returns>
        public int Find(string str, RichTextBoxFinds options)
        {
            return InternalRichTextBox.Find(str, options);
        }

        /// <summary>
        ///     Searches a range of text in a RichTextBox control for the first instance of a character from a list of
        ///     characters.
        /// </summary>
        /// <param name="characterSet">The array of characters to search for.</param>
        /// <param name="start">The location within the control's text at which to begin searching.</param>
        /// <param name="end">The location within the control's text at which to end searching.</param>
        /// <returns>The location within the control where the search characters are found.</returns>
        public int Find(char[] characterSet, int start, int end)
        {
            return InternalRichTextBox.Find(characterSet, start, end);
        }

        /// <summary>
        ///     Searches the text in a RichTextBox control for a string at a specific location within the control and with
        ///     specific options applied to the search.
        /// </summary>
        /// <param name="str">The text to locate in the control.</param>
        /// <param name="start">The location within the control's text at which to begin searching.</param>
        /// <param name="options">A bit wise combination of the RichTextBoxFinds values.</param>
        /// <returns>The location within the control where the search text was found.</returns>
        public int Find(string str, int start, RichTextBoxFinds options)
        {
            return InternalRichTextBox.Find(str, start, options);
        }

        /// <summary>
        ///     Searches the text in a RichTextBox control for a string within a range of text within the control and with
        ///     specific options applied to the search.
        /// </summary>
        /// <param name="str">The text to locate in the control.</param>
        /// <param name="start">The location within the control's text at which to begin searching.</param>
        /// <param name="end">
        ///     The location within the control's text at which to end searching. This value must be equal to
        ///     negative one (-1) or greater than or equal to the start parameter.
        /// </param>
        /// <param name="options">A bit wise combination of the RichTextBoxFinds values.</param>
        /// <returns>Returns search results.</returns>
        public int Find(string str, int start, int end, RichTextBoxFinds options)
        {
            return InternalRichTextBox.Find(str, start, end, options);
        }

        /// <summary>Retrieves the character that is closest to the specified location within the control.</summary>
        /// <param name="pt">The location from which to seek the nearest character.</param>
        /// <returns>The character at the specified location.</returns>
        public int GetCharFromPosition(Point pt)
        {
            return InternalRichTextBox.GetCharFromPosition(pt);
        }

        /// <summary>Retrieves the index of the character nearest to the specified location.</summary>
        /// <param name="pt">The location to search.</param>
        /// <returns>The zero-based character index at the specified location.</returns>
        public int GetCharIndexFromPosition(Point pt)
        {
            return InternalRichTextBox.GetCharIndexFromPosition(pt);
        }

        /// <summary>Retrieves the index of the first character of a given line.</summary>
        /// <param name="lineNumber">The line for which to get the index of its first character.</param>
        /// <returns>The zero-based character index in the specified line.</returns>
        public int GetFirstCharIndexFromLine(int lineNumber)
        {
            return InternalRichTextBox.GetFirstCharIndexFromLine(lineNumber);
        }

        /// <summary>Retrieves the index of the first character of the current line.</summary>
        /// <returns>The zero-based character index in the current line.</returns>
        public int GetFirstCharIndexOfCurrentLine()
        {
            return InternalRichTextBox.GetFirstCharIndexOfCurrentLine();
        }

        /// <summary>Retrieves the line number from the specified character position within the text of the RichTextBox control.</summary>
        /// <param name="index">The character index position to search.</param>
        /// <returns>The zero-based line number in which the character index is located.</returns>
        public int GetLineFromCharIndex(int index)
        {
            return InternalRichTextBox.GetLineFromCharIndex(index);
        }

        /// <summary>Retrieves the location within the control at the specified character index.</summary>
        /// <param name="index">The index of the character for which to retrieve the location.</param>
        /// <returns>The location of the specified character.</returns>
        public Point GetPositionFromCharIndex(int index)
        {
            return InternalRichTextBox.GetPositionFromCharIndex(index);
        }

        /// <summary>Loads a rich text format (RTF) or standard ASCII text file into the RichTextBox control.</summary>
        /// <param name="path">The name and location of the file to load into the control.</param>
        public void LoadFile(string path)
        {
            InternalRichTextBox.LoadFile(path);
        }

        /// <summary>Loads the contents of an existing data stream into the RichTextBox control.</summary>
        /// <param name="data">A stream of data to load into the RichTextBox control.</param>
        /// <param name="fileType">One of the RichTextBoxStreamType values.</param>
        public void LoadFile(Stream data, RichTextBoxStreamType fileType)
        {
            InternalRichTextBox.LoadFile(data, fileType);
        }

        /// <summary>Loads a specific type of file into the RichTextBox control.</summary>
        /// <param name="path">The name and location of the file to load into the control.</param>
        /// <param name="fileType">One of the RichTextBoxStreamType values.</param>
        public void LoadFile(string path, RichTextBoxStreamType fileType)
        {
            InternalRichTextBox.LoadFile(path, fileType);
        }

        /// <summary>Replaces the current selection in the text box with the contents of the Clipboard.</summary>
        public void Paste()
        {
            InternalRichTextBox.Paste();
        }

        /// <summary>Pastes the contents of the Clipboard in the specified Clipboard format.</summary>
        /// <param name="clipFormat">The Clipboard format in which the data should be obtained from the Clipboard.</param>
        public void Paste(DataFormats.Format clipFormat)
        {
            InternalRichTextBox.Paste(clipFormat);
        }

        /// <summary>Reapplies the last operation that was undone in the control.</summary>
        public void Redo()
        {
            InternalRichTextBox.Redo();
        }

        /// <summary>Saves the contents of the RichTextBox to a rich text format (RTF) file.</summary>
        /// <param name="path">The name and location of the file to save.</param>
        public void SaveFile(string path)
        {
            InternalRichTextBox.SaveFile(path);
        }

        /// <summary>Saves the contents of a RichTextBox control to an open data stream.</summary>
        /// <param name="data">The data stream that contains the file to save to.</param>
        /// <param name="fileType">One of the RichTextBoxStreamType values.</param>
        public void SaveFile(Stream data, RichTextBoxStreamType fileType)
        {
            InternalRichTextBox.SaveFile(data, fileType);
        }

        /// <summary>Saves the contents of the KryptonRichTextBox to a specific type of file.</summary>
        /// <param name="path">The name and location of the file to save.</param>
        /// <param name="fileType">One of the RichTextBoxStreamType values.</param>
        public void SaveFile(string path, RichTextBoxStreamType fileType)
        {
            InternalRichTextBox.SaveFile(path, fileType);
        }

        /// <summary>Scrolls the contents of the control to the current caret position.</summary>
        public void ScrollToCaret()
        {
            InternalRichTextBox.ScrollToCaret();
        }

        /// <summary>Selects a range of text in the control.</summary>
        /// <param name="start">The position of the first character in the current text selection within the text box.</param>
        /// <param name="length">The number of characters to select.</param>
        public void Select(int start, int length)
        {
            InternalRichTextBox.Select(start, length);
        }

        /// <summary>Selects all text in the control.</summary>
        public void SelectAll()
        {
            InternalRichTextBox.SelectAll();
        }

        public void TextBoxTextChanged(object s, EventArgs e)
        {
            InternalRichTextBox.Text = Text;
        }

        /// <summary>Undoes the last edit operation in the text box.</summary>
        public void Undo()
        {
            InternalRichTextBox.Undo();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            InternalRichTextBox.Font = Font;
            Invalidate();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            InternalRichTextBox.ForeColor = ForeColor;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            Color controlTempColor = Enabled ? backgroundColor : backgroundDisabledColor;

            InternalRichTextBox.BackColor = controlTempColor;
            InternalRichTextBox.ForeColor = ForeColor;

            controlGraphicsPath = Border.GetBorderShape(ClientRectangle, ControlBorder.Type, ControlBorder.Rounding);
            graphics.FillPath(new SolidBrush(controlTempColor), controlGraphicsPath);

            if (ControlBorder.Visible)
            {
                if ((MouseState == MouseStates.Hover) && ControlBorder.HoverVisible)
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
            InternalRichTextBox.Size = new Size(Width - 13, Height - 11);
        }

        #endregion
    }
}