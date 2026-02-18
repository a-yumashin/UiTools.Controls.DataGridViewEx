using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static UiTools.Controls.ExtendedDataGridView.CommonStuff;

namespace UiTools.Controls.ExtendedDataGridView
{
    internal class GridFilterHelper
    {
        private readonly DataGridViewEx dgv;
        private readonly GridFilterMenu gridFilterMenu = new GridFilterMenu();
        private ColumnFilterControl filterCtl;
        private readonly GridFilter gridFilter = new GridFilter();
        private string clickedColumnName;

        private readonly Color NORMAL_HEADER_COLOR = Color.FromKnownColor(KnownColor.ControlText);
        private readonly Color FILTERED_HEADER_COLOR = Color.Magenta;
        
        public GridFilterHelper(DataGridViewEx dgv)
        {
            this.dgv = dgv;
            dgv.ColumnAdded += dgv_ColumnAdded;
            gridFilterMenu.Opening += gridFilterMenu_Opening;
            gridFilterMenu.Closing += gridFilterMenu_Closing;
            dgv.CellMouseClick += dgv_CellMouseClick;
        }

        private void dgv_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || dgv.GroupColumnName == string.Empty)
                return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            var cr = dgv.Rows[e.RowIndex] as CustomRow;
            if (cr.IsHeaderRow)
            {
                var col = dgv.Columns[dgv.GroupColumnName];
                gridFilter.RemoveColumnFilter(dgv.GroupColumnName);
                gridFilter.AddColumnFilter(dgv.GroupColumnName, col.ExtInfo().DataType, new object[0], KnownFilterTypes.None);
                gridFilterMenu.Show(dgv, dgv.PointToClient(Control.MousePosition));
            }
        }

        private void gridFilterMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            // Fixing bug (see https://stackoverflow.com/a/6430323/1477680)
            if (filterCtl.IsDropDownShowing)
                e.Cancel = true;
        }

        private void ColumnFilterControl_CancelClicked(object sender, EventArgs e)
        {
            gridFilterMenu.Close();
        }

        private void ColumnFilterControl_ResetClicked(object sender, EventArgs e)
        {
            ResetColumnFilter(clickedColumnName);
            //gridFilterMenu.Close(); // NOTE: reset filter but leave menu open
            var clickedColumn = dgv.Columns[clickedColumnName];
            if (clickedColumn.ExtInfo().IsDistinctFilterAllowed)
                filterCtl.PrepareForShow(clickedColumn.ExtInfo().DataType, clickedColumn.GetDistinctValues());
            else
                filterCtl.PrepareForShow(clickedColumn.ExtInfo().DataType);
            filterCtl.SelectedFilterType = KnownFilterTypes.None;
        }

        private void ColumnFilterControl_ApplyClicked(object sender, EventArgs e)
        {
            if (filterCtl.SelectedFilterType == KnownFilterTypes.None)
                ResetColumnFilter(clickedColumnName);
            else if (filterCtl.SelectedFilterType == KnownFilterTypes.DistinctValues)
                ApplyDistinctFilter();
            else
                ApplyRegularFilter(filterCtl.SelectedFilterType,
                    filterCtl.FirstValue, filterCtl.SecondValue);
            gridFilterMenu.Close();
        }

        private void dgv_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            gridFilter.RemoveColumnFilter(e.Column.Name);
            gridFilter.AddColumnFilter(e.Column.Name, e.Column.ExtInfo().DataType, new object[0], KnownFilterTypes.None);
            e.Column.HeaderCell.ContextMenuStrip = gridFilterMenu;
        }

        private void gridFilterMenu_Opening(object sender, CancelEventArgs e)
        {
            if (dgv.Rows(dr => !dr.IsNewRow).Count() == 0)
            {
                e.Cancel = true;
                return;
            }
            var cursorPos = dgv.PointToClient(Cursor.Position);
            var hitTestInfo = dgv.HitTest(cursorPos.X, cursorPos.Y);
            //Console.WriteLine("R: {0}, C: {1}", hitTestInfo.RowIndex, hitTestInfo.ColumnIndex);
            var clickedColumn = hitTestInfo.RowIndex >= 0 && (dgv.Rows[hitTestInfo.RowIndex] as CustomRow).IsHeaderRow
                ? dgv.Columns[dgv.GroupColumnName]
                : dgv.Columns[hitTestInfo.ColumnIndex];
            if (!dgv.AllowColumnFilters || clickedColumn.ExtInfo().AllowedColumnFilters == AllowedColumnFiltersEnum.None)
            {
                e.Cancel = true;
                return;
            }
            clickedColumnName = clickedColumn.Name;

            filterCtl = new ColumnFilterControl();
            if (!clickedColumn.ExtInfo().IsDistinctFilterAllowed)
                filterCtl.HideDistinctFilter();
            filterCtl.ApplyClicked += ColumnFilterControl_ApplyClicked;
            filterCtl.ResetClicked += ColumnFilterControl_ResetClicked;
            filterCtl.CancelClicked += ColumnFilterControl_CancelClicked;
            gridFilterMenu.SetHostedControl(filterCtl);

            if (clickedColumn.ExtInfo().IsDistinctFilterAllowed)
                filterCtl.PrepareForShow(clickedColumn.ExtInfo().DataType, clickedColumn.GetDistinctValues());
            else
                filterCtl.PrepareForShow(clickedColumn.ExtInfo().DataType);

            filterCtl.SelectedFilterType = gridFilter.GetColumnFilterType(clickedColumnName);
            if (gridFilter.GetColumnFilterType(clickedColumnName) == KnownFilterTypes.DistinctValues)
            {
                filterCtl.CheckedDistinctValues = 
                    gridFilter.GetColumnFilterValues(clickedColumnName).Cast<CellContents>();
            }
            else if (gridFilter.GetColumnFilterType(clickedColumnName) != KnownFilterTypes.None)
            {
                filterCtl.FirstValue = gridFilter.GetColumnFilterValues(clickedColumnName)[0].ToString();
                filterCtl.SecondValue = gridFilter.GetColumnFilterValues(clickedColumnName)[1].ToString();
            }
            e.Cancel = false; // surprisingly, the 1st time menu is shown - it's true (2nd time and further on - it's false)
        }

        public void SetColumnFilter(string columnName, object[] columnFilterValues, KnownFilterTypes columnFilterType)
        {
            var columnDataType = dgv.Columns[columnName].ExtInfo().DataType;
            gridFilter.AddColumnFilter(columnName, columnDataType, columnFilterValues, columnFilterType);
            ApplyGridFilter();
            dgv.Columns[columnName].HeaderCell.ToolTipText = SR("(filter is set)");
            dgv.Columns[columnName].HeaderCell.Style.ForeColor = FILTERED_HEADER_COLOR;
        }

        private void ApplyGridFilter() // it's this guy that does the job - showing or hiding rows
        {
            foreach (var dr in dgv.Rows(p => !p.IsNewRow && p.IsRegularRow))
            {
                var rowValuesPerColumn = new Dictionary<string, object>();
                dr.Cells.Cast<DataGridViewCell>().ToList()
                    .ForEach(cell => { rowValuesPerColumn[cell.OwningColumn.Name] = cell.Value; });
                bool rowMustBeVisible = gridFilter.CheckValues(rowValuesPerColumn);
                dr.Visible = rowMustBeVisible && !dr.IsChildRowHiddenByCollapsedHeader;
                dr.IsChildRowHiddenByColumnFilter = !rowMustBeVisible;
                
                // Grouping support:
                if (dr.ParentGroupRow != null)
                {
                    var footerRow = dr.ParentGroupRow.ChildRows.GetFooterRow();
                    if (dr.ParentGroupRow.ChildRows.Where(p => p.IsRegularRow).All(p => p.IsChildRowHiddenByColumnFilter))
                    {
                        // all regular rows are hidden by the filter --> hide group header as well
                        // (and group footer too, if any)
                        dr.ParentGroupRow.Visible = false;
                        if (footerRow != null)
                        {
                            footerRow.Visible = false;
                            footerRow.IsChildRowHiddenByColumnFilter = true;
                        }
                    }
                    else
                    {
                        // some regular rows are not hidden by the filter --> show group header (and
                        // group footer too, if any and if group is not collapsed)
                        dr.ParentGroupRow.Visible = true;
                        if (footerRow != null)
                        {
                            footerRow.Visible = !footerRow.IsChildRowHiddenByCollapsedHeader;
                            footerRow.IsChildRowHiddenByColumnFilter = false;
                        }
                    }
                }
            }
        }

        private void ResetColumnFilter(string columnName)
        {
            gridFilter.RemoveColumnFilter(columnName);
            ApplyGridFilter();
            dgv.Columns[columnName].HeaderCell.Style.ForeColor = NORMAL_HEADER_COLOR;
            dgv.Columns[columnName].HeaderCell.ToolTipText = null;
        }

        public bool AnyColumnsFiltered()
        {
            return gridFilter.AnyColumnsFiltered();
        }

        //public bool IsColumnFiltered(string columnName)
        //{
        //    return gridFilter.IsColumnFiltered(columnName);
        //}

        public void RemoveAllColumnFilters()
        {
            gridFilter.RemoveAllColumnFilters();
            ApplyGridFilter();
            dgv.Columns(col => col.HeaderCell.Style.ForeColor == FILTERED_HEADER_COLOR).ForEach(col =>
            {
                col.HeaderCell.Style.ForeColor = NORMAL_HEADER_COLOR;
                col.HeaderCell.ToolTipText = null;
            });
        }

        private void ApplyDistinctFilter()
        {
            SetColumnFilter(clickedColumnName, filterCtl.CheckedDistinctValues.ToArray(), KnownFilterTypes.DistinctValues);
        }

        private void ApplyRegularFilter(KnownFilterTypes filterType, string filterValue1, string filterValue2)
        {
            if (filterValue1.Length == 0 ||
                (filterValue2.Length == 0 && (filterType == KnownFilterTypes.Between || filterType == KnownFilterTypes.NotBetween)))
                ResetColumnFilter(clickedColumnName); // NOTE: basically this check is redundant: we should never get here because of the 1st 'if' in the ColumnFilterControl_ApplyClicked() method
            else
                SetColumnFilter(clickedColumnName, new object[] { filterValue1, filterValue2 }, filterType);
        }

        private class GridFilter
        {
            private readonly Dictionary<string, ColumnFilter> columnFilters = new Dictionary<string, ColumnFilter>();

            public void AddColumnFilter(string columnName, GridColumnDataType columnDataType, object[] columnFilterValues, KnownFilterTypes columnFilterType)
            {
                columnFilters[columnName] = new ColumnFilter(columnName, columnDataType, columnFilterValues, columnFilterType);
            }

            public void RemoveColumnFilter(string columnName)
            {
                if (columnFilters.ContainsKey(columnName))
                    columnFilters[columnName].Reset();
            }

            public void RemoveAllColumnFilters()
            {
                foreach (var columnFilter in columnFilters.Values)
                    columnFilter.Reset();
            }

            public bool CheckValues(Dictionary<string, object> rowValuesPerColumn)
            {
                if (!AnyColumnsFiltered())
                    return true;
                foreach (var kvp in rowValuesPerColumn)
                {
                    var columnName = kvp.Key;
                    if (!IsColumnFiltered(columnName))
                        continue;
                    var cellValue = kvp.Value;
                    if (!columnFilters[columnName].CheckValue(cellValue))
                        return false; // no sense to check the remaining columns
                }
                return true;
            }

            public bool IsColumnFiltered(string columnName)
            {
                return columnFilters[columnName].FilterType != KnownFilterTypes.None;
            }

            public bool AnyColumnsFiltered()
            {
                return columnFilters.Values.Any(cf => cf.FilterType != KnownFilterTypes.None);
            }

            public KnownFilterTypes GetColumnFilterType(string columnName)
            {
                return columnFilters[columnName].FilterType;
            }

            public GridColumnDataType GetColumnDataType(string columnName)
            {
                return columnFilters[columnName].ColumnDataType;
            }

            public object[] GetColumnFilterValues(string columnName)
            {
                return columnFilters[columnName].FilterValues;
            }
        }

        private class ColumnFilter
        {
            public string ColumnName { get; }
            public GridColumnDataType ColumnDataType { get; }
            public object[] FilterValues { get; private set; }
            public KnownFilterTypes FilterType { get; private set; }

            public ColumnFilter(string columnName, GridColumnDataType columnDataType, object[] filterValues, KnownFilterTypes columnFilterType)
            {
                ColumnName = columnName;
                ColumnDataType = columnDataType;
                FilterValues = filterValues;
                FilterType = columnFilterType;
            }

            public void Reset()
            {
                FilterType = KnownFilterTypes.None;
                FilterValues = new object[0];
            }

            public bool CheckValue(object cellValue)
            {
                return FilterType == KnownFilterTypes.DistinctValues
                    ? CheckValueAgainstDistinctFilter(cellValue)
                    : CheckValueAgainstRegularFilter(cellValue);
            }

            private bool CheckValueAgainstDistinctFilter(object cellValue)
            {
                bool isCellValueEmpty = cellValue == null || cellValue == DBNull.Value || cellValue.ToString() == string.Empty;
                bool nullValuePresent = FilterValues.Cast<CellContents>().Any(cc => cc.IsEmpty);
                var listOfNonNullValues = FilterValues.Cast<CellContents>().Where(cc => !cc.IsEmpty).Select(cc => cc.Value);
                bool nonNullValuesPresent = listOfNonNullValues.Count() > 0;
                if (nullValuePresent)
                {
                    if (nonNullValuesPresent)
                        return isCellValueEmpty || cellValue.In(listOfNonNullValues);
                    else
                        return isCellValueEmpty;
                }
                else
                {
                    if (nonNullValuesPresent)
                        return !isCellValueEmpty && cellValue.In(listOfNonNullValues);
                    else
                        return false;
                }
            }

            private bool CheckValueAgainstRegularFilter(object cellValue)
            {
                bool isCellValueEmpty = cellValue == null || cellValue == DBNull.Value || cellValue.ToString() == string.Empty;
                if (ColumnDataType == GridColumnDataType.Text)
                {
                    switch (FilterType)
                    {
                        case KnownFilterTypes.Equals:
                            return isCellValueEmpty
                                ? object.Equals(string.Empty, FilterValues[0])
                                : object.Equals(cellValue, FilterValues[0]);
                        case KnownFilterTypes.NotEquals:
                            return isCellValueEmpty
                                ? !object.Equals(string.Empty, FilterValues[0])
                                : !object.Equals(cellValue, FilterValues[0]);
                        case KnownFilterTypes.Contains:
                            return isCellValueEmpty
                                ? false
                                : cellValue.ToString().Contains(FilterValues[0].ToString());
                        case KnownFilterTypes.NotContains:
                            return isCellValueEmpty
                                ? false
                                : !cellValue.ToString().Contains(FilterValues[0].ToString());
                        case KnownFilterTypes.StartsWith:
                            return isCellValueEmpty
                                ? false
                                : cellValue.ToString().StartsWith(FilterValues[0].ToString());
                        case KnownFilterTypes.NotStartsWith:
                            return isCellValueEmpty
                                ? false
                                : !cellValue.ToString().StartsWith(FilterValues[0].ToString());
                        case KnownFilterTypes.EndsWith:
                            return isCellValueEmpty
                                ? false
                                : cellValue.ToString().EndsWith(FilterValues[0].ToString());
                        case KnownFilterTypes.NotEndsWith:
                            return isCellValueEmpty
                                ? false
                                : !cellValue.ToString().EndsWith(FilterValues[0].ToString());
                        default: // KnownFilterTypes.RegExpression
                            var rx = new Regex(FilterValues[0].ToString());
                            return rx.IsMatch((cellValue ?? "").ToString());
                    }
                }
                else if (ColumnDataType == GridColumnDataType.Integer || ColumnDataType == GridColumnDataType.Decimal)
                {
                    var filterValue1 = Convert.ToDecimal(FilterValues[0]);
                    var cellValueDecimal = isCellValueEmpty ? 0 : Convert.ToDecimal(cellValue);
                    switch (FilterType)
                    {
                        case KnownFilterTypes.Equals:
                            return isCellValueEmpty
                                ? object.Equals(0, filterValue1)
                                : object.Equals(cellValueDecimal, filterValue1);
                        case KnownFilterTypes.NotEquals:
                            return isCellValueEmpty
                                ? !object.Equals(0, filterValue1)
                                : !object.Equals(cellValueDecimal, filterValue1);
                        case KnownFilterTypes.Greater:
                            return isCellValueEmpty
                                ? false
                                : cellValueDecimal > filterValue1;
                        case KnownFilterTypes.GreaterOrEqual:
                            return isCellValueEmpty
                                ? false
                                : cellValueDecimal >= filterValue1;
                        case KnownFilterTypes.Less:
                            return isCellValueEmpty
                                ? false
                                : cellValueDecimal < filterValue1;
                        case KnownFilterTypes.LessOrEqual:
                            return isCellValueEmpty
                                ? false
                                : cellValueDecimal <= filterValue1;
                        case KnownFilterTypes.Between:
                            {
                                var filterValue2 = Convert.ToDecimal(FilterValues[1]);
                                return isCellValueEmpty
                                    ? false
                                    : cellValueDecimal >= filterValue1 && cellValueDecimal <= filterValue2;
                            }
                        default: // KnownFilterTypes.NotBetween
                            {
                                var filterValue2 = Convert.ToDecimal(FilterValues[1]);
                                return isCellValueEmpty
                                    ? false
                                    : cellValueDecimal < filterValue1 || cellValueDecimal > filterValue2;
                            }
                    }
                }
                else // GridColumnDataType.DateTime
                {
                    var filterValue1 = Convert.ToDateTime(FilterValues[0]);
                    var cellValueDateTime = isCellValueEmpty ? DateTime.MinValue : Convert.ToDateTime(cellValue);
                    switch (FilterType)
                    {
                        case KnownFilterTypes.Equals:
                            return isCellValueEmpty
                                ? false
                                : object.Equals(cellValueDateTime, filterValue1);
                        case KnownFilterTypes.NotEquals:
                            return isCellValueEmpty
                                ? false
                                : !object.Equals(cellValueDateTime, filterValue1);
                        case KnownFilterTypes.Greater:
                            return isCellValueEmpty
                                ? false
                                : cellValueDateTime > filterValue1;
                        case KnownFilterTypes.GreaterOrEqual:
                            return isCellValueEmpty
                                ? false
                                : cellValueDateTime >= filterValue1;
                        case KnownFilterTypes.Less:
                            return isCellValueEmpty
                                ? false
                                : cellValueDateTime < filterValue1;
                        case KnownFilterTypes.LessOrEqual:
                            return isCellValueEmpty
                                ? false
                                : cellValueDateTime <= filterValue1;
                        case KnownFilterTypes.Between:
                            {
                                var filterValue2 = Convert.ToDateTime(FilterValues[1]);
                                return isCellValueEmpty
                                    ? false
                                    : cellValueDateTime >= filterValue1 && cellValueDateTime <= filterValue2;
                            }
                        default: // KnownFilterTypes.NotBetween
                            {
                                var filterValue2 = Convert.ToDateTime(FilterValues[1]);
                                return isCellValueEmpty
                                    ? false
                                    : cellValueDateTime < filterValue1 || cellValueDateTime > filterValue2;
                            }
                    }
                }
            }
        }
    }

    public class CellContents : IEquatable<CellContents>
    {
        public CellContents(object value, object formattedValue)
        {
            Value = value;
            FormattedValue = formattedValue;
        }

        public object Value { get; set; }
        public object FormattedValue { get; set; }

        public bool IsEmpty
        {
            get { return Value == null || Value == DBNull.Value || Value.ToString() == string.Empty; }
        }

        //public override bool Equals(object obj)
        //{
        //    return obj == Value;
        //}

        public bool Equals(CellContents other)
        {
            if (other == null)
                return false;
            var isThisEmpty = Value == null || Value == DBNull.Value || Value.ToString() == string.Empty;
            var isOtherEmpty = other.Value == null || other.Value == DBNull.Value || other.Value.ToString() == string.Empty;
            if (isThisEmpty && isOtherEmpty)
                return true;
            return EqualityComparer<object>.Default.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<object>.Default.GetHashCode(Value);
        }

        //public static bool operator ==(CellContents left, CellContents right)
        //{
        //    return EqualityComparer<CellContents>.Default.Equals(left, right);
        //}

        //public static bool operator !=(CellContents left, CellContents right)
        //{
        //    return !(left == right);
        //}

        public override string ToString()
        {
            return FormattedValue?.ToString();
        }
    }
}
