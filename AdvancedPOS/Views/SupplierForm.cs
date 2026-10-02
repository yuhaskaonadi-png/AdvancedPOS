using System;
using System.Drawing;
using System.Windows.Forms;
using AdvancedPOS.Models;
using AdvancedPOS.Repositories;

namespace AdvancedPOS.Views
{
    public partial class SupplierForm : Form
    {
        // ==========================================
        // UI Controls (Code එකෙන්ම හදනවා)
        // ==========================================
        private TextBox txtName, txtContactPerson, txtPhone, txtAddress;
        private Button btnAdd, btnUpdate, btnDelete, btnClear;
        private DataGridView dgvSuppliers;

        private readonly SupplierRepository supplierRepo = new SupplierRepository();
        private int selectedSupplierId = 0;
        private readonly Color darkColor = Color.FromArgb(43, 43, 54);

        public SupplierForm()
        {
            InitializeComponent();
            BuildUI();
            LoadSuppliers();
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
            lblHeading.Text = "Manage Suppliers";
            lblHeading.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(30, 20);
            leftPanel.Controls.Add(lblHeading);

            int y = 80;

            txtName = AddField(leftPanel, "Supplier Name", ref y);
            txtContactPerson = AddField(leftPanel, "Contact Person", ref y);
            txtPhone = AddField(leftPanel, "Phone", ref y);
            txtAddress = AddField(leftPanel, "Address", ref y);

            y += 15;

            // ---------- Buttons (Row 1: Add / Update) ----------
            btnAdd = new Button();
            btnAdd.Text = "Add";
            btnAdd.Size = new Size(165, 42);
            btnAdd.Location = new Point(30, y);
            StyleButton(btnAdd, Color.FromArgb(39, 174, 96));
            btnAdd.Click += btnAdd_Click;
            leftPanel.Controls.Add(btnAdd);

            btnUpdate = new Button();
            btnUpdate.Text = "Update";
            btnUpdate.Size = new Size(165, 42);
            btnUpdate.Location = new Point(215, y);
            StyleButton(btnUpdate, Color.FromArgb(243, 156, 18));
            btnUpdate.Click += btnUpdate_Click;
            leftPanel.Controls.Add(btnUpdate);

            y += 55;

            // ---------- Buttons (Row 2: Delete / Clear) ----------
            btnDelete = new Button();
            btnDelete.Text = "Delete";
            btnDelete.Size = new Size(165, 42);
            btnDelete.Location = new Point(30, y);
            StyleButton(btnDelete, Color.FromArgb(231, 76, 60));
            btnDelete.Click += btnDelete_Click;
            leftPanel.Controls.Add(btnDelete);

            btnClear = new Button();
            btnClear.Text = "Clear";
            btnClear.Size = new Size(165, 42);
            btnClear.Location = new Point(215, y);
            StyleButton(btnClear, Color.FromArgb(149, 165, 166));
            btnClear.Click += btnClear_Click;
            leftPanel.Controls.Add(btnClear);

            // ---------- දකුණු පැත්ත: Grid Container (Dock=Fill) ----------
            Panel rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.BackColor = Color.FromArgb(240, 242, 245);
            rightPanel.Padding = new Padding(20);
            this.Controls.Add(rightPanel);
            rightPanel.BringToFront();

            dgvSuppliers = new DataGridView();
            dgvSuppliers.Dock = DockStyle.Fill;
            dgvSuppliers.BackgroundColor = Color.White;
            dgvSuppliers.BorderStyle = BorderStyle.None;
            dgvSuppliers.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvSuppliers.RowHeadersVisible = false;
            dgvSuppliers.AllowUserToAddRows = false;
            dgvSuppliers.ReadOnly = true;
            dgvSuppliers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSuppliers.MultiSelect = false;
            dgvSuppliers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSuppliers.EnableHeadersVisualStyles = false;
            dgvSuppliers.ColumnHeadersHeight = 40;
            dgvSuppliers.ColumnHeadersDefaultCellStyle.BackColor = darkColor;
            dgvSuppliers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSuppliers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvSuppliers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvSuppliers.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvSuppliers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 240, 241);
            dgvSuppliers.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvSuppliers.RowTemplate.Height = 32;
            dgvSuppliers.CellClick += dgvSuppliers_CellClick;

            rightPanel.Controls.Add(dgvSuppliers);
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
        // 2. Data Loading Logic (ඔයාගේ පරණ code එකම)
        // ==========================================
        private void LoadSuppliers()
        {
            dgvSuppliers.DataSource = supplierRepo.GetAllSuppliers();
        }

        // ==========================================
        // 3. Add / Update / Delete / Clear Logic (ඔයාගේ පරණ code එකම)
        // ==========================================
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Supplier Name and Phone are required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Supplier supplier = new Supplier
            {
                SupplierName = txtName.Text.Trim(),
                ContactPerson = txtContactPerson.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Address = txtAddress.Text.Trim()
            };

            if (supplierRepo.AddSupplier(supplier))
            {
                MessageBox.Show("Supplier added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadSuppliers();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedSupplierId == 0)
            {
                MessageBox.Show("Please select a supplier from the table to update!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Supplier supplier = new Supplier
            {
                SupplierID = selectedSupplierId,
                SupplierName = txtName.Text.Trim(),
                ContactPerson = txtContactPerson.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Address = txtAddress.Text.Trim()
            };

            if (supplierRepo.UpdateSupplier(supplier))
            {
                MessageBox.Show("Supplier updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadSuppliers();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedSupplierId == 0)
            {
                MessageBox.Show("Please select a supplier to delete!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this supplier?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                if (supplierRepo.DeleteSupplier(selectedSupplierId))
                {
                    MessageBox.Show("Supplier deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    LoadSuppliers();
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtContactPerson.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            selectedSupplierId = 0;
        }

        private void dgvSuppliers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvSuppliers.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvSuppliers.SelectedRows[0];
                selectedSupplierId = Convert.ToInt32(row.Cells["SupplierID"].Value);
                txtName.Text = row.Cells["SupplierName"].Value?.ToString();
                txtContactPerson.Text = row.Cells["ContactPerson"].Value?.ToString();
                txtPhone.Text = row.Cells["Phone"].Value?.ToString();
                txtAddress.Text = row.Cells["Address"].Value?.ToString();
            }
        }
    }
}