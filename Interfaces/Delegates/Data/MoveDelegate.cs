namespace Interfaces.Delegates.Data
{
    public interface IMoveDelegate<T>
    {
        void Move(T from, T to);
    }
}