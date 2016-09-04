namespace Interfaces.UpdateDependencies
{
    public interface ISkipUpdateDelegate
    {
        bool SkipUpdate<T>(T item);
    }

    public interface ISkipUpdateController:
        ISkipUpdateDelegate
    {
        // ...
    }
}
