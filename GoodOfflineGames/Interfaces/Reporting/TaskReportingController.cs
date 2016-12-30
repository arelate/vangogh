namespace Interfaces.Reporting
{
    public interface IStartTaskDelegate
    {
        void StartTask(string template, params object[] values);
    }

    public interface ICompleTaskDelegate
    {
        void CompleteTask();
    }

    public interface IReportProgressDelegate
    {
        void ReportProgress(long value, long? maxValue, LongToStringFormattingDelegate formattingDelegate = null);
    }

    public interface IReportFailureDelegate
    {
        void ReportFailure(string errorMessage);
    }

    public interface IReportWarningDelegate
    {
        void ReportWarning(string warningMessage);
    }

    public delegate string LongToStringFormattingDelegate(long value);

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
