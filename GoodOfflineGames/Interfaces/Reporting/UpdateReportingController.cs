namespace Interfaces.Reporting
{
    public interface IReportUpdateDelegate
    {
        void ReportUpdate();
    }

    public interface IUpdateReportingController:
        IReportUpdateDelegate
    {
        // ...
    }
}
