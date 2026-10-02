using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using AdvancedPOS.Repositories;
using AdvancedPOS.Helpers;

namespace AdvancedPOS.Views
{
    public partial class ReportsForm : Form
    {
        // ==========================================
        // UI Controls (Code එකෙන්ම හදනවා)
        // ==========================================
        private ComboBox cmbReportType;
        private DateTimePicker dtpFromDate, dtpToDate;
        private Button btnGenerate;
        private DataGridView dgvReport;
        private Chart chartSales;
        private Button btnExportExcel, btnExportPdf;

        private readonly ReportRepository _reportRepo = new ReportRepository();
        private readonly Color darkColor = Color.FromArgb(43, 43, 54);

        public ReportsForm()
        {
            InitializeComponent();
            BuildUI();
            SetupForm();
        }

        // ==========================================
        // 1. UI එක සම්පූර්ණයෙන්ම Code එකෙන් හැදීම
        // ==========================================
        private void BuildUI()
        {
            this.BackColor = Color.White;

            // ---------- ඉහළ Filter Bar ----------
            Panel filterPanel = new Panel();
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Height = 100;
            filterPanel.BackColor = Color.FromArgb(240, 242, 245);
            filterPanel.Padding = new Padding(30, 20, 30, 20);
            this.Controls.Add(filterPanel);

            Label lblHeading = new Label();
            lblHeading.Text = "SALES REPORTS";
            lblHeading.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(0, 0);
            filterPanel.Controls.Add(lblHeading);

            int x = 0;
            int fieldY = 35;

            // Report Type
            Label lblType = new Label();
            lblType.Text = "Report Type";
            lblType.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblType.ForeColor = Color.Gray;
            lblType.AutoSize = true;
            lblType.Location = new Point(x, fieldY);
            filterPanel.Controls.Add(lblType);

            cmbReportType = new ComboBox();
            cmbReportType.Font = new Font("Segoe UI", 10.5F);
            cmbReportType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbReportType.Width = 200;
            cmbReportType.Location = new Point(x, fieldY + 18);
            filterPanel.Controls.Add(cmbReportType);

            x += 220;

            // From Date
            Label lblFrom = new Label();
            lblFrom.Text = "From Date";
            lblFrom.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblFrom.ForeColor = Color.Gray;
            lblFrom.AutoSize = true;
            lblFrom.Location = new Point(x, fieldY);
            filterPanel.Controls.Add(lblFrom);

            dtpFromDate = new DateTimePicker();
            dtpFromDate.Font = new Font("Segoe UI", 10.5F);
            dtpFromDate.Format = DateTimePickerFormat.Short;
            dtpFromDate.Width = 150;
            dtpFromDate.Location = new Point(x, fieldY + 18);
            filterPanel.Controls.Add(dtpFromDate);

            x += 170;

            // To Date
            Label lblTo = new Label();
            lblTo.Text = "To Date";
            lblTo.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblTo.ForeColor = Color.Gray;
            lblTo.AutoSize = true;
            lblTo.Location = new Point(x, fieldY);
            filterPanel.Controls.Add(lblTo);

            dtpToDate = new DateTimePicker();
            dtpToDate.Font = new Font("Segoe UI", 10.5F);
            dtpToDate.Format = DateTimePickerFormat.Short;
            dtpToDate.Width = 150;
            dtpToDate.Location = new Point(x, fieldY + 18);
            filterPanel.Controls.Add(dtpToDate);

            x += 170;

            // Generate Button
            btnGenerate = new Button();
            btnGenerate.Text = "Generate Report";
            btnGenerate.Size = new Size(160, 40);
            btnGenerate.Location = new Point(x, fieldY + 16);
            btnGenerate.FlatStyle = FlatStyle.Flat;
            btnGenerate.FlatAppearance.BorderSize = 0;
            btnGenerate.BackColor = Color.FromArgb(39, 174, 96);
            btnGenerate.ForeColor = Color.White;
            btnGenerate.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnGenerate.Cursor = Cursors.Hand;
            filterPanel.Controls.Add(btnGenerate);
            x += 175;

            btnExportExcel = new Button();
            btnExportExcel.Text = "📊 Excel";
            btnExportExcel.Size = new Size(110, 40);
            btnExportExcel.Location = new Point(x, fieldY + 16);
            btnExportExcel.FlatStyle = FlatStyle.Flat;
            btnExportExcel.FlatAppearance.BorderSize = 0;
            btnExportExcel.BackColor = Color.FromArgb(39, 174, 96);
            btnExportExcel.ForeColor = Color.White;
            btnExportExcel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnExportExcel.Cursor = Cursors.Hand;
            btnExportExcel.Click += btnExportExcel_Click;
            filterPanel.Controls.Add(btnExportExcel);

            x += 120;

            btnExportPdf = new Button();
            btnExportPdf.Text = "📄 PDF";
            btnExportPdf.Size = new Size(110, 40);
            btnExportPdf.Location = new Point(x, fieldY + 16);
            btnExportPdf.FlatStyle = FlatStyle.Flat;
            btnExportPdf.FlatAppearance.BorderSize = 0;
            btnExportPdf.BackColor = Color.FromArgb(231, 76, 60);
            btnExportPdf.ForeColor = Color.White;
            btnExportPdf.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnExportPdf.Cursor = Cursors.Hand;
            btnExportPdf.Click += btnExportPdf_Click;
            filterPanel.Controls.Add(btnExportPdf);

            // ---------- පහළ: Grid (වම) + Chart (දකුණ) ----------
            TableLayoutPanel splitLayout = new TableLayoutPanel();
            splitLayout.Dock = DockStyle.Fill;
            splitLayout.ColumnCount = 2;
            splitLayout.RowCount = 1;
            splitLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            splitLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            splitLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            splitLayout.BackColor = Color.White;
            splitLayout.Padding = new Padding(20);
            this.Controls.Add(splitLayout);
            splitLayout.BringToFront();

            // Grid Panel
            Panel gridCard = new Panel();
            gridCard.Dock = DockStyle.Fill;
            gridCard.Margin = new Padding(0, 0, 10, 0);
            gridCard.BackColor = Color.White;

            dgvReport = new DataGridView();
            dgvReport.Dock = DockStyle.Fill;
            dgvReport.BackgroundColor = Color.White;
            dgvReport.BorderStyle = BorderStyle.None;
            dgvReport.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvReport.RowHeadersVisible = false;
            dgvReport.AllowUserToAddRows = false;
            dgvReport.ReadOnly = true;
            dgvReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReport.EnableHeadersVisualStyles = false;
            dgvReport.ColumnHeadersHeight = 40;
            dgvReport.ColumnHeadersDefaultCellStyle.BackColor = darkColor;
            dgvReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReport.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvReport.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvReport.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvReport.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 240, 241);
            dgvReport.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvReport.RowTemplate.Height = 32;

            gridCard.Controls.Add(dgvReport);
            splitLayout.Controls.Add(gridCard, 0, 0);

            // Chart Panel
            Panel chartCard = new Panel();
            chartCard.Dock = DockStyle.Fill;
            chartCard.Margin = new Padding(10, 0, 0, 0);
            chartCard.BackColor = Color.White;

            chartSales = new Chart();
            chartSales.Dock = DockStyle.Fill;

            chartCard.Controls.Add(chartSales);
            splitLayout.Controls.Add(chartCard, 1, 0);

            AdvancedPOS.Helpers.ThemeManager.ApplyToForm(this,
            AdvancedPOS.Helpers.ThemeManager.MainBackground,
            AdvancedPOS.Helpers.ThemeManager.CardBackground,
            AdvancedPOS.Helpers.ThemeManager.PrimaryTextColor,
            AdvancedPOS.Helpers.ThemeManager.SecondaryTextColor);
        }

        // ==========================================
        // 2. Form Setup (ඔයාගේ පරණ code එකම)
        // ==========================================
        private void SetupForm()
        {
            cmbReportType.Items.Add("Daily Sales");
            cmbReportType.Items.Add("Monthly Sales");
            cmbReportType.Items.Add("Profit & Expense");
            cmbReportType.Items.Add("Low Stock Alerts");
            cmbReportType.SelectedIndex = 0;

            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Today;

            btnGenerate.Click += BtnGenerate_Click;

            SetupChart();
        }

        private void SetupChart()
        {
            chartSales.Series.Clear();
            chartSales.Titles.Clear();

            ChartArea area = new ChartArea("main");
            chartSales.ChartAreas.Add(area);

            Title chartTitle = new Title("Sales Trend", Docking.Top, new Font("Segoe UI", 14, FontStyle.Bold), Color.FromArgb(44, 62, 80));
            chartSales.Titles.Add(chartTitle);

            chartSales.ChartAreas[0].BackColor = Color.White;
            chartSales.BorderSkin.SkinStyle = BorderSkinStyle.None;

            chartSales.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            chartSales.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Segoe UI", 9);
            chartSales.ChartAreas[0].AxisX.LineColor = Color.LightGray;

            chartSales.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gainsboro;
            chartSales.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartSales.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Segoe UI", 9);
            chartSales.ChartAreas[0].AxisY.LineColor = Color.LightGray;
        }

        // ==========================================
        // 3. Report Generation Logic (ඔයාගේ පරණ code එකම)
        // ==========================================
        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            string reportType = cmbReportType.SelectedItem.ToString();
            DataTable dt = new DataTable();

            try
            {
                if (reportType == "Daily Sales")
                {
                    dt = _reportRepo.GetDailySalesReport(dtpFromDate.Value, dtpToDate.Value);
                    GenerateChart(dt, "Date", "Total Sales", "Daily Sales Trend");
                }
                else if (reportType == "Monthly Sales")
                {
                    dt = _reportRepo.GetMonthlySalesReport(dtpFromDate.Value, dtpToDate.Value);
                    dt.Columns.Add("MonthYear", typeof(string), "Month + ' ' + Year");
                    GenerateChart(dt, "MonthYear", "Total Sales", "Monthly Sales Trend");
                }
                else if (reportType == "Profit & Expense")
                {
                    dt = _reportRepo.GetProfitAndExpenseReport(dtpFromDate.Value, dtpToDate.Value);
                    GenerateChart(dt, "Date", "Net Profit", "Daily Net Profit Trend");
                }
                else if (reportType == "Low Stock Alerts")
                {
                    dt = _reportRepo.GetLowStockReport(10);
                    GenerateChart(dt, "Item Name", "Current Stock", "Items Running Out of Stock");
                }

                dgvReport.DataSource = dt;
                FormatGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateChart(DataTable dt, string xColumn, string yColumn, string chartTitle)
        {
            chartSales.Series.Clear();
            chartSales.Titles[0].Text = chartTitle;

            Series series = new Series
            {
                Name = "Sales",
                IsVisibleInLegend = false,
                Color = Color.FromArgb(41, 128, 185),
                ChartType = SeriesChartType.Column
            };

            chartSales.Series.Add(series);

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = dt.Rows[i];
                string xValue = row[xColumn].ToString();

                if (dt.Columns[xColumn].DataType == typeof(DateTime))
                {
                    xValue = Convert.ToDateTime(row[xColumn]).ToString("dd/MMM");
                }

                decimal yValue = Convert.ToDecimal(row[yColumn]);
                series.Points.AddXY(xValue, yValue);
            }
        }

        private void FormatGrid()
        {
            if (dgvReport.Columns["Total SubTotal"] != null) dgvReport.Columns["Total SubTotal"].DefaultCellStyle.Format = "N2";
            if (dgvReport.Columns["Total Discount"] != null) dgvReport.Columns["Total Discount"].DefaultCellStyle.Format = "N2";
            if (dgvReport.Columns["Total Sales"] != null) dgvReport.Columns["Total Sales"].DefaultCellStyle.Format = "N2";
            if (dgvReport.Columns["Total Expenses"] != null)
                dgvReport.Columns["Total Expenses"].DefaultCellStyle.Format = "N2";

            if (dgvReport.Columns["Net Profit"] != null)
            {
                dgvReport.Columns["Net Profit"].DefaultCellStyle.Format = "N2";
                dgvReport.Columns["Net Profit"].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }

            if (cmbReportType.SelectedItem.ToString() == "Low Stock Alerts")
            {
                foreach (DataGridViewRow row in dgvReport.Rows)
                {
                    if (row.Cells["Current Stock"].Value != null)
                    {
                        int stock = Convert.ToInt32(row.Cells["Current Stock"].Value);

                        if (stock <= 0)
                        {
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 204, 204);
                            row.DefaultCellStyle.ForeColor = Color.DarkRed;
                        }
                        else
                        {
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 250, 205);
                            row.DefaultCellStyle.ForeColor = Color.DarkOrange;
                        }
                    }
                }
            }
        }
        // ==========================================
        // 4. Export Logic
        // ==========================================
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            string reportTitle = cmbReportType.SelectedItem?.ToString() ?? "Report";
            AdvancedPOS.Helpers.ExportHelper.ExportToExcel(dgvReport, reportTitle);
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            string reportTitle = cmbReportType.SelectedItem?.ToString() ?? "Report";
            AdvancedPOS.Helpers.ExportHelper.ExportToPdf(dgvReport, reportTitle);
        }
    }
}