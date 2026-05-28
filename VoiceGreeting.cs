using System;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace CyberGuard
{
    internal class VoiceGreeting
    {
        public void PlayGreeting(Form owner)
        {
            try
            {
                // Search in multiple locations so it works both inside
                // Visual Studio (debug/release) and when run directly
                string[] searchPaths = new string[]
                {
                    // Same folder as the .exe (most common at runtime)
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "welcome.wav.wav"),

                    // Project root (two levels up from bin\Debug\)
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\welcome.wav.wav"),

                    // Current working directory
                    Path.Combine(Directory.GetCurrentDirectory(), "welcome.wav.wav"),

                    // One level up from exe
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\welcome.wav.wav")
                };

                foreach (string path in searchPaths)
                {
                    string fullPath = Path.GetFullPath(path);
                    if (File.Exists(fullPath))
                    {
                        SoundPlayer player = new SoundPlayer(fullPath);
                        player.Play();
                        return; // Found and played — done
                    }
                }

                // File not found anywhere — silently continue, no crash
            }
            catch
            {
                // Any other error — silently continue, no crash
            }
        }
    }
}