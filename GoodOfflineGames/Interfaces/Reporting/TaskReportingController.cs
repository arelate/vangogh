namespace Interfaces.Reporting
{
    public interface IAddTaskDelegate
    {
        void AddTask(string name);
    }

    public interface ICompleTaskDelegate
    {
        void CompleteTask();
    }

    public interface IReportProgressDelegate
    {
        void ReportProgress(long value, long maxValue);
    }

    public interface IReportFailureDelegate
    {
        void ReportFailure(string errorMessage);
    }

    public interface IReportWarningDelegate
    {
        void ReportWarning(string warningMessage);
    }

    public interface ITaskReportingController:
        IAddTaskDelegate,
        ICompleTaskDelegate,
        IReportProgressDelegate,
        IReportFailureDelegate,
        IReportWarningDelegate
    {
        // ...
    }
}
