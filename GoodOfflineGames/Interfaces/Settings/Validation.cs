namespace Interfaces.Settings
{
    public interface IUpdatedProperty
    {
        bool Updated { get; set; }
    }

    public interface IValidationProperties:
        IUpdatedProperty
    {
        // ...
    }
}
