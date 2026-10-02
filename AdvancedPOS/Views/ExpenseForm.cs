using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AdvancedPOS.Models;
using AdvancedPOS.Repositories;

namespace AdvancedPOS.Views
{
    public partial class ExpenseForm : Form
    {
        // ==========================================
        // UI Controls (Code එකෙන්ම හදනවා)
        // ==========================================
        private DateTimePicker dtpExpenseDate, dtpFrom, dtpTo;
        private ComboBox cmbCategory;
        private TextBox txtAmount, txtDescription;
        private Button btnSave, btnClear, btnDelete, btnSearch;
        private DataGridView dgvExpenses;

        private readonly ExpenseRepository _expenseRepo = new ExpenseRepository();
        private int _selectedExpenseId = 0;
        private readonly Color darkColor = Color.FromArgb(43, 43, 54);

        public ExpenseForm()
        {
            InitializeComponent();
            BuildUI();
            SetupForm();
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
            lblHeading.Text = "Manage Expenses";
            lblHeading.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(30, 20);
            leftPanel.Controls.Add(lblHeading);

            int y = 80;

            // Expense Date
            Label lblDate = new Label();
            lblDate.Text = "Expense Date";
            lblDate.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblDate.ForeColor = Color.Gray;
            lblDate.AutoSize = true;
            lblDate.Location = new Point(30, y);
            leftPanel.Controls.Add(lblDate);

            dtpExpenseDate = new DateTimePicker();
            dtpExpenseDate.Font = new Font("Segoe UI", 11F);
            dtpExpenseDate.Format = DateTimePickerFormat.Short;
            dtpExpenseDate.Width = 350;
            dtpExpenseDate.Location = new Point(30, y + 20);
            leftPanel.Controls.Add(dtpExpenseDate);

            y += 68;

            // Category
            Label lblCat = new Label();
            lblCat.Text = "Category";
            lblCat.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblCat.ForeColor = Color.Gray;
            lblCat.AutoSize = true;
            lblCat.Location = new Point(30, y);
            leftPanel.Controls.Add(lblCat);

            cmbCategory = new ComboBox();
            cmbCategory.Font = new Font("Segoe UI", 11F);
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.Width = 350;
            cmbCategory.Location = new Point(30, y + 20);
            leftPanel.Controls.Add(cmbCategory);

            y += 68;

            // Amount
            Label lblAmount = new Label();
            lblAmount.Text = "Amount";
            lblAmount.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblAmount.ForeColor = Color.Gray;
            lblAmount.AutoSize = true;
            lblAmount.Location = new Point(30, y);
            leftPanel.Controls.Add(lblAmount);

            txtAmount = new TextBox();
            txtAmount.Font = new Font("Segoe UI", 11F);
            txtAmount.BorderStyle = BorderStyle.FixedSingle;
            txtAmount.Width = 350;
            txtAmount.Location = new Point(30, y + 20);
            leftPanel.Controls.Add(txtAmount);

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
            txtDescription.Height = 100;
            txtDescription.Multiline = true;
            txtDescription.Location = new Point(30, y + 20);
            leftPanel.Controls.Add(txtDescription);

            y += 145;

            // ---------- Buttons ----------
            btnSave = new Button();
            btnSave.Text = "💾 Save Expense";
            btnSave.Size = new Size(350, 44);
            btnSave.Location = new Point(30, y);
            StyleButton(btnSave, Color.FromArgb(39, 174, 96));
            btnSave.Click += BtnSave_Click;
            leftPanel.Controls.Add(btnSave);

            y += 55;

            btnClear = new Button();
            btnClear.Text = "Clear";
            btnClear.Size = new Size(165, 42);
            btnClear.Location = new Point(30, y);
            StyleButton(btnClear, Color.FromArgb(149, 165, 166));
            btnClear.Click += BtnClear_Click;
            leftPanel.Controls.Add(btnClear);

            btnDelete = new Button();
            btnDelete.Text = "Delete";
            btnDelete.Size = new Size(165, 42);
            btnDelete.Location = new Point(215, y);
            btnDelete.Enabled = false;
            StyleButton(btnDelete, Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;
            leftPanel.Controls.Add(btnDelete);

            // ---------- දකුණු පැත්ත: Search Filter (Top) + Grid (Fill) ----------
            Panel rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.BackColor = Color.FromArgb(240, 242, 245);
            rightPanel.Padding = new Padding(20);
            this.Controls.Add(rightPanel);
            rightPanel.BringToFront();

            // Search Filter Bar
            Panel searchBar = new Panel();
            searchBar.Dock = DockStyle.Top;
            searchBar.Height = 75;
            searchBar.BackColor = Color.FromArgb(240, 242, 245);
            rightPanel.Controls.Add(searchBar);

            int sx = 0;

            Label lblFrom = new Label();
            lblFrom.Text = "From";
            lblFrom.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblFrom.ForeColor = Color.Gray;
            lblFrom.AutoSize = true;
            lblFrom.Location = new Point(sx, 0);
            searchBar.Controls.Add(lblFrom);

            dtpFrom = new DateTimePicker();
            dtpFrom.Font = new Font("Segoe UI", 10.5F);
            dtpFrom.Format = DateTimePickerFormat.Short;
            dtpFrom.Width = 150;
            dtpFrom.Location = new Point(sx, 20);
            searchBar.Controls.Add(dtpFrom);

            sx += 170;

            Label lblTo = new Label();
            lblTo.Text = "To";
            lblTo.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblTo.ForeColor = Color.Gray;
            lblTo.AutoSize = true;
            lblTo.Location = new Point(sx, 0);
            searchBar.Controls.Add(lblTo);

            dtpTo = new DateTimePicker();
            dtpTo.Font = new Font("Segoe UI", 10.5F);
            dtpTo.Format = DateTimePickerFormat.Short;
            dtpTo.Width = 150;
            dtpTo.Location = new Point(sx, 20);
            searchBar.Controls.Add(dtpTo);

            sx += 170;

            btnSearch = new Button();
            btnSearch.Text = "Search";
            btnSearch.Size = new Size(120, 38);
            btnSearch.Location = new Point(sx, 18);
            StyleButton(btnSearch, Color.FromArgb(41, 128, 185));
            btnSearch.Click += BtnSearch_Click;
            searchBar.Controls.Add(btnSearch);

            // Grid
            dgvExpenses = new DataGridView();
            dgvExpenses.Dock = DockStyle.Fill;
            dgvExpenses.BackgroundColor = Color.White;
            dgvExpenses.BorderStyle = BorderStyle.None;
            dgvExpenses.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvExpenses.RowHeadersVisible = false;
            dgvExpenses.AllowUserToAddRows = false;
            dgvExpenses.ReadOnly = true;
            dgvExpenses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvExpenses.MultiSelect = false;
            dgvExpenses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvExpenses.EnableHeadersVisualStyles = false;
            dgvExpenses.ColumnHeadersHeight = 40;
            dgvExpenses.ColumnHeadersDefaultCellStyle.BackColor = darkColor;
            dgvExpenses.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvExpenses.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvExpenses.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvExpenses.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 240, 241);
            dgvExpenses.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvExpenses.RowTemplate.Height = 32;

            rightPanel.Controls.Add(dgvExpenses);
            dgvExpenses.BringToFront();

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
        // 2. Form Setup (ඔයාගේ පරණ code එකම)
        // ==========================================
        private void SetupForm()
        {
            dtpExpenseDate.Value = DateTime.Today;
            dtpFrom.Value = DateTime.Today;
            dtpTo.Value = DateTime.Today;

            LoadCategories();
            LoadExpenses();

            dgvExpenses.SelectionChanged += DgvExpenses_SelectionChanged;
        }

        private void LoadCategories()
        {
            try
            {
                var categories = _expenseRepo.GetAllCategories();
                cmbCategory.DataSource = categories;
                cmbCategory.DisplayMember = "CategoryName";
                cmbCategory.ValueMember = "CategoryID";
                cmbCategory.SelectedIndex = -1;
            }
            catch (Exception ex) { MessageBox.Show("Error loading categories: " + ex.Message); }
        }

        private void LoadExpenses()
        {
            try
            {
                DataTable dt = _expenseRepo.GetExpensesHistory(dtpFrom.Value, dtpTo.Value);
                dgvExpenses.DataSource = dt;

                if (dgvExpenses.Columns["ExpenseID"] != null) dgvExpenses.Columns["ExpenseID"].Visible = false;
                if (dgvExpenses.Columns["CategoryID"] != null) dgvExpenses.Columns["CategoryID"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("Error loading expenses: " + ex.Message); }
        }

        // ==========================================
        // 3. Save / Update / Delete Logic (ඔයාගේ පරණ code එකම)
        // ==========================================
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadExpenses();
            ClearInputs();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cmbCategory.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an Expense Category.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid expense amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Expense exp = new Expense
            {
                ExpenseID = _selectedExpenseId,
                CategoryID = Convert.ToInt32(cmbCategory.SelectedValue),
                Amount = amount,
                ExpenseDate = dtpExpenseDate.Value,
                Description = txtDescription.Text.Trim(),
                UserID = 1
            };

            try
            {
                bool success = false;
                if (_selectedExpenseId == 0)
                {
                    success = _expenseRepo.AddExpense(exp);
                    if (success) MessageBox.Show("Expense added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    success = _expenseRepo.UpdateExpense(exp);
                    if (success) MessageBox.Show("Expense updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (success)
                {
                    LoadExpenses();
                    ClearInputs();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error saving expense: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void DgvExpenses_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvExpenses.CurrentRow != null && dgvExpenses.CurrentRow.Index > -1)
            {
                DataGridViewRow row = dgvExpenses.CurrentRow;

                _selectedExpenseId = Convert.ToInt32(row.Cells["ExpenseID"].Value);
                dtpExpenseDate.Value = Convert.ToDateTime(row.Cells["ExpenseDate"].Value);
                cmbCategory.SelectedValue = Convert.ToInt32(row.Cells["CategoryID"].Value);
                txtAmount.Text = row.Cells["Amount"].Value.ToString();
                txtDescription.Text = row.Cells["Description"].Value != DBNull.Value ? row.Cells["Description"].Value.ToString() : "";

                btnSave.Text = "📝 Update Expense";
                btnDelete.Enabled = true;
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedExpenseId == 0) return;

            if (MessageBox.Show("Are you sure you want to delete this expense?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (_expenseRepo.DeleteExpense(_selectedExpenseId))
                    {
                        MessageBox.Show("Expense deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadExpenses();
                        ClearInputs();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error deleting expense: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void ClearInputs()
        {
            _selectedExpenseId = 0;
            dtpExpenseDate.Value = DateTime.Today;
            cmbCategory.SelectedIndex = -1;
            txtAmount.Clear();
            txtDescription.Clear();

            btnSave.Text = "💾 Save Expense";
            btnDelete.Enabled = false;
            dgvExpenses.ClearSelection();
        }
    }
}