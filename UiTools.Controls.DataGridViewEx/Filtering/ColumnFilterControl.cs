using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static UiTools.Controls.ExtendedDataGridView.CommonStuff;

namespace UiTools.Controls.ExtendedDataGridView
{
    internal partial class ColumnFilterControl : UserControl
    {
        public string FirstValue
        {
            get
            {
                if (txtValue1.Mask == string.Empty)
                    return txtValue1.Text;
                var text = txtValue1.Text.Trim();
                return text == "." || text == ","
                    ? string.Empty
                    : text;
            }
            set { txtValue1.Text = value; }
        }

        public string SecondValue
        {
            get
            {
                if (txtValue2.Mask == string.Empty)
                    return txtValue2.Text;
                var text = txtValue2.Text.Trim();
                return text == "." || text == ","
                    ? string.Empty
                    : text;
            }
            set { txtValue2.Text = value; }
        }

        public event EventHandler ApplyClicked;
        public event EventHandler ResetClicked;
        public event EventHandler CancelClicked;

        public bool IsDropDownShowing { get; private set; }

        public ColumnFilterControl()
        {
            InitializeComponent();
            txtValue1.AllowPromptAsInput = false;
            txtValue1.AsciiOnly = true;
            txtValue2.AllowPromptAsInput = false;
            txtValue2.AsciiOnly = true;
            txtValue1.TextChanged += textValue_TextChanged;
            txtValue2.TextChanged += textValue_TextChanged;
            txtValue1.KeyPress += textValue_KeyPress;
            txtValue2.KeyPress += textValue_KeyPress;
            txtValue1.LostFocus += textValue_LostFocus;
            txtValue2.LostFocus += textValue_LostFocus;
            ShowTwoValues = false;
            label2.Text = SR("Or filter by distinct values:");
            lnkReset.Text = SR("Reset");
            lnkCancel.Text = SR("Close");
            lnkCancel.Left = Width - lnkCancel.Width;
            lnkApply.Text = SR("Apply");
            lnkReset.Left = lnkApply.Right + 10;
            label1.Text = SR("Filter:");
            lstDistinctValues.CheckOnClick = true;
            lstDistinctValues.ItemCheck += lstDistinctValues_ItemCheck;
            lstDistinctValues.EnabledChanged += (s, e) => lstDistinctValues.BackColor = lstDistinctValues.Enabled
                ? Color.White : Color.FromArgb(230, 230, 230);
            lstDistinctValues.MouseMove += (s, e) => ShowToolTip(e.Location);
            cboFilterType.SelectedIndexChanged += cboFilterType_SelectedIndexChanged;
            cboFilterType.DropDown += (o, args) => IsDropDownShowing = true;
            cboFilterType.DropDownClosed += (o, args) => IsDropDownShowing = false;
            lnkApply.LinkClicked += lnkApply_LinkClicked;
            lnkReset.LinkClicked += lnkReset_LinkClicked;
            lnkCancel.LinkClicked += lnkCancel_LinkClicked;
            ApplyAndResetButtonsEnabled = false;
        }
        
        private void textValue_LostFocus(object sender, EventArgs e)
        {
            var mtb = sender as MaskedTextBox;
            if (mtb.Mask == "#####.###")
                mtb.MoveIntegerPartToTheRight();
            else if (mtb.Mask == "#####")
                mtb.MoveIntegerNumberToTheRight();
        }

        private void textValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            var mtb = sender as MaskedTextBox;
            if (mtb.Mask == "#####.###" && (e.KeyChar == '.' || e.KeyChar == ','))
            {
                mtb.MoveIntegerPartToTheRight();
                mtb.SelectionStart = mtb.Mask.IndexOf('.') + 1;
            }
        }

        private int hoveredItemIndex = -1;
        private void ShowToolTip(Point mouseLocation)
        {
            if (hoveredItemIndex != lstDistinctValues.IndexFromPoint(mouseLocation))
            {
                hoveredItemIndex = lstDistinctValues.IndexFromPoint(lstDistinctValues.PointToClient(MousePosition));
                if (hoveredItemIndex > -1)
                {
                    var itemText = lstDistinctValues.Items[hoveredItemIndex].ToString();
                    var itemTextLength = TextRenderer.MeasureText(itemText, lstDistinctValues.Font).Width;
                    var checkBoxSize = CheckBoxRenderer.GetGlyphSize(Graphics.FromHwnd(lstDistinctValues.Handle), System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
                    if (itemTextLength + checkBoxSize.Width + 5 > lstDistinctValues.ClientSize.Width)
                        toolTip1.SetToolTip(lstDistinctValues, lstDistinctValues.Items[hoveredItemIndex].ToString());
                    else
                        toolTip1.SetToolTip(lstDistinctValues, "");
                }
                else
                    toolTip1.SetToolTip(lstDistinctValues, "");
            }
        }

        private void textValue_TextChanged(object sender, EventArgs e)
        {
            DistinctFilterEnabled = ShowTwoValues
                ? FirstValue.Length == 0 && SecondValue.Length == 0
                : FirstValue.Length == 0;
            ApplyAndResetButtonsEnabled = true;
        }

        private void cboFilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var filterType = cboFilterType.SelectedItem as GridColumnFilterType;
            ShowTwoValues = filterType.TypeCode == KnownFilterTypes.Between || filterType.TypeCode == KnownFilterTypes.NotBetween;
            toolTip1.SetToolTip(txtValue1, filterType.TypeCode == KnownFilterTypes.RegExpression
                ? "for regexps - use .NET flavor (compatible with Perl 5), with inline options if needed"
                : "");
            txtValue1.Focus();
            ApplyAndResetButtonsEnabled = true;
        }

        private void lnkCancel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CancelClicked?.Invoke(lnkCancel, EventArgs.Empty);
        }

        private void lnkReset_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ResetClicked?.Invoke(lnkReset, EventArgs.Empty);
        }

        private void lnkApply_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplyClicked?.Invoke(lnkApply, EventArgs.Empty);
        }

        public KnownFilterTypes SelectedFilterType
        {
            get
            {
                if (label1.Enabled && (FirstValue.Length > 0 || (txtValue2.Visible && SecondValue.Length > 0)))
                    return (cboFilterType.SelectedItem as GridColumnFilterType).TypeCode;
                if (lstDistinctValues.GetItemCheckState(0) == CheckState.Checked)
                    return KnownFilterTypes.None;
                return KnownFilterTypes.DistinctValues;
            }
            set
            {
                if (value == KnownFilterTypes.None)
                {
                    // Both filters must be available
                    RegularFilterEnabled = true;
                    DistinctFilterEnabled = true;
                    if (lstDistinctValues.Items.Count > 0) // list is empty when DistinctFilterVisible == false
                        lstDistinctValues.SetItemCheckState(0, CheckState.Checked);
                    ApplyAndResetButtonsEnabled = false;
                }
                else if (value == KnownFilterTypes.DistinctValues)
                {
                    // Only distinct values filter must be available
                    RegularFilterEnabled = false;
                    DistinctFilterEnabled = true;
                    ApplyAndResetButtonsEnabled = true;
                }
                else
                {
                    // Only regular filter must be available
                    RegularFilterEnabled = true;
                    DistinctFilterEnabled = false;
                    if (lstDistinctValues.Items.Count > 0) // list is empty when DistinctFilterVisible == false
                        lstDistinctValues.SetItemCheckState(0, CheckState.Checked);
                    ApplyAndResetButtonsEnabled = true;
                    cboFilterType.SelectedItem = cboFilterType.Items.Cast<GridColumnFilterType>().FirstOrDefault(ft => ft.TypeCode == value);
                }
            }
        }

        private bool RegularFilterEnabled
        {
            set
            {
                label1.Enabled = value;
                cboFilterType.Enabled = value;
                txtValue1.Enabled = value;
                txtValue2.Enabled = value;
            }
        }
        private bool DistinctFilterEnabled
        {
            set
            {
                label2.Enabled = value;
                lstDistinctValues.Enabled = value;
            }
        }
        private bool ApplyAndResetButtonsEnabled
        {
            set
            {
                lnkApply.Enabled = value;
                lnkReset.Enabled = value;
            }
        }

        private bool freezeItemCheckEvents = false;
        private void lstDistinctValues_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (lstDistinctValues.Items.Count <= 1)
                return;
            // Mimic Excel filter behavior:
            if (e.Index == 0)
            {
                if (!freezeItemCheckEvents)
                {
                    freezeItemCheckEvents = true;
                    if (e.CurrentValue == CheckState.Indeterminate)
                        lstDistinctValues.SetItemChecked(0, true);
                    else
                        lstDistinctValues.SetItemChecked(0, e.CurrentValue != CheckState.Checked);
                    Enumerable.Range(1, lstDistinctValues.Items.Count - 1).ToList()
                        .ForEach(idx => lstDistinctValues.SetItemChecked(idx, lstDistinctValues.GetItemCheckState(0) == CheckState.Checked));
                    freezeItemCheckEvents = false;
                }
            }
            else
            {
                new UIThreadRunner(this).DelayedRunSafe(100, () =>
                {
                    if (!freezeItemCheckEvents)
                    {
                        freezeItemCheckEvents = true;
                        int checkedItemsCount = lstDistinctValues.CheckedItems.Count;
                        if (lstDistinctValues.GetItemCheckState(0) != CheckState.Unchecked)
                            checkedItemsCount--;
                        //Console.WriteLine(checkedItemsCount);
                        if (checkedItemsCount == 0)
                            lstDistinctValues.SetItemChecked(0, false);
                        else if (checkedItemsCount == lstDistinctValues.Items.Count - 1)
                            lstDistinctValues.SetItemChecked(0, true);
                        else
                            lstDistinctValues.SetItemCheckState(0, CheckState.Indeterminate);
                        freezeItemCheckEvents = false;
                        if (Visible)
                            RegularFilterEnabled = SelectedFilterType == KnownFilterTypes.None;
                    }
                });
            }
            ApplyAndResetButtonsEnabled = true;
        }

        public IEnumerable<CellContents> CheckedDistinctValues
        {
            get { return lstDistinctValues.CheckedItems.Cast<CellContents>().Skip(1); } // to exclude "(Select all)" item
            set
            {
                // Check those lstDistinctValues.Items which can be found in distinctValues:
                Enumerable.Range(1, lstDistinctValues.Items.Count - 1).ToList() // starting from 1 - to exclude "(Select all)" item
                    .ForEach(idx => {
                        var listItemValue = (lstDistinctValues.Items[idx] as CellContents).Value;
                        //Console.WriteLine("{0} -- {1} -- {2}", cc, listItemValue, listItemValue.Equals(cc.Value));
                        lstDistinctValues.SetItemChecked(idx, value.Cast<CellContents>()
                            .Any(cc => object.Equals(listItemValue, cc.Value)));
                    });
            }
        }

        public void PrepareForShow(GridColumnDataType columnDataType)
        {
            if (columnDataType == GridColumnDataType.DateTime)
            {
                label1.Text = SR("Date filter");
                SetFilterTypesForDateTime();
                txtValue1.Mask = "00/00/0000";
                txtValue2.Mask = "00/00/0000";
                toolTip1.SetToolTip(txtValue1, "dd.MM.yyyy");
                toolTip1.SetToolTip(txtValue2, "dd.MM.yyyy");
            }
            else if (columnDataType == GridColumnDataType.Decimal || columnDataType == GridColumnDataType.Integer)
            {
                label1.Text = SR("Number filter");
                SetFilterTypesForIntegerOrDecimal();
                txtValue1.Mask = columnDataType == GridColumnDataType.Decimal
                    ? "#####.###"
                    : "#####";
                txtValue2.Mask = columnDataType == GridColumnDataType.Decimal
                    ? "#####.###"
                    : "#####";
                toolTip1.SetToolTip(txtValue1, "");
                toolTip1.SetToolTip(txtValue2, "");
            }
            else // GridColumnDataType.Text
            {
                label1.Text = SR("Text filter");
                SetFilterTypesForText();
                txtValue1.Mask = "";
                txtValue2.Mask = "";
                toolTip1.SetToolTip(txtValue1, "");
                toolTip1.SetToolTip(txtValue2, "");
            }
            txtValue1.Clear();
            txtValue2.Clear();
        }
        public void PrepareForShow(GridColumnDataType columnDataType, IEnumerable<CellContents> distinctColumnValues)
        {
            PrepareForShow(columnDataType);

            lstDistinctValues.Items.Clear();
            lstDistinctValues.Items.Add(new CellContents(SR("(Select all)"), SR("(Select all)")));
            lstDistinctValues.Items.AddRange(distinctColumnValues.ToArray());
        }

        private void SetFilterTypesForDateTime()
        {
            cboFilterType.Items.Clear();
            cboFilterType.Items.AddRange(new GridColumnFilterType[] {
                new GridColumnFilterType(KnownFilterTypes.Equals, SR("Equals")),
                new GridColumnFilterType(KnownFilterTypes.NotEquals, SR("Does not equal")),
                new GridColumnFilterType(KnownFilterTypes.Greater, SR("After")),
                new GridColumnFilterType(KnownFilterTypes.GreaterOrEqual, SR("After or equals")),
                new GridColumnFilterType(KnownFilterTypes.Less, SR("Before")),
                new GridColumnFilterType(KnownFilterTypes.LessOrEqual, SR("Before or equals")),
                new GridColumnFilterType(KnownFilterTypes.Between, SR("Between")),
                new GridColumnFilterType(KnownFilterTypes.NotBetween, SR("Not between"))
            });
            cboFilterType.SelectedIndex = 0;
        }

        private void SetFilterTypesForIntegerOrDecimal()
        {
            cboFilterType.Items.Clear();
            cboFilterType.Items.AddRange(new GridColumnFilterType[] {
                new GridColumnFilterType(KnownFilterTypes.Equals, SR("Equals")),
                new GridColumnFilterType(KnownFilterTypes.NotEquals, SR("Does not equal")),
                new GridColumnFilterType(KnownFilterTypes.Greater, SR("Greater than")),
                new GridColumnFilterType(KnownFilterTypes.GreaterOrEqual, SR("Greater than or equal to")),
                new GridColumnFilterType(KnownFilterTypes.Less, SR("Less than")),
                new GridColumnFilterType(KnownFilterTypes.LessOrEqual, SR("Less than or equal to")),
                new GridColumnFilterType(KnownFilterTypes.Between, SR("Between")),
                new GridColumnFilterType(KnownFilterTypes.NotBetween, SR("Not between"))
            });
            cboFilterType.SelectedIndex = 0;
        }

        private void SetFilterTypesForText()
        {
            cboFilterType.Items.Clear();
            cboFilterType.Items.AddRange(new GridColumnFilterType[] {
                new GridColumnFilterType(KnownFilterTypes.Equals, SR("Equals")),
                new GridColumnFilterType(KnownFilterTypes.NotEquals, SR("Does not equal")),
                new GridColumnFilterType(KnownFilterTypes.Contains, SR("Contains")),
                new GridColumnFilterType(KnownFilterTypes.NotContains, SR("Does not contain")),
                new GridColumnFilterType(KnownFilterTypes.StartsWith, SR("Starts with")),
                new GridColumnFilterType(KnownFilterTypes.NotStartsWith, SR("Does not start with")),
                new GridColumnFilterType(KnownFilterTypes.EndsWith, SR("Ends with")),
                new GridColumnFilterType(KnownFilterTypes.NotEndsWith, SR("Does not end with")),
                new GridColumnFilterType(KnownFilterTypes.RegExpression, SR("(regular expression)"))
            });
            cboFilterType.SelectedIndex = 0;
        }

        private bool _distinctFilterVisible = true;
        public void HideDistinctFilter()
        {
            _distinctFilterVisible = false;
            label2.Visible = false;
            lstDistinctValues.Visible = false;
            //lnkApply.Top = lnkReset.Top = lnkCancel.Top = ShowTwoValues ? txtValue2.Bottom + 9 : txtValue2.Top; // no need - Anchor works well
            Height = txtValue2.Bottom + 9 + lnkApply.Height + 9;
        }

        private bool _showTwoValues = true;
        private bool ShowTwoValues
        {
            get { return _showTwoValues; }
            set
            {
                _showTwoValues = value;
                txtValue2.Visible = value;
                if (_distinctFilterVisible)
                {
                    label2.Top = value ? txtValue2.Bottom + 9 : txtValue2.Top;
                    lstDistinctValues.Top = label2.Bottom + 9;
                    lstDistinctValues.Height = lnkApply.Top - 9 - lstDistinctValues.Top;
                }
            }
        }

        private class GridColumnFilterType
        {
            public GridColumnFilterType(KnownFilterTypes typeCode, string typeName)
            {
                TypeCode = typeCode;
                TypeName = typeName;
            }

            public KnownFilterTypes TypeCode { get; set; }
            public string TypeName { get; set; }

            public override string ToString()
            {
                return TypeName;
            }
        }
    }

    public enum KnownFilterTypes
    {
        None = 0,
        Equals,
        NotEquals,
        Contains,
        NotContains,
        StartsWith,
        NotStartsWith,
        EndsWith,
        NotEndsWith,
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual,
        Between,
        NotBetween,
        RegExpression,
        DistinctValues
    }
}
