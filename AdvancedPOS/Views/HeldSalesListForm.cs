using System;
using System.Drawing;
using System.Windows.Forms;
using AdvancedPOS.Models;
using AdvancedPOS.Repositories;

namespace AdvancedPOS.Views
{
    public partial class HeldSalesListForm : Form
    {
        // ==========================================
        // UI Controls
        // ==========================================
        private DataGridView dgvHeldSales;
        private Button btnLoad, btnDelete, btnClose;

        private readonly HeldSaleRepository heldSaleRepo = new HeldSaleRepository();
        private readonly Color darkColor = Color.FromArgb(43, 43, 54);

        // Selected Held Sale එකෙන් POSForm එකට දෙන Result එක
        public HeldSale SelectedHeldSale { get; private set; }

        public HeldSalesListForm()
        {
            InitializeComponent();
            BuildUI();
            LoadHeldSales();
        }

        // ==========================================
        // 1. UI එක සම්පූර්ණයෙන්ම Code එකෙන් හැදීම
        // ==========================================
        private void BuildUI()
        {
            this.Text = "Held / Parked Sales";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            Panel container = new Panel();
            container.Dock = DockStyle.Fill;
            container.Padding = new Padding(25, 20, 25, 20);
            this.Controls.Add(container);

            Label lblHeading = new Label();
            lblHeading.Text = "Held Sales";
            lblHeading.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(0, 0);
            container.Controls.Add(lblHeading);

            // Grid
            dgvHeldSales = new DataGridView();
            dgvHeldSales.Location = new Point(0, 45);
            dgvHeldSales.Size = new Size(650, 340);
            dgvHeldSales.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvHeldSales.BackgroundColor = Color.White;
            dgvHeldSales.BorderStyle = BorderStyle.None;
            dgvHeldSales.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvHeldSales.RowHeadersVisible = false;
            dgvHeldSales.AllowUserToAddRows = false;
            dgvHeldSales.ReadOnly = true;
            dgvHeldSales.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHeldSales.MultiSelect = false;
            dgvHeldSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHeldSales.EnableHeadersVisualStyles = false;
            dgvHeldSales.ColumnHeadersHeight = 38;
            dgvHeldSales.ColumnHeadersDefaultCellStyle.BackColor = darkColor;
            dgvHeldSales.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHeldSales.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvHeldSales.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvHeldSales.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 240, 241);
            dgvHeldSales.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvHeldSales.RowTemplate.Height = 32;
            dgvHeldSales.CellDoubleClick += (s, e) => btnLoad_Click(s, e);
            container.Controls.Add(dgvHeldSales);

            int btnY = 400;

            btnLoad = new Button();
            btnLoad.Text = "Load to Cart";
            btnLoad.Size = new Size(200, 44);
            btnLoad.Location = new Point(0, btnY);
            btnLoad.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            StyleButton(btnLoad, Color.FromArgb(39, 174, 96));
            btnLoad.Click += btnLoad_Click;
            container.Controls.Add(btnLoad);

            btnDelete = new Button();
            btnDelete.Text = "Delete";
            btnDelete.Size = new Size(150, 44);
            btnDelete.Location = new Point(215, btnY);
            btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            StyleButton(btnDelete, Color.FromArgb(231, 76, 60));
            btnDelete.Click += btnDelete_Click;
            container.Controls.Add(btnDelete);

            btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Size = new Size(150, 44);
            btnClose.Location = new Point(500, btnY);
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            StyleButton(btnClose, Color.FromArgb(149, 165, 166));
            btnClose.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            container.Controls.Add(btnClose);
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
        private void LoadHeldSales()
        {
            dgvHeldSales.DataSource = heldSaleRepo.GetAllHeldSales();
        }

        // ==========================================
        // 3. Load / Delete Logic
        // ==========================================
        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (dgvHeldSales.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a held sale to load.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int heldSaleId = Convert.ToInt32(dgvHeldSales.SelectedRows[0].Cells["ID"].Value);
            SelectedHeldSale = heldSaleRepo.GetHeldSaleById(heldSaleId);

            if (SelectedHeldSale != null)
            {
                // Load කරාට පස්සේ Database එකෙන් මකනවා (එකම Sale එක දෙපාරක් Load වෙන්නේ නැතුව)
                heldSaleRepo.DeleteHeldSale(heldSaleId);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvHeldSales.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a held sale to delete.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Are you sure you want to cancel and delete this held sale?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int heldSaleId = Convert.ToInt32(dgvHeldSales.SelectedRows[0].Cells["ID"].Value);
                heldSaleRepo.DeleteHeldSale(heldSaleId);
                LoadHeldSales();
            }
        }
    }
}