using System;
using System.Media;
using System.Windows.Forms;

namespace CyberGuard
{
    // Plays the welcome WAV file at startup
    // Place welcome.wav in the same folder as the .exe
    internal class VoiceGreeting
    {
        public void PlayGreeting(Form owner)
        {
            try
            {
                SoundPlayer player = new SoundPlayer("welcome.wav");
                player.Play(); // Async so the form still loads
            }
            catch
            {
                // Voice file missing — silently continue, no crash
                // Requirement 7: error handling, no termination on unexpected issues
            }
        }
    }
}
