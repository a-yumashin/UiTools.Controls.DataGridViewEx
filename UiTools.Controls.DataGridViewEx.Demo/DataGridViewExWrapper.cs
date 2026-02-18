using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace UiTools.Controls.ExtendedDataGridView.Demo
{
    internal class DataGridViewExWrapper : IDataGridViewEx
    {
        // NOTE: this class implements IDataGridViewEx interface with the only purpose - not to miss any properties.
        //       So implemented events and methods have no any practical sense - however, we're obliged to implement them as well.
        //       Probably we could split IDataGridViewEx into IDataGridViewExProperties and IDataGridViewExEventsAndMethods - so
        //       that this class would implement only IDataGridViewExProperties... but not sure that this complication has real
        //       value: interface IDataGridViewEx can be useful not only in this demo app.
        private readonly DataGridViewEx dgv;

        public event EventHandler<CustomGroupHeaderCaptionNeededArgs> CustomGroupHeaderCaptionNeeded;
        public event EventHandler<CustomColumnFooterValueNeededArgs> CustomColumnFooterValueNeeded;
        public event EventHandler<ColumnFooterFormattingArgs> ColumnFooterFormatting;
        public event EventHandler SortingComplete;

        public DataGridViewExWrapper(DataGridViewEx dgv)
        {
            this.dgv = dgv;
        }

        #region Grouping - main stuff

        [Category("Grouping - main stuff")]
        public string GroupColumnName { get => dgv.GroupColumnName; }

        [DefaultValue(GroupHeaderRowCaptionFormatEnum.ColumnNameAndValueAndCount)]
        [Category("Grouping - main stuff")]
        public GroupHeaderRowCaptionFormatEnum GroupHeaderRowCaptionFormat { get => dgv.GroupHeaderRowCaptionFormat; set { dgv.GroupHeaderRowCaptionFormat = value; } }

        [DefaultValue(30)]
        [Category("Grouping - main stuff")]
        public int GroupMembersFirstCellHorizOffset { get => dgv.GroupMembersFirstCellHorizOffset; set { dgv.GroupMembersFirstCellHorizOffset = value; } }

        [DefaultValue(NonRepeatingValuesTreatmentEnum.MergeInOneSpecialGroup)]
        [Category("Grouping - main stuff")]
        public NonRepeatingValuesTreatmentEnum NonRepeatingValuesTreatment { get => dgv.NonRepeatingValuesTreatment; set { dgv.NonRepeatingValuesTreatment = value; } }

        [DefaultValue("[non-repeating values]")]
        [Category("Grouping - main stuff")]
        public string NonRepeatingValuesGroupCaption { get => dgv.NonRepeatingValuesGroupCaption; set { dgv.NonRepeatingValuesGroupCaption = value; } }

        [DefaultValue(ShowGroupFootersEnum.None)]
        [Category("Grouping - main stuff")]
        public ShowGroupFootersEnum ShowGroupFooters { get => dgv.ShowGroupFooters; set { dgv.ShowGroupFooters = value; } }

        [DefaultValue(false)]
        [Category("Grouping - main stuff")]
        public bool AutoGroupOnRowSetChanged { get => dgv.AutoGroupOnRowSetChanged; set { dgv.AutoGroupOnRowSetChanged = value; } }

        #endregion Grouping - main stuff

        #region Grouping - secondary stuff

        [Category("Grouping - secondary stuff")]
        public Color GroupHeaderRowForeColor { get => dgv.GroupHeaderRowForeColor; set { dgv.GroupHeaderRowForeColor = value; } }
        [Category("Grouping - secondary stuff")]
        public Color GroupHeaderRowSelectionForeColor { get => dgv.GroupHeaderRowSelectionForeColor; set { dgv.GroupHeaderRowSelectionForeColor = value; } }
        [Category("Grouping - secondary stuff")]
        public Color GroupHeaderRowBackColor { get => dgv.GroupHeaderRowBackColor; set { dgv.GroupHeaderRowBackColor = value; } }
        [Category("Grouping - secondary stuff")]
        public Color GroupHeaderRowSelectionBackColor { get => dgv.GroupHeaderRowSelectionBackColor; set { dgv.GroupHeaderRowSelectionBackColor = value; } }

        [Category("Grouping - secondary stuff")]
        public Color GroupFooterRowForeColor { get => dgv.GroupFooterRowForeColor; set { dgv.GroupFooterRowForeColor = value; } }
        [Category("Grouping - secondary stuff")]
        public Color GroupFooterRowSelectionForeColor { get => dgv.GroupFooterRowSelectionForeColor; set { dgv.GroupFooterRowSelectionForeColor = value; } }
        [Category("Grouping - secondary stuff")]
        public Color GroupFooterRowBackColor { get => dgv.GroupFooterRowBackColor; set { dgv.GroupFooterRowBackColor = value; } }
        [Category("Grouping - secondary stuff")]
        public Color GroupFooterRowSelectionBackColor { get => dgv.GroupFooterRowSelectionBackColor; set { dgv.GroupFooterRowSelectionBackColor = value; } }

        [DefaultValue(false)]
        [Category("Grouping - secondary stuff")]
        public bool AvgGroupFooterAggregateTreatEmptyAsZero { get => dgv.AvgGroupFooterAggregateTreatEmptyAsZero; set { dgv.AvgGroupFooterAggregateTreatEmptyAsZero = value; } }

        [DefaultValue(true)]
        [Category("Grouping - secondary stuff")]
        public bool ShowHintForGroupFooters { get => dgv.ShowHintForGroupFooters; set { dgv.ShowHintForGroupFooters = value; } }

        #endregion Grouping - secondary stuff

        #region Sorting

        [DefaultValue(SortOrder.None)]
        [Category("Sorting")]
        public SortOrder CurrentSortOrder { get => dgv.CurrentSortOrder; }

        [DefaultValue(DataGridViewEx.NO_SORTING)]
        [Category("Sorting")]
        public int CurrentSortColumnIndex { get => dgv.CurrentSortColumnIndex; }

        [DefaultValue(false)]
        [Category("Sorting")]
        public bool AutoSortOnRowSetChanged { get => dgv.AutoSortOnRowSetChanged; set { dgv.AutoSortOnRowSetChanged = value; } }

        #endregion Sorting

        #region Misc

        [DefaultValue("(NULL)")]
        public string NullReplacementText { get => dgv.NullReplacementText; set { dgv.NullReplacementText = value; } }
        
        public Color NullReplacementColor { get => dgv.NullReplacementColor; set { dgv.NullReplacementColor = value; } }
        
        [DefaultValue(true)]
        public bool ShowHintForRowHeaders { get => dgv.ShowHintForRowHeaders; set { dgv.ShowHintForRowHeaders = value; } }

        [DefaultValue(true)]
        public bool ShowSandwichMenu { get => dgv.ShowSandwichMenu; set { dgv.ShowSandwichMenu = value; } }

        [DefaultValue(true)]
        public bool AllowColumnFilters { get => dgv.AllowColumnFilters; set { dgv.AllowColumnFilters = value; } }

        [DefaultValue(true)]
        public bool AllowHideColumns { get => dgv.AllowHideColumns; set { dgv.AllowHideColumns = value; } }

        [DefaultValue("en")]
        public string UiLocale { get => dgv.UiLocale; set { dgv.UiLocale = value; } }

        #endregion Misc

        public void GroupByColumn(string colName, object[] valuesWhichForceSeparateGroups = null) {}
        public void UpdateAggregatesInColumnFooter(int columnIndex) {}
        public void UpdateAggregatesInGroupFooterRow(CustomRow groupFooterRow) {}
        public void UpdateAggregateInColumnFooter(CustomRow groupFooterRow, int columnIndex) {}
        public void ExpandAllGroups() {}
        public void CollapseAllGroups() {}
        public void CustomSort(string columnName, SortOrder sortOrder) { }

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
    }
}
