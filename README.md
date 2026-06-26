# 🛡️ CyberGuard v3.0 — Cybersecurity Awareness Chatbot

> A Windows Forms chatbot application built in C# that educates users on cybersecurity topics through interactive conversation, sentiment detection, memory recall, NLP simulation, a cybersecurity quiz, and a MySQL-backed task assistant.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Requirements](#requirements)
- [Setup and Installation](#setup-and-installation)
- [How to Use](#how-to-use)
- [Project Structure](#project-structure)
- [Version History](#version-history)
- [Video Presentation](#video-presentation)

---

## Overview

CyberGuard is a three-part Portfolio of Evidence project for PROG6221 at The Independent Institute of Education (IIE). It progressively builds a cybersecurity awareness chatbot from a console application (Part 1) to a full GUI application (Part 2) and finally a feature-rich interactive system (Part 3) with database integration, a quiz engine, NLP simulation, and activity logging.

---

## Features

### Part 1 & 2 Features (carried into Part 3)
| Feature | Description |
|---|---|
| 🖥️ GUI Interface | Dark-themed Windows Forms app — black, orange, green, red colour scheme |
| 🎨 ASCII Art Banner | CyberGuard logo rendered in the chat window at startup |
| 🔊 Voice Greeting | Plays `welcome.wav` on launch |
| 🔑 Keyword Recognition | Recognises 9+ cybersecurity keywords with targeted responses |
| 🎲 Random Responses | 8 responses per topic selected randomly using `List<string>` |
| 💬 Conversation Flow | Follow-up phrases like 'more', 'tell me more', 'elaborate' |
| 🧠 Memory and Recall | Remembers name, favourite topic, topics discussed — recalls naturally |
| 😟 Sentiment Detection | Detects 10 emotions and responds empathetically before sharing a tip |
| ⚠️ Error Handling | Handles unknown input with varied error responses, no crashes |

### Part 3 New Features
| Feature | Description |
|---|---|
| 📋 Task Assistant | Add, view, complete, and delete cybersecurity tasks |
| 🗄️ MySQL Database | All tasks stored and retrieved from a MySQL database |
| ⏰ Reminders | Set reminder dates on tasks when adding them |
| 🎮 Cybersecurity Quiz | 10+ questions, multiple choice and true/false, score tracking |
| 🤖 NLP Simulation | Recognises naturally phrased commands using keyword intent detection |
| 📜 Activity Log | Records all bot actions with timestamps, shows last 10 entries |
| 🔗 Full Integration | All Part 1, 2, and 3 features work together seamlessly |

---

## Requirements

- Windows 10 or later
- Visual Studio 2019 or later
- .NET Framework 4.7.2
- MySQL Server 8.0 or later
- MySQL Workbench (optional but recommended)
- NuGet package: `MySql.Data` version 8.0.33

---

## Setup and Installation

### Step 1 — Clone the repository
```
git clone https://github.com/Tshabadira/CyberGuard3.git
```

### Step 2 — Set up MySQL Database

1. Open MySQL Workbench
2. Connect to your local instance
3. Run the following SQL:

```sql
CREATE DATABASE IF NOT EXISTS cyberguard;
USE cyberguard;

CREATE TABLE IF NOT EXISTS tasks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    reminder_date DATETIME NULL,
    is_completed BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### Step 3 — Configure the connection string

Open `App.config` and update the password to match your MySQL root password:

```xml
<connectionStrings>
    <add name="CyberGuardDB"
         connectionString="Server=localhost;Database=cyberguard;Uid=root;Pwd=YOUR_PASSWORD;" />
</connectionStrings>
```

### Step 4 — Install NuGet packages

In Visual Studio:
**Tools → NuGet Package Manager → Manage NuGet Packages for Solution**
Search for `MySql.Data` and install version `8.0.33`

### Step 5 — Add the voice file

1. Place `welcome.wav` in the project root folder
2. Right-click it in Solution Explorer → **Properties**
3. Set **Copy to Output Directory** to `Copy if newer`

### Step 6 — Build and Run

```
Build → Rebuild Solution
```
Press **F5** or click **Start**

---

## How to Use

### Getting Started
1. Launch the application
2. Enter your name in the dialog (letters only, 2–30 characters)
3. The chatbot will greet you and display the full menu

### Cybersecurity Topics
| Input | Topic |
|---|---|
| `1` or `password` | Password Safety |
| `2` or `phishing` | Phishing Awareness |
| `3` or `browsing` | Safe Browsing |
| `4` or `malware` | Malware Protection |
| `5` or `2fa` | Two-Factor Authentication |
| `6` or `social` | Social Engineering |
| `7` or `wifi` | Public Wi-Fi Safety |
| `8` | Show menu again |
| `more` | Extra tips on last topic |

### Task Assistant Commands
| Command | What it does |
|---|---|
| `add task - Enable two-factor authentication` | Adds a task to your list |
| `remind me to update my password` | Adds a task with NLP recognition |
| `view tasks` | Shows all pending tasks with IDs |
| `complete task 1` | Marks task 1 as completed |
| `delete task 2` | Removes task 2 from the database |

### Quiz Commands
| Command | What it does |
|---|---|
| `start quiz` | Begins the cybersecurity quiz |
| `A` / `B` / `C` / `D` | Answers a multiple choice question |
| `True` / `False` | Answers a true/false question |
| `quit` | Ends the quiz early |

### Activity Log
| Command | What it does |
|---|---|
| `activity log` | Shows last 10 recorded actions |
| `show activity log` | Same as above |
| `what have you done` | Same as above |

### Memory and Sentiment Examples
```
I'm interested in privacy
→ Bot saves it and recalls later: "As someone interested in privacy..."

I'm worried about online scams
→ Bot responds empathetically then immediately shares a scam tip

I'm confused about 2fa
→ Bot explains calmly then shares a 2FA tip
```

### NLP Examples — Natural Language Commands
```
"Can you remind me to check my privacy settings"   → adds a task
"I need to remember to update my antivirus"        → adds a task
"Don't let me forget to enable 2FA"                → adds a task
"Test my knowledge"                                → starts quiz
"What have you done for me?"                       → shows activity log
```

---

## Project Structure

```
CyberGuard/
│
├── Program.cs           → Entry point, launches MainForm
├── MainForm.cs          → WinForms UI — layout, colours, chat display
├── NameDialog.cs        → Startup name input dialog with validation
├── Chatbot.cs           → All chatbot logic — NLP, memory, sentiment, keywords
├── QuizEngine.cs        → Quiz questions, answer handling, score tracking
├── TaskManager.cs       → MySQL CRUD operations for tasks
├── DatabaseHelper.cs    → Ensures MySQL table exists on startup
├── ActivityLog.cs       → Static log with timestamps, last 10 entries
├── VoiceGreeting.cs     → Plays welcome.wav at startup
│
├── App.config           → MySQL connection string configuration
├── packages.config      → NuGet package references
├── welcome.wav          → Voice greeting audio file
├── CyberGuard.csproj    → Project file (.NET Framework 4.7.2)
├── CyberGuard.sln       → Visual Studio solution file
│
└── .github/
    └── workflows/
        └── build.yml    → GitHub Actions CI — builds on every push
```

---

## Version History

### v3.0 — Full POE Release (Part 3)
- MySQL database integration for task storage
- Task assistant: add, view, complete, delete with reminders
- Cybersecurity mini-game quiz with 10+ questions
- NLP simulation with expanded intent keyword detection
- Activity log recording all actions with timestamps
- Full integration of Parts 1, 2, and 3
- GitHub Actions CI/CD workflow

### v2.0 — GUI Edition (Part 2)
- Full Windows Forms interface with dark theme
- ASCII art banner preserved from Part 1
- Voice greeting integrated into GUI startup
- Sentiment detection (10 emotions)
- Memory and recall with favourite topic storage
- Random response pools for varied interaction
- Conversation flow with follow-up handling

### v1.1 — Console Improvements (Part 1 update)
- Added `more` command for extra topic details
- Input validation improved
- Error handling for unknown inputs

### v1.0 — Console Release (Part 1)
- Initial console-based chatbot
- 7 cybersecurity topics with responses
- Name input with validation
- ASCII art banner and numbered menu

---

## 📹 Video Presentation

> 🔗 **[Watch the project demonstration on YouTube](#)**
> *(Add your YouTube link here after recording)*

The video covers:
- Application launch with voice greeting and ASCII art
- All cybersecurity topics and random responses
- Sentiment detection demonstration
- Memory and recall demonstration
- Task assistant with MySQL database
- Cybersecurity quiz walkthrough
- Activity log display
- NLP natural language commands
- Code walkthrough of key classes

---

## Author

**Tshabadira**
IIE Student · PROG6221 · 2026

---

*© The Independent Institute of Education (Pty) Ltd 2026*
