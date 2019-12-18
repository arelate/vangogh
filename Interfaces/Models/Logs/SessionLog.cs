using System;
using System.Collections.Generic;

namespace Interfaces.Models.Logs
{
    public interface ISessionLog
    {
        DateTime Started { get; set; }
        DateTime Completed { get; set; }
        string Title { get; set; }
        bool Complete { get; set; }
        List<IActionLog> CompletedActions { get; set; }
        Stack<IActionLog> OngoingActions { get; set; }
    }
}