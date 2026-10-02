using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AdvancedPOS.Helpers;
using AdvancedPOS.Models;
using AdvancedPOS.Repositories;

namespace AdvancedPOS.Views
{
    public partial class StockAdjustmentForm : Form
    {
        // ==========================================
        // UI Controls (Code එකෙන්ම හදනවා)
        // ==========================================
        private ComboBox cmbProduct, cmbAdjustmentType;
        private Label lblCurrentStock;
        private TextBox txtQuantity, txtReason;
        private Button btnSaveAdjustment;
        private DateTimePicker dtpFrom, dtpTo;
        private Button btnRefreshHistory;
        private DataGridView dgvHistory;

        private readonly ProductRepository productRepo = new ProductRepository();
        private readonly StockAdjustmentRepository adjustmentRepo = new StockAdjustmentRepository();
        private readonly Color darkColor = Color.FromArgb(43, 43, 54);

        public StockAdjustmentForm()
        {
            InitializeComponent();
            BuildUI();
            LoadProducts();
            LoadHistory();
        }

        // ==========================================
        // 1. UI එක සම්පූර්ණයෙන්ම Code එකෙන් හැදීම
        // ==========================================
        private void BuildUI()
        {
            this.BackColor = Color.White;

            // ---------- වම් පැත්ත: Form Panel එක (Fixed Width) ----------
            Panel leftPanel = new Panel();
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Width = 420;
            leftPanel.BackColor = Color.White;
            leftPanel.Padding = new Padding(30, 20, 30, 20);
            leftPanel.AutoScroll = true;
            this.Controls.Add(leftPanel);

            Label lblHeading = new Label();
            lblHeading.Text = "Stock Adjustment";
            lblHeading.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(30, 20);
            leftPanel.Controls.Add(lblHeading);

            int y = 80;

            // Product
            Label lblProduct = new Label();
            lblProduct.Text = "Select Product";
            lblProduct.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblProduct.ForeColor = Color.Gray;
            lblProduct.AutoSize = true;
            lblProduct.Location = new Point(30, y);
            leftPanel.Controls.Add(lblProduct);

            cmbProduct = new ComboBox();
            cmbProduct.Font = new Font("Segoe UI", 11F);
            cmbProduct.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProduct.Width = 350;
            cmbProduct.Location = new Point(30, y + 20);
            cmbProduct.SelectedIndexChanged += cmbProduct_SelectedIndexChanged;
            leftPanel.Controls.Add(cmbProduct);

            y += 68;

            // Current Stock Display
            Panel stockCard = new Panel();
            stockCard.BackColor = Color.FromArgb(240, 242, 245);
            stockCard.Size = new Size(350, 55);
            stockCard.Location = new Point(30, y);
            leftPanel.Controls.Add(stockCard);

            Label lblStockLabel = new Label();
            lblStockLabel.Text = "CURRENT STOCK";
            lblStockLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblStockLabel.ForeColor = Color.Gray;
            lblStockLabel.AutoSize = true;
            lblStockLabel.Location = new Point(15, 8);
            stockCard.Controls.Add(lblStockLabel);

            lblCurrentStock = new Label();
            lblCurrentStock.Text = "0";
            lblCurrentStock.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblCurrentStock.ForeColor = darkColor;
            lblCurrentStock.AutoSize = true;
            lblCurrentStock.Location = new Point(15, 24);
            stockCard.Controls.Add(lblCurrentStock);

            y += 75;

            // Adjustment Type
            Label lblType = new Label();
            lblType.Text = "Adjustment Type";
            lblType.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblType.ForeColor = Color.Gray;
            lblType.AutoSize = true;
            lblType.Location = new Point(30, y);
            leftPanel.Controls.Add(lblType);

            cmbAdjustmentType = new ComboBox();
            cmbAdjustmentType.Font = new Font("Segoe UI", 11F);
            cmbAdjustmentType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAdjustmentType.Width = 350;
            cmbAdjustmentType.Location = new Point(30, y + 20);
            cmbAdjustmentType.Items.AddRange(new object[] { "Damaged", "Expired", "Lost", "Correction (Add Stock)" });
            cmbAdjustmentType.SelectedIndex = 0;
            leftPanel.Controls.Add(cmbAdjustmentType);

            y += 68;

            // Quantity
            Label lblQty = new Label();
            lblQty.Text = "Quantity";
            lblQty.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblQty.ForeColor = Color.Gray;
            lblQty.AutoSize = true;
            lblQty.Location = new Point(30, y);
            leftPanel.Controls.Add(lblQty);

            txtQuantity = new TextBox();
            txtQuantity.Font = new Font("Segoe UI", 11F);
            txtQuantity.BorderStyle = BorderStyle.FixedSingle;
            txtQuantity.Width = 350;
            txtQuantity.Location = new Point(30, y + 20);
            leftPanel.Controls.Add(txtQuantity);

            y += 68;

            // Reason
            Label lblReason = new Label();
            lblReason.Text = "Reason / Notes (Optional)";
            lblReason.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblReason.ForeColor = Color.Gray;
            lblReason.AutoSize = true;
            lblReason.Location = new Point(30, y);
            leftPanel.Controls.Add(lblReason);

            txtReason = new TextBox();
            txtReason.Font = new Font("Segoe UI", 11F);
            txtReason.BorderStyle = BorderStyle.FixedSingle;
            txtReason.Width = 350;
            txtReason.Height = 90;
            txtReason.Multiline = true;
            txtReason.Location = new Point(30, y + 20);
            leftPanel.Controls.Add(txtReason);

            y += 135;

            btnSaveAdjustment = new Button();
            btnSaveAdjustment.Text = "Save Adjustment";
            btnSaveAdjustment.Size = new Size(350, 46);
            btnSaveAdjustment.Location = new Point(30, y);
            StyleButton(btnSaveAdjustment, Color.FromArgb(231, 76, 60));
            btnSaveAdjustment.Click += btnSaveAdjustment_Click;
            leftPanel.Controls.Add(btnSaveAdjustment);

            // ---------- දකුණු පැත්ත: History Filter (Top) + Grid (Fill) ----------
            Panel rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.BackColor = Color.FromArgb(240, 242, 245);
            rightPanel.Padding = new Padding(20);
            this.Controls.Add(rightPanel);
            rightPanel.BringToFront();

            Panel filterBar = new Panel();
            filterBar.Dock = DockStyle.Top;
            filterBar.Height = 75;
            rightPanel.Controls.Add(filterBar);

            Label lblHistoryTitle = new Label();
            lblHistoryTitle.Text = "Adjustment History";
            lblHistoryTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHistoryTitle.ForeColor = darkColor;
            lblHistoryTitle.AutoSize = true;
            lblHistoryTitle.Location = new Point(0, 0);
            filterBar.Controls.Add(lblHistoryTitle);

            dtpFrom = new DateTimePicker();
            dtpFrom.Font = new Font("Segoe UI", 10F);
            dtpFrom.Format = DateTimePickerFormat.Short;
            dtpFrom.Width = 140;
            dtpFrom.Location = new Point(0, 32);
            filterBar.Controls.Add(dtpFrom);

            dtpTo = new DateTimePicker();
            dtpTo.Font = new Font("Segoe UI", 10F);
            dtpTo.Format = DateTimePickerFormat.Short;
            dtpTo.Width = 140;
            dtpTo.Location = new Point(150, 32);
            filterBar.Controls.Add(dtpTo);

            btnRefreshHistory = new Button();
            btnRefreshHistory.Text = "Filter";
            btnRefreshHistory.Size = new Size(100, 32);
            btnRefreshHistory.Location = new Point(300, 32);
            StyleButton(btnRefreshHistory, Color.FromArgb(41, 128, 185));
            btnRefreshHistory.Click += btnRefreshHistory_Click;
            filterBar.Controls.Add(btnRefreshHistory);

            // History Grid
            dgvHistory = new DataGridView();
            dgvHistory.Dock = DockStyle.Fill;
            dgvHistory.BackgroundColor = Color.White;
            dgvHistory.BorderStyle = BorderStyle.None;
            dgvHistory.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvHistory.RowHeadersVisible = false;
            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.ReadOnly = true;
            dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistory.EnableHeadersVisualStyles = false;
            dgvHistory.ColumnHeadersHeight = 40;
            dgvHistory.ColumnHeadersDefaultCellStyle.BackColor = darkColor;
            dgvHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvHistory.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvHistory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 240, 241);
            dgvHistory.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvHistory.RowTemplate.Height = 32;

            rightPanel.Controls.Add(dgvHistory);
            dgvHistory.BringToFront();

            AdvancedPOS.Helpers.ThemeManager.ApplyToForm(this,
            AdvancedPOS.Helpers.ThemeManager.MainBackground,
            AdvancedPOS.Helpers.ThemeManager.CardBackground,
            AdvancedPOS.Helpers.ThemeManager.PrimaryTextColor,
            AdvancedPOS.Helpers.ThemeManager.SecondaryTextColor);
        }

        private void StyleButton(Button btn, Color color)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        // ==========================================
        // 2. Data Loading
        // ==========================================
        private void LoadProducts()
        {
            DataTable dtProducts = productRepo.GetAllProducts();
            cmbProduct.DataSource = dtProducts;
            cmbProduct.DisplayMember = "ProductName";
            cmbProduct.ValueMember = "ProductID";
            cmbProduct.SelectedIndex = -1;
        }

        private void LoadHistory()
        {
            dtpFrom.Value = DateTime.Today.AddDays(-30);
            dtpTo.Value = DateTime.Today;
            RefreshHistoryGrid();
        }

        private void RefreshHistoryGrid()
        {
            DataTable dt = adjustmentRepo.GetAdjustmentHistory(dtpFrom.Value, dtpTo.Value);
            dgvHistory.DataSource = dt;

            // Negative (Loss) පේළි රතු පාටින්, Positive (Correction) හරිත පාටින් Highlight කිරීම
            foreach (DataGridViewRow row in dgvHistory.Rows)
            {
                if (row.Cells["Qty Change"].Value == null) continue;
                int qty = Convert.ToInt32(row.Cells["Qty Change"].Value);

                if (qty < 0)
                {
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                }
                else
                {
                    row.DefaultCellStyle.ForeColor = Color.DarkGreen;
                }
            }
        }

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedValue == null) return;

            int productId = Convert.ToInt32(cmbProduct.SelectedValue);
            int stock = adjustmentRepo.GetCurrentStock(productId);
            lblCurrentStock.Text = stock.ToString();
        }

        // ==========================================
        // 3. Save Logic
        // ==========================================
        private void btnSaveAdjustment_Click(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedValue == null)
            {
                MessageBox.Show("Please select a product.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out int qty) || qty <= 0)
            {
                MessageBox.Show("Please enter a valid quantity greater than zero.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedType = cmbAdjustmentType.SelectedItem.ToString();
            bool isCorrection = selectedType == "Correction (Add Stock)";

            // Correction නම් Stock එකට එකතු වෙනවා (+), අනිත් ඒවා නම් අඩුවෙනවා (-)
            int quantityChanged = isCorrection ? qty : -qty;

            int productId = Convert.ToInt32(cmbProduct.SelectedValue);
            int currentStock = adjustmentRepo.GetCurrentStock(productId);

            if (!isCorrection && qty > currentStock)
            {
                MessageBox.Show($"Cannot remove {qty} units. Only {currentStock} units are in stock.", "Insufficient Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string adjustmentType = isCorrection ? "Correction" : selectedType;

            StockAdjustment adjustment = new StockAdjustment
            {
                ProductID = productId,
                AdjustmentType = adjustmentType,
                QuantityChanged = quantityChanged,
                Reason = txtReason.Text.Trim(),
                UserID = UserSession.CurrentUserID
            };

            try
            {
                if (adjustmentRepo.SaveAdjustment(adjustment))
                {
                    MessageBox.Show("Stock adjustment saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    RefreshHistoryGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving adjustment: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefreshHistory_Click(object sender, EventArgs e)
        {
            RefreshHistoryGrid();
        }

        private void ClearFields()
        {
            cmbProduct.SelectedIndex = -1;
            cmbAdjustmentType.SelectedIndex = 0;
            txtQuantity.Clear();
            txtReason.Clear();
            lblCurrentStock.Text = "0";
        }
    }
}