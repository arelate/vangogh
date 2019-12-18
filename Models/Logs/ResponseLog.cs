using System;
using System.Collections.Generic;

using Interfaces.Models.Logs;

namespace Models.Logs
{
    public class ResponseLog : IResponseLog
    {
        public DateTime Started { get; set; }
        public DateTime Completed { get; set; }
        public string Title { get; set; }
        public bool Complete { get; set; }
        public int Progress { get; set; }
        public List<IActionLog> CompletedActions { get; set; } = new List<IActionLog>();
        public Stack<IActionLog> OngoingActions { get; set; } = new Stack<IActionLog>();
    }
}