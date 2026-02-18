using System.Collections.Generic;

namespace UiTools.Controls.ExtendedDataGridView
{
    internal class RowsByGroupDictionary
    {
        // Introduced this class only because stock Dictionary<T,U> doesn't allow NULL keys
        private Dictionary<object, List<CustomRow>> nonNullKeys = new Dictionary<object, List<CustomRow>>();
        private List<CustomRow> rowsWithNullKey = new List<CustomRow>();

        public void Add(object key, List<CustomRow> value)
        {
            if (key == null)
                rowsWithNullKey.AddRange(value);
            else
                nonNullKeys.Add(key, value);
        }

        public List<object> Keys
        {
            get
            {
                var list = new List<object>();
                list.AddRange(nonNullKeys.Keys);
                if (rowsWithNullKey.Count > 0)
                    list.Add(null);
                return list;
            }
        }

        public List<CustomRow> this[object key]
        {
            get
            {
                return key == null
                    ? rowsWithNullKey
                    : nonNullKeys[key];
            }
        }
    }
}
