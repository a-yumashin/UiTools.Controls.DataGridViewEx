using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static UiTools.Controls.ExtendedDataGridView.CommonStuff;

namespace UiTools.Controls.ExtendedDataGridView
{
    public static class GridExtensions
    {
        /*
         * NOTE: although DataGridViewEx inherits DataGridView, I prefer to use extension methods - just because I can name
         *       them as Rows(...). Should I create such methods right inside DataGridViewEx - I would be obliged to name
         *       them as GetRows(...) or RowsEx(...) or whatever (because property and method of the same class cannot have
         *       the same name). Rows(...) is simply shorter. Same for Columns(...).
         */
        public static List<DataGridViewColumn> Columns(this DataGridView dgv)
        {
            return dgv.Columns
                .Cast<DataGridViewColumn>()
                .ToList();
        }

        public static List<DataGridViewColumn> Columns(this DataGridView dgv, Func<DataGridViewColumn, bool> predicate)
        {
            return dgv.Columns
                .Cast<DataGridViewColumn>()
                .Where(predicate)
                .ToList();
        }

        //public static List<CustomRow> Rows(this DataGridView dgv)
        //{
        //    return dgv.Rows
        //        .Cast<CustomRow>()
        //        .ToList();
        //}

        public static List<CustomRow> Rows(this DataGridView dgv, Func<CustomRow, bool> predicate)
        {
            return dgv.Rows
                .Cast<CustomRow>()
                .Where(predicate)
                .ToList();
        }

        public static object GetValueSafe(this DataGridViewCell cell, object emptyValueReplacement)
        {
            return cell.Value == DBNull.Value || cell.Value == null || cell.Value.ToString() == ""
                ? emptyValueReplacement
                : cell.Value;
        }

        public static string GetFormattedValueSafe(this DataGridViewCell cell, string emptyValueReplacement = "")
        {
            return cell.Value == DBNull.Value || cell.Value == null || cell.Value.ToString() == ""
                ? emptyValueReplacement
                : cell.FormattedValue.ToString();
        }

        public static IEnumerable<CellContents> GetDistinctValues(this DataGridViewColumn col)
        {
            var values = new List<CellContents>();
            foreach (DataGridViewRow dr in col.DataGridView.Rows(dr => !dr.IsNewRow && dr.IsRegularRow))
            {
                var value = dr.Cells[col.Index].Value;
                value = value == null || value == DBNull.Value || value.ToString().Length == 0
                    ? null
                    : value;
                values.Add(new CellContents(
                    value,
                    value == null ? SR("(empty value)") : dr.Cells[col.Index].FormattedValue));
            }
            var distinctValues = values.Distinct();
            #region NOT USED
            /* NOTE: This helped to fix System.ArgumentException "Object must be of type String" at System.String.CompareTo(Object value)
             *       which was raised when ColumnFilterControl.PrepareForShow() tried to call .ToArray() on the return value of this method.
             *       However, adding " && dr.IsRegularRow" (to remove footer values from the list) also fixed this exception, so it seems
             *       that System.Linq.Enumerable.OrderBy() treated footer values like "Count: 15" AS NUMBERS, not like strings (strange!!!).
             *       
            var numericsPresent = distinctValues.Any(p => p.Value is int || p.Value is decimal);
            var nonNumericsPresent = distinctValues.Any(p => p.Value != null && !(p.Value is int || p.Value is decimal));
            if (nonNumericsPresent && numericsPresent)
                return distinctValues.OrderBy(p => p.Value?.ToString());
             */
            #endregion NOT USED
            return distinctValues.OrderBy(p => p.Value);
        }

        public static GridColumnExtInfo ExtInfo(this DataGridViewColumn col)
        {
            if (col.Tag == null)
                col.Tag = new GridColumnExtInfo(col);
            return col.Tag as GridColumnExtInfo;
        }
    }
}
