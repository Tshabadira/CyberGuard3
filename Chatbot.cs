using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberGuard
{
    internal class Chatbot
    {
        // ── Memory fields ──────────────────────────────────
        private string _userName = "";
        private string _lastTopic = "";
        private string _favouriteTopic = "";
        private List<string> _topicsDiscussed = new List<string>();
        private int _messageCount = 0;
        private readonly Random _random = new Random();

        // ── New components ────────────────────────────────
        private readonly TaskManager _taskManager;
        private readonly QuizEngine _quizEngine;
        private bool _quizActive => _quizEngine.IsActive;

        // ── Response pools (expanded) ─────────────────────
        private readonly List<string> _phishingResponses = new List<string>
        {
            "Be cautious of emails asking for personal information — scammers often disguise themselves as trusted organisations.",
            "Always verify the sender's full email address before clicking any link. Hover to preview the real URL first.",
            "Legitimate banks and companies never ask for your password via email. Contact them directly if unsure.",
            "Watch for urgent language like 'Act now!' or 'Your account will be closed!' — these are classic phishing tactics.",
            "Phishing emails often contain spelling errors and generic greetings like 'Dear Customer'.",
            "If you receive a suspicious email, forward it to the Anti-Phishing Working Group at reportphishing@apwg.org.",
            "Never click on links in unsolicited emails — type the URL directly into your browser instead.",
            "Phishing attempts are designed to create panic. Always take a moment to verify before you act."
        };

        private readonly List<string> _passwordResponses = new List<string>
        {
            "Use strong, unique passwords for every account. Avoid personal details like your name or birthday.",
            "A good password is at least 12 characters and mixes uppercase, lowercase, numbers, and symbols.",
            "Never reuse the same password across multiple sites — if one is breached, all your accounts are at risk.",
            "A passphrase — four random words joined together — is long, strong, and surprisingly easy to remember.",
            "Use a password manager to generate and store complex passwords securely.",
            "Change your passwords immediately if you suspect any account has been compromised.",
            "Enable 2FA whenever possible — it adds a critical second layer of protection.",
            "Check haveibeenpwned.com to see if your email or passwords have been exposed in a breach."
        };

        private readonly List<string> _privacyResponses = new List<string>
        {
            "Review your social media privacy settings regularly — control exactly who can see your information.",
            "Avoid sharing sensitive details like your ID number, home address, or phone number in public forums.",
            "A VPN encrypts your internet traffic and keeps your browsing private, especially on public networks.",
            "Check app permissions carefully — many apps request far more access than they actually need.",
            "Use a separate email address for newsletters and online sign-ups to protect your primary inbox.",
            "Delete old online accounts that you no longer use to reduce your digital footprint.",
            "Be cautious about what you post online — once it's out there, it's nearly impossible to remove.",
            "Consider using a data broker opt-out service to remove your personal information from public databases."
        };

        private readonly List<string> _scamResponses = new List<string>
        {
            "If an offer sounds too good to be true, it almost always is. Verify before you act.",
            "Scammers create urgency to stop you thinking clearly. Always slow down and verify the source.",
            "Never share banking details or a one-time PIN with anyone who contacts you unexpectedly.",
            "Report suspected scams to your bank and local cybercrime authorities immediately.",
            "Be wary of unexpected phone calls claiming to be from your bank — hang up and call them back.",
            "Job scams promise high pay for minimal work — always verify the company's legitimacy.",
            "Investment scams often promise guaranteed returns — these do not exist in the real world.",
            "Romance scams have cost South Africans millions — never send money to someone you haven't met."
        };

        private readonly List<string> _malwareResponses = new List<string>
        {
            "Malware is malicious software designed to damage, disrupt, or gain unauthorised access to your device.",
            "Install reputable antivirus software and keep it updated — outdated protection leaves you exposed.",
            "Never open email attachments from unknown senders, even if the file name looks harmless.",
            "Pirated software is one of the most common ways malware is distributed — always use legitimate sources.",
            "Ransomware encrypts your files and demands payment — back up your important files regularly.",
            "Spyware runs silently in the background and monitors your activity — scan your system regularly.",
            "Keep your operating system and all applications updated to patch security vulnerabilities.",
            "Be careful when plugging in USB drives from unknown sources — they could contain malware."
        };

        private readonly List<string> _wifiResponses = new List<string>
        {
            "Public Wi-Fi is unencrypted — anyone on the same network can potentially intercept your data.",
            "Avoid logging into banking, email, or sensitive accounts while connected to public Wi-Fi.",
            "A VPN creates an encrypted tunnel for your data, making public Wi-Fi much safer to use.",
            "Fake Wi-Fi hotspots mimic legitimate networks to steal your data — always verify the network name.",
            "Turn off Wi-Fi on your device when not in use to prevent automatic connections.",
            "Use mobile data instead of public Wi-Fi for sensitive activities whenever possible.",
            "Never accept unexpected certificate warnings on public networks — they could indicate an attack.",
            "Disable auto-connect to known networks that you may have used in the past."
        };

        private readonly List<string> _twoFaResponses = new List<string>
        {
            "Two-factor authentication adds a second verification step beyond your password — making accounts far harder to breach.",
            "Enable 2FA on every important account: email, banking, and social media at minimum.",
            "Authenticator apps like Google Authenticator are safer than SMS codes for 2FA.",
            "Never share your 2FA code with anyone — legitimate services will never ask for it.",
            "Hardware security keys like YubiKey provide the strongest 2FA protection available.",
            "Store your backup recovery codes somewhere safe and offline in case you lose access.",
            "SMS-based 2FA is better than nothing but weaker than app-based authenticators.",
            "Always verify that the 2FA request is from a legitimate source before entering the code."
        };

        private readonly List<string> _browsingResponses = new List<string>
        {
            "Only visit websites using HTTPS — the padlock icon in your browser confirms the connection is encrypted.",
            "Never download files or software from websites you do not fully trust.",
            "Keep your browser and all extensions updated — outdated browsers are a common attack vector.",
            "Use VirusTotal to scan suspicious links or files before opening them.",
            "Use a privacy-focused browser like Firefox or Brave with built-in tracking protection.",
            "Install uBlock Origin to block malicious ads and trackers from loading.",
            "Clear your browser cookies and cache regularly to reduce tracking and stored data.",
            "Consider using a separate browser for sensitive activities like banking and email."
        };

        private readonly List<string> _socialEngResponses = new List<string>
        {
            "Social engineering manipulates people rather than technology — attackers exploit trust to steal information.",
            "Always verify the identity of anyone requesting sensitive information, even if they seem authoritative.",
            "Never give passwords or access codes over the phone or via email — no legitimate IT team needs them.",
            "Baiting attacks leave infected USB drives in public places hoping someone will plug them in.",
            "Pretexting involves fabricating a scenario to extract information from you.",
            "Tailgating means following someone through a secure door without authorisation — always hold the door for strangers.",
            "Be sceptical of unsolicited calls, messages, or visits from people claiming to be from IT support.",
            "Phishing is the most common form of social engineering — always verify before you share."
        };

        // ── Greetings, farewells, thanks ──────────────────
        private readonly List<string> _greetings = new List<string>
        {
            "Hey {0}! Great to have you back.",
            "Hello {0}! How can I assist you today?",
            "Hi {0}! Ready to boost your cybersecurity?",
            "Good to see you, {0}! Let's stay safe online.",
            "Welcome back, {0}! I've got some new tips for you."
        };

        private readonly List<string> _farewells = new List<string>
        {
            "Goodbye {0}! Stay safe online.",
            "See you later, {0}! Remember to update your passwords.",
            "Take care, {0}! Always verify before you trust.",
            "Until next time, {0}! Keep learning.",
            "Stay secure, {0}! Come back anytime for more tips."
        };

        private readonly List<string> _thanks = new List<string>
        {
            "You're welcome, {0}!",
            "Anytime, {0}!",
            "Glad I could help, {0}!",
            "My pleasure, {0}!",
            "Always happy to help, {0}!"
        };

        private readonly List<string> _errorResponses = new List<string>
        {
            "I didn't quite catch that, {0}. Could you rephrase?",
            "Hmm, I'm not sure what you mean, {0}. Try a keyword from the menu.",
            "Sorry, I didn't understand that. Type 8 to see the menu.",
            "I'm still learning! Can you say that differently, {0}?",
            "I didn't recognise that command, {0}. Try a keyword or check the menu."
        };

        // ── Sentiment map ──────────────────────────────────
        private readonly Dictionary<string, string> _sentimentResponses = new Dictionary<string, string>
        {
            { "worried", "It is completely understandable to feel worried — cyber threats are real and growing.\nYou are already doing the right thing by seeking information. Here is something helpful:\n\n" },
            { "scared", "There is no need to be scared — knowledge is your strongest defence online.\nLet me share something that will help put your mind at ease:\n\n" },
            { "anxious", "I understand the anxiety. Cybersecurity can feel overwhelming at first.\nTake it one step at a time — here is a great place to start:\n\n" },
            { "confused", "No worries at all — this stuff can be confusing. Let me break it down clearly for you:\n\n" },
            { "frustrated", "I hear you — dealing with cyber threats can be exhausting and frustrating.\nLet me make this as simple as possible for you:\n\n" },
            { "curious", "I love the curiosity! Wanting to learn is the single best thing you can do for your security.\nHere is something interesting to get you started:\n\n" },
            { "unsure", "Being unsure is perfectly fine — that is exactly why CyberGuard exists.\nLet me guide you through this:\n\n" },
            { "overwhelmed", "Take a breath — you do not need to learn everything at once.\nLet us start with one simple, important topic:\n\n" },
            { "angry", "I understand the frustration — being targeted by cyber criminals is infuriating.\nLet me help you take back control:\n\n" },
            { "nervous", "It is okay to feel nervous about online safety. Many people do.\nHere is one practical thing you can do right now:\n\n" }
        };

        // ── Intent keywords (NLP simulation) ──────────────
        private readonly Dictionary<string, List<string>> _intentKeywords = new Dictionary<string, List<string>>
        {
            { "add_task", new List<string> { "add task", "create task", "new task", "set task", "task add", "add a task" } },
            { "reminder", new List<string> { "remind me", "set reminder", "create reminder", "remind", "add reminder" } },
            { "show_tasks", new List<string> { "show tasks", "list tasks", "my tasks", "view tasks", "tasks" } },
            { "delete_task", new List<string> { "delete task", "remove task", "clear task" } },
            { "complete_task", new List<string> { "complete task", "mark done", "finish task" } },
            { "start_quiz", new List<string> { "start quiz", "begin quiz", "play quiz", "quiz", "take quiz" } },
            { "show_log", new List<string> { "show activity log", "activity log", "what have you done", "log", "history" } },
        };

        // ── Constructor ────────────────────────────────────
        public Chatbot(string userName)
        {
            _userName = userName;
            _taskManager = new TaskManager();
            _quizEngine = new QuizEngine();
            // Ensure DB table exists
            DatabaseHelper.EnsureTableCreated();
        }

        // ── Welcome ────────────────────────────────────────
        public string GetWelcome()
        {
            return $"Hello {_userName}! Welcome to CyberGuard.\n" +
                   "I am your cybersecurity awareness assistant.\n\n" +
                   GetMenu();
        }

        // ── Main response engine ──────────────────────────
        public string Respond(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return GetRandomError();

            string lower = input.Trim().ToLower();
            _messageCount++;

            // Exit
            if (lower == "exit" || lower == "bye")
            {
                ActivityLog.AddEntry($"User ended conversation.");
                return string.Format(_farewells[_random.Next(_farewells.Count)], _userName);
            }

            // Detect sentiment first
            string sentiment = DetectSentiment(lower);

            // ── Quiz handling (if active) ──────────────────
            if (_quizActive)
            {
                string quizResult = _quizEngine.SubmitAnswer(lower);
                if (!_quizEngine.IsActive) // quiz ended
                    ActivityLog.AddEntry($"Quiz completed with score {_quizEngine.Score}/{_quizEngine.TotalQuestions}");
                else
                    ActivityLog.AddEntry($"Quiz in progress – answered Q{_quizEngine.CurrentQuestionNumber}");
                return quizResult;
            }

            // ── Intent detection (NLP) ────────────────────
            string intent = DetectIntent(lower);
            switch (intent)
            {
                case "add_task":
                    return HandleAddTask(lower, sentiment);
                case "reminder":
                    return HandleAddTask(lower, sentiment);
                case "show_tasks":
                    return HandleShowTasks();
                case "delete_task":
                    return HandleDeleteTask(lower);
                case "complete_task":
                    return HandleCompleteTask(lower);
                case "start_quiz":
                    return HandleStartQuiz(sentiment);
                case "show_log":
                    return HandleShowLog();
                default:
                    break;
            }

            // ── Menu shortcuts ─────────────────────────────
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

            // ── Memory: favourite topic ────────────────────
            if (lower.Contains("i am interested in") || lower.Contains("i'm interested in") ||
                lower.Contains("i like") || lower.Contains("my favourite topic is") ||
                lower.Contains("i want to learn about") || lower.Contains("tell me about"))
                return HandleFavouriteTopic(lower, sentiment);

            // ── Keyword recognition ────────────────────────
            if (ContainsAny(lower, new[] { "password", "passphrase", "credentials", "login", "pwd" }))
                return sentiment + PasswordInfo();
            if (ContainsAny(lower, new[] { "phish", "phishing", "spoof", "fake email", "vishing", "smishing" }))
                return sentiment + PhishingInfo();
            if (ContainsAny(lower, new[] { "browsing", "browser", "https", "website", "safe online" }))
                return sentiment + SafeBrowsing();
            if (ContainsAny(lower, new[] { "malware", "virus", "ransomware", "trojan", "spyware", "worm" }))
                return sentiment + MalwareInfo();
            if (ContainsAny(lower, new[] { "2fa", "two factor", "two-factor", "authenticat", "verification", "mfa" }))
                return sentiment + TwoFactorAuth();
            if (ContainsAny(lower, new[] { "social engineer", "social engineering", "bait", "manipulat", "tailgat", "pretext" }))
                return sentiment + SocialEngineering();
            if (ContainsAny(lower, new[] { "wifi", "wi-fi", "hotspot", "public network", "public wifi" }))
                return sentiment + PublicWifi();
            if (ContainsAny(lower, new[] { "privacy", "private", "personal data", "data protection", "gdpr" }))
                return sentiment + PrivacyInfo();
            if (ContainsAny(lower, new[] { "scam", "fraud", "trick", "con" }))
                return sentiment + ScamInfo();

            // ── "More" requests ────────────────────────────
            if (ContainsAny(lower, new[] { "more", "tell me more", "another tip", "elaborate", "expand", "give me more" }))
                return sentiment + GiveMoreDetails();

            // ── Conversational replies ─────────────────────
            if (ContainsAny(lower, new[] { "how are you", "how do you do" }))
                return string.Format(_greetings[_random.Next(_greetings.Count)], _userName) +
                       "\n" + RecallMemory() +
                       "Type 8 to see everything I can help with.";

            if (ContainsAny(lower, new[] { "thank", "thanks", "appreciate" }))
                return string.Format(_thanks[_random.Next(_thanks.Count)], _userName) +
                       "\n" + RecallMemory();

            if (ContainsAny(lower, new[] { "hello", "hi", "hey", "good morning", "good afternoon" }))
                return string.Format(_greetings[_random.Next(_greetings.Count)], _userName) +
                       "\n" + RecallMemory() +
                       "Type 8 to see what I can help with today.";

            if (ContainsAny(lower, new[] { "menu", "help", "topics", "options" }))
                return GetMenu();

            if (ContainsAny(lower, new[] { "purpose", "what do you do", "what can you", "who are you" }))
                return "I am CyberGuard — a cybersecurity awareness assistant.\n" +
                       "I can educate you on passwords, phishing, malware, scams, privacy, " +
                       "Wi-Fi safety, 2FA, and social engineering.\n\n" +
                       RecallMemory() +
                       "Type 8 to see the full menu.";

            // ── Periodic memory recall ─────────────────────
            if (_messageCount % 5 == 0 && !string.IsNullOrEmpty(_favouriteTopic))
                return $"Just a reminder — as someone interested in {_favouriteTopic}, " +
                       $"you might want to type '{_favouriteTopic.Split(' ')[0]}' to get the latest tips.\n\n" +
                       GetRandomError();

            // ── Fallback: unknown input ────────────────────
            ActivityLog.AddEntry($"Unrecognised input: '{input}'");
            return GetRandomError();
        }

        // ── Helper method ──────────────────────────────────
        private bool ContainsAny(string input, string[] keywords) =>
            keywords.Any(k => input.Contains(k));

        // ── Intent detection ──────────────────────────────
        private string DetectIntent(string lower)
        {
            foreach (var kvp in _intentKeywords)
                if (kvp.Value.Any(keyword => lower.Contains(keyword)))
                    return kvp.Key;
            return "";
        }

        // ── Topic response methods ─────────────────────────
        private string PasswordInfo()
        {
            _lastTopic = "password";
            if (!_topicsDiscussed.Contains(_lastTopic)) _topicsDiscussed.Add(_lastTopic);
            string tip = _passwordResponses[_random.Next(_passwordResponses.Count)];
            ActivityLog.AddEntry($"Provided password safety tip.");
            return "// PASSWORD SAFETY\n" + tip + "\n\n" +
                   "- Use at least 12 characters with letters, numbers and symbols.\n" +
                   "- Never reuse the same password on different accounts.\n" +
                   "- Use a password manager to store passwords securely.\n\n" +
                   RelatedTopicHint("password") +
                   "[ type 'more' for extra tips ]";
        }

        private string PhishingInfo()
        {
            _lastTopic = "phishing";
            if (!_topicsDiscussed.Contains(_lastTopic)) _topicsDiscussed.Add(_lastTopic);
            string tip = _phishingResponses[_random.Next(_phishingResponses.Count)];
            ActivityLog.AddEntry($"Provided phishing awareness tip.");
            return "// PHISHING\n" + tip + "\n\n" +
                   "- Always check the sender's email before clicking any links.\n" +
                   "- When in doubt, go directly to the website yourself.\n" +
                   "- Report phishing emails to your IT department or email provider.\n\n" +
                   RelatedTopicHint("phishing") +
                   "[ type 'more' for extra tips ]";
        }

        private string SafeBrowsing()
        {
            _lastTopic = "browsing";
            if (!_topicsDiscussed.Contains(_lastTopic)) _topicsDiscussed.Add(_lastTopic);
            string tip = _browsingResponses[_random.Next(_browsingResponses.Count)];
            ActivityLog.AddEntry($"Provided safe browsing tip.");
            return "// SAFE BROWSING\n" + tip + "\n\n" +
                   "- Only visit websites that use HTTPS.\n" +
                   "- Never download files from unknown websites.\n" +
                   "- Keep your browser updated at all times.\n\n" +
                   RelatedTopicHint("browsing") +
                   "[ type 'more' for extra tips ]";
        }

        private string MalwareInfo()
        {
            _lastTopic = "malware";
            if (!_topicsDiscussed.Contains(_lastTopic)) _topicsDiscussed.Add(_lastTopic);
            string tip = _malwareResponses[_random.Next(_malwareResponses.Count)];
            ActivityLog.AddEntry($"Provided malware protection tip.");
            return "// MALWARE\n" + tip + "\n\n" +
                   "- Install a reputable antivirus and keep it updated.\n" +
                   "- Never open email attachments from unknown senders.\n" +
                   "- Back up your files regularly to an external drive or cloud.\n\n" +
                   RelatedTopicHint("malware") +
                   "[ type 'more' for extra tips ]";
        }

        private string TwoFactorAuth()
        {
            _lastTopic = "2fa";
            if (!_topicsDiscussed.Contains(_lastTopic)) _topicsDiscussed.Add(_lastTopic);
            string tip = _twoFaResponses[_random.Next(_twoFaResponses.Count)];
            ActivityLog.AddEntry($"Provided 2FA tip.");
            return "// TWO-FACTOR AUTHENTICATION (2FA)\n" + tip + "\n\n" +
                   "- Enable 2FA on email, banking and social media accounts.\n" +
                   "- Use an authenticator app rather than SMS where possible.\n" +
                   "- Never share your 2FA code with anyone.\n\n" +
                   RelatedTopicHint("2fa") +
                   "[ type 'more' for extra tips ]";
        }

        private string SocialEngineering()
        {
            _lastTopic = "social";
            if (!_topicsDiscussed.Contains(_lastTopic)) _topicsDiscussed.Add(_lastTopic);
            string tip = _socialEngResponses[_random.Next(_socialEngResponses.Count)];
            ActivityLog.AddEntry($"Provided social engineering tip.");
            return "// SOCIAL ENGINEERING\n" + tip + "\n\n" +
                   "- Always verify who you are talking to before sharing anything.\n" +
                   "- Never give passwords over the phone or via email.\n" +
                   "- When in doubt, hang up and call back on an official number.\n\n" +
                   RelatedTopicHint("social") +
                   "[ type 'more' for extra tips ]";
        }

        private string PublicWifi()
        {
            _lastTopic = "wifi";
            if (!_topicsDiscussed.Contains(_lastTopic)) _topicsDiscussed.Add(_lastTopic);
            string tip = _wifiResponses[_random.Next(_wifiResponses.Count)];
            ActivityLog.AddEntry($"Provided public Wi-Fi safety tip.");
            return "// PUBLIC WI-FI SAFETY\n" + tip + "\n\n" +
                   "- Avoid accessing banking or email on public networks.\n" +
                   "- Use a VPN to encrypt your connection.\n" +
                   "- Disable auto-connect to unknown Wi-Fi networks.\n\n" +
                   RelatedTopicHint("wifi") +
                   "[ type 'more' for extra tips ]";
        }

        private string PrivacyInfo()
        {
            _lastTopic = "privacy";
            if (!_topicsDiscussed.Contains(_lastTopic)) _topicsDiscussed.Add(_lastTopic);
            string tip = _privacyResponses[_random.Next(_privacyResponses.Count)];
            ActivityLog.AddEntry($"Provided privacy tip.");
            return "// PRIVACY\n" + tip + "\n\n" +
                   "- Review your social media privacy settings regularly.\n" +
                   "- Delete old accounts you no longer use.\n" +
                   "- Use a separate email for online sign-ups.\n\n" +
                   RelatedTopicHint("privacy") +
                   "[ type 'more' for extra tips ]";
        }

        private string ScamInfo()
        {
            _lastTopic = "scam";
            if (!_topicsDiscussed.Contains(_lastTopic)) _topicsDiscussed.Add(_lastTopic);
            string tip = _scamResponses[_random.Next(_scamResponses.Count)];
            ActivityLog.AddEntry($"Provided scam awareness tip.");
            return "// SCAM AWARENESS\n" + tip + "\n\n" +
                   "- Never send money to someone you have not verified.\n" +
                   "- Hang up and call back on the official number if unsure.\n" +
                   "- Report scams to your bank and cybercrime authorities.\n\n" +
                   RelatedTopicHint("scam") +
                   "[ type 'more' for extra tips ]";
        }

        // ── More details: conversation flow ───────────────
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

        // ── Memory: favourite topic handling ──────────────
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

        // ── Extracts topic name from user input ────────────
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

        // ── Gets topic response by stored name ─────────────
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

        // ── Smart memory recall string ─────────────────────
        private string RecallMemory()
        {
            string result = "";
            if (_topicsDiscussed.Count > 0)
                result += $"We've discussed: {string.Join(", ", _topicsDiscussed.Distinct())}.\n";
            if (!string.IsNullOrEmpty(_favouriteTopic))
                result += $"Since you're interested in {_favouriteTopic}, you might enjoy more tips on that.\n";
            if (_topicsDiscussed.Count >= 2)
                result += "You can type 'more' on any of those topics for extra details.\n";
            return result;
        }

        // ── Related topic hint ─────────────────────────────
        private string RelatedTopicHint(string current)
        {
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

        // ── Sentiment detection ────────────────────────────
        private string DetectSentiment(string input)
        {
            // Simple scoring
            int score = 0;
            string[] positiveWords = { "curious", "interested", "good", "great", "happy", "excited", "love", "enjoy" };
            string[] negativeWords = { "worried", "scared", "anxious", "frustrated", "angry", "nervous", "overwhelmed", "confused", "unsure" };

            foreach (var w in positiveWords)
                if (input.Contains(w)) score++;
            foreach (var w in negativeWords)
                if (input.Contains(w)) score--;

            if (score <= -2) return "Take a deep breath – you've got this. Let me help you in a simple way:\n\n";
            if (score <= -1) return "I understand that can be concerning. Here's something reassuring:\n\n";
            if (score >= 2) return "That's a great attitude! Let's dive right in:\n\n";
            return "";
        }

        // ── Task handlers ───────────────────────────────────
        private string HandleAddTask(string input, string sentiment)
        {
            string taskText = input;
            foreach (var phrase in _intentKeywords["add_task"].Concat(_intentKeywords["reminder"]))
            {
                if (input.Contains(phrase))
                {
                    taskText = input.Substring(input.IndexOf(phrase) + phrase.Length).Trim();
                    break;
                }
            }
            if (string.IsNullOrEmpty(taskText))
                return "What task would you like to add? Please describe it.";

            DateTime? reminder = null;
            if (ContainsAny(taskText, new[] { "tomorrow", "in", "on", "next" }))
            {
                reminder = DateTime.Now.AddDays(3);
                taskText = taskText.Replace("tomorrow", "").Replace("in", "").Replace("on", "").Replace("next", "").Trim();
            }

            int id = _taskManager.AddTask(taskText, "", reminder);
            ActivityLog.AddEntry($"Task added: '{taskText}' (ID {id})" + (reminder.HasValue ? $" with reminder on {reminder.Value.ToShortDateString()}" : ""));
            string response = $"Task added: '{taskText}'.";
            if (reminder.HasValue)
                response += $" I'll remind you on {reminder.Value.ToShortDateString()}.";
            else
                response += " Would you like to set a reminder? (type 'remind me' with a date)";
            return sentiment + response;
        }

        private string HandleShowTasks()
        {
            var tasks = _taskManager.GetTasks(false);
            if (tasks.Count == 0)
                return "You have no pending tasks. Great job!";
            string output = "Your pending cybersecurity tasks:\n";
            foreach (var t in tasks)
                output += $"  [{t.Id}] {t.Title}" + (t.ReminderDate.HasValue ? $" (reminder: {t.ReminderDate.Value.ToShortDateString()})" : "") + "\n";
            output += "Type 'complete task [id]' or 'delete task [id]' to manage them.";
            ActivityLog.AddEntry("Viewed task list.");
            return output;
        }

        private string HandleDeleteTask(string input)
        {
            int id = ExtractId(input);
            if (id == 0) return "Please specify a task ID to delete, e.g., 'delete task 3'.";
            bool success = _taskManager.DeleteTask(id);
            if (success)
            {
                ActivityLog.AddEntry($"Deleted task ID {id}");
                return $"Task {id} deleted successfully.";
            }
            else
                return $"Task {id} not found or could not be deleted.";
        }

        private string HandleCompleteTask(string input)
        {
            int id = ExtractId(input);
            if (id == 0) return "Please specify a task ID to mark complete, e.g., 'complete task 3'.";
            bool success = _taskManager.MarkComplete(id);
            if (success)
            {
                ActivityLog.AddEntry($"Completed task ID {id}");
                return $"Task {id} marked as completed!";
            }
            else
                return $"Task {id} not found or already completed.";
        }

        private int ExtractId(string input)
        {
            var words = input.Split(' ');
            foreach (var w in words)
                if (int.TryParse(w, out int id))
                    return id;
            return 0;
        }

        // ── Quiz handler ────────────────────────────────────
        private string HandleStartQuiz(string sentiment)
        {
            string startMsg = _quizEngine.Start();
            ActivityLog.AddEntry($"Started quiz.");
            return sentiment + startMsg;
        }

        // ── Activity log handler ───────────────────────────
        private string HandleShowLog()
        {
            var log = ActivityLog.GetLog();
            if (log.Count == 0)
                return "No actions logged yet.";
            string output = "Recent activity log:\n";
            for (int i = 0; i < log.Count; i++)
                output += $"  {i + 1}. {log[i]}\n";
            ActivityLog.AddEntry("Viewed activity log.");
            return output;
        }

        // ── Random error responses ────────────────────────
        private string GetRandomError()
        {
            return string.Format(_errorResponses[_random.Next(_errorResponses.Count)], _userName);
        }

        // ── Topic menu ──────────────────────────────────────
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
                   "NEW COMMANDS:\n" +
                   "  • add task [description]  – add a cybersecurity task\n" +
                   "  • remind me [task]        – add with reminder (default 3 days)\n" +
                   "  • show tasks              – list pending tasks\n" +
                   "  • complete task [id]      – mark task as done\n" +
                   "  • delete task [id]        – remove a task\n" +
                   "  • start quiz              – take a cybersecurity quiz\n" +
                   "  • activity log            – see recent actions\n" +
                   "────────────────────────────────────────\n" +
                   "Type a keyword (password, scam, etc.) or use the new commands.\n" +
                   "Type 'exit' to quit.";
        }
    }
}