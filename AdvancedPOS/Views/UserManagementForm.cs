using System;
using System.Drawing;
using System.Windows.Forms;
using AdvancedPOS.Models;
using AdvancedPOS.Repositories;

namespace AdvancedPOS.Views
{
    public partial class UserManagementForm : Form
    {
        // ==========================================
        // UI Controls (Code එකෙන්ම හදනවා)
        // ==========================================
        private TextBox txtUsername, txtPassword;
        private ComboBox cmbRole;
        private Button btnAddUser, btnDeleteUser;
        private DataGridView dgvUsers;

        private readonly UserRepository userRepo = new UserRepository();
        private readonly Color darkColor = Color.FromArgb(43, 43, 54);

        public UserManagementForm()
        {
            InitializeComponent();
            BuildUI();
            LoadUsers();
        }

        // ==========================================
        // 1. UI එක සම්පූර්ණයෙන්ම Code එකෙන් හැදීම
        // ==========================================
        private void BuildUI()
        {
            this.BackColor = Color.White;
            this.Text = "User Management";
            this.StartPosition = FormStartPosition.CenterParent;

            // ---------- වම් පැත්ත: Form Panel එක (Fixed Width) ----------
            Panel leftPanel = new Panel();
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Width = 350;
            leftPanel.BackColor = Color.White;
            leftPanel.Padding = new Padding(30, 20, 30, 20);
            leftPanel.AutoScroll = true;
            this.Controls.Add(leftPanel);

            Label lblHeading = new Label();
            lblHeading.Text = "Add New User";
            lblHeading.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(30, 20);
            leftPanel.Controls.Add(lblHeading);

            int y = 75;

            // Username
            Label lblUser = new Label();
            lblUser.Text = "Username";
            lblUser.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblUser.ForeColor = Color.Gray;
            lblUser.AutoSize = true;
            lblUser.Location = new Point(30, y);
            leftPanel.Controls.Add(lblUser);

            txtUsername = new TextBox();
            txtUsername.Font = new Font("Segoe UI", 11F);
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.Width = 280;
            txtUsername.Location = new Point(30, y + 20);
            leftPanel.Controls.Add(txtUsername);

            y += 68;

            // Password
            Label lblPass = new Label();
            lblPass.Text = "Password";
            lblPass.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblPass.ForeColor = Color.Gray;
            lblPass.AutoSize = true;
            lblPass.Location = new Point(30, y);
            leftPanel.Controls.Add(lblPass);

            txtPassword = new TextBox();
            txtPassword.Font = new Font("Segoe UI", 11F);
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Width = 280;
            txtPassword.Location = new Point(30, y + 20);
            leftPanel.Controls.Add(txtPassword);

            y += 68;

            // Role
            Label lblRole = new Label();
            lblRole.Text = "Role";
            lblRole.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblRole.ForeColor = Color.Gray;
            lblRole.AutoSize = true;
            lblRole.Location = new Point(30, y);
            leftPanel.Controls.Add(lblRole);

            cmbRole = new ComboBox();
            cmbRole.Font = new Font("Segoe UI", 11F);
            cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRole.Width = 280;
            cmbRole.Location = new Point(30, y + 20);
            cmbRole.Items.Add("Admin");
            cmbRole.Items.Add("Cashier");
            leftPanel.Controls.Add(cmbRole);

            y += 75;

            // ---------- Buttons ----------
            btnAddUser = new Button();
            btnAddUser.Text = "Add User";
            btnAddUser.Size = new Size(280, 44);
            btnAddUser.Location = new Point(30, y);
            StyleButton(btnAddUser, Color.FromArgb(39, 174, 96));
            btnAddUser.Click += btnAddUser_Click;
            leftPanel.Controls.Add(btnAddUser);

            y += 55;

            btnDeleteUser = new Button();
            btnDeleteUser.Text = "Delete Selected User";
            btnDeleteUser.Size = new Size(280, 44);
            btnDeleteUser.Location = new Point(30, y);
            StyleButton(btnDeleteUser, Color.FromArgb(231, 76, 60));
            btnDeleteUser.Click += btnDeleteUser_Click;
            leftPanel.Controls.Add(btnDeleteUser);

            // ---------- දකුණු පැත්ත: Grid Container (Dock=Fill) ----------
            Panel rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.BackColor = Color.FromArgb(240, 242, 245);
            rightPanel.Padding = new Padding(20);
            this.Controls.Add(rightPanel);
            rightPanel.BringToFront();

            dgvUsers = new DataGridView();
            dgvUsers.Dock = DockStyle.Fill;
            dgvUsers.BackgroundColor = Color.White;
            dgvUsers.BorderStyle = BorderStyle.None;
            dgvUsers.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvUsers.RowHeadersVisible = false;
            dgvUsers.AllowUserToAddRows = false;
            dgvUsers.ReadOnly = true;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.MultiSelect = false;
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsers.EnableHeadersVisualStyles = false;
            dgvUsers.ColumnHeadersHeight = 40;
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = darkColor;
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvUsers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvUsers.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvUsers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 240, 241);
            dgvUsers.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvUsers.RowTemplate.Height = 32;

            rightPanel.Controls.Add(dgvUsers);
        }

        private void StyleButton(Button btn, Color color)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;

            AdvancedPOS.Helpers.ThemeManager.ApplyToForm(this,
            AdvancedPOS.Helpers.ThemeManager.MainBackground,
            AdvancedPOS.Helpers.ThemeManager.CardBackground,
            AdvancedPOS.Helpers.ThemeManager.PrimaryTextColor,
            AdvancedPOS.Helpers.ThemeManager.SecondaryTextColor);
        }

        // ==========================================
        // 2. Data Loading + Logic (ඔයාගේ පරණ code එකම)
        // ==========================================
        private void LoadUsers()
        {
            dgvUsers.DataSource = userRepo.GetAllUsers();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text) || cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Please fill all fields!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            User newUser = new User
            {
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Text.Trim(),
                Role = cmbRole.SelectedItem.ToString()
            };

            if (userRepo.AddUser(newUser))
            {
                MessageBox.Show("User added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers();
                txtUsername.Clear();
                txtPassword.Clear();
                cmbRole.SelectedIndex = -1;
            }
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["UserID"].Value);
                if (userRepo.DeleteUser(id))
                {
                    MessageBox.Show("User deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers();
                }
            }
            else
            {
                MessageBox.Show("Please select a user from the table to delete!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}