using System;
using System.Drawing;
using System.Windows.Forms;

namespace CyberGuard
{
    public class MainForm : Form
    {
        private Chatbot _bot;
        private string _userName = "";

        private RichTextBox rtbChat;
        private TextBox txtInput;
        private Button btnSend;

        // ── Color palette ──────────────────────────────────────────────────
        private readonly Color Black = Color.FromArgb(10, 10, 10);
        private readonly Color DarkPanel = Color.FromArgb(15, 15, 15);
        private readonly Color Orange = Color.FromArgb(255, 106, 0);
        private readonly Color OrangeDim = Color.FromArgb(204, 68, 0);
        private readonly Color OrangeBg = Color.FromArgb(22, 8, 0);
        private readonly Color Green = Color.FromArgb(34, 197, 94);
        private readonly Color GreenDim = Color.FromArgb(13, 31, 16);
        private readonly Color GreenText = Color.FromArgb(163, 217, 165);
        private readonly Color Red = Color.FromArgb(224, 48, 48);
        private readonly Color RedBg = Color.FromArgb(22, 0, 0);
        private readonly Color Border = Color.FromArgb(42, 42, 42);
        private readonly Color DarkBorder = Color.FromArgb(26, 26, 26);
        private readonly Color MutedText = Color.FromArgb(100, 100, 100);

        public MainForm()
        {
            BuildUI();
            AskForName();
        }

        private void BuildUI()
        {
            this.Text = "CyberGuard v3.0 — Cybersecurity Awareness Chatbot";
            this.Size = new Size(860, 640);
            this.MinimumSize = new Size(700, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Black;
            this.ForeColor = Color.White;
            this.Font = new Font("Consolas", 9.5f);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ── Title bar ─────────────────────────────────────────────────
            Panel pnlTitle = new Panel
            {
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = DarkPanel
            };
            Panel pnlTitleBorder = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = Orange
            };
            Label lblTitle = new Label
            {
                Text = "[ CyberGuard v3.0  —  Cybersecurity Awareness Chatbot ]",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Orange,
                Font = new Font("Consolas", 9.5f, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            pnlTitle.Controls.Add(lblTitle);
            pnlTitle.Controls.Add(pnlTitleBorder);

            // ── Chat header bar ────────────────────────────────────────────
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 32,
                BackColor = DarkPanel
            };
            Panel pnlHeaderBorder = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = DarkBorder
            };
            Panel pnlDot = new Panel
            {
                Size = new Size(8, 8),
                Location = new Point(14, 12),
                BackColor = Green
            };
            pnlDot.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(new SolidBrush(Green), 0, 0, 8, 8);
            };
            Label lblHeaderTitle = new Label
            {
                Text = "> CyberGuard Assistant",
                Location = new Point(28, 7),
                AutoSize = true,
                ForeColor = Green,
                Font = new Font("Consolas", 9.5f, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            Label lblHeaderMeta = new Label
            {
                Text = "session active  |  type a topic or number, then press Enter",
                Location = new Point(210, 9),
                AutoSize = true,
                ForeColor = MutedText,
                Font = new Font("Consolas", 8.5f),
                BackColor = Color.Transparent
            };
            pnlHeader.Controls.Add(pnlDot);
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Controls.Add(lblHeaderMeta);
            pnlHeader.Controls.Add(pnlHeaderBorder);

            // ── Chat RichTextBox ───────────────────────────────────────────
            rtbChat = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Black,
                ForeColor = GreenText,
                Font = new Font("Consolas", 10f),
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                WordWrap = true,
                Padding = new Padding(10)
            };

            // ── Footer strip ───────────────────────────────────────────────
            Panel pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 22,
                BackColor = DarkPanel
            };
            Panel pnlFooterBorder = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = DarkBorder
            };
            Label lblFooter = new Label
            {
                Text = "  add task · view tasks · start quiz · activity log · password · phishing · scam · wifi · 2fa  |  type 'exit' to quit",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = MutedText,
                Font = new Font("Consolas", 7.5f),
                BackColor = Color.Transparent
            };
            pnlFooter.Controls.Add(lblFooter);
            pnlFooter.Controls.Add(pnlFooterBorder);

            // ── Input bar ──────────────────────────────────────────────────
            Panel pnlInput = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 46,
                BackColor = DarkPanel,
                Padding = new Padding(10, 7, 10, 7)
            };
            Panel pnlInputBorder = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = DarkBorder
            };

            txtInput = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10.5f),
                BackColor = Black,
                
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.FromArgb(255, 154, 74)
            };

            btnSend = new Button
            {
                Text = "Send  >",
                Dock = DockStyle.Right,
                Width = 90,
                BackColor = Orange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.FlatAppearance.MouseOverBackColor = OrangeDim;

            txtInput.KeyDown += TxtInput_KeyDown;
            btnSend.Click += BtnSend_Click;

            pnlInput.Controls.Add(txtInput);
            pnlInput.Controls.Add(btnSend);
            pnlInput.Controls.Add(pnlInputBorder);

            // ── Assemble form ──────────────────────────────────────────────
            this.Controls.Add(rtbChat);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlTitle);
            this.Controls.Add(pnlFooter);
            this.Controls.Add(pnlInput);
        }

        // ── Name dialog ────────────────────────────────────────────────────
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
            VoiceGreeting voice = new VoiceGreeting();
            voice.PlayGreeting(this);

            _bot = new Chatbot(_userName);

            // ASCII banner
            AppendText("\n", GreenText, false);
            AppendText(
        "  ____        _                  ____                      _ \n" +
        " / ___|  _   | |__   ___ _ __  / ___|_   _  __ _ _ __ __| |\n" +
        "| |     | | | | '_ \\/ _ \\ '__| |  _| | | |/ _` | '__/ _` |\n" +
        "| |___  | |_| | |_) |  __/ |  | |_| | |_| | (_| | | | (_| |\n" +
        " \\____|  \\__, |_.__/ \\___|_|   \\____|\\__,_|\\__,_|_|  \\__,_|\n" +
        "         |___/\n",
                Orange, false);
            AppendText("   >> CYBERSECURITY AWARENESS CHATBOT v3.0 <<\n", OrangeDim, true);
            AppendText("────────────────────────────────────────────────────────────\n\n",
                Color.FromArgb(40, 40, 40), false);

            AppendBotMessage(_bot.GetWelcome());
        }

        // ── Message pipeline ───────────────────────────────────────────────
        private void ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;
            AppendUserMessage(input);
            string response = _bot.Respond(input);
            AppendBotMessage(response);
        }

        private void SendMessage()
        {
            string input = txtInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;
            txtInput.Clear();
            ProcessInput(input);
        }

        // ── Display helpers ────────────────────────────────────────────────
        private void AppendUserMessage(string text)
        {
            AppendText("You > ", Orange, true);
            AppendText(text + "\n\n", Color.FromArgb(255, 154, 74), false);
        }

        private void AppendBotMessage(string text)
        {
            bool isError = text.StartsWith("[ERROR]") || text.StartsWith("I didn't") || text.StartsWith("Hmm") || text.StartsWith("Sorry") || text.StartsWith("I'm still");
            string display = text.StartsWith("[ERROR]") ? text.Substring(7).Trim() : text;

            if (isError)
            {
                AppendText("CyberGuard > ", Red, true);
                AppendText(display + "\n\n", Red, false);
            }
            else
            {
                AppendText("CyberGuard > ", Green, true);
                AppendText(display + "\n\n", GreenText, false);
            }

            rtbChat.ScrollToCaret();
        }

        private void AppendText(string text, Color color, bool bold)
        {
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionLength = 0;
            rtbChat.SelectionColor = color;
            rtbChat.SelectionFont = new Font("Consolas", 10f,
                bold ? FontStyle.Bold : FontStyle.Regular);
            rtbChat.AppendText(text);
            rtbChat.SelectionColor = rtbChat.ForeColor;
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; SendMessage(); }
        }

        private void BtnSend_Click(object sender, EventArgs e) { SendMessage(); }
    }
}