namespace UiTools.Controls.ExtendedDataGridView
{
    partial class GridExporter
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridExporter));
            this.tsExport = new System.Windows.Forms.ToolStrip();
            this.tsddbExport = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsiExportCsv = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiExportHtml = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiExportPdf = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiExportExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsExport.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsExport
            // 
            this.tsExport.BackColor = System.Drawing.Color.Transparent;
            this.tsExport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tsExport.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsExport.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.tsExport.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsddbExport});
            this.tsExport.Location = new System.Drawing.Point(0, 0);
            this.tsExport.Name = "tsExport";
            this.tsExport.Size = new System.Drawing.Size(221, 34);
            this.tsExport.TabIndex = 10;
            this.tsExport.Text = "toolStrip1";
            // 
            // tsddbExport
            // 
            this.tsddbExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsddbExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiExportCsv,
            this.tsiExportHtml,
            this.tsiExportPdf,
            this.tsiExportExcel});
            this.tsddbExport.Image = ((System.Drawing.Image)(resources.GetObject("tsddbExport.Image")));
            this.tsddbExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbExport.Name = "tsddbExport";
            this.tsddbExport.Size = new System.Drawing.Size(154, 29);
            this.tsddbExport.DropDownOpening += new System.EventHandler(this.tsddbExport_DropDownOpening);
            // 
            // tsiExportCsv
            // 
            this.tsiExportCsv.Image = global::UiTools.Controls.ExtendedDataGridView.Properties.Resources.ExportToCsv;
            this.tsiExportCsv.Name = "tsiExportCsv";
            this.tsiExportCsv.Size = new System.Drawing.Size(240, 34);
            this.tsiExportCsv.Click += new System.EventHandler(this.tsiExportCsv_Click);
            // 
            // tsiExportHtml
            // 
            this.tsiExportHtml.Image = global::UiTools.Controls.ExtendedDataGridView.Properties.Resources.ExportToHtml;
            this.tsiExportHtml.Name = "tsiExportHtml";
            this.tsiExportHtml.Size = new System.Drawing.Size(240, 34);
            this.tsiExportHtml.Click += new System.EventHandler(this.tsiExportHtml_Click);
            // 
            // tsiExportPdf
            // 
            this.tsiExportPdf.Image = global::UiTools.Controls.ExtendedDataGridView.Properties.Resources.ExportToPdf;
            this.tsiExportPdf.Name = "tsiExportPdf";
            this.tsiExportPdf.Size = new System.Drawing.Size(240, 34);
            this.tsiExportPdf.Click += new System.EventHandler(this.tsiExportPdf_Click);
            // 
            // tsiExportExcel
            // 
            this.tsiExportExcel.Image = global::UiTools.Controls.ExtendedDataGridView.Properties.Resources.ExportToExcel;
            this.tsiExportExcel.Name = "tsiExportExcel";
            this.tsiExportExcel.Size = new System.Drawing.Size(240, 34);
            this.tsiExportExcel.Click += new System.EventHandler(this.tsiExportExcel_Click);
            // 
            // GridExporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tsExport);
            this.Name = "GridExporter";
            this.Size = new System.Drawing.Size(221, 34);
            this.tsExport.ResumeLayout(false);
            this.tsExport.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsExport;
        private System.Windows.Forms.ToolStripDropDownButton tsddbExport;
        private System.Windows.Forms.ToolStripMenuItem tsiExportCsv;
        private System.Windows.Forms.ToolStripMenuItem tsiExportHtml;
        private System.Windows.Forms.ToolStripMenuItem tsiExportPdf;
        private System.Windows.Forms.ToolStripMenuItem tsiExportExcel;
    }
}
