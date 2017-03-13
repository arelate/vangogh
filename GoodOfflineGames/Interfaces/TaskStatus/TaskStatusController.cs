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

    public interface IUpdateProgressDelegate
    {
        void UpdateProgress(ITaskStatus taskStatus, long current, long total, string target, string unit = "");
    }

    public interface IFailDelegate
    {
        void Fail(ITaskStatus taskStatus, string failureMessage);
    }

    public interface IWarnDelegate
    {
        void Warn(ITaskStatus taskStatus, string warningMessage);
    }

    public interface ITaskStatusController:
        ICreateDelegate,
        ICompleteDelegate,
        IUpdateProgressDelegate,
        IFailDelegate,
        IWarnDelegate
    {
        // ...
    }
}
