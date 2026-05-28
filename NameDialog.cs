using System;
using System.Drawing;
using System.Windows.Forms;

namespace CyberGuard
{
    public class NameDialog : Form
    {
        public string UserName { get; private set; } = "";

        private TextBox txtName;
        private Label lblError;

        private readonly Color Black = Color.FromArgb(10, 10, 10);
        private readonly Color DarkPanel = Color.FromArgb(15, 15, 15);
        private readonly Color Orange = Color.FromArgb(255, 106, 0);
        private readonly Color OrangeDim = Color.FromArgb(204, 68, 0);
        private readonly Color Green = Color.FromArgb(34, 197, 94);
        private readonly Color Red = Color.FromArgb(224, 48, 48);
        private readonly Color Border = Color.FromArgb(42, 42, 42);

        public NameDialog()
        {
            this.Text = "CyberGuard — Identify Yourself";
            this.Size = new Size(440, 240);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Black;
            this.ForeColor = Color.White;
            this.Font = new Font("Consolas", 9.5f);

            // Orange top strip
            Panel pnlStrip = new Panel
            {
                Dock = DockStyle.Top,
                Height = 42,
                BackColor = DarkPanel
            };
            Panel pnlStripBorder = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = Orange
            };
            Label lblHeader = new Label
            {
                Text = "[ CyberGuard v2.0  —  Enter your name to begin ]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Orange,
                Font = new Font("Consolas", 9f, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            pnlStrip.Controls.Add(lblHeader);
            pnlStrip.Controls.Add(pnlStripBorder);

            Label lblPrompt = new Label
            {
                Text = "> Your name:",
                Location = new Point(24, 60),
                AutoSize = true,
                ForeColor = Green,
                Font = new Font("Consolas", 9.5f)
            };

            txtName = new TextBox
            {
                Location = new Point(24, 82),
                Size = new Size(380, 28),
                Font = new Font("Consolas", 10.5f),
                BackColor = Color.FromArgb(15, 15, 15),
                ForeColor = Color.FromArgb(255, 154, 74),
                BorderStyle = BorderStyle.FixedSingle,
                MaxLength = 30
            };
            txtName.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; TryAccept(); }
            };

            lblError = new Label
            {
                Location = new Point(24, 114),
                Size = new Size(380, 18),
                ForeColor = Red,
                Font = new Font("Consolas", 8.5f),
                Text = ""
            };

            Button btnOK = new Button
            {
                Text = "Start Chat  >",
                Location = new Point(196, 140),
                Size = new Size(130, 34),
                BackColor = Orange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.FlatAppearance.MouseOverBackColor = OrangeDim;
            btnOK.Click += (s, e) => TryAccept();

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(80, 140),
                Size = new Size(100, 34),
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.FromArgb(130, 130, 130),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 9f),
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 1;
            btnCancel.FlatAppearance.BorderColor = Border;

            this.Controls.AddRange(new Control[]
            {
                pnlStrip, lblPrompt, txtName, lblError, btnOK, btnCancel
            });
            this.CancelButton = btnCancel;
        }

        private void TryAccept()
        {
            string name = txtName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            { lblError.Text = "[!] Name cannot be empty. Please try again."; return; }

            if (name.Length < 2)
            { lblError.Text = "[!] Name must be at least 2 characters."; return; }

            if (name.Length > 30)
            { lblError.Text = "[!] Name must be 30 characters or less."; return; }

            foreach (char c in name)
            {
                if (!char.IsLetter(c) && c != ' ')
                { lblError.Text = "[!] Letters only. No numbers or symbols."; return; }
            }

            UserName = name;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}