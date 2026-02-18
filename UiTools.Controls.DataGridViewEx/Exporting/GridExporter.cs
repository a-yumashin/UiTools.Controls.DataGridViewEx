using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static UiTools.Controls.ExtendedDataGridView.MessageHelper;
using static UiTools.Controls.ExtendedDataGridView.CommonStuff;

namespace UiTools.Controls.ExtendedDataGridView
{
    public partial class GridExporter : UserControl
    {
        public GridExporter()
        {
            InitializeComponent();
        }

        public event EventHandler<RequestHeaderAndFooterArgs> RequestHeaderAndFooter;
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string HeaderText { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SubHeaderText { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FooterText { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataGridViewEx Grid
        {
            get => grid;
            set
            {
                grid = value;
                tsddbExport.Text = SR("Export contents");
                Width = TextRenderer.MeasureText(tsddbExport.Text, tsddbExport.Font).Width + 20;
                tsiExportCsv.Text = SR("Export to CSV");
                tsiExportHtml.Text = SR("Export to HTML");
                tsiExportPdf.Text = SR("Export to PDF");
                tsiExportExcel.Text = SR("Export to Excel");
            }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DefaultFileName { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public /*WebView2*/dynamic WebView2 { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Action<string, int> StatusUpdater { get; set; }

        private void tsddbExport_DropDownOpening(object sender, EventArgs e)
        {
            if (Grid == null)
                throw new InvalidOperationException(SR("GridExporter.Grid property not set"));
            tsiExportPdf.Enabled = WebView2 != null;
        }

        private void tsiExportExcel_Click(object sender, EventArgs e)
        {
            if (!CheckIfDataPresent())
                return;
            OnRequestHeaderAndFooter();
            bool success = true;
            string excelFileName = null;
            dynamic workSheet = null;
            dynamic workbook = null;
            dynamic workbooks = null;
            dynamic excelObj = null;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Enabled = false;
                UpdateStatus(SR("Export to MS Excel in progress..."));
                dynamic excelType = Type.GetTypeFromProgID("Excel.Application");
                if (excelType == null)
                    throw new Exception(SR("ProgID 'Excel.Application' is not registered in the system"));
                excelObj = Activator.CreateInstance(excelType);

                workbooks = excelObj.Workbooks;
                workbook = workbooks.Add();
                workSheet = excelObj.ActiveSheet;

                var dtColumn = Grid
                    .Columns(col => col.ExtInfo().DataType == GridColumnDataType.DateTime && string.IsNullOrEmpty(col.DefaultCellStyle.Format))
                    .FirstOrDefault();
                if (dtColumn != null)
                    throw new Exception(
                        $"Export aborted: column '{dtColumn.HeaderText}' has 'DateTime' data type, but its DefaultCellStyle.Format is not set.");

                int rowIndex = 1;
                if (!string.IsNullOrEmpty(HeaderText))
                {
                    workSheet.Cells[1, 1].Font.Bold = true; // HeaderText
                    rowIndex = 3;
                    if (!string.IsNullOrEmpty(SubHeaderText))
                    {
                        workSheet.Cells[2, 1].Font.Bold = true; // SubHeaderText
                        workSheet.Cells[2, 1].Font.Color = -4165632;
                        rowIndex = 4;
                    }
                }
                workSheet.Cells[rowIndex + 1, 1].Select();
                excelObj.ActiveWindow.FreezePanes = true;

                int colIndex = 0;
                foreach (DataGridViewColumn col in Grid.Columns(col => col.Visible))
                {
                    colIndex++;
                    workSheet.Cells[rowIndex, colIndex].Value = col.HeaderText;
                    workSheet.Cells[rowIndex, colIndex].Interior.Color = 11004671;
                    workSheet.Cells[rowIndex, colIndex].Font.Bold = true;
                    if (col.DefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleCenter)
                        workSheet.Cells[rowIndex, colIndex].HorizontalAlignment = -4108; // xlCenter
                    switch (col.ExtInfo().DataType)
                    {
                        case GridColumnDataType.Integer:
                            workSheet.Columns[colIndex].NumberFormat = "0";
                            break;
                        case GridColumnDataType.Decimal:
                            workSheet.Columns[colIndex].NumberFormat = "0,00";
                            break;
                        case GridColumnDataType.DateTime:
                            var fmt = col.ExtInfo().DateTimeFormatForExcel;
                            if (string.IsNullOrEmpty(fmt))
                                fmt = "@";
                            workSheet.Columns[colIndex].NumberFormat = fmt;
                            break;
                        default: // GridColumnDataType.Text, GridColumnDataType.Boolean
                            workSheet.Columns[colIndex].NumberFormat = "@";
                            break;
                    }
                }
                UpdateStatus(10);

                foreach (var dr in Grid.Rows(r => !r.IsNewRow))
                {
                    rowIndex++;
                    colIndex = 0;

                    if (dr.IsRegularRow)
                    {
                        foreach (DataGridViewColumn col in Grid.Columns(col => col.Visible))
                        {
                            colIndex++;
                            if (col.ExtInfo().DataType == GridColumnDataType.Integer)
                            {
                                var cellValue = dr.Cells[col.Name].GetFormattedValueSafe();
                                if (cellValue != string.Empty)
                                    workSheet.Cells[rowIndex, colIndex].Value = int.Parse(cellValue);
                            }
                            else if (col.ExtInfo().DataType == GridColumnDataType.Decimal)
                            {
                                var cellValue = dr.Cells[col.Name].GetFormattedValueSafe();
                                if (cellValue != string.Empty)
                                    workSheet.Cells[rowIndex, colIndex].Value = cellValue.ParseAsDecimal();
                            }
                            else if (col.ExtInfo().DataType == GridColumnDataType.DateTime)
                            {
                                var cellValue = dr.Cells[col.Name].GetFormattedValueSafe();
                                if (cellValue != string.Empty)
                                    workSheet.Cells[rowIndex, colIndex].Value = DateTime.ParseExact(
                                        cellValue, col.DefaultCellStyle.Format, CultureInfo.CurrentCulture);
                            }
                            else
                            {
                                workSheet.Cells[rowIndex, colIndex].Value = dr.Cells[col.Name].GetFormattedValueSafe();
                            }
                            // NOTE: If we don't explicitly check the data type in the grid cell and cast it to 'decimal',
                            //       *some* decimal values (though not all) are inserted into Excel incorrectly.
                            //       For example, in Russian Excel 1.335 might be inserted as 1335 instead of 1,335.
                            //       Consequently, the following two approaches proved unreliable for decimals:
                            //         workSheet.Cells[rowIndex, colIndex].Value = dr.Cells[col.Name].Value; // returns object
                            //         workSheet.Cells[rowIndex, colIndex].Value = dr.Cells[col.Name].GetFormattedValueSafe(); // returns non-null string
                            //       I'm not sure if similar issues could occur with integers (haven't encountered any yet), but I've
                            //       applied the same logic to integers as a precaution. While the decimal separator isn't an issue for
                            //       integers, who knows what other quirks Excel might have as it clearly expects to receive *unboxed* values.
                            workSheet.Cells[rowIndex, colIndex].Font.Color = dr.Cells[col.Name].Style.ForeColor;
                            workSheet.Cells[rowIndex, colIndex].Interior.Color = dr.Cells[col.Name].Style.BackColor == Color.Empty
                                ? (dr.DefaultCellStyle.BackColor == Color.Empty ? Color.White : dr.DefaultCellStyle.BackColor)
                                : dr.Cells[col.Name].Style.BackColor;
                            if (dr.Cells[col.Name].Style.Font != null && dr.Cells[col.Name].Style.Font.Bold)
                                workSheet.Cells[rowIndex, colIndex].Font.Bold = true;
                            if (col.DefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleCenter
                                && !(col.ExtInfo().DataType == GridColumnDataType.Integer || col.ExtInfo().DataType == GridColumnDataType.Decimal))
                                workSheet.Cells[rowIndex, colIndex].HorizontalAlignment = -4108; // xlCenter
                            // Fixing "Number stored as text":
                            //workSheet.Cells[rowIndex, colIndex].FormulaLocal = workSheet.Cells[rowIndex, colIndex].FormulaLocal; // this well known hack doesn't work
                            if (workSheet.Cells[rowIndex, colIndex].Errors.Item(3).Value)
                                workSheet.Cells[rowIndex, colIndex].Errors.Item(3).Ignore = true; // 3 == xlNumberAsText
                        }
                    }
                    else if (dr.IsHeaderRow)
                    {
                        var visibleColumnsCount = Grid.Columns(col => col.Visible).Count;
                        dynamic headerRange = workSheet.Range(workSheet.Cells(rowIndex, 1), workSheet.Cells(rowIndex, visibleColumnsCount));
                        headerRange.Merge();
                        headerRange.HorizontalAlignment = -4131; // xlLeft
                        workSheet.Cells[rowIndex, 1].Value = dr.HeaderRowCaption;
                        workSheet.Cells[rowIndex, 1].Font.Bold = true;
                        workSheet.Cells[rowIndex, 1].Font.Color = Grid.GroupHeaderRowForeColor == Color.Empty
                            ? Color.Black
                            : Grid.GroupHeaderRowForeColor;
                        workSheet.Cells[rowIndex, 1].Interior.Color = Grid.GroupHeaderRowBackColor == Color.Empty
                            ? Color.White
                            : Grid.GroupHeaderRowBackColor;
                    }
                    else if (dr.IsFooterRow)
                    {
                        foreach (DataGridViewColumn col in Grid.Columns(col => col.Visible))
                        {
                            colIndex++;
                            workSheet.Cells[rowIndex, colIndex].Value = dr.Cells[col.Index].FormattedValue;
                            workSheet.Cells[rowIndex, colIndex].Font.Bold = true;
                            workSheet.Cells[rowIndex, colIndex].Font.Color = dr.Cells[col.Name].Style.ForeColor == Color.Empty
                                ? Grid.GroupFooterRowForeColor
                                : dr.Cells[col.Name].Style.ForeColor;
                            workSheet.Cells[rowIndex, colIndex].Interior.Color = dr.Cells[col.Name].Style.BackColor == Color.Empty
                                ? Grid.GroupFooterRowBackColor
                                : dr.Cells[col.Name].Style.BackColor;
                            if (col.ExtInfo().GroupFooterAggregateAlignment == HorizontalAlignment.Left)
                                workSheet.Cells[rowIndex, colIndex].HorizontalAlignment = -4131; // xlLeft
                            else if (col.ExtInfo().GroupFooterAggregateAlignment == HorizontalAlignment.Right)
                                workSheet.Cells[rowIndex, colIndex].HorizontalAlignment = -4152; // xlRight
                            else
                                workSheet.Cells[rowIndex, colIndex].HorizontalAlignment = -4108; // xlCenter
                        }
                    }
                    UpdateStatus((int)(10 + 90 * (dr.Index / (decimal)Grid.Rows.Count)));
                }
                for (int i = 1; i <= colIndex; i++)
                    workSheet.Columns(i).AutoFit();
                if (!string.IsNullOrEmpty(HeaderText))
                {
                    workSheet.Cells[1, 1].Value = HeaderText; // NOTE: doing this AFTER AutoFit()!
                    if (!string.IsNullOrEmpty(SubHeaderText))
                        workSheet.Cells[2, 1].Value = SubHeaderText;
                }
                //excelObj.Visible = true;
                if (!string.IsNullOrEmpty(FooterText))
                    workSheet.Cells[rowIndex + 2, 1].Value = FooterText;
                excelFileName = GetFileNameToSave(DefaultFileName, "Excel workbook (*.xlsx)|*.xlsx", this, false);
                if (excelFileName != null)
                    workbook.SaveAs(excelFileName);
                UpdateStatus(100);
            }
            catch (Exception ex)
            {
                success = false;
                Cursor.Current = Cursors.Default;
                ShowError(ex.Message);
            }
            finally
            {
                // https://support.microsoft.com/en-us/topic/office-application-does-not-exit-after-automation-from-visual-studio-net-client-96068fdb-7a84-ecf0-3b91-282fae81a618
                if (workSheet != null)
                {
                    Marshal.FinalReleaseComObject(workSheet);
                    workSheet = null;
                }
                if (workbook != null)
                {
                    workbook.Close(false);
                    //Marshal.FinalReleaseComObject(workbook); // gives error
                    workbook = null;
                }
                if (workbooks != null)
                {
                    Marshal.FinalReleaseComObject(workbooks);
                    workbooks = null;
                }
                if (excelObj != null)
                {
                    excelObj.Quit();
                    Marshal.FinalReleaseComObject(excelObj);
                    excelObj = null;
                }
                Cursor.Current = Cursors.Default;
                Enabled = true;
                currentExportProgress = 0;
                Invalidate();
                UpdateStatus(SR("Export to MS Excel completed"), 0);
                if (success && !string.IsNullOrEmpty(excelFileName))
                    OpenFileWithAssociation(excelFileName);
            }
        }

        private void UpdateStatus(string message)
        {
            if (StatusUpdater != null)
                StatusUpdater.Invoke(message, -1);
        }
        private void UpdateStatus(int progress)
        {
            if (StatusUpdater != null)
                StatusUpdater.Invoke(null, progress);
            currentExportProgress = progress;
            Refresh();
        }
        private void UpdateStatus(string message, int progress)
        {
            if (StatusUpdater != null)
                StatusUpdater.Invoke(message, progress);
            currentExportProgress = progress;
            Refresh();
        }

        private int currentExportProgress;
        private DataGridViewEx grid;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Very simple progress bar:
            if (currentExportProgress > 0)
            {
                e.Graphics.FillRectangle(Grid.BrushCache.Get(Color.PaleGreen), 0, Height - 6,
                    Width, 5);
                e.Graphics.FillRectangle(Grid.BrushCache.Get(Color.MediumSeaGreen), 0, Height - 6,
                    (int)(Width * currentExportProgress / 100m), 5);
            }
        }

        private void OnRequestHeaderAndFooter()
        {
            if (RequestHeaderAndFooter != null)
            {
                var args = new RequestHeaderAndFooterArgs
                    { HeaderText = HeaderText, SubHeaderText = SubHeaderText, FooterText = FooterText };
                RequestHeaderAndFooter(this, args);
                if (args.Handled)
                {
                    HeaderText = args.HeaderText;
                    SubHeaderText = args.SubHeaderText;
                    FooterText = args.FooterText;
                }
            }
        }

        private void tsiExportPdf_Click(object sender, EventArgs e)
        {
            if (!CheckIfDataPresent())
                return;
            OnRequestHeaderAndFooter();
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Enabled = false;
                UpdateStatus(SR("Export to PDF in progress..."));
                var html = GenerateHtmlReport();
                ConvertReportFromHtmlToPdf(html, DefaultFileName);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                Enabled = true;
                UpdateStatus(SR("Export to PDF completed"), 0);
            }
        }

        private void tsiExportHtml_Click(object sender, EventArgs e)
        {
            if (!CheckIfDataPresent())
                return;
            OnRequestHeaderAndFooter();
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Enabled = false;
                UpdateStatus(SR("Export to HTML in progress..."));
                var html = GenerateHtmlReport();
                SaveToFileAndOpen(DefaultFileName, html, Encoding.UTF8, "HTML files (*.html)|*.html", this);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                Enabled = true;
                UpdateStatus(SR("Export to HTML completed"), 0);
            }
        }

        private string GenerateHtmlReport()
        {
            var template = CommonStuff.GetEmbeddedResource("Exporting.GridExportTemplate.html");
            template = template.Replace("#HEADER#", HeaderText).Replace("#FOOTER#", FooterText);
            template = template.Replace("#SUBHEADER#", "<div>" + SubHeaderText + "</div><br/>");
            var sb = new StringBuilder();
            foreach (DataGridViewColumn col in Grid.Columns(col => col.Visible))
            {
                string cssStyle = col.DefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleCenter
                    ? " style='text-align: center;'"
                    : "";
                sb.AppendLine(string.Format("<td{0}>{1}</td>", cssStyle, col.HeaderText));
            }
            template = template.Replace("#HEADER_CELLS#", sb.ToString());
            UpdateStatus(10);

            sb.Clear();
            foreach (var dr in Grid.Rows(r => !r.IsNewRow))
            {
                sb.Append("<tr>");
                if (dr.IsRegularRow)
                {
                    foreach (DataGridViewColumn col in Grid.Columns(col => col.Visible))
                    {
                        string cssStyle = "";
                        var color = dr.Cells[col.Name].Style.ForeColor;
                        if (color.Name != "0")
                            cssStyle = string.Format("color: {0};", color.ToHtml());
                        var bgColor = dr.Cells[col.Name].Style.BackColor == Color.Empty
                            ? (dr.DefaultCellStyle.BackColor == Color.Empty ? Color.White : dr.DefaultCellStyle.BackColor)
                            : dr.Cells[col.Name].Style.BackColor;
                        if (bgColor.Name != "0")
                            cssStyle += string.Format("background-color: {0};", bgColor.ToHtml());
                        if (dr.Cells[col.Name].Style.Font != null && dr.Cells[col.Name].Style.Font.Bold)
                            cssStyle += "font-weight: bold;";
                        if (col.DefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleCenter)
                            cssStyle += "text-align: center;";
                        if (cssStyle.Length > 0)
                            cssStyle = string.Format(" style='{0}'", cssStyle);
                        sb.AppendLine(string.Format("<td{0}>{1}</td>", cssStyle, dr.Cells[col.Name].GetFormattedValueSafe()));
                    }
                }
                else if (dr.IsHeaderRow)
                {
                    var cssStyle = string.Format(" style='font-weight: bold; color: {0}; background-color: {1};'",
                        Grid.GroupHeaderRowForeColor.ToHtml(), Grid.GroupHeaderRowBackColor.ToHtml());
                    var visibleColumnsCount = Grid.Columns(col => col.Visible).Count;
                    sb.AppendLine(string.Format("<td{0} colSpan={1}>{2}</td>", cssStyle, visibleColumnsCount, dr.HeaderRowCaption));
                }
                else if (dr.IsFooterRow)
                {
                    foreach (DataGridViewColumn col in Grid.Columns(col => col.Visible))
                    {
                        var cssStyle = "font-weight: bold;";
                        var color = dr.Cells[col.Name].Style.ForeColor == Color.Empty
                            ? Grid.GroupFooterRowForeColor
                            : dr.Cells[col.Name].Style.ForeColor;
                        cssStyle += string.Format("color: {0};", color.ToHtml());
                        var bgColor = dr.Cells[col.Name].Style.BackColor == Color.Empty
                            ? Grid.GroupFooterRowBackColor
                            : dr.Cells[col.Name].Style.BackColor;
                        cssStyle += string.Format("background-color: {0};", bgColor.ToHtml());
                        if (col.ExtInfo().GroupFooterAggregateAlignment == HorizontalAlignment.Left)
                            cssStyle += "text-align: left;";
                        else if (col.ExtInfo().GroupFooterAggregateAlignment == HorizontalAlignment.Right)
                            cssStyle += "text-align: right;";
                        else
                            cssStyle += "text-align: center;";
                        sb.AppendLine(string.Format("<td style='{0}'>{1}</td>", cssStyle, dr.Cells[col.Index].FormattedValue));
                    }
                }
                sb.AppendLine("</tr>");
                UpdateStatus((int)(10 + 90 * (dr.Index / (decimal)Grid.Rows.Count)));
            }
            currentExportProgress = 0;
            Invalidate();
            return template.Replace("#ROWS#", sb.ToString());
        }

        private async void ConvertReportFromHtmlToPdf(string html, string defaultPdfFileName)
        {
            string htmlFileName = Path.GetRandomFileName();
            htmlFileName = Path.ChangeExtension(htmlFileName, ".html");
            htmlFileName = Path.Combine(Path.GetTempPath(), htmlFileName);
            string pdfFileName = GetFileNameToSave(defaultPdfFileName, "PDF files (*.pdf)|*.pdf", this);
            if (pdfFileName != null)
            {
                string documentTitle = Path.GetFileNameWithoutExtension(pdfFileName);
                string modifiedHtml = SetHtmlTitle(html, documentTitle);
                File.WriteAllText(htmlFileName, modifiedHtml, Encoding.UTF8);
                WebView2.Tag = pdfFileName;
                await WebView2.EnsureCoreWebView2Async();
                //WebView2.CoreWebView2.NavigationCompleted += (s, e) =>
                //{
                //    Thread.Sleep(500); // just for the case
                //    PrintToPdf(WebView2.Tag.ToString());
                //};
                MethodInfo handlerMethod = typeof(GridExporter).GetMethod("OnNavigationCompleted",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                // Magic:
                DynamicEventHelper.AddDynamicEventHandler(
                    WebView2.CoreWebView2,
                    "NavigationCompleted",
                    "System.EventHandler`1[[Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs, Microsoft.Web.WebView2.Core]]",
                    this,
                    handlerMethod);
                WebView2.CoreWebView2.Navigate(htmlFileName);
            }
        }
        private void OnNavigationCompleted(object sender, EventArgs e)
        {
            Thread.Sleep(500); // just for the case
            PrintToPdf(WebView2.Tag.ToString());
        }

        private async void PrintToPdf(string pdfFileName)
        {
            Cursor.Current = Cursors.WaitCursor;
            /*CoreWebView2PrintSettings*/dynamic printSettings = WebView2.CoreWebView2.Environment.CreatePrintSettings();
            printSettings.ShouldPrintBackgrounds = true;
            printSettings.ShouldPrintHeaderAndFooter = false;
            printSettings.Orientation = Enum.Parse(printSettings.Orientation.GetType(), "Landscape");
            try
            {
                bool success = await WebView2.CoreWebView2.PrintToPdfAsync(pdfFileName, printSettings);
                if (success)
                    OpenFileWithAssociation(pdfFileName);
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                ShowError(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private static string SetHtmlTitle(string html, string title)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            string encodedTitle = System.Net.WebUtility.HtmlEncode(title);

            // Try to find existing <title>
            int titleStart = html.IndexOf("<title", StringComparison.OrdinalIgnoreCase);
            if (titleStart >= 0)
            {
                int titleOpenEnd = html.IndexOf(">", titleStart);
                int titleClose = html.IndexOf("</title>", titleStart, StringComparison.OrdinalIgnoreCase);

                if (titleOpenEnd >= 0 && titleClose > titleOpenEnd)
                {
                    // Replace only text between <title...> and </title>
                    return html.Remove(titleOpenEnd + 1, titleClose - titleOpenEnd - 1)
                               .Insert(titleOpenEnd + 1, encodedTitle);
                }
            }

            // If <title> not found - look for <head>
            int headStart = html.IndexOf("<head", StringComparison.OrdinalIgnoreCase);
            if (headStart >= 0)
            {
                int headOpenEnd = html.IndexOf(">", headStart);
                if (headOpenEnd >= 0)
                {
                    return html.Insert(headOpenEnd + 1, $"\n<title>{encodedTitle}</title>");
                }
            }

            // If <head> not found - look for <html>
            int htmlTagStart = html.IndexOf("<html", StringComparison.OrdinalIgnoreCase);
            if (htmlTagStart >= 0)
            {
                int htmlOpenEnd = html.IndexOf(">", htmlTagStart);
                if (htmlOpenEnd >= 0)
                {
                    return html.Insert(htmlOpenEnd + 1, $"\n<head><title>{encodedTitle}</title></head>");
                }
            }

            // If neither found
            return $"<html><head><title>{encodedTitle}</title></head><body>{html}</body></html>";
        }

        private void tsiExportCsv_Click(object sender, EventArgs e)
        {
            if (!CheckIfDataPresent())
                return;
            OnRequestHeaderAndFooter();
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Enabled = false;
                UpdateStatus(SR("Export to CSV in progress..."));
                string csv;
                var values = Grid.Columns(col => col.Visible)
                    .Select(col => col.HeaderText)
                    .ToList();
                csv = string.Join("|", values) + "\n";
                foreach (var dr in Grid.Rows(r => !r.IsNewRow))
                {
                    values.Clear();
                    if (dr.IsRegularRow || dr.IsFooterRow)
                    {
                        values.AddRange(Grid.Columns(col => col.Visible)
                            .Select(col => dr.Cells[col.Name].GetFormattedValueSafe().PrepareForCsv()));
                    }
                    else if (dr.IsHeaderRow)
                    {
                        values.Add(dr.HeaderRowCaption);
                        values.AddRange(Grid.Columns(col => col.Visible)
                            .Skip(1)
                            .Select(col => string.Empty));
                    }
                    csv += string.Join("|", values) + "\n";
                }
                SaveToFileAndOpen(DefaultFileName, csv, Encoding.UTF8, "CSV files (*.csv)|*.csv", "notepad.exe", this);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                Enabled = true;
                UpdateStatus(SR("Export to CSV completed"));
            }
        }

        private bool CheckIfDataPresent()
        {
            bool dataPresent = true;
            if (Grid.Rows(r => !r.IsNewRow).Count == 0)
            {
                ShowInfo(SR("Nothing to export"));
                dataPresent = false;
            }
            if (Grid.Columns(col => col.Visible).Count == 0)
            {
                ShowInfo(SR("No visible columns, nothing to export"));
                dataPresent = false;
            }
            return dataPresent;
        }

        private static void OpenFileWithAssociation(string fileName)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    UseShellExecute = true
                }
            };
            process.Start();
        }

        private static void OpenFileInApplication(string fileName, string appName)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = appName,
                    Arguments = fileName
                }
            };
            process.Start();
        }

        private static void SaveToFileAndOpen(string defaultFileName, string fileContents, Encoding encoding, string saveFileDialogFilter, IWin32Window owner)
        {
            var fileName = GetFileNameToSave(defaultFileName, saveFileDialogFilter, owner);
            if (fileName != null)
            {
                File.WriteAllText(fileName, fileContents, encoding);
                OpenFileWithAssociation(fileName);
            }
        }

        private static void SaveToFileAndOpen(string defaultFileName, string fileContents, Encoding encoding, string saveFileDialogFilter, string appName, IWin32Window owner)
        {
            var fileName = GetFileNameToSave(defaultFileName, saveFileDialogFilter, owner);
            if (fileName != null)
            {
                File.WriteAllText(fileName, fileContents, encoding);
                OpenFileInApplication(fileName, appName);
            }
        }

        private static string GetFileNameToSave(string defaultFileName, string saveFileDialogFilter, IWin32Window owner, bool overwritePrompt = true)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Title = SR("Export to file");
                dlg.FileName = defaultFileName;
                dlg.Filter = saveFileDialogFilter;
                dlg.OverwritePrompt = overwritePrompt;
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return dlg.ShowDialog(owner) == DialogResult.OK
                    ? dlg.FileName
                    : null;
            }
        }
    }

    public class RequestHeaderAndFooterArgs : EventArgs
    {
        public RequestHeaderAndFooterArgs()
            : base()
        {
        }

        public string HeaderText { get; set; }
        public string SubHeaderText { get; set; }
        public string FooterText { get; set; }
        public bool Handled { get; set; }
    }
}
