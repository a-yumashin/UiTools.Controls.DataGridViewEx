using System;
using System.Drawing;
using System.Windows.Forms;

namespace UiTools.Controls.ExtendedDataGridView
{
    public class ColumnFooterFormattingArgs : EventArgs
    {
        public ColumnFooterFormattingArgs(CustomRow groupFooterRow, DataGridViewColumn column)
            : base()
        {
            GroupFooterRow = groupFooterRow;
            Column = column;
        }

        public CustomRow GroupFooterRow { get; }
        public DataGridViewColumn Column { get; }
        public Color ColumnFooterBackColor { get; set; }
        public Color ColumnFooterSelectionBackColor { get; set; }
        public Color ColumnFooterForeColor { get; set; }
        public Color ColumnFooterSelectionForeColor { get; set; }
        public bool Handled { get; set; }
    }
}
