namespace UiTools.Controls.ExtendedDataGridView
{
    partial class ColumnFilterControl
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
            this.components = new System.ComponentModel.Container();
            this.txtValue1 = new System.Windows.Forms.MaskedTextBox();
            this.lstDistinctValues = new System.Windows.Forms.CheckedListBox();
            this.cboFilterType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtValue2 = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lnkCancel = new LinkLabelEx();
            this.lnkReset = new LinkLabelEx();
            this.lnkApply = new LinkLabelEx();
            this.SuspendLayout();
            // 
            // txtValue1
            // 
            this.txtValue1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValue1.Location = new System.Drawing.Point(0, 77);
            this.txtValue1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtValue1.Name = "txtValue1";
            this.txtValue1.Size = new System.Drawing.Size(393, 31);
            this.txtValue1.TabIndex = 2;
            // 
            // lstDistinctValues
            // 
            this.lstDistinctValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstDistinctValues.FormattingEnabled = true;
            this.lstDistinctValues.IntegralHeight = false;
            this.lstDistinctValues.Location = new System.Drawing.Point(0, 186);
            this.lstDistinctValues.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstDistinctValues.Name = "lstDistinctValues";
            this.lstDistinctValues.Size = new System.Drawing.Size(393, 323);
            this.lstDistinctValues.TabIndex = 5;
            // 
            // cboFilterType
            // 
            this.cboFilterType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFilterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFilterType.FormattingEnabled = true;
            this.cboFilterType.Location = new System.Drawing.Point(0, 36);
            this.cboFilterType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cboFilterType.Name = "cboFilterType";
            this.cboFilterType.Size = new System.Drawing.Size(393, 33);
            this.cboFilterType.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(0, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 25);
            this.label1.TabIndex = 0;
            // 
            // txtValue2
            // 
            this.txtValue2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValue2.Location = new System.Drawing.Point(0, 116);
            this.txtValue2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtValue2.Name = "txtValue2";
            this.txtValue2.Size = new System.Drawing.Size(393, 31);
            this.txtValue2.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(0, 153);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(276, 25);
            this.label2.TabIndex = 4;
            // 
            // lnkCancel
            // 
            this.lnkCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkCancel.AutoSize = true;
            this.lnkCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkCancel.Location = new System.Drawing.Point(315, 513);
            this.lnkCancel.Name = "lnkCancel";
            this.lnkCancel.Size = new System.Drawing.Size(80, 25);
            this.lnkCancel.TabIndex = 8;
            this.lnkCancel.TabStop = true;
            // 
            // lnkReset
            // 
            this.lnkReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkReset.AutoSize = true;
            this.lnkReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkReset.Location = new System.Drawing.Point(110, 513);
            this.lnkReset.Name = "lnkReset";
            this.lnkReset.Size = new System.Drawing.Size(89, 25);
            this.lnkReset.TabIndex = 7;
            this.lnkReset.TabStop = true;
            // 
            // lnkApply
            // 
            this.lnkApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkApply.AutoSize = true;
            this.lnkApply.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkApply.Location = new System.Drawing.Point(0, 513);
            this.lnkApply.Name = "lnkApply";
            this.lnkApply.Size = new System.Drawing.Size(104, 25);
            this.lnkApply.TabIndex = 6;
            this.lnkApply.TabStop = true;
            // 
            // ColumnFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lnkCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtValue2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lnkReset);
            this.Controls.Add(this.lnkApply);
            this.Controls.Add(this.cboFilterType);
            this.Controls.Add(this.lstDistinctValues);
            this.Controls.Add(this.txtValue1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ColumnFilterControl";
            this.Size = new System.Drawing.Size(393, 550);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MaskedTextBox txtValue1;
        private System.Windows.Forms.CheckedListBox lstDistinctValues;
        private System.Windows.Forms.ComboBox cboFilterType;
        private LinkLabelEx lnkApply;
        private LinkLabelEx lnkReset;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MaskedTextBox txtValue2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip1;
        private LinkLabelEx lnkCancel;
    }
}