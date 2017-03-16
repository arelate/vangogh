using System;
using System.Collections.Generic;

using Interfaces.Tree;
using Interfaces.TaskStatus;

namespace Controllers.Tree
{
    public class TaskStatusTreeToEnumerableController : ITreeToEnumerableController<ITaskStatus>
    {
        public IEnumerable<ITaskStatus> ToEnumerable(ITaskStatus taskStatus)
        {
            if (taskStatus == null) yield break;

            var taskStatusQueue = new Queue<ITaskStatus>();
            taskStatusQueue.Enqueue(taskStatus);

            while (taskStatusQueue.Count > 0)
            {
                var currentTaskStatus = taskStatusQueue.Dequeue();
                if (currentTaskStatus == null) continue;

                yield return currentTaskStatus;

                if (currentTaskStatus.Children == null) continue;

                foreach (var child in currentTaskStatus.Children)
                    taskStatusQueue.Enqueue(child);
            }
        }
    }
}
