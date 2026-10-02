using System;
using System.Drawing;
using System.Windows.Forms;
using AdvancedPOS.Models;
using AdvancedPOS.Repositories;

namespace AdvancedPOS.Views
{
    public partial class CustomerForm : Form
    {
        // ==========================================
        // UI Controls (Code එකෙන්ම හදනවා)
        // ==========================================
        private TextBox txtName, txtPhone, txtEmail, txtAddress, txtSearch;
        private Button btnSave, btnUpdate, btnDelete, btnClear, btnSearch;
        private Label lblLoyaltyPoints;
        private DataGridView dgvCustomers;

        private readonly CustomerRepository customerRepo = new CustomerRepository();
        private int selectedCustomerId = 0;
        private readonly Color darkColor = Color.FromArgb(43, 43, 54);

        public CustomerForm()
        {
            InitializeComponent();
            BuildUI();
            LoadCustomers();
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
            lblHeading.Text = "Manage Customers";
            lblHeading.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(30, 20);
            leftPanel.Controls.Add(lblHeading);

            int y = 80;

            txtName = AddField(leftPanel, "Customer Name", ref y);
            txtPhone = AddField(leftPanel, "Phone", ref y);
            txtEmail = AddField(leftPanel, "Email", ref y);
            txtAddress = AddField(leftPanel, "Address", ref y);

            y += 5;

            // Loyalty Points display (read-only, shown when editing)
            lblLoyaltyPoints = new Label();
            lblLoyaltyPoints.Text = "";
            lblLoyaltyPoints.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblLoyaltyPoints.ForeColor = Color.FromArgb(41, 128, 185);
            lblLoyaltyPoints.AutoSize = true;
            lblLoyaltyPoints.Location = new Point(30, y);
            leftPanel.Controls.Add(lblLoyaltyPoints);

            y += 40;

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

            btnClear = new Button();
            btnClear.Text = "Clear Fields";
            btnClear.Size = new Size(350, 40);
            btnClear.Location = new Point(30, y);
            StyleButton(btnClear, Color.FromArgb(149, 165, 166));
            btnClear.Click += btnClear_Click;
            leftPanel.Controls.Add(btnClear);

            // ---------- දකුණු පැත්ත: Search Bar (Top) + Grid (Fill) ----------
            Panel rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.BackColor = Color.FromArgb(240, 242, 245);
            rightPanel.Padding = new Padding(20);
            this.Controls.Add(rightPanel);
            rightPanel.BringToFront();

            // Search Bar
            Panel searchBar = new Panel();
            searchBar.Dock = DockStyle.Top;
            searchBar.Height = 55;
            rightPanel.Controls.Add(searchBar);

            txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 11F);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.Width = 300;
            txtSearch.Location = new Point(0, 10);
            searchBar.Controls.Add(txtSearch);

            btnSearch = new Button();
            btnSearch.Text = "Search";
            btnSearch.Size = new Size(110, 36);
            btnSearch.Location = new Point(310, 9);
            StyleButton(btnSearch, Color.FromArgb(41, 128, 185));
            btnSearch.Click += btnSearch_Click;
            searchBar.Controls.Add(btnSearch);

            // Grid
            dgvCustomers = new DataGridView();
            dgvCustomers.Dock = DockStyle.Fill;
            dgvCustomers.BackgroundColor = Color.White;
            dgvCustomers.BorderStyle = BorderStyle.None;
            dgvCustomers.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCustomers.RowHeadersVisible = false;
            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.ReadOnly = true;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCustomers.EnableHeadersVisualStyles = false;
            dgvCustomers.ColumnHeadersHeight = 40;
            dgvCustomers.ColumnHeadersDefaultCellStyle.BackColor = darkColor;
            dgvCustomers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCustomers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvCustomers.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvCustomers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 240, 241);
            dgvCustomers.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvCustomers.RowTemplate.Height = 32;
            dgvCustomers.CellClick += dgvCustomers_CellClick;

            rightPanel.Controls.Add(dgvCustomers);
            dgvCustomers.BringToFront();
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
        private void LoadCustomers()
        {
            dgvCustomers.DataSource = customerRepo.GetAllCustomers();
        }

        // ==========================================
        // 3. Save / Update / Delete / Search Logic
        // ==========================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Customer Name is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Customer customer = new Customer
            {
                CustomerName = txtName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Address = txtAddress.Text.Trim()
            };

            if (customerRepo.AddCustomer(customer))
            {
                MessageBox.Show("Customer added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadCustomers();
            }
            else
            {
                MessageBox.Show("Failed to add customer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedCustomerId == 0)
            {
                MessageBox.Show("Please select a customer from the table to update!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Customer Name is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Customer customer = new Customer
            {
                CustomerID = selectedCustomerId,
                CustomerName = txtName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Address = txtAddress.Text.Trim()
            };

            if (customerRepo.UpdateCustomer(customer))
            {
                MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadCustomers();
            }
            else
            {
                MessageBox.Show("Failed to update customer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedCustomerId == 0)
            {
                MessageBox.Show("Please select a customer from the table to delete!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this customer?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                if (customerRepo.DeleteCustomer(selectedCustomerId))
                {
                    MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    LoadCustomers();
                }
                else
                {
                    MessageBox.Show("Failed to delete customer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                LoadCustomers();
            }
            else
            {
                dgvCustomers.DataSource = customerRepo.SearchCustomers(keyword);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            lblLoyaltyPoints.Text = "";
            selectedCustomerId = 0;
        }

        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCustomers.Rows[e.RowIndex];

                selectedCustomerId = Convert.ToInt32(row.Cells["CustomerID"].Value);
                txtName.Text = row.Cells["CustomerName"].Value?.ToString();
                txtPhone.Text = row.Cells["Phone"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtAddress.Text = row.Cells["Address"].Value?.ToString();

                int points = Convert.ToInt32(row.Cells["LoyaltyPoints"].Value);
                lblLoyaltyPoints.Text = "⭐ Loyalty Points: " + points;
            }
        }
    }
}