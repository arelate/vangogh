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

        public void UpdateProgress(ITaskStatus taskStatus, long current, long total, string unit = "")
        {
            ReleaseAssertTaskStatusNotNull(taskStatus);

            if (taskStatus.Progress == null)
                taskStatus.Progress = new TaskProgress();

            taskStatus.Progress.Current = current;
            taskStatus.Progress.Total = total;
            taskStatus.Progress.Unit = unit;

            taskStatusViewController.CreateView();
        }
    }
}
