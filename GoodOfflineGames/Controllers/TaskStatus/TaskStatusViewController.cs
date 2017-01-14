using System;
using System.Text;
using System.Collections.Generic;

using Interfaces.Console;
using Interfaces.TaskStatus;
using Interfaces.Formatting;
using Interfaces.Presentation;

namespace Controllers.TaskStatus
{
    public class TaskStatusViewController : ITaskStatusViewController
    {
        private ITaskStatus taskStatus;
        private IPresentationController<string> consolePresentationController;
        private IFormattingController bytesFormattingController;
        private IFormattingController secondsFormattingController;

        private Queue<ITaskStatus> taskStatusQueue;
        private Queue<int> taskStatusLevelsQueue;

        private const int throttleMilliseconds = 100;
        private DateTime lastReportedTimestamp = DateTime.MinValue;

        private const string taskStatusProgressViewTemplate = ": {0}: {1:P1}, {2} of {3}";
        private const string taskStatusProgressETAViewTemplate = ", ETA: {0}";
        private const string warningsTemplate = ". Warning(s): {0}.";
        private const string failuresTemplate = ". Failures(s): {0}.";
        private const int showETAThreshold = 2;
        private const char childPrefix = '>';

        public TaskStatusViewController(
            ITaskStatus taskStatus,
            IFormattingController bytesFormattingController,
            IFormattingController secondsFormattingController,
            IPresentationController<string> consolePresentationController)
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

            var views = new List<string>();

            taskStatusQueue.Clear();
            taskStatusLevelsQueue.Clear();
            taskStatusQueue.Enqueue(taskStatus);
            taskStatusLevelsQueue.Enqueue(0);

            while (taskStatusQueue.Count > 0)
            {
                var currentTaskStatus = taskStatusQueue.Dequeue();
                var currentLevel = taskStatusLevelsQueue.Dequeue();
                var taskStatusView = GetTaskStatusView(currentTaskStatus);

                if (!string.IsNullOrEmpty(taskStatusView))
                {
                    if (currentLevel > 0)
                    {
                        taskStatusView = " " + taskStatusView;
                        taskStatusView = taskStatusView.PadLeft(taskStatusView.Length + currentLevel, childPrefix);
                    }

                    views.Add(taskStatusView);
                }

                if (currentTaskStatus.ChildTasks != null)
                    foreach (var childTaskStatus in currentTaskStatus.ChildTasks)
                    {
                        taskStatusQueue.Enqueue(childTaskStatus);
                        taskStatusLevelsQueue.Enqueue(currentLevel + 1);
                    }
            }

            consolePresentationController.Present(views);

            lastReportedTimestamp = DateTime.UtcNow;
        }

        private string GetTaskStatusView(ITaskStatus taskStatus)
        {
            var taskStatusView = new StringBuilder();

            if (taskStatus.Complete) return taskStatusView.ToString();

            // list completed children
            var completedChildTasksCount = 0;
            if (taskStatus.ChildTasks != null)
                foreach (var task in taskStatus.ChildTasks)
                    if (task.Complete)
                        completedChildTasksCount++;

            taskStatusView.Append(taskStatus.Title);

            if (taskStatus.Progress != null)
            {
                var percentage = (double)taskStatus.Progress.Current / taskStatus.Progress.Total;

                var currentFormatted = taskStatus.Progress.Current.ToString();
                var totalFormatted = taskStatus.Progress.Total.ToString();

                if (taskStatus.Progress.Unit == "byte(s)")
                {
                    currentFormatted = bytesFormattingController.Format(taskStatus.Progress.Current);
                    totalFormatted = bytesFormattingController.Format(taskStatus.Progress.Total);
                }

                taskStatusView.Append(string.Format(
                    taskStatusProgressViewTemplate,
                    taskStatus.Progress.Target,
                    percentage,
                    currentFormatted,
                    totalFormatted));

                // show ETA only for bytes, downloads, validation
                if (taskStatus.Progress.Unit == "byte(s)")
                {
                    var elapsed = DateTime.UtcNow - taskStatus.Started;
                    var unitsPerSecond = taskStatus.Progress.Current / elapsed.TotalSeconds;
                    var remainingSeconds = (taskStatus.Progress.Total - taskStatus.Progress.Current) / unitsPerSecond;

                    if (remainingSeconds > showETAThreshold)
                    {
                        taskStatusView.Append(string.Format(
                            taskStatusProgressETAViewTemplate,
                            secondsFormattingController.Format(
                                (long)remainingSeconds)));
                    }
                }
            }

            if (completedChildTasksCount > 0)
                taskStatusView.Append(string.Format(". {0} child task(s) complete.", completedChildTasksCount));

            if (taskStatus.Warnings != null &&
                taskStatus.Warnings.Count > 0)
                taskStatusView.Append(string.Format(warningsTemplate, taskStatus.Warnings.Count));

            if (taskStatus.Failures != null &&
                taskStatus.Failures.Count > 0)
                taskStatusView.Append(string.Format(failuresTemplate, taskStatus.Failures.Count));

            return taskStatusView.ToString();
        }
    }
}
