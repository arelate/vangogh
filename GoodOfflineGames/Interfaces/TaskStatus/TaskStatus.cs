using System;
using System.Collections.Generic;

namespace Interfaces.TaskStatus
{
    public interface ICompleteProperty
    {
        bool Complete { get; set; }
    }

    public interface ITitleProperty
    {
        string Title { get; set; }
    }

    public interface IChildTasksProperty
    {
        IList<ITaskStatus> ChildTasks { get; set; }
    }

    public interface IStartedProperty
    {
        DateTime Started { get; set; }
    }

    public interface ICompletedProperty
    {
        DateTime Completed { get; set; }
    }

    public interface IUnitProperty
    {
        string Unit { get; set; }
    }

    public interface ICurrentProperty
    {
        long Current { get; set; }
    }

    public interface ITotalProperty
    {
        long Total { get; set; }
    }

    public interface ITaskProgress:
        ICurrentProperty,
        ITotalProperty,
        IUnitProperty
    {
        // ...
    }

    public interface IProgressProperty
    {
        ITaskProgress Progress { get; set; }
    }

    public interface ITaskStatus :
        ICompleteProperty,
        ITitleProperty,
        IChildTasksProperty,
        IProgressProperty,
        IStartedProperty,
        ICompletedProperty
    {
        // ...
    }
}
