using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.Tree;
using Interfaces.Status;

namespace Controllers.Tree
{
    public class StatusTreeToEnumerableController : ITreeToEnumerableController<IStatus>
    {
        public IEnumerable<IStatus> ToEnumerable(IStatus status)
        {
            if (status == null) yield break;

            var statusQueue = new List<IStatus>();

            statusQueue.Insert(0, status);

            while (statusQueue.Any())
            {
                var currentstatus = statusQueue[0];
                statusQueue.RemoveAt(0);

                if (currentstatus == null) continue;

                yield return currentstatus;

                if (currentstatus.Children == null) continue;

                statusQueue.InsertRange(0, currentstatus.Children);
            }
        }
    }
}
