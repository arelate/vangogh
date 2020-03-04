using System;

using Interfaces.Models.Logs;

namespace Models.Logs
{
    public class ActionLog : IActionLog
    {
        public string Title { get; set; }
        public int Progress { get; set; }
        public int Target { get; set; }
        public bool Complete { get; set; }
        public DateTime Started { get; set; }
        public DateTime Completed { get; set; }
    }
}