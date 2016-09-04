namespace Interfaces.UpdateDependencies
{
    public interface IGetUpdateUriDelegate
    {
        string GetUpdateUri<T>(T item);
    }

    public interface IUpdateUriController:
        IGetUpdateUriDelegate
    {
        // ...
    }
}
