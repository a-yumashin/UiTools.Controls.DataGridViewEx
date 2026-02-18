using System.ComponentModel;

namespace UiTools.Controls.ExtendedDataGridView
{
    public enum GroupFooterAggregateEnum
    {
        None = 0,
        [Description("Count(*)")]
        [Hint("all values count")]
        TotalCount,
        [Description("Count")]
        [Hint("non-empty values count")]
        NonEmptyCount,
        Sum,
        Min,
        Max,
        Avg,
        //[Hint("custom aggregate (provided via CustomColumnFooterValueNeeded event handler)")]
        Custom
    }
}
