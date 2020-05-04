namespace Interfaces.Delegates.Confirmations
{
    public interface IConfirmDelegate<in T>
    {
        bool Confirm(T data);
    }
}