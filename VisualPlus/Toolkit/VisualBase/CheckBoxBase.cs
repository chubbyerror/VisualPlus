﻿namespace VisualPlus.Toolkit.VisualBase
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    #endregion

    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public abstract class CheckBoxBase : ToggleButtonBase
    {
        #region Variables

        private CheckState _checkState = CheckState.Unchecked;
        private bool _threeState;

        #endregion

        #region Constructors

        [Category(Localize.EventsCategory.PropertyChanged)]
        [Description(Localize.Description.Checkmark.Checked)]
        public event EventHandler CheckStateChanged;

        #endregion

        #region Properties

        [DefaultValue(false)]
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Checkmark.Checked)]
        public bool Checked
        {
            get
            {
                return Toggle;
            }

            set
            {
                if (Toggle != value)
                {
                    // Store new values
                    Toggle = value;

                    // Generate events
                    OnToggleChanged(EventArgs.Empty);

                    // Repaint
                    Invalidate();
                }
            }
        }

        [DefaultValue(typeof(CheckState), "Unchecked")]
        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Checkmark.Checked)]
        public CheckState CheckState
        {
            get
            {
                return _checkState;
            }

            set
            {
                if (_checkState != value)
                {
                    // Store new values
                    _checkState = value;
                    bool newChecked = _checkState != CheckState.Unchecked;
                    bool checkedChanged = Checked != newChecked;
                    Checked = newChecked;

                    // Generate events
                    if (checkedChanged)
                    {
                        OnToggleChanged(EventArgs.Empty);
                    }

                    OnCheckStateChanged(EventArgs.Empty);

                    // Repaint
                    Invalidate();
                }
            }
        }

        [Category(Localize.PropertiesCategory.Behavior)]
        [Description(Localize.Description.Common.Toggle)]
        [DefaultValue(false)]
        public bool ThreeState
        {
            get
            {
                return _threeState;
            }

            set
            {
                if (_threeState != value)
                {
                    _threeState = value;
                    Invalidate();
                }
            }
        }

        #endregion

        #region Events

        protected virtual void OnCheckStateChanged(EventArgs e)
        {
            CheckStateChanged?.Invoke(this, e);
        }

        protected override void OnClick(EventArgs e)
        {
            switch (CheckState)
            {
                case CheckState.Unchecked:
                    {
                        CheckState = CheckState.Checked;
                        break;
                    }

                case CheckState.Checked:
                    {
                        CheckState = ThreeState ? CheckState.Indeterminate : CheckState.Unchecked;
                        break;
                    }

                case CheckState.Indeterminate:
                    {
                        CheckState = CheckState.Unchecked;
                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
            }

            base.OnClick(e);
        }

        protected override void OnToggleChanged(EventArgs e)
        {
            base.OnToggleChanged(e);
            _checkState = Checked ? CheckState.Checked : CheckState.Unchecked;
            OnCheckStateChanged(EventArgs.Empty);
            Invalidate();
        }

        #endregion
    }
}