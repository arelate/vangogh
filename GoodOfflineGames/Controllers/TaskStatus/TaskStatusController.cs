using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.TaskStatus;

using Models.TaskStatus;

namespace Controllers.TaskStatus
{
    public class TaskStatusController : ITaskStatusController
    {
        private ITaskStatusViewController taskStatusViewController;

        public TaskStatusController(
            ITaskStatusViewController taskStatusViewController)
        {
            this.taskStatusViewController = taskStatusViewController;
        }

        private void ReleaseAssertTaskStatusNotNull(ITaskStatus taskStatus)
        {
            if (taskStatus == null)
                throw new ArgumentNullException("Current task status cannot be null");
        }

        public ITaskStatus Create(ITaskStatus taskStatus, string title)
        {
            ReleaseAssertTaskStatusNotNull(taskStatus);

            if (taskStatus.ChildTasks == null)
                taskStatus.ChildTasks = new List<ITaskStatus>();

            var childTaskStatus = new Models.TaskStatus.TaskStatus() {
                Title = title,
                Started = DateTime.UtcNow
            };
            taskStatus.ChildTasks.Add(childTaskStatus);

            taskStatusViewController.CreateView();

            return childTaskStatus;
        }

        public void Complete(ITaskStatus taskStatus)
        {
            ReleaseAssertTaskStatusNotNull(taskStatus);

            taskStatus.Complete = true;
            taskStatus.Completed = DateTime.UtcNow;

            taskStatusViewController.CreateView();
        }

        public void UpdateProgress(ITaskStatus taskStatus, long current, long total, string target, string unit = "")
        {
            ReleaseAssertTaskStatusNotNull(taskStatus);

            if (taskStatus.Progress == null)
                taskStatus.Progress = new TaskProgress();

            taskStatus.Progress.Target = target;
            taskStatus.Progress.Current = current;
            taskStatus.Progress.Total = total;
            taskStatus.Progress.Unit = unit;

            taskStatusViewController.CreateView();
        }

        public void Fail(ITaskStatus taskStatus, string failureMessage, params object[] data)
        {
            ReleaseAssertTaskStatusNotNull(taskStatus);

            if (taskStatus.Failures == null)
                taskStatus.Failures = new List<string>();

            taskStatus.Failures.Add(string.Format(failureMessage, data));
        }

        public void Warn(ITaskStatus taskStatus, string warningMessage, params object[] data)
        {
            ReleaseAssertTaskStatusNotNull(taskStatus);

            if (taskStatus.Warnings == null)
                taskStatus.Warnings = new List<string>();

            taskStatus.Warnings.Add(string.Format(warningMessage, data));
        }
    }
}
