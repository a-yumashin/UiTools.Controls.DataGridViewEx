using System;
using System.Drawing;
using System.Windows.Forms;

namespace UiTools.Controls.ExtendedDataGridView
{
    public interface IDataGridViewEx
    {
        bool AutoGroupOnRowSetChanged { get; set; }
        bool AutoSortOnRowSetChanged { get; set; }
        bool AvgGroupFooterAggregateTreatEmptyAsZero { get; set; }
        int CurrentSortColumnIndex { get; }
        SortOrder CurrentSortOrder { get; }
        string GroupColumnName { get; }
        Color GroupFooterRowBackColor { get; set; }
        Color GroupFooterRowForeColor { get; set; }
        Color GroupFooterRowSelectionBackColor { get; set; }
        Color GroupFooterRowSelectionForeColor { get; set; }
        Color GroupHeaderRowBackColor { get; set; }
        GroupHeaderRowCaptionFormatEnum GroupHeaderRowCaptionFormat { get; set; }
        Color GroupHeaderRowForeColor { get; set; }
        Color GroupHeaderRowSelectionBackColor { get; set; }
        Color GroupHeaderRowSelectionForeColor { get; set; }
        int GroupMembersFirstCellHorizOffset { get; set; }
        string NonRepeatingValuesGroupCaption { get; set; }
        NonRepeatingValuesTreatmentEnum NonRepeatingValuesTreatment { get; set; }
        Color NullReplacementColor { get; set; }
        string NullReplacementText { get; set; }
        ShowGroupFootersEnum ShowGroupFooters { get; set; }
        bool ShowHintForGroupFooters { get; set; }
        bool ShowHintForRowHeaders { get; set; }
        bool ShowSandwichMenu { get; set; }
        bool AllowColumnFilters { get; set; }
        bool AllowHideColumns { get; set; }
        string UiLocale { get; set; }

        void CustomSort(string columnName, SortOrder sortOrder);
        void GroupByColumn(string colName, object[] valuesWhichForceSeparateGroups = null);
        void UpdateAggregatesInColumnFooter(int columnIndex);
        void UpdateAggregatesInGroupFooterRow(CustomRow groupFooterRow);
        void UpdateAggregateInColumnFooter(CustomRow groupFooterRow, int columnIndex);
        void ExpandAllGroups();
        void CollapseAllGroups();

        event EventHandler<CustomGroupHeaderCaptionNeededArgs> CustomGroupHeaderCaptionNeeded;
        event EventHandler<CustomColumnFooterValueNeededArgs> CustomColumnFooterValueNeeded;
        event EventHandler<ColumnFooterFormattingArgs> ColumnFooterFormatting;
        event EventHandler SortingComplete;
    }
}