using System;

using Interfaces.Controllers.Logs;
using Interfaces.Models.Logs;

using Models.Logs;

namespace Controllers.Logs
{
    public class SessionLogController: ISessionLogController
    {
        private ISessionLog currentSessionLog;
        private IActionLog currentActionLog
        {
            get
            {
                return currentSessionLog.OngoingActions.Peek();
            }
        }

        public void StartSession(string title)
        {
            if (currentSessionLog != null &&
                !currentSessionLog.Complete)
                throw new System.InvalidOperationException();

            currentSessionLog = new SessionLog() { Title = title };
            currentSessionLog.Started = DateTime.Now;
            System.Console.WriteLine($"Started {currentSessionLog.Title}");
        }

        public void StartAction(string title)
        {
            var action = new ActionLog() { Title = title };
            currentSessionLog.OngoingActions.Push(action);
            System.Console.WriteLine($"Started action {action.Title}");
        }

        public void SetActionProgress(int progress)
        {
            currentActionLog.Progress = progress;
            System.Console.WriteLine($"Action {currentActionLog.Title} progress: {currentActionLog.Progress}");
        }

        public double GetActionProgressPercent(int total)
        {
            return (double)currentActionLog.Progress / total;
        }

        public void IncrementActionProgress()
        {
            currentActionLog.Progress++;
            System.Console.WriteLine($"Action {currentActionLog.Title} progress: {currentActionLog.Progress}");
        }

        public void CompleteAction()
        {
            var action = currentSessionLog.OngoingActions.Pop();
            action.Complete = true;
            currentSessionLog.CompletedActions.Add(action);
            System.Console.WriteLine($"Completed action {action.Title}");
        }

        public ISessionLog CompleteSession()
        {
            currentSessionLog.Complete = true;
            currentSessionLog.Completed = DateTime.Now;
            System.Console.WriteLine($"Completed session {currentSessionLog.Title}");
            return currentSessionLog;
        }
    }
}