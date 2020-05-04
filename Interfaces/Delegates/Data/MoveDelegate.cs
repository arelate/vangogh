namespace Interfaces.Delegates.Data
{
    public interface IMoveDelegate<in T>
    {
        void Move(T from, T to);
    }
}