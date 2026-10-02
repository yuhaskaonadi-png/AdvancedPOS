using AdvancedPOS.Helpers;
using AdvancedPOS.Models;
using AdvancedPOS.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace AdvancedPOS.Views
{
    public partial class POSForm : Form
    {
        // ==========================================
        // UI Controls (Code එකෙන්ම හදනවා)
        // ==========================================
        private TextBox textBox1;      // Barcode scan box
        private TextBox txtDiscount;
        private Label lblSubTotal;
        private Label lblGrandTotal;
        private Button btnCheckout;
        private DataGridView dgvCart;
        private Button btnHoldSale, btnViewHeld;

        private readonly Color darkColor = Color.FromArgb(43, 43, 54);
        private readonly HeldSaleRepository heldSaleRepo = new HeldSaleRepository();

        public POSForm()
        {
            InitializeComponent();
            BuildUI();
        }

        // ==========================================
        // 1. UI එක සම්පූර්ණයෙන්ම Code එකෙන් හැදීම
        // ==========================================
        private void BuildUI()
        {
            this.BackColor = Color.White;

            // ---------- වම් පැත්ත: Billing Summary Panel (Fixed Width) ----------
            Panel leftPanel = new Panel();
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Width = 380;
            leftPanel.BackColor = Color.FromArgb(240, 242, 245);
            leftPanel.Padding = new Padding(30, 30, 30, 30);
            this.Controls.Add(leftPanel);

            // Sub Total
            Label lblSubTitle = new Label();
            lblSubTitle.Text = "Sub Total:";
            lblSubTitle.Font = new Font("Segoe UI", 11F);
            lblSubTitle.ForeColor = Color.Gray;
            lblSubTitle.AutoSize = true;
            lblSubTitle.Location = new Point(30, 30);
            leftPanel.Controls.Add(lblSubTitle);

            lblSubTotal = new Label();
            lblSubTotal.Text = "0.00";
            lblSubTotal.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblSubTotal.ForeColor = darkColor;
            lblSubTotal.AutoSize = true;
            lblSubTotal.Location = new Point(150, 25);
            leftPanel.Controls.Add(lblSubTotal);

            // Discount
            Label lblDiscTitle = new Label();
            lblDiscTitle.Text = "Discount:";
            lblDiscTitle.Font = new Font("Segoe UI", 11F);
            lblDiscTitle.ForeColor = Color.Gray;
            lblDiscTitle.AutoSize = true;
            lblDiscTitle.Location = new Point(30, 90);
            leftPanel.Controls.Add(lblDiscTitle);

            txtDiscount = new TextBox();
            txtDiscount.Text = "0";
            txtDiscount.Font = new Font("Segoe UI", 12F);
            txtDiscount.BorderStyle = BorderStyle.FixedSingle;
            txtDiscount.Width = 150;
            txtDiscount.Location = new Point(150, 87);
            txtDiscount.TextChanged += txtDiscount_TextChanged;
            leftPanel.Controls.Add(txtDiscount);

            // Grand Total Badge
            Label lblGrandTitle = new Label();
            lblGrandTitle.Text = "GRAND TOTAL";
            lblGrandTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblGrandTitle.ForeColor = Color.White;
            lblGrandTitle.BackColor = Color.FromArgb(231, 76, 60);
            lblGrandTitle.AutoSize = false;
            lblGrandTitle.Size = new Size(210, 34);
            lblGrandTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblGrandTitle.Location = new Point(30, 165);
            leftPanel.Controls.Add(lblGrandTitle);

            lblGrandTotal = new Label();
            lblGrandTotal.Text = "Rs. 0.00";
            lblGrandTotal.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblGrandTotal.ForeColor = Color.FromArgb(39, 174, 96);
            lblGrandTotal.AutoSize = true;
            lblGrandTotal.Location = new Point(30, 215);
            leftPanel.Controls.Add(lblGrandTotal);

            // Pay Now Button
            btnCheckout = new Button();
            btnCheckout.Text = "PAY NOW (F12)";
            btnCheckout.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            btnCheckout.Size = new Size(300, 60);
            btnCheckout.Location = new Point(30, 300);
            btnCheckout.FlatStyle = FlatStyle.Flat;
            btnCheckout.FlatAppearance.BorderSize = 0;
            btnCheckout.BackColor = Color.FromArgb(39, 174, 96);
            btnCheckout.ForeColor = Color.White;
            btnCheckout.Cursor = Cursors.Hand;
            btnCheckout.Click += btnCheckout_Click;
            leftPanel.Controls.Add(btnCheckout);

            btnHoldSale = new Button();
            btnHoldSale.Text = "⏸ Hold Sale";
            btnHoldSale.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnHoldSale.Size = new Size(300, 46);
            btnHoldSale.Location = new Point(30, 375);
            btnHoldSale.FlatStyle = FlatStyle.Flat;
            btnHoldSale.FlatAppearance.BorderSize = 0;
            btnHoldSale.BackColor = Color.FromArgb(243, 156, 18);
            btnHoldSale.ForeColor = Color.White;
            btnHoldSale.Cursor = Cursors.Hand;
            btnHoldSale.Click += btnHoldSale_Click;
            leftPanel.Controls.Add(btnHoldSale);

            btnViewHeld = new Button();
            btnViewHeld.Text = "📋 View Held Sales";
            btnViewHeld.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnViewHeld.Size = new Size(300, 46);
            btnViewHeld.Location = new Point(30, 435);
            btnViewHeld.FlatStyle = FlatStyle.Flat;
            btnViewHeld.FlatAppearance.BorderSize = 0;
            btnViewHeld.BackColor = Color.FromArgb(41, 128, 185);
            btnViewHeld.ForeColor = Color.White;
            btnViewHeld.Cursor = Cursors.Hand;
            btnViewHeld.Click += btnViewHeld_Click;
            leftPanel.Controls.Add(btnViewHeld);

            // ---------- දකුණු පැත්ත: Search + Cart Panel (Dock=Fill) ----------
            Panel rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.BackColor = Color.White;
            rightPanel.Padding = new Padding(30, 20, 30, 20);
            this.Controls.Add(rightPanel);
            rightPanel.BringToFront();

            // Search Label + Barcode Box (Top)
            Panel searchArea = new Panel();
            searchArea.Dock = DockStyle.Top;
            searchArea.Height = 90;
            rightPanel.Controls.Add(searchArea);

            Label lblSearch = new Label();
            lblSearch.Text = "Scan Barcode or Search:";
            lblSearch.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblSearch.ForeColor = darkColor;
            lblSearch.AutoSize = true;
            lblSearch.Location = new Point(0, 0);
            searchArea.Controls.Add(lblSearch);

            textBox1 = new TextBox();
            textBox1.Font = new Font("Segoe UI", 13F);
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.Width = 450;
            textBox1.Location = new Point(0, 35);
            textBox1.KeyDown += textBox1_KeyDown;
            searchArea.Controls.Add(textBox1);

            // Cart Grid (Fill)
            dgvCart = new DataGridView();
            dgvCart.Dock = DockStyle.Fill;
            dgvCart.BackgroundColor = Color.White;
            dgvCart.BorderStyle = BorderStyle.None;
            dgvCart.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCart.RowHeadersVisible = false;
            dgvCart.AllowUserToAddRows = false;
            dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCart.EnableHeadersVisualStyles = false;
            dgvCart.ColumnHeadersHeight = 40;
            dgvCart.ColumnHeadersDefaultCellStyle.BackColor = darkColor;
            dgvCart.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCart.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvCart.DefaultCellStyle.Font = new Font("Segoe UI", 10.5F);
            dgvCart.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 240, 241);
            dgvCart.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvCart.RowTemplate.Height = 34;

            // Columns: ProductID (hidden), ProductName, Price, Qty (editable), Total
            dgvCart.Columns.Add("ProductID", "ProductID");
            dgvCart.Columns["ProductID"].Visible = false;

            dgvCart.Columns.Add("ProductName", "Product Name");
            dgvCart.Columns.Add("Price", "Price");
            dgvCart.Columns.Add("Qty", "Qty");
            dgvCart.Columns.Add("Total", "Total");

            dgvCart.Columns["ProductName"].ReadOnly = true;
            dgvCart.Columns["Price"].ReadOnly = true;
            dgvCart.Columns["Total"].ReadOnly = true;
            // Qty තීරුව විතරක් Edit කරන්න පුළුවන්

            dgvCart.CellValueChanged += dgvCart_CellValueChanged;

            rightPanel.Controls.Add(dgvCart);
            dgvCart.BringToFront();

            // F12 එබුවම Checkout වෙන්න
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.F12) btnCheckout_Click(this, EventArgs.Empty);
            };

            textBox1.Focus();
            AdvancedPOS.Helpers.ThemeManager.ApplyToForm(this,
            AdvancedPOS.Helpers.ThemeManager.MainBackground,
            AdvancedPOS.Helpers.ThemeManager.CardBackground,
            AdvancedPOS.Helpers.ThemeManager.PrimaryTextColor,
            AdvancedPOS.Helpers.ThemeManager.SecondaryTextColor,
            leftPanel, AdvancedPOS.Helpers.ThemeManager.MainBackground);
        }

        // ==========================================
        // 2. Business Logic (ඔයාගේ පරණ code එකම)
        // ==========================================
        private void CalculateTotals()
        {
            decimal subTotal = 0;
            foreach (DataGridViewRow row in dgvCart.Rows)
            {
                if (row.IsNewRow) continue;
                subTotal += Convert.ToDecimal(row.Cells["Total"].Value);
            }

            lblSubTotal.Text = subTotal.ToString("0.00");

            decimal discount = 0;
            decimal.TryParse(txtDiscount.Text, out discount);

            decimal grandTotal = subTotal - discount;
            lblGrandTotal.Text = "Rs. " + grandTotal.ToString("0.00");
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string barcode = textBox1.Text.Trim();
                if (string.IsNullOrEmpty(barcode)) return;

                ProductRepository prodRepo = new ProductRepository();
                Product prod = prodRepo.GetProductByBarcode(barcode);

                if (prod != null)
                {
                    bool found = false;
                    foreach (DataGridViewRow row in dgvCart.Rows)
                    {
                        if (Convert.ToInt32(row.Cells["ProductID"].Value) == prod.ProductID)
                        {
                            int currentQty = Convert.ToInt32(row.Cells["Qty"].Value);
                            int newQty = currentQty + 1;
                            row.Cells["Qty"].Value = newQty;
                            ApplyRowPricing(row, prod, newQty);
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        int newRowIndex = dgvCart.Rows.Add(prod.ProductID, prod.ProductName, prod.SellingPrice, 1, prod.SellingPrice);
                        ApplyRowPricing(dgvCart.Rows[newRowIndex], prod, 1);
                    }

                    CalculateTotals();
                    textBox1.Clear();
                }
                else
                {
                    MessageBox.Show("Product not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.SelectAll();
                }
            }
        }

        // ==========================================
        // Product Discount % සහ Buy X Get Y Free Promotion එක ගණනය කර Total එකට Apply කිරීම
        // ==========================================
        private void ApplyRowPricing(DataGridViewRow row, Product prod, int qty)
        {
            decimal basePrice = prod.SellingPrice;
            decimal lineTotal = basePrice * qty;

            // 1. Product Level Discount % Apply කිරීම
            if (prod.DiscountPercent > 0)
            {
                decimal discountAmount = lineTotal * (prod.DiscountPercent / 100m);
                lineTotal -= discountAmount;
            }

            // 2. Buy X Get Y Free Promotion Apply කිරීම
            PromotionRepository promoRepo = new PromotionRepository();
            Promotion promo = promoRepo.GetActivePromotion(prod.ProductID);

            if (promo != null && promo.BuyQty > 0)
            {
                // කීයක් "Sets" සම්පූර්ණ වෙනවද කියලා ගණනය කිරීම
                // උදා: Buy 2 Get 1 Free, Qty = 6 නම් -> Sets = 6 / (2+1) = 2 Sets -> Free Items = 2
                int setSize = promo.BuyQty + promo.FreeQty;
                int completeSets = qty / setSize;
                int freeItems = completeSets * promo.FreeQty;

                if (freeItems > 0)
                {
                    decimal freeValue = freeItems * basePrice;
                    lineTotal -= freeValue;
                }
            }

            row.Cells["Total"].Value = Math.Round(lineTotal, 2);
        }

        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void dgvCart_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvCart.Columns[e.ColumnIndex].Name == "Qty")
            {
                try
                {
                    int productId = Convert.ToInt32(dgvCart.Rows[e.RowIndex].Cells["ProductID"].Value);
                    int newQty = Convert.ToInt32(dgvCart.Rows[e.RowIndex].Cells["Qty"].Value);

                    ProductRepository prodRepo = new ProductRepository();
                    Product prod = prodRepo.GetProductById(productId);

                    if (prod != null)
                    {
                        ApplyRowPricing(dgvCart.Rows[e.RowIndex], prod, newQty);
                        CalculateTotals();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Please enter a valid number for Quantity.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvCart.Rows[e.RowIndex].Cells["Qty"].Value = 1;
                }
            }
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            List<SaleDetail> detailsList = new List<SaleDetail>();

            foreach (DataGridViewRow row in dgvCart.Rows)
            {
                if (row.IsNewRow) continue;

                var detail = new SaleDetail
                {
                    ProductID = Convert.ToInt32(row.Cells["ProductID"].Value),
                    UnitPrice = Convert.ToDecimal(row.Cells["Price"].Value),
                    Quantity = Convert.ToInt32(row.Cells["Qty"].Value),
                    TotalPrice = Convert.ToDecimal(row.Cells["Total"].Value)
                };
                detailsList.Add(detail);
            }

            if (detailsList.Count == 0)
            {
                MessageBox.Show("Please add items to the cart before proceeding to payment.", "Cart Empty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal subTotal = Convert.ToDecimal(lblSubTotal.Text);
            decimal.TryParse(txtDiscount.Text, out decimal discount);
            decimal totalBill = subTotal - discount;

            using (PaymentDialogForm paymentModal = new PaymentDialogForm(totalBill))
            {
                if (paymentModal.ShowDialog(this) == DialogResult.OK)
                {
                    Sale newSale = new Sale
                    {
                        SubTotal = subTotal,
                        Discount = discount,
                        GrandTotal = totalBill,
                        PaidAmount = paymentModal.PaidAmount,
                        ChangeAmount = paymentModal.ChangeAmount,
                        PaymentStatus = "Paid",
                        UserID = UserSession.CurrentUserID
                    };

                    try
                    {
                        SaleRepository saleRepo = new SaleRepository();
                        if (saleRepo.SaveTransaction(newSale, detailsList, paymentModal.Payments))
                        {
                            MessageBox.Show($"Payment completed successfully!\nChange: Rs. {paymentModal.ChangeAmount:N2}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            AdvancedPOS.Helpers.ReceiptPrinter printer = new AdvancedPOS.Helpers.ReceiptPrinter(newSale, detailsList, paymentModal.Payments);
                            printer.PrintReceipt();

                            dgvCart.Rows.Clear();
                            lblSubTotal.Text = "0.00";
                            txtDiscount.Text = "0.00";
                            lblGrandTotal.Text = "Rs. 0.00";
                            textBox1.Focus();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Transaction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        // ==========================================
        // Hold Sale (Cart එක Database එකට Temporary ලෙස Save කිරීම)
        // ==========================================
        private void btnHoldSale_Click(object sender, EventArgs e)
        {
            if (dgvCart.Rows.Count == 0)
            {
                MessageBox.Show("The cart is empty. Add items before holding the sale.", "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string note = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter a note for this held sale (e.g. Customer name or Table number):",
                "Hold Sale", "");

            decimal.TryParse(txtDiscount.Text, out decimal discount);

            HeldSale heldSale = new HeldSale
            {
                Note = string.IsNullOrWhiteSpace(note) ? "Held Sale" : note.Trim(),
                Discount = discount,
                UserID = AdvancedPOS.Helpers.UserSession.CurrentUserID
            };

            foreach (DataGridViewRow row in dgvCart.Rows)
            {
                if (row.IsNewRow) continue;

                heldSale.Items.Add(new HeldSaleItem
                {
                    ProductID = Convert.ToInt32(row.Cells["ProductID"].Value),
                    ProductName = row.Cells["ProductName"].Value.ToString(),
                    Price = Convert.ToDecimal(row.Cells["Price"].Value),
                    Qty = Convert.ToInt32(row.Cells["Qty"].Value),
                    Total = Convert.ToDecimal(row.Cells["Total"].Value)
                });
            }

            try
            {
                if (heldSaleRepo.HoldSale(heldSale))
                {
                    MessageBox.Show("Sale held successfully! You can resume it anytime from 'View Held Sales'.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Cart එක Clear කර, ඊළඟ පාරිභෝගිකයාට POS එක Free කිරීම
                    dgvCart.Rows.Clear();
                    lblSubTotal.Text = "0.00";
                    txtDiscount.Text = "0.00";
                    lblGrandTotal.Text = "Rs. 0.00";
                    textBox1.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error holding sale: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // View Held Sales (Popup එකෙන් Select කරලා Cart එකට Load කිරීම)
        // ==========================================
        private void btnViewHeld_Click(object sender, EventArgs e)
        {
            if (dgvCart.Rows.Count > 0)
            {
                DialogResult confirm = MessageBox.Show(
                    "The current cart has items. Loading a held sale will clear the current cart. Continue?",
                    "Cart Not Empty", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes) return;
            }

            using (HeldSalesListForm listForm = new HeldSalesListForm())
            {
                if (listForm.ShowDialog(this) == DialogResult.OK && listForm.SelectedHeldSale != null)
                {
                    LoadHeldSaleIntoCart(listForm.SelectedHeldSale);
                }
            }
        }

        private void LoadHeldSaleIntoCart(HeldSale heldSale)
        {
            dgvCart.Rows.Clear();

            foreach (var item in heldSale.Items)
            {
                dgvCart.Rows.Add(item.ProductID, item.ProductName, item.Price, item.Qty, item.Total);
            }

            txtDiscount.Text = heldSale.Discount.ToString("0.00");
            CalculateTotals();

            MessageBox.Show($"Held sale \"{heldSale.Note}\" loaded to cart.", "Loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}