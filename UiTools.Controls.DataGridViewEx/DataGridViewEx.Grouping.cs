using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UiTools.Controls.ExtendedDataGridView
{
    public partial class DataGridViewEx
    {
        private const string COLUMN_FOOTER_DEFAULT_HINT = "right click to select aggregate function for this column";

        public void GroupByColumn(string colName, object[] valuesWhichForceSeparateGroups = null)
        {
            if (DataSource != null)
                throw new InvalidOperationException("Grouping by column works only in unbound mode");

            if (Columns.Count == 0)
                return;

            //if (colName == GroupColumnName)
            //    return; // already sorted by this column
            // (sorting by the same column DOES HAVE sense sometimes - for example, when it's done to bring footer rows to scene)

            ignoreRowsAddedEvent = true;
            try
            {
                SuspendLayout();
                if (string.IsNullOrEmpty(colName))
                {
                    // grouping --> OFF
                    if (IsGroupingOFF)
                        return; // grouping is already OFF - nothing to do
                    LeaveOnlyRegularRows(); // remove header and footer rows (to restore the initial row set)
                    Columns[GroupColumnName].Visible = true; // restore visibility of the old group column
                    GroupColumnName = "";
                    return;
                }
                else
                {
                    // grouping --> ON
                    if (!Columns.Contains(colName))
                        throw new ArgumentOutOfRangeException(nameof(colName), $"No such column: {colName}");
                    if (!Columns[colName].ExtInfo().AllowGrouping)
                        throw new InvalidOperationException(
                            $"Cannot group by column \"{colName}\" because its GridColumnExtInfo.AllowGrouping is set to false");
                    
                    ValuesWhichForceSeparateGroups = valuesWhichForceSeparateGroups;
                    ExpandAllGroups();
                    // before we clear DataGridViewRowCollection and all rows loose their Cells collection - obtain
                    // distinct values (in the group column cell) and corresponding regular rows:
                    var rowsByGroupDict = new RowsByGroupDictionary();
                    var distinctValues = RegularRows
                        .Select(dr => dr.Cells[colName].Value)
                        .Distinct();
                    foreach (var dv in distinctValues)
                    {
                        var groupMembers = RegularRows
                            .Where(dr => Equals(dr.Cells[colName].Value, dv))
                            .ToList();
                        rowsByGroupDict.Add(dv, groupMembers);
                    }

                    // now clear all rows:
                    ClearRowsSafe(RowsEx, true);

                    CurrentSortColumnIndex = NO_SORTING;
                    CurrentSortOrder = SortOrder.None;

                    if (IsGroupingON)
                        Columns[GroupColumnName].Visible = true; // restore visibility of the old group column

                    GroupColumnName = colName;
                    CustomRow mergedGroupHeaderRow = null;
                    CustomRow mergedGroupFooterRow = null;
                    foreach (var dv in rowsByGroupDict.Keys)
                    {
                        // Find rows which are members of this group:
                        var groupMembers = rowsByGroupDict[dv];
                        if (groupMembers.Count > 1 || (valuesWhichForceSeparateGroups != null && valuesWhichForceSeparateGroups.Contains(dv)))
                        {
                            // Create group header row with child rows:
                            CreateAndAddGroupHeaderRowWithChildren(dv, groupMembers);
                        }
                        else if (groupMembers.Count == 1)
                        {
                            switch (NonRepeatingValuesTreatment)
                            {
                                case NonRepeatingValuesTreatmentEnum.PlaceInSeparateGroups:
                                    // Create group header row with child rows:
                                    CreateAndAddGroupHeaderRowWithChildren(dv, groupMembers);
                                    break;
                                case NonRepeatingValuesTreatmentEnum.MergeInOneSpecialGroup:
                                    if (mergedGroupHeaderRow == null)
                                    {
                                        mergedGroupHeaderRow = CreateAndInsertMergedGroupHeaderRow(); // has zero index!
                                        if (ShowGroupFooters != ShowGroupFootersEnum.None)
                                        {
                                            mergedGroupFooterRow = CreateAndInsertFooterRow(1);
                                            mergedGroupHeaderRow.ChildRows.Add(mergedGroupFooterRow);
                                        }
                                    }
                                    // Add child rows:
                                    Rows.Insert(mergedGroupHeaderRow.Index + 1, groupMembers[0]);
                                    mergedGroupHeaderRow.ChildRows.Insert(0, Rows[mergedGroupHeaderRow.Index + 1] as CustomRow);
                                    break;
                                default: // NonRepeatingValuesTreatmentEnum.DoNotGroup
                                    Rows.Insert(0, groupMembers[0]); // inserting at zero index!
                                    break;
                            }
                        }
                    }
                    if (NonRepeatingValuesTreatment == NonRepeatingValuesTreatmentEnum.PlaceInSeparateGroups)
                        Columns[colName].Visible = false;
                    AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                    //Invalidate();
                }
            }
            finally
            {
                ResumeLayout(true);
                ignoreRowsAddedEvent = false;
            }
        }

        private CustomRow CreateAndInsertFooterRow(int index)
        {
            var values = new List<object>(Enumerable.Repeat("", Columns.Count));
            Rows.Insert(index, values.ToArray()); // !!! and yes - all values are empty strings
            var groupFooterRow = Rows[index] as CustomRow;
            if (ShowHintForGroupFooters)
                groupFooterRow.Cells
                    .Cast<DataGridViewCell>()
                    .ToList()
                    .ForEach(cell => cell.ToolTipText = cell.OwningColumn.ExtInfo().AllowChangeGroupFooterAggregate
                        ? COLUMN_FOOTER_DEFAULT_HINT
                        : string.Empty);
            groupFooterRow.RowKind = CustomRowKindEnum.Footer;
            groupFooterRow.DefaultCellStyle.Font = BoldFont;
            groupFooterRow.DefaultCellStyle.BackColor = GroupFooterRowBackColor;
            groupFooterRow.DefaultCellStyle.ForeColor = GroupFooterRowForeColor;
            groupFooterRow.DefaultCellStyle.SelectionBackColor = GroupFooterRowSelectionBackColor;
            groupFooterRow.DefaultCellStyle.SelectionForeColor = GroupFooterRowSelectionForeColor;
            return groupFooterRow;
        }

        private void ShowFooterAggregateMenu(DataGridViewColumn column)
        {
            if (!column.ExtInfo().AllowChangeGroupFooterAggregate)
                return;
            ctxFooterMenu.RemoveRightSideEmptySpace();
            ctxFooterMenu.Items.Clear();
            ctxFooterMenu.Items.AddRange(
                typeof(GroupFooterAggregateEnum)
                    .GetEnumAttributes()
                    .Where(m => m.Value != (int)GroupFooterAggregateEnum.Custom)
                    .Select(m => new ToolStripMenuItem(m.Description) { CheckOnClick = true, ToolTipText = m.Hint, Tag = m.Value })
                    .ToArray());
            if (column.ExtInfo().SupportsCustomGroupFooterAggregate)
            {
                ctxFooterMenu.Items.Add(new ToolStripSeparator());
                ctxFooterMenu.Items.Add(new ToolStripMenuItem("Custom") { CheckOnClick = true, Tag = GroupFooterAggregateEnum.Custom });
            }
            ctxFooterMenu.Items
                .OfType<ToolStripMenuItem>()
                .ToList()
                .ForEach(p => p.Checked = (int)p.Tag == (int)column.ExtInfo().GroupFooterAggregate);
            var tsiCustom = ctxFooterMenu.Items
                .OfType<ToolStripMenuItem>()
                .FirstOrDefault(m => (int)m.Tag == (int)GroupFooterAggregateEnum.Custom);
            if (tsiCustom != null)
            {
                tsiCustom.Text = string.IsNullOrEmpty(column.ExtInfo().CustomGroupFooterAggregateDisplayName)
                    ? GroupFooterAggregateEnum.Custom.GetDescription()
                    : column.ExtInfo().CustomGroupFooterAggregateDisplayName;
                tsiCustom.ToolTipText = string.IsNullOrEmpty(column.ExtInfo().CustomGroupFooterAggregateHint)
                    ? GroupFooterAggregateEnum.Custom.GetHint()
                    : column.ExtInfo().CustomGroupFooterAggregateHint;
            }
            ctxFooterMenu.Tag = column;
            ctxFooterMenu.Show(this, PointToClient(MousePosition));
        }

        private void ctxFooterMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var value = typeof(GroupFooterAggregateEnum)
                .GetEnumAttributes()
                .FirstOrDefault(p => p.Value == (int)e.ClickedItem.Tag)?.Value;
            if (value == null)
                return;
            var column = ctxFooterMenu.Tag as DataGridViewColumn;
            column.ExtInfo().GroupFooterAggregate = (GroupFooterAggregateEnum)value;
            FooterRows.ToList().ForEach(cr => UpdateAggregateInColumnFooter(cr, column.Index));
        }

        private void CreateAndAddGroupHeaderRowWithChildren(object dv, List<CustomRow> groupMembers)
        {
            var values = new List<object>(Enumerable.Repeat("", Columns.Count));
            values[Columns[GroupColumnName].Index] = dv;
            int idx = Rows.Add(values.ToArray());
            var groupHeaderRow = Rows[idx] as CustomRow;
            groupHeaderRow.RowKind = CustomRowKindEnum.Header;
            groupHeaderRow.DefaultCellStyle.Font = Font;
            // Create rows for group members:
            foreach (var gm in groupMembers)
            {
                idx = Rows.Add(gm);
                groupHeaderRow.ChildRows.Add(Rows[idx] as CustomRow);
            }
            UpdateGroupHeaderRowCaption(groupHeaderRow);
            if (ShowGroupFooters != ShowGroupFootersEnum.None &&
                !(ShowGroupFooters == ShowGroupFootersEnum.ForAllGroupsExceptSingleMemberOnes && groupMembers.Count == 1))
            {
                //var groupFooterRow = CreateAndAddFooterRow();
                var groupFooterRow = CreateAndInsertFooterRow(AllowUserToAddRows ? Rows.Count - 1 : Rows.Count);
                groupHeaderRow.ChildRows.Add(groupFooterRow);
                UpdateAggregatesInGroupFooterRow(groupFooterRow);
            }
        }

        private CustomRow CreateAndInsertMergedGroupHeaderRow()
        {
            var values = new List<object>(Enumerable.Repeat("", Columns.Count));
            values[Columns[GroupColumnName].Index] = NonRepeatingValuesGroupCaption;
            Rows.Insert(0, values.ToArray());
            var mergedGroupHeaderRow = Rows[0] as CustomRow;
            mergedGroupHeaderRow.RowKind = CustomRowKindEnum.Header;
            mergedGroupHeaderRow.IsMergedGroupHeaderRow = true;
            mergedGroupHeaderRow.DefaultCellStyle.Font = Font;
            return mergedGroupHeaderRow;
        }

        internal void UpdateGroupHeaderRowCaption(CustomRow groupHeaderRow)
        {
            var value = this[GroupColumnName, groupHeaderRow.Index].FormattedValue;
            string valueForDisplay;
            if (value == null)
                valueForDisplay = "[NULL]";
            else
                valueForDisplay = value.ToString() == string.Empty ? "[empty string]" : value.ToString();
            switch (GroupHeaderRowCaptionFormat)
            {
                case GroupHeaderRowCaptionFormatEnum.ValueOnly:
                    groupHeaderRow.HeaderRowCaption = valueForDisplay;
                    break;
                case GroupHeaderRowCaptionFormatEnum.ColumnNameAndValue:
                    groupHeaderRow.HeaderRowCaption = string.Format("{0}: {1}",
                        GroupColumnName,
                        valueForDisplay);
                    break;
                case GroupHeaderRowCaptionFormatEnum.ColumnNameAndValueAndCount:
                    groupHeaderRow.HeaderRowCaption = string.Format("{0}: {1} ({2})",
                        GroupColumnName,
                        valueForDisplay,
                        groupHeaderRow.ChildRows.Where(cr => cr.IsRegularRow).Count()); // (exclude footer rows if any)
                    break;
                default: // GroupHeaderRowCaptionFormatEnum.Custom
                    if (CustomGroupHeaderCaptionNeeded != null)
                    {
                        var args = new CustomGroupHeaderCaptionNeededArgs(groupHeaderRow);
                        CustomGroupHeaderCaptionNeeded(this, args);
                        if (args.Handled)
                        {
                            groupHeaderRow.HeaderRowCaption = args.Caption;
                            groupHeaderRow.HeaderRowHint = string.Empty;
                        }
                        //else
                        //{
                        //    groupHeaderRow.HeaderRowCaption = "n/a";
                        //    groupHeaderRow.HeaderRowHint = "(event CustomGroupHeaderCaptionNeeded: Handled flag not set)";
                        //}
                    }
                    else
                    {
                        groupHeaderRow.HeaderRowCaption = "n/a";
                        groupHeaderRow.HeaderRowHint = "(event CustomGroupHeaderCaptionNeeded has no handler)";
                    }
                    break;
            }
        }

        /// <summary>
        /// Updates aggregate value and formatting in the given column, in all footer rows
        /// (if the given column is visible and has ExtInfo().GroupFooterAggregate != GroupFooterAggregateEnum.None,
        /// and if grid has ShowGroupFooters != ShowGroupFootersEnum.None).
        /// </summary>
        public void UpdateAggregatesInColumnFooter(int columnIndex)
        {
            if (columnIndex <= 0 || columnIndex > Columns.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            if (!Columns[columnIndex].Visible /*|| Columns[columnIndex].ExtInfo().GroupFooterAggregate == GroupFooterAggregateEnum.None*/)
                return;
            if (ShowGroupFooters == ShowGroupFootersEnum.None)
                return;

            FooterRows.ForEach(r => UpdateAggregateInColumnFooter(r, columnIndex));
        }

        /// <summary>
        /// Updates aggregate value and formatting in the given footer row, in all visible columns
        /// having ExtInfo().GroupFooterAggregate != GroupFooterAggregateEnum.None (if grid has
        /// ShowGroupFooters != ShowGroupFootersEnum.None).
        /// </summary>
        public void UpdateAggregatesInGroupFooterRow(CustomRow groupFooterRow)
        {
            if (groupFooterRow == null)
                throw new ArgumentNullException(nameof(groupFooterRow));
            if (groupFooterRow.RowKind != CustomRowKindEnum.Footer)
                throw new ArgumentException("Supplied row must be a footer row", nameof(groupFooterRow));
            if (ShowGroupFooters == ShowGroupFootersEnum.None)
                return;

            this.Columns(c => c.Visible /*&& c.ExtInfo().GroupFooterAggregate != GroupFooterAggregateEnum.None*/)
                .ForEach(col => UpdateAggregateInColumnFooter(groupFooterRow, col.Index));
        }

        /// <summary>
        /// Updates aggregate value and formatting in the given footer cell defined by the provided footer row and column
        /// index (if the given column is visible and has ExtInfo().GroupFooterAggregate != GroupFooterAggregateEnum.None,
        /// and if grid has ShowGroupFooters != ShowGroupFootersEnum.None).
        /// </summary>
        public void UpdateAggregateInColumnFooter(CustomRow groupFooterRow, int columnIndex)
        {
            if (groupFooterRow == null)
                throw new ArgumentNullException(nameof(groupFooterRow));
            if (groupFooterRow.RowKind != CustomRowKindEnum.Footer)
                throw new ArgumentException("Supplied row must be a footer row", nameof(groupFooterRow));
            if (columnIndex < 0 || columnIndex > Columns.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            if (!Columns[columnIndex].Visible /*|| Columns[columnIndex].ExtInfo().GroupFooterAggregate == GroupFooterAggregateEnum.None*/)
                return;
            if (ShowGroupFooters == ShowGroupFootersEnum.None)
                return;

            var col = Columns[columnIndex];
            var cell = groupFooterRow.Cells[columnIndex];
            var regularRows = groupFooterRow.ParentGroupRow.ChildRows.Where(cr => cr.IsRegularRow);

            ResetFooterCellToDefaults(cell, col.ExtInfo().AllowChangeGroupFooterAggregate);

            switch (col.ExtInfo().GroupFooterAggregate)
            {
                case GroupFooterAggregateEnum.None:
                    cell.Value = string.Empty;
                    break;
                case GroupFooterAggregateEnum.TotalCount:
                    cell.Value = AggregateHelper.Count(regularRows);
                    break;
                case GroupFooterAggregateEnum.NonEmptyCount:
                    cell.Value = AggregateHelper.NonEmptyCount(regularRows, col.Index);
                    break;
                case GroupFooterAggregateEnum.Sum:
                    cell.Value = AggregateHelper.Sum(regularRows, col.Index, col.ExtInfo().DataType);
                    break;
                case GroupFooterAggregateEnum.Min:
                    cell.Value = AggregateHelper.Min(regularRows, col.Index, col.ExtInfo().DataType);
                    break;
                case GroupFooterAggregateEnum.Max:
                    cell.Value = AggregateHelper.Max(regularRows, col.Index, col.ExtInfo().DataType);
                    break;
                case GroupFooterAggregateEnum.Avg:
                    cell.Value = AggregateHelper.Avg(regularRows, col.Index, col.ExtInfo().DataType,
                        AvgGroupFooterAggregateTreatEmptyAsZero);
                    break;
                case GroupFooterAggregateEnum.Custom:
                    if (CustomColumnFooterValueNeeded != null)
                    {
                        var args = new CustomColumnFooterValueNeededArgs(groupFooterRow, col);
                        CustomColumnFooterValueNeeded(this, args);
                        if (args.Handled)
                        {
                            cell.Value = args.ColumnFooterValue;
                            cell.ToolTipText = string.IsNullOrEmpty(args.ColumnFooterHint)
                                ? (col.ExtInfo().AllowChangeGroupFooterAggregate
                                    ? COLUMN_FOOTER_DEFAULT_HINT
                                    : string.Empty)
                                : args.ColumnFooterHint;
                        }
                        //else
                        //{
                        //    cell.Value = "n/a";
                        //    cell.ToolTipText = "(event CustomColumnFooterValueNeeded: Handled flag not set)";
                        //}
                    }
                    else
                    {
                        cell.Value = "n/a";
                        cell.ToolTipText = "(event CustomColumnFooterValueNeeded has no handler)";
                    }
                    break;
            }
            if (ColumnFooterFormatting != null)
            {
                var args = new ColumnFooterFormattingArgs(groupFooterRow, col);
                ColumnFooterFormatting(this, args);
                if (args.Handled)
                {
                    cell.Style.BackColor = args.ColumnFooterBackColor;
                    cell.Style.SelectionBackColor = args.ColumnFooterSelectionBackColor;
                    cell.Style.ForeColor = args.ColumnFooterForeColor;
                    cell.Style.SelectionForeColor = args.ColumnFooterSelectionForeColor;
                }
            }
            if (Columns[col.Index].ExtInfo().GroupFooterAggregateAlignment == HorizontalAlignment.Left)
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            else if (Columns[col.Index].ExtInfo().GroupFooterAggregateAlignment == HorizontalAlignment.Center)
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            else
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void ResetFooterCellToDefaults(DataGridViewCell cell, bool allowChangeGroupFooterAggregate)
        {
            cell.Value = string.Empty;
            cell.ToolTipText = allowChangeGroupFooterAggregate
                ? COLUMN_FOOTER_DEFAULT_HINT
                : string.Empty;
            cell.Style.BackColor = GroupFooterRowBackColor;
            cell.Style.SelectionBackColor = GroupFooterRowSelectionBackColor;
            cell.Style.ForeColor = GroupFooterRowForeColor;
            cell.Style.SelectionForeColor = GroupFooterRowSelectionForeColor;
        }

        public void ExpandAllGroups()
        {
            HeaderRows.ToList().ForEach(cr => cr.IsHeaderRowExpanded = true);
        }

        public void CollapseAllGroups()
        {
            HeaderRows.ToList().ForEach(cr => cr.IsHeaderRowExpanded = false);
        }

        private void ToggleExpandCollapse(int headerRowIndex)
        {
            var cr = Rows[headerRowIndex] as CustomRow;
            if (cr.RowKind != CustomRowKindEnum.Header)
                return;
            cr.IsHeaderRowExpanded = !cr.IsHeaderRowExpanded;
        }

        private void ApplyGroupingToNewOrChangedRow(CustomRow cr)
        {
            int rowIndex = cr.Index;
            var parentRow = cr.ParentGroupRow; // << can be not null only with _changed_ rows (not _added_ ones)
            var groupColumnValue = cr.Cells[GroupColumnName].Value;
            var matchingGroupHeaderRow = HeaderRows
                .Where(r => Equals(r.Cells[GroupColumnName].Value, groupColumnValue))
                .FirstOrDefault();
            // remove new row from its current position
            RowsEx.RemoveAt(rowIndex);
            if (NonRepeatingValuesTreatment == NonRepeatingValuesTreatmentEnum.PlaceInSeparateGroups ||
                (ValuesWhichForceSeparateGroups != null && ValuesWhichForceSeparateGroups.Contains(groupColumnValue)))
            {
                if (matchingGroupHeaderRow == null)
                {
                    // create a new group and insert into it
                    CreateAndAddGroupHeaderRowWithChildren(groupColumnValue, new List<CustomRow> { cr });
                }
                else
                {
                    // insert into existing group
                    Rows.Insert(matchingGroupHeaderRow.Index + 1, cr);
                    matchingGroupHeaderRow.ChildRows.Insert(0, cr); // Add() would require check for footer row presence (can't add below footer row!)
                }
            }
            else if (NonRepeatingValuesTreatment == NonRepeatingValuesTreatmentEnum.MergeInOneSpecialGroup) // non-repeating values must be placed to a separate ("merged") group
            {
                if (matchingGroupHeaderRow == null) // no matching group found --> insert new row into merged group
                {
                    // find merged group header row
                    var mergedGroupHeaderRow = HeaderRows.Where(r => r.IsMergedGroupHeaderRow).FirstOrDefault();
                    if (mergedGroupHeaderRow == null)
                    {
                        CreateMergedGroupAndAddToIt(cr);
                    }
                    else
                    {
                        // merged group exists
                        var anotherRowWithSameGroupColumnValue = mergedGroupHeaderRow.ChildRows
                            .FirstOrDefault(r => r.IsRegularRow && r.Index >= 0 && Equals(r.Cells[GroupColumnName].Value, groupColumnValue));
                        if (anotherRowWithSameGroupColumnValue == null)
                        {
                            // just insert new row into the merged group
                            Rows.Insert(mergedGroupHeaderRow.Index + 1, cr);
                            mergedGroupHeaderRow.ChildRows.Insert(0, Rows[mergedGroupHeaderRow.Index + 1] as CustomRow);
                        }
                        else
                        {
                            // remove 'anotherRowWithSameGroupColumnValue' from merged group (because it's specially made for non-repeating values)
                            RowsEx.RemoveAt(anotherRowWithSameGroupColumnValue.Index);
                            // create new group for both rows ('anotherRowWithSameGroupColumnValue' and the new one)
                            CreateAndAddGroupHeaderRowWithChildren(groupColumnValue,
                                new List<CustomRow> { anotherRowWithSameGroupColumnValue, cr });
                        }
                    }
                }
                else
                {
                    // insert into existing group
                    Rows.Insert(matchingGroupHeaderRow.Index + 1, cr);
                    matchingGroupHeaderRow.ChildRows.Insert(0, cr); // Add() would require check for footer row presence (can't add below footer row!)
                }
            }
            else if (NonRepeatingValuesTreatment == NonRepeatingValuesTreatmentEnum.DoNotGroup)
            {
                if (matchingGroupHeaderRow == null) // no matching group found
                {
                    var rowsWithoutGroup = RegularRows.Where(p => p.ParentGroupRow == null).ToList();
                    var anotherRowWithSameGroupColumnValue = rowsWithoutGroup
                        .FirstOrDefault(r => r.IsRegularRow && r.Index >= 0 && Equals(r.Cells[GroupColumnName].Value, groupColumnValue));
                    if (anotherRowWithSameGroupColumnValue == null)
                    {
                        Rows.Insert(0, cr); // place new row to the top - where all parentless rows reside
                    }
                    else
                    {
                        // remove 'anotherRowWithSameGroupColumnValue' from parentless rows
                        RowsEx.RemoveAt(anotherRowWithSameGroupColumnValue.Index);
                        // create new group for both rows ('anotherRowWithSameGroupColumnValue' and the new one)
                        CreateAndAddGroupHeaderRowWithChildren(groupColumnValue,
                            new List<CustomRow> { anotherRowWithSameGroupColumnValue, cr });
                    }
                }
                else
                {
                    // insert into existing group
                    Rows.Insert(matchingGroupHeaderRow.Index + 1, cr);
                    matchingGroupHeaderRow.ChildRows.Insert(0, cr); // Add() would require check for footer row presence (can't add below footer row!)
                }
            }
        }

        internal void CreateMergedGroupAndAddToIt(CustomRow cr)
        {
            var mergedGroupHeaderRow = CreateAndInsertMergedGroupHeaderRow(); // has zero index!
            // insert new row into merged group
            Rows.Insert(mergedGroupHeaderRow.Index + 1, cr);
            mergedGroupHeaderRow.ChildRows.Insert(0, Rows[1] as CustomRow);
            // insert footer row as well
            if (ShowGroupFooters != ShowGroupFootersEnum.None) // more strict would be "if (ShouldCreateFooter(mergedGroupHeaderRow.ChildRows))"
                                                               // but... here we know for sure that mergedGroupHeaderRow.ChildRows.Count == 1
            {
                var mergedGroupFooterRow = CreateAndInsertFooterRow(cr.Index + 1);
                mergedGroupHeaderRow.ChildRows.Add(mergedGroupFooterRow);
                UpdateAggregatesInGroupFooterRow(mergedGroupFooterRow);
            }
        }

        internal void RowRemovalGuard(CustomRow cr)
        {
            if (cr.IsHeaderRow || cr.IsFooterRow)
                throw new InvalidOperationException("Header and footer rows cannot be deleted");
        }

        private void LeaveOnlyRegularRows()
        {
            for (int i = Rows.Count - 1; i >= 0; i--)
            {
                if (Rows[i].IsNewRow)
                    continue;
                var cr = Rows[i] as CustomRow;
                if (cr.RowKind != CustomRowKindEnum.Regular)
                {
                    RowsEx.NativeRemoveAt(i);
                    if (cr.IsHeaderRow)
                        cr.ChildRows.Clear();
                }
                else
                    cr.ParentGroupRow = null;
            }
        }
    }
}
