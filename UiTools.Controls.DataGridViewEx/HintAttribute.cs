using System;

namespace UiTools.Controls.ExtendedDataGridView
{
    [AttributeUsage(AttributeTargets.All)]
    internal class HintAttribute : Attribute
    {
        private readonly string hint;

        public HintAttribute(string hint)
        {
            this.hint = hint;
        }

        public string Hint { get => hint; }
    }
}
