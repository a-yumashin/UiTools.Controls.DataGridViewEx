using System.Reflection;
using System.Windows.Forms;

namespace UiTools.Controls.ExtendedDataGridView
{
    internal partial class GridFilterMenu : ContextMenuStrip
    {
        public GridFilterMenu()
        {
            InitializeComponent();
            ShowCheckMargin = false;
            ShowImageMargin = false;
            RemoveRightSideEmptySpace();
        }

        public void SetHostedControl(ColumnFilterControl filterCtl)
        {
            Items.Clear();
            Items.Add(new ToolStripControlHost(filterCtl)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = false
            });
        }

        private void RemoveRightSideEmptySpace()
        {
            // Inspired by https://stackoverflow.com/questions/23724662/how-to-remove-right-side-empty-space-in-toolstripmenuitem
            var padding = new Padding(0, 0, -12, 0);
            //typeof(ContextMenuStrip).BaseType.GetField("ArrowPadding", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, padding);
            // NOTE: ^^^^^ seems it's not necessary; besides, as this is *static* field, it affects SandwichMenu as well - arrow is lost ((
            typeof(ContextMenuStrip).BaseType.GetField("scaledArrowPadding", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, padding);
        }
    }
}
