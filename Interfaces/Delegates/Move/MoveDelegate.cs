namespace Interfaces.Delegates.Move
{
    public interface IMoveDelegate<T>
    {
        void Move(T from, T to);
    }
}