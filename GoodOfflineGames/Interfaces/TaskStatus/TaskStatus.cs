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

    public interface IChildrenProperty
    {
        IList<ITaskStatus> Children { get; set; }
    }

    public interface IStartedProperty
    {
        DateTime Started { get; set; }
    }

    public interface ICompletedProperty
    {
        DateTime Completed { get; set; }
    }

    public interface IWarningsProperty
    {
        IList<string> Warnings { get; set; }
    }

    public interface IFailuresProperty
    {
        IList<string> Failures { get; set; }
    }

    public interface IInformationProperty
    {
        IList<string> Information { get; set; }
    }

    public interface IUnitProperty
    {
        string Unit { get; set; }
    }

    public interface ITargetProperty
    {
        string Target { get; set; }
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
        ITargetProperty,
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
        IChildrenProperty,
        IProgressProperty,
        IStartedProperty,
        ICompletedProperty,
        IWarningsProperty,
        IFailuresProperty,
        IInformationProperty
    {
        // ...
    }
}
