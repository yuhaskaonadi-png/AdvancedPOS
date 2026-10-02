using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AdvancedPOS.Views
{
    public partial class LoginForm : Form
    {
        private readonly Color darkColor = Color.FromArgb(43, 43, 54);
        private readonly Color accentColor = Color.FromArgb(41, 128, 185);

        public LoginForm()
        {
            InitializeComponent();
            ApplyStyling();

            // Logout කරලා ආපහු Login Form එක පෙන්නද්දී පරණ දත්ත මකනවා
            this.VisibleChanged += (s, e) =>
            {
                if (this.Visible)
                {
                    txtUsername.Clear();
                    txtPassword.Clear();
                    txtUsername.Focus();
                }
            };
        }

        // ==========================================
        // Login Form එකේ පෙනුම (Design View එකට අත නොගා)
        // ==========================================
        private void ApplyStyling()
        {
            this.SuspendLayout();

            // ---- Form එකේ මූලික සැකසුම් ----
            this.Text = "Advanced POS - Login";
            this.ClientSize = new Size(900, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.AcceptButton = button1; // Enter එබුවාම Login වෙනවා

            // ---- පරණ Labels හංගනවා (අලුත් ඒවා අපි හදනවා) ----
            List<Label> oldLabels = this.Controls.OfType<Label>().ToList();
            foreach (Label l in oldLabels)
            {
                l.Visible = false;
            }

            // ---- වම් පැත්තේ Branding Panel එක ----
            Panel leftPanel = new Panel();
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Width = 380;
            leftPanel.BackColor = darkColor;

            Label lblBrand = new Label();
            lblBrand.Text = "POS SYSTEM";
            lblBrand.Dock = DockStyle.Fill;
            lblBrand.TextAlign = ContentAlignment.MiddleCenter;
            lblBrand.ForeColor = Color.White;
            lblBrand.Font = new Font("Segoe UI", 26F, FontStyle.Bold);

            Label lblTagline = new Label();
            lblTagline.Text = "Point of Sale & Inventory Management";
            lblTagline.Dock = DockStyle.Bottom;
            lblTagline.Height = 90;
            lblTagline.TextAlign = ContentAlignment.TopCenter;
            lblTagline.ForeColor = Color.Silver;
            lblTagline.Font = new Font("Segoe UI", 10F);

            leftPanel.Controls.Add(lblBrand);
            leftPanel.Controls.Add(lblTagline);
            lblBrand.BringToFront();

            // ---- දකුණු පැත්තේ සුදු Panel එක ----
            Panel rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.BackColor = Color.White;

            // ---- Form එක තියෙන Holder Panel එක (රවුම් මැදට) ----
            Panel holder = new Panel();
            holder.Size = new Size(360, 380);
            holder.BackColor = Color.White;
            holder.Anchor = AnchorStyles.None;
            rightPanel.Controls.Add(holder);

            rightPanel.Resize += (s, e) =>
            {
                holder.Location = new Point(
                    (rightPanel.ClientSize.Width - holder.Width) / 2,
                    (rightPanel.ClientSize.Height - holder.Height) / 2);
            };

            // ---- මාතෘකා ----
            Label lblHeading = new Label();
            lblHeading.Text = "Welcome Back";
            lblHeading.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(0, 0);
            holder.Controls.Add(lblHeading);

            Label lblSub = new Label();
            lblSub.Text = "Please login to your account";
            lblSub.Font = new Font("Segoe UI", 10F);
            lblSub.ForeColor = Color.Gray;
            lblSub.AutoSize = true;
            lblSub.Location = new Point(2, 50);
            holder.Controls.Add(lblSub);

            // ---- Username ----
            Label lblUser = new Label();
            lblUser.Text = "USERNAME";
            lblUser.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblUser.ForeColor = Color.Gray;
            lblUser.AutoSize = true;
            lblUser.Location = new Point(2, 105);
            holder.Controls.Add(lblUser);

            holder.Controls.Add(txtUsername);
            txtUsername.Visible = true;
            txtUsername.Font = new Font("Segoe UI", 12F);
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.Width = 360;
            txtUsername.Location = new Point(0, 130);

            // ---- Password ----
            Label lblPass = new Label();
            lblPass.Text = "PASSWORD";
            lblPass.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPass.ForeColor = Color.Gray;
            lblPass.AutoSize = true;
            lblPass.Location = new Point(2, 190);
            holder.Controls.Add(lblPass);

            holder.Controls.Add(txtPassword);
            txtPassword.Visible = true;
            txtPassword.Font = new Font("Segoe UI", 12F);
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Width = 360;
            txtPassword.Location = new Point(0, 215);

            // ---- Show password checkbox ----
           
            CheckBox chkShow = new CheckBox();
            chkShow.Text = "Show password";
            chkShow.Font = new Font("Segoe UI", 9F);
            chkShow.ForeColor = Color.Gray;
            chkShow.AutoSize = true;
            chkShow.Location = new Point(2, 262);
            chkShow.Cursor = Cursors.Hand;

            // Best Practice: CheckBox Click Event එක නිවැරදිව හැසිරවීම
            chkShow.CheckedChanged += (s, e) =>
            {
                if (chkShow.Checked)
                {
                    // මුරපදය පෙන්වන්න (Hide කරන Properties දෙකම අක්‍රිය කිරීම)
                    txtPassword.UseSystemPasswordChar = false;
                    txtPassword.PasswordChar = '\0'; // '\0' යනු C# වල හිස් (Null) අක්ෂරයයි
                }
                else
                {
                    // මුරපදය නැවත සඟවන්න
                    txtPassword.UseSystemPasswordChar = true;
                }
            };
            holder.Controls.Add(chkShow);

            // ---- Login බොත්තම ----
            holder.Controls.Add(button1);
            button1.Visible = true;
            button1.Text = "LOGIN";
            button1.Size = new Size(360, 48);
            button1.Location = new Point(0, 310);
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(31, 108, 160);
            button1.BackColor = accentColor;
            button1.ForeColor = Color.White;
            button1.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            button1.Cursor = Cursors.Hand;

            // ---- Panels දෙක Form එකට දැමීම ----
            this.Controls.Add(rightPanel);
            this.Controls.Add(leftPanel);
            rightPanel.BringToFront(); // Fill panel එක අන්තිමට Dock වෙන්න

            // පළමු වතාවටම holder එක මැදට ගන්න
            holder.Location = new Point(
                (this.ClientSize.Width - leftPanel.Width - holder.Width) / 2,
                (this.ClientSize.Height - holder.Height) / 2);

            this.ResumeLayout(false);
            txtUsername.Focus();
        }

        // ==========================================
        // Login Logic (වෙනසක් නෑ)
        // ==========================================
        private void button1_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both Username and Password!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AdvancedPOS.Repositories.UserRepository userRepo = new AdvancedPOS.Repositories.UserRepository();
            AdvancedPOS.Models.User user = userRepo.AuthenticateUser(username, password);

            if (user != null)
            {
                AdvancedPOS.Helpers.UserSession.CurrentUserID = user.UserID;
                AdvancedPOS.Helpers.UserSession.CurrentUsername = user.Username;
                AdvancedPOS.Helpers.UserSession.CurrentRole = user.Role;

                // Login එක සාර්ථකයි නම් Dashboard එක Open කරනවා
                Views.DashboardForm dashboard = new Views.DashboardForm(user.Role);
                dashboard.Show();

                // Login Form එක හංගනවා
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid Username or Password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
    }
}