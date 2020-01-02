using System;
using System.Collections.Generic;

using Interfaces.Controllers.Logs;
using Interfaces.Models.Logs;

using Models.Logs;

namespace Controllers.Logs
{
    // TODO: This can be used to track response and session aggregate metrics - 
    // bytes transferred, read from disk, written to disk etc
    public class ActionLogController : IActionLogController
    {
        private List<IActionLog> completedActionLogs { get; set; } = new List<IActionLog>();
        private Stack<IActionLog> ongoingActionLogs { get; set; } = new Stack<IActionLog>();

        public IActionLog CurrentActionLog
        {
            get
            {
                return ongoingActionLogs.Peek();
            }
        }

        public void StartAction(string title)
        {
            var actionLog = new ActionLog() { Title = title };
            actionLog.Started = DateTime.UtcNow;
            ongoingActionLogs.Push(actionLog);
            System.Console.WriteLine($"Started action {actionLog.Title}");
        }

        public void SetActionTarget(int target)
        {
            CurrentActionLog.Target = target;
        }

        public void IncrementActionProgress(int increment = 1)
        {
            CurrentActionLog.Progress += increment;
            System.Console.WriteLine($"{CurrentActionLog.Title} progress: {CurrentActionLog.Progress}");
        }

        public void CompleteAction()
        {
            var actionLog = ongoingActionLogs.Pop();
            actionLog.Complete = true;
            actionLog.Completed = DateTime.UtcNow;
            completedActionLogs.Add(actionLog);
            System.Console.WriteLine($"Completed action {actionLog.Title}");
        }
    }
}