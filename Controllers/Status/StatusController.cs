using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;

namespace Controllers.Status
{
    public class StatusController : IStatusController
    {
        public event StatusChangedNotificationAsyncDelegate NotifyStatusChangedAsync;

        void AssertValidStatus(IStatus status)
        {
            if (status == null)
                throw new ArgumentNullException();
        }

        public async Task<IStatus> CreateAsync(IStatus status, string title, bool notifyStatusChanged = true)
        {
            AssertValidStatus(status);

            if (status.Children == null)
                status.Children = new List<IStatus>();

            var childStatus = new Models.Status.Status
            {
                Title = title,
                Started = DateTime.UtcNow
            };
            status.Children.Add(childStatus);

            if (notifyStatusChanged)
                await NotifyStatusChangedAsync?.Invoke();

            return childStatus;
        }

        public async Task CompleteAsync(IStatus status, bool notifyStatusChanged = true)
        {
            AssertValidStatus(status);

            if (status.Complete)
                throw new InvalidOperationException("Task status is already complete.");

            status.Complete = true;
            status.Completed = DateTime.UtcNow;

            if (notifyStatusChanged)
                await NotifyStatusChangedAsync?.Invoke();
        }

        public async Task UpdateProgressAsync(IStatus status, long current, long total, string target, string unit = "")
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

            await NotifyStatusChangedAsync?.Invoke();
        }

        public async Task FailAsync(IStatus status, string failureMessage)
        {
            AssertValidStatus(status);

            if (status.Failures == null)
                status.Failures = new List<string>();

            status.Failures.Add(failureMessage);

            await CompleteAsync(status); // this will notify status change subscribers
        }

        public async Task WarnAsync(IStatus status, string warningMessage)
        {
            AssertValidStatus(status);

            if (status.Warnings == null)
                status.Warnings = new List<string>();

            status.Warnings.Add(warningMessage);

            await NotifyStatusChangedAsync?.Invoke();
        }

        public async Task InformAsync(IStatus status, string informationMessage)
        {
            AssertValidStatus(status);

            if (status.Information == null)
                status.Information = new List<string>();

            status.Information.Add(informationMessage);

            await NotifyStatusChangedAsync?.Invoke();
        }

        public async Task PostSummaryResultsAsync(IStatus status, params string[] summaryResults)
        {
            if (summaryResults == null) return;

            AssertValidStatus(status);

            if (status.SummaryResults == null)
                status.SummaryResults = new List<string>();

            foreach (var summaryResult in summaryResults)
                status.SummaryResults.Add(summaryResult);

            await NotifyStatusChangedAsync?.Invoke();
        }
    }
}
