using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UiTools.Controls.ExtendedDataGridView
{
    /// <summary>
    /// This custom comparer is needed to ensure that footer row (if any)
    /// keeps to be the last (bottom) row in the group (when sorting is made).
    /// </summary>
    internal class CellComparer : IComparer<DataGridViewCell>
    {
        private readonly SortOrder sortOrder;

        public CellComparer(SortOrder sortOrder)
        {
            this.sortOrder = sortOrder;
        }

        // Compares two objects. An implementation of this method must return a
        // value less than zero if x is less than y, zero if x is equal to y, or a
        // value greater than zero if x is greater than y.
        public int Compare(DataGridViewCell x, DataGridViewCell y)
        {
            if ((x.OwningRow as CustomRow).IsFooterRow)
            {
                return sortOrder == SortOrder.Ascending ? 1 : -1;
            }
            if ((y.OwningRow as CustomRow).IsFooterRow)
            {
                return sortOrder == SortOrder.Ascending ? -1 : 1;
            }
            // Perform default compare:
            var vX = x.Value;
            var vY = y.Value;
            if (vX != null && vX.ToString() == string.Empty)
                if (y.OwningColumn.ExtInfo().IsNumeric)
                    vX = int.MinValue;
                else if (y.OwningColumn.ExtInfo().DataType == GridColumnDataType.DateTime)
                    vX = DateTime.MinValue;
            if (vY != null && vY.ToString() == string.Empty)
                if (x.OwningColumn.ExtInfo().IsNumeric)
                    vY = int.MinValue;
                else if (x.OwningColumn.ExtInfo().DataType == GridColumnDataType.DateTime)
                    vY = DateTime.MinValue;
            return System.Collections.Comparer.Default.Compare(vX, vY);
            //return System.Collections.Comparer.Default.Compare(x.Value, y.Value);
        }
    }
}
