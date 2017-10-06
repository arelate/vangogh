using System;
using System.Collections.Generic;

using Interfaces.Status;
using Interfaces.ViewController;

namespace Controllers.Status
{
    public class StatusController : IStatusController
    {
        private IViewController<string[]> statusViewController;

        public StatusController(
            IViewController<string[]> statusViewController)
        {
            this.statusViewController = statusViewController;
        }

        private void AssertValidStatus(IStatus status)
        {
            if (status == null)
                throw new ArgumentNullException("Current task status cannot be null");
        }

        public IStatus Create(IStatus status, string title)
        {
            AssertValidStatus(status);

            if (status.Children == null)
                status.Children = new List<IStatus>();

            var childStatus = new Models.Status.Status()
            {
                Title = title,
                Started = DateTime.UtcNow
            };
            status.Children.Add(childStatus);

            statusViewController.PostUpdateNotification();

            return childStatus;
        }

        public void Complete(IStatus status)
        {
            AssertValidStatus(status);

            if (status.Complete)
                throw new InvalidOperationException("Task status is already complete.");

            status.Complete = true;
            status.Completed = DateTime.UtcNow;

            statusViewController.PostUpdateNotification();
        }

        public void UpdateProgress(IStatus status, long current, long total, string target, string unit = "")
        {
            AssertValidStatus(status);

            if (status.Complete)
                throw new InvalidOperationException("Cannot update completed task status.");

            if (status.Progress == null)
                status.Progress = new Models.Status.StatusProgress();

            status.Progress.Target = target;
            status.Progress.Current = current;
            status.Progress.Total = total;
            status.Progress.Unit = unit;

            statusViewController.PostUpdateNotification();
        }

        public void Fail(IStatus status, string failureMessage)
        {
            AssertValidStatus(status);

            if (status.Failures == null)
                status.Failures = new List<string>();

            status.Failures.Add(failureMessage);
        }

        public void Warn(IStatus status, string warningMessage)
        {
            AssertValidStatus(status);

            if (status.Warnings == null)
                status.Warnings = new List<string>();

            status.Warnings.Add(warningMessage);
        }

        public void Inform(IStatus status, string informationMessage)
        {
            AssertValidStatus(status);

            if (status.Information == null)
                status.Information = new List<string>();

            status.Information.Add(informationMessage);
        }

        public void AddSummaryResults(IStatus status, params string[] summaryResults)
        {
            if (summaryResults == null) return;

            AssertValidStatus(status);

            if (status.SummaryResults == null)
                status.SummaryResults = new List<string>();

            foreach (var summaryResult in summaryResults)
                status.SummaryResults.Add(summaryResult);
        }
    }
}
