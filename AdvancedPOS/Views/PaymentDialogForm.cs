using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AdvancedPOS.Models;

namespace AdvancedPOS.Views
{
    public partial class PaymentDialogForm : Form
    {
        // ==========================================
        // UI Controls (Code එකෙන්ම හදනවා)
        // ==========================================
        private Label lblTotalBill;
        private ComboBox cmbPaymentMethod;

        private Panel pnlCash;
        private TextBox txtCash;

        private Panel pnlCard;
        private TextBox txtCardAmount, txtCardRef;

        private Panel pnlQR;
        private TextBox txtQRAmount, txtQRRef;

        private Label lblBalanceStatus;
        private Button btnConfirmPayment, btnCancelPayment;

        public decimal TotalBillAmount { get; private set; }
        public decimal PaidAmount { get; private set; }
        public decimal ChangeAmount { get; private set; }
        public List<SalePayment> Payments { get; private set; } = new List<SalePayment>();

        private readonly Color darkColor = Color.FromArgb(43, 43, 54);

        public PaymentDialogForm(decimal billAmount)
        {
            InitializeComponent();
            TotalBillAmount = billAmount;

            BuildUI();

            lblTotalBill.Text = $"Total Bill: Rs. {TotalBillAmount:N2}";
            cmbPaymentMethod.SelectedIndex = 0;

            AttachEventHandlers();
            UpdateInputVisibility();
            AutoFillSelectedMethod();
        }

        // ==========================================
        // 1. UI එක සම්පූර්ණයෙන්ම Code එකෙන් හැදීම
        // ==========================================
        private void BuildUI()
        {
            this.Text = "Process Payment";
            this.Size = new Size(450, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            Panel container = new Panel();
            container.Dock = DockStyle.Fill;
            container.Padding = new Padding(30, 25, 30, 25);
            this.Controls.Add(container);

            // Header
            Label lblHeading = new Label();
            lblHeading.Text = "Payment";
            lblHeading.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(0, 0);
            container.Controls.Add(lblHeading);

            lblTotalBill = new Label();
            lblTotalBill.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTotalBill.ForeColor = Color.FromArgb(41, 128, 185);
            lblTotalBill.AutoSize = true;
            lblTotalBill.Location = new Point(0, 40);
            container.Controls.Add(lblTotalBill);

            int y = 85;

            // Payment Method
            Label lblMethod = new Label();
            lblMethod.Text = "Payment Method";
            lblMethod.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblMethod.ForeColor = Color.Gray;
            lblMethod.AutoSize = true;
            lblMethod.Location = new Point(0, y);
            container.Controls.Add(lblMethod);

            cmbPaymentMethod = new ComboBox();
            cmbPaymentMethod.Font = new Font("Segoe UI", 11F);
            cmbPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPaymentMethod.Width = 370;
            cmbPaymentMethod.Location = new Point(0, y + 20);
            cmbPaymentMethod.Items.AddRange(new object[] { "Cash", "Card", "QR", "Split Payment" });
            container.Controls.Add(cmbPaymentMethod);

            y += 65;

            // ---- Cash Panel ----
            pnlCash = new Panel();
            pnlCash.Location = new Point(0, y);
            pnlCash.Size = new Size(370, 60);

            Label lblCashTitle = new Label();
            lblCashTitle.Text = "Cash Amount";
            lblCashTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblCashTitle.ForeColor = Color.Gray;
            lblCashTitle.AutoSize = true;
            lblCashTitle.Location = new Point(0, 0);
            pnlCash.Controls.Add(lblCashTitle);

            txtCash = new TextBox();
            txtCash.Font = new Font("Segoe UI", 11F);
            txtCash.BorderStyle = BorderStyle.FixedSingle;
            txtCash.Width = 370;
            txtCash.Location = new Point(0, 20);
            pnlCash.Controls.Add(txtCash);

            container.Controls.Add(pnlCash);

            // ---- Card Panel ----
            pnlCard = new Panel();
            pnlCard.Location = new Point(0, y);
            pnlCard.Size = new Size(370, 100);

            Label lblCardAmountTitle = new Label();
            lblCardAmountTitle.Text = "Card Amount";
            lblCardAmountTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblCardAmountTitle.ForeColor = Color.Gray;
            lblCardAmountTitle.AutoSize = true;
            lblCardAmountTitle.Location = new Point(0, 0);
            pnlCard.Controls.Add(lblCardAmountTitle);

            txtCardAmount = new TextBox();
            txtCardAmount.Font = new Font("Segoe UI", 11F);
            txtCardAmount.BorderStyle = BorderStyle.FixedSingle;
            txtCardAmount.Width = 370;
            txtCardAmount.Location = new Point(0, 20);
            pnlCard.Controls.Add(txtCardAmount);

            Label lblCardRefTitle = new Label();
            lblCardRefTitle.Text = "Reference Number";
            lblCardRefTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblCardRefTitle.ForeColor = Color.Gray;
            lblCardRefTitle.AutoSize = true;
            lblCardRefTitle.Location = new Point(0, 55);
            pnlCard.Controls.Add(lblCardRefTitle);

            txtCardRef = new TextBox();
            txtCardRef.Font = new Font("Segoe UI", 11F);
            txtCardRef.BorderStyle = BorderStyle.FixedSingle;
            txtCardRef.Width = 370;
            txtCardRef.Location = new Point(0, 75);
            pnlCard.Controls.Add(txtCardRef);

            container.Controls.Add(pnlCard);

            // ---- QR Panel ----
            pnlQR = new Panel();
            pnlQR.Location = new Point(0, y);
            pnlQR.Size = new Size(370, 100);

            Label lblQRAmountTitle = new Label();
            lblQRAmountTitle.Text = "QR Amount";
            lblQRAmountTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblQRAmountTitle.ForeColor = Color.Gray;
            lblQRAmountTitle.AutoSize = true;
            lblQRAmountTitle.Location = new Point(0, 0);
            pnlQR.Controls.Add(lblQRAmountTitle);

            txtQRAmount = new TextBox();
            txtQRAmount.Font = new Font("Segoe UI", 11F);
            txtQRAmount.BorderStyle = BorderStyle.FixedSingle;
            txtQRAmount.Width = 370;
            txtQRAmount.Location = new Point(0, 20);
            pnlQR.Controls.Add(txtQRAmount);

            Label lblQRRefTitle = new Label();
            lblQRRefTitle.Text = "Reference Number";
            lblQRRefTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblQRRefTitle.ForeColor = Color.Gray;
            lblQRRefTitle.AutoSize = true;
            lblQRRefTitle.Location = new Point(0, 55);
            pnlQR.Controls.Add(lblQRRefTitle);

            txtQRRef = new TextBox();
            txtQRRef.Font = new Font("Segoe UI", 11F);
            txtQRRef.BorderStyle = BorderStyle.FixedSingle;
            txtQRRef.Width = 370;
            txtQRRef.Location = new Point(0, 75);
            pnlQR.Controls.Add(txtQRRef);

            container.Controls.Add(pnlQR);

            // Balance Status (fixed lower position so panels don't overlap it)
            int statusY = y + 220;

            lblBalanceStatus = new Label();
            lblBalanceStatus.Text = "Change: Rs. 0.00";
            lblBalanceStatus.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblBalanceStatus.ForeColor = Color.ForestGreen;
            lblBalanceStatus.AutoSize = true;
            lblBalanceStatus.Location = new Point(0, statusY);
            container.Controls.Add(lblBalanceStatus);

            int btnY = statusY + 50;

            btnConfirmPayment = new Button();
            btnConfirmPayment.Text = "Confirm Payment";
            btnConfirmPayment.Size = new Size(370, 46);
            btnConfirmPayment.Location = new Point(0, btnY);
            StyleButton(btnConfirmPayment, Color.FromArgb(39, 174, 96));
            container.Controls.Add(btnConfirmPayment);

            btnCancelPayment = new Button();
            btnCancelPayment.Text = "Cancel";
            btnCancelPayment.Size = new Size(370, 40);
            btnCancelPayment.Location = new Point(0, btnY + 55);
            StyleButton(btnCancelPayment, Color.FromArgb(149, 165, 166));
            container.Controls.Add(btnCancelPayment);

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
        private void AttachEventHandlers()
        {
            cmbPaymentMethod.SelectedIndexChanged += (s, e) =>
            {
                UpdateInputVisibility();
                AutoFillSelectedMethod();
                CalculateBalance();
            };

            txtCash.TextChanged += (s, e) => CalculateBalance();
            txtCardAmount.TextChanged += (s, e) => CalculateBalance();
            txtQRAmount.TextChanged += (s, e) => CalculateBalance();

            btnConfirmPayment.Click += BtnConfirmPayment_Click;
            btnCancelPayment.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
        }

        private void UpdateInputVisibility()
        {
            string selected = cmbPaymentMethod.SelectedItem?.ToString();

            pnlCash.Visible = (selected == "Cash" || selected == "Split Payment");
            pnlCard.Visible = (selected == "Card" || selected == "Split Payment");
            pnlQR.Visible = (selected == "QR" || selected == "Split Payment");
        }

        private void AutoFillSelectedMethod()
        {
            string selected = cmbPaymentMethod.SelectedItem?.ToString();

            txtCash.Clear();
            txtCardAmount.Clear();
            txtQRAmount.Clear();
            txtCardRef.Clear();
            txtQRRef.Clear();

            if (selected == "Cash") txtCash.Text = TotalBillAmount.ToString("0.00");
            else if (selected == "Card") txtCardAmount.Text = TotalBillAmount.ToString("0.00");
            else if (selected == "QR") txtQRAmount.Text = TotalBillAmount.ToString("0.00");
        }

        private void CalculateBalance()
        {
            decimal.TryParse(txtCash.Text.Trim(), out decimal cash);
            decimal.TryParse(txtCardAmount.Text.Trim(), out decimal card);
            decimal.TryParse(txtQRAmount.Text.Trim(), out decimal qr);

            PaidAmount = cash + card + qr;
            ChangeAmount = PaidAmount - TotalBillAmount;

            if (ChangeAmount >= 0)
            {
                lblBalanceStatus.ForeColor = Color.ForestGreen;
                lblBalanceStatus.Text = $"Change: Rs. {ChangeAmount:N2}";
            }
            else
            {
                lblBalanceStatus.ForeColor = Color.Crimson;
                lblBalanceStatus.Text = $"Due: Rs. {Math.Abs(ChangeAmount):N2}";
            }
        }

        private void BtnConfirmPayment_Click(object sender, EventArgs e)
        {
            CalculateBalance();

            if (PaidAmount < TotalBillAmount)
            {
                MessageBox.Show("The entered payment amount is less than the total bill!", "Insufficient Payment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal.TryParse(txtCash.Text.Trim(), out decimal cash);
            decimal.TryParse(txtCardAmount.Text.Trim(), out decimal card);
            decimal.TryParse(txtQRAmount.Text.Trim(), out decimal qr);

            Payments.Clear();

            if (cash > 0)
            {
                decimal netCash = (ChangeAmount > 0) ? (cash - ChangeAmount) : cash;
                Payments.Add(new SalePayment
                {
                    PaymentMethod = "Cash",
                    Amount = netCash,
                    ReferenceNumber = "CASH"
                });
            }

            if (card > 0)
            {
                Payments.Add(new SalePayment
                {
                    PaymentMethod = "Card",
                    Amount = card,
                    ReferenceNumber = txtCardRef.Text.Trim()
                });
            }

            if (qr > 0)
            {
                Payments.Add(new SalePayment
                {
                    PaymentMethod = "QR",
                    Amount = qr,
                    ReferenceNumber = txtQRRef.Text.Trim()
                });
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}