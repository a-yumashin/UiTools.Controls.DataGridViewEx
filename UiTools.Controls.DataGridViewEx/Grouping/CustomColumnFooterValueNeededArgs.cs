using System;
using System.Windows.Forms;

namespace UiTools.Controls.ExtendedDataGridView
{
    public class CustomColumnFooterValueNeededArgs : EventArgs
    {
        public CustomColumnFooterValueNeededArgs(CustomRow groupFooterRow, DataGridViewColumn column)
            : base()
        {
            GroupFooterRow = groupFooterRow;
            Column = column;
        }

        public CustomRow GroupFooterRow { get; }
        public DataGridViewColumn Column { get; }
        public object ColumnFooterValue { get; set; }
        public string ColumnFooterHint { get; set; }
        public bool Handled { get; set; }
    }
}
