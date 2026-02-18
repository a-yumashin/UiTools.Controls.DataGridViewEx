using System.Windows.Forms;

namespace UiTools.Controls.ExtendedDataGridView
{
    public class DataGridViewRowCollectionEx : DataGridViewRowCollection
    {
        public DataGridViewRowCollectionEx(DataGridView dataGridView)
            : base(dataGridView)
        {
        }

        public override void RemoveAt(int index)
        {
            // Prevent removal of header and footer rows by external (user's) code
            var cr = this[index] as CustomRow;
            (DataGridView as DataGridViewEx).RowRemovalGuard(cr);
            // Remove regular row from DGV rows:
            base.RemoveAt(index);
            // Remove regular row from its parent ChildRows collection (if parent is present):
            if (cr.ParentGroupRow != null)
                cr.ParentGroupRow.ChildRows.Remove(cr); // << this will also update - or remove - header and footer
        }
        // NOTE: no need to override also the Remove() method because it calls the RemoveAt() method under the hood

        internal void NativeRemoveAt(int index)
        {
            // But in DataGridViewEx code we need a "pure" removal when grouping or sorting:
            // - we must be able to remove both header and footer rows,
            // - and when removing regular rows - we don't need to update their header and footer.
            base.RemoveAt(index);
        }

        internal void NativeRemove(CustomRow cr)
        {
            NativeRemoveAt(cr.Index); // NOTE: cannot call base.Remove(cr) because it calls the [overriden!] RemoveAt() method under the hood
        }

        public override string ToString()
        {
            return $"Count: {Count}";
        }
    }
}
