using System;
using System.Drawing;
using System.Windows.Forms;
using AdvancedPOS.Models;
using AdvancedPOS.Repositories;

namespace AdvancedPOS.Views
{
    public partial class ReturnItemForm : Form
    {
        // ==========================================
        // UI Controls (Code එකෙන්ම හදනවා)
        // ==========================================
        private Label lblProductName, lblUnitPrice, lblMaxQty, lblRefundAmount;
        private NumericUpDown nudReturnQty;
        private TextBox txtReason;
        private Button btnConfirmReturn, btnCancelReturn;

        private readonly int _saleId;
        private readonly int _productId;
        private readonly decimal _unitPrice;
        private readonly int _maxQty;
        private readonly SaleRepository _saleRepo = new SaleRepository();
        private readonly Color darkColor = Color.FromArgb(43, 43, 54);

        public ReturnItemForm(int saleId, int productId, string productName, decimal unitPrice, int maxQty)
        {
            InitializeComponent();

            _saleId = saleId;
            _productId = productId;
            _unitPrice = unitPrice;
            _maxQty = maxQty;

            BuildUI(productName);

            nudReturnQty.Maximum = maxQty;
            nudReturnQty.Minimum = 1;
            nudReturnQty.Value = 1;

            nudReturnQty.ValueChanged += NudReturnQty_ValueChanged;
            CalculateRefund();
        }

        // ==========================================
        // 1. UI එක සම්පූර්ණයෙන්ම Code එකෙන් හැදීම
        // ==========================================
        private void BuildUI(string productName)
        {
            this.Text = "Return Item";
            this.Size = new Size(420, 460);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            Panel container = new Panel();
            container.Dock = DockStyle.Fill;
            container.Padding = new Padding(30, 25, 30, 25);
            this.Controls.Add(container);

            Label lblHeading = new Label();
            lblHeading.Text = "Process Return";
            lblHeading.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(0, 0);
            container.Controls.Add(lblHeading);

            // ---- Item Info Card ----
            Panel infoCard = new Panel();
            infoCard.BackColor = Color.FromArgb(240, 242, 245);
            infoCard.Size = new Size(360, 100);
            infoCard.Location = new Point(0, 45);
            infoCard.Padding = new Padding(15);
            container.Controls.Add(infoCard);

            lblProductName = new Label();
            lblProductName.Text = "Item: " + productName;
            lblProductName.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblProductName.ForeColor = darkColor;
            lblProductName.AutoSize = true;
            lblProductName.Location = new Point(15, 12);
            infoCard.Controls.Add(lblProductName);

            lblUnitPrice = new Label();
            lblUnitPrice.Font = new Font("Segoe UI", 9.5F);
            lblUnitPrice.ForeColor = Color.Gray;
            lblUnitPrice.AutoSize = true;
            lblUnitPrice.Location = new Point(15, 42);
            infoCard.Controls.Add(lblUnitPrice);

            lblMaxQty = new Label();
            lblMaxQty.Font = new Font("Segoe UI", 9.5F);
            lblMaxQty.ForeColor = Color.Gray;
            lblMaxQty.AutoSize = true;
            lblMaxQty.Location = new Point(15, 65);
            infoCard.Controls.Add(lblMaxQty);

            int y = 165;

            // Return Quantity
            Label lblQtyTitle = new Label();
            lblQtyTitle.Text = "Return Quantity";
            lblQtyTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblQtyTitle.ForeColor = Color.Gray;
            lblQtyTitle.AutoSize = true;
            lblQtyTitle.Location = new Point(0, y);
            container.Controls.Add(lblQtyTitle);

            nudReturnQty = new NumericUpDown();
            nudReturnQty.Font = new Font("Segoe UI", 12F);
            nudReturnQty.Width = 360;
            nudReturnQty.Height = 32;
            nudReturnQty.Location = new Point(0, y + 20);
            nudReturnQty.TextAlign = HorizontalAlignment.Center;
            container.Controls.Add(nudReturnQty);

            y += 68;

            // Reason
            Label lblReasonTitle = new Label();
            lblReasonTitle.Text = "Reason (Optional)";
            lblReasonTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblReasonTitle.ForeColor = Color.Gray;
            lblReasonTitle.AutoSize = true;
            lblReasonTitle.Location = new Point(0, y);
            container.Controls.Add(lblReasonTitle);

            txtReason = new TextBox();
            txtReason.Font = new Font("Segoe UI", 10.5F);
            txtReason.BorderStyle = BorderStyle.FixedSingle;
            txtReason.Width = 360;
            txtReason.Height = 60;
            txtReason.Multiline = true;
            txtReason.Location = new Point(0, y + 20);
            container.Controls.Add(txtReason);

            y += 95;

            // Refund Amount
            lblRefundAmount = new Label();
            lblRefundAmount.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblRefundAmount.ForeColor = Color.FromArgb(231, 76, 60);
            lblRefundAmount.AutoSize = true;
            lblRefundAmount.Location = new Point(0, y);
            container.Controls.Add(lblRefundAmount);

            y += 45;

            btnConfirmReturn = new Button();
            btnConfirmReturn.Text = "Confirm Return";
            btnConfirmReturn.Size = new Size(360, 44);
            btnConfirmReturn.Location = new Point(0, y);
            StyleButton(btnConfirmReturn, Color.FromArgb(243, 156, 18));
            btnConfirmReturn.Click += btnConfirmReturn_Click;
            container.Controls.Add(btnConfirmReturn);

            btnCancelReturn = new Button();
            btnCancelReturn.Text = "Cancel";
            btnCancelReturn.Size = new Size(360, 38);
            btnCancelReturn.Location = new Point(0, y + 52);
            StyleButton(btnCancelReturn, Color.FromArgb(149, 165, 166));
            btnCancelReturn.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            container.Controls.Add(btnCancelReturn);

            // Info labels updated now that unit price/max qty known
            lblUnitPrice.Text = "Unit Price: Rs. " + _unitPrice.ToString("0.00");
            lblMaxQty.Text = "Available to Return: " + _maxQty;

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
            btn.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        // ==========================================
        // 2. Business Logic (ඔයාගේ පරණ code එකම)
        // ==========================================
        private void NudReturnQty_ValueChanged(object sender, EventArgs e)
        {
            CalculateRefund();
        }

        private void CalculateRefund()
        {
            decimal refund = nudReturnQty.Value * _unitPrice;
            lblRefundAmount.Text = "Refund Amount: Rs. " + refund.ToString("0.00");
        }

        private void btnConfirmReturn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to process this return and update stock?", "Confirm Return", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SaleReturn returnData = new SaleReturn
                {
                    SaleID = _saleId,
                    ProductID = _productId,
                    Quantity = (int)nudReturnQty.Value,
                    RefundAmount = nudReturnQty.Value * _unitPrice,
                    Reason = txtReason.Text.Trim()
                };

                try
                {
                    if (_saleRepo.ProcessReturn(returnData))
                    {
                        MessageBox.Show("Return processed successfully. Stock has been updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Return Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}