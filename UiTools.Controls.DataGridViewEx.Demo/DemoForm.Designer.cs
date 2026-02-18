using UiTools.Controls.ExtendedDataGridView;
using System.Windows.Forms;

namespace UiTools.Controls.ExtendedDataGridView.Demo
{
    partial class DemoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.cboGroupColumn = new System.Windows.Forms.ComboBox();
            this.cmdGroup = new System.Windows.Forms.Button();
            this.cmdSort = new System.Windows.Forms.Button();
            this.cboSortColumn = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboSortOrder = new System.Windows.Forms.ComboBox();
            this.cmdDeleteRow = new System.Windows.Forms.Button();
            this.cmdAddRow = new System.Windows.Forms.Button();
            this.pgrExProperties = new System.Windows.Forms.PropertyGrid();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkRightAlignFooter = new System.Windows.Forms.CheckBox();
            this.chkAllowChangeGroupFooterAggregate = new System.Windows.Forms.CheckBox();
            this.chkUseCustomFooterColors = new System.Windows.Forms.CheckBox();
            this.chkAddHintToCustomMenuItem = new System.Windows.Forms.CheckBox();
            this.chkRenameCustomMenuItem = new System.Windows.Forms.CheckBox();
            this.chkSupportCustomAggregateFunction = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdPopulateGrid = new System.Windows.Forms.Button();
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.gridExporter1 = new UiTools.Controls.ExtendedDataGridView.GridExporter();
            this.dataGridViewEx1 = new UiTools.Controls.ExtendedDataGridView.DataGridViewEx();
            this.lnkCollapseAllGroups = new UiTools.Controls.ExtendedDataGridView.Demo.LinkLabelEx();
            this.lnkExpandAllGroups = new UiTools.Controls.ExtendedDataGridView.Demo.LinkLabelEx();
            this.picInfoFooterColors = new UiTools.Controls.ExtendedDataGridView.Demo.PictureBoxEx();
            this.picInfoAlignFooter = new UiTools.Controls.ExtendedDataGridView.Demo.PictureBoxEx();
            this.picInfoAllowChangeAggr = new UiTools.Controls.ExtendedDataGridView.Demo.PictureBoxEx();
            this.picInfoAddHintToCustom = new UiTools.Controls.ExtendedDataGridView.Demo.PictureBoxEx();
            this.picInfoRenameCustom = new UiTools.Controls.ExtendedDataGridView.Demo.PictureBoxEx();
            this.picInfoSupportCustomAggr = new UiTools.Controls.ExtendedDataGridView.Demo.PictureBoxEx();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEx1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInfoFooterColors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInfoAlignFooter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInfoAllowChangeAggr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInfoAddHintToCustom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInfoRenameCustom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInfoSupportCustomAggr)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Group by column:";
            // 
            // cboGroupColumn
            // 
            this.cboGroupColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGroupColumn.FormattingEnabled = true;
            this.cboGroupColumn.Location = new System.Drawing.Point(148, 9);
            this.cboGroupColumn.Margin = new System.Windows.Forms.Padding(2);
            this.cboGroupColumn.Name = "cboGroupColumn";
            this.cboGroupColumn.Size = new System.Drawing.Size(73, 28);
            this.cboGroupColumn.TabIndex = 1;
            // 
            // cmdGroup
            // 
            this.cmdGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cmdGroup.Location = new System.Drawing.Point(222, 8);
            this.cmdGroup.Margin = new System.Windows.Forms.Padding(2);
            this.cmdGroup.Name = "cmdGroup";
            this.cmdGroup.Size = new System.Drawing.Size(61, 30);
            this.cmdGroup.TabIndex = 2;
            this.cmdGroup.Text = "Group";
            this.cmdGroup.UseVisualStyleBackColor = true;
            this.cmdGroup.Click += new System.EventHandler(this.cmdGroup_Click);
            // 
            // cmdSort
            // 
            this.cmdSort.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cmdSort.Location = new System.Drawing.Point(607, 7);
            this.cmdSort.Margin = new System.Windows.Forms.Padding(2);
            this.cmdSort.Name = "cmdSort";
            this.cmdSort.Size = new System.Drawing.Size(61, 30);
            this.cmdSort.TabIndex = 6;
            this.cmdSort.Text = "Sort";
            this.cmdSort.UseVisualStyleBackColor = true;
            this.cmdSort.Click += new System.EventHandler(this.cmdSort_Click);
            // 
            // cboSortColumn
            // 
            this.cboSortColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSortColumn.FormattingEnabled = true;
            this.cboSortColumn.Location = new System.Drawing.Point(455, 8);
            this.cboSortColumn.Margin = new System.Windows.Forms.Padding(2);
            this.cboSortColumn.Name = "cboSortColumn";
            this.cboSortColumn.Size = new System.Drawing.Size(73, 28);
            this.cboSortColumn.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(320, 13);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 20);
            this.label5.TabIndex = 3;
            this.label5.Text = "Sort by column:";
            // 
            // cboSortOrder
            // 
            this.cboSortOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSortOrder.FormattingEnabled = true;
            this.cboSortOrder.Location = new System.Drawing.Point(530, 8);
            this.cboSortOrder.Margin = new System.Windows.Forms.Padding(2);
            this.cboSortOrder.Name = "cboSortOrder";
            this.cboSortOrder.Size = new System.Drawing.Size(75, 28);
            this.cboSortOrder.TabIndex = 5;
            // 
            // cmdDeleteRow
            // 
            this.cmdDeleteRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdDeleteRow.ForeColor = System.Drawing.Color.Red;
            this.cmdDeleteRow.Location = new System.Drawing.Point(670, 738);
            this.cmdDeleteRow.Margin = new System.Windows.Forms.Padding(2);
            this.cmdDeleteRow.Name = "cmdDeleteRow";
            this.cmdDeleteRow.Size = new System.Drawing.Size(146, 30);
            this.cmdDeleteRow.TabIndex = 13;
            this.cmdDeleteRow.Text = "Delete current row";
            this.cmdDeleteRow.UseVisualStyleBackColor = true;
            this.cmdDeleteRow.Click += new System.EventHandler(this.cmdDeleteRow_Click);
            // 
            // cmdAddRow
            // 
            this.cmdAddRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAddRow.Location = new System.Drawing.Point(550, 738);
            this.cmdAddRow.Margin = new System.Windows.Forms.Padding(2);
            this.cmdAddRow.Name = "cmdAddRow";
            this.cmdAddRow.Size = new System.Drawing.Size(115, 30);
            this.cmdAddRow.TabIndex = 12;
            this.cmdAddRow.Text = "Add new row";
            this.cmdAddRow.UseVisualStyleBackColor = true;
            this.cmdAddRow.Click += new System.EventHandler(this.cmdAddRow_Click);
            // 
            // pgrExProperties
            // 
            this.pgrExProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgrExProperties.HelpVisible = false;
            this.pgrExProperties.Location = new System.Drawing.Point(827, 43);
            this.pgrExProperties.Margin = new System.Windows.Forms.Padding(2);
            this.pgrExProperties.Name = "pgrExProperties";
            this.pgrExProperties.Size = new System.Drawing.Size(732, 725);
            this.pgrExProperties.TabIndex = 15;
            this.pgrExProperties.ToolbarVisible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.picInfoFooterColors);
            this.groupBox1.Controls.Add(this.picInfoAlignFooter);
            this.groupBox1.Controls.Add(this.picInfoAllowChangeAggr);
            this.groupBox1.Controls.Add(this.picInfoAddHintToCustom);
            this.groupBox1.Controls.Add(this.picInfoRenameCustom);
            this.groupBox1.Controls.Add(this.picInfoSupportCustomAggr);
            this.groupBox1.Controls.Add(this.chkRightAlignFooter);
            this.groupBox1.Controls.Add(this.chkAllowChangeGroupFooterAggregate);
            this.groupBox1.Controls.Add(this.chkUseCustomFooterColors);
            this.groupBox1.Controls.Add(this.chkAddHintToCustomMenuItem);
            this.groupBox1.Controls.Add(this.chkRenameCustomMenuItem);
            this.groupBox1.Controls.Add(this.chkSupportCustomAggregateFunction);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(13, 47);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(672, 122);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "[Value] column footer:";
            // 
            // chkRightAlignFooter
            // 
            this.chkRightAlignFooter.AutoSize = true;
            this.chkRightAlignFooter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkRightAlignFooter.Location = new System.Drawing.Point(328, 56);
            this.chkRightAlignFooter.Margin = new System.Windows.Forms.Padding(2);
            this.chkRightAlignFooter.Name = "chkRightAlignFooter";
            this.chkRightAlignFooter.Size = new System.Drawing.Size(249, 24);
            this.chkRightAlignFooter.TabIndex = 4;
            this.chkRightAlignFooter.Text = "Align footer contents to the right";
            this.chkRightAlignFooter.UseVisualStyleBackColor = true;
            this.chkRightAlignFooter.CheckedChanged += new System.EventHandler(this.chkRightAlignFooter_CheckedChanged);
            // 
            // chkAllowChangeGroupFooterAggregate
            // 
            this.chkAllowChangeGroupFooterAggregate.AutoSize = true;
            this.chkAllowChangeGroupFooterAggregate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkAllowChangeGroupFooterAggregate.Location = new System.Drawing.Point(328, 27);
            this.chkAllowChangeGroupFooterAggregate.Margin = new System.Windows.Forms.Padding(2);
            this.chkAllowChangeGroupFooterAggregate.Name = "chkAllowChangeGroupFooterAggregate";
            this.chkAllowChangeGroupFooterAggregate.Size = new System.Drawing.Size(301, 24);
            this.chkAllowChangeGroupFooterAggregate.TabIndex = 3;
            this.chkAllowChangeGroupFooterAggregate.Text = "Allow user to change aggregate function";
            this.chkAllowChangeGroupFooterAggregate.UseVisualStyleBackColor = true;
            this.chkAllowChangeGroupFooterAggregate.CheckedChanged += new System.EventHandler(this.chkAllowChangeGroupFooterAggregate_CheckedChanged);
            // 
            // chkUseCustomFooterColors
            // 
            this.chkUseCustomFooterColors.AutoSize = true;
            this.chkUseCustomFooterColors.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkUseCustomFooterColors.Location = new System.Drawing.Point(328, 85);
            this.chkUseCustomFooterColors.Margin = new System.Windows.Forms.Padding(2);
            this.chkUseCustomFooterColors.Name = "chkUseCustomFooterColors";
            this.chkUseCustomFooterColors.Size = new System.Drawing.Size(279, 24);
            this.chkUseCustomFooterColors.TabIndex = 5;
            this.chkUseCustomFooterColors.Text = "Use custom colors for footer contents";
            this.chkUseCustomFooterColors.UseVisualStyleBackColor = true;
            this.chkUseCustomFooterColors.CheckedChanged += new System.EventHandler(this.chkUseCustomFooterColors_CheckedChanged);
            // 
            // chkAddHintToCustomMenuItem
            // 
            this.chkAddHintToCustomMenuItem.AutoSize = true;
            this.chkAddHintToCustomMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkAddHintToCustomMenuItem.Location = new System.Drawing.Point(33, 85);
            this.chkAddHintToCustomMenuItem.Margin = new System.Windows.Forms.Padding(2);
            this.chkAddHintToCustomMenuItem.Name = "chkAddHintToCustomMenuItem";
            this.chkAddHintToCustomMenuItem.Size = new System.Drawing.Size(241, 24);
            this.chkAddHintToCustomMenuItem.TabIndex = 2;
            this.chkAddHintToCustomMenuItem.Text = "Add hint to \'Custom\' menu item";
            this.chkAddHintToCustomMenuItem.UseVisualStyleBackColor = true;
            this.chkAddHintToCustomMenuItem.CheckedChanged += new System.EventHandler(this.chkAddHintToCustomMenuItem_CheckedChanged);
            // 
            // chkRenameCustomMenuItem
            // 
            this.chkRenameCustomMenuItem.AutoSize = true;
            this.chkRenameCustomMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkRenameCustomMenuItem.Location = new System.Drawing.Point(33, 56);
            this.chkRenameCustomMenuItem.Margin = new System.Windows.Forms.Padding(2);
            this.chkRenameCustomMenuItem.Name = "chkRenameCustomMenuItem";
            this.chkRenameCustomMenuItem.Size = new System.Drawing.Size(220, 24);
            this.chkRenameCustomMenuItem.TabIndex = 1;
            this.chkRenameCustomMenuItem.Text = "Rename \'Custom\' menu item";
            this.chkRenameCustomMenuItem.UseVisualStyleBackColor = true;
            this.chkRenameCustomMenuItem.CheckedChanged += new System.EventHandler(this.chkRenameCustomMenuItem_CheckedChanged);
            // 
            // chkSupportCustomAggregateFunction
            // 
            this.chkSupportCustomAggregateFunction.AutoSize = true;
            this.chkSupportCustomAggregateFunction.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkSupportCustomAggregateFunction.Location = new System.Drawing.Point(9, 27);
            this.chkSupportCustomAggregateFunction.Margin = new System.Windows.Forms.Padding(2);
            this.chkSupportCustomAggregateFunction.Name = "chkSupportCustomAggregateFunction";
            this.chkSupportCustomAggregateFunction.Size = new System.Drawing.Size(267, 24);
            this.chkSupportCustomAggregateFunction.TabIndex = 0;
            this.chkSupportCustomAggregateFunction.Text = "Support custom aggregate function";
            this.chkSupportCustomAggregateFunction.UseVisualStyleBackColor = true;
            this.chkSupportCustomAggregateFunction.CheckedChanged += new System.EventHandler(this.chkSupportCustomAggregateFunction_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(823, 13);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(201, 20);
            this.label2.TabIndex = 14;
            this.label2.Text = "IDataGridViewEx members:";
            // 
            // cmdPopulateGrid
            // 
            this.cmdPopulateGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdPopulateGrid.Location = new System.Drawing.Point(13, 738);
            this.cmdPopulateGrid.Margin = new System.Windows.Forms.Padding(2);
            this.cmdPopulateGrid.Name = "cmdPopulateGrid";
            this.cmdPopulateGrid.Size = new System.Drawing.Size(236, 30);
            this.cmdPopulateGrid.TabIndex = 11;
            this.cmdPopulateGrid.Text = "Populate grid with random data";
            this.cmdPopulateGrid.UseVisualStyleBackColor = true;
            this.cmdPopulateGrid.Click += new System.EventHandler(this.cmdPopulateGrid_Click);
            // 
            // webView21
            // 
            this.webView21.AllowExternalDrop = false;
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView21.Location = new System.Drawing.Point(1312, 10);
            this.webView21.Margin = new System.Windows.Forms.Padding(2);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(116, 28);
            this.webView21.TabIndex = 17;
            this.webView21.TabStop = false;
            this.webView21.Visible = false;
            this.webView21.ZoomFactor = 1D;
            // 
            // gridExporter1
            // 
            this.gridExporter1.Location = new System.Drawing.Point(498, 170);
            this.gridExporter1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.gridExporter1.Name = "gridExporter1";
            this.gridExporter1.Size = new System.Drawing.Size(188, 33);
            this.gridExporter1.TabIndex = 16;
            // 
            // dataGridViewEx1
            // 
            this.dataGridViewEx1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewEx1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewEx1.GroupFooterRowBackColor = System.Drawing.Color.LightGray;
            this.dataGridViewEx1.GroupFooterRowForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGridViewEx1.GroupFooterRowSelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.dataGridViewEx1.GroupFooterRowSelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridViewEx1.GroupHeaderRowBackColor = System.Drawing.SystemColors.Control;
            this.dataGridViewEx1.GroupHeaderRowForeColor = System.Drawing.SystemColors.WindowText;
            this.dataGridViewEx1.GroupHeaderRowSelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.dataGridViewEx1.GroupHeaderRowSelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridViewEx1.Location = new System.Drawing.Point(13, 204);
            this.dataGridViewEx1.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridViewEx1.Name = "dataGridViewEx1";
            this.dataGridViewEx1.NonRepeatingValuesTreatment = UiTools.Controls.ExtendedDataGridView.NonRepeatingValuesTreatmentEnum.DoNotGroup;
            this.dataGridViewEx1.RowHeadersWidth = 62;
            this.dataGridViewEx1.RowTemplate.Height = 28;
            this.dataGridViewEx1.Size = new System.Drawing.Size(803, 521);
            this.dataGridViewEx1.TabIndex = 10;
            // 
            // lnkCollapseAllGroups
            // 
            this.lnkCollapseAllGroups.AutoSize = true;
            this.lnkCollapseAllGroups.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkCollapseAllGroups.Location = new System.Drawing.Point(143, 175);
            this.lnkCollapseAllGroups.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lnkCollapseAllGroups.Name = "lnkCollapseAllGroups";
            this.lnkCollapseAllGroups.Size = new System.Drawing.Size(136, 20);
            this.lnkCollapseAllGroups.TabIndex = 9;
            this.lnkCollapseAllGroups.TabStop = true;
            this.lnkCollapseAllGroups.Text = "Collapse all groups";
            this.lnkCollapseAllGroups.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCollapseAllGroups_LinkClicked);
            // 
            // lnkExpandAllGroups
            // 
            this.lnkExpandAllGroups.AutoSize = true;
            this.lnkExpandAllGroups.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkExpandAllGroups.Location = new System.Drawing.Point(10, 175);
            this.lnkExpandAllGroups.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lnkExpandAllGroups.Name = "lnkExpandAllGroups";
            this.lnkExpandAllGroups.Size = new System.Drawing.Size(128, 20);
            this.lnkExpandAllGroups.TabIndex = 8;
            this.lnkExpandAllGroups.TabStop = true;
            this.lnkExpandAllGroups.Text = "Expand all groups";
            this.lnkExpandAllGroups.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkExpandAllGroups_LinkClicked);
            // 
            // picInfoFooterColors
            // 
            this.picInfoFooterColors.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picInfoFooterColors.Image = global::UiTools.Controls.ExtendedDataGridView.Demo.Properties.Resources.Info;
            this.picInfoFooterColors.Location = new System.Drawing.Point(635, 89);
            this.picInfoFooterColors.Margin = new System.Windows.Forms.Padding(2);
            this.picInfoFooterColors.Name = "picInfoFooterColors";
            this.picInfoFooterColors.Size = new System.Drawing.Size(20, 20);
            this.picInfoFooterColors.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picInfoFooterColors.TabIndex = 13;
            this.picInfoFooterColors.TabStop = false;
            this.picInfoFooterColors.Click += new System.EventHandler(this.pictureBoxEx4_Click);
            // 
            // picInfoAlignFooter
            // 
            this.picInfoAlignFooter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picInfoAlignFooter.Image = global::UiTools.Controls.ExtendedDataGridView.Demo.Properties.Resources.Info;
            this.picInfoAlignFooter.Location = new System.Drawing.Point(635, 60);
            this.picInfoAlignFooter.Margin = new System.Windows.Forms.Padding(2);
            this.picInfoAlignFooter.Name = "picInfoAlignFooter";
            this.picInfoAlignFooter.Size = new System.Drawing.Size(20, 20);
            this.picInfoAlignFooter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picInfoAlignFooter.TabIndex = 12;
            this.picInfoAlignFooter.TabStop = false;
            this.picInfoAlignFooter.Click += new System.EventHandler(this.picInfoAlignFooter_Click);
            // 
            // picInfoAllowChangeAggr
            // 
            this.picInfoAllowChangeAggr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picInfoAllowChangeAggr.Image = global::UiTools.Controls.ExtendedDataGridView.Demo.Properties.Resources.Info;
            this.picInfoAllowChangeAggr.Location = new System.Drawing.Point(635, 31);
            this.picInfoAllowChangeAggr.Margin = new System.Windows.Forms.Padding(2);
            this.picInfoAllowChangeAggr.Name = "picInfoAllowChangeAggr";
            this.picInfoAllowChangeAggr.Size = new System.Drawing.Size(20, 20);
            this.picInfoAllowChangeAggr.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picInfoAllowChangeAggr.TabIndex = 11;
            this.picInfoAllowChangeAggr.TabStop = false;
            this.picInfoAllowChangeAggr.Click += new System.EventHandler(this.picInfoAllowChangeAggr_Click);
            // 
            // picInfoAddHintToCustom
            // 
            this.picInfoAddHintToCustom.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picInfoAddHintToCustom.Image = global::UiTools.Controls.ExtendedDataGridView.Demo.Properties.Resources.Info;
            this.picInfoAddHintToCustom.Location = new System.Drawing.Point(284, 89);
            this.picInfoAddHintToCustom.Margin = new System.Windows.Forms.Padding(2);
            this.picInfoAddHintToCustom.Name = "picInfoAddHintToCustom";
            this.picInfoAddHintToCustom.Size = new System.Drawing.Size(20, 20);
            this.picInfoAddHintToCustom.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picInfoAddHintToCustom.TabIndex = 10;
            this.picInfoAddHintToCustom.TabStop = false;
            this.picInfoAddHintToCustom.Click += new System.EventHandler(this.picInfoAddHintToCustom_Click);
            // 
            // picInfoRenameCustom
            // 
            this.picInfoRenameCustom.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picInfoRenameCustom.Image = global::UiTools.Controls.ExtendedDataGridView.Demo.Properties.Resources.Info;
            this.picInfoRenameCustom.Location = new System.Drawing.Point(284, 60);
            this.picInfoRenameCustom.Margin = new System.Windows.Forms.Padding(2);
            this.picInfoRenameCustom.Name = "picInfoRenameCustom";
            this.picInfoRenameCustom.Size = new System.Drawing.Size(20, 20);
            this.picInfoRenameCustom.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picInfoRenameCustom.TabIndex = 9;
            this.picInfoRenameCustom.TabStop = false;
            this.picInfoRenameCustom.Click += new System.EventHandler(this.picInfoRenameCustom_Click);
            // 
            // picInfoSupportCustomAggr
            // 
            this.picInfoSupportCustomAggr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picInfoSupportCustomAggr.Image = global::UiTools.Controls.ExtendedDataGridView.Demo.Properties.Resources.Info;
            this.picInfoSupportCustomAggr.Location = new System.Drawing.Point(284, 31);
            this.picInfoSupportCustomAggr.Margin = new System.Windows.Forms.Padding(2);
            this.picInfoSupportCustomAggr.Name = "picInfoSupportCustomAggr";
            this.picInfoSupportCustomAggr.Size = new System.Drawing.Size(20, 20);
            this.picInfoSupportCustomAggr.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picInfoSupportCustomAggr.TabIndex = 8;
            this.picInfoSupportCustomAggr.TabStop = false;
            this.picInfoSupportCustomAggr.Click += new System.EventHandler(this.picInfoSupportCustomAggr_Click);
            // 
            // DemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1569, 778);
            this.Controls.Add(this.webView21);
            this.Controls.Add(this.gridExporter1);
            this.Controls.Add(this.cmdPopulateGrid);
            this.Controls.Add(this.lnkCollapseAllGroups);
            this.Controls.Add(this.lnkExpandAllGroups);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pgrExProperties);
            this.Controls.Add(this.cmdAddRow);
            this.Controls.Add(this.cmdDeleteRow);
            this.Controls.Add(this.cboSortOrder);
            this.Controls.Add(this.cmdSort);
            this.Controls.Add(this.cboSortColumn);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmdGroup);
            this.Controls.Add(this.cboGroupColumn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridViewEx1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DemoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Demo Form";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEx1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInfoFooterColors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInfoAlignFooter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInfoAllowChangeAggr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInfoAddHintToCustom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInfoRenameCustom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInfoSupportCustomAggr)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UiTools.Controls.ExtendedDataGridView.DataGridViewEx dataGridViewEx1;
        private Label label1;
        private ComboBox cboGroupColumn;
        private Button cmdGroup;
        private Button cmdSort;
        private ComboBox cboSortColumn;
        private Label label5;
        private ComboBox cboSortOrder;
        private Button cmdDeleteRow;
        private Button cmdAddRow;
        private PropertyGrid pgrExProperties;
        private GroupBox groupBox1;
        private CheckBox chkSupportCustomAggregateFunction;
        private CheckBox chkAddHintToCustomMenuItem;
        private CheckBox chkRenameCustomMenuItem;
        private CheckBox chkUseCustomFooterColors;
        private CheckBox chkRightAlignFooter;
        private Label label2;
        private CheckBox chkAllowChangeGroupFooterAggregate;
        private PictureBoxEx picInfoSupportCustomAggr;
        private PictureBoxEx picInfoFooterColors;
        private PictureBoxEx picInfoAlignFooter;
        private PictureBoxEx picInfoAllowChangeAggr;
        private PictureBoxEx picInfoAddHintToCustom;
        private PictureBoxEx picInfoRenameCustom;
        private LinkLabelEx lnkExpandAllGroups;
        private LinkLabelEx lnkCollapseAllGroups;
        private Button cmdPopulateGrid;
        private GridExporter gridExporter1;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
    }
}

