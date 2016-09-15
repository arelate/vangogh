namespace Interfaces.SourceDestination
{
    public interface IGetSourceDestinationDelegate
    {
        string GetSourceDestination(string source, string destination);
    }

    public interface ISourceDestinationController:
        IGetSourceDestinationDelegate
    {
        // ...
    }
}
