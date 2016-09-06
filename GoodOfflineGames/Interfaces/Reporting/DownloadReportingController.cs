namespace Interfaces.Reporting
{
    public interface IDownloadReportingController:
        IStartTaskDelegate,
        ICompleTaskDelegate,
        IReportProgressDelegate
    {
        // ...
    }
}
