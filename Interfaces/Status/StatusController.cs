namespace Interfaces.Status
{
    public interface ICreateDelegate
    {
        IStatus Create(IStatus status, string title);
    }

    public interface ICompleteDelegate
    {
        void Complete(IStatus status);
    }

    public interface IUpdateProgressDelegate
    {
        void UpdateProgress(IStatus status, long current, long total, string target, string unit = "");
    }

    public interface IFailDelegate
    {
        void Fail(IStatus status, string failureMessage);
    }

    public interface IWarnDelegate
    {
        void Warn(IStatus status, string warningMessage);
    }

    public interface IInformDelegate
    {
        void Inform(IStatus status, string informationMessage);
    }

    public interface IAddSummaryResultsDelegate
    {
        void AddSummaryResults(IStatus status, params string[] summaryResults);
    }

    public delegate void StatusChangedNotificationDelegate();

    public interface IStatusChangedNotifictionEvent {
        event StatusChangedNotificationDelegate StatusChangedNotification;
    }

    public interface IStatusController:
        ICreateDelegate,
        ICompleteDelegate,
        IUpdateProgressDelegate,
        IFailDelegate,
        IWarnDelegate,
        IInformDelegate,
        IAddSummaryResultsDelegate,
        IStatusChangedNotifictionEvent
    {
        // ...
    }
}
