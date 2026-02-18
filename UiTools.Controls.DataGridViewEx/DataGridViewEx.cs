using UiTools.Controls.ExtendedDataGridView.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static UiTools.Controls.ExtendedDataGridView.CommonStuff;

namespace UiTools.Controls.ExtendedDataGridView
{
    public partial class DataGridViewEx : DataGridView, IDataGridViewEx
    {
        public const int NO_SORTING = -200;

        public event EventHandler<CustomGroupHeaderCaptionNeededArgs> CustomGroupHeaderCaptionNeeded;
        public event EventHandler<CustomColumnFooterValueNeededArgs> CustomColumnFooterValueNeeded;
        public event EventHandler<ColumnFooterFormattingArgs> ColumnFooterFormatting;
        public event EventHandler SortingComplete;
        public event DataGridViewCellPaintingEventHandler MoreCellPainting;

        #region Public properties

        #region Grouping - main stuff

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Grouping - main stuff")]
        public string GroupColumnName { get; private set; }

        [DefaultValue(GroupHeaderRowCaptionFormatEnum.ColumnNameAndValueAndCount)]
        [Category("Grouping - main stuff")]
        public GroupHeaderRowCaptionFormatEnum GroupHeaderRowCaptionFormat { get; set; } = GroupHeaderRowCaptionFormatEnum.ColumnNameAndValueAndCount;

        [DefaultValue(30)]
        [Category("Grouping - main stuff")]
        public int GroupMembersFirstCellHorizOffset { get; set; } = 30;

        [DefaultValue(NonRepeatingValuesTreatmentEnum.MergeInOneSpecialGroup)]
        [Category("Grouping - main stuff")]
        public NonRepeatingValuesTreatmentEnum NonRepeatingValuesTreatment { get; set; } = NonRepeatingValuesTreatmentEnum.MergeInOneSpecialGroup;

        [DefaultValue("[non-repeating values]")]
        [Category("Grouping - main stuff")]
        public string NonRepeatingValuesGroupCaption { get; set; } = "[non-repeating values]";

        [DefaultValue(ShowGroupFootersEnum.None)]
        [Category("Grouping - main stuff")]
        public ShowGroupFootersEnum ShowGroupFooters { get; set; } = ShowGroupFootersEnum.None;

        [DefaultValue(false)]
        [Category("Grouping - main stuff")]
        public bool AutoGroupOnRowSetChanged { get; set; } = false;

        #endregion Grouping - main stuff

        #region Grouping - secondary stuff

        [Category("Grouping - secondary stuff")]
        public Color GroupHeaderRowForeColor { get; set; }
        [Category("Grouping - secondary stuff")]
        public Color GroupHeaderRowSelectionForeColor { get; set; }
        [Category("Grouping - secondary stuff")]
        public Color GroupHeaderRowBackColor { get; set; }
        [Category("Grouping - secondary stuff")]
        public Color GroupHeaderRowSelectionBackColor { get; set; }

        [Category("Grouping - secondary stuff")]
        public Color GroupFooterRowForeColor { get; set; } = Color.FromKnownColor(KnownColor.ControlText);
        [Category("Grouping - secondary stuff")]
        public Color GroupFooterRowSelectionForeColor { get; set; } = Color.FromKnownColor(KnownColor.HighlightText);
        [Category("Grouping - secondary stuff")]
        public Color GroupFooterRowBackColor { get; set; } = Color.FromKnownColor(KnownColor.Control);
        [Category("Grouping - secondary stuff")]
        public Color GroupFooterRowSelectionBackColor { get; set; } = Color.FromKnownColor(KnownColor.Highlight);

        [DefaultValue(false)]
        [Category("Grouping - secondary stuff")]
        public bool AvgGroupFooterAggregateTreatEmptyAsZero { get; set; } = false;

        [DefaultValue(true)]
        [Category("Grouping - secondary stuff")]
        public bool ShowHintForGroupFooters { get; set; } = true;

        #endregion Grouping - secondary stuff

        #region Sorting

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(SortOrder.None)]
        [Category("Sorting")]
        public SortOrder CurrentSortOrder { get; private set; } = SortOrder.None;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(NO_SORTING)]
        [Category("Sorting")]
        public int CurrentSortColumnIndex { get; private set; } = NO_SORTING;

        [DefaultValue(false)]
        [Category("Sorting")]
        public bool AutoSortOnRowSetChanged { get; set; } = false;

        #endregion Sorting

        #region Misc

        [DefaultValue("(NULL)")]
        public string NullReplacementText { get; set; } = "(NULL)"; // can be overriden at column/cell level: [Default]CellStyle.NullValue
        
        public Color NullReplacementColor { get; set; } = Color.Red;
        
        [DefaultValue(true)]
        public bool ShowHintForRowHeaders { get; set; } = true;

        [DefaultValue(true)]
        public bool ShowSandwichMenu
        {
            get => showSandwichMenu;
            set
            {
                showSandwichMenu = value;
                var gridMenu = Controls["gridSandwichMenu"];
                if (gridMenu != null)
                    UpdateGridSandwichMenuVisibility(gridMenu);
            }
        }

        [DefaultValue(true)]
        public bool AllowColumnFilters
        {
            get => allowColumnFilters;
            set
            {
                allowColumnFilters = value;
                var gridMenu = Controls["gridSandwichMenu"] as SandwichMenu;
                if (gridMenu != null)
                {
                    gridMenu.CtxMenu.Items["tsiResetAllFilters"].Visible = allowColumnFilters;
                    UpdateGridSandwichMenuVisibility(gridMenu);
                }
            }
        }

        [DefaultValue(true)]
        public bool AllowHideColumns
        {
            get => allowHideColumns;
            set
            {
                allowHideColumns = value;
                var gridMenu = Controls["gridSandwichMenu"] as SandwichMenu;
                if (gridMenu != null)
                {
                    gridMenu.CtxMenu.Items["tsiColumnsDisplay"].Visible = allowHideColumns;
                    UpdateGridSandwichMenuVisibility(gridMenu);
                }
            }
        }

        [DefaultValue("en")]
        public string UiLocale
        {
            get => Strings.Instance.CurrentLocale;
            set
            {
                if (!Strings.Instance.SupportedLocales.Contains(value))
                    throw new ArgumentOutOfRangeException("UiLocale", string.Format(
                        "UI locale '{0}' is not supported for now.\nCurrently supported locales are: {1}.",
                            value, string.Join(", ", Strings.Instance.SupportedLocales)));
                Strings.Instance.CurrentLocale = value;
            }
        }

        #endregion Misc

        #endregion Public properties

        #region Providing default values for PropertyGrid component

        private bool ShouldSerializeNullReplacementColor() => NullReplacementColor != Color.Red;
        private void ResetNullReplacementColor() => NullReplacementColor = Color.Red;

        private bool ShouldSerializeGroupHeaderRowForeColor() => GroupHeaderRowForeColor != Color.Red;
        private void ResetGroupHeaderRowForeColor() => GroupHeaderRowForeColor = Color.Red;

        private bool ShouldSerializeGroupHeaderRowSelectionForeColor() => GroupHeaderRowSelectionForeColor != Color.Red;
        private void ResetGroupHeaderRowSelectionForeColor() => GroupHeaderRowSelectionForeColor = Color.Red;

        private bool ShouldSerializeGroupHeaderRowBackColor() => GroupHeaderRowBackColor != Color.Red;
        private void ResetGroupHeaderRowBackColor() => GroupHeaderRowBackColor = Color.Red;

        private bool ShouldSerializeGroupHeaderRowSelectionBackColor() => GroupHeaderRowSelectionBackColor != Color.Red;
        private void ResetGroupHeaderRowSelectionBackColor() => GroupHeaderRowSelectionBackColor = Color.Red;

        private bool ShouldSerializeGroupFooterRowForeColor() => GroupFooterRowForeColor != Color.Red;
        private void ResetGroupFooterRowForeColor() => GroupFooterRowForeColor = Color.Red;

        private bool ShouldSerializeGroupFooterRowSelectionForeColor() => GroupFooterRowSelectionForeColor != Color.Red;
        private void ResetGroupFooterRowSelectionForeColor() => GroupFooterRowSelectionForeColor = Color.Red;

        private bool ShouldSerializeGroupFooterRowBackColor() => GroupFooterRowBackColor != Color.Red;
        private void ResetGroupFooterRowBackColor() => GroupFooterRowBackColor = Color.Red;

        private bool ShouldSerializeGroupFooterRowSelectionBackColor() => GroupFooterRowSelectionBackColor != Color.Red;
        private void ResetGroupFooterRowSelectionBackColor() => GroupFooterRowSelectionBackColor = Color.Red;

        #endregion Providing default values for PropertyGrid component

        #region Internal and private fields/properties

        internal Font BoldFont;
        internal BrushCache BrushCache = new BrushCache();
        internal bool ignoreRowsAddedEvent = false;
        private readonly ContextMenuStrip ctxFooterMenu = new ContextMenuStrip();
        internal object[] ValuesWhichForceSeparateGroups { get; private set; }
        private GridFilterHelper gridFilterHelper;
        private bool showSandwichMenu = true;
        private bool allowColumnFilters = true;
        private bool allowHideColumns = true;

        private bool IsGroupingON => !string.IsNullOrEmpty(GroupColumnName);
        private bool IsGroupingOFF => string.IsNullOrEmpty(GroupColumnName);
        private bool IsSortingON => CurrentSortColumnIndex != NO_SORTING;

        private List<CustomRow> RegularRows => this.Rows(r => r.IsRegularRow && r.Index >= 0 && !r.IsNewRow);
        internal List<CustomRow> HeaderRows => this.Rows(r => r.IsHeaderRow);
        private List<CustomRow> FooterRows => this.Rows(r => r.IsFooterRow);
        internal DataGridViewRowCollectionEx RowsEx => Rows as DataGridViewRowCollectionEx;

        #endregion Internal and private fields/properties

        public DataGridViewEx()
        {
            RowTemplate = new CustomRow(); // IMPORTANT POINT #1
            ctxFooterMenu.ItemClicked += ctxFooterMenu_ItemClicked;
            GroupHeaderRowForeColor = ColumnHeadersDefaultCellStyle.ForeColor;
            GroupHeaderRowBackColor = ColumnHeadersDefaultCellStyle.BackColor;
            GroupHeaderRowSelectionForeColor = ColumnHeadersDefaultCellStyle.SelectionForeColor;
            GroupHeaderRowSelectionBackColor = ColumnHeadersDefaultCellStyle.SelectionBackColor;
            gridFilterHelper = new GridFilterHelper(this);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            DoubleBuffered = true;
            BoldFont = new Font(DefaultCellStyle.Font, FontStyle.Bold);
            AddGridSandwichMenu();
            Controls["gridSandwichMenu"].Visible = ShowSandwichMenu && !DesignMode && !AllSandwichMenuItemsAreHidden;
            DropDownMenuScrollWheelHandler.Enable(ShowSandwichMenu && !DesignMode && !AllSandwichMenuItemsAreHidden);
        }

        protected override DataGridViewRowCollection CreateRowsInstance()
        {
            return new DataGridViewRowCollectionEx(this); // IMPORTANT POINT #2
            // the purpose is to take control over the RemoveAt() method
        }

        private static void ClearRowsSafe(DataGridViewRowCollectionEx rows, bool destroyParentChildRelations)
        {
            // cannot use rows.Clear() - will get error "Uncommitted new row cannot be deleted" if AllowUserToAddRows == true
            int allRowsCount = rows.Count;
            for (int i = 0; i < allRowsCount; i++)
            {
                if (rows[0].IsNewRow)
                    continue;
                if (destroyParentChildRelations)
                {
                    var cr = rows[0] as CustomRow;
                    if (cr.IsHeaderRow)
                        cr.ChildRows.Clear();
                    else
                        cr.ParentGroupRow = null;
                }
                rows.NativeRemoveAt(0);
            }
        }

        protected override void OnCellMouseClick(DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && e.Button == MouseButtons.Right && (Rows[e.RowIndex] as CustomRow).IsFooterRow)
                ShowFooterAggregateMenu(Columns[e.ColumnIndex]);
            base.OnCellMouseClick(e);
        }

        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            base.OnColumnAdded(e);
            e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
        {
            base.OnColumnHeaderMouseClick(e);
            if (e.Button == MouseButtons.Left)
            {
                // Left mouse click on column header - sorts by this column
                if (!Columns[e.ColumnIndex].ExtInfo().AllowSorting)
                    return;
                CurrentSortOrder = CurrentSortOrder == SortOrder.Ascending
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
                CurrentSortColumnIndex = e.ColumnIndex;
                CustomSort();
                Invalidate();
            }
        }

        DataGridViewRow previousCurrentRow;
        protected override void OnCurrentCellChanged(EventArgs e)
        {
            base.OnCurrentCellChanged(e);
            if (CurrentRow != null && CurrentRow != previousCurrentRow)
            {
                if (previousCurrentRow != null && previousCurrentRow.Index >= 0)
                    InvalidateRow(previousCurrentRow.Index);
                previousCurrentRow = CurrentRow;
                InvalidateRow(CurrentRow.Index);
                // without InvalidateRow() - CustomRow.Paint() doesn't redraw all cells of the row
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (CurrentRow == null)
                return;
            var cr = CurrentRow as CustomRow;
            if (cr.IsHeaderRow)
            {
                if (e.KeyCode == Keys.Left && cr.IsHeaderRowExpanded)
                    cr.IsHeaderRowExpanded = false;
                else if (e.KeyCode == Keys.Right && !cr.IsHeaderRowExpanded)
                    cr.IsHeaderRowExpanded = true;
            }
            base.OnKeyDown(e);
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);
            if (e.RowIndex == -1)
            {
                // That's column header
                e.PaintBackground(e.CellBounds, false);
                e.Paint(e.CellBounds, DataGridViewPaintParts.ContentForeground);
                e.Paint(e.CellBounds, DataGridViewPaintParts.ContentBackground);
                if (CurrentSortColumnIndex == e.ColumnIndex)
                {
                    // Draw sorting glyph
                    var sortIcon = CurrentSortOrder == SortOrder.Ascending ?
                        VisualStyleElement.Header.SortArrow.SortedUp :
                        VisualStyleElement.Header.SortArrow.SortedDown;
                    var renderer = new VisualStyleRenderer(sortIcon);
                    var size = renderer.GetPartSize(e.Graphics, ThemeSizeType.Draw);
                    renderer.DrawBackground(e.Graphics,
                        new Rectangle(e.CellBounds.Right - size.Width,
                        e.CellBounds.Top, size.Width, e.CellBounds.Height));
                }
                e.Handled = true;
            }
            else
            {
                var firstVisibleColumn = Columns.GetFirstColumn(DataGridViewElementStates.Visible);
                if (e.ColumnIndex == firstVisibleColumn.Index && IsGroupingON)
                {
                    /*
                     * Which rows have offset in the first cell - depends on NonRepeatingValuesTreatment value:
                     * PlaceInSeparateGroups, MergeInOneSpecialGroup:
                     *   - all Regular and Footer rows
                     * DoNotGroup:
                     *   - only Regular rows which do have parent (group) row
                     */
                    var cr = Rows[e.RowIndex] as CustomRow;
                    if ((cr.IsRegularRow && cr.ParentGroupRow != null) || cr.IsFooterRow)
                    {
                        var isCellSelected = this[e.ColumnIndex, e.RowIndex].Selected;
                        e.PaintBackground(e.CellBounds, /*false*/isCellSelected);
                        var rectToFill = e.CellBounds;
                        rectToFill.Width = GroupMembersFirstCellHorizOffset;
                        e.Graphics.FillRectangle(BrushCache.Get(GroupHeaderRowBackColor), rectToFill);
                        // No need to use e.Graphics.DrawString() - we can just set left padding:
                        if (e.CellStyle.Padding.Left < GroupMembersFirstCellHorizOffset)
                            e.CellStyle.Padding = new Padding(GroupMembersFirstCellHorizOffset, 0, 0, 0);
                        e.PaintContent(e.CellBounds);
                        e.Handled = true;
                    }
                }
                MoreCellPainting?.Invoke(this, e);
            }
        }

        protected override void OnCellBeginEdit(DataGridViewCellCancelEventArgs e)
        {
            var cr = Rows[e.RowIndex] as CustomRow;
            if (cr.RowKind != CustomRowKindEnum.Regular)
                e.Cancel = true; // cells in header/footer rows cannot be edited
            base.OnCellBeginEdit(e);
        }

        protected override void OnEditingControlShowing(DataGridViewEditingControlShowingEventArgs e)
        {
            if (CurrentCell.ColumnIndex == 0 && IsGroupingON)
            {
                var cr = CurrentRow as CustomRow;
                if (cr.IsRegularRow && cr.ParentGroupRow != null)
                    e.Control.VisibleChanged += editingControl_VisibleChanged;
            }
            base.OnEditingControlShowing(e);
        }

        private void editingControl_VisibleChanged(object sender, EventArgs e)
        {
            if (EditingControl != null && EditingControl.Visible)
            {
                //EditingControl.BackColor = Color.Yellow;
                if (EditingControl.Parent != null)
                {
                    EditingControl.Parent.Left += GroupMembersFirstCellHorizOffset;
                    EditingControl.Parent.Width = Columns[CurrentCell.ColumnIndex].Width - GroupMembersFirstCellHorizOffset - 1;
                    //EditingControl.Parent.BackColor = Color.Magenta;
                }
                else
                {
                    // NOTE: not sure we can get here (seems EditingControl always has Panel control as its parent container)
                    EditingControl.Left = GroupMembersFirstCellHorizOffset;
                    EditingControl.Width = Columns[CurrentCell.ColumnIndex].Width - GroupMembersFirstCellHorizOffset - 1;
                }
                EditingControl.VisibleChanged -= editingControl_VisibleChanged;
            }
        }

        protected override void WndProc(ref Message m)
        {
            bool handled = false;

            // Cannot use OnMouseDown() override because I need to suppress WM_LBUTTONDOWN message (when sorting with mouse click on a column header)
            if (m.Msg == Win32.WM_LBUTTONDOWN)
            {
                // left mouse click...
                var pt = new Point(Win32.LOWORD(m.LParam), Win32.HIWORD(m.LParam));
                var hitTestInfo = HitTest(pt.X, pt.Y);
                if (hitTestInfo.ColumnIndex >= 0 && hitTestInfo.RowIndex >= 0)
                {
                    // ...on a regular cell (neither DGV column header nor DGV row header)...
                    var cr = Rows[hitTestInfo.RowIndex] as CustomRow;
                    if (cr.IsHeaderRow)
                    {
                        // ...belonging to a group header row...
                        bool insideImageRect = false;
                        var firstVisibleColumn = Columns.GetFirstColumn(DataGridViewElementStates.Visible);
                        if (Columns[hitTestInfo.ColumnIndex].Index == firstVisibleColumn.Index)
                        {
                            // ...and to the very first visible column
                            var cellRect = GetCellDisplayRectangle(hitTestInfo.ColumnIndex, hitTestInfo.RowIndex, true);
                            var imageTop = (cellRect.Height - Resources.Expanded.Height) / 2;
                            var imageRect = new Rectangle(cellRect.X + CustomRow.COLLAPSE_EXPAND_IMAGE_HORIZ_MARGIN, cellRect.Y + imageTop,
                                Resources.Expanded.Width, Resources.Expanded.Height);
                            //Console.WriteLine("{0}, {1}", imageRect, pt);
                            if (imageRect.Contains(pt))
                            {
                                // and this click is exactly on the expand/collapse (+/-) image
                                insideImageRect = true;
                                ToggleExpandCollapse(hitTestInfo.RowIndex);
                            }
                        }
                        if (!insideImageRect && Columns[GroupColumnName].ExtInfo().AllowSorting)
                        {
                            // sort by the group column
                            CurrentSortOrder = CurrentSortOrder == SortOrder.Ascending
                                ? SortOrder.Descending
                                : SortOrder.Ascending;
                            CurrentSortColumnIndex = Columns[GroupColumnName].Index;
                            CustomSort();
                            Invalidate();
                            handled = true; // suppress WM_LBUTTONDOWN message - otherwise we occasionally get unwanted cell(s) selection
                        }
                    }
                }
            }
            else if (m.Msg == Win32.WM_SETCURSOR && IsCursorOverLinkCell())
            {
                // Replace ugly cursor with a nice one :)
                Win32.SetCursor(Win32.LoadCursor(IntPtr.Zero, Win32.IDC_HAND));
                m.Result = IntPtr.Zero;
                handled = true;
            }

            if (!handled)
                base.WndProc(ref m);
        }

        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            base.OnCellFormatting(e);
            if ((Rows[e.RowIndex] as CustomRow).IsFooterRow)
            {
                var aggr = Columns[e.ColumnIndex].ExtInfo().GroupFooterAggregate;
                switch (aggr)
                {
                    case GroupFooterAggregateEnum.None:
                        e.Value = string.Empty;
                        break;
                    case GroupFooterAggregateEnum.TotalCount:
                    case GroupFooterAggregateEnum.NonEmptyCount:
                    case GroupFooterAggregateEnum.Sum:
                        e.Value = string.Format("{0}: {1}", aggr.GetDescription(), e.Value);
                        break;
                    case GroupFooterAggregateEnum.Min:
                    case GroupFooterAggregateEnum.Max:
                        e.Value = Columns[e.ColumnIndex].ExtInfo().DataType == GridColumnDataType.DateTime
                            ? string.Format("{0}: {1}", aggr, ((DateTime)e.Value).ToString(Columns[e.ColumnIndex].DefaultCellStyle.Format))
                            : string.Format("{0}: {1}", aggr, e.Value);
                        break;
                    case GroupFooterAggregateEnum.Avg:
                        e.Value = $"Avg: {e.Value:0.00}";
                        break;
                    case GroupFooterAggregateEnum.Custom:
                        e.Value = e.Value == null
                            ? string.Empty
                            : string.Format(Columns[e.ColumnIndex].ExtInfo().CustomGroupFooterValueFormatString, e.Value);
                        break;
                }
                e.FormattingApplied = true;
            }
            else
            {
                if ((e.Value == null || e.Value == DBNull.Value) && !Rows[e.RowIndex].IsNewRow)
                {
                    string nullReplacementEffective = e.CellStyle.NullValue == null || e.CellStyle.NullValue.ToString() == string.Empty
                        ? NullReplacementText
                        : e.CellStyle.NullValue.ToString();
                    if (!string.IsNullOrEmpty(nullReplacementEffective))
                    {
                        e.Value = nullReplacementEffective;
                        e.CellStyle.ForeColor = NullReplacementColor;
                        e.FormattingApplied = true;
                    }
                }
            }
            Rows[e.RowIndex].HeaderCell.ToolTipText = ShowHintForRowHeaders
                ? $"row index: {e.RowIndex}"
                : string.Empty;
        }

        protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
        {
            if (ignoreRowsAddedEvent)
                return;
            var cr = Rows[e.RowIndex] as CustomRow;
            if (cr.IsNewRow)
                return;
            if (IsGroupingON && AutoGroupOnRowSetChanged)
            {
                // grouping is ON
                ignoreRowsAddedEvent = true;
                ApplyGroupingToNewOrChangedRow(cr);
                ignoreRowsAddedEvent = false;
            }
            if (IsSortingON && AutoSortOnRowSetChanged)
            {
                // sorting is ON
                ignoreRowsAddedEvent = true;
                CustomSort(Columns[CurrentSortColumnIndex].Name, CurrentSortOrder);
                ignoreRowsAddedEvent = false;
            }
            if ((IsGroupingON && AutoGroupOnRowSetChanged) || (IsSortingON && AutoSortOnRowSetChanged))
            {
                // focus new row and bring it into view:
                BeginInvoke((Action)(() =>
                {
                    CurrentCell = cr.Cells[0]; // NOTE: doesn't work properly without BeginInvoke() - activates cell in the wrong row!
                    if (!cr.Displayed)
                        FirstDisplayedScrollingRowIndex = cr.ParentGroupRow == null ? cr.Index : cr.ParentGroupRow.Index;
                    CurrentCell.Selected = true;
                    //Console.WriteLine("CurrentCell RC: {0}, {1}", CurrentCell.RowIndex, CurrentCell.ColumnIndex);
                }));
                //Invalidate();
            }
            base.OnRowsAdded(e);
        }

        protected override void OnUserDeletingRow(DataGridViewRowCancelEventArgs e)
        {
            RowRemovalGuard(e.Row as CustomRow);
            base.OnUserDeletingRow(e);
        }

        protected override void OnUserDeletedRow(DataGridViewRowEventArgs e)
        {
            var cr = e.Row as CustomRow;
            var parentRow = cr.ParentGroupRow;
            if (parentRow != null)
                parentRow.ChildRows.Remove(cr);
            base.OnUserDeletedRow(e);
        }

        protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
        {
            BeginInvoke((Action)(() => ProcessCellEndEdit(e)));
            base.OnCellEndEdit(e);
        }

        private void ProcessCellEndEdit(DataGridViewCellEventArgs e)
        {
            var cr = this.Rows(r => r.Index == e.RowIndex).FirstOrDefault();
            if (cr.IsRegularRow)
            {
                if (IsGroupingON)
                {
                    if (GroupColumnName == Columns[e.ColumnIndex].Name ||
                        (cr.ParentGroupRow == null && AllowUserToAddRows && cr.Index == Rows.Count - 2))
                    {
                        // need to update grouping only if value was changed in the group column's cell
                        // OR in other cells of the row which was just created (in the last case - default
                        // value of the group column's cell may already match one of the existing groups)
                        ignoreRowsAddedEvent = true;
                        ApplyGroupingToNewOrChangedRow(cr);
                        ignoreRowsAddedEvent = false;
                    }
                    if (cr.ParentGroupRow != null && ShowGroupFooters != ShowGroupFootersEnum.None &&
                        Columns[e.ColumnIndex].ExtInfo().GroupFooterAggregate != GroupFooterAggregateEnum.None)
                    {
                        // value was changed in a column which has some aggregate in footer --> update this aggregate
                        var groupFooterRow = cr.ParentGroupRow.ChildRows.GetFooterRow();
                        if (groupFooterRow != null)
                            UpdateAggregatesInGroupFooterRow(groupFooterRow);
                    }
                }
                if (IsSortingON && CurrentSortColumnIndex == e.ColumnIndex)
                {
                    // need to update sorting only if value was changed in the sort column's cell
                    ignoreRowsAddedEvent = true;
                    CustomSort(Columns[CurrentSortColumnIndex].Name, CurrentSortOrder);
                    ignoreRowsAddedEvent = false;
                }
                if ((IsGroupingON && GroupColumnName == Columns[e.ColumnIndex].Name) || (IsSortingON && CurrentSortColumnIndex == e.ColumnIndex))
                {
                    // don't need BeginInvoke() here because the WHOLE method is run via BeginInvoke()
                    //BeginInvoke((Action)(() =>
                    //{
                    CurrentCell = cr.Cells[e.ColumnIndex]; // NOTE: doesn't work properly without BeginInvoke() - activates cell in the wrong ("old") row!
                    if (!cr.Displayed)
                        FirstDisplayedScrollingRowIndex = cr.ParentGroupRow == null ? cr.Index : cr.ParentGroupRow.Index;
                    //}));
                }
            }
        }

        private bool IsCursorOverLinkCell() // except header cell!
        {
            var cursorPos = PointToClient(Cursor.Position);
            var hitTestInfo = HitTest(cursorPos.X, cursorPos.Y);
            return hitTestInfo.ColumnIndex < 0 || hitTestInfo.RowIndex < 0
                ? false
                : Rows[hitTestInfo.RowIndex].Cells[hitTestInfo.ColumnIndex].GetType() == typeof(DataGridViewLinkCell);
        }

        private void AddGridSandwichMenu()
        {
            var gridMenu = new SandwichMenu();
            gridMenu.Name = "gridSandwichMenu";
            gridMenu.Left = (RowHeadersWidth - gridMenu.Width) / 2 + 1;
            gridMenu.Top = (ColumnHeadersHeight - gridMenu.Height) / 2 + 2;

            var tsiColumnsDisplay = new ToolStripMenuItem(SR("Columns display")) { Name = "tsiColumnsDisplay" };
            tsiColumnsDisplay.DropDown.Closing += (s, e) =>
            {
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                    e.Cancel = true;
                else
                    gridMenu.Close();
            };
            tsiColumnsDisplay.Visible = AllowHideColumns;
            gridMenu.CtxMenu.Items.Add(tsiColumnsDisplay);

            var tsiResetAllFilters = new ToolStripMenuItem(SR("Reset all filters")) { Name = "tsiResetAllFilters" };
            tsiResetAllFilters.Click += (s, _) => gridFilterHelper.RemoveAllColumnFilters();
            gridMenu.CtxMenu.Items.Add(tsiResetAllFilters);
            gridMenu.BeforeShow += (s, e) =>
            {
                tsiColumnsDisplay.DropDownItems.Clear();
                var tsiShowAll = new ToolStripMenuItem(SR("Show all columns")) { Name = "tsiShowAll" };
                tsiShowAll.Click += (sender, args) =>
                {
                    this.Columns(col => !col.ExtInfo().AlwaysHidden)
                        .ForEach(col => col.Visible = true);
                    tsiColumnsDisplay.DropDownItems
                        .OfType<ToolStripMenuItem>()
                        .Where(p => p != tsiShowAll)
                        .ToList()
                        .ForEach(p => p.Checked = true);
                };
                tsiColumnsDisplay.DropDownItems.Add(tsiShowAll);
                tsiColumnsDisplay.DropDownItems.Add(new ToolStripSeparator());
                tsiColumnsDisplay.DropDownItems.AddRange(this.Columns(col => !col.ExtInfo().AlwaysHidden)
                    .Select(col => new ToolStripMenuItem { Name = col.Name, Text = col.HeaderText, CheckOnClick = true, Checked = col.Visible })
                    .ToArray());

                tsiResetAllFilters.Visible = AllowColumnFilters && Rows.Count > 0;
                tsiResetAllFilters.Enabled = gridFilterHelper.AnyColumnsFiltered() && Rows.Count > 0;
            };
            tsiColumnsDisplay.DropDownItemClicked += (s, e) =>
            {
                if (e.ClickedItem.Name != "tsiShowAll")
                    Columns[e.ClickedItem.Name].Visible = !(e.ClickedItem as ToolStripMenuItem).Checked;
            };
            Controls.Add(gridMenu);
        }
        protected override void OnColumnHeadersHeightChanged(EventArgs e)
        {
            base.OnColumnHeadersHeightChanged(e);
            var gridMenu = Controls["gridSandwichMenu"];
            if (gridMenu != null)
                gridMenu.Top = (ColumnHeadersHeight - gridMenu.Height) / 2 + 2;
        }
        protected override void OnRowHeadersWidthChanged(EventArgs e)
        {
            base.OnRowHeadersWidthChanged(e);
            var gridMenu = Controls["gridSandwichMenu"];
            if (gridMenu != null)
                gridMenu.Left = (RowHeadersWidth - gridMenu.Width) / 2 + 1;
        }

        private bool AllSandwichMenuItemsAreHidden => !AllowColumnFilters && !AllowHideColumns;

        private void UpdateGridSandwichMenuVisibility(Control gridMenu)
        {
            bool show = showSandwichMenu && !DesignMode && !AllSandwichMenuItemsAreHidden;
            gridMenu.Visible = show;
            Refresh();
            DropDownMenuScrollWheelHandler.Enable(show);
        }
    }
}
