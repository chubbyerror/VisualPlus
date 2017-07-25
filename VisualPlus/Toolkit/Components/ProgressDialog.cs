namespace VisualPlus.Toolkit.Components
{
    #region Namespace

    using System;
    using System.Runtime.InteropServices;

    #endregion

    public class ProgressDialog
    {
        #region Variables

        private readonly IntPtr _parentHandle;

        private string _cancelMessage = string.Empty;

        private string _line1 = string.Empty;

        private string _line2 = string.Empty;

        private string _line3 = string.Empty;

        private uint _maximum = 100;

        private IProgressDialog _progressDialog;

        private string _title = string.Empty;

        private uint _value;

        #endregion

        #region Constructors

        public ProgressDialog(IntPtr parentHandle)
        {
            _parentHandle = parentHandle;
        }

        /// <summary>Flags that indicate the action to be taken by the ProgressDialog.SetTime() method.</summary>
        public enum ProgressDialogAction : uint
        {
            /// <summary>Resets the timer to zero. Progress will be calculated from the time this method is called.</summary>
            Reset = 0x01,

            /// <summary>Progress has been suspended.</summary>
            Pause = 0x02,

            /// <summary>Progress has been resumed.</summary>
            Resume = 0x03
        }

        [Flags]
        public enum ProgressDialogBehavior : uint
        {
            /// <summary>Normal progress dialog box behavior.</summary>
            Normal = 0x00000000,

            /// <summary>
            ///     The progress dialog box will be modal to the window specified by hwndParent. By default, a progress dialog box
            ///     is modeless.
            /// </summary>
            Modal = 0x00000001,

            /// <summary>Automatically estimate the remaining time and display the estimate on line 3. </summary>
            /// <remarks>If this flag is set, IProgressDialog::SetLine can be used only to display text on lines 1 and 2.</remarks>
            AutoTime = 0x00000002,

            /// <summary>Do not show the "time remaining" text.</summary>
            NoTime = 0x00000004,

            /// <summary>Do not display a minimize button on the dialog box's caption bar.</summary>
            NoMinimize = 0x00000008,

            /// <summary>Do not display a progress bar.</summary>
            /// <remarks>
            ///     Typically, an application can quantitatively determine how much of the operation remains and periodically pass
            ///     that value to IProgressDialog::SetProgress. The progress dialog box uses this information to update its progress
            ///     bar. This flag is typically set when the calling application must wait for an operation to finish, but does not
            ///     have any quantitative information it can use to update the dialog box.
            /// </remarks>
            NoProgressBar = 0x00000010
        }

        #endregion

        #region Properties

        public string CancelMessage
        {
            get
            {
                return _cancelMessage;
            }

            set
            {
                _cancelMessage = value;
                _progressDialog?.SetCancelMsg(_cancelMessage, null);
            }
        }

        public bool HasUserCanceled => (_progressDialog != null) && _progressDialog.HasUserCanceled();

        public string Line1
        {
            get
            {
                return _line1;
            }

            set
            {
                _line1 = value;
                _progressDialog?.SetLine(1, _line1, false, IntPtr.Zero);
            }
        }

        public string Line2
        {
            get
            {
                return _line2;
            }

            set
            {
                _line2 = value;
                _progressDialog?.SetLine(2, _line2, false, IntPtr.Zero);
            }
        }

        public string Line3
        {
            get
            {
                return _line3;
            }

            set
            {
                _line3 = value;
                _progressDialog?.SetLine(3, _line3, false, IntPtr.Zero);
            }
        }

        public uint Maximum
        {
            get
            {
                return _maximum;
            }

            set
            {
                _maximum = value;
                _progressDialog?.SetProgress(_value, _maximum);
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
                _progressDialog?.SetTitle(_title);
            }
        }

        public uint Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
                _progressDialog?.SetProgress(_value, _maximum);
            }
        }

        #endregion

        #region Events

        public void CloseDialog()
        {
            if (_progressDialog == null)
            {
                return;
            }

            _progressDialog.StopProgressDialog();

            // Marshal.ReleaseComObject(pd);
            _progressDialog = null;
        }

        public void ShowDialog(params ProgressDialogBehavior[] flags)
        {
            if (_progressDialog != null)
            {
                return;
            }

            _progressDialog = new Win32ProgressDialog() as IProgressDialog;

            _progressDialog.SetTitle(_title);
            _progressDialog.SetCancelMsg(_cancelMessage, null);
            _progressDialog.SetLine(1, _line1, false, IntPtr.Zero);
            _progressDialog.SetLine(2, _line2, false, IntPtr.Zero);
            _progressDialog.SetLine(3, _line3, false, IntPtr.Zero);

            ProgressDialogBehavior dialogFlags = ProgressDialogBehavior.Normal;
            if (flags.Length != 0)
            {
                dialogFlags = flags[0];
                for (var i = 1; i < flags.Length; i++)
                {
                    dialogFlags = dialogFlags | flags[i];
                }
            }

            _progressDialog.StartProgressDialog(_parentHandle, null, dialogFlags, IntPtr.Zero);
        }

        #endregion

        #region Methods

        [ComImport]
        [Guid("F8383852-FCD3-11d1-A6B9-006097DF5BD4")]
        public class Win32ProgressDialog
        {
        }

        [ComImport]
        [Guid("EBBC7C04-315E-11d2-B62F-006097DF5BD4")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IProgressDialog
        {
            /// <summary>Starts the progress dialog box.</summary>
            /// <param name="hwndParent">A handle to the dialog box's parent window.</param>
            /// <param name="punkEnableModless">Reserved. Set to null.</param>
            /// <param name="dwFlags">Flags that control the operation of the progress dialog box. </param>
            /// <param name="pvReserved">Reserved. Set to IntPtr.Zero</param>
            void StartProgressDialog(IntPtr hwndParent, [MarshalAs(UnmanagedType.IUnknown)] object punkEnableModless, ProgressDialogBehavior dwFlags, IntPtr pvReserved);

            /// <summary>Stops the progress dialog box and removes it from the screen.</summary>
            void StopProgressDialog();

            /// <summary>Sets the title of the progress dialog box.</summary>
            /// <param name="pwzTitle">A pointer to a null-terminated Unicode string that contains the dialog box title.</param>
            void SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pwzTitle);

            /// <summary>
            ///     Specifies an Audio-Video Interleaved (AVI) clip that runs in the dialog box. Note: Note  This method is not
            ///     supported in Windows Vista or later versions.
            /// </summary>
            /// <param name="hInstAnimation">An instance handle to the module from which the AVI resource should be loaded.</param>
            /// <param name="idAnimation">
            ///     An AVI resource identifier. To create this value, use the MAKEINTRESOURCE macro. The control
            ///     loads the AVI resource from the module specified by hInstAnimation.
            /// </param>
            void SetAnimation(IntPtr hInstAnimation, ushort idAnimation);

            /// <summary>Checks whether the user has canceled the operation.</summary>
            /// <returns>TRUE if the user has cancelled the operation; otherwise, FALSE.</returns>
            /// <remarks>
            ///     The system does not send a message to the application when the user clicks the Cancel button.
            ///     You must periodically use this function to poll the progress dialog box object to determine
            ///     whether the operation has been canceled.
            /// </remarks>
            [PreserveSig]
            [return: MarshalAs(UnmanagedType.Bool)]
            bool HasUserCanceled();

            /// <summary>Updates the progress dialog box with the current state of the operation.</summary>
            /// <param name="dwCompleted">
            ///     An application-defined value that indicates what proportion of the operation has been
            ///     completed at the time the method was called.
            /// </param>
            /// <param name="dwTotal">
            ///     An application-defined value that specifies what value dwCompleted will have when the operation
            ///     is complete.
            /// </param>
            void SetProgress(uint dwCompleted, uint dwTotal);

            /// <summary>Updates the progress dialog box with the current state of the operation.</summary>
            /// <param name="ullCompleted">
            ///     An application-defined value that indicates what proportion of the operation has been
            ///     completed at the time the method was called.
            /// </param>
            /// <param name="ullTotal">
            ///     An application-defined value that specifies what value ullCompleted will have when the operation
            ///     is complete.
            /// </param>
            void SetProgress64(ulong ullCompleted, ulong ullTotal);

            /// <summary>Displays a message in the progress dialog.</summary>
            /// <param name="dwLineNum">
            ///     The line number on which the text is to be displayed. Currently there are three lines—1, 2, and
            ///     3. If the PROGDLG_AUTOTIME flag was included in the dwFlags parameter when IProgressDialog::StartProgressDialog was
            ///     called, only lines 1 and 2 can be used. The estimated time will be displayed on line 3.
            /// </param>
            /// <param name="pwzString">A null-terminated Unicode string that contains the text.</param>
            /// <param name="fCompactPath">
            ///     TRUE to have path strings compacted if they are too large to fit on a line. The paths are
            ///     compacted with PathCompactPath.
            /// </param>
            /// <param name="pvReserved"> Reserved. Set to IntPtr.Zero.</param>
            /// <remarks>
            ///     This function is typically used to display a message such as "Item XXX is now being processed." typically,
            ///     messages are displayed on lines 1 and 2, with line 3 reserved for the estimated time.
            /// </remarks>
            void SetLine(uint dwLineNum, [MarshalAs(UnmanagedType.LPWStr)] string pwzString, [MarshalAs(UnmanagedType.VariantBool)] bool fCompactPath, IntPtr pvReserved);

            /// <summary>Sets a message to be displayed if the user cancels the operation.</summary>
            /// <param name="pwzCancelMsg">A pointer to a null-terminated Unicode string that contains the message to be displayed.</param>
            /// <param name="pvReserved">Reserved. Set to NULL.</param>
            /// <remarks>
            ///     Even though the user clicks Cancel, the application cannot immediately call
            ///     IProgressDialog::StopProgressDialog to close the dialog box. The application must wait until the
            ///     next time it calls IProgressDialog::HasUserCancelled to discover that the user has canceled the
            ///     operation. Since this delay might be significant, the progress dialog box provides the user with
            ///     immediate feedback by clearing text lines 1 and 2 and displaying the cancel message on line 3.
            ///     The message is intended to let the user know that the delay is normal and that the progress dialog
            ///     box will be closed shortly.
            ///     It is typically is set to something like "Please wait while ...".
            /// </remarks>
            void SetCancelMsg([MarshalAs(UnmanagedType.LPWStr)] string pwzCancelMsg, object pvReserved);

            /// <summary>Resets the progress dialog box timer to zero.</summary>
            /// <param name="dwDialogAction">Flags that indicate the action to be taken by the timer.</param>
            /// <param name="pvReserved">Reserved. Set to NULL.</param>
            /// <remarks>
            ///     The timer is used to estimate the remaining time. It is started when your application
            ///     calls IProgressDialog::StartProgressDialog. Unless your application will start immediately,
            ///     it should call Timer just before starting the operation.
            ///     This practice ensures that the time estimates will be as accurate as possible. This method
            ///     should not be called after the first call to IProgressDialog::SetProgress.
            /// </remarks>
            void Timer(ProgressDialogAction dwDialogAction, object pvReserved);
        }

        #endregion
    }
}