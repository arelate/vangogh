using System;
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
            //public const string Title = "title";
            //public const string ContainsProgress = "containsProgress";
            //public const string ProgressTarget = "progressTarget";
            //public const string ProgressPercent = "progressPercent";
            //public const string ProgressCurrent = "progressCurrent";
            //public const string ProgressTotal = "progressTotal";
            //public const string ContainsETA = "containsEta";
            //public const string RemainingTime = "remainingTime";
            //public const string AverageUnitsPerSecond = "averageUnitsPerSecond";
            //public const string ContainsFailures = "containsFailures";
            //public const string FailuresCount = "failuresCount";
            //public const string ContainsWarnings = "containsWarnings";
            //public const string WarningsCount = "warningsCount";
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
            var viewModel = new Dictionary<string, string>();
            //{
            //    { TaskStatusReportViewModelSchema.Title, "" },
            //    { TaskStatusReportViewModelSchema.ContainsProgress, "" },
            //    { TaskStatusReportViewModelSchema.ProgressTarget, "" },
            //    { TaskStatusReportViewModelSchema.ProgressPercent, "" },
            //    { TaskStatusReportViewModelSchema.ProgressCurrent, "" },
            //    { TaskStatusReportViewModelSchema.ProgressTotal, "" },
            //    { TaskStatusReportViewModelSchema.ContainsETA, "" },
            //    { TaskStatusReportViewModelSchema.RemainingTime, "" },
            //    { TaskStatusReportViewModelSchema.AverageUnitsPerSecond, "" },
            //    { TaskStatusReportViewModelSchema.ContainsFailures, ""},
            //    { TaskStatusReportViewModelSchema.FailuresCount, ""},
            //    { TaskStatusReportViewModelSchema.ContainsWarnings, ""},
            //    { TaskStatusReportViewModelSchema.WarningsCount, ""}
            //};

            //viewModel[TaskStatusReportViewModelSchema.Title] = taskStatus.Title;

            //if (taskStatus.Progress != null)
            //{
            //    var current = taskStatus.Progress.Current;
            //    var total = taskStatus.Progress.Total;

            //    viewModel[TaskStatusReportViewModelSchema.ContainsProgress] = "true";
            //    viewModel[TaskStatusReportViewModelSchema.ProgressTarget] = taskStatus.Progress.Target;
            //    viewModel[TaskStatusReportViewModelSchema.ProgressPercent] = string.Format("{0:P1}", (double)current / total);

            //    var currentFormatted = current.ToString();
            //    var totalFormatted = total.ToString();

            //    if (taskStatus.Progress.Unit == DataUnits.Bytes)
            //    {
            //        viewModel[TaskStatusReportViewModelSchema.ContainsETA] = "true";

            //        currentFormatted = bytesFormattingController.Format(current);
            //        totalFormatted = bytesFormattingController.Format(total);

            //        var elapsed = DateTime.UtcNow - taskStatus.Started;
            //        var unitsPerSecond = current / elapsed.TotalSeconds;
            //        var speed = bytesFormattingController.Format((long)unitsPerSecond);
            //        var remainingTime = secondsFormattingController.Format((long)((total - current) / unitsPerSecond));

            //        viewModel[TaskStatusReportViewModelSchema.RemainingTime] = remainingTime;
            //        viewModel[TaskStatusReportViewModelSchema.AverageUnitsPerSecond] = speed;
            //    }

            //    viewModel[TaskStatusReportViewModelSchema.ProgressCurrent] = currentFormatted;
            //    viewModel[TaskStatusReportViewModelSchema.ProgressTotal] = totalFormatted;
            //}

            //if (taskStatus.Failures != null && taskStatus.Failures.Count > 0)
            //{
            //    viewModel[TaskStatusReportViewModelSchema.ContainsFailures] = "true";
            //    viewModel[TaskStatusReportViewModelSchema.FailuresCount] = taskStatus.Failures.Count.ToString();
            //}

            //if (taskStatus.Warnings != null && taskStatus.Warnings.Count > 0)
            //{
            //    viewModel[TaskStatusReportViewModelSchema.ContainsWarnings] = "true";
            //    viewModel[TaskStatusReportViewModelSchema.WarningsCount] = taskStatus.Warnings.Count.ToString();
            //}

            return viewModel;
        }
    }
}
