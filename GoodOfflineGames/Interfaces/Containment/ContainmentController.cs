namespace Interfaces.Containment
{
    public interface IContainedDelegate<T>
    {
        bool Contained(T data);
    }

    public interface IContainmentController<T>:
        IContainedDelegate<T>
    {
        // ...
    }

}
