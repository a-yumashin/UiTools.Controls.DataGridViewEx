using System;
using System.Collections.Generic;
using System.Linq;

namespace UiTools.Controls.ExtendedDataGridView
{
    internal static class AggregateHelper
    {
        internal static int Count(IEnumerable<CustomRow> regularRows)
        {
            return regularRows.Count();
        }

        internal static int NonEmptyCount(IEnumerable<CustomRow> regularRows, int columnIndex)
        {
            return GetNonEmptyValues(regularRows, columnIndex).Count();
        }

        internal static object Sum(IEnumerable<CustomRow> regularRows, int columnIndex, GridColumnDataType columnDataType)
        {
            var nonEmptyValues = GetNonEmptyValues(regularRows, columnIndex);
            try
            {
                if (columnDataType == GridColumnDataType.Integer)
                    return nonEmptyValues.Select(v => Convert.ToInt32(v)).Sum();
                else if (columnDataType == GridColumnDataType.Decimal)
                    return nonEmptyValues.Select(v => Convert.ToDecimal(v)).Sum();
                else
                    return "n/a";
            }
            catch
            {
                return "###";
            }
        }

        internal static object Min(IEnumerable<CustomRow> regularRows, int columnIndex, GridColumnDataType columnDataType)
        {
            var nonEmptyValues = GetNonEmptyValues(regularRows, columnIndex);
            try
            {
                if (columnDataType == GridColumnDataType.Integer)
                    return nonEmptyValues.Select(v => Convert.ToInt32(v)).Min();
                else if (columnDataType == GridColumnDataType.Decimal)
                    return nonEmptyValues.Select(v => Convert.ToDecimal(v)).Min();
                else if (columnDataType == GridColumnDataType.DateTime)
                    return nonEmptyValues.Select(v => Convert.ToDateTime(v)).Min();
                else
                    return "n/a";
            }
            catch
            {
                return "###";
            }
        }

        internal static object Max(IEnumerable<CustomRow> regularRows, int columnIndex, GridColumnDataType columnDataType)
        {
            var nonEmptyValues = GetNonEmptyValues(regularRows, columnIndex);
            try
            {
                if (columnDataType == GridColumnDataType.Integer)
                    return nonEmptyValues.Select(v => Convert.ToInt32(v)).Max();
                else if (columnDataType == GridColumnDataType.Decimal)
                    return nonEmptyValues.Select(v => Convert.ToDecimal(v)).Max();
                else if (columnDataType == GridColumnDataType.DateTime)
                    return nonEmptyValues.Select(v => Convert.ToDateTime(v)).Max();
                else
                    return "n/a";
            }
            catch
            {
                return "###";
            }
        }

        internal static object Avg(IEnumerable<CustomRow> regularRows, int columnIndex, GridColumnDataType columnDataType,
            bool avgGroupFooterAggregateTreatEmptyAsZero)
        {
            var nonEmptyValues = avgGroupFooterAggregateTreatEmptyAsZero
                ? GetValuesWithEmptyAsZero(regularRows, columnIndex)
                : GetNonEmptyValues(regularRows, columnIndex);
            try
            {
                if (columnDataType == GridColumnDataType.Integer)
                    return nonEmptyValues.Select(v => Convert.ToInt32(v)).Average();
                else if (columnDataType == GridColumnDataType.Decimal)
                    return nonEmptyValues.Select(v => Convert.ToDecimal(v)).Average();
                else
                    return "n/a";
            }
            catch
            {
                return "###";
            }
        }

        private static IEnumerable<object> GetNonEmptyValues(IEnumerable<CustomRow> regularRows, int columnIndex)
        {
            return regularRows
                .Where(r => r.Cells[columnIndex].Value != null && r.Cells[columnIndex].Value.ToString() != string.Empty)
                .Select(r => r.Cells[columnIndex].Value);
        }

        private static IEnumerable<object> GetValuesWithEmptyAsZero(IEnumerable<CustomRow> regularRows, int columnIndex)
        {
            return regularRows
                .Select(r => r.Cells[columnIndex].Value != null && r.Cells[columnIndex].Value.ToString() != string.Empty
                                ? r.Cells[columnIndex].Value
                                : 0);
        }
    }
}
