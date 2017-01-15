using System;
using System.Text;
using System.Collections.Generic;

using Interfaces.Console;
using Interfaces.TaskStatus;
using Interfaces.Formatting;
using Interfaces.Presentation;

using Models.Units;
using Models.ViewModels;

namespace Controllers.TaskStatus
{
    public class TaskStatusViewController : ITaskStatusViewController
    {
        private ITaskStatus applicationTaskStatus;
        private IPresentationController<Tuple<string, string[]>> consolePresentationController;
        private IFormattingController bytesFormattingController;
        private IFormattingController secondsFormattingController;

        private Queue<ITaskStatus> taskStatusQueue;

        private const int throttleMilliseconds = 100;
        private DateTime lastReportedTimestamp = DateTime.MinValue;

        private const string suffix = ". ";

        private const string progressTargetTemplate = "%c{0}%c";
        private readonly string[] progressTargetColors = new string[] { "white", "default" };

        private const string progressTemplate = "{0:P1}, {1} of {2}";

        private const string progressETATemplate = "ETA: %c{0}%c";
        private readonly string[] progressETAColors = new string[] { "white", "default" };

        private const string childTasksTemplate = "%c{0}%c child task(s) complete";
        private readonly string[] childTasksColors = new string[] { "white", "default" };

        private const string warningsTemplate = "%cWarning(s): {0}%c";
        private readonly string[] warningsColors = new string[] { "yellow", "default" };

        private const string failuresTemplate = "%cFailures(s): {0}%c";
        private readonly string[] failuresColors = new string[] { "red", "default" };

        public TaskStatusViewController(
            ITaskStatus applicationTaskStatus,
            IFormattingController bytesFormattingController,
            IFormattingController secondsFormattingController,
            IPresentationController<Tuple<string, string[]>> consolePresentationController)
        {
            this.applicationTaskStatus = applicationTaskStatus;

            this.bytesFormattingController = bytesFormattingController;
            this.secondsFormattingController = secondsFormattingController;

            this.consolePresentationController = consolePresentationController;

            taskStatusQueue = new Queue<ITaskStatus>();
        }

        public void CreateView()
        {
            if ((DateTime.UtcNow - lastReportedTimestamp).TotalMilliseconds < throttleMilliseconds) return;

            var viewModels = new List<TaskStatusViewModel>();

            taskStatusQueue.Clear();
            taskStatusQueue.Enqueue(applicationTaskStatus);

            while (taskStatusQueue.Count > 0)
            {
                var currentTaskStatus = taskStatusQueue.Dequeue();
                var taskStatusViewModel = GetViewModel(currentTaskStatus);

                if (taskStatusViewModel != null)
                    viewModels.Add(taskStatusViewModel);

                if (currentTaskStatus.Children == null) continue;

                foreach (var childTaskStatus in currentTaskStatus.Children)
                    taskStatusQueue.Enqueue(childTaskStatus);
            }

            consolePresentationController.Present(GetTuples(viewModels));

            lastReportedTimestamp = DateTime.UtcNow;
        }

        private IEnumerable<Tuple<string, string[]>> GetTuples(IEnumerable<TaskStatusViewModel> taskStatusViewModels)
        {
            foreach (var taskStatusViewModel in taskStatusViewModels)
            {
                yield return new Tuple<string, string[]>(
                    taskStatusViewModel.Text,
                    taskStatusViewModel.Colors);
            }
        }

        private TaskStatusViewModel GetViewModel(ITaskStatus taskStatus)
        {
            var taskStatusStringBuilder = new StringBuilder();
            var taskStatusColors = new List<string>();

            if (taskStatus.Complete) return null;

            // list completed children
            var completedChildTasksCount = 0;
            if (taskStatus.Children != null)
                foreach (var task in taskStatus.Children)
                    if (task.Complete) ++completedChildTasksCount;

            AppendTitle(
                taskStatusStringBuilder, 
                taskStatusColors, 
                taskStatus.Title);

            if (taskStatus.Progress != null)
            {
                AppendProgressTarget(
                    taskStatusStringBuilder,
                    taskStatusColors,
                    taskStatus.Progress.Target);

                AppendProgressCompletion(
                    taskStatusStringBuilder,
                    taskStatusColors,
                    taskStatus.Progress.Current,
                    taskStatus.Progress.Total,
                    taskStatus.Progress.Unit);

                // show ETA only for bytes, downloads, validation
                if (taskStatus.Progress.Unit == DataUnits.Bytes)
                    AppendProgressETA(
                        taskStatusStringBuilder,
                        taskStatusColors,
                        taskStatus.Started,
                        taskStatus.Progress.Current,
                        taskStatus.Progress.Total,
                        taskStatus.Progress.Unit);
            }

            if (completedChildTasksCount > 0)
                AppendCompletedChildTasks(
                    taskStatusStringBuilder,
                    taskStatusColors,
                    completedChildTasksCount);

            if (taskStatus.Warnings != null &&
                taskStatus.Warnings.Count > 0)
                AppendWarnings(
                    taskStatusStringBuilder,
                    taskStatusColors,
                    taskStatus.Warnings.Count);

            if (taskStatus.Failures != null &&
                taskStatus.Failures.Count > 0)
                AppendFailures(
                    taskStatusStringBuilder,
                    taskStatusColors,
                    taskStatus.Failures.Count);

            var taskStatusViewModel = new TaskStatusViewModel()
            {
                Text = taskStatusStringBuilder.ToString(),
                Colors = taskStatusColors.ToArray()
            };

            return taskStatusViewModel;
        }

        private void GetViewModelComponentWithSuffix(StringBuilder stringBuilder, string template, params string[] data)
        {
            stringBuilder.AppendFormat(template, data);
            stringBuilder.Append(suffix);
        }

        private void AppendTitle(StringBuilder stringBuilder, List<string> colors, string title)
        {
            GetViewModelComponentWithSuffix(stringBuilder, title, new string[0]);
        }

        private void AppendProgressTarget(StringBuilder stringBuilder, List<string> colors, string target)
        {
            GetViewModelComponentWithSuffix(
                stringBuilder,
                progressTargetTemplate,
                target);

            colors.AddRange(progressTargetColors);
        }

        private void AppendProgressCompletion(StringBuilder stringBuilder, List<string> colors, long current, long total, string unit)
        {
            var percentage = (double)current / total;

            var currentFormatted = current.ToString();
            var totalFormatted = total.ToString();

            if (unit == DataUnits.Bytes)
            {
                currentFormatted = bytesFormattingController.Format(current);
                totalFormatted = bytesFormattingController.Format(total);
            }

            stringBuilder.AppendFormat(
                progressTemplate,
                percentage,
                currentFormatted,
                totalFormatted);
            stringBuilder.Append(suffix);
        }

        private void AppendProgressETA(StringBuilder stringBuilder, List<string> colors, DateTime started, long current, long total, string unit)
        {
            var elapsed = DateTime.UtcNow - started;
            var unitsPerSecond = current / elapsed.TotalSeconds;
            var remainingSeconds = (total - current) / unitsPerSecond;

            GetViewModelComponentWithSuffix(
                stringBuilder,
                progressETATemplate,
                secondsFormattingController.Format((long)remainingSeconds));

            colors.AddRange(progressETAColors);
        }

        private void AppendCompletedChildTasks(StringBuilder stringBuilder, List<string> colors, int completedChildTasksCount)
        {
            GetViewModelComponentWithSuffix(
                stringBuilder,
                childTasksTemplate,
                completedChildTasksCount.ToString());

            colors.AddRange(childTasksColors);
        }

        private void AppendWarnings(StringBuilder stringBuilder, List<string> colors, int warningsCount)
        {
            GetViewModelComponentWithSuffix(
                stringBuilder,
                warningsTemplate,
                warningsCount.ToString());
            colors.AddRange(warningsColors);
        }

        private void AppendFailures(StringBuilder stringBuilder, List<string> colors, int failuresCount)
        {
            GetViewModelComponentWithSuffix(
                stringBuilder,
                failuresTemplate,
                failuresCount.ToString());
            colors.AddRange(failuresColors);
        }
    }
}
