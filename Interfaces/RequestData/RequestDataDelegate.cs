namespace Interfaces.RequestData
{
    public interface IRequestDataDelegate<T>
    {
        T RequestData(string message);
    }

    public interface IRequestPrivateDataDelegate<T>
    {
        T RequestPrivateData(string message);
    }

    public interface IRequestDataController<T>:
        IRequestDataDelegate<T>,
        IRequestPrivateDataDelegate<T>
    {
        // ...
    }
}
