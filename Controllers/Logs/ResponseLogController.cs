using System;

using Interfaces.Controllers.Logs;
using Interfaces.Models.Logs;

using Models.Logs;

namespace Controllers.Logs
{
    // TODO: This can be used to track response and session aggregate metrics - 
    // bytes transferred, read from disk, written to disk etc
    public class ResponseLogController : IResponseLogController
    {
        private IResponseLog currentResponseLog;
        private IActionLog currentActionLog
        {
            get
            {
                return currentResponseLog.OngoingActions.Peek();
            }
        }

        public void OpenResponseLog(string title)
        {
            if (currentResponseLog != null &&
                !currentResponseLog.Complete)
                throw new System.InvalidOperationException();

            currentResponseLog = new ResponseLog() { Title = title };
            currentResponseLog.Started = DateTime.Now;
            System.Console.WriteLine($"Started {currentResponseLog.Title}");
        }

        public void StartAction(string title)
        {
            var action = new ActionLog() { Title = title };
            currentResponseLog.OngoingActions.Push(action);
            System.Console.WriteLine($"Started action {action.Title}");
        }

        public void SetActionProgress(int progress)
        {
            IActionLog currentLog = currentActionLog != null ?
                currentActionLog :
                currentResponseLog;

            currentLog.Progress = progress;
            System.Console.WriteLine($"Action {currentActionLog.Title} progress: {currentActionLog.Progress}");
        }

        public double GetActionProgressPercent(int total)
        {
            return (double)currentActionLog.Progress / total;
        }

        public void IncrementActionProgress()
        {
            IActionLog currentLog = currentActionLog != null ?
                currentActionLog :
                currentResponseLog;

            currentLog.Progress++;
            System.Console.WriteLine($"Action {currentActionLog.Title} progress: {currentActionLog.Progress}");
        }

        public void CompleteAction()
        {
            var action = currentResponseLog.OngoingActions.Pop();
            action.Complete = true;
            currentResponseLog.CompletedActions.Add(action);
            System.Console.WriteLine($"Completed action {action.Title}");
        }

        public IResponseLog CloseResponseLog()
        {
            currentResponseLog.Complete = true;
            currentResponseLog.Completed = DateTime.Now;
            System.Console.WriteLine($"Completed session {currentResponseLog.Title}");
            return currentResponseLog;
        }
    }
}