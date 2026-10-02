using System;
using System.Drawing;
using System.Windows.Forms;

namespace AdvancedPOS.Views
{
    public partial class ProductForm : Form
    {
        // ==========================================
        // UI Controls (Code එකෙන්ම හදනවා)
        // ==========================================
        private TextBox txtBarcode, txtProductName, txtPurchasePrice, txtSellingPrice, txtStockQuantity, txtReorderLevel;
        private ComboBox cmbCategory;
        private Button btnSave, btnUpdate, btnDelete, btnPrintBarcode;
        private DataGridView dgvProducts;
        private TextBox txtDiscountPercent;

        private int selectedProductId = 0;
        private readonly Color darkColor = Color.FromArgb(43, 43, 54);

        public ProductForm()
        {
            InitializeComponent();
            BuildUI();
            LoadCategories();
            LoadProductData();
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
            lblHeading.Text = "Manage Products";
            lblHeading.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(30, 20);
            leftPanel.Controls.Add(lblHeading);

            int y = 80;

            txtBarcode = AddField(leftPanel, "Barcode", ref y);
            txtProductName = AddField(leftPanel, "Product Name", ref y);

            cmbCategory = AddComboField(leftPanel, "Category", ref y);

            txtPurchasePrice = AddField(leftPanel, "Purchase Price", ref y);
            txtSellingPrice = AddField(leftPanel, "Selling Price", ref y);
            txtStockQuantity = AddField(leftPanel, "Stock Quantity", ref y);
            txtReorderLevel = AddField(leftPanel, "Reorder Level", ref y);
            txtDiscountPercent = AddField(leftPanel, "Discount % (0 = No Discount)", ref y);
            y += 15;

            // ---------- Buttons ----------
            btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Size = new Size(110, 42);
            btnSave.Location = new Point(30, y);
            StyleButton(btnSave, Color.FromArgb(39, 174, 96));
            btnSave.Click += btnSave_Click;
            leftPanel.Controls.Add(btnSave);

            btnUpdate = new Button();
            btnUpdate.Text = "Update";
            btnUpdate.Size = new Size(110, 42);
            btnUpdate.Location = new Point(150, y);
            StyleButton(btnUpdate, Color.FromArgb(243, 156, 18));
            btnUpdate.Click += btnUpdate_Click;
            leftPanel.Controls.Add(btnUpdate);

            btnDelete = new Button();
            btnDelete.Text = "Delete";
            btnDelete.Size = new Size(110, 42);
            btnDelete.Location = new Point(270, y);
            StyleButton(btnDelete, Color.FromArgb(231, 76, 60));
            btnDelete.Click += btnDelete_Click;
            leftPanel.Controls.Add(btnDelete);

            y += 55;

            btnPrintBarcode = new Button();
            btnPrintBarcode.Text = "🏷 Print Barcode";
            btnPrintBarcode.Size = new Size(350, 42);
            btnPrintBarcode.Location = new Point(30, y);
            StyleButton(btnPrintBarcode, Color.FromArgb(155, 89, 182));
            btnPrintBarcode.Click += btnPrintBarcode_Click;
            leftPanel.Controls.Add(btnPrintBarcode);

            // ---------- දකුණු පැත්ත: Grid Container (Dock=Fill, Auto-Resize) ----------
            Panel rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.BackColor = Color.FromArgb(240, 242, 245);
            rightPanel.Padding = new Padding(20);
            this.Controls.Add(rightPanel);
            rightPanel.BringToFront();

            dgvProducts = new DataGridView();
            dgvProducts.Dock = DockStyle.Fill;
            dgvProducts.BackgroundColor = Color.White;
            dgvProducts.BorderStyle = BorderStyle.None;
            dgvProducts.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvProducts.RowHeadersVisible = false;
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.ReadOnly = true;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProducts.EnableHeadersVisualStyles = false;
            dgvProducts.ColumnHeadersHeight = 40;
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = darkColor;
            dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvProducts.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvProducts.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvProducts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 240, 241);
            dgvProducts.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvProducts.RowTemplate.Height = 32;
            dgvProducts.CellDoubleClick += dgvProducts_CellDoubleClick;

            rightPanel.Controls.Add(dgvProducts);

            AdvancedPOS.Helpers.ThemeManager.ApplyToForm(this,
            AdvancedPOS.Helpers.ThemeManager.MainBackground,
            AdvancedPOS.Helpers.ThemeManager.CardBackground,
            AdvancedPOS.Helpers.ThemeManager.PrimaryTextColor,
            AdvancedPOS.Helpers.ThemeManager.SecondaryTextColor);
        }

        // Label + TextBox යුගලයක් හදන Helper
        private TextBox AddField(Panel parent, string labelText, ref int y)
        {
            Label lbl = new Label();
            lbl.Text = labelText;
            lbl.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lbl.ForeColor = Color.Gray;
            lbl.AutoSize = true;
            lbl.Location = new Point(30, y);
            parent.Controls.Add(lbl);

            TextBox txt = new TextBox();
            txt.Font = new Font("Segoe UI", 11F);
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Width = 350;
            txt.Location = new Point(30, y + 20);
            parent.Controls.Add(txt);

            y += 68;
            return txt;
        }

        // Label + ComboBox යුගලයක් හදන Helper
        private ComboBox AddComboField(Panel parent, string labelText, ref int y)
        {
            Label lbl = new Label();
            lbl.Text = labelText;
            lbl.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lbl.ForeColor = Color.Gray;
            lbl.AutoSize = true;
            lbl.Location = new Point(30, y);
            parent.Controls.Add(lbl);

            ComboBox cmb = new ComboBox();
            cmb.Font = new Font("Segoe UI", 11F);
            cmb.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb.Width = 350;
            cmb.Location = new Point(30, y + 20);
            parent.Controls.Add(cmb);

            y += 68;
            return cmb;
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
        // 2. Data Loading Logic 
        // ==========================================
        private void LoadCategories()
        {
            AdvancedPOS.Repositories.CategoryRepository catRepo = new AdvancedPOS.Repositories.CategoryRepository();
            cmbCategory.DataSource = catRepo.GetCategories();
            cmbCategory.DisplayMember = "CategoryName";
            cmbCategory.ValueMember = "CategoryID";
            cmbCategory.SelectedIndex = -1;
        }

        private void LoadProductData()
        {
            AdvancedPOS.Repositories.ProductRepository prodRepo = new AdvancedPOS.Repositories.ProductRepository();
            dgvProducts.DataSource = prodRepo.GetProducts();
        }

        // ==========================================
        // 3. Save / Update / Delete Logic 
        // ==========================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProductName.Text) || cmbCategory.SelectedIndex == -1)
            {
                MessageBox.Show("Product Name and Category are required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // නිවැරදි කර ඇති Validation කොටස
            if (!decimal.TryParse(txtPurchasePrice.Text, out decimal pPrice) ||
                !decimal.TryParse(txtSellingPrice.Text, out decimal sPrice) ||
                !int.TryParse(txtStockQuantity.Text, out int qty) ||
                !int.TryParse(txtReorderLevel.Text, out int reorder))
            {
                MessageBox.Show("Please enter valid numbers for Prices and Quantities!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Discount එක ලබා ගැනීම (හිස්ව තිබුණොත් 0 ලෙස සලකයි)
            decimal.TryParse(txtDiscountPercent.Text.Trim(), out decimal discountPercent);

            AdvancedPOS.Models.Product newProduct = new AdvancedPOS.Models.Product
            {
                Barcode = txtBarcode.Text.Trim(),
                ProductName = txtProductName.Text.Trim(),
                CategoryID = Convert.ToInt32(cmbCategory.SelectedValue),
                PurchasePrice = pPrice,
                SellingPrice = sPrice,
                StockQuantity = qty,
                ReorderLevel = reorder,
                DiscountPercent = discountPercent // අලුතින් එකතු කළ කොටස
            };

            AdvancedPOS.Repositories.ProductRepository prodRepo = new AdvancedPOS.Repositories.ProductRepository();
            bool success = prodRepo.AddProduct(newProduct);

            if (success)
            {
                MessageBox.Show("Product saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtBarcode.Clear();
                txtProductName.Clear();
                cmbCategory.SelectedIndex = -1;
                txtPurchasePrice.Clear();
                txtSellingPrice.Clear();
                txtStockQuantity.Clear();
                txtReorderLevel.Clear();
                txtDiscountPercent.Clear(); // මෙයත් හිස් කිරීම හොඳයි
                LoadProductData();
            }
            else
            {
                MessageBox.Show("Failed to save product.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvProducts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvProducts.Rows[e.RowIndex];

                selectedProductId = Convert.ToInt32(row.Cells["ProductID"].Value);
                txtBarcode.Text = row.Cells["Barcode"].Value.ToString();
                txtProductName.Text = row.Cells["ProductName"].Value.ToString();
                cmbCategory.Text = row.Cells["CategoryName"].Value.ToString();
                txtPurchasePrice.Text = row.Cells["PurchasePrice"].Value.ToString();
                txtSellingPrice.Text = row.Cells["SellingPrice"].Value.ToString();
                txtStockQuantity.Text = row.Cells["StockQuantity"].Value.ToString();
                txtReorderLevel.Text = row.Cells["ReorderLevel"].Value.ToString();

                // Grid එකෙන් Discount Percent අගය Textbox එකට දැමීම
                txtDiscountPercent.Text = row.Cells["DiscountPercent"].Value?.ToString() ?? "0";
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedProductId == 0)
            {
                MessageBox.Show("Please double-click a product from the table to update!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!decimal.TryParse(txtPurchasePrice.Text, out decimal pPrice) ||
                !decimal.TryParse(txtSellingPrice.Text, out decimal sPrice) ||
                !int.TryParse(txtStockQuantity.Text, out int qty) ||
                !int.TryParse(txtReorderLevel.Text, out int reorder))
            {
                MessageBox.Show("Please enter valid numbers!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Update කිරීම සඳහාත් Discount අගය ලබා ගැනීම
            decimal.TryParse(txtDiscountPercent.Text.Trim(), out decimal discountPercent);

            AdvancedPOS.Models.Product updateProduct = new AdvancedPOS.Models.Product
            {
                ProductID = selectedProductId,
                Barcode = txtBarcode.Text.Trim(),
                ProductName = txtProductName.Text.Trim(),
                CategoryID = Convert.ToInt32(cmbCategory.SelectedValue),
                PurchasePrice = pPrice,
                SellingPrice = sPrice,
                StockQuantity = qty,
                ReorderLevel = reorder,
                DiscountPercent = discountPercent // අලුතින් එකතු කළ කොටස
            };

            AdvancedPOS.Repositories.ProductRepository prodRepo = new AdvancedPOS.Repositories.ProductRepository();
            if (prodRepo.UpdateProduct(updateProduct))
            {
                MessageBox.Show("Product updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                selectedProductId = 0;
                LoadProductData();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedProductId == 0)
            {
                MessageBox.Show("Please double-click a product from the table to delete!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dialogResult == DialogResult.Yes)
            {
                AdvancedPOS.Repositories.ProductRepository prodRepo = new AdvancedPOS.Repositories.ProductRepository();
                if (prodRepo.DeleteProduct(selectedProductId))
                {
                    MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    selectedProductId = 0;
                    LoadProductData();
                }
            }
        }

        private void btnPrintBarcode_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBarcode.Text))
            {
                MessageBox.Show("This product doesn't have a barcode. Please enter one first.", "No Barcode", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Please select or enter a product first.", "No Product", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string price = string.IsNullOrWhiteSpace(txtSellingPrice.Text) ? "0.00" : txtSellingPrice.Text;

            using (BarcodePrintForm barcodeForm = new BarcodePrintForm(txtBarcode.Text.Trim(), txtProductName.Text.Trim(), price))
            {
                barcodeForm.ShowDialog(this);
            }
        }
    }
}