using System;
using System.Runtime.InteropServices;

namespace UiTools.Controls.ExtendedDataGridView
{
    internal static class Win32
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SetCursor(IntPtr hCursor);

        internal const int WM_LBUTTONDOWN = 0x0201;
        internal const int WM_SETCURSOR = 0x0020;
        internal const int IDC_HAND = 32649;

        internal static ushort LOWORD(IntPtr lparam)
        {
            return unchecked((ushort)((long)lparam & 0xffff));
        }

        internal static ushort HIWORD(IntPtr lparam)
        {
            return unchecked((ushort)(((long)lparam >> 16) & 0xffff));
        }
    }
}
