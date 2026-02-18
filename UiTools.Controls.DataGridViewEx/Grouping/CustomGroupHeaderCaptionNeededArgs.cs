using System;

namespace UiTools.Controls.ExtendedDataGridView
{
    public class CustomGroupHeaderCaptionNeededArgs : EventArgs
    {
        public CustomGroupHeaderCaptionNeededArgs(CustomRow headerRow)
            : base()
        {
            HeaderRow = headerRow;
        }

        public CustomRow HeaderRow { get; }
        public string Caption { get; set; }
        public bool Handled { get; set; }
    }
}
