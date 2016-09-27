namespace Interfaces.Settings
{
    public interface IAfterDownload
    {
        bool AfterDownload { get; set; }
    }

    public interface IValidationProperties:
        IAfterDownload
    {
        // ...
    }
}
