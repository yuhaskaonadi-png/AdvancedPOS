using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using AdvancedPOS.Helpers;

namespace AdvancedPOS.Views
{
    public partial class BarcodePrintForm : Form
    {
        // ==========================================
        // UI Controls
        // ==========================================
        private PictureBox picPreview;
        private Label lblProductName, lblPrice;
        private NumericUpDown nudCopies;
        private Button btnPrint, btnCancel;

        private readonly string _barcodeValue;
        private readonly string _productName;
        private readonly string _priceText;
        private Bitmap _barcodeBitmap;

        private readonly PrintDocument printDoc = new PrintDocument();
        private int currentCopy = 0;
        private int totalCopies = 1;
        private readonly Color darkColor = Color.FromArgb(43, 43, 54);

        public BarcodePrintForm(string barcodeValue, string productName, string priceText)
        {
            InitializeComponent();

            _barcodeValue = string.IsNullOrWhiteSpace(barcodeValue) ? "N/A" : barcodeValue;
            _productName = productName;
            _priceText = priceText;

            BuildUI();
            GeneratePreview();

            printDoc.PrintPage += PrintDoc_PrintPage;
        }

        // ==========================================
        // 1. UI එක සම්පූර්ණයෙන්ම Code එකෙන් හැදීම
        // ==========================================
        private void BuildUI()
        {
            this.Text = "Print Barcode Label";
            this.Size = new Size(420, 480);
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
            lblHeading.Text = "Print Barcode";
            lblHeading.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(0, 0);
            container.Controls.Add(lblHeading);

            // ---- Preview Card ----
            Panel previewCard = new Panel();
            previewCard.BackColor = Color.FromArgb(240, 242, 245);
            previewCard.Size = new Size(360, 200);
            previewCard.Location = new Point(0, 45);
            container.Controls.Add(previewCard);

            picPreview = new PictureBox();
            picPreview.Size = new Size(300, 100);
            picPreview.Location = new Point(30, 15);
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picPreview.BackColor = Color.White;
            previewCard.Controls.Add(picPreview);

            lblProductName = new Label();
            lblProductName.Text = _productName;
            lblProductName.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblProductName.ForeColor = darkColor;
            lblProductName.AutoSize = true;
            lblProductName.Location = new Point(30, 125);
            previewCard.Controls.Add(lblProductName);

            lblPrice = new Label();
            lblPrice.Text = "Rs. " + _priceText;
            lblPrice.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblPrice.ForeColor = Color.FromArgb(39, 174, 96);
            lblPrice.AutoSize = true;
            lblPrice.Location = new Point(30, 155);
            previewCard.Controls.Add(lblPrice);

            int y = 265;

            // Copies
            Label lblCopiesTitle = new Label();
            lblCopiesTitle.Text = "Number of Labels to Print";
            lblCopiesTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblCopiesTitle.ForeColor = Color.Gray;
            lblCopiesTitle.AutoSize = true;
            lblCopiesTitle.Location = new Point(0, y);
            container.Controls.Add(lblCopiesTitle);

            nudCopies = new NumericUpDown();
            nudCopies.Font = new Font("Segoe UI", 12F);
            nudCopies.Width = 360;
            nudCopies.Height = 32;
            nudCopies.Minimum = 1;
            nudCopies.Maximum = 500;
            nudCopies.Value = 1;
            nudCopies.TextAlign = HorizontalAlignment.Center;
            nudCopies.Location = new Point(0, y + 20);
            container.Controls.Add(nudCopies);

            y += 75;

            btnPrint = new Button();
            btnPrint.Text = "🖨 Print Labels";
            btnPrint.Size = new Size(360, 46);
            btnPrint.Location = new Point(0, y);
            StyleButton(btnPrint, Color.FromArgb(39, 174, 96));
            btnPrint.Click += btnPrint_Click;
            container.Controls.Add(btnPrint);

            btnCancel = new Button();
            btnCancel.Text = "Close";
            btnCancel.Size = new Size(360, 40);
            btnCancel.Location = new Point(0, y + 55);
            StyleButton(btnCancel, Color.FromArgb(149, 165, 166));
            btnCancel.Click += (s, e) => this.Close();
            container.Controls.Add(btnCancel);
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
        // 2. Barcode Generate කිරීම
        // ==========================================
        private void GeneratePreview()
        {
            try
            {
                _barcodeBitmap = BarcodeHelper.GenerateBarcode(_barcodeValue, 300, 100);
                picPreview.Image = _barcodeBitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not generate barcode: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // 3. Print Logic
        // ==========================================
        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (_barcodeBitmap == null)
            {
                MessageBox.Show("No barcode to print.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            totalCopies = (int)nudCopies.Value;
            currentCopy = 0;

            using (PrintDialog pd = new PrintDialog())
            {
                pd.Document = printDoc;
                if (pd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        printDoc.Print();
                        MessageBox.Show("Labels sent to printer!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Print error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Labels ටික Page එකේ Grid එකක් විදිහට තියලා print කිරීම
        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            int labelWidth = 220;
            int labelHeight = 110;

            int cols = Math.Max(1, e.MarginBounds.Width / labelWidth);
            int rows = Math.Max(1, e.MarginBounds.Height / labelHeight);

            int col = 0, row = 0;

            Font nameFont = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            Font priceFont = new Font("Segoe UI", 10F, FontStyle.Bold);

            while (currentCopy < totalCopies && row < rows)
            {
                int x = e.MarginBounds.Left + (col * labelWidth);
                int y = e.MarginBounds.Top + (row * labelHeight);

                e.Graphics.DrawRectangle(Pens.LightGray, x, y, labelWidth - 10, labelHeight - 10);
                e.Graphics.DrawString(_productName, nameFont, Brushes.Black, x + 8, y + 5);
                e.Graphics.DrawImage(_barcodeBitmap, x + 8, y + 25, labelWidth - 26, 50);
                e.Graphics.DrawString("Rs. " + _priceText, priceFont, Brushes.Black, x + 8, y + 80);

                currentCopy++;
                col++;
                if (col >= cols)
                {
                    col = 0;
                    row++;
                }
            }

            e.HasMorePages = currentCopy < totalCopies;
        }
    }
}