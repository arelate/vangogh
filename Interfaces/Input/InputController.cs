namespace Interfaces.Input
{
    public interface IRequestInputDelegate<T>
    {
        T RequestInput(string message);
    }

    public interface IRequestPrivateInputDelegate<T>
    {
        T RequestPrivateInput(string message);
    }

    public interface IInputController<T>:
        IRequestInputDelegate<T>,
        IRequestPrivateInputDelegate<T>
    {
        // ...
    }
}
