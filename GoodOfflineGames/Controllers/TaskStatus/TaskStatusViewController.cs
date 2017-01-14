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
        private ITaskStatus taskStatus;
        private IPresentationController<Tuple<string, string[]>> consolePresentationController;
        private IFormattingController bytesFormattingController;
        private IFormattingController secondsFormattingController;

        private Queue<ITaskStatus> taskStatusQueue;
        private Queue<int> taskStatusLevelsQueue;

        private const int throttleMilliseconds = 100;
        private DateTime lastReportedTimestamp = DateTime.MinValue;

        private const string progressTemplate = ": %c{0}%c: {1:P1}, {2} of {3}";
        private readonly string[] progressColors = new string[] { "white", "default" };
        private const string progressETATemplate = ", ETA: %c{0}%c";
        private readonly string[] progressETAColors = new string[] { "white", "default" };

        private const string childTasksTemplate = ". %c{0}%c child task(s) complete.";
        private readonly string[] childTasksColors = new string[] { "white", "default" };

        private const string warningsTemplate = ". %cWarning(s): {0}%c.";
        private readonly string[] warningsColors = new string[] { "red", "default" };
        private const string failuresTemplate = ". %cFailures(s): {0}%c.";
        private readonly string[] failuresColors = new string[] { "yellow", "default" };

        private const int showETAThreshold = 2;
        private const char childPrefix = '>';
        private const char childPrefixSeparator = ' ';

        public TaskStatusViewController(
            ITaskStatus taskStatus,
            IFormattingController bytesFormattingController,
            IFormattingController secondsFormattingController,
            IPresentationController<Tuple<string, string[]>> consolePresentationController)
        {
            this.taskStatus = taskStatus;

            this.bytesFormattingController = bytesFormattingController;
            this.secondsFormattingController = secondsFormattingController;

            this.consolePresentationController = consolePresentationController;

            taskStatusQueue = new Queue<ITaskStatus>();
            taskStatusLevelsQueue = new Queue<int>();
        }

        public void CreateView()
        {
            if ((DateTime.UtcNow - lastReportedTimestamp).TotalMilliseconds < throttleMilliseconds) return;

            var viewModels = new List<TaskStatusViewModel>();

            taskStatusQueue.Clear();
            taskStatusLevelsQueue.Clear();
            taskStatusQueue.Enqueue(taskStatus);
            taskStatusLevelsQueue.Enqueue(0);

            while (taskStatusQueue.Count > 0)
            {
                var currentTaskStatus = taskStatusQueue.Dequeue();
                var currentLevel = taskStatusLevelsQueue.Dequeue();
                var taskStatusViewModel = GetViewModel(currentTaskStatus);

                if (taskStatusViewModel != null)
                {
                    if (currentLevel > 0)
                        taskStatusViewModel.Text =
                            string.Empty.PadLeft(currentLevel, childPrefix) +
                            childPrefixSeparator +
                            taskStatusViewModel.Text;

                    viewModels.Add(taskStatusViewModel);
                }

                if (currentTaskStatus.ChildTasks != null)
                    foreach (var childTaskStatus in currentTaskStatus.ChildTasks)
                    {
                        taskStatusQueue.Enqueue(childTaskStatus);
                        taskStatusLevelsQueue.Enqueue(currentLevel + 1);
                    }
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
            var taskStatusText = new StringBuilder();
            var colors = new List<string>();

            if (taskStatus.Complete) return null;

            // list completed children
            var completedChildTasksCount = 0;
            if (taskStatus.ChildTasks != null)
                foreach (var task in taskStatus.ChildTasks)
                    if (task.Complete)
                        completedChildTasksCount++;

            taskStatusText.Append(taskStatus.Title);

            if (taskStatus.Progress != null)
            {
                var percentage = (double)taskStatus.Progress.Current / taskStatus.Progress.Total;

                var currentFormatted = taskStatus.Progress.Current.ToString();
                var totalFormatted = taskStatus.Progress.Total.ToString();

                if (taskStatus.Progress.Unit == DataUnits.Bytes)
                {
                    currentFormatted = bytesFormattingController.Format(taskStatus.Progress.Current);
                    totalFormatted = bytesFormattingController.Format(taskStatus.Progress.Total);
                }

                taskStatusText.Append(string.Format(
                    progressTemplate,
                    taskStatus.Progress.Target,
                    percentage,
                    currentFormatted,
                    totalFormatted));
                colors.AddRange(progressColors);

                // show ETA only for bytes, downloads, validation
                if (taskStatus.Progress.Unit == DataUnits.Bytes)
                {
                    var elapsed = DateTime.UtcNow - taskStatus.Started;
                    var unitsPerSecond = taskStatus.Progress.Current / elapsed.TotalSeconds;
                    var remainingSeconds = (taskStatus.Progress.Total - taskStatus.Progress.Current) / unitsPerSecond;

                    if (remainingSeconds > showETAThreshold)
                    {
                        taskStatusText.Append(string.Format(
                            progressETATemplate,
                            secondsFormattingController.Format(
                                (long)remainingSeconds)));
                        colors.AddRange(progressETAColors);
                    }
                }
            }

            if (completedChildTasksCount > 0)
            {
                taskStatusText.Append(string.Format(childTasksTemplate, completedChildTasksCount));
                colors.AddRange(childTasksColors);
            }

            if (taskStatus.Warnings != null &&
                taskStatus.Warnings.Count > 0)
            {
                taskStatusText.Append(string.Format(warningsTemplate, taskStatus.Warnings.Count));
                colors.AddRange(warningsColors);
            }

            if (taskStatus.Failures != null &&
                taskStatus.Failures.Count > 0)
            {
                taskStatusText.Append(string.Format(failuresTemplate, taskStatus.Failures.Count));
                colors.AddRange(failuresColors);
            }

            var taskStatusViewModel = new TaskStatusViewModel()
            {
                Text = taskStatusText.ToString(),
                Colors = colors.ToArray()
            };

            return taskStatusViewModel;
        }
    }
}
