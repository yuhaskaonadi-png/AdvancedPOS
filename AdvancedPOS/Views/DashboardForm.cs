using System;
using System.Windows.Forms;
using AdvancedPOS.Repositories;
using AdvancedPOS.Models;
using System.Drawing;
using System.Linq;
using System.Data;
using System.Windows.Forms.DataVisualization.Charting;

namespace AdvancedPOS.Views
{
    public partial class DashboardForm : Form
    {
        // ==========================================
        // 1. Variables
        // ==========================================
        private readonly string currentUserRole;
        private readonly DashboardRepository _dashboardRepo;
        private Form activeForm = null;
        private bool isLoggingOut = false;
        private Chart chartWeekly;
        private DataGridView dgvLowStock;
        private Label lblClock;
        private Timer clockTimer;
        private readonly ReportRepository _reportRepo = new ReportRepository();
        private Button btnCustomers;
        private Button btnStockAdjustment;
        private Button btnBackupRestore;
        private Button btnThemeToggle;

        // ==========================================
        // 2. Constructor (LoginForm එකෙන් role එක එනවා)
        // ==========================================
        public DashboardForm(string role)
        {
            InitializeComponent();

            // ---- Customers Button (Settings එකේ Style එකම Copy කරලා) ----
            btnCustomers = new Button();
            btnCustomers.Text = "Customers";
            btnCustomers.Dock = DockStyle.Top;
            btnCustomers.Cursor = Cursors.Hand;
            btnCustomers.Height = btnSettings.Height;
            btnCustomers.Font = btnSettings.Font;
            btnCustomers.ForeColor = btnSettings.ForeColor;
            btnCustomers.FlatStyle = btnSettings.FlatStyle;
            btnCustomers.FlatAppearance.BorderSize = btnSettings.FlatAppearance.BorderSize;
            btnCustomers.FlatAppearance.BorderColor = btnSettings.FlatAppearance.BorderColor;
            btnCustomers.TextAlign = btnSettings.TextAlign;
            btnCustomers.Padding = btnSettings.Padding;
            sidePanel.Controls.Add(btnCustomers);
            btnCustomers.BringToFront();
            btnCustomers.Visible = false; // සුපර්මාකට් එකට අවශ්‍ය නැති නිසා හංගා ඇත

            // ---- Stock Adjustment Button (Settings එකේ Style එකම Copy කරලා) ----
            btnStockAdjustment = new Button();
            btnStockAdjustment.Text = "Stock Adjustment";
            btnStockAdjustment.Dock = DockStyle.Top;
            btnStockAdjustment.Cursor = Cursors.Hand;
            btnStockAdjustment.Height = btnSettings.Height;
            btnStockAdjustment.Font = btnSettings.Font;
            btnStockAdjustment.ForeColor = btnSettings.ForeColor;
            btnStockAdjustment.FlatStyle = btnSettings.FlatStyle;
            btnStockAdjustment.FlatAppearance.BorderSize = btnSettings.FlatAppearance.BorderSize;
            btnStockAdjustment.FlatAppearance.BorderColor = btnSettings.FlatAppearance.BorderColor;
            btnStockAdjustment.TextAlign = btnSettings.TextAlign;
            btnStockAdjustment.Padding = btnSettings.Padding;
            sidePanel.Controls.Add(btnStockAdjustment);
            btnStockAdjustment.BringToFront();

            // ---- Backup & Restore Button (Settings එකේ Style එකම Copy කරලා) ----
            btnBackupRestore = new Button();
            btnBackupRestore.Text = "Backup and Restore";
            btnBackupRestore.Dock = DockStyle.Top;
            btnBackupRestore.Cursor = Cursors.Hand;
            btnBackupRestore.Height = btnSettings.Height;
            btnBackupRestore.Font = btnSettings.Font;
            btnBackupRestore.ForeColor = btnSettings.ForeColor;
            btnBackupRestore.FlatStyle = btnSettings.FlatStyle;
            btnBackupRestore.FlatAppearance.BorderSize = btnSettings.FlatAppearance.BorderSize;
            btnBackupRestore.FlatAppearance.BorderColor = btnSettings.FlatAppearance.BorderColor;
            btnBackupRestore.TextAlign = btnSettings.TextAlign;
            btnBackupRestore.Padding = btnSettings.Padding;
            sidePanel.Controls.Add(btnBackupRestore);
            btnBackupRestore.BringToFront();

            // ---- Dark Mode Toggle Button ----
            btnThemeToggle = new Button();
            btnThemeToggle.Text = AdvancedPOS.Helpers.ThemeManager.IsDarkMode ? "☀ Light Mode" : "🌙 Dark Mode";
            btnThemeToggle.Dock = DockStyle.Top;
            btnThemeToggle.Cursor = Cursors.Hand;
            btnThemeToggle.Height = btnSettings.Height;
            btnThemeToggle.Font = btnSettings.Font;
            btnThemeToggle.ForeColor = btnSettings.ForeColor;
            btnThemeToggle.FlatStyle = btnSettings.FlatStyle;
            btnThemeToggle.FlatAppearance.BorderSize = btnSettings.FlatAppearance.BorderSize;
            btnThemeToggle.FlatAppearance.BorderColor = btnSettings.FlatAppearance.BorderColor;
            btnThemeToggle.TextAlign = btnSettings.TextAlign;
            btnThemeToggle.Padding = btnSettings.Padding;
            sidePanel.Controls.Add(btnThemeToggle);
            btnThemeToggle.BringToFront();
            currentUserRole = role;
            _dashboardRepo = new DashboardRepository();

            lblRole.Text = "Logged in as: " + currentUserRole;

            WireUpEvents();
            ApplyRolePermissions();
            ApplyStyling();
            ApplyTheme();
            BuildWidgets();
            StartClock();

            this.Load += DashboardForm_Load;

            // Logout නොවී Dashboard එක වහනවා නම් මුළු Application එකම වහනවා
            this.FormClosed += (s, e) =>
            {
                clockTimer?.Stop();
                if (!isLoggingOut) Application.Exit();
            };
        }

        // ==========================================
        // 3. Button Click Events සම්බන්ධ කිරීම
        // ==========================================
        private void WireUpEvents()
        {
            btnDashboard.Click += btnDashboard_Click;
            btnPOS.Click += btnPOS_Click;
            btnProducts.Click += btnProducts_Click;
            btnCategories.Click += btnCategories_Click;
            btnSuppliers.Click += btnSuppliers_Click;
            btnPurchases.Click += btnPurchases_Click;
            btnReports.Click += btnReports_Click;
            btnExpenses.Click += btnExpenses_Click;
            btnInvoiceHistory.Click += btnInvoiceHistory_Click;
            btnSettings.Click += btnSettings_Click;
            btnLogout.Click += btnLogout_Click;
            btnCustomers.Click += btnCustomers_Click;
            btnStockAdjustment.Click += btnStockAdjustment_Click;
            btnBackupRestore.Click += btnBackupRestore_Click;
            btnThemeToggle.Click += btnThemeToggle_Click;
        }

        private void ApplyRolePermissions()
        {
            if (currentUserRole == "Cashier")
            {
                btnProducts.Visible = false;
                btnCategories.Visible = false;
                btnSuppliers.Visible = false;
                btnPurchases.Visible = false;
                btnSettings.Visible = false;
                btnStockAdjustment.Visible = false;
                btnBackupRestore.Visible = false;
            }
        }

        // ==========================================
        // 4. Dashboard Cards වලට Data Load කිරීම
        // ==========================================
        private void DashboardForm_Load(object sender, EventArgs e)
        {
            LoadDashboardMetrics();
        }

        private void LoadDashboardMetrics()
        {
            try
            {
                DashboardSummary summary = _dashboardRepo.GetSummaryForToday();

                lblTodaySales.Text = "Rs. " + summary.TodaySales.ToString("N2");
                lblInvoices.Text = summary.TodayInvoices.ToString();
                lblExpenses.Text = "Rs. " + summary.TodayExpenses.ToString("N2");
                lblLowStock.Text = summary.LowStockCount.ToString();
                LoadWidgets();
                if (AdvancedPOS.Helpers.ThemeManager.IsDarkMode)
                {
                    dgvLowStock.BackgroundColor = AdvancedPOS.Helpers.ThemeManager.GridBackground;
                    dgvLowStock.DefaultCellStyle.BackColor = AdvancedPOS.Helpers.ThemeManager.GridBackground;
                    dgvLowStock.DefaultCellStyle.ForeColor = AdvancedPOS.Helpers.ThemeManager.PrimaryTextColor;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("දත්ත ලබා ගැනීමේදී දෝෂයක් ඇති විය: " + ex.Message,
                    "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // 5. Child Form එකක් panelMain ඇතුළේ Open කිරීම
        // ==========================================
        private void openChildForm(Form childForm)
        {
            if (activeForm != null)
            {
                activeForm.Close();
            }

            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            panelMain.Controls.Add(childForm);
            panelMain.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        // ==========================================
        // 6. Menu Button Click Events
        // ==========================================
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            // Open වෙලා තියෙන Form එක වහලා Cards ආයෙත් පෙන්වනවා
            if (activeForm != null)
            {
                activeForm.Close();
                activeForm = null;
            }
            LoadDashboardMetrics();
        }

        private void btnPOS_Click(object sender, EventArgs e)
        {
            openChildForm(new Views.POSForm());
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            openChildForm(new Views.ProductForm());
        }

        private void btnCategories_Click(object sender, EventArgs e)
        {
            openChildForm(new Views.CategoryForm());
        }

        private void btnSuppliers_Click(object sender, EventArgs e)
        {
            openChildForm(new Views.SupplierForm());
        }

        private void btnPurchases_Click(object sender, EventArgs e)
        {
            openChildForm(new Views.PurchaseForm());
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            openChildForm(new Views.ReportsForm());
        }

        private void btnExpenses_Click(object sender, EventArgs e)
        {
            openChildForm(new Views.ExpenseForm());
        }

        private void btnInvoiceHistory_Click(object sender, EventArgs e)
        {
            openChildForm(new Views.InvoiceHistoryForm());
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {

            openChildForm(new Views.UserManagementForm());
        }
        private void btnCustomers_Click(object sender, EventArgs e)
        {
            openChildForm(new Views.CustomerForm());
        }

        private void btnStockAdjustment_Click(object sender, EventArgs e)
        {
            openChildForm(new Views.StockAdjustmentForm());
        }
        private void btnBackupRestore_Click(object sender, EventArgs e)
        {
            using (BackupRestoreForm form = new BackupRestoreForm())
            {
                form.ShowDialog(this);
            }
        }
        private void btnThemeToggle_Click(object sender, EventArgs e)
        {
            AdvancedPOS.Helpers.ThemeManager.IsDarkMode = !AdvancedPOS.Helpers.ThemeManager.IsDarkMode;
            btnThemeToggle.Text = AdvancedPOS.Helpers.ThemeManager.IsDarkMode ? "☀ Light Mode" : "🌙 Dark Mode";
            ApplyTheme();
        }

        // ==========================================
        // Theme එක Dashboard Shell එකට Apply කිරීම
        // ==========================================
        private void ApplyTheme()
        {
            // Sidebar
            sidePanel.BackColor = AdvancedPOS.Helpers.ThemeManager.SidebarColor;
            foreach (Control c in sidePanel.Controls)
            {
                if (c is Button b && b != btnLogout)
                {
                    b.BackColor = AdvancedPOS.Helpers.ThemeManager.SidebarColor;
                    b.FlatAppearance.MouseOverBackColor = AdvancedPOS.Helpers.ThemeManager.SidebarHoverColor;
                }
            }
            if (lblClock != null) lblClock.ForeColor = AdvancedPOS.Helpers.ThemeManager.ClockTextColor;

            // Header + Background
            topPanel.BackColor = AdvancedPOS.Helpers.ThemeManager.HeaderBackground;
            this.BackColor = AdvancedPOS.Helpers.ThemeManager.MainBackground;
            panelMain.BackColor = AdvancedPOS.Helpers.ThemeManager.MainBackground;
            lblRole.ForeColor = AdvancedPOS.Helpers.ThemeManager.PrimaryTextColor;

            // Summary Cards (Today's Sales, Invoices, Expenses, Low Stock)
            Label[] valueLabels = { lblTodaySales, lblInvoices, lblExpenses, lblLowStock };
            foreach (Label value in valueLabels)
            {
                Panel card = value.Parent as Panel;
                if (card == null) continue;

                card.BackColor = AdvancedPOS.Helpers.ThemeManager.CardBackground;
                value.ForeColor = AdvancedPOS.Helpers.ThemeManager.PrimaryTextColor;
                value.BackColor = AdvancedPOS.Helpers.ThemeManager.CardBackground;

                Label title = card.Controls.OfType<Label>().FirstOrDefault(l => l != value);
                if (title != null)
                {
                    title.ForeColor = AdvancedPOS.Helpers.ThemeManager.CardTitleColor;
                    title.BackColor = AdvancedPOS.Helpers.ThemeManager.CardBackground;
                }
            }

            // Chart + Low Stock Grid Widgets
            if (chartWeekly != null)
            {
                chartWeekly.BackColor = AdvancedPOS.Helpers.ThemeManager.ChartAreaColor;
                if (chartWeekly.ChartAreas.Count > 0)
                {
                    var area = chartWeekly.ChartAreas[0];
                    area.BackColor = AdvancedPOS.Helpers.ThemeManager.ChartAreaColor;
                    area.AxisX.LineColor = AdvancedPOS.Helpers.ThemeManager.ChartAxisColor;
                    area.AxisY.LineColor = AdvancedPOS.Helpers.ThemeManager.ChartAxisColor;
                    area.AxisY.MajorGrid.LineColor = AdvancedPOS.Helpers.ThemeManager.ChartGridLineColor;
                    area.AxisX.LabelStyle.ForeColor = AdvancedPOS.Helpers.ThemeManager.PrimaryTextColor;
                    area.AxisY.LabelStyle.ForeColor = AdvancedPOS.Helpers.ThemeManager.PrimaryTextColor;
                }
                if (chartWeekly.Titles.Count > 0)
                    chartWeekly.Titles[0].ForeColor = AdvancedPOS.Helpers.ThemeManager.PrimaryTextColor;
            }

            if (dgvLowStock != null)
            {
                dgvLowStock.BackgroundColor = AdvancedPOS.Helpers.ThemeManager.GridBackground;
                dgvLowStock.ColumnHeadersDefaultCellStyle.BackColor = AdvancedPOS.Helpers.ThemeManager.GridHeaderColor;
                dgvLowStock.DefaultCellStyle.BackColor = AdvancedPOS.Helpers.ThemeManager.GridBackground;
                dgvLowStock.DefaultCellStyle.ForeColor = AdvancedPOS.Helpers.ThemeManager.PrimaryTextColor;
                dgvLowStock.DefaultCellStyle.SelectionBackColor = AdvancedPOS.Helpers.ThemeManager.GridSelectionColor;

                // Low stock row-level red/orange highlighting ආපහු Apply කිරීම (Theme මාරු වුණාට පස්සේ මැකෙන්නේ නැතුව)
                LoadWidgets();
            }

            // Widget Cards (Chart/Grid ඇතුළත් Container Panels)
            if (panelMain.Controls.Count > 0)
            {
                foreach (Control ctrl in panelMain.Controls)
                {
                    if (ctrl is TableLayoutPanel tlp)
                    {
                        foreach (Control cardCtrl in tlp.Controls)
                        {
                            if (cardCtrl is Panel widgetCard)
                            {
                                widgetCard.BackColor = AdvancedPOS.Helpers.ThemeManager.CardBackground;
                                foreach (Control inner in widgetCard.Controls)
                                {
                                    if (inner is Label lbl)
                                    {
                                        lbl.ForeColor = AdvancedPOS.Helpers.ThemeManager.CardTitleColor;
                                        lbl.BackColor = AdvancedPOS.Helpers.ThemeManager.CardBackground;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // ==========================================
        // 7. Logout
        // ==========================================
        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?",
                "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            // Session එක clear කිරීම
            AdvancedPOS.Helpers.UserSession.CurrentUserID = 0;
            AdvancedPOS.Helpers.UserSession.CurrentUsername = null;
            AdvancedPOS.Helpers.UserSession.CurrentRole = null;

            isLoggingOut = true;

            // Hide කරලා තිබුණු Login Form එක ආපහු පෙන්වීම
            foreach (Form f in Application.OpenForms)
            {
                if (f is LoginForm)
                {
                    f.Show();
                    break;
                }
            }

            this.Close();
        }
        private void ApplyStyling()
        {
            sidePanel.BackColor = Color.FromArgb(43, 43, 54);
            // ---- Cards 4ම එකම size එකට, එක පෙළට ----
            Label[] valueLabels = { lblTodaySales, lblInvoices, lblExpenses, lblLowStock };
            int x = 20;

            foreach (Label value in valueLabels)
            {
                Panel card = value.Parent as Panel;
                if (card == null) continue;

                card.Size = new Size(250, 120);
                card.Location = new Point(x, 20);
                x += 270;

                // මාතෘකා Label එක (අගය පෙන්වන Label එක නෙවෙයි ඒක)
                Label title = card.Controls.OfType<Label>().FirstOrDefault(l => l != value);

                value.AutoSize = false;
                value.Dock = DockStyle.Fill;
                value.TextAlign = ContentAlignment.MiddleCenter;

                if (title != null)
                {
                    title.AutoSize = false;
                    title.Dock = DockStyle.Top;
                    title.Height = 40;
                    title.TextAlign = ContentAlignment.BottomCenter;
                    title.SendToBack();
                }
                value.BringToFront();
            }

            // ---- Sidebar buttons තද අළු/කළු පාටට ----
            foreach (Control c in sidePanel.Controls)
            {
                if (c is Button b && b != btnLogout)
                {
                    b.BackColor = Color.FromArgb(43, 43, 54);
                    b.FlatAppearance.MouseOverBackColor = Color.FromArgb(63, 63, 80);
                }
            }
        }
        // ==========================================
        // 8. Dashboard Widgets (Chart + Low Stock Grid)
        // ==========================================
        private Panel CreateWidgetCard(string title, Padding margin)
        {
            Panel card = new Panel();
            card.Dock = DockStyle.Fill;
            card.BackColor = Color.White;
            card.Margin = margin;
            card.Padding = new Padding(10);

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 35;
            lblTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTitle.ForeColor = Color.Gray;
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            card.Controls.Add(lblTitle);

            return card;
        }

        private void BuildWidgets()
        {
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Location = new Point(20, 160);
            layout.Size = new Size(panelMain.ClientSize.Width - 40, panelMain.ClientSize.Height - 180);
            layout.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            layout.ColumnCount = 2;
            layout.RowCount = 1;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.BackColor = Color.Transparent;

            // ---------- වම: Sales Chart ----------
            Panel chartCard = CreateWidgetCard("SALES - LAST 7 DAYS", new Padding(0, 0, 10, 0));

            chartWeekly = new Chart();
            chartWeekly.Dock = DockStyle.Fill;
            chartWeekly.BackColor = Color.White;

            ChartArea area = new ChartArea("main");
            area.BackColor = Color.White;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.LineColor = Color.LightGray;
            area.AxisY.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartWeekly.ChartAreas.Add(area);

            chartCard.Controls.Add(chartWeekly);
            chartWeekly.BringToFront();
            layout.Controls.Add(chartCard, 0, 0);

            // ---------- දකුණ: Low Stock Grid ----------
            Panel stockCard = CreateWidgetCard("LOW STOCK ITEMS", new Padding(10, 0, 0, 0));

            dgvLowStock = new DataGridView();
            dgvLowStock.Dock = DockStyle.Fill;
            dgvLowStock.BackgroundColor = Color.White;
            dgvLowStock.BorderStyle = BorderStyle.None;
            dgvLowStock.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvLowStock.RowHeadersVisible = false;
            dgvLowStock.AllowUserToAddRows = false;
            dgvLowStock.AllowUserToDeleteRows = false;
            dgvLowStock.ReadOnly = true;
            dgvLowStock.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLowStock.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLowStock.EnableHeadersVisualStyles = false;
            dgvLowStock.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvLowStock.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(43, 43, 54);
            dgvLowStock.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLowStock.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvLowStock.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvLowStock.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 240, 241);
            dgvLowStock.DefaultCellStyle.SelectionForeColor = Color.Black;

            stockCard.Controls.Add(dgvLowStock);
            dgvLowStock.BringToFront();
            layout.Controls.Add(stockCard, 1, 0);

            panelMain.Controls.Add(layout);
        }

        private void LoadWidgets()
        {
            // ---- Chart: පසුගිය දින 7 ----
            DataTable sales = _reportRepo.GetDailySalesReport(DateTime.Today.AddDays(-6), DateTime.Today);

            chartWeekly.Series.Clear();
            Series series = new Series("Sales");
            series.ChartType = SeriesChartType.Column;
            series.Color = Color.FromArgb(41, 128, 185);
            series.IsVisibleInLegend = false;

            // Report එක අලුත්ම දවස මුලින් එන නිසා reverse කරනවා
            for (int i = sales.Rows.Count - 1; i >= 0; i--)
            {
                DataRow r = sales.Rows[i];
                series.Points.AddXY(Convert.ToDateTime(r["Date"]).ToString("dd MMM"),
                                    Convert.ToDecimal(r["Total Sales"]));
            }
            chartWeekly.Series.Add(series);

            // ---- Grid: Low Stock ----
            dgvLowStock.DataSource = _reportRepo.GetLowStockReport(10);

            foreach (DataGridViewRow row in dgvLowStock.Rows)
            {
                if (row.Cells["Current Stock"].Value == null) continue;
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

        // ---- Sidebar එකේ මැද හිස් ඉඩේ Date/Time Clock එක ----
        private void StartClock()
        {
            lblClock = new Label();
            lblClock.AutoSize = false;
            lblClock.Size = new Size(sidePanel.Width, 70);
            lblClock.Location = new Point(0, sidePanel.ClientSize.Height - btnLogout.Height - 90);
            lblClock.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblClock.ForeColor = Color.Silver;
            lblClock.Font = new Font("Segoe UI", 10F);
            lblClock.TextAlign = ContentAlignment.MiddleCenter;
            sidePanel.Controls.Add(lblClock);

            clockTimer = new Timer();
            clockTimer.Interval = 1000;
            clockTimer.Tick += (s, e) => UpdateClock();
            clockTimer.Start();
            UpdateClock();
        }

        private void UpdateClock()
        {
            lblClock.Text = DateTime.Now.ToString("dddd, dd MMM yyyy")
                            + Environment.NewLine
                            + DateTime.Now.ToString("hh:mm:ss tt");
        }
    }
}