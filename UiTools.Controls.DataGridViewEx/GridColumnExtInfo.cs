using System;
using System.Windows.Forms;

namespace UiTools.Controls.ExtendedDataGridView
{
    public class GridColumnExtInfo
    {
        private readonly DataGridViewColumn col;
        private GridColumnDataType dataType = GridColumnDataType.Text;

        public GridColumnExtInfo(DataGridViewColumn col)
        {
            this.col = col;
        }

        public GridColumnDataType DataType
        {
            get => dataType;
            set
            {
                dataType = value;
                switch (dataType)
                {
                    case GridColumnDataType.Text:
                        col.ValueType = typeof(string);
                        break;
                    case GridColumnDataType.Integer:
                        col.ValueType = typeof(int);
                        break;
                    case GridColumnDataType.Decimal:
                        col.ValueType = typeof(decimal);
                        break;
                    case GridColumnDataType.DateTime:
                        col.ValueType = typeof(DateTime);
                        break;
                    case GridColumnDataType.Boolean:
                        col.ValueType = typeof(bool);
                        break;
                }
            }
        }

        public string DateTimeFormatForExcel { get; set; }
        public GroupFooterAggregateEnum GroupFooterAggregate { get; set; } = GroupFooterAggregateEnum.None;
        public bool SupportsCustomGroupFooterAggregate { get; set; }
        public string CustomGroupFooterValueFormatString { get; set; }
        public string CustomGroupFooterAggregateDisplayName { get; set; }
        public string CustomGroupFooterAggregateHint { get; set; }
        public HorizontalAlignment GroupFooterAggregateAlignment { get; set; } = HorizontalAlignment.Left;
        public bool AllowChangeGroupFooterAggregate { get; set; } = true;
        public bool AllowGrouping { get; set; } = true;
        public bool AllowSorting { get; set; } = true;
        public bool AlwaysHidden { get; set; }
        public bool NeedsAutoSizing { get; set; }
        public bool AllowWrapping { get; set; }
        public AllowedColumnFiltersEnum AllowedColumnFilters { get; set; } = AllowedColumnFiltersEnum.RegularOnly;
        public bool Mandatory { get; set; }

        // Convenience properties:
        public bool IsNumeric => DataType == GridColumnDataType.Integer || DataType == GridColumnDataType.Decimal;
        public bool IsDistinctFilterAllowed => AllowedColumnFilters == AllowedColumnFiltersEnum.RegularAndDistinct;
    }
}
