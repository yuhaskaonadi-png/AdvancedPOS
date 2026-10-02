using System;
using System.Drawing;
using System.Windows.Forms;
using AdvancedPOS.Helpers;

namespace AdvancedPOS.Views
{
    public partial class BackupRestoreForm : Form
    {
        // ==========================================
        // UI Controls (Code එකෙන්ම හදනවා)
        // ==========================================
        private Button btnBackup, btnRestore, btnClose;
        private Label lblStatus;
        private ProgressBar progressBar;

        private readonly Color darkColor = Color.FromArgb(43, 43, 54);

        public BackupRestoreForm()
        {
            InitializeComponent();
            BuildUI();
        }

        // ==========================================
        // 1. UI එක සම්පූර්ණයෙන්ම Code එකෙන් හැදීම
        // ==========================================
        private void BuildUI()
        {
            this.Text = "Database Backup & Restore";
            this.Size = new Size(480, 420);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            Panel container = new Panel();
            container.Dock = DockStyle.Fill;
            container.Padding = new Padding(35, 30, 35, 30);
            this.Controls.Add(container);

            Label lblHeading = new Label();
            lblHeading.Text = "Database Backup & Restore";
            lblHeading.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblHeading.ForeColor = darkColor;
            lblHeading.AutoSize = true;
            lblHeading.Location = new Point(0, 0);
            container.Controls.Add(lblHeading);

            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Keep a safe copy of your data, or restore from a previous backup.";
            lblSubtitle.Font = new Font("Segoe UI", 9.5F);
            lblSubtitle.ForeColor = Color.Gray;
            lblSubtitle.AutoSize = true;
            lblSubtitle.MaximumSize = new Size(400, 0);
            lblSubtitle.Location = new Point(0, 35);
            container.Controls.Add(lblSubtitle);

            int y = 90;

            // ---- Backup Card ----
            Panel backupCard = new Panel();
            backupCard.BackColor = Color.FromArgb(240, 242, 245);
            backupCard.Size = new Size(400, 100);
            backupCard.Location = new Point(0, y);
            backupCard.Padding = new Padding(20);
            container.Controls.Add(backupCard);

            Label lblBackupTitle = new Label();
            lblBackupTitle.Text = "💾 Create Backup";
            lblBackupTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblBackupTitle.ForeColor = darkColor;
            lblBackupTitle.AutoSize = true;
            lblBackupTitle.Location = new Point(20, 15);
            backupCard.Controls.Add(lblBackupTitle);

            Label lblBackupDesc = new Label();
            lblBackupDesc.Text = "Save all your data as a .bak file";
            lblBackupDesc.Font = new Font("Segoe UI", 9F);
            lblBackupDesc.ForeColor = Color.Gray;
            lblBackupDesc.AutoSize = true;
            lblBackupDesc.Location = new Point(20, 42);
            backupCard.Controls.Add(lblBackupDesc);

            btnBackup = new Button();
            btnBackup.Text = "Backup Now";
            btnBackup.Size = new Size(150, 38);
            btnBackup.Location = new Point(230, 30);
            StyleButton(btnBackup, Color.FromArgb(39, 174, 96));
            btnBackup.Click += btnBackup_Click;
            backupCard.Controls.Add(btnBackup);

            y += 120;

            // ---- Restore Card ----
            Panel restoreCard = new Panel();
            restoreCard.BackColor = Color.FromArgb(240, 242, 245);
            restoreCard.Size = new Size(400, 100);
            restoreCard.Location = new Point(0, y);
            restoreCard.Padding = new Padding(20);
            container.Controls.Add(restoreCard);

            Label lblRestoreTitle = new Label();
            lblRestoreTitle.Text = "♻ Restore Backup";
            lblRestoreTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblRestoreTitle.ForeColor = darkColor;
            lblRestoreTitle.AutoSize = true;
            lblRestoreTitle.Location = new Point(20, 15);
            restoreCard.Controls.Add(lblRestoreTitle);

            Label lblRestoreDesc = new Label();
            lblRestoreDesc.Text = "This will overwrite all current data!";
            lblRestoreDesc.Font = new Font("Segoe UI", 9F);
            lblRestoreDesc.ForeColor = Color.Crimson;
            lblRestoreDesc.AutoSize = true;
            lblRestoreDesc.Location = new Point(20, 42);
            restoreCard.Controls.Add(lblRestoreDesc);

            btnRestore = new Button();
            btnRestore.Text = "Restore";
            btnRestore.Size = new Size(150, 38);
            btnRestore.Location = new Point(230, 30);
            StyleButton(btnRestore, Color.FromArgb(231, 76, 60));
            btnRestore.Click += btnRestore_Click;
            restoreCard.Controls.Add(btnRestore);

            y += 120;

            // ---- Status / Progress ----
            lblStatus = new Label();
            lblStatus.Text = "";
            lblStatus.Font = new Font("Segoe UI", 9.5F);
            lblStatus.ForeColor = darkColor;
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(0, y);
            container.Controls.Add(lblStatus);

            progressBar = new ProgressBar();
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 30;
            progressBar.Size = new Size(400, 10);
            progressBar.Location = new Point(0, y + 22);
            progressBar.Visible = false;
            container.Controls.Add(progressBar);

            btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Size = new Size(400, 40);
            btnClose.Location = new Point(0, y + 45);
            StyleButton(btnClose, Color.FromArgb(149, 165, 166));
            btnClose.Click += (s, e) => this.Close();
            container.Controls.Add(btnClose);
        }

        private void StyleButton(Button btn, Color color)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        // ==========================================
        // 2. Backup Logic
        // ==========================================
        private async void btnBackup_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "SQL Server Backup Files (*.bak)|*.bak";
                sfd.FileName = $"AdvancedPOS_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                sfd.Title = "Save Database Backup";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                SetBusy(true, "Backing up database... Please wait.");

                try
                {
                    await System.Threading.Tasks.Task.Run(() =>
                        BackupRestoreHelper.BackupDatabase(sfd.FileName));

                    lblStatus.ForeColor = Color.ForestGreen;
                    lblStatus.Text = "✔ Backup completed successfully!";
                    MessageBox.Show("Database backed up successfully to:\n" + sfd.FileName,
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.Crimson;
                    lblStatus.Text = "✖ Backup failed.";
                    MessageBox.Show("Backup failed:\n" + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SetBusy(false, lblStatus.Text);
                }
            }
        }

        // ==========================================
        // 3. Restore Logic
        // ==========================================
        private async void btnRestore_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(
                "WARNING: This will completely replace all current data with the backup file.\n\n" +
                "This action cannot be undone. Are you sure you want to continue?",
                "Confirm Restore", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "SQL Server Backup Files (*.bak)|*.bak";
                ofd.Title = "Select Backup File to Restore";

                if (ofd.ShowDialog() != DialogResult.OK) return;

                SetBusy(true, "Restoring database... Please wait. Do not close the application.");

                try
                {
                    await System.Threading.Tasks.Task.Run(() =>
                        BackupRestoreHelper.RestoreDatabase(ofd.FileName));

                    lblStatus.ForeColor = Color.ForestGreen;
                    lblStatus.Text = "✔ Restore completed successfully!";
                    MessageBox.Show("Database restored successfully!\n\nPlease restart the application to ensure all data refreshes correctly.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.Crimson;
                    lblStatus.Text = "✖ Restore failed.";
                    MessageBox.Show("Restore failed:\n" + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SetBusy(false, lblStatus.Text);
                }
            }
        }

        private void SetBusy(bool busy, string statusText)
        {
            btnBackup.Enabled = !busy;
            btnRestore.Enabled = !busy;
            btnClose.Enabled = !busy;
            progressBar.Visible = busy;
            lblStatus.Text = statusText;
        }
    }
}