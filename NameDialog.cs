using System;
using System.Drawing;
using System.Windows.Forms;

namespace CyberGuard
{
    // A simple dialog to collect and validate the user's name at startup
    public class NameDialog : Form
    {
        public string UserName { get; private set; } = "";

        private TextBox txtName;
        private Button btnOK;
        private Button btnCancel;
        private Label lblError;

        public NameDialog()
        {
            this.Text = "Welcome to CyberGuard";
            this.Size = new Size(380, 200);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Font = new Font("Segoe UI", 9.5f);

            Label lblPrompt = new Label
            {
                Text = "Enter your name to get started:",
                Location = new Point(20, 20),
                Size = new Size(330, 22),
                ForeColor = Color.FromArgb(0, 80, 0),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };

            txtName = new TextBox
            {
                Location = new Point(20, 50),
                Size = new Size(330, 26),
                Font = new Font("Segoe UI", 10f),
                MaxLength = 30
            };
            txtName.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; TryAccept(); }
            };

            lblError = new Label
            {
                Location = new Point(20, 82),
                Size = new Size(330, 20),
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 8.5f),
                Text = ""
            };

            btnOK = new Button
            {
                Text = "Start Chat",
                Location = new Point(160, 110),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(0, 100, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                DialogResult = DialogResult.None
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += (s, e) => TryAccept();

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(260, 110),
                Size = new Size(90, 30),
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(new Control[] { lblPrompt, txtName, lblError, btnOK, btnCancel });
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void TryAccept()
        {
            string name = txtName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                lblError.Text = "Name cannot be empty. Please try again.";
                return;
            }
            if (name.Length < 2)
            {
                lblError.Text = "Name must be at least 2 characters.";
                return;
            }
            if (name.Length > 30)
            {
                lblError.Text = "Name must be 30 characters or less.";
                return;
            }
            foreach (char c in name)
            {
                if (!char.IsLetter(c) && c != ' ')
                {
                    lblError.Text = "Name must contain letters only. No numbers or symbols.";
                    return;
                }
            }

            UserName = name;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
