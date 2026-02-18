using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UiTools.Controls.ExtendedDataGridView.Properties;

namespace UiTools.Controls.ExtendedDataGridView
{
    internal partial class SandwichMenu : PictureBox
    {
        public event EventHandler BeforeShow;
        private readonly ContextMenuStrip ctxMenu = new ContextMenuStrip();

        private const int WM_SETCURSOR = 0x0020;
        private const int IDC_HAND = 32649;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetCursor(IntPtr hCursor);

        public ContextMenuStrip CtxMenu { get => ctxMenu; }

        public SandwichMenu()
        {
            InitializeComponent();
            Size = Resources.GridMenu.Size;
            Image = Resources.GridMenu;
            SizeMode = PictureBoxSizeMode.StretchImage;
            BackColor = Color.Transparent;
            ctxMenu.ShowImageMargin = false;
        }

        public void Close()
        {
            ctxMenu.Close();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SETCURSOR)
            {
                // Set the cursor to use the system hand cursor
                SetCursor(LoadCursor(IntPtr.Zero, IDC_HAND));
                // Indicate that the message has been handled
                m.Result = IntPtr.Zero;
                return;
            }
            base.WndProc(ref m);
        }

        protected override void OnClick(EventArgs e)
        {
            BeforeShow?.Invoke(this, e);
            ctxMenu.Show(Parent, Left, Top + Height);
            base.OnClick(e);
        }
    }
}
