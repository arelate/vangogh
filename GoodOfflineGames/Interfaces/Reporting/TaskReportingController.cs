namespace Interfaces.Reporting
{
    public interface IStartTaskDelegate
    {
        void StartTask(string name);
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
        IStartTaskDelegate,
        ICompleTaskDelegate,
        IReportProgressDelegate,
        IReportFailureDelegate,
        IReportWarningDelegate
    {
        // ...
    }
}
