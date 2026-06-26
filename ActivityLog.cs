using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberGuard
{
    internal static class ActivityLog
    {
        private static readonly List<string> _log = new List<string>();
        private const int MaxEntries = 10;

        public static void AddEntry(string action)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            _log.Insert(0, $"{timestamp}: {action}");
            if (_log.Count > MaxEntries)
                _log.RemoveAt(MaxEntries);
        }

        public static List<string> GetLog()
        {
            return _log.ToList();
        }

        public static string GetLogFormatted()
        {
            var log = GetLog();
            if (log.Count == 0)
                return "No actions recorded yet.";

            string output = "// ACTIVITY LOG — Last 10 Actions\n";
            output += "────────────────────────────────────────\n";
            for (int i = 0; i < log.Count; i++)
                output += $"  {i + 1}. {log[i]}\n";
            output += "────────────────────────────────────────\n";
            output += $"Total actions this session: {log.Count}\n";
            output += "Type 'full log' to see complete history.";
            return output;
        }

        public static void Clear() => _log.Clear();
    }
}