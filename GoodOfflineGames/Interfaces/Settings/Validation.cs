namespace Interfaces.Settings
{
    public interface IDownloadProperty
    {
        bool Download { get; set; }
    }

    public interface IValidateUpdatedProperty
    {
        bool ValidateUpdated { get; set; }
    }

    public interface IValidationProperties:
        IDownloadProperty,
        IValidateUpdatedProperty
    {
        // ...
    }
}
