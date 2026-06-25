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

        public static void Clear() => _log.Clear();
    }
}