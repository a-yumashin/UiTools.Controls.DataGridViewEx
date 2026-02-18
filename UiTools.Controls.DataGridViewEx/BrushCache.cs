using System;
using System.Collections.Generic;
using System.Drawing;

namespace UiTools.Controls.ExtendedDataGridView
{
    internal class BrushCache : IDisposable
    {
        private Dictionary<int, Brush> innerDict = new Dictionary<int, Brush>();

        public Brush Get(Color color)
        {
            var brushKey = color.ToArgb();
            if (innerDict.ContainsKey(brushKey))
                return innerDict[brushKey];
            var brush = new SolidBrush(color);
            innerDict.Add(brushKey, brush);
            return brush;
        }

        public void Remove(Color color)
        {
            var brushKey = color.ToArgb();
            if (innerDict.ContainsKey(brushKey))
                innerDict.Remove(brushKey);
        }

        public void Clear()
        {
            innerDict.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
