using System;
using System.Collections.Generic;

namespace Interfaces.Models.Logs
{
    public interface IResponseLog: IActionLog
    {
        DateTime Started { get; set; }
        DateTime Completed { get; set; }
        List<IActionLog> CompletedActions { get; set; }
        Stack<IActionLog> OngoingActions { get; set; }
    }
}