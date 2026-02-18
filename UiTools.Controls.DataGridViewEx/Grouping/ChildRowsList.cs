using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UiTools.Controls.ExtendedDataGridView
{
    public class ChildRowsList : IEnumerable<CustomRow>
    {
        private readonly List<CustomRow> innerList = new List<CustomRow>();
        private CustomRow owningRow;

        internal ChildRowsList(CustomRow owningRow)
        {
            if (owningRow == null)
                throw new ArgumentNullException(nameof(owningRow));
            if (!owningRow.IsHeaderRow)
                throw new InvalidOperationException("ChildRowsList can be created for header row only");
            this.owningRow = owningRow;
        }

        internal void Add(CustomRow row)
        {
            RowKindGuard();
            row.ParentGroupRow = owningRow;
            innerList.Add(row);
            if (!owningRow.IsHeaderRowExpanded)
                owningRow.IsHeaderRowExpanded = true;
            // NOTE: currently this method is used either in batch mode (in CreateAndAddGroupHeaderRowWithChildren())
            //       or to add footer rows - so it's not necessary to call UpdateGroupHeaderRowCaption() here
        }

        internal void Insert(int index, CustomRow row)
        {
            RowKindGuard();
            row.ParentGroupRow = owningRow;
            innerList.Insert(index, row);
            if (!owningRow.IsHeaderRowExpanded)
                owningRow.IsHeaderRowExpanded = true;
            var dgv = owningRow.DataGridView as DataGridViewEx;
            dgv.UpdateGroupHeaderRowCaption(owningRow);
            if (dgv.ShowGroupFooters != ShowGroupFootersEnum.None)
            {
                var groupFooterRow = GetFooterRow();
                if (groupFooterRow != null)
                    dgv.UpdateAggregatesInGroupFooterRow(groupFooterRow);
            }
        }

        //internal void RemoveAt(int index) // not used anymore
        //{
        //    RowKindGuard();
        //    bool isRegularRow = innerList[index].IsRegularRow;
        //    innerList[index].ParentGroupRow = null;
        //    innerList.RemoveAt(index);
        //    if (isRegularRow)
        //        UpdateOrRemoveHeaderAndFooterAfterRegularRowRemoval();
        //}

        internal bool Remove(CustomRow row)
        {
            RowKindGuard();
            row.ParentGroupRow = null;
            var result = innerList.Remove(row);
            if (row.IsRegularRow)
                UpdateOrRemoveHeaderAndFooterAfterRegularRowRemoval();
            return result;
        }

        internal void Clear()
        {
            RowKindGuard();
            innerList.ForEach(cr => cr.ParentGroupRow = null);
            innerList.Clear();
        }

        public int Count => innerList.Count;

        public CustomRow this[int index] => innerList[index];

        public int FindIndex(Predicate<CustomRow> match)
        {
            return innerList.FindIndex(match);
        }

        public void ForEach(Action<CustomRow> action)
        {
            innerList.ForEach(action);
        }

        public IEnumerator<CustomRow> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public CustomRow GetFooterRow()
        {
            return innerList.FirstOrDefault(cr => cr.IsFooterRow);
        }

        public override string ToString()
        {
            return $"Count: {innerList.Count}";
        }

        private int RegularRowsCount => innerList.Where(r => r.IsRegularRow).Count();

        private void RowKindGuard()
        {
            if (owningRow.RowKind != CustomRowKindEnum.Header)
                throw new InvalidOperationException("Applicable only when owning row is a group header row");
        }

        internal void UpdateOrRemoveHeaderAndFooterAfterRegularRowRemoval()
        {
            var dgv = owningRow.DataGridView as DataGridViewEx;
            var groupFooterRow = GetFooterRow();
            if (RegularRowsCount == 0)
            {
                // Group is empty now
                if (groupFooterRow != null)
                {
                    // Remove group footer row:
                    dgv.RowsEx.NativeRemoveAt(groupFooterRow.Index);
                    innerList.Remove(groupFooterRow);
                    groupFooterRow.Dispose();
                    groupFooterRow = null;
                }
                // Remove group header row:
                dgv.RowsEx.NativeRemoveAt(owningRow.Index);
                owningRow.Dispose();
                owningRow = null;
            }
            else
            {
                // Group is not empty
                if (RegularRowsCount == 1 && !owningRow.IsMergedGroupHeaderRow)
                {
                    // owningRow is a usual group row, and there's only 1 regular row left in this group
                    if (dgv.NonRepeatingValuesTreatment == NonRepeatingValuesTreatmentEnum.PlaceInSeparateGroups)
                    {
                        if (groupFooterRow != null)
                            dgv.UpdateAggregatesInGroupFooterRow(groupFooterRow);
                        dgv.UpdateGroupHeaderRowCaption(owningRow);
                    }
                    else
                    {
                        if (dgv.AutoGroupOnRowSetChanged)
                        {
                            var value = owningRow.Cells[dgv.GroupColumnName].Value;
                            if (dgv.ValuesWhichForceSeparateGroups == null || !dgv.ValuesWhichForceSeparateGroups.Contains(value))
                            {
                                // we must move this last row to either "merged" group or to the "parentless" rows area
                                var theLastRegularRow = innerList.FirstOrDefault(r => r.IsRegularRow);
                                dgv.RowsEx.NativeRemove(theLastRegularRow);
                                theLastRegularRow.ParentGroupRow = null;
                                innerList.Remove(theLastRegularRow);
                                dgv.ignoreRowsAddedEvent = true; // << important!
                                if (dgv.NonRepeatingValuesTreatment == NonRepeatingValuesTreatmentEnum.MergeInOneSpecialGroup)
                                {
                                    var mergedGroupHeaderRow = dgv.HeaderRows.Where(r => r.IsMergedGroupHeaderRow).FirstOrDefault();
                                    if (mergedGroupHeaderRow == null)
                                        dgv.CreateMergedGroupAndAddToIt(theLastRegularRow);
                                    else
                                    {
                                        dgv.Rows.Insert(mergedGroupHeaderRow.Index + 1, theLastRegularRow);
                                        mergedGroupHeaderRow.ChildRows.Insert(0, dgv.Rows[mergedGroupHeaderRow.Index + 1] as CustomRow);
                                    }
                                }
                                else // NonRepeatingValuesTreatmentEnum.DoNotGroup
                                {
                                    dgv.Rows.Insert(0, theLastRegularRow); // place new row to the top - where all parentless rows reside
                                }
                                dgv.ignoreRowsAddedEvent = false;
                                // and now we must remove the empty group:
                                UpdateOrRemoveHeaderAndFooterAfterRegularRowRemoval(); // recursion (RegularRowsCount will be ZERO so no infinite recursion will happen)
                            }
                        }
                    }
                }
                else
                {
                    // EITHER more than 1 regular rows left in a usual group OR it's a "merged" group (where 1 row is OK)
                    if (groupFooterRow != null)
                        dgv.UpdateAggregatesInGroupFooterRow(groupFooterRow);
                    dgv.UpdateGroupHeaderRowCaption(owningRow);
                }
            }
        }
    }
}
