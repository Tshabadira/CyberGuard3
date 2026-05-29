# 🛡️ CyberGuard v2.0 — Cybersecurity Awareness Chatbot

> A Windows Forms chatbot application built in C# that educates users on cybersecurity topics through interactive conversation, sentiment detection, memory recall, and dynamic responses.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Screenshots](#screenshots)
- [Requirements](#requirements)
- [Setup and Installation](#setup-and-installation)
- [How to Use](#how-to-use)
- [Project Structure](#project-structure)
- [Version History](#version-history)

---

## Overview

CyberGuard is a Part 2 Portfolio of Evidence project for PROG6221 at The Independent Institute of Education (IIE). It expands on a console-based chatbot (Part 1) by adding a full graphical user interface, dynamic responses, sentiment detection, memory and recall, and improved keyword recognition.

The chatbot is designed to raise cybersecurity awareness by guiding users through topics such as password safety, phishing, malware, scams, and more — responding naturally to typed input and adapting based on the user's mood and interests.

---

## Features

| Feature | Description |
|---|---|
| 🖥️ GUI Interface | Dark-themed Windows Forms app with orange, green, red and black colour scheme |
| 🎨 ASCII Art Banner | CyberGuard logo rendered in the chat window at startup |
| 🔊 Voice Greeting | Plays a welcome audio file (`welcome.wav`) on launch |
| 🔑 Keyword Recognition | Recognises 9+ cybersecurity keywords and responds with targeted tips |
| 🎲 Random Responses | Randomly selects from multiple predefined responses for varied interaction |
| 💬 Conversation Flow | Handles follow-up phrases like "more", "tell me more", "explain more" |
| 🧠 Memory and Recall | Remembers the user's name and favourite topic, references them later |
| 😟 Sentiment Detection | Detects emotions (worried, confused, frustrated, curious, etc.) and responds empathetically |
| ⚠️ Error Handling | Handles unknown input gracefully without crashing |
| 🏗️ OOP Structure | Clean class separation: `Chatbot`, `MainForm`, `NameDialog`, `VoiceGreeting` |

---

## Requirements

- Windows 10 or later
- [Visual Studio 2019 or later](https://visualstudio.microsoft.com/)
- .NET Framework 4.7.2
- Windows Forms workload installed in Visual Studio

---

## Setup and Installation

### Option A — Run from Visual Studio

1. **Clone the repository**
   ```
   git clone https://github.com/Tshabadira/CyberGuard3.git
   ```

2. **Open the solution**
   - Open Visual Studio
   - Click **File → Open → Project/Solution**
   - Select `CyberGuard.sln`

3. **Add the voice file** *(optional)*
   - Place `welcome.wav` in the project root folder
   - In Solution Explorer, right-click it → **Add → Existing Item**
   - Select the file, then in the **Properties panel** set **Copy to Output Directory** to `Copy if newer`

4. **Build the project**
   - Click **Build → Rebuild Solution**
   - Confirm `0 errors` in the Output panel

5. **Run the application**
   - Press **F5** or click the green **Start** button

### Option B — Download the Release

1. Go to the [Releases](https://github.com/Tshabadira/CyberGuard3/releases) page
2. Download the latest `CyberGuard-Release.zip`
3. Extract the zip
4. Run `CyberGuard.exe`

---

## How to Use

1. Launch the application — a name dialog will appear
2. Enter your name (letters only, 2–30 characters) and click **Start Chat**
3. The chatbot will greet you and display the topic menu
4. Interact by:
   - Typing a **number** (e.g. `1` for Password Safety)
   - Typing a **keyword** (e.g. `phishing`, `malware`, `scam`)
   - Typing a **sentence** (e.g. `I'm worried about online scams`)
   - Typing `more` or `tell me more` for extra details on the last topic
   - Typing `I'm interested in privacy` to set your favourite topic
   - Typing `exit` or `bye` to quit

### Available Topics

| Number | Topic |
|---|---|
| 1 | Password Safety |
| 2 | Phishing Awareness |
| 3 | Safe Browsing |
| 4 | Malware Protection |
| 5 | Two-Factor Authentication |
| 6 | Social Engineering |
| 7 | Public Wi-Fi Safety |
| 8 | Show menu again |

### Recognised Keywords

`password` · `phishing` · `malware` · `virus` · `scam` · `fraud` · `privacy` · `wifi` · `wi-fi` · `2fa` · `two factor` · `browsing` · `browser` · `social engineering`

### Sentiment Words Detected

`worried` · `scared` · `anxious` · `confused` · `frustrated` · `curious` · `unsure` · `overwhelmed`

---

## Project Structure

```
CyberGuard/
│
├── Program.cs          → Entry point, launches MainForm
├── MainForm.cs         → WinForms UI — layout, colours, chat display
├── Chatbot.cs          → All chatbot logic — keywords, memory, sentiment, random responses
├── NameDialog.cs       → Startup name input dialog with validation
├── VoiceGreeting.cs    → Plays welcome.wav at startup
├── welcome.wav         → Voice greeting audio file
├── CyberGuard.csproj   → Project configuration (.NET Framework 4.7.2)
├── CyberGuard.sln      → Visual Studio solution file
│
└── .github/
    └── workflows/
        └── build.yml   → GitHub Actions CI — builds and packages on every push
```

---

## Version History

### v2.0 — GUI Edition
- Full Windows Forms interface with dark theme (black, orange, green, red)
- ASCII art banner preserved from Part 1
- Voice greeting integrated into GUI startup
- Sentiment detection added (8 emotions)
- Memory and recall feature added
- Random response pools added for varied interaction
- Conversation flow improved with follow-up handling
- GitHub Actions CI/CD workflow added

### v1.1 — Console Improvements
- Added `more` command for extra topic details
- Input validation improved
- Error handling added for unknown inputs
- Voice greeting added

### v1.0 — Console Release
- Initial console-based chatbot
- 7 cybersecurity topics with static responses
- Name input with validation
- ASCII art banner
- Numbered menu system

---

## 📹 Video Presentation

> 🔗 [Watch the project demonstration on YouTube](#) *(link to be added)*

---

## Author

**Tshabadira**
IIE Student · PROG6221 · 2026

---

*© The Independent Institute of Education (Pty) Ltd 2026*# CyberGuard
