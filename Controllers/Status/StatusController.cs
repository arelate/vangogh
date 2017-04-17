using System;
using System.Collections.Generic;

using Interfaces.Status;
using Interfaces.ViewController;

namespace Controllers.Status
{
    public class StatusController : IStatusController
    {
        private IViewController statusViewController;
        private DateTime lastPresentedProgress = DateTime.MinValue;
        private int presentProgressThreshold = 200; //ms

        public StatusController(
            IViewController statusViewController)
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

            var childStatus = new Models.Status.Status() {
                Title = title,
                Started = DateTime.UtcNow
            };
            status.Children.Add(childStatus);

            statusViewController.PresentViews();

            return childStatus;
        }

        public void Complete(IStatus status)
        {
            AssertValidStatus(status);

            if (status.Complete)
                throw new InvalidOperationException("Task status is already complete.");

            status.Complete = true;
            status.Completed = DateTime.UtcNow;

            statusViewController.PresentViews();
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

            var presentView = current == total ||
                (DateTime.Now - lastPresentedProgress).TotalMilliseconds > presentProgressThreshold;

            if (presentView)
            {
                statusViewController.PresentViews();
                lastPresentedProgress = DateTime.Now;
            }
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
    }
}
