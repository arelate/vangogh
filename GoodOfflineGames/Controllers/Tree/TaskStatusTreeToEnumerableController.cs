using System;
using System.Linq;
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

            var taskStatusQueue = new List<ITaskStatus>();

            taskStatusQueue.Insert(0, taskStatus);

            while (taskStatusQueue.Any())
            {
                var currentTaskStatus = taskStatusQueue[0];
                taskStatusQueue.RemoveAt(0);

                if (currentTaskStatus == null) continue;

                yield return currentTaskStatus;

                if (currentTaskStatus.Children == null) continue;

                taskStatusQueue.InsertRange(0, currentTaskStatus.Children);
            }
        }
    }
}
