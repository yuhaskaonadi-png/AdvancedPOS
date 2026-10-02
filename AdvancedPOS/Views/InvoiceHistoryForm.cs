using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AdvancedPOS.Models;
using AdvancedPOS.Repositories;
using AdvancedPOS.Helpers;

namespace AdvancedPOS.Views
{
    public partial class InvoiceHistoryForm : Form
    {
        // ==========================================
        // UI Controls (Code එකෙන්ම හදනවා)
        // ==========================================
        private DateTimePicker dtpFromDate, dtpToDate;
        private Button btnSearch, btnReprint, btnReturnItem;
        private DataGridView dgvSales, dgvSaleDetails;

        private readonly SaleRepository saleRepo = new SaleRepository();
        private int selectedSaleID = 0;
        private readonly Color darkColor = Color.FromArgb(43, 43, 54);

        public InvoiceHistoryForm()
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
            filterPanel.Height = 90;
            filterPanel.BackColor = Color.FromArgb(240, 242, 245);
            filterPanel.Padding = new Padding(30, 15, 30, 15);
            this.Controls.Add(filterPanel);

            Label lblHeading = new Label();
            lblHeading.Text = "Invoice History";
            lblHeading.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(0, 0);
            filterPanel.Controls.Add(lblHeading);

            int x = 0;
            int fieldY = 35;

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

            btnSearch = new Button();
            btnSearch.Text = "Search";
            btnSearch.Size = new Size(130, 38);
            btnSearch.Location = new Point(x, fieldY + 16);
            StyleButton(btnSearch, Color.FromArgb(41, 128, 185));
            filterPanel.Controls.Add(btnSearch);

            // ---------- පහළ: Sales Grid (උඩ) + Details Grid (යට) ----------
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 1;
            mainLayout.RowCount = 2;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            mainLayout.BackColor = Color.FromArgb(240, 242, 245);
            mainLayout.Padding = new Padding(20);
            this.Controls.Add(mainLayout);
            mainLayout.BringToFront();

            // ---- Sales List (Top) ----
            Panel salesCard = new Panel();
            salesCard.Dock = DockStyle.Fill;
            salesCard.Margin = new Padding(0, 0, 0, 10);
            salesCard.BackColor = Color.White;
            salesCard.Padding = new Padding(15);

            Label lblSalesTitle = new Label();
            lblSalesTitle.Text = "Sales List";
            lblSalesTitle.Dock = DockStyle.Top;
            lblSalesTitle.Height = 30;
            lblSalesTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblSalesTitle.ForeColor = Color.Gray;
            salesCard.Controls.Add(lblSalesTitle);

            dgvSales = new DataGridView();
            dgvSales.Dock = DockStyle.Fill;
            StyleGrid(dgvSales);
            dgvSales.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSales.MultiSelect = false;

            salesCard.Controls.Add(dgvSales);
            dgvSales.BringToFront();
            mainLayout.Controls.Add(salesCard, 0, 0);

            // ---- Sale Details (Bottom) ----
            Panel detailsCard = new Panel();
            detailsCard.Dock = DockStyle.Fill;
            detailsCard.Margin = new Padding(0, 10, 0, 0);
            detailsCard.BackColor = Color.White;
            detailsCard.Padding = new Padding(15);

            // Details title bar with buttons
            Panel detailsTop = new Panel();
            detailsTop.Dock = DockStyle.Top;
            detailsTop.Height = 45;
            detailsCard.Controls.Add(detailsTop);

            Label lblDetailsTitle = new Label();
            lblDetailsTitle.Text = "Invoice Details";
            lblDetailsTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDetailsTitle.ForeColor = Color.Gray;
            lblDetailsTitle.AutoSize = true;
            lblDetailsTitle.Location = new Point(0, 8);
            detailsTop.Controls.Add(lblDetailsTitle);

            btnReturnItem = new Button();
            btnReturnItem.Text = "Return Item";
            btnReturnItem.Size = new Size(140, 34);
            StyleButton(btnReturnItem, Color.FromArgb(243, 156, 18));
            btnReturnItem.Click += btnReturnItem_Click;
            detailsTop.Controls.Add(btnReturnItem);

            btnReprint = new Button();
            btnReprint.Text = "🖨 Reprint";
            btnReprint.Size = new Size(140, 34);
            StyleButton(btnReprint, Color.FromArgb(39, 174, 96));
            detailsTop.Controls.Add(btnReprint);

            detailsTop.Resize += (s, e) =>
            {
                btnReprint.Location = new Point(detailsTop.ClientSize.Width - btnReprint.Width, 5);
                btnReturnItem.Location = new Point(detailsTop.ClientSize.Width - btnReprint.Width - 150, 5);
            };

            dgvSaleDetails = new DataGridView();
            dgvSaleDetails.Dock = DockStyle.Fill;
            StyleGrid(dgvSaleDetails);
            dgvSaleDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSaleDetails.MultiSelect = false;

            detailsCard.Controls.Add(dgvSaleDetails);
            dgvSaleDetails.BringToFront();
            mainLayout.Controls.Add(detailsCard, 0, 1);

            AdvancedPOS.Helpers.ThemeManager.ApplyToForm(this,
            AdvancedPOS.Helpers.ThemeManager.MainBackground,
            AdvancedPOS.Helpers.ThemeManager.CardBackground,
            AdvancedPOS.Helpers.ThemeManager.PrimaryTextColor,
            AdvancedPOS.Helpers.ThemeManager.SecondaryTextColor);
        }

        private void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersHeight = 38;
            grid.ColumnHeadersDefaultCellStyle.BackColor = darkColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 240, 241);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.RowTemplate.Height = 30;
        }

        private void StyleButton(Button btn, Color color)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        // ==========================================
        // 2. Form Setup (ඔයාගේ පරණ code එකම)
        // ==========================================
        private void SetupForm()
        {
            dtpFromDate.Value = DateTime.Today;
            dtpToDate.Value = DateTime.Today;

            btnSearch.Click += BtnSearch_Click;
            dgvSales.SelectionChanged += DgvSales_SelectionChanged;
            btnReprint.Click += BtnReprint_Click;

            LoadSales();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadSales();
        }

        private void LoadSales()
        {
            try
            {
                DataTable dtSales = saleRepo.GetSalesHistory(dtpFromDate.Value, dtpToDate.Value);
                dgvSales.DataSource = dtSales;

                dgvSaleDetails.DataSource = null;
                selectedSaleID = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sales history: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvSales_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSales.CurrentRow != null && dgvSales.CurrentRow.Index > -1)
            {
                selectedSaleID = Convert.ToInt32(dgvSales.CurrentRow.Cells["SaleID"].Value);
                LoadSaleDetails(selectedSaleID);
            }
        }

        private void LoadSaleDetails(int saleId)
        {
            try
            {
                var details = saleRepo.GetSaleDetails(saleId);
                dgvSaleDetails.DataSource = details;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sale details: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReprint_Click(object sender, EventArgs e)
        {
            if (selectedSaleID == 0)
            {
                MessageBox.Show("Please select an invoice from the top list first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Sale sale = saleRepo.GetSaleByID(selectedSaleID);
                List<SaleDetail> details = saleRepo.GetSaleDetails(selectedSaleID);
                List<SalePayment> payments = saleRepo.GetSalePayments(selectedSaleID);

                ReceiptPrinter printer = new ReceiptPrinter(sale, details, payments);
                printer.PrintReceipt();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reprinting invoice: " + ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReturnItem_Click(object sender, EventArgs e)
        {
            if (dgvSaleDetails.CurrentRow == null || dgvSaleDetails.CurrentRow.Index < 0)
            {
                MessageBox.Show("Please select an item from the Details list to return.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int productId = Convert.ToInt32(dgvSaleDetails.CurrentRow.Cells["ProductID"].Value);
                string productName = dgvSaleDetails.CurrentRow.Cells["ProductName"].Value != null ? dgvSaleDetails.CurrentRow.Cells["ProductName"].Value.ToString() : "Unknown Item";
                decimal unitPrice = Convert.ToDecimal(dgvSaleDetails.CurrentRow.Cells["UnitPrice"].Value);

                int boughtQty = Convert.ToInt32(dgvSaleDetails.CurrentRow.Cells["Quantity"].Value);
                int returnedQty = Convert.ToInt32(dgvSaleDetails.CurrentRow.Cells["ReturnedQty"].Value);

                int maxReturnable = boughtQty - returnedQty;

                if (maxReturnable <= 0)
                {
                    MessageBox.Show("This item has already been fully returned.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (ReturnItemForm returnForm = new ReturnItemForm(selectedSaleID, productId, productName, unitPrice, maxReturnable))
                {
                    if (returnForm.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadSaleDetails(selectedSaleID);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing return selection: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}