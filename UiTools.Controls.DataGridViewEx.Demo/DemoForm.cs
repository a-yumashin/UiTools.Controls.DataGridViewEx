using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace UiTools.Controls.ExtendedDataGridView.Demo
{
    public partial class DemoForm : Form
    {
        public DemoForm()
        {
            InitializeComponent();
            dataGridViewEx1.RowHeadersWidth = 30;
            dataGridViewEx1.MultiSelect = false;
            dataGridViewEx1.CurrentCellChanged += dataGridViewEx1_CurrentCellChanged;
            dataGridViewEx1.SortingComplete += dataGridViewEx1_SortingComplete;
            selectedCellBorderPen = new Pen(Color.Cyan, 2);
        }

        private void dataGridViewEx1_SortingComplete(object sender, EventArgs e)
        {
            pgrExProperties.Refresh();
        }

        private void dataGridViewEx1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridViewEx1.CurrentCell != null)
            {
                var currentRow = dataGridViewEx1.CurrentCell.OwningRow as CustomRow;
                cmdDeleteRow.Enabled = currentRow.IsRegularRow;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            //dataGridViewEx1.AllowUserToAddRows = false;

            dataGridViewEx1.ShowGroupFooters = ShowGroupFootersEnum.ForAllGroupsExceptSingleMemberOnes;
            dataGridViewEx1.NonRepeatingValuesTreatment = NonRepeatingValuesTreatmentEnum.MergeInOneSpecialGroup;
            dataGridViewEx1.EnableHeadersVisualStyles = false;
            //dataGridViewEx1.UiLocale = "ru";

            gridExporter1.Grid = dataGridViewEx1; // << must be done AFTER dataGridViewEx1.UiLocale is set
            gridExporter1.WebView2 = webView21;
            gridExporter1.DefaultFileName = "Exported grid data";
            gridExporter1.HeaderText = "(HEADER TEXT)";
            gridExporter1.FooterText = "(FOOTER TEXT)";
            gridExporter1.SubHeaderText = "(SUBHEADER TEXT)";
            gridExporter1.Left = dataGridViewEx1.Right - gridExporter1.Width;

            dataGridViewEx1.GroupHeaderRowForeColor = Color.DarkBlue;
            dataGridViewEx1.GroupHeaderRowBackColor = Color.LightGray;
            dataGridViewEx1.GroupHeaderRowSelectionBackColor = Color.RoyalBlue;
            dataGridViewEx1.GroupHeaderRowSelectionForeColor = Color.White;

            dataGridViewEx1.GroupFooterRowBackColor = Color.LightYellow;
            dataGridViewEx1.GroupFooterRowForeColor = Color.DarkRed;
            dataGridViewEx1.GroupFooterRowSelectionBackColor = Color.DarkRed;
            dataGridViewEx1.GroupFooterRowSelectionForeColor = Color.Yellow;

            dataGridViewEx1.CustomGroupHeaderCaptionNeeded += dataGridViewEx1_CustomRowHeaderCaptionNeeded;
            dataGridViewEx1.CustomColumnFooterValueNeeded += dataGridViewEx1_CustomColumnFooterValueNeeded;
            dataGridViewEx1.ColumnFooterFormatting += dataGridViewEx1_ColumnFooterFormatting;

            dataGridViewEx1.Columns.Add("id", "Id");
            dataGridViewEx1.Columns.Add("code", "Code");
            dataGridViewEx1.Columns.Add("name", "Name");
            dataGridViewEx1.Columns.Add("value", "Value");
            dataGridViewEx1.Columns.Add("datetime", "DateTime");
            dataGridViewEx1.Columns.Add("flag", "Flag");

            dataGridViewEx1.Columns["id"].ExtInfo().DataType = GridColumnDataType.Integer;
            dataGridViewEx1.Columns["id"].Width = 80;

            dataGridViewEx1.Columns["code"].ExtInfo().AllowedColumnFilters = AllowedColumnFiltersEnum.RegularAndDistinct;
            dataGridViewEx1.Columns["code"].Width = 100;

            dataGridViewEx1.Columns["name"].ExtInfo().GroupFooterAggregate = GroupFooterAggregateEnum.NonEmptyCount;
            dataGridViewEx1.Columns["name"].ExtInfo().AllowedColumnFilters = AllowedColumnFiltersEnum.RegularAndDistinct;
            dataGridViewEx1.Columns["name"].Width = 120;

            dataGridViewEx1.Columns["value"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewEx1.Columns["value"].Width = 160;
            dataGridViewEx1.Columns["value"].ExtInfo().DataType = GridColumnDataType.Decimal;
            dataGridViewEx1.Columns["value"].ExtInfo().GroupFooterAggregate = GroupFooterAggregateEnum.Avg;

            dataGridViewEx1.Columns["datetime"].ExtInfo().DataType = GridColumnDataType.DateTime;
            dataGridViewEx1.Columns["datetime"].ExtInfo().GroupFooterAggregate = GroupFooterAggregateEnum.Max;
            // Adjust DateTime format for Excel cells if so desired; for example:
            //dataGridViewEx1.Columns["datetime"].ExtInfo().DateTimeFormatForExcel = "mm/dd/yyyy hh:nn"; // "дд.мм.гггг чч:мм" - for Russian Excel
            dataGridViewEx1.Columns["datetime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dataGridViewEx1.Columns["datetime"].Width = 210;

            dataGridViewEx1.Columns["flag"].ExtInfo().DataType = GridColumnDataType.Boolean;
            dataGridViewEx1.Columns["flag"].ExtInfo().AllowedColumnFilters = AllowedColumnFiltersEnum.RegularAndDistinct;
            dataGridViewEx1.Columns["flag"].Width = 70;

            chkSupportCustomAggregateFunction.Checked = true;
            chkRenameCustomMenuItem.Checked = true;
            chkAddHintToCustomMenuItem.Checked = true;
            chkUseCustomFooterColors.Checked = true;
            chkRightAlignFooter.Checked = true;
            chkAllowChangeGroupFooterAggregate.Checked = true;

            pgrExProperties.SelectedObject = new DataGridViewExWrapper(dataGridViewEx1);
            pgrExProperties.PropertyValueChanged += pgrExProperties_PropertyValueChanged;

            PopulateGridWithRandomData();
            dataGridViewEx1.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);

            dataGridViewEx1.GroupByColumn("code", new object[] { "hhh" });
            dataGridViewEx1.CustomSort("code", SortOrder.Ascending);

            cboGroupColumn.Items.Add("(none)");
            cboGroupColumn.Items.AddRange(dataGridViewEx1.Columns().Select(c => c.HeaderText).ToArray());
            cboGroupColumn.Text = "code";

            cboSortColumn.Items.Add("(none)");
            cboSortColumn.Items.AddRange(dataGridViewEx1.Columns().Select(c => c.HeaderText).ToArray());
            cboSortColumn.Text = "code";

            cboSortOrder.Items.Add("ASC");
            cboSortOrder.Items.Add("DESC");
            cboSortOrder.SelectedIndex = 0;

            dataGridViewEx1.MoreCellPainting += DataGridViewEx1_MoreCellPainting;
        }

        private readonly Pen selectedCellBorderPen; // NOTE: If initialized right here, the form gets distorted!!! Go figure...
        private void DataGridViewEx1_MoreCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex < 0)
                return;
            var dgv = sender as DataGridView;
            // Perform default drawing
            e.Paint(e.CellBounds, DataGridViewPaintParts.All);
            if (dgv.CurrentCell != null && dgv.CurrentCell.ColumnIndex == e.ColumnIndex && dgv.CurrentCell.RowIndex == e.RowIndex)
            {
                // Draw border for the current cell
                var rect = new Rectangle(e.CellBounds.X + 1, e.CellBounds.Y + 1, e.CellBounds.Width - 3, e.CellBounds.Height - 3);
                e.Graphics.DrawRectangle(selectedCellBorderPen, rect);
            }
            e.Handled = true;
        }

        private void pgrExProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            dataGridViewEx1.Refresh();
        }

        private void dataGridViewEx1_CustomColumnFooterValueNeeded(object sender, CustomColumnFooterValueNeededArgs e)
        {
            if (e.Column.Name == "value")
            {
                int oddsSum = e.GroupFooterRow.ParentGroupRow.ChildRows
                    .Where(r => r.IsRegularRow)
                    .Select(r => (int)r.Cells["value"].GetValueSafe(0))
                    .Where(v => v % 2 == 1)
                    .Sum();
                e.ColumnFooterValue = oddsSum;
                e.ColumnFooterHint = "sum of odd values";
                e.Handled = true;
            }
        }

        private void dataGridViewEx1_ColumnFooterFormatting(object sender, ColumnFooterFormattingArgs e)
        {
            if (e.Column.Name == "value")
            {
                if (chkUseCustomFooterColors.Checked)
                {
                    e.ColumnFooterBackColor = Color.PaleGreen;
                    e.ColumnFooterForeColor = Color.Green;
                    e.ColumnFooterSelectionBackColor = Color.Green;
                    e.ColumnFooterSelectionForeColor = Color.Lime;
                }
                e.Handled = true;
            }
        }

        private void dataGridViewEx1_CustomRowHeaderCaptionNeeded(object sender, CustomGroupHeaderCaptionNeededArgs e)
        {
            var value = e.HeaderRow.Cells[dataGridViewEx1.GroupColumnName].FormattedValue?.ToString();
            e.Caption = e.HeaderRow.IsMergedGroupHeaderRow
                ? "Rows with non-repeating values"
                : $"Rows with [{dataGridViewEx1.GroupColumnName}] = {(value == string.Empty ? "[empty string]" : value)}";
            e.Handled = true;
        }

        private void PopulateGridWithRandomData()
        {
            dataGridViewEx1.AutoGroupOnRowSetChanged = false; // recommended before populating
            dataGridViewEx1.AutoSortOnRowSetChanged = false;  // grid with initial data!

            const int N = 10;
            int startIndex = 0;
            int i;

            dataGridViewEx1.GroupByColumn("");
            dataGridViewEx1.CustomSort("");
            dataGridViewEx1.Rows.Clear();

            for (i = startIndex; i < startIndex + N; i++)
            {
                dataGridViewEx1.Rows.Add(i, GetRandomCode(), GetRandomName(), GetRandomValue(), GetRandomDateTime(), GetRandomFlag());
            }

            dataGridViewEx1.Rows.Add(i, "111", "11111", 123, GetRandomDateTime(), GetRandomFlag());
            dataGridViewEx1.Rows.Add(i + 1, "222", "22222", 456, null, GetRandomFlag());
            dataGridViewEx1.Rows.Add(i + 2, "333", "33333", 789, GetRandomDateTime(), GetRandomFlag());

            dataGridViewEx1.Rows.Add(i + 3, null, "10000", 100, GetRandomDateTime(), GetRandomFlag());
            dataGridViewEx1.Rows.Add(i + 4, "200", null, 200, GetRandomDateTime(), GetRandomFlag());
            dataGridViewEx1.Rows.Add(i + 5, "300", null, 300, GetRandomDateTime(), GetRandomFlag());
            dataGridViewEx1.Rows.Add(i + 6, "400", "40000", null, GetRandomDateTime(), GetRandomFlag());

            startIndex = dataGridViewEx1.Rows.Count;
            for (i = startIndex; i < startIndex + N; i++)
            {
                dataGridViewEx1.Rows.Add(i, GetRandomCode(), GetRandomName(), GetRandomValue(), GetRandomDateTime(), GetRandomFlag());
            }

            dataGridViewEx1.Rows.Add(i, "444", "44444", 321, GetRandomDateTime(), GetRandomFlag());
            dataGridViewEx1.Rows.Add(i + 1, "555", "55555", 654, GetRandomDateTime(), GetRandomFlag());
            dataGridViewEx1.Rows.Add(i + 2, "666", "66666", 987, GetRandomDateTime(), GetRandomFlag());

            dataGridViewEx1.Rows.Add(i + 3, "", "50000", 500, GetRandomDateTime(), GetRandomFlag());
            dataGridViewEx1.Rows.Add(i + 4, "", "60000", 600, GetRandomDateTime(), GetRandomFlag());
            dataGridViewEx1.Rows.Add(i + 5, "700", "", 700, GetRandomDateTime(), GetRandomFlag());
            dataGridViewEx1.Rows.Add(i + 6, "800", "80000", "", "");

            dataGridViewEx1.AutoGroupOnRowSetChanged = true;
            dataGridViewEx1.AutoSortOnRowSetChanged = true;
        }

        private readonly string[] knownCodeValues = new string[]
        {
            "aaa", "bbb", "ccc", "ddd", "eee", "fff", "ggg", "hhh", "iii", "jjj", "kkk", "lll", "mmm", "nnn", "ooo"
        };
        private string GetRandomCode()
        {
            int arrayIndex = GetCryptographicRandom(0, knownCodeValues.Length - 1);
            return knownCodeValues[arrayIndex];
        }

        private readonly string[] knownNameValues = new string[]
        {
            "AAAAA", "BBBBB", "CCCCC", "DDDDD", "EEEEE", "FFFFF", "GGGGG", "HHHHH", "IIIII", "JJJJJ", "KKKKK", "LLLLL", "MMMMM", "NNNNN", "OOOOO"
        };
        private string GetRandomName()
        {
            int arrayIndex = GetCryptographicRandom(0, knownNameValues.Length - 1);
            return knownNameValues[arrayIndex];
        }

        private int GetRandomValue()
        {
            return GetCryptographicRandom(0, 1000);
        }

        private DateTime GetRandomDateTime()
        {
            return DateTime.Now.AddMinutes(-GetCryptographicRandom(0, 1000000));
        }

        private bool GetRandomFlag()
        {
            return 1 == GetCryptographicRandom(0, 1);
        }

        private static int GetCryptographicRandom(int minValue, int maxValue)
        {
            if (minValue > maxValue || minValue < 0)
                throw new ArgumentException("Invalid value range");

            byte[] randomBytes = new byte[sizeof(uint)];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);

                uint randomUint = BitConverter.ToUInt32(randomBytes, 0);
                double rangeSize = maxValue - minValue + 1;
                return (int)(randomUint % rangeSize) + minValue;
            }
        }

        private void cmdGroup_Click(object sender, EventArgs e)
        {
            dataGridViewEx1.Columns["value"].ExtInfo().GroupFooterAggregate = chkSupportCustomAggregateFunction.Checked
                ? GroupFooterAggregateEnum.Custom
                : GroupFooterAggregateEnum.Avg;
            var groupColumnName = cboGroupColumn.SelectedIndex == 0 ? "" : cboGroupColumn.Text;
            var valuesWhichForceSeparateGroups = groupColumnName == "Code"
                ? new object[] { "hhh" }
                : null;
            dataGridViewEx1.GroupByColumn(groupColumnName, valuesWhichForceSeparateGroups);
            pgrExProperties.Refresh();
        }

        private void cmdSort_Click(object sender, EventArgs e)
        {
            dataGridViewEx1.CustomSort(cboSortColumn.SelectedIndex == 0 ? string.Empty : cboSortColumn.Text,
                cboSortOrder.SelectedIndex == 0 ? SortOrder.Ascending : SortOrder.Descending);
        }

        private void cmdDeleteRow_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, $"Delete row {dataGridViewEx1.CurrentCell.OwningRow}?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            dataGridViewEx1.Rows.RemoveAt(dataGridViewEx1.CurrentCell.OwningRow.Index);
        }

        private void cmdAddRow_Click(object sender, EventArgs e)
        {
            //dataGridViewEx1.Rows.Add(999, "900", "90000", 900);
            dataGridViewEx1.Rows.Add(999, "kkk", "90000", 900);
        }

        private void chkSupportCustomAggregateFunction_CheckedChanged(object sender, EventArgs e)
        {
            chkRenameCustomMenuItem.Enabled = chkSupportCustomAggregateFunction.Checked;
            chkAddHintToCustomMenuItem.Enabled = chkSupportCustomAggregateFunction.Checked;
            dataGridViewEx1.Columns["value"].ExtInfo().SupportsCustomGroupFooterAggregate = chkSupportCustomAggregateFunction.Checked;
            if (!chkSupportCustomAggregateFunction.Checked)
            {
                dataGridViewEx1.Columns["value"].ExtInfo().GroupFooterAggregate = GroupFooterAggregateEnum.Avg;
                dataGridViewEx1.Columns["value"].ExtInfo().CustomGroupFooterValueFormatString = string.Empty;
            }
            else
            {
                dataGridViewEx1.Columns["value"].ExtInfo().GroupFooterAggregate = GroupFooterAggregateEnum.Custom;
                dataGridViewEx1.Columns["value"].ExtInfo().CustomGroupFooterValueFormatString = "Odds sum: {0}";
            }
            dataGridViewEx1.UpdateAggregatesInColumnFooter(dataGridViewEx1.Columns["value"].Index);
        }

        private void chkRenameCustomMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            dataGridViewEx1.Columns["value"].ExtInfo().CustomGroupFooterAggregateDisplayName = chkRenameCustomMenuItem.Checked
                ? "Odds sum"
                : "Custom";
        }

        private void chkAddHintToCustomMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            dataGridViewEx1.Columns["value"].ExtInfo().CustomGroupFooterAggregateHint = chkAddHintToCustomMenuItem.Checked
                ? "sum of odd values"
                : string.Empty;
        }

        private void chkRightAlignFooter_CheckedChanged(object sender, EventArgs e)
        {
            dataGridViewEx1.Columns["value"].ExtInfo().GroupFooterAggregateAlignment = chkRightAlignFooter.Checked
                ? HorizontalAlignment.Right
                : HorizontalAlignment.Left;
            dataGridViewEx1.UpdateAggregatesInColumnFooter(dataGridViewEx1.Columns["value"].Index);
        }

        private void chkAllowChangeGroupFooterAggregate_CheckedChanged(object sender, EventArgs e)
        {
            dataGridViewEx1.Columns["value"].ExtInfo().AllowChangeGroupFooterAggregate = chkAllowChangeGroupFooterAggregate.Checked;
        }

        private void chkUseCustomFooterColors_CheckedChanged(object sender, EventArgs e)
        {
            dataGridViewEx1.Rows(r => r.IsFooterRow).ForEach(r => dataGridViewEx1.UpdateAggregatesInGroupFooterRow(r));
        }

        private void lnkExpandAllGroups_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            dataGridViewEx1.ExpandAllGroups();
        }

        private void lnkCollapseAllGroups_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            dataGridViewEx1.CollapseAllGroups();
        }

        private void picInfoSupportCustomAggr_Click(object sender, EventArgs e)
        {
            ShowInfo("Check to show (and automatically select) 'Custom' menu item in the footer cell context menu. When this menu item " +
                "is selected - the CustomColumnFooterValueNeeded event is fired, and the code of this demo form handles it providing " +
                "value of a custom aggregate function for the [Value] column (this function calculates sum of odd values in each group).");
        }

        private void picInfoRenameCustom_Click(object sender, EventArgs e)
        {
            ShowInfo("Check to rename menu item from 'Custom' to 'Odds sum' in the footer cell context menu.\r\n" +
                "Has sense only when the 'Support custom aggregate function' checkbox is checked.");
        }

        private void picInfoAddHintToCustom_Click(object sender, EventArgs e)
        {
            ShowInfo("Check to add hint (tooltip) 'sum of odd values' to 'Custom' menu item in the footer cell context menu.\r\n" +
                "Has sense only when the 'Support custom aggregate function' checkbox is checked.");
        }

        private void picInfoAllowChangeAggr_Click(object sender, EventArgs e)
        {
            ShowInfo("Check to show footer cell context menu when user right-clicks footer cell. This menu allows user to select " +
                "aggregate function for the corresponding column in the run time.");
        }

        private void picInfoAlignFooter_Click(object sender, EventArgs e)
        {
            ShowInfo("Check to right-align footer cells contents in the corresponding column.");
        }

        private void pictureBoxEx4_Click(object sender, EventArgs e)
        {
            ShowInfo("Check to use custom colors in the footer cells of the corresponding column. When this checkbox is checked - " +
                "the ColumnFooterFormatting event is fired, and the code of this demo form handles it providing custom colors which " +
                "differ from the default ones defined in the following grid's properties:\r\nGroupFooterRowBackColor,\r\n" +
                "GroupFooterRowForeColor,\r\nGroupFooterRowSelectionBackColor,\r\nGroupFooterRowSelectionForeColor.");
        }

        private void ShowInfo(string message)
        {
            MessageBox.Show(this, message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cmdPopulateGrid_Click(object sender, EventArgs e)
        {
            PopulateGridWithRandomData();
            pgrExProperties.Refresh();
        }
    }
}
