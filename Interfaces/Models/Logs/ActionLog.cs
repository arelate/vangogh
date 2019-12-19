using System;

namespace Interfaces.Models.Logs
{
    public interface IActionLog
    {
        string Title { get; set; }
        int Progress { get; set; }
        int Target { get; set; }
        bool Complete { get; set; }
        DateTime Started { get; set; }
        DateTime Completed { get; set; }
    }
}