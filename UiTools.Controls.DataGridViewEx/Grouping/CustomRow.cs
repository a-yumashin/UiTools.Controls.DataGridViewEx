using UiTools.Controls.ExtendedDataGridView.Properties;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace UiTools.Controls.ExtendedDataGridView
{
    public class CustomRow : DataGridViewRow
    {
        public const int COLLAPSE_EXPAND_IMAGE_HORIZ_MARGIN = 6; // left margin as well as right margin
        
        private bool isChildRowCollapsed = false;
        private CustomRowKindEnum rowKind = CustomRowKindEnum.Regular;
        private CustomRow parentGroupRow;
        private bool isHeaderRowExpanded = true;
        private bool isMergedGroupHeaderRow = false;
        private string headerRowCaption;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CustomRowKindEnum RowKind
        {
            get => rowKind;
            internal set
            {
                rowKind = value;
                if (rowKind == CustomRowKindEnum.Header)
                    ChildRows = new ChildRowsList(this);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChildRowsList ChildRows { get; private set; } // gets created only for header rows

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CustomRow ParentGroupRow
        {
            get => parentGroupRow;
            internal set
            {
                if (rowKind == CustomRowKindEnum.Header)
                    throw new InvalidOperationException(
                        $"{nameof(CustomRow)}.{nameof(ParentGroupRow)} property can be set only for regular and footer rows");
                parentGroupRow = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsHeaderRowExpanded
        {
            get => isHeaderRowExpanded;
            internal set
            {
                if (rowKind != CustomRowKindEnum.Header)
                    throw new InvalidOperationException(
                        $"{nameof(CustomRow)}.{nameof(IsHeaderRowExpanded)} property can be set only for header rows");
                isHeaderRowExpanded = value;
                ChildRows.ForEach(r => r.IsChildRowHiddenByCollapsedHeader = !isHeaderRowExpanded);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool IsChildRowHiddenByCollapsedHeader // 'true' means that the row is hidden because its parent row is collapsed
        {
            get { return isChildRowCollapsed; }
            set
            {
                if (rowKind == CustomRowKindEnum.Header)
                    throw new InvalidOperationException(
                        $"{nameof(CustomRow)}.{nameof(IsChildRowHiddenByCollapsedHeader)} property can be set only for regular and footer rows");
                isChildRowCollapsed = value;
                Visible = !isChildRowCollapsed && !IsChildRowHiddenByColumnFilter;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsChildRowHiddenByColumnFilter { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMergedGroupHeaderRow
        {
            get => isMergedGroupHeaderRow;
            internal set
            {
                if (rowKind != CustomRowKindEnum.Header)
                    throw new InvalidOperationException(
                        $"{nameof(CustomRow)}.{nameof(IsMergedGroupHeaderRow)} property can be set only for header rows");
                isMergedGroupHeaderRow = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsRegularRow => RowKind == CustomRowKindEnum.Regular;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsHeaderRow => RowKind == CustomRowKindEnum.Header;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFooterRow => RowKind == CustomRowKindEnum.Footer;

        private bool IsCurrentRow => Equals(DataGridView.CurrentRow);

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow)
        {
            base.Paint(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow);
            // This method is all about painting group header rows
            if (RowKind != CustomRowKindEnum.Header)
                return;

            // 1. Emulate colSpan with continuous background color fill:
            var rect = rowBounds;
            rect.Offset(DataGridView.RowHeadersWidth, 0); // exclude "row headers" from background color fill
            rect.Width = DataGridView.Columns(c => c.Visible).Select(c => c.Width).Sum() - 1;
            var dgv = DataGridView as DataGridViewEx;
            graphics.FillRectangle(dgv.BrushCache.Get(IsCurrentRow ? dgv.GroupHeaderRowSelectionBackColor : dgv.GroupHeaderRowBackColor), rect);

            graphics.SetClip(rect);
            // 2. Paint contents:
            // 2.1. Paint expand/collapse image (+/-):
            var img = IsHeaderRowExpanded ? Resources.Expanded : Resources.Collapsed;
            var imageTop = (rowBounds.Height - img.Height) / 2;
            graphics.DrawImage(img, DataGridView.RowHeadersWidth - DataGridView.HorizontalScrollingOffset + COLLAPSE_EXPAND_IMAGE_HORIZ_MARGIN,
                rowBounds.Y + imageTop, img.Width, img.Height);
            // 2.2. Paint text (group header caption):
            var textSize = graphics.MeasureString(HeaderRowCaption, dgv.BoldFont);
            var textTop = (rowBounds.Height - textSize.Height) / 2;
            RectangleF textRect = rect;
            var textOffset = COLLAPSE_EXPAND_IMAGE_HORIZ_MARGIN + img.Width + COLLAPSE_EXPAND_IMAGE_HORIZ_MARGIN - 4
                             - DataGridView.HorizontalScrollingOffset;
            textRect.Offset(textOffset, 0);
            textRect.Width -= textOffset + DataGridView.HorizontalScrollingOffset;
            var sf = new StringFormat { Trimming = StringTrimming.EllipsisCharacter };
            graphics.DrawString(HeaderRowCaption, dgv.BoldFont,
                dgv.BrushCache.Get(IsCurrentRow ? dgv.GroupHeaderRowSelectionForeColor : dgv.GroupHeaderRowForeColor),
                textRect, sf);
            // 2.3. Support sorting:
            if (dgv.CurrentSortColumnIndex == dgv.Columns[dgv.GroupColumnName].Index) // because RowKind == CustomRowKind.Header - we can be sure that dgv.GroupColumnName is set
            {
                // Draw sorting glyph
                var sortIcon = dgv.CurrentSortOrder == SortOrder.Ascending ?
                    VisualStyleElement.Header.SortArrow.SortedUp :
                    VisualStyleElement.Header.SortArrow.SortedDown;
                var renderer = new VisualStyleRenderer(sortIcon);
                var size = renderer.GetPartSize(graphics, ThemeSizeType.Draw);
                renderer.DrawBackground(graphics,
                    new Rectangle(rect.Right - size.Width - DataGridView.HorizontalScrollingOffset,
                    rect.Top, size.Width, rect.Height));
            }
            graphics.ResetClip();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string HeaderRowCaption
        {
            get => headerRowCaption;
            set
            {
                if (rowKind != CustomRowKindEnum.Header)
                    throw new InvalidOperationException(
                        $"{nameof(CustomRow)}.{nameof(HeaderRowCaption)} property can be set only for header rows");
                headerRowCaption = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string HeaderRowHint
        {
            get => Cells[0].ToolTipText;
            set
            {
                if (rowKind != CustomRowKindEnum.Header)
                    throw new InvalidOperationException(
                        $"{nameof(CustomRow)}.{nameof(HeaderRowHint)} property can be set only for header rows");
                Cells[0].ToolTipText = value;
            }
        }

        public override string ToString() // for debug only
        {
            if (RowKind == CustomRowKindEnum.Regular)
            {
                if (IsNewRow)
                    return string.Format("{0}: (new row)", Index);
                if (DataGridView == null)
                    return string.Format("{0}: Regular row (DGV == null)", Index);
                return GetCellValues();
            }
            if (RowKind == CustomRowKindEnum.Header)
            {
                if (DataGridView == null)
                    return string.Format("{0}: Header row (DGV == null)", Index);
                var dgv = DataGridView as DataGridViewEx;
                return "Header row: " + string.Format("{0}: {1} == {2} ({3})",
                    Index,
                    dgv.GroupColumnName,
                    dgv[dgv.GroupColumnName, Index].Value,
                    ChildRows.Count);
            }
            return GetCellValues("Footer row: ");
        }

        private string GetCellValues(string prefix = "", int maxCellCount = 3)
        {
            return string.Format("{0}{1}: {2}", prefix, Index,
                string.Join(" | ", Cells.Cast<DataGridViewCell>().Take(maxCellCount).Select(c => c.Value)));
        }
    }
}
