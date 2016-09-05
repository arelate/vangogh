namespace Interfaces.Destination
{
    public interface IDestinationDelegate
    {
        string GetDestination(string source, string destination);
    }

    public interface IDestinationController:
        IDestinationDelegate
    {
        // ...
    }
}
