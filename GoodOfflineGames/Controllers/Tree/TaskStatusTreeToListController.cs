using System;
using System.Collections.Generic;

using Interfaces.Tree;
using Interfaces.TaskStatus;

namespace Controllers.Tree
{
    public class TaskStatusTreeToListController : ITreeToListController<ITaskStatus>
    {
        public IList<ITaskStatus> ToList(ITaskStatus taskStatus)
        {
            var taskStatusList = new List<ITaskStatus>();

            if (taskStatus == null) return taskStatusList;

            var taskStatusQueue = new Queue<ITaskStatus>();
            taskStatusQueue.Enqueue(taskStatus);

            while (taskStatusQueue.Count > 0)
            {
                var currentTaskStatus = taskStatusQueue.Dequeue();
                if (currentTaskStatus == null) continue;

                taskStatusList.Add(currentTaskStatus);

                if (currentTaskStatus.Children == null) continue;

                foreach (var child in currentTaskStatus.Children)
                    taskStatusQueue.Enqueue(child);
            }

            return taskStatusList;
        }
    }
}
