namespace Interfaces.UpdateDependencies
{
    public interface IGetRequiredUpdatesDelegate
    {
        long[] GetRequiredUpdates();
    }

    public interface IRequiredUpdatesController:
        IGetRequiredUpdatesDelegate
    {
        // ...
    }
}
