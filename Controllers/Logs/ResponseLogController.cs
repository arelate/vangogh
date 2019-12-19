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
        private List<IActionLog> completedActions { get; set; } = new List<IActionLog>();
        private Stack<IActionLog> ongoingActions { get; set; } = new Stack<IActionLog>();

        public IActionLog CurrentActionLog
        {
            get
            {
                return ongoingActions.Peek();
            }
        }

        public void StartAction(string title)
        {
            var action = new ActionLog() { Title = title };
            action.Started = DateTime.UtcNow;
            ongoingActions.Push(action);
            System.Console.WriteLine($"Started action {action.Title}");
        }

        public void SetActionTarget(int target)
        {
            CurrentActionLog.Target = target;
        }

        public void IncrementActionProgress(int increment = 1)
        {
            CurrentActionLog.Progress += increment;
            System.Console.WriteLine($"Action {CurrentActionLog.Title} progress: {CurrentActionLog.Progress}");
        }

        public void CompleteAction()
        {
            var action = ongoingActions.Pop();
            action.Complete = true;
            action.Completed = DateTime.UtcNow;
            completedActions.Add(action);
            System.Console.WriteLine($"Completed action {action.Title}");
        }
    }
}