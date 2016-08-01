namespace Interfaces.Network
{
    public interface IReportCurrentMaxValuesDelegate<T>
    {
        void Report(T currentValue, T maxValue);
    }

    public interface IInitializeDelegate
    {
        void Initialize();
    }

    public interface IThrottleMillisecondsDelegate
    {
        int ThrottleMilliseconds { get; set; }
    }

    public interface IDownloadProgressReportingController :
        IReportCurrentMaxValuesDelegate<long>,
        IInitializeDelegate,
        IThrottleMillisecondsDelegate
    {
        // ...
    }
}
