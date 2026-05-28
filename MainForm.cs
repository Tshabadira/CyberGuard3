using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace CyberGuard
{
    public partial class MainForm : Form
    {
        private Chatbot _bot;
        private string _userName = "";

        // Controls
        private RichTextBox rtbChat;
        private TextBox txtInput;
        private Button btnSend;
        private Panel pnlInput;
        private Label lblStatus;

        public MainForm()
        {
            
            BuildUI();
            AskForName();
        }

        // Builds all UI controls manually (no designer dependency)
        private void BuildUI()
        {
            this.Text = "CyberGuard - Cybersecurity Awareness Chatbot";
            this.Size = new Size(750, 600);
            this.MinimumSize = new Size(600, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.5f);
            this.BackColor = SystemColors.Control;

            // Status bar at top
            lblStatus = new Label
            {
                Dock = DockStyle.Top,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0),
                BackColor = Color.FromArgb(0, 100, 0),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Text = "CyberGuard  |  Your Cybersecurity Awareness Assistant"
            };

            // Chat display
            rtbChat = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Consolas", 10f),
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                WordWrap = true,
                Margin = new Padding(0)
            };

            // Input panel at bottom
            pnlInput = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 48,
                Padding = new Padding(8, 8, 8, 8),
                BackColor = SystemColors.ControlLight
            };

            txtInput = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10f),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtInput.KeyDown += TxtInput_KeyDown;

            btnSend = new Button
            {
                Text = "Send",
                Dock = DockStyle.Right,
                Width = 80,
                BackColor = Color.FromArgb(0, 100, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Click += BtnSend_Click;

            pnlInput.Controls.Add(txtInput);
            pnlInput.Controls.Add(btnSend);

            // Separator line above input
            Panel pnlSep = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = Color.Silver
            };

            this.Controls.Add(rtbChat);
            this.Controls.Add(pnlSep);
            this.Controls.Add(pnlInput);
            this.Controls.Add(lblStatus);
        }

        // Prompts for name at startup using an InputBox-style dialog
        private void AskForName()
        {
            using (NameDialog dlg = new NameDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _userName = dlg.UserName;
                    StartChatbot();
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        private void StartChatbot()
        {
            // Play voice greeting
            VoiceGreeting voice = new VoiceGreeting();
            voice.PlayGreeting(this);

            lblStatus.Text = $"CyberGuard  |  Welcome, {_userName}!  |  Type a topic or number, then press Enter or Send.";

            _bot = new Chatbot(_userName);

            // Print ASCII banner
            AppendText("============================================================\n", Color.DarkGreen, bold: true);
            AppendText(
@"  ____        _                  ____                      _ 
 / ___|  _   | |__   ___ _ __  / ___|_   _  __ _ _ __ __| |
| |     | | | | '_ \ / _ \ '__| |  _| | | |/ _` | '__/ _` |
| |___  | |_| | |_) |  __/ |  | |_| | |_| | (_| | | | (_| |
 \____|  \__, |_.__/ \___|_|   \____|\__,_|\__,_|_|  \__,_|
         |___/                                               " + "\n",
                Color.DarkGreen, bold: false, mono: true);
            AppendText("          CYBERSECURITY AWARENESS CHATBOT\n", Color.DarkGreen, bold: true);
            AppendText("     Your guide to staying safe in the digital world.\n", Color.DarkGreen, bold: false);
            AppendText("============================================================\n\n", Color.DarkGreen, bold: true);

            // Initial bot greeting + menu
            string welcome = _bot.GetWelcome();
            AppendBotMessage(welcome);
        }

        // Sends user input to chatbot and displays response
        private void SendMessage()
        {
            string input = txtInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            AppendUserMessage(input);
            txtInput.Clear();

            string response = _bot.Respond(input);
            AppendBotMessage(response);
        }

        // Appends a user message (black, right-labelled)
        private void AppendUserMessage(string text)
        {
            AppendText("You: ", Color.Black, bold: true);
            AppendText(text + "\n\n", Color.Black, bold: false);
        }

        // Appends a bot message (green or red for errors)
        private void AppendBotMessage(string text)
        {
            bool isError = text.StartsWith("[ERROR]");
            Color color = isError ? Color.Red : Color.DarkGreen;
            string display = isError ? text.Substring(7).Trim() : text;

            AppendText("CyberGuard: ", color, bold: true);
            AppendText(display + "\n\n", color, bold: false);

            rtbChat.ScrollToCaret();
        }

        // Core append helper — appends styled text to RichTextBox
        private void AppendText(string text, Color color, bool bold, bool mono = false)
        {
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionLength = 0;
            rtbChat.SelectionColor = color;

            FontStyle style = bold ? FontStyle.Bold : FontStyle.Regular;
            string family = mono ? "Consolas" : "Segoe UI";
            rtbChat.SelectionFont = new Font(family, 10f, style);

            rtbChat.AppendText(text);
            rtbChat.SelectionColor = rtbChat.ForeColor;
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendMessage();
            }
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }
    }
}
