using System;
using System.Collections.Generic;

using Interfaces.Console;
using Interfaces.TaskStatus;
using Interfaces.Formatting;

namespace Controllers.TaskStatus
{
    public class TaskStatusViewController : ITaskStatusViewController
    {
        private ITaskStatus taskStatus;
        private IConsoleController consoleController;
        private IFormattingController bytesFormattingController;
        private IFormattingController secondsFormattingController;

        private Queue<ITaskStatus> taskStatusQueue;

        private const string taskStatusProgressViewTemplate = ": {0}: {1:P1}, {2} of {3}";
        private const string taskStatusProgressETAViewTemplate = ", ETA: {0}";
        private const int throttleMilliseconds = 1000;
        private DateTime lastReportedTimestamp = DateTime.MinValue;

        public TaskStatusViewController(
            ITaskStatus taskStatus,
            IFormattingController bytesFormattingController,
            IFormattingController secondsFormattingController,
            IConsoleController consoleController)
        {
            this.taskStatus = taskStatus;
            this.bytesFormattingController = bytesFormattingController;
            this.secondsFormattingController = secondsFormattingController;
            this.consoleController = consoleController;

            taskStatusQueue = new Queue<ITaskStatus>();
        }

        public void CreateView()
        {
            if ((DateTime.Now - lastReportedTimestamp).TotalMilliseconds < throttleMilliseconds) return;

            consoleController.Clear();

            taskStatusQueue.Clear();
            taskStatusQueue.Enqueue(taskStatus);

            while (taskStatusQueue.Count > 0)
            {
                var currentTaskStatus = taskStatusQueue.Dequeue();
                var taskStatusView = GetTaskStatusView(currentTaskStatus);
                if (!string.IsNullOrEmpty(taskStatusView))
                    consoleController.WriteLine(taskStatusView);

                if (currentTaskStatus.ChildTasks != null)
                    foreach (var childTaskStatus in currentTaskStatus.ChildTasks)
                        taskStatusQueue.Enqueue(childTaskStatus);
            }

            lastReportedTimestamp = DateTime.Now;
        }

        private string GetTaskStatusView(ITaskStatus taskStatus)
        {
            var taskStatusView = string.Empty;

            if (taskStatus.Complete) return taskStatusView;

            // list completed children
            var completedChildTasksCount = 0;
            if (taskStatus.ChildTasks != null)
                foreach (var task in taskStatus.ChildTasks)
                    if (task.Complete)
                        completedChildTasksCount++;

            taskStatusView += taskStatus.Title;

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

                taskStatusView += string.Format(
                    taskStatusProgressViewTemplate,
                    taskStatus.Progress.Target,
                    percentage,
                    currentFormatted,
                    totalFormatted);

                // units

                var elapsed = DateTime.UtcNow - taskStatus.Started;
                var unitsPerSecond = taskStatus.Progress.Current / elapsed.TotalSeconds;
                var remainingSeconds = (taskStatus.Progress.Total - taskStatus.Progress.Current) / unitsPerSecond;

                if (remainingSeconds > 5)
                {
                    taskStatusView += string.Format(
                        taskStatusProgressETAViewTemplate, 
                        secondsFormattingController.Format(
                            (long) remainingSeconds));
                }
            }

            if (completedChildTasksCount > 0)
                taskStatusView += string.Format(". {0} child task(s) complete.", completedChildTasksCount);

            return taskStatusView;
        }
    }
}
