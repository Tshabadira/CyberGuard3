using System;
using System.Collections.Generic;

namespace CyberGuard
{
    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }  // for multiple choice; for true/false use two options
        public int CorrectIndex { get; set; }      // zero-based
        public string Explanation { get; set; }
        public bool IsTrueFalse => Options.Count == 2 &&
                                   (Options[0].ToLower() == "true" || Options[0].ToLower() == "false");
    }

    internal class QuizEngine
    {
        private List<QuizQuestion> _questions;
        private int _currentIndex;
        private int _score;

        public bool IsActive { get; private set; }
        public int TotalQuestions => _questions?.Count ?? 0;
        public int CurrentQuestionNumber => IsActive ? _currentIndex + 1 : 0;
        public int Score => _score;

        public QuizEngine()
        {
            InitializeQuestions();
            Reset();
        }

        private void InitializeQuestions()
        {
            _questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    CorrectIndex = 2,
                    Explanation = "Reporting phishing emails helps prevent scams and protects others."
                },
                new QuizQuestion
                {
                    Question = "A strong password should be at least how many characters?",
                    Options = new List<string> { "6", "8", "12", "16" },
                    CorrectIndex = 2,
                    Explanation = "A minimum of 12 characters with a mix of letters, numbers, and symbols is recommended."
                },
                new QuizQuestion
                {
                    Question = "True or False: Using the same password for multiple accounts is safe.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "Reusing passwords puts all your accounts at risk if one is breached."
                },
                // Add 7+ more questions to reach >10 total
                new QuizQuestion
                {
                    Question = "What is a common sign of a phishing email?",
                    Options = new List<string> { "Personalised greeting", "Urgent language", "Correct spelling", "Trusted sender" },
                    CorrectIndex = 1,
                    Explanation = "Phishing emails often create urgency to trick you into acting without thinking."
                },
                new QuizQuestion
                {
                    Question = "Which of these is a safe browsing practice?",
                    Options = new List<string> { "Click any link", "Visit HTTPS sites", "Download from unknown sources", "Share your location" },
                    CorrectIndex = 1,
                    Explanation = "HTTPS encrypts the connection, protecting your data from eavesdroppers."
                },
                new QuizQuestion
                {
                    Question = "True or False: Public Wi-Fi is always safe to use for banking.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "Public Wi-Fi is unencrypted; hackers can intercept your data. Use a VPN instead."
                },
                new QuizQuestion
                {
                    Question = "What is two-factor authentication (2FA)?",
                    Options = new List<string> { "A second password", "A second verification step", "A fingerprint scan", "A password manager" },
                    CorrectIndex = 1,
                    Explanation = "2FA adds an extra layer of security by requiring a second form of verification."
                },
                new QuizQuestion
                {
                    Question = "Which of the following is a form of social engineering?",
                    Options = new List<string> { "Phishing", "Malware", "Ransomware", "Spyware" },
                    CorrectIndex = 0,
                    Explanation = "Phishing is a social engineering attack that manipulates people into revealing information."
                },
                new QuizQuestion
                {
                    Question = "True or False: Antivirus software alone can protect you from all threats.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "Antivirus is important, but you also need safe browsing habits, 2FA, and regular updates."
                },
                new QuizQuestion
                {
                    Question = "What should you do if you suspect a scam call?",
                    Options = new List<string> { "Engage with the caller", "Hang up and call back on an official number", "Give minimal info", "Send a follow-up email" },
                    CorrectIndex = 1,
                    Explanation = "Always verify by calling the official number; do not trust the caller's provided number."
                }
            };
        }

        public void Reset()
        {
            _currentIndex = 0;
            _score = 0;
            IsActive = false;
        }

        public string Start()
        {
            Reset();
            IsActive = true;
            return GetCurrentQuestionText();
        }

        private string GetCurrentQuestionText()
        {
            if (!IsActive || _currentIndex >= _questions.Count)
                return "No active quiz.";

            var q = _questions[_currentIndex];
            string output = $"Q{_currentIndex + 1}. {q.Question}\n";
            for (int i = 0; i < q.Options.Count; i++)
            {
                output += $"   {(char)('A' + i)}) {q.Options[i]}\n";
            }
            output += "Type your answer (A, B, C, D) or 'quit' to end the quiz.";
            return output;
        }

        public string SubmitAnswer(string input)
        {
            if (!IsActive)
                return "No quiz is running. Type 'start quiz' to begin.";

            if (input.Trim().ToLower() == "quit")
            {
                IsActive = false;
                return $"Quiz ended. Your score: {_score}/{_questions.Count}. Stay safe!";
            }

            // Map input to index (A->0, B->1, etc.)
            int answerIndex = -1;
            string trimmed = input.Trim().ToUpper();
            if (trimmed.Length == 1 && trimmed[0] >= 'A' && trimmed[0] <= 'D')
                answerIndex = trimmed[0] - 'A';
            else
                return "Invalid option. Please type A, B, C, or D.";

            var q = _questions[_currentIndex];
            bool correct = answerIndex == q.CorrectIndex;
            if (correct) _score++;

            string result = correct ? "Correct! " : $"Incorrect. The correct answer was {(char)('A' + q.CorrectIndex)}. ";
            result += q.Explanation;

            // Move to next question
            _currentIndex++;
            if (_currentIndex >= _questions.Count)
            {
                IsActive = false;
                string feedback = _score >= 8 ? "Great job! You’re a cybersecurity pro!" :
                                  _score >= 5 ? "Good effort! Keep learning to stay safe online." :
                                  "Keep learning! Review the topics to improve your security knowledge.";
                return result + $"\n\nQuiz complete! Your final score: {_score}/{_questions.Count}. {feedback}";
            }
            else
            {
                return result + "\n\n" + GetCurrentQuestionText();
            }
        }
    }
}