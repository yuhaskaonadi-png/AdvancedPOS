using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AdvancedPOS.Helpers;
using AdvancedPOS.Models;
using AdvancedPOS.Repositories;

namespace AdvancedPOS.Views
{
    public partial class PurchaseForm : Form
    {
        // ==========================================
        // UI Controls (Code එකෙන්ම හදනවා)
        // ==========================================
        private ComboBox cmbSelectSupplier, cmbSelectProduct;
        private TextBox txtItemCost, txtItemQty;
        private Button btnAddItemToCart, btnRemoveCartItem, btnSaveStockIn;
        private Label lblGrandTotal;
        private DataGridView dgvCart;

        private readonly SupplierRepository supplierRepo = new SupplierRepository();
        private readonly ProductRepository productRepo = new ProductRepository();
        private readonly PurchaseRepository purchaseRepo = new PurchaseRepository();

        private DataTable cartTable = new DataTable();
        private decimal grandTotal = 0;
        private readonly Color darkColor = Color.FromArgb(43, 43, 54);

        public PurchaseForm()
        {
            InitializeComponent();
            BuildUI();
            SetupCartTable();
            LoadDropdownData();
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
            lblHeading.Text = "New Purchase";
            lblHeading.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(30, 20);
            leftPanel.Controls.Add(lblHeading);

            int y = 75;

            // Supplier ComboBox
            Label lblSupplier = new Label();
            lblSupplier.Text = "Supplier";
            lblSupplier.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblSupplier.ForeColor = Color.Gray;
            lblSupplier.AutoSize = true;
            lblSupplier.Location = new Point(30, y);
            leftPanel.Controls.Add(lblSupplier);

            cmbSelectSupplier = new ComboBox();
            cmbSelectSupplier.Font = new Font("Segoe UI", 11F);
            cmbSelectSupplier.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSelectSupplier.Width = 350;
            cmbSelectSupplier.Location = new Point(30, y + 20);
            leftPanel.Controls.Add(cmbSelectSupplier);

            y += 75;

            // ---- Divider ----
            Panel divider1 = new Panel();
            divider1.BackColor = Color.FromArgb(230, 230, 230);
            divider1.Size = new Size(350, 1);
            divider1.Location = new Point(30, y);
            leftPanel.Controls.Add(divider1);

            y += 15;

            Label lblAddItem = new Label();
            lblAddItem.Text = "ADD ITEM";
            lblAddItem.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblAddItem.ForeColor = darkColor;
            lblAddItem.AutoSize = true;
            lblAddItem.Location = new Point(30, y);
            leftPanel.Controls.Add(lblAddItem);

            y += 35;

            // Product ComboBox
            Label lblProduct = new Label();
            lblProduct.Text = "Product";
            lblProduct.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblProduct.ForeColor = Color.Gray;
            lblProduct.AutoSize = true;
            lblProduct.Location = new Point(30, y);
            leftPanel.Controls.Add(lblProduct);

            cmbSelectProduct = new ComboBox();
            cmbSelectProduct.Font = new Font("Segoe UI", 11F);
            cmbSelectProduct.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSelectProduct.Width = 350;
            cmbSelectProduct.Location = new Point(30, y + 20);
            leftPanel.Controls.Add(cmbSelectProduct);

            y += 68;

            // Cost Price
            Label lblCost = new Label();
            lblCost.Text = "Cost Price";
            lblCost.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblCost.ForeColor = Color.Gray;
            lblCost.AutoSize = true;
            lblCost.Location = new Point(30, y);
            leftPanel.Controls.Add(lblCost);

            txtItemCost = new TextBox();
            txtItemCost.Font = new Font("Segoe UI", 11F);
            txtItemCost.BorderStyle = BorderStyle.FixedSingle;
            txtItemCost.Width = 165;
            txtItemCost.Location = new Point(30, y + 20);
            leftPanel.Controls.Add(txtItemCost);

            // Quantity (side by side with Cost Price)
            Label lblQty = new Label();
            lblQty.Text = "Quantity";
            lblQty.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblQty.ForeColor = Color.Gray;
            lblQty.AutoSize = true;
            lblQty.Location = new Point(215, y);
            leftPanel.Controls.Add(lblQty);

            txtItemQty = new TextBox();
            txtItemQty.Font = new Font("Segoe UI", 11F);
            txtItemQty.BorderStyle = BorderStyle.FixedSingle;
            txtItemQty.Width = 165;
            txtItemQty.Location = new Point(215, y + 20);
            leftPanel.Controls.Add(txtItemQty);

            y += 68;

            // Add Item Button
            btnAddItemToCart = new Button();
            btnAddItemToCart.Text = "+ Add Item to Cart";
            btnAddItemToCart.Size = new Size(350, 42);
            btnAddItemToCart.Location = new Point(30, y);
            StyleButton(btnAddItemToCart, Color.FromArgb(41, 128, 185));
            btnAddItemToCart.Click += BtnAddItemToCart_Click;
            leftPanel.Controls.Add(btnAddItemToCart);

            y += 60;

            // ---- Divider ----
            Panel divider2 = new Panel();
            divider2.BackColor = Color.FromArgb(230, 230, 230);
            divider2.Size = new Size(350, 1);
            divider2.Location = new Point(30, y);
            leftPanel.Controls.Add(divider2);

            y += 25;

            // Grand Total
            lblGrandTotal = new Label();
            lblGrandTotal.Text = "Total: Rs. 0.00";
            lblGrandTotal.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblGrandTotal.ForeColor = Color.FromArgb(39, 174, 96);
            lblGrandTotal.AutoSize = true;
            lblGrandTotal.Location = new Point(30, y);
            leftPanel.Controls.Add(lblGrandTotal);

            y += 55;

            // Save Button
            btnSaveStockIn = new Button();
            btnSaveStockIn.Text = "SAVE PURCHASE";
            btnSaveStockIn.Size = new Size(350, 48);
            btnSaveStockIn.Location = new Point(30, y);
            StyleButton(btnSaveStockIn, Color.FromArgb(39, 174, 96));
            btnSaveStockIn.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnSaveStockIn.Click += BtnSaveStockIn_Click;
            leftPanel.Controls.Add(btnSaveStockIn);

            // ---------- දකුණු පැත්ත: Cart Grid Container (Dock=Fill) ----------
            Panel rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.BackColor = Color.FromArgb(240, 242, 245);
            rightPanel.Padding = new Padding(20);
            this.Controls.Add(rightPanel);
            rightPanel.BringToFront();

            // Top bar with Remove button
            Panel topBar = new Panel();
            topBar.Dock = DockStyle.Top;
            topBar.Height = 55;
            rightPanel.Controls.Add(topBar);

            Label lblCartTitle = new Label();
            lblCartTitle.Text = "Purchase Cart";
            lblCartTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblCartTitle.ForeColor = darkColor;
            lblCartTitle.AutoSize = true;
            lblCartTitle.Location = new Point(0, 10);
            topBar.Controls.Add(lblCartTitle);

            btnRemoveCartItem = new Button();
            btnRemoveCartItem.Text = "Remove Selected Item";
            btnRemoveCartItem.Size = new Size(190, 38);
            btnRemoveCartItem.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            StyleButton(btnRemoveCartItem, Color.FromArgb(231, 76, 60));
            btnRemoveCartItem.Click += BtnRemoveCartItem_Click;
            topBar.Controls.Add(btnRemoveCartItem);

            topBar.Resize += (s, e) =>
            {
                btnRemoveCartItem.Location = new Point(topBar.ClientSize.Width - btnRemoveCartItem.Width, 8);
            };

            // Cart Grid
            dgvCart = new DataGridView();
            dgvCart.Dock = DockStyle.Fill;
            dgvCart.BackgroundColor = Color.White;
            dgvCart.BorderStyle = BorderStyle.None;
            dgvCart.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCart.RowHeadersVisible = false;
            dgvCart.AllowUserToAddRows = false;
            dgvCart.ReadOnly = true;
            dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCart.MultiSelect = false;
            dgvCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCart.EnableHeadersVisualStyles = false;
            dgvCart.ColumnHeadersHeight = 40;
            dgvCart.ColumnHeadersDefaultCellStyle.BackColor = darkColor;
            dgvCart.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCart.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvCart.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvCart.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 240, 241);
            dgvCart.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvCart.RowTemplate.Height = 32;

            rightPanel.Controls.Add(dgvCart);
            dgvCart.BringToFront();

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
        // 2. Cart Table Setup (ඔයාගේ පරණ code එකම)
        // ==========================================
        private void SetupCartTable()
        {
            cartTable.Columns.Add("ProductID", typeof(int));
            cartTable.Columns.Add("ProductName", typeof(string));
            cartTable.Columns.Add("CostPrice", typeof(decimal));
            cartTable.Columns.Add("Quantity", typeof(int));
            cartTable.Columns.Add("LineTotal", typeof(decimal));

            dgvCart.DataSource = cartTable;

            if (dgvCart.Columns.Contains("ProductID"))
            {
                dgvCart.Columns["ProductID"].Visible = false;
            }
        }

        // ==========================================
        // 3. Dropdown Loading (ඔයාගේ පරණ code එකම)
        // ==========================================
        private void LoadDropdownData()
        {
            DataTable dtSuppliers = supplierRepo.GetAllSuppliers();
            cmbSelectSupplier.DataSource = dtSuppliers;
            cmbSelectSupplier.DisplayMember = "SupplierName";
            cmbSelectSupplier.ValueMember = "SupplierID";
            cmbSelectSupplier.SelectedIndex = -1;

            DataTable dtProducts = productRepo.GetAllProducts();
            cmbSelectProduct.DataSource = dtProducts;
            cmbSelectProduct.DisplayMember = "ProductName";
            cmbSelectProduct.ValueMember = "ProductID";
            cmbSelectProduct.SelectedIndex = -1;
        }

        // ==========================================
        // 4. Business Logic (ඔයාගේ පරණ code එකම)
        // ==========================================
        private void BtnAddItemToCart_Click(object sender, EventArgs e)
        {
            if (cmbSelectProduct.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a product from the list.", "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtItemCost.Text.Trim(), out decimal costPrice) || costPrice <= 0)
            {
                MessageBox.Show("Please enter a valid cost price greater than zero.", "Invalid Cost Price", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtItemCost.Focus();
                return;
            }

            if (!int.TryParse(txtItemQty.Text.Trim(), out int qty) || qty <= 0)
            {
                MessageBox.Show("Please enter a valid quantity greater than zero.", "Invalid Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtItemQty.Focus();
                return;
            }

            int productId = Convert.ToInt32(cmbSelectProduct.SelectedValue);
            string productName = cmbSelectProduct.Text;
            decimal lineTotal = costPrice * qty;

            DataRow[] existingRows = cartTable.Select($"ProductID = {productId}");
            if (existingRows.Length > 0)
            {
                int currentQty = Convert.ToInt32(existingRows[0]["Quantity"]);
                int newQty = currentQty + qty;
                existingRows[0]["Quantity"] = newQty;
                existingRows[0]["CostPrice"] = costPrice;
                existingRows[0]["LineTotal"] = newQty * costPrice;
            }
            else
            {
                cartTable.Rows.Add(productId, productName, costPrice, qty, lineTotal);
            }

            CalculateGrandTotal();

            cmbSelectProduct.SelectedIndex = -1;
            txtItemCost.Clear();
            txtItemQty.Clear();
            cmbSelectProduct.Focus();
        }

        private void BtnRemoveCartItem_Click(object sender, EventArgs e)
        {
            if (dgvCart.SelectedRows.Count > 0)
            {
                int rowIndex = dgvCart.SelectedRows[0].Index;
                cartTable.Rows.RemoveAt(rowIndex);
                CalculateGrandTotal();
            }
            else
            {
                MessageBox.Show("Please select an item from the table to remove.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CalculateGrandTotal()
        {
            grandTotal = 0;
            foreach (DataRow row in cartTable.Rows)
            {
                grandTotal += Convert.ToDecimal(row["LineTotal"]);
            }
            lblGrandTotal.Text = $"Total: Rs. {grandTotal:N2}";
        }

        private void BtnSaveStockIn_Click(object sender, EventArgs e)
        {
            if (cmbSelectSupplier.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a supplier before saving the purchase.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbSelectSupplier.Focus();
                return;
            }

            if (cartTable.Rows.Count == 0)
            {
                MessageBox.Show("No items have been added to the purchase order. Please add at least one item.", "Empty Order", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Purchase purchase = new Purchase
            {
                SupplierID = Convert.ToInt32(cmbSelectSupplier.SelectedValue),
                PurchaseDate = DateTime.Now,
                TotalAmount = grandTotal,
                UserID = UserSession.CurrentUserID
            };

            foreach (DataRow row in cartTable.Rows)
            {
                purchase.Details.Add(new PurchaseDetail
                {
                    ProductID = Convert.ToInt32(row["ProductID"]),
                    CostPrice = Convert.ToDecimal(row["CostPrice"]),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    LineTotal = Convert.ToDecimal(row["LineTotal"])
                });
            }

            try
            {
                if (purchaseRepo.SavePurchaseTransaction(purchase))
                {
                    MessageBox.Show("Purchase order saved successfully! Product stock has been updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred while processing the purchase transaction:\n" + ex.Message, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetForm()
        {
            cartTable.Rows.Clear();
            cmbSelectSupplier.SelectedIndex = -1;
            cmbSelectProduct.SelectedIndex = -1;
            txtItemCost.Clear();
            txtItemQty.Clear();
            grandTotal = 0;
            lblGrandTotal.Text = "Total: Rs. 0.00";
        }
    }
}