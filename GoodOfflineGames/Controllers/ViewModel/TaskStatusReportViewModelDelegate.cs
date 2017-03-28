using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.ViewModel;
using Interfaces.TaskStatus;
using Interfaces.Formatting;

using Models.Units;

namespace Controllers.ViewModel
{
    public class TaskStatusReportViewModelDelegate : IGetViewModelDelegate<ITaskStatus>
    {
        private static class TaskStatusReportViewModelSchema
        {
            public const string Complete = "complete";
            public const string Title = "title";
            public const string Started = "started";
            public const string Duration = "duration";
            public const string Result = "result";
            public const string ContainsProgress = "containsProgress";
            public const string ProgressTarget = "progressTarget";
            public const string ProgressCurrent = "progressCurrent";
            public const string ProgressTotal = "progressTotal";
            public const string ContainsFailures = "containsFailures";
            public const string Failures = "failures";
            public const string ContainsWarnings = "containsWarnings";
            public const string Warnings = "warnings";
            public const string ContainsInformation = "containsInformation";
            public const string Information = "information";
        }

        private IFormattingController bytesFormattingController;
        private IFormattingController secondsFormattingController;

        public TaskStatusReportViewModelDelegate(
            IFormattingController bytesFormattingController,
            IFormattingController secondsFormattingController)
        {
            this.bytesFormattingController = bytesFormattingController;
            this.secondsFormattingController = secondsFormattingController;
        }

        public IDictionary<string, string> GetViewModel(ITaskStatus taskStatus)
        {
            // viewmodel schemas
            var viewModel = new Dictionary<string, string>()
            {
                { TaskStatusReportViewModelSchema.Title, "" },
                { TaskStatusReportViewModelSchema.Complete, "" },
                { TaskStatusReportViewModelSchema.Started, "" },
                { TaskStatusReportViewModelSchema.Duration, "" },
                { TaskStatusReportViewModelSchema.Result, "" },
                { TaskStatusReportViewModelSchema.ContainsProgress, "" },
                { TaskStatusReportViewModelSchema.ProgressTarget, "" },
                { TaskStatusReportViewModelSchema.ProgressCurrent, "" },
                { TaskStatusReportViewModelSchema.ProgressTotal, "" },
                { TaskStatusReportViewModelSchema.ContainsFailures, ""},
                { TaskStatusReportViewModelSchema.Failures, ""},
                { TaskStatusReportViewModelSchema.ContainsWarnings, ""},
                { TaskStatusReportViewModelSchema.Warnings, ""},
                { TaskStatusReportViewModelSchema.ContainsInformation, ""},
                { TaskStatusReportViewModelSchema.Information, ""}
            };

            viewModel[TaskStatusReportViewModelSchema.Title] = taskStatus.Title;
            viewModel[TaskStatusReportViewModelSchema.Complete] = taskStatus.Complete ? "true" : "";
            viewModel[TaskStatusReportViewModelSchema.Started] = taskStatus.Started.ToLocalTime().ToString();
            viewModel[TaskStatusReportViewModelSchema.Duration] =
                taskStatus.Complete ?
                secondsFormattingController.Format((taskStatus.Completed - taskStatus.Started).Seconds) :
                "";

            var results = new List<string>();
            if (taskStatus.Failures != null && taskStatus.Failures.Any()) results.Add("Failure(s)");
            if (taskStatus.Warnings != null && taskStatus.Warnings.Any()) results.Add("Warning(s)");
            var result = string.Join(",", results);
            if (string.IsNullOrEmpty(result)) result = "Success";
            viewModel[TaskStatusReportViewModelSchema.Result] = result;

            if (taskStatus.Progress != null)
            {
                var current = taskStatus.Progress.Current;
                var total = taskStatus.Progress.Total;

                viewModel[TaskStatusReportViewModelSchema.ContainsProgress] = "true";
                viewModel[TaskStatusReportViewModelSchema.ProgressTarget] = taskStatus.Progress.Target;

                var currentFormatted = current.ToString();
                var totalFormatted = total.ToString();

                if (taskStatus.Progress.Unit == DataUnits.Bytes)
                {
                    currentFormatted = bytesFormattingController.Format(current);
                    totalFormatted = bytesFormattingController.Format(total);
                }

                viewModel[TaskStatusReportViewModelSchema.ProgressCurrent] = currentFormatted;
                viewModel[TaskStatusReportViewModelSchema.ProgressTotal] = totalFormatted;
            }

            if (taskStatus.Failures != null && taskStatus.Failures.Any())
            {
                viewModel[TaskStatusReportViewModelSchema.ContainsFailures] = "true";
                viewModel[TaskStatusReportViewModelSchema.Failures] = string.Join("; ", taskStatus.Failures);
            }

            if (taskStatus.Warnings != null && taskStatus.Warnings.Any())
            {
                viewModel[TaskStatusReportViewModelSchema.ContainsWarnings] = "true";
                viewModel[TaskStatusReportViewModelSchema.Warnings] = string.Join("; ", taskStatus.Warnings);
            }

            if (taskStatus.Information != null && taskStatus.Information.Any())
            {
                viewModel[TaskStatusReportViewModelSchema.ContainsInformation] = "true";
                viewModel[TaskStatusReportViewModelSchema.Information] = string.Join("; ", taskStatus.Information);
            }

            return viewModel;
        }
    }
}
