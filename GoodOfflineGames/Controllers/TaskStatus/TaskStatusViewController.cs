using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Console;
using Interfaces.TaskStatus;

using Interfaces.Serialization;

namespace Controllers.TaskStatus
{
    public class TaskStatusViewController: ITaskStatusViewController
    {
        private ITaskStatus taskStatus;
        private IConsoleController consoleController;

        private Queue<ITaskStatus> taskStatusQueue;

        public TaskStatusViewController(
            ITaskStatus taskStatus,
            IConsoleController consoleController)
        {
            this.taskStatus = taskStatus;
            this.consoleController = consoleController;

            taskStatusQueue = new Queue<ITaskStatus>();
        }

        public void CreateView()
        {
            consoleController.Clear();

            taskStatusQueue.Clear();
            taskStatusQueue.Enqueue(taskStatus);

            while (taskStatusQueue.Count > 0)
            {
                var currentTaskStatus = taskStatusQueue.Dequeue();
                consoleController.WriteLine(GetTaskStatusView(currentTaskStatus));

                if (currentTaskStatus.ChildTasks != null)
                    foreach (var childTaskStatus in currentTaskStatus.ChildTasks)
                        taskStatusQueue.Enqueue(childTaskStatus);
            }

            //System.Threading.Thread.Sleep(1000);
        }

        private string GetTaskStatusView(ITaskStatus taskStatus)
        {
            var taskStatusView = string.Empty;

            taskStatusView = taskStatus.Title;
            if (taskStatus.Complete)
                taskStatusView += " [COMPLETE]";
            else
            {
                if (taskStatus.Progress != null)
                {
                    taskStatusView += ": " + taskStatus.Progress.Current + "/" + taskStatus.Progress.Total + " " + taskStatus.Progress.Unit + ": " + taskStatus.Progress.Target;
                }
            }

            return taskStatusView;
        }
    }
}
