using System;
using System.Collections.Generic;

namespace CyberGuard
{
    internal class Chatbot
    {
        private string _userName;
        private string _lastTopic = "";
        private string _favouriteTopic = "";
        private readonly Random _random = new Random();

        // ── Random response pools (Requirement 3) ──────────────────────────
        private readonly List<string> _phishingResponses = new List<string>
        {
            "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
            "Always verify the sender's email address before clicking any link. Hover over links to preview the real URL.",
            "Legitimate banks never ask for your password via email. If in doubt, contact the organisation directly.",
            "Look out for urgent language in emails — 'Act now!' or 'Your account will be closed!' are classic phishing tricks."
        };

        private readonly List<string> _passwordResponses = new List<string>
        {
            "Make sure to use strong, unique passwords for each account. Avoid using personal details in your passwords.",
            "A good password is at least 12 characters long and mixes uppercase, lowercase, numbers, and symbols.",
            "Never reuse the same password across multiple sites. If one is breached, all your accounts are at risk.",
            "Consider using a passphrase — four random words joined together — it is long, strong, and easy to remember."
        };

        private readonly List<string> _privacyResponses = new List<string>
        {
            "Review your privacy settings on all social media accounts regularly to control who sees your information.",
            "Avoid sharing sensitive personal details like your ID number, address, or phone number in public forums.",
            "Use a VPN to keep your browsing private, especially on public Wi-Fi networks.",
            "Read app permissions carefully — many apps request access to data they do not actually need."
        };

        private readonly List<string> _scamResponses = new List<string>
        {
            "If an offer sounds too good to be true, it usually is. Never send money to someone you have not verified.",
            "Scammers often create urgency to prevent you from thinking clearly. Slow down and verify before acting.",
            "Never give your banking details or OTP to anyone who contacts you unexpectedly.",
            "Report suspected scams to your bank and local cybercrime authorities immediately."
        };

        // ── Sentiment detection map (Requirement 6) ───────────────────────
        private readonly Dictionary<string, string> _sentimentPrefixes =
            new Dictionary<string, string>
        {
            { "worried",     "It is completely understandable to feel worried. Cybersecurity can feel overwhelming, but you are already taking the right steps by learning about it.\n\n" },
            { "scared",      "Do not be scared — awareness is your best defence. Let me share something helpful.\n\n" },
            { "anxious",     "I understand the anxiety around cyber threats. You are doing the right thing by staying informed.\n\n" },
            { "confused",    "No worries at all — this can be confusing at first. Let me explain it clearly.\n\n" },
            { "frustrated",  "I hear you — it can be frustrating dealing with all these threats. Let me help make it simpler.\n\n" },
            { "curious",     "Great to see your curiosity! Asking questions is the first step to staying safe online.\n\n" },
            { "unsure",      "It is okay to be unsure — that is exactly what I am here for.\n\n" },
            { "overwhelmed", "Take a breath — you do not have to learn everything at once. Let me break it down for you.\n\n" }
        };

        public Chatbot(string userName)
        {
            _userName = userName;
        }

        public string GetWelcome()
        {
            return $"Hello {_userName}! Welcome to CyberGuard.\n" +
                   "I am your cybersecurity awareness assistant.\n\n" +
                   GetMenu();
        }

        public string Respond(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "[ERROR] Please type something before sending.";

            string lower = input.Trim().ToLower();

            if (lower == "exit" || lower == "bye")
                return $"Goodbye {_userName}! Stay safe online. You can close the window now.";

            // Sentiment detection (Requirement 6)
            string sentiment = DetectSentiment(lower);

            // Follow-up conversation flow (Requirement 4)
            if (lower.Contains("more") || lower.Contains("another tip") ||
                lower.Contains("tell me more") || lower.Contains("explain more") ||
                lower.Contains("give me more"))
                return sentiment + GiveMoreDetails();

            // Menu number shortcuts
            switch (lower)
            {
                case "1": return sentiment + PasswordInfo();
                case "2": return sentiment + PhishingInfo();
                case "3": return sentiment + SafeBrowsing();
                case "4": return sentiment + MalwareInfo();
                case "5": return sentiment + TwoFactorAuth();
                case "6": return sentiment + SocialEngineering();
                case "7": return sentiment + PublicWifi();
                case "8": return GetMenu();
            }

            // Memory: user states a favourite topic (Requirement 5)
            if (lower.Contains("i am interested in") || lower.Contains("i'm interested in") ||
                lower.Contains("i like") || lower.Contains("my favourite topic is") ||
                lower.Contains("i want to learn about"))
                return HandleFavouriteTopic(lower, sentiment);

            // Keyword recognition (Requirement 2)
            if (lower.Contains("password") || lower.Contains("passwords"))
                return sentiment + PasswordInfo();
            if (lower.Contains("phish"))
                return sentiment + PhishingInfo();
            if (lower.Contains("browsing") || lower.Contains("browser") ||
                lower.Contains("https") || lower.Contains("website"))
                return sentiment + SafeBrowsing();
            if (lower.Contains("malware") || lower.Contains("virus") || lower.Contains("ransomware"))
                return sentiment + MalwareInfo();
            if (lower.Contains("2fa") || lower.Contains("two factor") ||
                lower.Contains("two-factor") || lower.Contains("authenticat"))
                return sentiment + TwoFactorAuth();
            if (lower.Contains("social engineer") || lower.Contains("bait") || lower.Contains("pretend"))
                return sentiment + SocialEngineering();
            if (lower.Contains("wifi") || lower.Contains("wi-fi") ||
                lower.Contains("hotspot") || lower.Contains("public network"))
                return sentiment + PublicWifi();
            if (lower.Contains("privacy") || lower.Contains("private"))
                return sentiment + PrivacyInfo();
            if (lower.Contains("scam") || lower.Contains("fraud"))
                return sentiment + ScamInfo();

            // Conversational replies
            if (lower.Contains("how are you"))
                return $"I am doing great and ready to help you, {_userName}! {RecallFavourite()}Type 8 to see the menu.";
            if (lower.Contains("purpose") || lower.Contains("what do you do") ||
                lower.Contains("what can you do"))
                return "I am here to educate you on cybersecurity topics. Type 8 to see the full menu.";
            if (lower.Contains("thank"))
                return $"You are welcome, {_userName}! Stay cyber safe! {RecallFavourite()}";
            if (lower.Contains("hello") || lower.Contains("hi") || lower.Contains("hey"))
                return $"Hey {_userName}! {RecallFavourite()}Type 8 to see what I can help with.";
            if (lower.Contains("menu") || lower.Contains("help") || lower.Contains("topics"))
                return GetMenu();

            // Error: unrecognised input (Requirement 7)
            return $"[ERROR] I did not understand that, {_userName}. " +
                   "Try: password, phishing, malware, scam, privacy, wifi, 2fa — or type 8 for the menu.";
        }

        // ── Topic methods ──────────────────────────────────────────────────

        private string PasswordInfo()
        {
            _lastTopic = "password";
            string tip = _passwordResponses[_random.Next(_passwordResponses.Count)];
            return "// PASSWORD SAFETY\n" +
                   tip + "\n\n" +
                   "- Use at least 12 characters with letters, numbers and symbols.\n" +
                   "- Never reuse the same password on different accounts.\n" +
                   "- Use a password manager to keep your passwords safe.\n\n" +
                   "[ type 'more' for extra tips ]";
        }

        private string PhishingInfo()
        {
            _lastTopic = "phishing";
            string tip = _phishingResponses[_random.Next(_phishingResponses.Count)];
            return "// PHISHING\n" +
                   tip + "\n\n" +
                   "- Always check the sender email before clicking any links.\n" +
                   "- When in doubt, go directly to the website yourself.\n\n" +
                   "[ type 'more' for extra tips ]";
        }

        private string SafeBrowsing()
        {
            _lastTopic = "browsing";
            return "// SAFE BROWSING\n" +
                   "- Only visit websites that use HTTPS.\n" +
                   "- Never download files from unknown websites.\n" +
                   "- Keep your browser updated at all times.\n\n" +
                   "[ type 'more' for extra tips ]";
        }

        private string MalwareInfo()
        {
            _lastTopic = "malware";
            return "// MALWARE\n" +
                   "- Malware is harmful software that damages your device.\n" +
                   "- Install a good antivirus and keep it updated.\n" +
                   "- Never open email attachments from unknown senders.\n\n" +
                   "[ type 'more' for extra tips ]";
        }

        private string TwoFactorAuth()
        {
            _lastTopic = "2fa";
            return "// TWO-FACTOR AUTHENTICATION (2FA)\n" +
                   "- 2FA adds a second layer of protection beyond your password.\n" +
                   "- Enable it on your email, banking and social media accounts.\n" +
                   "- Never share your 2FA code with anyone.\n\n" +
                   "[ type 'more' for extra tips ]";
        }

        private string SocialEngineering()
        {
            _lastTopic = "social";
            return "// SOCIAL ENGINEERING\n" +
                   "- Attackers pretend to be trusted people to steal your info.\n" +
                   "- Always verify who you are talking to before sharing anything.\n" +
                   "- Never give passwords over the phone or via email.\n\n" +
                   "[ type 'more' for extra tips ]";
        }

        private string PublicWifi()
        {
            _lastTopic = "wifi";
            return "// PUBLIC WI-FI SAFETY\n" +
                   "- Public Wi-Fi is risky as others can intercept your data.\n" +
                   "- Avoid accessing banking or email on public networks.\n" +
                   "- Use a VPN to keep your connection private.\n\n" +
                   "[ type 'more' for extra tips ]";
        }

        private string PrivacyInfo()
        {
            _lastTopic = "privacy";
            string tip = _privacyResponses[_random.Next(_privacyResponses.Count)];
            return "// PRIVACY\n" + tip + "\n\n[ type 'more' for extra tips ]";
        }

        private string ScamInfo()
        {
            _lastTopic = "scam";
            string tip = _scamResponses[_random.Next(_scamResponses.Count)];
            return "// SCAM AWARENESS\n" + tip + "\n\n[ type 'more' for extra tips ]";
        }

        // ── More details (Requirement 4) ───────────────────────────────────
        private string GiveMoreDetails()
        {
            switch (_lastTopic)
            {
                case "password":
                    return "// MORE ON PASSWORDS\n" +
                           "- Use a passphrase: four random words joined together.\n" +
                           "- Check haveibeenpwned.com to see if your password was leaked.\n" +
                           "- A password manager generates and stores strong passwords for you.";
                case "phishing":
                    return "// MORE ON PHISHING\n" +
                           "- Vishing is phishing done over phone calls.\n" +
                           "- Smishing is phishing done through SMS messages.\n" +
                           "- Always check the full email address, not just the display name.";
                case "browsing":
                    return "// MORE ON SAFE BROWSING\n" +
                           "- Use a privacy browser like Firefox or Brave.\n" +
                           "- Use VirusTotal to check if a website is safe.\n" +
                           "- Extensions like uBlock Origin block malicious ads.";
                case "malware":
                    return "// MORE ON MALWARE\n" +
                           "- Ransomware locks your files and demands payment to unlock them.\n" +
                           "- Always back up your files to an external drive or cloud.\n" +
                           "- Avoid pirated software — it is a common malware delivery method.";
                case "2fa":
                    return "// MORE ON 2FA\n" +
                           "- Authenticator apps are safer than SMS-based 2FA codes.\n" +
                           "- Store your backup codes somewhere safe and offline.\n" +
                           "- Hardware keys like YubiKey are the strongest form of 2FA.";
                case "social":
                    return "// MORE ON SOCIAL ENGINEERING\n" +
                           "- Baiting leaves infected USB drives in public places.\n" +
                           "- Always challenge unknown visitors in your workplace.\n" +
                           "- Tailgating is when attackers follow staff through secure doors.";
                case "wifi":
                    return "// MORE ON PUBLIC WI-FI\n" +
                           "- Fake hotspots can steal your data without you knowing.\n" +
                           "- Use mobile data instead of public Wi-Fi when possible.\n" +
                           "- Disable auto-connect to Wi-Fi networks in your settings.";
                case "privacy":
                    return "// MORE ON PRIVACY\n" +
                           "- Use private/incognito browsing for sensitive searches.\n" +
                           "- Delete old accounts you no longer use.\n" +
                           "- Use a separate email address for online sign-ups.";
                case "scam":
                    return "// MORE ON SCAMS\n" +
                           "- Romance scams are rising — never send money to someone you met online.\n" +
                           "- Employment scams promise high pay for little work. Verify the company.\n" +
                           "- Hang up and call the organisation back on their official number.";
                default:
                    return "[ERROR] Please select a topic first, then type 'more' for extra details.";
            }
        }

        // ── Memory (Requirement 5) ─────────────────────────────────────────
        private string HandleFavouriteTopic(string lower, string sentiment)
        {
            string topic = "";
            if (lower.Contains("password")) topic = "password safety";
            else if (lower.Contains("phish")) topic = "phishing";
            else if (lower.Contains("privacy")) topic = "privacy";
            else if (lower.Contains("malware") || lower.Contains("virus")) topic = "malware";
            else if (lower.Contains("scam")) topic = "scam awareness";
            else if (lower.Contains("wifi") || lower.Contains("wi-fi")) topic = "public Wi-Fi safety";
            else if (lower.Contains("2fa") || lower.Contains("two factor")) topic = "two-factor authentication";
            else if (lower.Contains("browsing") || lower.Contains("browser")) topic = "safe browsing";
            else if (lower.Contains("social")) topic = "social engineering";

            if (!string.IsNullOrEmpty(topic))
            {
                _favouriteTopic = topic;
                return sentiment +
                       $"Noted! I will remember that you are interested in {topic}, {_userName}.\n" +
                       $"It is a crucial part of staying safe online.\n\n" +
                       GetTopicByName(topic);
            }

            return sentiment +
                   $"Great mindset, {_userName}! Type 8 to see all the topics I can help with.";
        }

        private string GetTopicByName(string topic)
        {
            if (topic.Contains("password")) return PasswordInfo();
            if (topic.Contains("phish")) return PhishingInfo();
            if (topic.Contains("privacy")) return PrivacyInfo();
            if (topic.Contains("malware")) return MalwareInfo();
            if (topic.Contains("scam")) return ScamInfo();
            if (topic.Contains("wi-fi")) return PublicWifi();
            if (topic.Contains("two-factor")) return TwoFactorAuth();
            if (topic.Contains("browsing")) return SafeBrowsing();
            if (topic.Contains("social")) return SocialEngineering();
            return "";
        }

        private string RecallFavourite()
        {
            if (!string.IsNullOrEmpty(_favouriteTopic))
                return $"As someone interested in {_favouriteTopic}, you might want to review your settings. ";
            return "";
        }

        private string DetectSentiment(string lower)
        {
            foreach (var kvp in _sentimentPrefixes)
                if (lower.Contains(kvp.Key)) return kvp.Value;
            return "";
        }

        private string GetMenu()
        {
            return "CYBERGUARD MENU\n" +
                   "────────────────────────────────\n" +
                   "  1.  Password Safety\n" +
                   "  2.  Phishing Awareness\n" +
                   "  3.  Safe Browsing\n" +
                   "  4.  Malware Protection\n" +
                   "  5.  Two-Factor Authentication\n" +
                   "  6.  Social Engineering\n" +
                   "  7.  Public Wi-Fi Safety\n" +
                   "  8.  Show menu again\n" +
                   "────────────────────────────────\n" +
                   "Type a number or keyword (e.g. password, scam, wifi).\n" +
                   "Type 'more' for extra details on the last topic.\n" +
                   "Type 'exit' to quit.";
        }
    }
}