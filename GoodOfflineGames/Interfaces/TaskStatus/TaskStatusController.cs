using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.TaskStatus
{
    public interface ICreateDelegate
    {
        ITaskStatus Create(ITaskStatus taskStatus, string title);
    }

    public interface ICompleteDelegate
    {
        void Complete(ITaskStatus taskStatus);
    }

    public interface IUpdateProgress
    {
        void UpdateProgress(ITaskStatus taskStatus, long current, long total, string unit = "");
    }

    public interface ITaskStatusController:
        ICreateDelegate,
        ICompleteDelegate,
        IUpdateProgress
    {
        // ...
    }
}
