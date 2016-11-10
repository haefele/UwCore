using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace UwCore.Logging
{
    internal class InMemoryLogMessages
    {
        private static readonly ConcurrentQueue<string> _logs = new ConcurrentQueue<string>();

        public static void AddLog(string log)
        {
            _logs.Enqueue(log);
        }

        public static IList<string> GetLogs()
        {
            return _logs.ToArray();
        }
    }
}