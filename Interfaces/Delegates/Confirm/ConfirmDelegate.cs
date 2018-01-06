namespace Interfaces.Delegates.Confirm
{
    public interface IConfirmDelegate<T>
    {
        bool Confirm(T data);
    }
}
