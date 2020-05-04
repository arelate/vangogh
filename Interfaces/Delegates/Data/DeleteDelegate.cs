namespace Interfaces.Delegates.Data
{
    public interface IDeleteDelegate<T>
    {
        void Delete(T uri);
    }
}