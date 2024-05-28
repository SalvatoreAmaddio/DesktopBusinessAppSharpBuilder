using System.Runtime.InteropServices;
using System.Text;

namespace FrontEnd.Dialogs
{
    /// <summary>
    /// This class holds a collection of constant uint. These are flags used by the <see cref="FolderDialog"/> class.
    /// </summary>
    public static class FolderDialogFalgs
    {
        /// <summary>
        /// Only return file system directories. 
        /// If the user selects folders that are not part of the file system, the OK button is grayed.
        /// </summary>
        public const uint BIF_RETURNONLYFSDIRS = 0x00000001;

        /// <summary>
        /// Do not include network folders below the domain level in the dialog box's tree view.
        /// </summary>
        public const uint BIF_DONTGOBELOWDOMAIN = 0x00000002;

        /// <summary>
        /// Include a status area in the dialog box. 
        /// The callback function can set the status text by sending messages to the dialog box.
        /// </summary>
        public const uint BIF_STATUSTEXT = 0x00000004;

        /// <summary>
        /// Only return file system ancestors. An ancestor is a folder that has file system descendants.
        /// </summary>
        public const uint BIF_RETURNFSANCESTORS = 0x00000008;

        /// <summary>
        /// Version 4.71. If the user types an invalid name into the edit box, 
        /// the dialog box calls the application's BrowseCallbackProc with BFFM_VALIDATEFAILED. 
        /// This flag is ignored if <see cref="BIF_NEWDIALOGSTYLE"/> is specified.
        /// </summary>
        public const uint BIF_EDITBOX = 0x00000010;

        /// <summary>
        /// ersion 4.71. If the user types an invalid name into the edit box, 
        /// the dialog box calls the application's BrowseCallbackProc with BFFM_VALIDATEFAILED. 
        /// This flag is ignored if  <see cref="BIF_NEWDIALOGSTYLE"/> is specified.
        /// </summary>
        public const uint BIF_VALIDATE = 0x00000020;

        /// <summary>
        /// Version 4.71. Use the new user interface. Setting this flag provides the user with a larger dialog box that can be resized. This flag is equivalent to <see cref="BIF_USENEWUI"/>.
        /// </summary>
        public const uint BIF_NEWDIALOGSTYLE = 0x00000040;

        /// <summary>
        /// This flag is a combination of two flags: <see cref="BIF_NEWDIALOGSTYLE"/> and <see cref="BIF_EDITBOX"/>.
        /// </summary>
        public const uint BIF_USENEWUI = 0x00000050;

        /// <summary>
        /// HERE
        /// Version 5.0. The browse dialog box will display URLs. The <see cref="BIF_USENEWUI"/> and <see cref="BIF_BROWSEINCLUDEFILES"/> flags must also be set.
        /// </summary>
        public const uint BIF_BROWSEINCLUDEURLS = 0x00000080;

        /// <summary>
        /// Version 6.0. When combined with <see cref="BIF_NEWDIALOGSTYLE"/>, adds a usage hint to the dialog box, in place of the edit box. If the <see cref="BIF_EDITBOX"/> flag is specified, it is ignored.
        /// </summary>
        public const uint BIF_UAHINT = 0x00000100;

        /// <summary>
        /// Version 6.0. Do not include the New Folder button in the browse dialog box.
        /// </summary>
        public const uint BIF_NONEWFOLDERBUTTON = 0x00000200;

        /// <summary>
        /// When the selected folder is a shortcut, return the PIDL of the shortcut itself rather than its target.
        /// </summary>
        public const uint BIF_NOTRANSLATETARGETS = 0x00000400;

        /// <summary>
        /// The browse dialog box will display an edit box for the user to type a name into.This flag is ignored if <see cref="BIF_NEWDIALOGSTYLE"/> is specified.
        /// </summary>
        public const uint BIF_BROWSEFORCOMPUTER = 0x00001000;

        /// <summary>
        /// The browse dialog box will display an edit box for the user to type a name into.This flag is ignored if <see cref="BIF_NEWDIALOGSTYLE"/> is specified.
        /// </summary>
        public const uint BIF_BROWSEFORPRINTER = 0x00002000;

        /// <summary>
        /// The browse dialog box will display files as well as folders.
        /// </summary>
        public const uint BIF_BROWSEINCLUDEFILES = 0x00004000;

        /// <summary>
        /// Version 5.0. The browse dialog box can display shareable resources on remote systems.
        /// </summary>
        public const uint BIF_SHAREABLE = 0x00008000;

        /// <summary>
        /// Version 6.0. The browse dialog box will display file system junctions.
        /// </summary>
        public const uint BIF_BROWSEFILEJUNCTIONS = 0x00010000;
    }

    /// <summary>
    /// This class uses the WIN32 API to instantiate a dialog to select folders.
    /// </summary>
    public class FolderDialog(string title = "Select a folder")
    {
        /// <summary>
        /// Gets and sets the title to be displayed above the tree view control in the dialog box. 
        /// This property can be used to provide instructions to the user.
        /// The default value is "Select a folder".
        /// </summary>
        public string Title { get; set; } = title;

        private const int MAX_PATH = 260;

        /// <summary>
        ///  Opens a folder browser dialog.
        /// </summary>
        /// <param name="bi"></param>
        /// <returns>The Dialog displayed</returns>
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHBrowseForFolder(ref BROWSEINFO bi);

        /// <summary>
        /// Converts the item identifier list to a file system path.
        /// </summary>
        /// <param name="pidl"></param>
        /// <param name="pszPath"></param>
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern bool SHGetPathFromIDList(IntPtr pidl, StringBuilder pszPath);

        /// <summary>
        /// This struct contains parameters for the SHBrowseForFolder function, 
        /// including the dialog's owner window handle, root folder, 
        /// display name buffer, dialog title, flags, callback function pointer, and image index.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct BROWSEINFO
        {
            public IntPtr hwndOwner;
            public IntPtr pidlRoot;
            public IntPtr pszDisplayName;
            public string lpszTitle;
            public uint ulFlags;
            public IntPtr lpfn;
            public IntPtr lParam;
            public int iImage;
        }

        private IntPtr _handle;
        public string SelectedPath { get; private set; } = string.Empty;

        /// <summary>
        /// Creates and initializes a <see cref="BROWSEINFO"/> struct
        /// which calls <see cref="SHBrowseForFolder"/> to display the dialog.
        /// If a folder is selected, <see cref="SHGetPathFromIDList"/> retrieves the path, and <see cref="SelectedPath"/> is set.
        /// <para/>
        /// By default, the <paramref name="flags"/> argument is set to <see cref="FolderDialogFalgs.BIF_RETURNONLYFSDIRS"/> | <see cref="FolderDialogFalgs.BIF_NEWDIALOGSTYLE"/> | <see cref="FolderDialogFalgs.BIF_NONEWFOLDERBUTTON"/>
        /// </summary>
        /// <param name="flags"> A uint value or a combiniation of both.</param>
        /// <returns>true if a path is selected.</returns>
        public bool ShowDialog(uint flags = FolderDialogFalgs.BIF_RETURNONLYFSDIRS | FolderDialogFalgs.BIF_NEWDIALOGSTYLE | FolderDialogFalgs.BIF_NONEWFOLDERBUTTON)
        {
            var bi = new BROWSEINFO
            {
                hwndOwner = IntPtr.Zero, //Handle to the owner window for the dialog box. IntPtr.Zero means that the dialog box has no owner window.
                pidlRoot = IntPtr.Zero, //Pointer to an item identifier list specifying the location of the root folder from which to start browsing. IntPtr.Zero refers to the desktop.
                pszDisplayName = Marshal.AllocHGlobal(MAX_PATH), //Address of a buffer to receive the display name of the folder selected by the user.
                lpszTitle = this.Title, //Pointer to a null-terminated string that is displayed above the tree view control in the dialog box. This string can be used to provide instructions to the user.
                ulFlags = flags, //Controls the behavior of the dialog box by using flags that can be combined using the bitwise OR operator.
                lpfn = IntPtr.Zero, //Address of an application-defined function that the dialog box calls when an event occurs.
                lParam = IntPtr.Zero, //Application-defined value that the dialog box passes to the callback function (lpfn).
                iImage = 0 // Variable that receives the index of the image associated with the selected folder.
            };

            _handle = SHBrowseForFolder(ref bi);

            if (_handle != IntPtr.Zero)
            {
                StringBuilder path = new(MAX_PATH);
                if (SHGetPathFromIDList(_handle, path))
                {
                    SelectedPath = path.ToString();
                    Marshal.FreeHGlobal(bi.pszDisplayName);
                    return true;
                }
            }

            Marshal.FreeHGlobal(bi.pszDisplayName);
            return false;
        }

    }
}