using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using AdvancedPOS.Helpers;
using AdvancedPOS.Repositories;

namespace AdvancedPOS.Views
{
    public partial class SalesForecastForm : Form
    {
        // ==========================================
        // UI Controls (Code එකෙන්ම හදනවා)
        // ==========================================
        private Chart chartForecast;
        private DataGridView dgvForecast;
        private Button btnRegenerate;
        private Label lblInfo;
        private NumericUpDown nudHorizon;

        private readonly SalesForecastRepository forecastRepo = new SalesForecastRepository();
        private readonly Color darkColor = Color.FromArgb(43, 43, 54);

        public SalesForecastForm()
        {
            InitializeComponent();
            BuildUI();
            AdvancedPOS.Helpers.ThemeManager.ApplyToForm(this,
                AdvancedPOS.Helpers.ThemeManager.MainBackground,
                AdvancedPOS.Helpers.ThemeManager.CardBackground,
                AdvancedPOS.Helpers.ThemeManager.PrimaryTextColor,
                AdvancedPOS.Helpers.ThemeManager.SecondaryTextColor);

            GenerateForecast();
        }

        // ==========================================
        // 1. UI එක සම්පූර්ණයෙන්ම Code එකෙන් හැදීම
        // ==========================================
        private void BuildUI()
        {
            this.BackColor = Color.White;

            // ---------- ඉහළ Filter Bar ----------
            Panel topBar = new Panel();
            topBar.Dock = DockStyle.Top;
            topBar.Height = 90;
            topBar.BackColor = Color.FromArgb(240, 242, 245);
            topBar.Padding = new Padding(30, 15, 30, 15);
            this.Controls.Add(topBar);

            Label lblHeading = new Label();
            lblHeading.Text = "📈 AI Sales Forecast";
            lblHeading.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(0, 0);
            topBar.Controls.Add(lblHeading);

            Label lblHorizonTitle = new Label();
            lblHorizonTitle.Text = "Forecast Days";
            lblHorizonTitle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblHorizonTitle.ForeColor = Color.Gray;
            lblHorizonTitle.AutoSize = true;
            lblHorizonTitle.Location = new Point(0, 35);
            topBar.Controls.Add(lblHorizonTitle);

            nudHorizon = new NumericUpDown();
            nudHorizon.Font = new Font("Segoe UI", 10.5F);
            nudHorizon.Width = 100;
            nudHorizon.Minimum = 3;
            nudHorizon.Maximum = 30;
            nudHorizon.Value = 7;
            nudHorizon.Location = new Point(0, 55);
            topBar.Controls.Add(nudHorizon);

            btnRegenerate = new Button();
            btnRegenerate.Text = "🔄 Regenerate Forecast";
            btnRegenerate.Size = new Size(190, 38);
            btnRegenerate.Location = new Point(120, 55);
            btnRegenerate.FlatStyle = FlatStyle.Flat;
            btnRegenerate.FlatAppearance.BorderSize = 0;
            btnRegenerate.BackColor = Color.FromArgb(41, 128, 185);
            btnRegenerate.ForeColor = Color.White;
            btnRegenerate.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnRegenerate.Cursor = Cursors.Hand;
            btnRegenerate.Click += btnRegenerate_Click;
            topBar.Controls.Add(btnRegenerate);

            lblInfo = new Label();
            lblInfo.Text = "";
            lblInfo.Font = new Font("Segoe UI", 9F);
            lblInfo.ForeColor = Color.Gray;
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(330, 62);
            topBar.Controls.Add(lblInfo);

            // ---------- පහළ: Chart (වම) + Table (දකුණ) ----------
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 2;
            layout.RowCount = 1;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.Padding = new Padding(20);
            layout.BackColor = Color.FromArgb(240, 242, 245);
            this.Controls.Add(layout);
            layout.BringToFront();

            // Chart Card
            Panel chartCard = new Panel();
            chartCard.Dock = DockStyle.Fill;
            chartCard.Margin = new Padding(0, 0, 10, 0);
            chartCard.BackColor = Color.White;
            chartCard.Padding = new Padding(10);

            chartForecast = new Chart();
            chartForecast.Dock = DockStyle.Fill;
            chartCard.Controls.Add(chartForecast);
            layout.Controls.Add(chartCard, 0, 0);

            // Grid Card
            Panel gridCard = new Panel();
            gridCard.Dock = DockStyle.Fill;
            gridCard.Margin = new Padding(10, 0, 0, 0);
            gridCard.BackColor = Color.White;
            gridCard.Padding = new Padding(10);

            Label lblGridTitle = new Label();
            lblGridTitle.Text = "FORECASTED SALES";
            lblGridTitle.Dock = DockStyle.Top;
            lblGridTitle.Height = 30;
            lblGridTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblGridTitle.ForeColor = Color.Gray;
            gridCard.Controls.Add(lblGridTitle);

            dgvForecast = new DataGridView();
            dgvForecast.Dock = DockStyle.Fill;
            dgvForecast.BackgroundColor = Color.White;
            dgvForecast.BorderStyle = BorderStyle.None;
            dgvForecast.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvForecast.RowHeadersVisible = false;
            dgvForecast.AllowUserToAddRows = false;
            dgvForecast.ReadOnly = true;
            dgvForecast.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvForecast.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvForecast.EnableHeadersVisualStyles = false;
            dgvForecast.ColumnHeadersHeight = 38;
            dgvForecast.ColumnHeadersDefaultCellStyle.BackColor = darkColor;
            dgvForecast.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvForecast.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvForecast.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvForecast.RowTemplate.Height = 32;

            dgvForecast.Columns.Add("Date", "Date");
            dgvForecast.Columns.Add("Predicted", "Predicted Sales");
            dgvForecast.Columns.Add("Range", "Range (Low - High)");

            gridCard.Controls.Add(dgvForecast);
            dgvForecast.BringToFront();
            layout.Controls.Add(gridCard, 1, 0);
        }

        // ==========================================
        // 2. Forecast Generate කිරීම
        // ==========================================
        private void btnRegenerate_Click(object sender, EventArgs e)
        {
            GenerateForecast();
        }

        private void GenerateForecast()
        {
            try
            {
                int horizon = (int)nudHorizon.Value;
                int historyDays = 60; // පසුගිය 60 Days ගේ Data පාවිච්චි කරනවා

                var salesHistory = forecastRepo.GetDailySalesForForecast(historyDays);
                List<float> values = salesHistory.Select(x => x.Total).ToList();

                SalesForecastResult result = SalesForecastHelper.ForecastSales(values, horizon);

                DrawChart(salesHistory, result, horizon);
                FillGrid(result, horizon);

                lblInfo.Text = $"Based on last {historyDays} days of sales data";
                lblInfo.ForeColor = Color.Gray;
            }
            catch (Exception ex)
            {
                lblInfo.Text = "⚠ " + ex.Message;
                lblInfo.ForeColor = Color.Crimson;
                chartForecast.Series.Clear();
                dgvForecast.Rows.Clear();
            }
        }

        // ==========================================
        // 3. Chart එක අඳිනවා (Actual Sales + Forecast + Confidence Band)
        // ==========================================
        private void DrawChart(List<(DateTime Date, float Total)> history, SalesForecastResult result, int horizon)
        {
            chartForecast.Series.Clear();
            chartForecast.Titles.Clear();
            chartForecast.ChartAreas.Clear();

            ChartArea area = new ChartArea("main");
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.Interval = Math.Max(1, (history.Count + horizon) / 15);
            chartForecast.ChartAreas.Add(area);

            Title title = new Title("Sales Trend & Forecast", Docking.Top, new Font("Segoe UI", 12, FontStyle.Bold), darkColor);
            chartForecast.Titles.Add(title);

            // ---- Actual Sales (පසුගිය දවස්) ----
            Series actualSeries = new Series("Actual Sales");
            actualSeries.ChartType = SeriesChartType.Line;
            actualSeries.Color = Color.FromArgb(41, 128, 185);
            actualSeries.BorderWidth = 2;

            // Chart එක Crowded වෙන්නේ නැතුව, Last 20 Days විතරක් Actual විදිහට පෙන්වනවා
            var recentHistory = history.Skip(Math.Max(0, history.Count - 20)).ToList();
            foreach (var point in recentHistory)
            {
                actualSeries.Points.AddXY(point.Date.ToString("dd MMM"), point.Total);
            }
            chartForecast.Series.Add(actualSeries);

            // ---- Forecast (ඉදිරි දවස්) ----
            Series forecastSeries = new Series("Forecast");
            forecastSeries.ChartType = SeriesChartType.Line;
            forecastSeries.Color = Color.FromArgb(243, 156, 18);
            forecastSeries.BorderWidth = 2;
            forecastSeries.BorderDashStyle = ChartDashStyle.Dash;

            DateTime lastDate = history.Last().Date;
            for (int i = 0; i < result.ForecastedSales.Count; i++)
            {
                DateTime futureDate = lastDate.AddDays(i + 1);
                forecastSeries.Points.AddXY(futureDate.ToString("dd MMM"), result.ForecastedSales[i]);
            }
            chartForecast.Series.Add(forecastSeries);

            chartForecast.Legends.Add(new Legend("main"));
        }

        // ==========================================
        // 4. Grid එකේ Forecast Values පෙන්වනවා
        // ==========================================
        private void FillGrid(SalesForecastResult result, int horizon)
        {
            dgvForecast.Rows.Clear();
            DateTime startDate = DateTime.Today.AddDays(1);

            for (int i = 0; i < result.ForecastedSales.Count; i++)
            {
                string date = startDate.AddDays(i).ToString("dd MMM yyyy");
                string predicted = "Rs. " + result.ForecastedSales[i].ToString("N2");
                string range = $"Rs. {result.LowerBound[i]:N0} - Rs. {result.UpperBound[i]:N0}";

                dgvForecast.Rows.Add(date, predicted, range);
            }
        }
    }
}
