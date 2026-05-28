using System;
using System.Collections.Generic;

namespace CyberGuard
{
    /// <summary>
    /// Core chatbot engine for CyberGuard v2.0.
    /// Handles keyword recognition, random responses, conversation flow,
    /// memory/recall, sentiment detection, and error handling.
    /// </summary>
    internal class Chatbot
    {
        // ── Memory fields (Requirement 5) ──────────────────────────────────
        private string _userName = "";   // Stores user name for personalisation
        private string _lastTopic = "";   // Tracks last discussed topic for follow-up
        private string _favouriteTopic = "";   // Stores user's stated favourite topic
        private string _previousTopic = "";   // Tracks topic before current for smart recall
        private int _messageCount = 0;    // Counts messages to trigger periodic recall
        private readonly Random _random = new Random();

        // ── Random response pools (Requirement 3) ──────────────────────────
        // Using List<string> to store multiple responses per topic.
        // _random.Next() selects one at random each time, keeping
        // interactions varied and engaging.

        private readonly List<string> _phishingResponses = new List<string>
        {
            "Be cautious of emails asking for personal information — scammers often disguise themselves as trusted organisations.",
            "Always verify the sender's full email address before clicking any link. Hover to preview the real URL first.",
            "Legitimate banks and companies never ask for your password via email. Contact them directly if unsure.",
            "Watch for urgent language like 'Act now!' or 'Your account will be closed!' — these are classic phishing tactics."
        };

        private readonly List<string> _passwordResponses = new List<string>
        {
            "Use strong, unique passwords for every account. Avoid personal details like your name or birthday.",
            "A good password is at least 12 characters and mixes uppercase, lowercase, numbers, and symbols.",
            "Never reuse the same password across multiple sites — if one is breached, all your accounts are at risk.",
            "A passphrase — four random words joined together — is long, strong, and surprisingly easy to remember."
        };

        private readonly List<string> _privacyResponses = new List<string>
        {
            "Review your social media privacy settings regularly — control exactly who can see your information.",
            "Avoid sharing sensitive details like your ID number, home address, or phone number in public forums.",
            "A VPN encrypts your internet traffic and keeps your browsing private, especially on public networks.",
            "Check app permissions carefully — many apps request far more access than they actually need."
        };

        private readonly List<string> _scamResponses = new List<string>
        {
            "If an offer sounds too good to be true, it almost always is. Verify before you act.",
            "Scammers create urgency to stop you thinking clearly. Always slow down and verify the source.",
            "Never share banking details or a one-time PIN with anyone who contacts you unexpectedly.",
            "Report suspected scams to your bank and local cybercrime authorities immediately."
        };

        private readonly List<string> _malwareResponses = new List<string>
        {
            "Malware is malicious software designed to damage, disrupt, or gain unauthorised access to your device.",
            "Install reputable antivirus software and keep it updated — outdated protection leaves you exposed.",
            "Never open email attachments from unknown senders, even if the file name looks harmless.",
            "Pirated software is one of the most common ways malware is distributed — always use legitimate sources."
        };

        private readonly List<string> _wifiResponses = new List<string>
        {
            "Public Wi-Fi is unencrypted — anyone on the same network can potentially intercept your data.",
            "Avoid logging into banking, email, or sensitive accounts while connected to public Wi-Fi.",
            "A VPN creates an encrypted tunnel for your data, making public Wi-Fi much safer to use.",
            "Fake Wi-Fi hotspots mimic legitimate networks to steal your data — always verify the network name."
        };

        private readonly List<string> _twoFaResponses = new List<string>
        {
            "Two-factor authentication adds a second verification step beyond your password — making accounts far harder to breach.",
            "Enable 2FA on every important account: email, banking, and social media at minimum.",
            "Authenticator apps like Google Authenticator are safer than SMS codes for 2FA.",
            "Never share your 2FA code with anyone — legitimate services will never ask for it."
        };

        private readonly List<string> _browsingResponses = new List<string>
        {
            "Only visit websites using HTTPS — the padlock icon in your browser confirms the connection is encrypted.",
            "Never download files or software from websites you do not fully trust.",
            "Keep your browser and all extensions updated — outdated browsers are a common attack vector.",
            "Use VirusTotal to scan suspicious links or files before opening them."
        };

        private readonly List<string> _socialEngResponses = new List<string>
        {
            "Social engineering manipulates people rather than technology — attackers exploit trust to steal information.",
            "Always verify the identity of anyone requesting sensitive information, even if they seem authoritative.",
            "Never give passwords or access codes over the phone or via email — no legitimate IT team needs them.",
            "Baiting attacks leave infected USB drives in public places hoping someone will plug them in."
        };

        // ── Sentiment detection map (Requirement 6) ───────────────────────
        // Dictionary maps emotion keywords to empathetic opening responses.
        // Detected sentiment is prepended to the topic response automatically,
        // then a relevant tip follows without requiring further user input.

        private readonly Dictionary<string, string> _sentimentResponses =
            new Dictionary<string, string>
        {
            { "worried",
              "It is completely understandable to feel worried — cyber threats are real and growing.\n" +
              "You are already doing the right thing by seeking information. Here is something helpful:\n\n" },
            { "scared",
              "There is no need to be scared — knowledge is your strongest defence online.\n" +
              "Let me share something that will help put your mind at ease:\n\n" },
            { "anxious",
              "I understand the anxiety. Cybersecurity can feel overwhelming at first.\n" +
              "Take it one step at a time — here is a great place to start:\n\n" },
            { "confused",
              "No worries at all — this stuff can be confusing. Let me break it down clearly for you:\n\n" },
            { "frustrated",
              "I hear you — dealing with cyber threats can be exhausting and frustrating.\n" +
              "Let me make this as simple as possible for you:\n\n" },
            { "curious",
              "I love the curiosity! Wanting to learn is the single best thing you can do for your security.\n" +
              "Here is something interesting to get you started:\n\n" },
            { "unsure",
              "Being unsure is perfectly fine — that is exactly why CyberGuard exists.\n" +
              "Let me guide you through this:\n\n" },
            { "overwhelmed",
              "Take a breath — you do not need to learn everything at once.\n" +
              "Let us start with one simple, important topic:\n\n" },
            { "angry",
              "I understand the frustration — being targeted by cyber criminals is infuriating.\n" +
              "Let me help you take back control:\n\n" },
            { "nervous",
              "It is okay to feel nervous about online safety. Many people do.\n" +
              "Here is one practical thing you can do right now:\n\n" }
        };

        public Chatbot(string userName)
        {
            _userName = userName;
        }

        // ── Welcome message ────────────────────────────────────────────────
        public string GetWelcome()
        {
            return $"Hello {_userName}! Welcome to CyberGuard.\n" +
                   "I am your cybersecurity awareness assistant.\n\n" +
                   GetMenu();
        }

        // ── Main response engine ───────────────────────────────────────────
        public string Respond(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "[ERROR] Please type something before sending.";

            string lower = input.Trim().ToLower();
            _messageCount++;

            // Exit
            if (lower == "exit" || lower == "bye")
                return $"Goodbye {_userName}! Stay safe online.\n" +
                       "Remember: strong passwords, 2FA, and staying alert go a long way.";

            // Sentiment detection (Requirement 6) — checked first
            string sentiment = DetectSentiment(lower);

            // Conversation flow: follow-up phrases (Requirement 4)
            if (lower.Contains("more") || lower.Contains("another tip") ||
                lower.Contains("tell me more") || lower.Contains("explain more") ||
                lower.Contains("give me more") || lower.Contains("elaborate") ||
                lower.Contains("expand"))
                return sentiment + GiveMoreDetails();

            // Menu shortcuts
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
                lower.Contains("i want to learn about") || lower.Contains("tell me about"))
                return HandleFavouriteTopic(lower, sentiment);

            // Keyword recognition (Requirement 2)
            if (lower.Contains("password") || lower.Contains("passwords"))
                return sentiment + PasswordInfo();
            if (lower.Contains("phish"))
                return sentiment + PhishingInfo();
            if (lower.Contains("browsing") || lower.Contains("browser") ||
                lower.Contains("https") || lower.Contains("website") ||
                lower.Contains("safe online"))
                return sentiment + SafeBrowsing();
            if (lower.Contains("malware") || lower.Contains("virus") ||
                lower.Contains("ransomware") || lower.Contains("trojan") ||
                lower.Contains("spyware"))
                return sentiment + MalwareInfo();
            if (lower.Contains("2fa") || lower.Contains("two factor") ||
                lower.Contains("two-factor") || lower.Contains("authenticat") ||
                lower.Contains("verification"))
                return sentiment + TwoFactorAuth();
            if (lower.Contains("social engineer") || lower.Contains("bait") ||
                lower.Contains("pretend") || lower.Contains("manipulat") ||
                lower.Contains("tailgat"))
                return sentiment + SocialEngineering();
            if (lower.Contains("wifi") || lower.Contains("wi-fi") ||
                lower.Contains("hotspot") || lower.Contains("public network") ||
                lower.Contains("public wifi"))
                return sentiment + PublicWifi();
            if (lower.Contains("privacy") || lower.Contains("private") ||
                lower.Contains("personal data") || lower.Contains("data protection"))
                return sentiment + PrivacyInfo();
            if (lower.Contains("scam") || lower.Contains("fraud") ||
                lower.Contains("trick") || lower.Contains("con "))
                return sentiment + ScamInfo();

            // Conversational replies
            if (lower.Contains("how are you"))
                return $"I am doing great and fully focused on keeping you safe online, {_userName}!\n" +
                       RecallMemory() +
                       "Type 8 to see everything I can help with.";

            if (lower.Contains("purpose") || lower.Contains("what do you do") ||
                lower.Contains("what can you") || lower.Contains("who are you"))
                return "I am CyberGuard — a cybersecurity awareness assistant.\n" +
                       "I can educate you on passwords, phishing, malware, scams, privacy, " +
                       "Wi-Fi safety, 2FA, and social engineering.\n\n" +
                       RecallMemory() +
                       "Type 8 to see the full menu.";

            if (lower.Contains("thank"))
                return $"You are very welcome, {_userName}! Staying informed is the best " +
                       $"defence against cyber threats.\n{RecallMemory()}";

            if (lower.Contains("hello") || lower.Contains("hi") ||
                lower.Contains("hey") || lower.Contains("good morning") ||
                lower.Contains("good afternoon"))
                return $"Hey {_userName}! Great to have you back.\n" +
                       RecallMemory() +
                       "Type 8 to see what I can help with today.";

            if (lower.Contains("menu") || lower.Contains("help") ||
                lower.Contains("topics") || lower.Contains("options"))
                return GetMenu();

            // Periodic memory recall every 5 messages (Requirement 5)
            if (_messageCount % 5 == 0 && !string.IsNullOrEmpty(_favouriteTopic))
                return $"Just a reminder — as someone interested in {_favouriteTopic}, " +
                       $"you might want to type '{_favouriteTopic.Split(' ')[0]}' to get the latest tips.\n\n" +
                       "[ERROR] I did not quite catch that though. Try a keyword or type 8 for the menu.";

            // Error: unrecognised input (Requirement 7)
            return $"[ERROR] I did not understand that, {_userName}.\n" +
                   "Try keywords like: password, phishing, malware, scam, privacy, wifi, 2fa\n" +
                   "Or type 8 for the full menu.";
        }

        // ── Topic response methods ─────────────────────────────────────────
        // Each method picks a random tip from its response pool (Requirement 3),
        // sets _lastTopic and _previousTopic for memory/recall (Requirement 5),
        // and appends a 'more' prompt for conversation flow (Requirement 4).

        private string PasswordInfo()
        {
            _previousTopic = _lastTopic;
            _lastTopic = "password";
            string tip = _passwordResponses[_random.Next(_passwordResponses.Count)];
            return "// PASSWORD SAFETY\n" +
                   tip + "\n\n" +
                   "- Use at least 12 characters with letters, numbers and symbols.\n" +
                   "- Never reuse the same password on different accounts.\n" +
                   "- Use a password manager to store passwords securely.\n\n" +
                   RelatedTopicHint("password") +
                   "[ type 'more' for extra tips ]";
        }

        private string PhishingInfo()
        {
            _previousTopic = _lastTopic;
            _lastTopic = "phishing";
            string tip = _phishingResponses[_random.Next(_phishingResponses.Count)];
            return "// PHISHING\n" +
                   tip + "\n\n" +
                   "- Always check the sender's email before clicking any links.\n" +
                   "- When in doubt, go directly to the website yourself.\n" +
                   "- Report phishing emails to your IT department or email provider.\n\n" +
                   RelatedTopicHint("phishing") +
                   "[ type 'more' for extra tips ]";
        }

        private string SafeBrowsing()
        {
            _previousTopic = _lastTopic;
            _lastTopic = "browsing";
            string tip = _browsingResponses[_random.Next(_browsingResponses.Count)];
            return "// SAFE BROWSING\n" +
                   tip + "\n\n" +
                   "- Only visit websites that use HTTPS.\n" +
                   "- Never download files from unknown websites.\n" +
                   "- Keep your browser updated at all times.\n\n" +
                   RelatedTopicHint("browsing") +
                   "[ type 'more' for extra tips ]";
        }

        private string MalwareInfo()
        {
            _previousTopic = _lastTopic;
            _lastTopic = "malware";
            string tip = _malwareResponses[_random.Next(_malwareResponses.Count)];
            return "// MALWARE\n" +
                   tip + "\n\n" +
                   "- Install a reputable antivirus and keep it updated.\n" +
                   "- Never open email attachments from unknown senders.\n" +
                   "- Back up your files regularly to an external drive or cloud.\n\n" +
                   RelatedTopicHint("malware") +
                   "[ type 'more' for extra tips ]";
        }

        private string TwoFactorAuth()
        {
            _previousTopic = _lastTopic;
            _lastTopic = "2fa";
            string tip = _twoFaResponses[_random.Next(_twoFaResponses.Count)];
            return "// TWO-FACTOR AUTHENTICATION (2FA)\n" +
                   tip + "\n\n" +
                   "- Enable 2FA on email, banking and social media accounts.\n" +
                   "- Use an authenticator app rather than SMS where possible.\n" +
                   "- Never share your 2FA code with anyone.\n\n" +
                   RelatedTopicHint("2fa") +
                   "[ type 'more' for extra tips ]";
        }

        private string SocialEngineering()
        {
            _previousTopic = _lastTopic;
            _lastTopic = "social";
            string tip = _socialEngResponses[_random.Next(_socialEngResponses.Count)];
            return "// SOCIAL ENGINEERING\n" +
                   tip + "\n\n" +
                   "- Always verify who you are talking to before sharing anything.\n" +
                   "- Never give passwords over the phone or via email.\n" +
                   "- When in doubt, hang up and call back on an official number.\n\n" +
                   RelatedTopicHint("social") +
                   "[ type 'more' for extra tips ]";
        }

        private string PublicWifi()
        {
            _previousTopic = _lastTopic;
            _lastTopic = "wifi";
            string tip = _wifiResponses[_random.Next(_wifiResponses.Count)];
            return "// PUBLIC WI-FI SAFETY\n" +
                   tip + "\n\n" +
                   "- Avoid accessing banking or email on public networks.\n" +
                   "- Use a VPN to encrypt your connection.\n" +
                   "- Disable auto-connect to unknown Wi-Fi networks.\n\n" +
                   RelatedTopicHint("wifi") +
                   "[ type 'more' for extra tips ]";
        }

        private string PrivacyInfo()
        {
            _previousTopic = _lastTopic;
            _lastTopic = "privacy";
            string tip = _privacyResponses[_random.Next(_privacyResponses.Count)];
            return "// PRIVACY\n" +
                   tip + "\n\n" +
                   "- Review your social media privacy settings regularly.\n" +
                   "- Delete old accounts you no longer use.\n" +
                   "- Use a separate email for online sign-ups.\n\n" +
                   RelatedTopicHint("privacy") +
                   "[ type 'more' for extra tips ]";
        }

        private string ScamInfo()
        {
            _previousTopic = _lastTopic;
            _lastTopic = "scam";
            string tip = _scamResponses[_random.Next(_scamResponses.Count)];
            return "// SCAM AWARENESS\n" +
                   tip + "\n\n" +
                   "- Never send money to someone you have not verified.\n" +
                   "- Hang up and call back on the official number if unsure.\n" +
                   "- Report scams to your bank and cybercrime authorities.\n\n" +
                   RelatedTopicHint("scam") +
                   "[ type 'more' for extra tips ]";
        }

        // ── More details: conversation flow (Requirement 4) ───────────────
        private string GiveMoreDetails()
        {
            switch (_lastTopic)
            {
                case "password":
                    return "// MORE ON PASSWORDS\n" +
                           "- Use a passphrase: four random words joined together.\n" +
                           "- Check haveibeenpwned.com to see if your email was leaked.\n" +
                           "- A password manager like Bitwarden generates and stores strong passwords.\n" +
                           "- Change passwords immediately if you suspect a breach.\n\n" +
                           "[ type 'more' again for another tip ]";
                case "phishing":
                    return "// MORE ON PHISHING\n" +
                           "- Vishing is phishing conducted over phone calls.\n" +
                           "- Smishing is phishing conducted through SMS messages.\n" +
                           "- Spear phishing targets specific individuals using personal details.\n" +
                           "- Always verify by calling the organisation directly on their official number.\n\n" +
                           "[ type 'more' again for another tip ]";
                case "browsing":
                    return "// MORE ON SAFE BROWSING\n" +
                           "- Use a privacy-focused browser like Firefox or Brave.\n" +
                           "- Install uBlock Origin to block malicious ads and trackers.\n" +
                           "- Use VirusTotal.com to scan any suspicious link before clicking.\n" +
                           "- Clear cookies and cache regularly to reduce tracking.\n\n" +
                           "[ type 'more' again for another tip ]";
                case "malware":
                    return "// MORE ON MALWARE\n" +
                           "- Ransomware encrypts your files and demands payment — back up regularly.\n" +
                           "- Spyware runs silently in the background and monitors your activity.\n" +
                           "- Avoid pirated software — it is a primary delivery method for malware.\n" +
                           "- Scan USB drives before opening any files on them.\n\n" +
                           "[ type 'more' again for another tip ]";
                case "2fa":
                    return "// MORE ON 2FA\n" +
                           "- Google Authenticator and Microsoft Authenticator are both excellent apps.\n" +
                           "- Store your backup recovery codes somewhere safe and offline.\n" +
                           "- Hardware security keys like YubiKey provide the strongest 2FA available.\n" +
                           "- SMS-based 2FA is better than nothing but weaker than app-based.\n\n" +
                           "[ type 'more' again for another tip ]";
                case "social":
                    return "// MORE ON SOCIAL ENGINEERING\n" +
                           "- Baiting places infected USB drives in public hoping someone plugs one in.\n" +
                           "- Pretexting involves fabricating a scenario to extract information.\n" +
                           "- Tailgating means following someone through a secure door without authorisation.\n" +
                           "- Always challenge people you do not recognise in secure areas.\n\n" +
                           "[ type 'more' again for another tip ]";
                case "wifi":
                    return "// MORE ON PUBLIC WI-FI\n" +
                           "- Evil twin attacks create fake hotspots that mimic legitimate ones.\n" +
                           "- Use mobile data instead of public Wi-Fi for sensitive activities.\n" +
                           "- Turn off Wi-Fi on your device when not in use.\n" +
                           "- Never accept unexpected certificate warnings on public networks.\n\n" +
                           "[ type 'more' again for another tip ]";
                case "privacy":
                    return "// MORE ON PRIVACY\n" +
                           "- Use private/incognito browsing when researching sensitive topics.\n" +
                           "- Consider a data broker opt-out service to remove your info from databases.\n" +
                           "- Use a separate email address for newsletters and online sign-ups.\n" +
                           "- Audit which apps have access to your camera, microphone, and location.\n\n" +
                           "[ type 'more' again for another tip ]";
                case "scam":
                    return "// MORE ON SCAMS\n" +
                           "- Romance scams have cost South Africans millions — never send money online.\n" +
                           "- Job scams promise high pay for minimal work — always verify the company.\n" +
                           "- Investment scams often promise guaranteed returns — these do not exist.\n" +
                           "- Report to the South African Fraud Prevention Service at 0800 222 999.\n\n" +
                           "[ type 'more' again for another tip ]";
                default:
                    return "[ERROR] Please select a topic first, then type 'more' for extra details.\n" +
                           "Type 8 to see all available topics.";
            }
        }

        // ── Memory: favourite topic handling (Requirement 5) ───────────────
        private string HandleFavouriteTopic(string lower, string sentiment)
        {
            string topic = ExtractTopicFromInput(lower);

            if (!string.IsNullOrEmpty(topic))
            {
                _favouriteTopic = topic;
                return sentiment +
                       $"Noted, {_userName}! I will remember that you are interested in {topic}.\n" +
                       $"It is a crucial part of staying safe online. Here is what you need to know:\n\n" +
                       GetTopicByName(topic);
            }

            return sentiment +
                   $"Great mindset, {_userName}! Type 8 to see all the topics I can help with.";
        }

        // ── Extracts topic name from user input ────────────────────────────
        private string ExtractTopicFromInput(string lower)
        {
            if (lower.Contains("password")) return "password safety";
            if (lower.Contains("phish")) return "phishing";
            if (lower.Contains("privacy") || lower.Contains("private")) return "privacy";
            if (lower.Contains("malware") || lower.Contains("virus")) return "malware";
            if (lower.Contains("scam") || lower.Contains("fraud")) return "scam awareness";
            if (lower.Contains("wifi") || lower.Contains("wi-fi")) return "public Wi-Fi safety";
            if (lower.Contains("2fa") || lower.Contains("two factor")) return "two-factor authentication";
            if (lower.Contains("browsing") || lower.Contains("browser")) return "safe browsing";
            if (lower.Contains("social")) return "social engineering";
            return "";
        }

        // ── Gets topic response by stored name ─────────────────────────────
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

        // ── Smart memory recall string (Requirement 5) ────────────────────
        // Builds a personalised recall sentence using stored memory fields.
        // Referenced naturally in conversational replies to enhance engagement.
        private string RecallMemory()
        {
            List<string> recalls = new List<string>();

            if (!string.IsNullOrEmpty(_favouriteTopic))
                recalls.Add($"As someone interested in {_favouriteTopic}, " +
                            $"remember to keep reviewing your settings regularly.");

            if (!string.IsNullOrEmpty(_lastTopic) && _lastTopic != _previousTopic)
                recalls.Add($"Last time we spoke about {_lastTopic} — " +
                            $"type 'more' if you want extra tips on that.");

            if (!string.IsNullOrEmpty(_previousTopic))
                recalls.Add($"You also explored {_previousTopic} earlier — " +
                            $"great topics to revisit as your knowledge grows.");

            if (recalls.Count == 0) return "";

            string result = "";
            foreach (string r in recalls)
                result += r + "\n";
            return result + "\n";
        }

        // ── Related topic hint (enhances conversation flow) ────────────────
        private string RelatedTopicHint(string current)
        {
            // Suggests a related topic to keep the conversation flowing naturally
            Dictionary<string, string> related = new Dictionary<string, string>
            {
                { "password",  "Related: type 'phishing' to learn how passwords get stolen.\n" },
                { "phishing",  "Related: type 'scam' to learn about broader fraud techniques.\n" },
                { "browsing",  "Related: type 'privacy' to learn how to protect your online data.\n" },
                { "malware",   "Related: type 'browsing' to learn how to avoid malicious sites.\n" },
                { "2fa",       "Related: type 'password' to strengthen your first line of defence.\n" },
                { "social",    "Related: type 'phishing' to see how social engineering is used in email attacks.\n" },
                { "wifi",      "Related: type '2fa' to add protection even when on unsafe networks.\n" },
                { "privacy",   "Related: type 'wifi' to learn how public networks expose your data.\n" },
                { "scam",      "Related: type 'social' to understand the manipulation tactics behind scams.\n" }
            };

            if (related.ContainsKey(current))
                return related[current] + "\n";
            return "";
        }

        // ── Sentiment detection (Requirement 6) ───────────────────────────
        private string DetectSentiment(string lower)
        {
            foreach (var kvp in _sentimentResponses)
                if (lower.Contains(kvp.Key)) return kvp.Value;
            return "";
        }

        // ── Topic menu ─────────────────────────────────────────────────────
        private string GetMenu()
        {
            return "CYBERGUARD MENU\n" +
                   "────────────────────────────────────────\n" +
                   "  1.  Password Safety\n" +
                   "  2.  Phishing Awareness\n" +
                   "  3.  Safe Browsing\n" +
                   "  4.  Malware Protection\n" +
                   "  5.  Two-Factor Authentication (2FA)\n" +
                   "  6.  Social Engineering\n" +
                   "  7.  Public Wi-Fi Safety\n" +
                   "  8.  Show this menu again\n" +
                   "────────────────────────────────────────\n" +
                   "Type a number or keyword (password, scam, wifi...).\n" +
                   "Type 'more' for extra details on the last topic.\n" +
                   "Type 'I'm interested in [topic]' to save your favourite.\n" +
                   "Type 'exit' to quit.";
        }
    }
}