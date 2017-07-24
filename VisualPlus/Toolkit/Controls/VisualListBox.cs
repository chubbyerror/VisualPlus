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
    using VisualPlus.Structure;
    using VisualPlus.Toolkit.ActionList;
    using VisualPlus.Toolkit.VisualBase;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ListBox))]
    [DefaultEvent("SelectedIndexChanged")]
    [DefaultProperty("Items")]
    [DefaultBindingProperty("SelectedValue")]
    [Description("The Visual ListBox")]
    [Designer(typeof(VisualListBoxTasks))]
    public class VisualListBox : ContainedControlBase, IContainedControl
    {
        #region Variables

        private bool _alternateColors;

        private FixedContentValue _contentValues = new FixedContentValue();
        private Color _itemAlternate;
        private Color _itemNormal;
        private Color _itemSelected;
        private ListBox _listBox;

        #endregion

        #region Constructors

        public VisualListBox()
        {
            // Contains another control
            SetStyle(ControlStyles.ContainerControl, true);

            // Cannot select this control, only the child ListView and does not generate a click event
            SetStyle(ControlStyles.Selectable | ControlStyles.StandardClick, false);

            Background = StyleManager.ControlStyle.Background(3);

            _listBox = new ListBox
                {
                    BackColor = Background,
                    Size = GetInternalControlSize(Size, Border),
                    BorderStyle = BorderStyle.None,
                    IntegralHeight = false,
                    MultiColumn = false,
                    DrawMode = DrawMode.OwnerDrawVariable,
                    ItemHeight = 18,
                    Location = GetInternalControlLocation(ControlBorder)
                };

            AutoSize = true;
            BackColor = Color.Transparent;
            Size = new Size(250, 150);

            _itemNormal = StyleManager.ControlStyle.Background(0);
            _itemAlternate = StyleManager.BorderStyle.Color;
            _itemSelected = StyleManager.BorderStyle.HoverColor;

            _listBox.DataSourceChanged += ListBox_DataSourceChanged;
            _listBox.DisplayMemberChanged += ListBox_DisplayMemberChanged;
            _listBox.ValueMemberChanged += ListBox_ValueMemberChanged;
            _listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            _listBox.SelectedValueChanged += ListBox_SelectedValueChanged;
            _listBox.Format += ListBox_Format;
            _listBox.FormatInfoChanged += ListBox_FormatInfoChanged;
            _listBox.FormatStringChanged += ListBox_FormatStringChanged;
            _listBox.PreviewKeyDown += ListBox_PreviewKeyDown;
            _listBox.Validating += ListBox_Validating;
            _listBox.Validated += ListBox_Validated;
            _listBox.DrawItem += ListBox_DrawItem;
            _listBox.MeasureItem += ListBox_MeasureItem;
            _listBox.MouseDown += ListBox_MouseDown;
            _listBox.KeyDown += ListBox_KeyDown;
            _listBox.GotFocus += ListBox_GotFocus;
            _listBox.LostFocus += ListBox_LostFocus;
            _listBox.KeyDown += ListBox_KeyDown;
            _listBox.KeyUp += ListBox_KeyUp;
            _listBox.KeyPress += ListBox_KeyPress;

            Controls.Add(_listBox);
        }

        [Description("Occurs when the value of the DataSource property changes.")]
        [Category("Property Changed")]
        public event EventHandler DataSourceChanged;

        [Description("Occurs when the value of the DisplayMember property changes.")]
        [Category("Property Changed")]
        public event EventHandler DisplayMemberChanged;

        [Description("Occurs when the property of a control is bound to a data value.")]
        [Category("Property Changed")]
        public event EventHandler Format;

        [Description("Occurs when the value of the FormatInfo property changes.")]
        [Category("Property Changed")]
        public event EventHandler FormatInfoChanged;

        [Description("Occurs when the value of the FormatString property changes.")]
        [Category("Property Changed")]
        public event EventHandler FormatStringChanged;

        [Description("Occurs when the value of the FormattingEnabled property changes.")]
        [Category("Property Changed")]
        public event EventHandler FormattingEnabledChanged;

        [Description("Occurs when the value of the SelectedIndex property changes.")]
        [Category("Behavior")]
        public event EventHandler SelectedIndexChanged;

        [Description("Occurs when the value of the SelectedValue property changes.")]
        [Category("Property Changed")]
        public event EventHandler SelectedValueChanged;

        [Description("Occurs when the value of the ValueMember property changes.")]
        [Category("Property Changed")]
        public event EventHandler ValueMemberChanged;

        #endregion

        #region Properties

        [DefaultValue(true)]
        [Category(Localize.PropertiesCategory.Behavior)]
        public bool AlternateColors
        {
            get
            {
                return _alternateColors;
            }

            set
            {
                _alternateColors = value;
                _listBox.Invalidate();
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
                return _listBox;
            }
        }

        [Category("Data")]
        [Description("Indicates the list that this control will use to gets its items.")]
        [AttributeProvider(typeof(IListSource))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(null)]
        public virtual object DataSource
        {
            get
            {
                return _listBox.DataSource;
            }

            set
            {
                _listBox.DataSource = value;
            }
        }

        [Category("Data")]
        [Description("Indicates the property to display for the items in this control.")]
        [TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DefaultValue("")]
        public virtual string DisplayMember
        {
            get
            {
                return _listBox.DisplayMember;
            }

            set
            {
                _listBox.DisplayMember = value;
            }
        }

        [Category("Behavior")]
        [Description("Controls list box painting. Either the system [NORMAL] or the user [OWNERDRAW] paints each item.")]
        [DefaultValue(typeof(DrawMode), "OwnerDrawVariable")]
        public DrawMode DrawMode
        {
            get
            {
                return _listBox.DrawMode;
            }

            set
            {
                _listBox.DrawMode = value;
            }
        }

        /// <summary>Gets or sets the format specifier characters that indicate how a value is to be displayed.</summary>
        [Description("The format specifier characters that indicate how a value is to be displayed.")]
        [Editor("System.Windows.Forms.Design.FormatStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [MergableProperty(false)]
        [DefaultValue("")]
        public string FormatString
        {
            get
            {
                return _listBox.FormatString;
            }

            set
            {
                _listBox.FormatString = value;
            }
        }

        /// <summary>
        ///     Gets or sets if this property is true, the value of FormatString is used to convert the value of DisplayMember
        ///     into a value that can be displayed.
        /// </summary>
        [Description("If this property is true, the value of FormatString is used to convert the value of DisplayMember into a value that can be displayed.")]
        [DefaultValue(false)]
        public bool FormattingEnabled
        {
            get
            {
                return _listBox.FormattingEnabled;
            }

            set
            {
                _listBox.FormattingEnabled = value;
                OnFormattingEnabledChanged(EventArgs.Empty);
            }
        }

        /// <summary>Gets or sets the width by which the horizontal scroll bar of a VisualListBox can scroll.</summary>
        [Category("Behavior")]
        [Description("The width, in pixels, by which a list box can be scrolled horizontally. Only valid HorizontalScrollbar is true.")]
        [Localizable(true)]
        [DefaultValue(0)]
        public virtual int HorizontalExtent
        {
            get
            {
                return _listBox.HorizontalExtent;
            }

            set
            {
                _listBox.HorizontalExtent = value;
            }
        }

        /// <summary>Gets or sets a value indicating whether a horizontal scroll bar is displayed in the control.</summary>
        [Category("Behavior")]
        [Description("Indicates whether the VisualListBox will display a horizontal scrollbar for items beyond the right edge of the VisualListBox.")]
        [Localizable(true)]
        [DefaultValue(false)]
        public virtual bool HorizontalScrollBar
        {
            get
            {
                return _listBox.HorizontalScrollbar;
            }

            set
            {
                _listBox.HorizontalScrollbar = value;
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color ItemAlternate
        {
            get
            {
                return _itemAlternate;
            }

            set
            {
                _itemAlternate = value;
                _listBox.Invalidate();
            }
        }

        [Category("Behavior")]
        [Description("The height, in pixels, of items in a fixed-height owner-draw list box.")]
        [DefaultValue(typeof(int), "13")]
        public int ItemHeight
        {
            get
            {
                return _listBox.ItemHeight;
            }

            set
            {
                _listBox.ItemHeight = value;
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color ItemNormal
        {
            get
            {
                return _itemNormal;
            }

            set
            {
                _itemNormal = value;
                _listBox.Invalidate();
            }
        }

        [Category("Data")]
        [Description("The items in the VisualListBox.")]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [MergableProperty(false)]
        [Localizable(true)]
        public virtual ListBox.ObjectCollection Items
        {
            get
            {
                return _listBox.Items;
            }
        }

        [Category(Localize.PropertiesCategory.Appearance)]
        [Description(Localize.Description.Common.Color)]
        public Color ItemSelected
        {
            get
            {
                return _itemSelected;
            }

            set
            {
                _itemSelected = value;
                _listBox.Invalidate();
            }
        }

        [Category("Behavior")]
        [Description("Indicates if the list box should always have a scroll bar present, regardless of how many items are present.")]
        [Localizable(true)]
        [DefaultValue(false)]
        public virtual bool ScrollAlwaysVisible
        {
            get
            {
                return _listBox.ScrollAlwaysVisible;
            }

            set
            {
                _listBox.ScrollAlwaysVisible = value;
            }
        }

        [Bindable(true)]
        [Browsable(false)]
        [Description("Gets or sets the zero-based index of the currently selected item in a VisualListBox.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get
            {
                return _listBox.SelectedIndex;
            }

            set
            {
                _listBox.SelectedIndex = value;
            }
        }

        [Browsable(false)]
        [Description("Gets a collection that contains the zero-based indexes of all currently selected items in the VisualListBox.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ListBox.SelectedIndexCollection SelectedIndices
        {
            get
            {
                return _listBox.SelectedIndices;
            }
        }

        [Bindable(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedItem
        {
            get
            {
                return _listBox.SelectedItem;
            }

            set
            {
                _listBox.SelectedItem = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ListBox.SelectedObjectCollection SelectedItems
        {
            get
            {
                return _listBox.SelectedItems;
            }
        }

        [Category("Data")]
        [Bindable(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(null)]
        public object SelectedValue
        {
            get
            {
                return _listBox.SelectedValue;
            }

            set
            {
                _listBox.SelectedValue = value;
            }
        }

        [Category("Behavior")]
        [Description("Indicates if the list box is to be single-select, multi-select or not selectable.")]
        [DefaultValue(typeof(SelectionMode), "One")]
        public virtual SelectionMode SelectionMode
        {
            get
            {
                return _listBox.SelectionMode;
            }

            set
            {
                _listBox.SelectionMode = value;
            }
        }

        [Category("Behavior")]
        [Description("Controls whether the list is sorted.")]
        [DefaultValue(false)]
        public virtual bool Sorted
        {
            get
            {
                return _listBox.Sorted;
            }

            set
            {
                _listBox.Sorted = value;
            }
        }

        [Browsable(false)]
        [Description("Gets or sets the index of the first visible item in the VisualListBox.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TopIndex
        {
            get
            {
                return _listBox.TopIndex;
            }

            set
            {
                _listBox.TopIndex = value;
            }
        }

        [Category("Data")]
        [Description("Indicates the property to use as the actual value of the items in the control.")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DefaultValue("")]
        public virtual string ValueMember
        {
            get
            {
                return _listBox.ValueMember;
            }

            set
            {
                _listBox.ValueMember = value;
            }
        }

        #endregion

        #region Events

        /// <summary>
        ///     Maintains performance while items are added to the ListBox one at a time by preventing the control from
        ///     drawing until the EndUpdate method is called.
        /// </summary>
        public void BeginUpdate()
        {
            _listBox.BeginUpdate();
        }

        /// <summary>Un-select all items in the VisualListBox.</summary>
        public void ClearSelected()
        {
            _listBox.ClearSelected();
        }

        /// <summary>Resumes painting the ListBox control after painting is suspended by the BeginUpdate method.</summary>
        public void EndUpdate()
        {
            _listBox.EndUpdate();
        }

        /// <summary>Finds the first item in the list box that starts with the specified string.</summary>
        /// <param name="str">The String to search for.</param>
        /// <returns>The zero-based index of the first item found; returns -1 if no match is found.</returns>
        public int FindString(string str)
        {
            return _listBox.FindString(str);
        }

        /// <summary>
        ///     Finds the first item after the given index which starts with the given string. The search is not case
        ///     sensitive.
        /// </summary>
        /// <param name="str">The String to search for.</param>
        /// <param name="startIndex">
        ///     The zero-based index of the item before the first item to be searched. Set to -1 to search
        ///     from the beginning of the control.
        /// </param>
        /// <returns>
        ///     The zero-based index of the first item found; returns -1 if no match is found, or 0 if the s parameter
        ///     specifies Empty.
        /// </returns>
        public int FindString(string str, int startIndex)
        {
            return _listBox.FindString(str, startIndex);
        }

        /// <summary>Finds the first item in the list box that matches the specified string.</summary>
        /// <param name="str">The String to search for.</param>
        /// <returns>The zero-based index of the first item found; returns -1 if no match is found.</returns>
        public int FindStringExact(string str)
        {
            return _listBox.FindStringExact(str);
        }

        /// <summary>
        ///     Finds the first item after the specified index that matches the specified string.
        /// </summary>
        /// <param name="str">The String to search for.</param>
        /// <param name="startIndex">
        ///     The zero-based index of the item before the first item to be searched. Set to -1 to search
        ///     from the beginning of the control.
        /// </param>
        /// <returns>
        ///     The zero-based index of the first item found; returns -1 if no match is found, or 0 if the s parameter
        ///     specifies Empty.
        /// </returns>
        public int FindStringExact(string str, int startIndex)
        {
            return _listBox.FindStringExact(str, startIndex);
        }

        /// <summary>Returns the height of an item in the VisualListBox.</summary>
        /// <param name="index">The index of the item to return the height of.</param>
        /// <returns>The height, in pixels, of the item at the specified index.</returns>
        public int GetItemHeight(int index)
        {
            return _listBox.GetItemHeight(index);
        }

        /// <summary>Returns the bounding rectangle for an item in the VisualListBox.</summary>
        /// <param name="index">The zero-based index of item whose bounding rectangle you want to return.</param>
        /// <returns>A Rectangle that represents the bounding rectangle for the specified item.</returns>
        public Rectangle GetItemRectangle(int index)
        {
            return _listBox.GetItemRectangle(index);
        }

        /// <summary>Returns the text representation of the specified item.</summary>
        /// <param name="item">The object from which to get the contents to display.</param>
        /// <returns>
        ///     If the DisplayMember property is not specified, the value returned by GetItemText is the value of the item's
        ///     ToString method. Otherwise, the method returns the string value of the member specified in the DisplayMember
        ///     property for the object specified in the item parameter.
        /// </returns>
        public string GetItemText(object item)
        {
            return _listBox.GetItemText(item);
        }

        /// <summary>
        ///     Returns a value indicating whether the specified item is selected.
        /// </summary>
        /// <param name="index">The zero-based index of the item that determines whether it is selected.</param>
        /// <returns>true if the specified item is currently selected in the VisualListBox; otherwise, false.</returns>
        public bool GetSelected(int index)
        {
            return _listBox.GetSelected(index);
        }

        /// <summary>Returns the zero-based index of the item at the specified coordinates.</summary>
        /// <param name="p">A Point object containing the coordinates used to obtain the item index.</param>
        /// <returns>
        ///     The zero-based index of the item found at the specified coordinates; returns ListBox.NoMatches if no match is
        ///     found.
        /// </returns>
        public int IndexFromPoint(Point p)
        {
            return _listBox.IndexFromPoint(p);
        }

        /// <summary>Returns the zero-based index of the item at the specified coordinates.</summary>
        /// <param name="x">The x-coordinate of the location to search.</param>
        /// <param name="y">The y-coordinate of the location to search.</param>
        /// <returns>
        ///     The zero-based index of the item found at the specified coordinates; returns ListBox.NoMatches if no match is
        ///     found.
        /// </returns>
        public int IndexFromPoint(int x, int y)
        {
            return _listBox.IndexFromPoint(x, y);
        }

        /// <summary>Selects or clears the selection for the specified item in a VisualListBox.</summary>
        /// <param name="index">The zero-based index of the item in a VisualListBox to select or clear the selection for.</param>
        /// <param name="value">true to select the specified item; otherwise, false.</param>
        public void SetSelected(int index, bool value)
        {
            _listBox.SetSelected(index, value);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Invalidate();
        }

        protected virtual void OnDataSourceChanged(EventArgs e)
        {
            DataSourceChanged?.Invoke(this, e);
        }

        protected virtual void OnDisplayMemberChanged(EventArgs e)
        {
            DisplayMemberChanged?.Invoke(this, e);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            MouseState = MouseStates.Hover;
            Invalidate();
        }

        protected virtual void OnFormat(ListControlConvertEventArgs e)
        {
            Format?.Invoke(this, e);
        }

        protected virtual void OnFormatInfoChanged(EventArgs e)
        {
            FormatInfoChanged?.Invoke(this, e);
        }

        protected virtual void OnFormatStringChanged(EventArgs e)
        {
            FormatStringChanged?.Invoke(this, e);
        }

        protected virtual void OnFormattingEnabledChanged(EventArgs e)
        {
            FormattingEnabledChanged?.Invoke(this, e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            MouseState = MouseStates.Hover;
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            MouseState = MouseStates.Normal;
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            MouseState = MouseStates.Normal;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;

            ControlGraphicsPath = Border.GetBorderShape(ClientRectangle, ControlBorder);

            graphics.SetClip(ControlGraphicsPath);

            if (Background != _listBox.BackColor)
            {
                _listBox.BackColor = Background;
            }

            graphics.FillRectangle(new SolidBrush(Background), ClientRectangle);

            graphics.ResetClip();

            Border.DrawBorderStyle(graphics, ControlBorder, MouseState, ControlGraphicsPath);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _listBox.Location = GetInternalControlLocation(ControlBorder);
            _listBox.Size = GetInternalControlSize(Size, ControlBorder);
        }

        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            SelectedIndexChanged?.Invoke(this, e);
        }

        protected virtual void OnSelectedValueChanged(EventArgs e)
        {
            SelectedValueChanged?.Invoke(this, e);
        }

        protected virtual void OnValueMemberChanged(EventArgs e)
        {
            ValueMemberChanged?.Invoke(this, e);
        }

        private void ListBox_DataSourceChanged(object sender, EventArgs e)
        {
            OnDataSourceChanged(e);
        }

        private void ListBox_DisplayMemberChanged(object sender, EventArgs e)
        {
            OnDisplayMemberChanged(e);
        }

        private void ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            // We cannot do anything with an invalid index
            if (e.Index < 0)
            {
                return;
            }

            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint;

            // Draw the background of the ListBox control for each item.
            e.DrawBackground();

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            if (e.Index > -1)
            {
                Color color;

                if (_alternateColors)
                {
                    if (isSelected)
                    {
                        color = _itemSelected;
                    }
                    else
                    {
                        if (e.Index % 2 == 0)
                        {
                            color = _itemNormal;
                        }
                        else
                        {
                            color = _itemAlternate;
                        }
                    }
                }
                else
                {
                    if (isSelected)
                    {
                        color = _itemSelected;
                    }
                    else
                    {
                        color = _itemNormal;
                    }
                }

                // Background item brush
                SolidBrush backgroundBrush = new SolidBrush(color);

                // Text color brush
                SolidBrush textBrush = new SolidBrush(e.ForeColor);

                // Draw the background
                e.Graphics.FillRectangle(backgroundBrush, e.Bounds);

                // Draw the text
                e.Graphics.DrawString(_listBox.GetItemText(_listBox.Items[e.Index].ToString()), e.Font, new SolidBrush(ForeColor), new RectangleF(e.Bounds.Location, e.Bounds.Size), StringFormat.GenericDefault);

                // Clean up
                backgroundBrush.Dispose();
                textBrush.Dispose();
            }

            // Update our content object with values from the list item
            UpdateContentFromItemIndex(e.Index);
        }

        private void ListBox_Format(object sender, ListControlConvertEventArgs e)
        {
            OnFormat(e);
        }

        private void ListBox_FormatInfoChanged(object sender, EventArgs e)
        {
            OnFormatInfoChanged(e);
        }

        private void ListBox_FormatStringChanged(object sender, EventArgs e)
        {
            OnFormatStringChanged(e);
        }

        private void ListBox_GotFocus(object sender, EventArgs e)
        {
            _listBox.Invalidate();
            OnGotFocus(e);
        }

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            _listBox.Invalidate();
            OnKeyDown(e);
        }

        private void ListBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnKeyPress(e);
        }

        private void ListBox_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        private void ListBox_LostFocus(object sender, EventArgs e)
        {
            _listBox.Invalidate();
            OnLostFocus(e);
        }

        private void ListBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (_listBox.DrawMode == DrawMode.OwnerDrawFixed)
            {
                e.ItemHeight = ItemHeight;
            }
            else
            {
                e.ItemHeight = GDI.MeasureText(e.Graphics, Items[e.Index].ToString(), Font).Height;
            }
        }

        private void ListBox_MouseDown(object sender, EventArgs e)
        {
            _listBox.Invalidate();
        }

        private void ListBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            OnPreviewKeyDown(e);
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedIndexChanged(e);
        }

        private void ListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            OnSelectedValueChanged(e);
        }

        private void ListBox_Validated(object sender, EventArgs e)
        {
            OnValidated(e);
        }

        private void ListBox_Validating(object sender, CancelEventArgs e)
        {
            OnValidating(e);
        }

        private void ListBox_ValueMemberChanged(object sender, EventArgs e)
        {
            OnValueMemberChanged(e);
        }

        private void UpdateContentFromItemIndex(int index)
        {
            IContentValues itemValues = Items[index] as IContentValues;

            // If the object exposes the rich interface then use is...
            if (itemValues != null)
            {
                _contentValues.ShortText = itemValues.GetShortText();
                _contentValues.LongText = itemValues.GetLongText();

                // _contentValues.Image = itemValues.GetImage(PaletteState.Normal);
                // _contentValues.ImageTransparentColor = itemValues.GetImageTransparentColor(PaletteState.Normal);
            }
            else
            {
                // Get the text string for the item
                _contentValues.ShortText = _listBox.GetItemText(Items[index]);
                _contentValues.LongText = null;

                // _contentValues.Image = null;
                // _contentValues.ImageTransparentColor = Color.Empty;
            }
        }

        #endregion
    }
}