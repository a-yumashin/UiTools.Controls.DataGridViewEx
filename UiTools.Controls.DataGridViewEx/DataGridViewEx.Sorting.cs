using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UiTools.Controls.ExtendedDataGridView
{
    public partial class DataGridViewEx
    {

        public void CustomSort(string columnName, SortOrder sortOrder = SortOrder.None)
        {
            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));
            if (columnName.Length == 0)
            {
                CurrentSortColumnIndex = NO_SORTING;
                CurrentSortOrder = SortOrder.None;
                Invalidate();
                return;
            }
            if (!Columns.Contains(columnName))
                throw new ArgumentOutOfRangeException(nameof(columnName), "No such column found: " + columnName);
            if (!Columns[columnName].ExtInfo().AllowSorting)
                throw new InvalidOperationException(
                    $"Cannot sort by column \"{columnName}\" because its GridColumnExtInfo.AllowSorting is set to false");

            CurrentSortColumnIndex = Columns[columnName].Index;
            CurrentSortOrder = sortOrder;
            CustomSort();
            //Invalidate();
        }

        private void CustomSort()
        {
            ignoreRowsAddedEvent = true;
            try
            {
                SuspendLayout();
                if (IsGroupingOFF)
                {
                    // no grouping by column (and ALL rows are "regular" rows)
                    SortRegularRows(this.Rows(p => !p.IsNewRow).OrderBy(p => p.Index).ToList(),
                        CurrentSortColumnIndex, CurrentSortOrder, Rows);
                }
                else if (CurrentSortColumnIndex == Columns[GroupColumnName].Index)
                {
                    // sort column == group column
                    SortHeaderRows(CurrentSortColumnIndex, CurrentSortOrder, Rows, NonRepeatingValuesTreatment);
                    if (NonRepeatingValuesTreatment == NonRepeatingValuesTreatmentEnum.MergeInOneSpecialGroup)
                    {
                        // group column is shown --> also sort child rows within merged group:
                        var mergedGroupHeaderRow = HeaderRows.FirstOrDefault(cr => cr.IsMergedGroupHeaderRow);
                        if (mergedGroupHeaderRow != null && mergedGroupHeaderRow.ChildRows.Count > 0)
                        {
                            SortRegularRows(mergedGroupHeaderRow.ChildRows.OrderBy(p => p.Index).ToList(),
                                CurrentSortColumnIndex, CurrentSortOrder, Rows);
                        }
                    }
                }
                else
                {
                    // sort column != group column
                    if (NonRepeatingValuesTreatment == NonRepeatingValuesTreatmentEnum.DoNotGroup) // no group row for groups with 1 member
                    {
                        // Make all "parentless" rows adjacent (by moving them to the top of the grid):
                        var rowsWithoutGroup = RegularRows.Where(r => r.ParentGroupRow == null).ToList();
                        rowsWithoutGroup.ForEach(cr => RowsEx.NativeRemove(cr));
                        rowsWithoutGroup.ForEach(cr => Rows.Insert(0, cr));
                        // Sort "parentless" rows:
                        SortRegularRows(rowsWithoutGroup.OrderBy(p => p.Index).ToList(), CurrentSortColumnIndex, CurrentSortOrder, Rows);
                    }
                    // sort by regular column within each group:
                    foreach (var cr in HeaderRows.Where(p => p.ChildRows.Count > 0))
                        SortRegularRows(cr.ChildRows.OrderBy(p => p.Index).ToList(), CurrentSortColumnIndex, CurrentSortOrder, Rows);
                }
            }
            finally
            {
                ResumeLayout(true);
                ignoreRowsAddedEvent = false;
            }
            SortingComplete?.Invoke(this, EventArgs.Empty);
        }

        private static void SortRegularRows(List<CustomRow> adjacentRowsToSort, int sortColumnIndex, SortOrder sortOrder,
            DataGridViewRowCollection allRows)
        {
            if (adjacentRowsToSort.Count == 0)
                return;
            var firstIndex = adjacentRowsToSort.Select(r => r.Index).First();
            var cellComparer = new CellComparer(sortOrder);
            var sortedRows = sortOrder == SortOrder.Ascending
                ? adjacentRowsToSort.OrderBy(r => r.Cells[sortColumnIndex], cellComparer).ToList()            // .ToList() -- important! otherwise allRows.RemoveAt() will clear sortedRows as well
                : adjacentRowsToSort.OrderByDescending(r => r.Cells[sortColumnIndex], cellComparer).ToList(); // .ToList() -- the same comment as above
            for (int i = 0; i < adjacentRowsToSort.Count(); i++)
            {
                (allRows as DataGridViewRowCollectionEx).NativeRemoveAt(firstIndex);
            }
            for (int i = sortedRows.Count - 1; i >= 0; i--)
            {
                allRows.Insert(firstIndex, sortedRows[i]);
            }
        }

        private static void SortHeaderRows(int sortColumnIndex, SortOrder sortOrder, DataGridViewRowCollection allRows,
            NonRepeatingValuesTreatmentEnum singleMemberGroupsTreatment)
        {
            var rowsToSort = allRows
                .Cast<CustomRow>()
                .Where(p => p.IsHeaderRow/* && !p.IsNewRow*/)
                .ToList();
            if (singleMemberGroupsTreatment == NonRepeatingValuesTreatmentEnum.DoNotGroup)
            {
                var rowsWithoutGroup = allRows
                    .Cast<CustomRow>()
                    .Where(p => p.IsRegularRow && !p.IsNewRow && p.ParentGroupRow == null)
                    .ToList();
                //Console.WriteLine(string.Join(", ", rowsWithoutGroup.Select(p => p.Cells["name"].Value)));
                rowsToSort.AddRange(rowsWithoutGroup);
            }
            var sortedRows = sortOrder == SortOrder.Ascending
                ? rowsToSort.OrderBy(r => r.Cells[sortColumnIndex].Value).ToList()            // .ToList() -- important! otherwise allRows.RemoveAt() will clear sortedRows as well
                : rowsToSort.OrderByDescending(r => r.Cells[sortColumnIndex].Value).ToList(); // .ToList() -- the same comment as above
            ClearRowsSafe(allRows as DataGridViewRowCollectionEx, false);
            for (int i = 0; i < sortedRows.Count; i++)
            {
                allRows.Add(sortedRows[i]);
                if (sortedRows[i].IsHeaderRow)
                    allRows.AddRange(sortedRows[i].ChildRows.ToArray());
            }
        }
    }
}
