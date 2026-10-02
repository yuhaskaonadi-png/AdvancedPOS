using System;
using System.Drawing;
using System.Windows.Forms;

namespace AdvancedPOS.Views
{
    public partial class CategoryForm : Form
    {
        // ==========================================
        // UI Controls (Code එකෙන්ම හදනවා)
        // ==========================================
        private TextBox txtCategoryName;
        private TextBox txtDescription;
        private Button btnSave, btnUpdate, btnDelete;
        private DataGridView dgvCategories;

        private int selectedCategoryId = 0;
        private readonly Color darkColor = Color.FromArgb(43, 43, 54);

        public CategoryForm()
        {
            InitializeComponent();
            BuildUI();
            LoadCategoryData();
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
            lblHeading.Text = "Manage Categories";
            lblHeading.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(30, 20);
            leftPanel.Controls.Add(lblHeading);

            int y = 80;

            // Category Name
            Label lblName = new Label();
            lblName.Text = "Category Name";
            lblName.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblName.ForeColor = Color.Gray;
            lblName.AutoSize = true;
            lblName.Location = new Point(30, y);
            leftPanel.Controls.Add(lblName);

            txtCategoryName = new TextBox();
            txtCategoryName.Font = new Font("Segoe UI", 11F);
            txtCategoryName.BorderStyle = BorderStyle.FixedSingle;
            txtCategoryName.Width = 350;
            txtCategoryName.Location = new Point(30, y + 20);
            leftPanel.Controls.Add(txtCategoryName);

            y += 68;

            // Description
            Label lblDesc = new Label();
            lblDesc.Text = "Description";
            lblDesc.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblDesc.ForeColor = Color.Gray;
            lblDesc.AutoSize = true;
            lblDesc.Location = new Point(30, y);
            leftPanel.Controls.Add(lblDesc);

            txtDescription = new TextBox();
            txtDescription.Font = new Font("Segoe UI", 11F);
            txtDescription.BorderStyle = BorderStyle.FixedSingle;
            txtDescription.Width = 350;
            txtDescription.Height = 150;
            txtDescription.Multiline = true;
            txtDescription.Location = new Point(30, y + 20);
            leftPanel.Controls.Add(txtDescription);

            y += 195;

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

            // ---------- දකුණු පැත්ත: Grid Container (Dock=Fill) ----------
            Panel rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.BackColor = Color.FromArgb(240, 242, 245);
            rightPanel.Padding = new Padding(20);
            this.Controls.Add(rightPanel);
            rightPanel.BringToFront();

            dgvCategories = new DataGridView();
            dgvCategories.Dock = DockStyle.Fill;
            dgvCategories.BackgroundColor = Color.White;
            dgvCategories.BorderStyle = BorderStyle.None;
            dgvCategories.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCategories.RowHeadersVisible = false;
            dgvCategories.AllowUserToAddRows = false;
            dgvCategories.ReadOnly = true;
            dgvCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCategories.MultiSelect = false;
            dgvCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCategories.EnableHeadersVisualStyles = false;
            dgvCategories.ColumnHeadersHeight = 40;
            dgvCategories.ColumnHeadersDefaultCellStyle.BackColor = darkColor;
            dgvCategories.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCategories.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvCategories.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvCategories.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvCategories.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 240, 241);
            dgvCategories.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvCategories.RowTemplate.Height = 32;
            dgvCategories.CellDoubleClick += dgvCategories_CellDoubleClick;

            rightPanel.Controls.Add(dgvCategories);

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
        private void LoadCategoryData()
        {
            AdvancedPOS.Repositories.CategoryRepository catRepo = new AdvancedPOS.Repositories.CategoryRepository();
            dgvCategories.DataSource = catRepo.GetCategories();

            if (dgvCategories.Columns.Contains("CategoryID"))
            {
                dgvCategories.Columns["CategoryID"].Visible = false;
            }
        }

        // ==========================================
        // 3. Save / Update / Delete Logic (ඔයාගේ පරණ code එකම)
        // ==========================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            string catName = txtCategoryName.Text.Trim();
            string catDesc = txtDescription.Text.Trim();

            if (string.IsNullOrEmpty(catName))
            {
                MessageBox.Show("Please enter the Category Name!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AdvancedPOS.Models.Category newCategory = new AdvancedPOS.Models.Category
            {
                CategoryName = catName,
                Description = catDesc
            };

            AdvancedPOS.Repositories.CategoryRepository catRepo = new AdvancedPOS.Repositories.CategoryRepository();
            bool success = catRepo.AddCategory(newCategory);

            if (success)
            {
                MessageBox.Show("Category saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtCategoryName.Clear();
                txtDescription.Clear();
                LoadCategoryData();
            }
            else
            {
                MessageBox.Show("Failed to save category.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCategories_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCategories.Rows[e.RowIndex];

                selectedCategoryId = Convert.ToInt32(row.Cells["CategoryID"].Value);
                txtCategoryName.Text = row.Cells["CategoryName"].Value.ToString();
                txtDescription.Text = row.Cells["Description"].Value.ToString();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedCategoryId == 0)
            {
                MessageBox.Show("Please double-click a category from the table to update!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string catName = txtCategoryName.Text.Trim();
            string catDesc = txtDescription.Text.Trim();

            if (string.IsNullOrEmpty(catName))
            {
                MessageBox.Show("Please enter the Category Name!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AdvancedPOS.Models.Category updateCat = new AdvancedPOS.Models.Category
            {
                CategoryID = selectedCategoryId,
                CategoryName = catName,
                Description = catDesc
            };

            AdvancedPOS.Repositories.CategoryRepository catRepo = new AdvancedPOS.Repositories.CategoryRepository();
            bool success = catRepo.UpdateCategory(updateCat);

            if (success)
            {
                MessageBox.Show("Category updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtCategoryName.Clear();
                txtDescription.Clear();
                selectedCategoryId = 0;
                LoadCategoryData();
            }
            else
            {
                MessageBox.Show("Failed to update category.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedCategoryId == 0)
            {
                MessageBox.Show("Please double-click a category from the table to delete!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this category?\nThis action cannot be undone.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dialogResult == DialogResult.Yes)
            {
                AdvancedPOS.Repositories.CategoryRepository catRepo = new AdvancedPOS.Repositories.CategoryRepository();
                bool success = catRepo.DeleteCategory(selectedCategoryId);

                if (success)
                {
                    MessageBox.Show("Category deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtCategoryName.Clear();
                    txtDescription.Clear();
                    selectedCategoryId = 0;
                    LoadCategoryData();
                }
                else
                {
                    MessageBox.Show("Failed to delete category.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}